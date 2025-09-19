namespace Bank.Api.Domain;

public class Movement
{
    public int Id { get; set; }
    public int AccountId { get; set; }
    public Account Account { get; set; } = null!;
    public DateTime OccurredAt { get; set; } = DateTime.UtcNow;
    public MovementType MovementType { get; set; }
    public decimal Amount { get; set; }                     // + always; sign is defined by type
    public decimal AvailableBalanceAfter { get; set; }      // saldo resultante
}

