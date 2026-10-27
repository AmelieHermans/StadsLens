using AutoMapper;
using Microsoft.Extensions.Configuration;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using StadsLens.Application.Interfaces.Repositories;
using StadsLens.Application.Interfaces.Services;
using StadsLens.Contracts.Routes;

namespace StadsLens.Application.Services;

public class RouteService : IRouteService
{
    private readonly IRouteRepository _routeRepository;
    private readonly IMapper _mapper;
    private readonly ILandmarkRepository _landmarkRepository;
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;

    public RouteService(IRouteRepository routeRepository, ILandmarkRepository landmarkRepository, IMapper mapper, HttpClient httpClient, IConfiguration configuration)
    {
        _routeRepository = routeRepository;
        _landmarkRepository = landmarkRepository;
        _mapper = mapper;
        _httpClient = httpClient;
        _configuration = configuration;
    }

    public async Task<IEnumerable<RouteResponse>> GetAllAsync()
    {
        var routes = await _routeRepository.GetAllAsync();
        return _mapper.Map<IEnumerable<RouteResponse>>(routes);
    }

    public async Task<RouteResponse?> GetByIdAsync(int id)
    {
        var route = await _routeRepository.GetByIdAsync(id);
        return route is null ? null : _mapper.Map<RouteResponse>(route);
    }

    public async Task<RoutePlanResponse> PlanAsync(CreateRouteRequest request, CancellationToken cancellationToken = default)
    {
        var route = await _routeRepository.GetByIdAsync(request.RouteId)
            ?? throw new KeyNotFoundException($"Route {request.RouteId} bestaat niet.");
        var landmarks = (await _landmarkRepository.GetByRouteIdAsync(request.RouteId)).ToList();
        if (landmarks.Count == 0)
            throw new InvalidOperationException("Deze route heeft nog geen landmarks.");

        var start = request.StartLatitude.HasValue && request.StartLongitude.HasValue
            ? (Latitude: request.StartLatitude.Value, Longitude: request.StartLongitude.Value)
            : (landmarks[0].Landmark.Latitude, landmarks[0].Landmark.Longitude);
        var nearestIndex = landmarks.Select((landmark, index) => (index, distance: DistanceMeters(start.Latitude, start.Longitude, landmark.Landmark.Latitude, landmark.Landmark.Longitude))).OrderBy(x => x.distance).First().index;
        var selected = new List<StadsLens.Domain.Entities.RouteLandmark>();
        var budgetMeters = request.DistanceKm * 1000;
        var estimatedMeters = 0d;
        var previous = start;
        foreach (var landmark in landmarks.Skip(nearestIndex))
        {
            var leg = DistanceMeters(previous.Latitude, previous.Longitude, landmark.Landmark.Latitude, landmark.Landmark.Longitude) * 1.25;
            if (selected.Count > 0 && estimatedMeters + leg > budgetMeters) break;
            selected.Add(landmark);
            estimatedMeters += leg;
            previous = (landmark.Landmark.Latitude, landmark.Landmark.Longitude);
        }

        var waypoints = new[] { start }.Concat(selected.Select(x => (x.Landmark.Latitude, x.Landmark.Longitude))).ToList();
        var orsResult = await GetOrsRouteAsync(waypoints, cancellationToken);
        var geometry = orsResult?.Geometry ?? waypoints.Select(x => new RouteCoordinateResponse { Latitude = x.Latitude, Longitude = x.Longitude }).ToList();
        var distance = orsResult?.DistanceMeters ?? estimatedMeters;
        return new RoutePlanResponse
        {
            RouteId = route.Id, RouteName = route.Name, RequestedDistanceKm = request.DistanceKm,
            DistanceMeters = distance, DurationSeconds = orsResult?.DurationSeconds ?? distance / 1.4,
            StartLatitude = start.Latitude, StartLongitude = start.Longitude, IsEstimated = orsResult is null,
            Geometry = geometry,
            Landmarks = selected.Select(x => new RouteLandmarkResponse { Id = x.LandmarkId, Name = x.Landmark.Name, Latitude = x.Landmark.Latitude, Longitude = x.Landmark.Longitude, OrderInRoute = x.OrderInRoute }).ToList()
        };
    }

    private async Task<(double DistanceMeters, double DurationSeconds, IReadOnlyList<RouteCoordinateResponse> Geometry)?> GetOrsRouteAsync(IReadOnlyList<(double Latitude, double Longitude)> waypoints, CancellationToken cancellationToken)
    {
        var apiKey = _configuration["OpenRouteService:ApiKey"];
        if (string.IsNullOrWhiteSpace(apiKey)) return null;
        using var request = new HttpRequestMessage(HttpMethod.Post, _configuration["OpenRouteService:DirectionsUrl"] ?? "https://api.openrouteservice.org/v2/directions/foot-walking/geojson");
        request.Headers.TryAddWithoutValidation("Authorization", apiKey);
        request.Content = JsonContent.Create(new { coordinates = waypoints.Select(x => new[] { x.Longitude, x.Latitude }) });
        HttpResponseMessage response;
        try
        {
            response = await _httpClient.SendAsync(request, cancellationToken);
            if (!response.IsSuccessStatusCode)
                return null;
        }
        catch (HttpRequestException)
        {
            return null;
        }
        using (response)
        {
            using var document = JsonDocument.Parse(await response.Content.ReadAsStringAsync(cancellationToken));
            var feature = document.RootElement.GetProperty("features")[0];
            var summary = feature.GetProperty("properties").GetProperty("summary");
            var coordinates = feature.GetProperty("geometry").GetProperty("coordinates").EnumerateArray().Select(x => new RouteCoordinateResponse { Longitude = x[0].GetDouble(), Latitude = x[1].GetDouble() }).ToList();
            return (summary.GetProperty("distance").GetDouble(), summary.GetProperty("duration").GetDouble(), coordinates);
        }
    }

    private static double DistanceMeters(double lat1, double lon1, double lat2, double lon2)
    {
        const double earthRadius = 6371000;
        var dLat = DegreesToRadians(lat2 - lat1); var dLon = DegreesToRadians(lon2 - lon1);
        var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) + Math.Cos(DegreesToRadians(lat1)) * Math.Cos(DegreesToRadians(lat2)) * Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
        return earthRadius * 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
    }
    private static double DegreesToRadians(double degrees) => degrees * Math.PI / 180;
}
