using Microsoft.AspNetCore.Mvc;
using s20522_APBD_CodeFirst.Services;

namespace s20522_APBD_CodeFirst.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ClientsController : ControllerBase
{
    private readonly IClientService clientService;

    public ClientsController(IClientService clientService)
    {
        this.clientService = clientService;
    }

    [HttpDelete("{idClient}")]
    public async Task<IActionResult> DeleteClient(int idClient)
    {
        try
        {
            var deleted = await clientService.DeleteClientAsync(idClient);
            if (!deleted)
            {
                return NotFound(new { message = $"Klient o id {idClient} nie istnieje." });
            }
            return NoContent();
        }
        catch (ClientService.ClientHasTripsException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}