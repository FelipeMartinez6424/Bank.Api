using static System.Object;

namespace Bank.Api.Domain;

public class Account
{
    public int Id { get; set; }
    public string AccountNumber { get; set; } = null!; // unique
    public string AccountType { get; set; } = null!;   // Savings/Checking (Ahorro/Corriente)
    public decimal InitialBalance { get; set; }
    public decimal CurrentBalance { get; set; }
    public bool IsActive { get; set; } = true;

    public int ClientId { get; set; }
    public Client Client { get; set; } = null!;
    public ICollection<Movement> Movements { get; set; } = new List<Movement>();
}
