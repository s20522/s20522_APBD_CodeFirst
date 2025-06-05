using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using s20522_APBD_CodeFirst.Data;
using s20522_APBD_CodeFirst.DTO;
using s20522_APBD_CodeFirst.Services;

namespace s20522_APBD_CodeFirst.Controllers;

[ApiController]
[Route("[controller]")]
public class TripsController (ITripService tripService): Controller
{
    [HttpGet]
    public async Task<IActionResult> GetTrips([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        var result = await tripService.GetTripsAsync(page, pageSize);
        return Ok(result);
    }
    
    [HttpPost("{idTrip}/clients")]
    public async Task<IActionResult> AddClientToTrip(int idTrip, [FromBody] ClientCreateDto clientDto)
    {
        var (success, errorMessage) = await tripService.AddClientToTripAsync(idTrip, clientDto);

        if (!success)
        {
            return BadRequest(new { message = errorMessage });
        }

        return Ok(new { message = "Klient został pomyślnie przypisany do wycieczki." });
    }
    
}