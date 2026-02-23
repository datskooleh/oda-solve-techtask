using System.Diagnostics;

namespace Oda.HospitalManagement.API.Middleware
{
    public sealed class LoggerMiddleware(RequestDelegate next, ILogger<LoggerMiddleware> logger)
    {
        public async Task InvokeAsync(HttpContext context)
        {
            var stopwatch = Stopwatch.StartNew();
            try
            {
                await next.Invoke(context);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Request failed");
            }
            finally
            {
                stopwatch.Stop();
                logger.LogInformation($"Request processed in {stopwatch.Elapsed}");
            }
        }
    }
}