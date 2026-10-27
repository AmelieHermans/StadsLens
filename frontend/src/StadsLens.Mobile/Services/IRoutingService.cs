
using StadsLens.Mobile.Models;

namespace StadsLens.Mobile.Services;

public interface IRoutingService
{
    Task<GeneratedRoute> GenerateRouteAsync(
        double startLatitude,
        double startLongitude,
        double targetDistanceKm,
        IEnumerable<Landmark> landmarks,
        CancellationToken cancellationToken = default);
}
