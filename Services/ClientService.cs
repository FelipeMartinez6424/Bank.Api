using Bank.Api.Domain;
using Bank.Api.Infrastructure;
using Bank.Api.Validation;
using Microsoft.EntityFrameworkCore;

namespace Bank.Api.Services;

public class ClientService : IClientService
{
    private readonly AppDbContext _db;
    public ClientService(AppDbContext db) => _db = db;

    public async Task<ClientDto> CreateAsync(ClientCreateDto dto)
    {
        if (await _db.Clients.AnyAsync(c => c.ClientCode == dto.ClientCode))
            throw new InvalidOperationException("Client code already exists.");

        var person = new Person
        {
            Name = dto.Name,
            Gender = dto.Gender,
            Age = dto.Age,
            Identification = dto.Identification,
            Address = dto.Address,
            Phone = dto.Phone
        };

        var client = new Client
        {
            Person = person,
            ClientCode = dto.ClientCode,
            PasswordHash = dto.Password, // luego podemos hacer hashing
            IsActive = true
        };

        _db.Clients.Add(client);
        await _db.SaveChangesAsync();

        return new ClientDto(client.Id, person.Name, client.ClientCode, client.IsActive);
    }

    public async Task<IEnumerable<ClientDto>> GetAllAsync() =>
        await _db.Clients
            .Include(c => c.Person)
            .OrderBy(c => c.Person.Name)
            .Select(c => new ClientDto(c.Id, c.Person.Name, c.ClientCode, c.IsActive))
            .ToListAsync();
}

