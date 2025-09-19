using Bank.Api.Domain;
using Bank.Api.Infrastructure;
using Bank.Api.Validation;
using Microsoft.EntityFrameworkCore;

namespace Bank.Api.Services;

public class AccountService : IAccountService
{
    private readonly AppDbContext _db;
    public AccountService(AppDbContext db) => _db = db;

    public async Task<AccountDto> CreateAsync(AccountCreateDto dto)
    {
        if (await _db.Accounts.AnyAsync(a => a.AccountNumber == dto.AccountNumber))
            throw new InvalidOperationException("Account number already exists.");

        var acc = new Account
        {
            ClientId = dto.ClientId,
            AccountNumber = dto.AccountNumber,
            AccountType = dto.AccountType,
            InitialBalance = dto.InitialBalance,
            CurrentBalance = dto.InitialBalance,
            IsActive = dto.IsActive
        };

        _db.Accounts.Add(acc);
        await _db.SaveChangesAsync();

        return new AccountDto(acc.Id, acc.AccountNumber, acc.AccountType, acc.CurrentBalance, acc.IsActive, acc.ClientId);
    }

    public async Task<IEnumerable<AccountDto>> GetAllAsync() =>
        await _db.Accounts
          .OrderBy(a => a.AccountNumber)
          .Select(a => new AccountDto(a.Id, a.AccountNumber, a.AccountType, a.CurrentBalance, a.IsActive, a.ClientId))
          .ToListAsync();

    public async Task<AccountDto?> GetByIdAsync(int id) =>
        await _db.Accounts
          .Where(a => a.Id == id)
          .Select(a => new AccountDto(a.Id, a.AccountNumber, a.AccountType, a.CurrentBalance, a.IsActive, a.ClientId))
          .FirstOrDefaultAsync();

    public async Task<bool> UpdateAsync(int id, AccountUpdateDto dto)
    {
        var acc = await _db.Accounts.FindAsync(id);
        if (acc is null) return false;
        acc.AccountType = dto.AccountType;
        acc.IsActive = dto.IsActive;
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var acc = await _db.Accounts.FindAsync(id);
        if (acc is null) return false;
        _db.Accounts.Remove(acc);
        await _db.SaveChangesAsync();
        return true;
    }
}
