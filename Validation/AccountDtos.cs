namespace Bank.Api.Validation;

public record AccountCreateDto(int ClientId, string AccountNumber, string AccountType, decimal InitialBalance, bool IsActive);
public record AccountUpdateDto(string AccountType, bool IsActive);
public record AccountDto(int Id, string AccountNumber, string AccountType, decimal CurrentBalance, bool IsActive, int ClientId);

