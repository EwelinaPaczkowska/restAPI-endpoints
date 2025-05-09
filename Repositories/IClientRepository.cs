using webAPI.DTOs;
using webAPI.Models;

public interface IClientRepository
{
    Task<IEnumerable<TripForClientDTO>> GetTripsForClientAsync(int id, CancellationToken cancellationToken);
    Task<int> CreateNewClientAsync(Client client, CancellationToken cancellationToken);
    Task<bool> DoesPeselExistAsync(string pesel, CancellationToken cancellationToken);
    Task<bool> DoesClientExistAsync(int id, CancellationToken cancellationToken);
    Task<bool> DoesTripExistAsync(int id, CancellationToken cancellationToken);
    Task<bool> DoesClientTripAssignmentExist(int ClientId, int TripId, CancellationToken cancellationToken);
    Task<int> HowManyPeopleAreAssignedToTripAsync(int TripId, CancellationToken cancellationToken);
    Task<int> MaxPeopleOnTrip(int TripId, CancellationToken cancellationToken);
    Task AssignClientToTripAsync(int id, int tripId,int registrationDate, int? paymentDate , CancellationToken cancellationToken);
    Task DeleteClientToTripAssignmentAsync(int id, int tripId, CancellationToken cancellationToken);
}