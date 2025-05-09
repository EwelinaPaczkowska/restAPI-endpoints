using Microsoft.AspNetCore.Mvc;
using webAPI.DTOs;
using webAPI.Services;

[ApiController]
[Route("api/[controller]")]

public class ClientController: ControllerBase
{
    private readonly IClientService _clientService;
    
    public ClientController(IClientService clientService)
    {
        _clientService = clientService;
    }

    [HttpGet("{id}/trips")]
    public async Task<IActionResult> GetTripsForClientAsync(int id, CancellationToken cancellationToken)
    {
        var response = await _clientService.GetTripsForClientAsync(id, cancellationToken);
        return Ok(response);
    }

    [HttpPost]
    public async Task<IActionResult> CreateNewClientAsync([FromBody] CClientDTO dto, CancellationToken cancellationToken)
    {
        var clientsid = await _clientService.CreateNewClientAsync(dto, cancellationToken);
        return Created($"/api/clients", new { ClientsId = clientsid});
    }

    [HttpPut("{id}/trips/{tId}")]
    public async Task<IActionResult> AssignClientToTripAsync(int id, int tId, CancellationToken cancellationToken)
    {
        await _clientService.AssignClientToTripAsync(id, tId, cancellationToken);
        return Created($"/api/clients/{id}/trips/{tId}", new { ClientsId = id, TripId = tId });
    }

    [HttpDelete("{id}/trips/{tId}")]
    public async Task<IActionResult> DeleteClientToTripAssignmentAsync(int id, int tId, CancellationToken cancellationToken)
    {
        await _clientService.DeleteClientToTripAssignmentAsync(id, tId, cancellationToken);
        return NoContent();
    }
}