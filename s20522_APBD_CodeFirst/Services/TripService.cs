using Microsoft.EntityFrameworkCore;
using s20522_APBD_CodeFirst.Data;
using s20522_APBD_CodeFirst.DTO;
using s20522_APBD_CodeFirst.Models;

namespace s20522_APBD_CodeFirst.Services;


public interface ITripService
{ 
    Task<object> GetTripsAsync(int page, int pageSize);
    Task<(bool Success, string? ErrorMessage)> AddClientToTripAsync(int idTrip, ClientCreateDto clientDto);

}

public class TripService : ITripService
{
    private readonly ApbdCodeFirstContext data;

    public TripService(ApbdCodeFirstContext data)
    {
        this.data = data;
    }

    public async Task<object> GetTripsAsync(int page, int pageSize)
    {
        if (page < 1) page = 1;
        if (pageSize < 1) pageSize = 10;

        var totalTrips = await data.Trips.CountAsync();
        var allPages = (int)Math.Ceiling(totalTrips / (double)pageSize);

        var trips = await data.Trips
            .Include(t => t.ClientTrips)
            .ThenInclude(ct => ct.IdclientNavigation)
            .Include(t => t.Idcountries)
            .OrderByDescending(t => t.Datefrom)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(t => new
            {
                name = t.Name,
                description = t.Description,
                datefrom = t.Datefrom,
                dateto = t.Dateto,
                maxpeople = t.Maxpeople,
                countries = t.Idcountries.Select(c => new
                {
                    name = c.Name
                }),
                clients = t.ClientTrips.Select(ct => new
                {
                    firstName = ct.IdclientNavigation.Firstname,
                    lastName = ct.IdclientNavigation.Lastname
                })
            })
            .ToListAsync();

        return new
        {
            pageNum = page,
            pageSize = pageSize,
            allPages = allPages,
            trips = trips
        };
    }
    
    public async Task<(bool Success, string? ErrorMessage)> AddClientToTripAsync(int idTrip, ClientCreateDto clientDto)
    {
        var existingClient = await data.Clients
            .Include(c => c.ClientTrips)
            .FirstOrDefaultAsync(c => c.Pesel == clientDto.Pesel);

        if (existingClient != null)
        {
            var isAlreadyRegistered = existingClient.ClientTrips.Any(ct => ct.Idtrip == idTrip);
            if (isAlreadyRegistered)
                return (false, "Klient jest już zapisany na tę wycieczkę.");

        }

        var trip = await data.Trips.FirstOrDefaultAsync(t => t.Idtrip == idTrip);
        if (trip == null)
            return (false, "Wycieczka nie istnieje.");

        if (trip.Datefrom <= DateTime.Now)
            return (false, "Nie można zapisać się na wycieczkę, bo już się odbyła. Trzeba było zapisać się przed rozpoczęciem !");

        Client clientToAssign;
        if (existingClient == null)
        {
            clientToAssign = new Client
            {
                Firstname = clientDto.FirstName,
                Lastname = clientDto.LastName,
                Email = clientDto.Email,
                Telephone = clientDto.Telephone,
                Pesel = clientDto.Pesel
            };
            await data.Clients.AddAsync(clientToAssign);
        }
        else
        {
            clientToAssign = existingClient;
        }

        var clientTrip = new ClientTrip
        {
            IdclientNavigation = clientToAssign,
            IdtripNavigation = trip,
            Registeredat = DateTime.Now,
            Paymentdate = clientDto.PaymentDate
        };

        await data.ClientTrips.AddAsync(clientTrip);
        await data.SaveChangesAsync();

        return (true, null);
    }
}