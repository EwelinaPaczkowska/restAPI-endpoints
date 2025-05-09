using webAPI.DTOs;

namespace webAPI.Services;

public abstract class IClientService
{
    public abstract Task<IEnumerable<TripForClientDTO>> GetTripsForClientAsync(int id, CancellationToken cancellationToken);
    public abstract Task<int> CreateNewClientAsync(CClientDTO dto, CancellationToken cancellationToken);
    public abstract Task AssignClientToTripAsync(int id, int tripId, CancellationToken cancellationToken);
    public abstract Task DeleteClientToTripAssignmentAsync(int id, int tripId, CancellationToken cancellationToken);
}