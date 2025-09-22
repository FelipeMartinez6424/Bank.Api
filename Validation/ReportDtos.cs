namespace Bank.Api.Validation;

public record ReportQuery(int ClientId, DateTime From, DateTime To, string Format); 
public record ReportAccountItem(string AccountNumber, string AccountType, decimal CurrentBalance);
public record ReportMovementItem(DateTime Date, string AccountNumber, string MovementType, decimal Amount, decimal AvailableBalanceAfter);
public record ReportResult(
    string ClientName,
    DateTime From,
    DateTime To,
    List<ReportAccountItem> Accounts,
    List<ReportMovementItem> Movements,
    decimal TotalCredits,
    decimal TotalDebits);

