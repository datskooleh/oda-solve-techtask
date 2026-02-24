using System.Diagnostics;

namespace Oda.HospitalManagement.API.Middleware
{
    public sealed class TaskCancellationMiddleware(RequestDelegate next, ILogger<TaskCancellationMiddleware> logger)
    {
        public async Task InvokeAsync(HttpContext context)
        {
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
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                logger.LogError(ex, "Unexpected exception on request");
            }
        }
    }
}
