using Bank.Api.Services;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class ReportsController : ControllerBase
{
    private readonly IReportService _svc;
    public ReportsController(IReportService svc) => _svc = svc;

    
    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] int clientId, [FromQuery] DateTime from, [FromQuery] DateTime to, [FromQuery] string format = "json")
    {
        if (string.Equals(format, "pdf", StringComparison.OrdinalIgnoreCase))
        {
            var b64 = await _svc.GetStatementPdfBase64Async(clientId, from, to);
            return Ok(new { contentType = "application/pdf", base64 = b64, fileName = $"statement_{clientId}_{from:yyyyMMdd}_{to:yyyyMMdd}.pdf" });
        }

        var data = await _svc.GetStatementAsync(clientId, from, to);
        return Ok(data);
    }
}

