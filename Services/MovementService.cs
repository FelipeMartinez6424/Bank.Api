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
}

