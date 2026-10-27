
using System.Globalization;
using System.Net.Http.Json;
using System.Text.Json;
using StadsLens.Mobile.Models;
using StadsLens.Mobile.Constants;

namespace StadsLens.Mobile.Services;

public class RoutingService : IRoutingService
{
    private readonly HttpClient _httpClient;

    public RoutingService(HttpClient httpClient)
    {
        _httpClient = httpClient;

        _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd(
            "StadsLens-Mobile/1.0 (routeplanning)");
    }

    public async Task<GeneratedRoute> GenerateRouteAsync(
        double startLatitude,
        double startLongitude,
        double targetDistanceKm,
        IEnumerable<Landmark> landmarks,
        CancellationToken cancellationToken = default)
    {
        if (targetDistanceKm <= 0)
            throw new ArgumentException(
                "De gewenste afstand moet groter zijn dan nul.");

        var candidates = landmarks
            .Where(l =>
                l.Latitude >= -90 && l.Latitude <= 90 &&
                l.Longitude >= -180 && l.Longitude <= 180)
            .Select(l => new
            {
                Landmark = l,
                Distance = HaversineKm(
                    startLatitude,
                    startLongitude,
                    l.Latitude,
                    l.Longitude)
            })
            .OrderBy(x => x.Distance)
            .ToList();

        if (candidates.Count == 0)
            throw new InvalidOperationException(
                "Er zijn geen landmarks beschikbaar om een route te maken.");

        // Schat de afstand over wandelpaden.
        // De factor 1,35 is slechts een heuristische schatting.
        var selected = new List<Landmark>();
        var estimatedKm = 0d;
        var previousLat = startLatitude;
        var previousLon = startLongitude;

        foreach (var candidate in candidates)
        {
            var landmark = candidate.Landmark;

            var legKm = HaversineKm(
                previousLat,
                previousLon,
                landmark.Latitude,
                landmark.Longitude) * 1.35;

            var returnKm = HaversineKm(
                landmark.Latitude,
                landmark.Longitude,
                startLatitude,
                startLongitude) * 1.35;

            // Voeg het landmark alleen toe als dit de route
            // dichter bij de gewenste afstand brengt.
            var currentTotal = estimatedKm +
                HaversineKm(
                    previousLat,
                    previousLon,
                    startLatitude,
                    startLongitude) * 1.35;

            var proposedTotal = estimatedKm + legKm + returnKm;

            if (selected.Count > 0 &&
                Math.Abs(currentTotal - targetDistanceKm) <
                Math.Abs(proposedTotal - targetDistanceKm))
            {
                break;
            }

            selected.Add(landmark);
            estimatedKm += legKm;

            previousLat = landmark.Latitude;
            previousLon = landmark.Longitude;

            if (estimatedKm >= targetDistanceKm)
                break;
        }

        // OSRM verwacht longitude,latitude.
        var coordinates = new List<string>
        {
            FormatCoordinate(startLongitude, startLatitude)
        };

        coordinates.AddRange(selected.Select(l =>
            FormatCoordinate(l.Longitude, l.Latitude)));

        // Maak er een lus van: terug naar het vertrekpunt.
        coordinates.Add(FormatCoordinate(startLongitude, startLatitude));

        var url =
            "https://routing.openstreetmap.de/routed-foot/route/v1/driving/" +
            string.Join(";", coordinates) +
            "?overview=full&geometries=geojson&steps=false";

        using var response = await _httpClient.GetAsync(
    url, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            var body = await response.Content.ReadAsStringAsync(
                cancellationToken);

            throw new HttpRequestException(
                $"Router gaf HTTP {(int)response.StatusCode} " +
                $"({response.ReasonPhrase}). Antwoord: {body}");
        }

        var result = await response.Content.ReadFromJsonAsync<OsrmResponse>(
            cancellationToken: cancellationToken);

        var route = result?.Routes?.FirstOrDefault();

        if (route?.Geometry?.Coordinates is null)
            throw new InvalidOperationException(
                "De wandelroute kon niet worden berekend.");

        var mapHtml = CreateMapHtml(
            startLatitude,
            startLongitude,
            route.Geometry.Coordinates,
            selected);

        return new GeneratedRoute
        {
            DistanceKm = route.Distance / 1000d,
            DurationMinutes = route.Duration / 60d,
            MapHtml = mapHtml,
            Landmarks = selected
        };
    }

    private static string FormatCoordinate(double longitude, double latitude)
    {
        return string.Create(
            CultureInfo.InvariantCulture,
            $"{longitude},{latitude}");
    }

    private static double HaversineKm(
        double lat1, double lon1,
        double lat2, double lon2)
    {
        const double earthRadiusKm = 6371.0;

        var dLat = DegreesToRadians(lat2 - lat1);
        var dLon = DegreesToRadians(lon2 - lon1);

        var a =
            Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
            Math.Cos(DegreesToRadians(lat1)) *
            Math.Cos(DegreesToRadians(lat2)) *
            Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

        return earthRadiusKm * 2 *
            Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
    }

    private static double DegreesToRadians(double degrees)
        => degrees * Math.PI / 180d;

    private static string CreateMapHtml(
        double startLatitude,
        double startLongitude,
        List<double[]> coordinates,
        List<Landmark> landmarks)
    {
        var geoJsonCoordinates = JsonSerializer.Serialize(coordinates);

        var markers = JsonSerializer.Serialize(
            landmarks.Select(l => new
            {
                l.Name,
                lat = l.Latitude,
                lon = l.Longitude
            }));

        var start = JsonSerializer.Serialize(new
        {
            lat = startLatitude,
            lon = startLongitude
        });

        return $$"""
        <!DOCTYPE html>
        <html>
        <head>
          <meta name="viewport"
                content="width=device-width, initial-scale=1.0">
          <link rel="stylesheet"
                href="https://unpkg.com/leaflet@1.9.4/dist/leaflet.css">
          <style>
            html, body, #map {
              height: 100%;
              width: 100%;
              margin: 0;
            }

          </style>
        </head>
        <body>
          <div id="map"></div>
          <script src="https://unpkg.com/leaflet@1.9.4/dist/leaflet.js"></script>
          <script>
            const map = L.map('map');
            L.tileLayer('{{MapSettings.CartoTileUrl}}', {
                subdomains: 'abcd',
                maxZoom: 20,
                attribution: '&copy; OpenStreetMap contributors &copy; CARTO'
            }).addTo(map);

            const coords = {{geoJsonCoordinates}};
            const path = coords.map(c => [c[1], c[0]]);
            const line = L.polyline(path, { color: '#1769aa', weight: 5 })
              .addTo(map);

           const start = {{start}};

        L.circleMarker([start.lat, start.lon], {
            radius: 9,
            color: '#ffffff',
            weight: 2,
            fillColor: '#16a34a',
            fillOpacity: 1
        })
        .addTo(map)
        .bindPopup('Start en einde');

            const landmarks = {{markers}};
            landmarks.forEach(l => {
              L.marker([l.lat, l.lon])
                .addTo(map)
                .bindPopup(l.Name);
            });

            map.fitBounds(line.getBounds(), { padding: [20, 20] });
          </script>
        </body>
        </html>
        """;
    }

    private sealed class OsrmResponse
    {
        public List<OsrmRoute>? Routes { get; set; }
    }

    private sealed class OsrmRoute
    {
        public double Distance { get; set; }
        public double Duration { get; set; }
        public OsrmGeometry? Geometry { get; set; }
    }

    private sealed class OsrmGeometry
    {
        public List<double[]>? Coordinates { get; set; }
    }
}
