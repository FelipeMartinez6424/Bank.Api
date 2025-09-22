using Bank.Api.Validation;

namespace Bank.Api.Services;

public interface IReportService
{
    Task<ReportResult> GetStatementAsync(int clientId, DateTime from, DateTime to);
    Task<string> GetStatementPdfBase64Async(int clientId, DateTime from, DateTime to);
}
