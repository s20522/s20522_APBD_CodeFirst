using Microsoft.EntityFrameworkCore;
using s20522_APBD_CodeFirst.Data;

namespace s20522_APBD_CodeFirst.Services;


public interface IClientService
{
    Task<bool> DeleteClientAsync(int clientId);
}

public class ClientService : IClientService
{
    private readonly ApbdCodeFirstContext data;

    public ClientService(ApbdCodeFirstContext data)
    {
        this.data = data;
    }

    public async Task<bool> DeleteClientAsync(int clientId)
    {
        var client = await data.Clients
            .Include(c => c.ClientTrips)
            .FirstOrDefaultAsync(c => c.Idclient == clientId);

        if (client == null)
        {
            return false;
        }

        if (client.ClientTrips.Any())
        {
            throw new ClientHasTripsException("Klient ma przypisane wycieczki i nie może zostać usunięty.");
        }

        data.Clients.Remove(client);
        await data.SaveChangesAsync();
        return true;
    }
    public class ClientHasTripsException : System.Exception
    {
        public ClientHasTripsException(string message) : base(message) { }
    }
}

