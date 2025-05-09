using webAPI.DTOs;
using webAPI.Exceptions;
using webAPI.Models;

namespace webAPI.Services;

public class ClientService : IClientService
{
    private readonly IClientRepository _clientsRepository;
    public ClientService(IClientRepository clientsRepository)
    {
        _clientsRepository = clientsRepository;
    }

    public override async Task<IEnumerable<TripForClientDTO>> GetTripsForClientAsync(int id, CancellationToken cancellationToken)
    {
        if(id<0)
            throw new BadRequestException("id musi byc wieksze niz 0");
        
        if(!await _clientsRepository.DoesClientExistAsync(id, cancellationToken))
            throw new NotFoundException("klient z id: "+id+" nie istnieje");
        
        return await _clientsRepository.GetTripsForClientAsync(id, cancellationToken);
    }

    public override async Task<int> CreateNewClientAsync(CClientDTO dto, CancellationToken cancellationToken)
    {
        if (await _clientsRepository.DoesPeselExistAsync(dto.pesel, cancellationToken))
        {
            throw new ConflictException("taki pesel juz jest w bazie");
        }
        
        var client = new Client()
        {
            email = dto.email,
            first_name = dto.first_name,
            last_name = dto.last_name,
            phone = dto.tel,
            pesel = dto.pesel
        };
        
        return await _clientsRepository.CreateNewClientAsync(client, cancellationToken);
    }

    public override async Task AssignClientToTripAsync(int id, int tId, CancellationToken cancellationToken)
    {
        if(id<0)
            throw new BadRequestException("id musi byc wieksze niz 0");
        
        if(tId<0)
            throw new BadRequestException("id wycieczki musi byc wieksze niz 0");
        
        if(!await _clientsRepository.DoesClientExistAsync(id, cancellationToken))
            throw new NotFoundException("klient z id: "+id+" nie istnieje");
        
        if(!await _clientsRepository.DoesTripExistAsync(tId, cancellationToken))
            throw new NotFoundException("id wycieczki: "+tId+" nie istnieje");
        
        if(await _clientsRepository.DoesClientTripAssignmentExist(id, tId,cancellationToken))
            throw new ConflictException("klient z id: "+id+", id wycieczki: "+tId+" sa juz polaczone");
        
        var alreadyassignednumber = await _clientsRepository.HowManyPeopleAreAssignedToTripAsync(tId, cancellationToken);
        
        var maxpeople = await _clientsRepository.MaxPeopleOnTrip(tId, cancellationToken);
        
        if(alreadyassignednumber>=maxpeople)
            throw new ConflictException("id wycieczki: "+tId+" sa juz polaczone");
        
        var currDate = int.Parse(DateTime.Now.ToString("yyyyMMdd"));

        await _clientsRepository.AssignClientToTripAsync(id, tId, currDate, null, cancellationToken);
    }

    public override async Task DeleteClientToTripAssignmentAsync(int id, int tId, CancellationToken cancellationToken)
    {
        if(! await _clientsRepository.DoesClientTripAssignmentExist(id, tId,cancellationToken))
            throw new ConflictException("klient z id: "+id+", id wycieczki: "+tId+" nie sa polaczone");
        await _clientsRepository.DeleteClientToTripAssignmentAsync(id, tId, cancellationToken);
    }
}