using System.Net;

namespace Etammen.GlobalExceptionHandlingMiddleware;

public class GlobalExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionHandlingMiddleware> _logger;

    public GlobalExceptionHandlingMiddleware(RequestDelegate next, ILogger<GlobalExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        
        catch (Exception ex)
        {
            _logger.LogError($"An unhandled exception occurred.\n exception code: {ex}");
            context.Response.Redirect("/Error/Error");
        }
    }
}

