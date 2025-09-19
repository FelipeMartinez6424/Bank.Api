using Bank.Api.Domain;

namespace Bank.Api.Validation;

public record MovementCreateDto(int AccountId, MovementType MovementType, decimal Amount, DateTime? OccurredAt);
public record MovementDto(int Id, int AccountId, string AccountNumber, MovementType MovementType, decimal Amount, DateTime OccurredAt, decimal AvailableBalanceAfter);

