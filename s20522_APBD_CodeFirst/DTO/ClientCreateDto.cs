namespace s20522_APBD_CodeFirst.DTO;

public class ClientCreateDto
{
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Telephone { get; set; } = null!;
    public string Pesel { get; set; } = null!;
    public DateTime? PaymentDate { get; set; }
}