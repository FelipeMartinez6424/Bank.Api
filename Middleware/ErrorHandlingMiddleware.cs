using System.Net;
using System.Text.Json;

namespace Bank.Api.Middleware;

public class ErrorHandlingMiddleware
{
    private readonly RequestDelegate _next;
    public ErrorHandlingMiddleware(RequestDelegate next) => _next = next;

    public async Task Invoke(HttpContext ctx)
    {
        try { await _next(ctx); }
        catch (InvalidOperationException ex)
        {
            ctx.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            await ctx.Response.WriteAsJsonAsync(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            ctx.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            var payload = new { error = "Unexpected error", detail = ex.Message };
            ctx.Response.ContentType = "application/json";
            await ctx.Response.WriteAsync(JsonSerializer.Serialize(payload));
        }
    }
}
