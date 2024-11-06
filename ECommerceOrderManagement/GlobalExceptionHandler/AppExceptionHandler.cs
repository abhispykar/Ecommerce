    using Microsoft.AspNetCore.Diagnostics;

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

                    // Handle other exceptions 
                    var genericErrorResponse = new ErrorResponse
                    {
                        StatusCode = StatusCodes.Status500InternalServerError,
                        ExceptionMessage = exception.Message,
                        Title = "Something went wrong"
                    };

                    await httpContext.Response.WriteAsJsonAsync(genericErrorResponse, cancellationToken);
                    httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
                    return true;
                }
            }
    
    }
