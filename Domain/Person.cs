using static System.Object;

namespace Bank.Api.Domain;

public class Person
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string Gender { get; set; } = null!;
    public int Age { get; set; }
    public string Identification { get; set; } = null!;
    public string Address { get; set; } = null!;
    public string Phone { get; set; } = null!;
    public Client? Client { get; set; } 
}

