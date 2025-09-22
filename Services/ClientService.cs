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
            PasswordHash = dto.Password, 
            IsActive = true
        };

        _db.Clients.Add(client);
        await _db.SaveChangesAsync();

        return new ClientDto(client.Id, person.Name, client.ClientCode, client.IsActive);
    }


    public async Task<IEnumerable<ClientDto>> GetAllAsync() =>
        await _db.Clients.Include(c => c.Person)
            .OrderBy(c => c.Person.Name)
            .Select(c => new ClientDto(c.Id, c.Person.Name, c.ClientCode, c.IsActive))
            .ToListAsync();

    public async Task<ClientDetailDto?> GetByIdAsync(int id) =>
        await _db.Clients.Include(c => c.Person)
            .Where(c => c.Id == id)
            .Select(c => new ClientDetailDto(
                c.Id, c.Person.Name, c.Person.Gender, c.Person.Age, c.Person.Identification,
                c.Person.Address, c.Person.Phone, c.ClientCode, c.IsActive))
            .FirstOrDefaultAsync();

    public async Task<bool> UpdateAsync(int id, ClientUpdateDto dto)
    {
        var client = await _db.Clients.Include(c => c.Person).FirstOrDefaultAsync(c => c.Id == id);
        if (client is null) return false;

        client.Person.Name = dto.Name;
        client.Person.Gender = dto.Gender;
        client.Person.Age = dto.Age;
        client.Person.Identification = dto.Identification;
        client.Person.Address = dto.Address;
        client.Person.Phone = dto.Phone;
        client.IsActive = dto.IsActive;

        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var client = await _db.Clients.FindAsync(id);
        if (client is null) return false;

        var hasAccounts = await _db.Accounts.AnyAsync(a => a.ClientId == id);
        if (hasAccounts)
            throw new InvalidOperationException("Client has accounts assigned. Delete or reassign them first.");

        _db.Clients.Remove(client);
        await _db.SaveChangesAsync();
        return true;
    }
}

