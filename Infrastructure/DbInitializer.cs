using System.Security.Cryptography;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Bank.Api.Domain;
using Microsoft.EntityFrameworkCore;

namespace Bank.Api.Infrastructure;

public static class DbInitializer
{
    public static async Task SeedAsync(AppDbContext db)
    {
        if (await db.Persons.AnyAsync()) return;

        var jose = new Person { Name = "Jose Lema", Gender = "M", Age = 40, Identification = "CC-1", Address = "Otavalo sn y principal", Phone = "098254785" };
        var marianela = new Person { Name = "Marianela Montalvo", Gender = "F", Age = 38, Identification = "CC-2", Address = "Amazonas y NNUU", Phone = "097548965" };
        var juan = new Person { Name = "Juan Osorio", Gender = "M", Age = 35, Identification = "CC-3", Address = "13 junio y Equinoccial", Phone = "098874587" };

        db.Persons.AddRange(jose, marianela, juan);
        await db.SaveChangesAsync(); 

       
        var clients = new[]
        {
                new Client { Id=jose.Id,      ClientCode="1234", PasswordHash="1234", IsActive=true },
                new Client { Id=marianela.Id, ClientCode="5678", PasswordHash="5678", IsActive=true },
                new Client { Id=juan.Id,      ClientCode="1245", PasswordHash="1245", IsActive=true }
            };
        db.Clients.AddRange(clients);

        await db.SaveChangesAsync();

        var a1 = new Account { AccountNumber = "478758", AccountType = "Savings", InitialBalance = 2000, CurrentBalance = 2000, IsActive = true, ClientId = clients[0].Id };
        var a2 = new Account { AccountNumber = "225487", AccountType = "Checking", InitialBalance = 100, CurrentBalance = 100, IsActive = true, ClientId = clients[1].Id };
        var a3 = new Account { AccountNumber = "495878", AccountType = "Savings", InitialBalance = 0, CurrentBalance = 0, IsActive = true, ClientId = clients[2].Id };
        var a4 = new Account { AccountNumber = "496825", AccountType = "Savings", InitialBalance = 540, CurrentBalance = 540, IsActive = true, ClientId = clients[1].Id };

        db.Accounts.AddRange(a1, a2, a3, a4);
        await db.SaveChangesAsync();
    }

    
}


