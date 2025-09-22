using System.Text.Json.Serialization;
using static System.Object;

namespace Bank.Api.Domain;

public class Client
{
    public int Id { get; set; }
    public string ClientCode { get; set; } = null!;
    [JsonIgnore]
    public string PasswordHash { get; set; } = string.Empty;
    
    public bool IsActive { get; set; } = true;

    public Person Person { get; set; } = null!;
    public ICollection<Account> Accounts { get; set; } = new List<Account>();
}


