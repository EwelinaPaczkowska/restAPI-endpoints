using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using webAPI.DTOs;

namespace webAPI.Services;

public interface ITripService
{
    Task<IEnumerable<TripDTO>> GetTripsAsync(CancellationToken cancellationToken);
}