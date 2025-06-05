namespace s20522_APBD_CodeFirst.DTO;

public class TripGetDto
{
    public String name;
    public String description;
    public String datefrom;
    public String dateto;
    public int maxpeople;
    public List<CountryGetDto> countries;
    public List<ClientGetDto> clients;
}