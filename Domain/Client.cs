using static System.Object;

namespace Bank.Api.Domain;

public class Client
{
    public int Id { get; set; }            // PK = PersonId
    public string ClientCode { get; set; } = null!; // unique (clientId requerido)
    public string PasswordHash { get; set; } = null!;
    public bool IsActive { get; set; } = true;

    public Person Person { get; set; } = null!;
    public ICollection<Account> Accounts { get; set; } = new List<Account>();
}

