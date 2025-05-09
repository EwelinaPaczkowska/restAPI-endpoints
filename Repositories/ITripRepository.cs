using webAPI.DTOs;
namespace webAPI.Repositories;

public interface ITripRepository
{
    Task<IEnumerable<TripDTO>> GetTripsAsync(CancellationToken cancellationToken);
}