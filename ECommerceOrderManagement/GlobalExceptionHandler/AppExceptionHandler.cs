using Microsoft.AspNetCore.Diagnostics;
using System.Text;

namespace ECommerceOrderManagement.GlobalExceptionHandler
{
    public class AppExceptionHandler : IExceptionHandler
    {
        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        {
            if (exception is OrderNotFoundException orderNotFoundException)
            {
                var response = new ErrorResponse
                {
                    StatusCode = StatusCodes.Status404NotFound,
                    ExceptionMessage = orderNotFoundException.Message,
                    Title = "Order Not Found"
                };

                await httpContext.Response.WriteAsJsonAsync(response, cancellationToken);
                httpContext.Response.StatusCode = StatusCodes.Status404NotFound;
                return true;
            }

            // Handle other exceptions with detailed information
            var detailedErrorResponse = new ErrorResponse
            {
                StatusCode = StatusCodes.Status500InternalServerError,
                ExceptionMessage = exception.Message,
                Title = "Something went wrong",
                Details = FormatDetailedException(exception) 
            };

            await httpContext.Response.WriteAsJsonAsync(detailedErrorResponse, cancellationToken);
            httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
            return true;
        }

        private string FormatDetailedException(Exception exception)
        {
            var exceptionDetails = new StringBuilder();
            exceptionDetails.AppendLine($"Exception: {exception.Message}");
            exceptionDetails.AppendLine("Stack Trace:");
            exceptionDetails.AppendLine(exception.StackTrace);

            // Include inner exceptions if any
            var innerException = exception.InnerException;
            while (innerException != null)
            {
                exceptionDetails.AppendLine();
                exceptionDetails.AppendLine($"Inner Exception: {innerException.Message}");
                exceptionDetails.AppendLine("Inner Stack Trace:");
                exceptionDetails.AppendLine(innerException.StackTrace);

                innerException = innerException.InnerException;
            }

            return exceptionDetails.ToString();
        }
    }
}
