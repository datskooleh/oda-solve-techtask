using System.Diagnostics;

namespace Oda.HospitalManagement.API.Middleware
{
    public sealed class TaskCancellationMiddleware(RequestDelegate next, ILogger<LoggerMiddleware> logger)
    {
        public async Task InvokeAsync(HttpContext context)
        {
            var stopwatch = Stopwatch.StartNew();
            try
            {
                await next.Invoke(context);
            }
            catch (Exception ex) when (ex is OperationCanceledException || ex is TaskCanceledException)
            {
                if (context.RequestAborted.IsCancellationRequested)
                    logger.LogWarning(ex, "Request was cancelled by user");
                else
                {
                    context.Response.StatusCode = StatusCodes.Status400BadRequest;
                    logger.LogError(ex, "Unexpected task cancellation");
                }
            }
            catch (Exception ex)
            {
                if (context.Response.StatusCode > 500)
                    context.Response.StatusCode = StatusCodes.Status400BadRequest;
                logger.LogError(ex, "Unexpected exception on request");
            }
            finally
            {
                stopwatch.Stop();
                logger.LogInformation($"Request processed in {stopwatch.Elapsed}");
            }
        }
    }
}
