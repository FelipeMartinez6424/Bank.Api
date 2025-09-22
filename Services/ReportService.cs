using Bank.Api.Infrastructure;
using Bank.Api.Validation;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
using System.Globalization;

namespace Bank.Api.Services;

public class ReportService : IReportService
{
    private readonly AppDbContext _db;
    public ReportService(AppDbContext db) => _db = db;

    public async Task<ReportResult> GetStatementAsync(int clientId, DateTime from, DateTime to)
    {
        
        var fromDate = from.Date;
        var toExclusive = to.Date.AddDays(1);

        var client = await _db.Clients
            .AsNoTracking()
            .Include(c => c.Person)
            .FirstOrDefaultAsync(c => c.Id == clientId)
            ?? throw new InvalidOperationException("Client not found.");

        var accounts = await _db.Accounts
            .AsNoTracking()
            .Where(a => a.ClientId == clientId)
            .OrderBy(a => a.AccountNumber)
            .Select(a => new ReportAccountItem(
                a.AccountNumber,
                a.AccountType,          
                a.CurrentBalance))
            .ToListAsync();

       
        var movements = await _db.Movements
            .AsNoTracking()
            .Include(m => m.Account)
            .Where(m => m.Account.ClientId == clientId
                        && m.OccurredAt >= fromDate
                        && m.OccurredAt < toExclusive)
            .OrderBy(m => m.Id)        
            .Select(m => new ReportMovementItem(
                m.OccurredAt,
                m.Account.AccountNumber,
                m.MovementType.ToString(),   
                m.MovementType == Domain.MovementType.Credit ? m.Amount : -m.Amount,
                m.AvailableBalanceAfter))
            .ToListAsync();

        
        var totalCredits = movements
            .Where(x => x.MovementType == "Credit")
            .Sum(x => x.Amount);

        var totalDebits = movements
            .Where(x => x.MovementType == "Debit")
            .Sum(x => Math.Abs(x.Amount));

        return new ReportResult(
            client.Person.Name,
            fromDate,
            to.Date,          
            accounts,
            movements,
            totalCredits,
            totalDebits
        );
    }

    public async Task<string> GetStatementPdfBase64Async(int clientId, DateTime from, DateTime to)
    {
        var data = await GetStatementAsync(clientId, from, to);

        
        var culture = new CultureInfo("es-CO");
        var prevCulture = CultureInfo.CurrentCulture;
        var prevUICulture = CultureInfo.CurrentUICulture;
        CultureInfo.CurrentCulture = culture;
        CultureInfo.CurrentUICulture = culture;

        
        string AccountTypeEs(string t)
            => (t ?? "").Equals("Savings", StringComparison.OrdinalIgnoreCase) ? "Ahorros"
             : (t ?? "").Equals("Checking", StringComparison.OrdinalIgnoreCase) ? "Corriente"
             : t ?? "";

        string MovementTypeEs(string t)
            => (t ?? "").Equals("Credit", StringComparison.OrdinalIgnoreCase) ? "Crédito" : "Débito";

        try
        {
            var bytes = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Margin(30);

                    page.Header()
                        .Text($"Estado de Cuenta - {data.ClientName}")
                        .SemiBold().FontSize(18);

                    page.Content().Column(col =>
                    {
                        col.Item().Text($"Desde: {data.From:yyyy-MM-dd}   Hasta: {data.To:yyyy-MM-dd}");
                        col.Item().Text($"Totales → Créditos: {data.TotalCredits:C} | Débitos: {data.TotalDebits:C}");

                        
                        col.Item().PaddingTop(10).Text("Cuentas").SemiBold().FontSize(14);
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
                                h.Cell().Text("Número").SemiBold();
                                h.Cell().Text("Tipo").SemiBold();
                                h.Cell().Text("Saldo").SemiBold();
                            });

                            foreach (var a in data.Accounts)
                            {
                                t.Cell().Text(a.AccountNumber);
                                t.Cell().Text(AccountTypeEs(a.AccountType?.ToString() ?? ""));
                                t.Cell().Text($"{a.CurrentBalance:0,0.00}");
                            }
                        });

                        
                        col.Item().PaddingTop(10).Text("Movimientos").SemiBold().FontSize(14);
                        col.Item().Table(t =>
                        {
                            t.ColumnsDefinition(c =>
                            {
                                c.ConstantColumn(120); 
                                c.RelativeColumn();    
                                c.ConstantColumn(70);  
                                c.ConstantColumn(90);  
                                c.ConstantColumn(110); 
                            });

                            t.Header(h =>
                            {
                                h.Cell().Text("Fecha").SemiBold();
                                h.Cell().Text("Cuenta").SemiBold();
                                h.Cell().Text("Tipo").SemiBold();
                                h.Cell().Text("Monto").SemiBold();
                                h.Cell().Text("Saldo Luego").SemiBold();
                            });

                            foreach (var m in data.Movements)
                            {
                                t.Cell().Text(m.Date.ToString("yyyy-MM-dd HH:mm"));
                                t.Cell().Text(m.AccountNumber);
                                t.Cell().Text(MovementTypeEs(m.MovementType));
                                t.Cell().Text($"{m.Amount:0,0.00}");
                                t.Cell().Text($"{m.AvailableBalanceAfter:0,0.00}");
                            }
                        });
                    });
                });
            }).GeneratePdf();

            return Convert.ToBase64String(bytes);
        }
        finally
        {
            
            CultureInfo.CurrentCulture = prevCulture;
            CultureInfo.CurrentUICulture = prevUICulture;
        }
    }


}
