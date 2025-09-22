using Bank.Api.Infrastructure;
using Bank.Api.Validation;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace Bank.Api.Services;

public class ReportService : IReportService
{
    private readonly AppDbContext _db;
    public ReportService(AppDbContext db) => _db = db;

    public async Task<ReportResult> GetStatementAsync(int clientId, DateTime from, DateTime to)
    {
        var client = await _db.Clients.Include(c => c.Person)
            .FirstOrDefaultAsync(c => c.Id == clientId)
            ?? throw new InvalidOperationException("Client not found.");

        var accounts = await _db.Accounts
            .Where(a => a.ClientId == clientId)
            .OrderBy(a => a.AccountNumber)
            .Select(a => new ReportAccountItem(a.AccountNumber, a.AccountType, a.CurrentBalance))
            .ToListAsync();

        var movements = await _db.Movements
            .Where(m => m.Account.ClientId == clientId && m.OccurredAt >= from && m.OccurredAt <= to)
            .OrderBy(m => m.OccurredAt)
            .Select(m => new ReportMovementItem(
                m.OccurredAt,
                m.Account.AccountNumber,
                m.MovementType.ToString(),
                m.MovementType == Domain.MovementType.Credit ? m.Amount : -m.Amount,
                m.AvailableBalanceAfter))
            .ToListAsync();

        var totalCredits = movements.Where(x => x.MovementType == "Credit").Sum(x => x.Amount);
        var totalDebits = movements.Where(x => x.MovementType == "Debit").Sum(x => Math.Abs(x.Amount));

        return new ReportResult(client.Person.Name, from, to, accounts, movements, totalCredits, totalDebits);
    }

    public async Task<string> GetStatementPdfBase64Async(int clientId, DateTime from, DateTime to)
    {
        var data = await GetStatementAsync(clientId, from, to);

        var bytes = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Margin(30);
                page.Header().Text($"Account Statement - {data.ClientName}").SemiBold().FontSize(18);
                page.Content().Column(col =>
                {
                    col.Item().Text($"From: {data.From:yyyy-MM-dd}   To: {data.To:yyyy-MM-dd}");
                    col.Item().Text($"Totals → Credits: {data.TotalCredits:C} | Debits: {data.TotalDebits:C}");

                    col.Item().PaddingTop(10).Text("Accounts").SemiBold().FontSize(14);
                    col.Item().Table(t =>
                    {
                        t.ColumnsDefinition(c =>
                        {
                            c.RelativeColumn(); 
                            c.RelativeColumn(); 
                            c.ConstantColumn(100); 
                        });
                        t.Header(h =>
                        {
                            h.Cell().Text("Number").SemiBold();
                            h.Cell().Text("Type").SemiBold();
                            h.Cell().Text("Balance").SemiBold();
                        });
                        foreach (var a in data.Accounts)
                        {
                            t.Cell().Text(a.AccountNumber);
                            t.Cell().Text(a.AccountType);
                            t.Cell().Text($"{a.CurrentBalance:0.00}");
                        }
                    });

                    col.Item().PaddingTop(10).Text("Movements").SemiBold().FontSize(14);
                    col.Item().Table(t =>
                    {
                        t.ColumnsDefinition(c =>
                        {
                            c.ConstantColumn(90);
                            c.RelativeColumn();
                            c.ConstantColumn(70);
                            c.ConstantColumn(80);
                            c.ConstantColumn(100);
                        });
                        t.Header(h =>
                        {
                            h.Cell().Text("Date").SemiBold();
                            h.Cell().Text("Account").SemiBold();
                            h.Cell().Text("Type").SemiBold();
                            h.Cell().Text("Amount").SemiBold();
                            h.Cell().Text("Balance After").SemiBold();
                        });

                        foreach (var m in data.Movements)
                        {
                            t.Cell().Text(m.Date.ToString("yyyy-MM-dd"));
                            t.Cell().Text(m.AccountNumber);
                            t.Cell().Text(m.MovementType);
                            t.Cell().Text($"{m.Amount:0.00}");
                            t.Cell().Text($"{m.AvailableBalanceAfter:0.00}");
                        }
                    });
                });
            });
        }).GeneratePdf();

        return Convert.ToBase64String(bytes);
    }
}
