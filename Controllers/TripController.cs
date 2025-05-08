using Microsoft.AspNetCore.Mvc;
using webAPI.Services;

namespace webAPI.Controllers;

[ApiController]
[Route("api/[controller]")]

public class TripController : ControllerBase
{
    private readonly ITripService _tripsService;
    public TripController(ITripService tripsService)
    {
        _tripsService = tripsService;
    }
    
    [HttpGet]
    public async Task<IActionResult> GetTripsAsync(CancellationToken cancellationToken)
    {
        var trips = await _tripsService.GetTripsAsync(cancellationToken);
        return Ok(trips);
    }
}