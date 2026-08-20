namespace ArgosSharp.Api.Middlewares
{
    internal class MainMiddleware(RequestDelegate next, ILogger<MainMiddleware> logger)
    {
        internal async Task InvokeAsync(HttpContext context)
        {
            try
            {
                logger.LogInformation("Executing Rote");
                await next(context);
            }
            catch (Exception ex)
            {
                logger.LogError("An error ocurred executing rote");
            }
        }
    }
}
