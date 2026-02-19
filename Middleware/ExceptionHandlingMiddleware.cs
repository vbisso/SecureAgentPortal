using System.Net;

namespace SecureAgentPortal.Middleware;

public class ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
{
    public async Task Invoke(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unhandled exception");

            if (context.Request.Path.StartsWithSegments("/api"))
            {
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                await context.Response.WriteAsJsonAsync(new { error = "Unexpected error occurred." });
                return;
            }

            context.Response.Redirect("/Home/Error");
        }
    }
}
