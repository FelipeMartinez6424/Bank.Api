using Bank.Api.Domain;
using Bank.Api.Infrastructure;
using Bank.Api.Validation;
using Microsoft.EntityFrameworkCore;

namespace Bank.Api.Services;

public class MovementService : IMovementService
{
    private readonly AppDbContext _db;
    private readonly decimal _dailyLimit;

    public MovementService(AppDbContext db, IConfiguration cfg)
    {
        _db = db;
        _dailyLimit = cfg.GetValue<decimal>("DailyDebitLimit", 1000m);
    }

    public async Task<MovementDto> RegisterAsync(MovementCreateDto dto)
    {
        var acc = await _db.Accounts.Include(a => a.Client)
            .FirstOrDefaultAsync(a => a.Id == dto.AccountId)
            ?? throw new InvalidOperationException("Account not found.");

        var amount = Math.Abs(dto.Amount);

        if (dto.MovementType == MovementType.Debit)
        {
            if (acc.CurrentBalance <= 0 || acc.CurrentBalance < amount)
                throw new InvalidOperationException("Saldo no disponible");

            var day = (dto.OccurredAt ?? DateTime.UtcNow).Date;
            var totalDebitsToday = await _db.Movements
                .Where(m => m.Account.ClientId == acc.ClientId &&
                            m.MovementType == MovementType.Debit &&
                            m.OccurredAt.Date == day)
                .SumAsync(m => (decimal?)m.Amount) ?? 0m;

            if (totalDebitsToday + amount > _dailyLimit)
                throw new InvalidOperationException("Cupo diario Excedido");

            acc.CurrentBalance -= amount;
        }
        else
        {
            acc.CurrentBalance += amount;
        }

        var mov = new Movement
        {
            AccountId = acc.Id,
            MovementType = dto.MovementType,
            Amount = amount,
            OccurredAt = dto.OccurredAt ?? DateTime.UtcNow,
            AvailableBalanceAfter = acc.CurrentBalance
        };

        _db.Movements.Add(mov);
        await _db.SaveChangesAsync();

        return new MovementDto(mov.Id, mov.AccountId, acc.AccountNumber, mov.MovementType,
                               mov.Amount, mov.OccurredAt, mov.AvailableBalanceAfter);
    }
    public async Task<MovementDto?> GetByIdAsync(int id) =>
    await _db.Movements
        .Where(m => m.Id == id)
        .Select(m => new MovementDto(m.Id, m.AccountId, m.Account.AccountNumber, m.MovementType, m.Amount, m.OccurredAt, m.AvailableBalanceAfter))
        .FirstOrDefaultAsync();

    public async Task<IEnumerable<MovementDto>> GetAllAsync(int? accountId = null, DateTime? from = null, DateTime? to = null)
    {
        var q = _db.Movements.AsQueryable();

        if (accountId.HasValue) q = q.Where(m => m.AccountId == accountId.Value);
        if (from.HasValue) q = q.Where(m => m.OccurredAt >= from.Value);
        if (to.HasValue) q = q.Where(m => m.OccurredAt <= to.Value);

        return await q
            .OrderByDescending(m => m.OccurredAt)
            .Select(m => new MovementDto(m.Id, m.AccountId, m.Account.AccountNumber, m.MovementType, m.Amount, m.OccurredAt, m.AvailableBalanceAfter))
            .ToListAsync();
    }

    
    public async Task<bool> DeleteAsync(int id)
    {
        var mov = await _db.Movements.Include(m => m.Account).FirstOrDefaultAsync(m => m.Id == id);
        if (mov is null) return false;

        var hasNewer = await _db.Movements.AnyAsync(m => m.AccountId == mov.AccountId && m.OccurredAt > mov.OccurredAt);
        if (hasNewer)
            throw new InvalidOperationException("Cannot delete a non-last movement for this account.");

    
        if (mov.MovementType == MovementType.Credit)
            mov.Account.CurrentBalance -= mov.Amount;
        else
            mov.Account.CurrentBalance += mov.Amount;

        _db.Movements.Remove(mov);
        await _db.SaveChangesAsync();
        return true;
    }

}

