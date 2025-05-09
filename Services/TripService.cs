using webAPI.DTOs;
using webAPI.Repositories;
using webAPI.Services;
using webAPI.Repositories;

namespace webAPI.Services;

public class TripService : ITripService
{
    private readonly ITripRepository _repository;
    public TripService(ITripRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<TripDTO>> GetTripsAsync(CancellationToken cancellationToken)
    {
        return await _repository.GetTripsAsync(cancellationToken);
    }
}