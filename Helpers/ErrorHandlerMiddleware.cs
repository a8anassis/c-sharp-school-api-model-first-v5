using System.Net;
using System.Text.Json;
using UsersStudentsAPIApp.Services.Exceptions;

namespace UsersStudentsAPIApp.Helpers
{
    public class ErrorHandlerMiddleware
    {
        private readonly RequestDelegate _next;

        public ErrorHandlerMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception exception)
            {
                var response = context.Response;
                response.ContentType = "application/json";

                response.StatusCode = exception switch
                {
                    InvalidRegistrationException or 
                    InvalidRoleException or
                    UserAlreadyExistsException => (int) HttpStatusCode.BadRequest,

                    UnauthorizedAccessException => (int) HttpStatusCode.Unauthorized, 
                    
                    KeyNotFoundException or
                    UserNotFoundException => (int) HttpStatusCode.NotFound,
                    _ => (int) HttpStatusCode.InternalServerError
                };

                var result = JsonSerializer.Serialize(new { message = exception?.Message });
                await response.WriteAsync(result);
            }
        }
    }
}

/* switch (error)
               {
                   case InvalidRegistrationException:
                       response.StatusCode = (int)HttpStatusCode.BadRequest;
                       break;
                   case KeyNotFoundException e:
                       // not found error
                       response.StatusCode = (int)HttpStatusCode.NotFound;
                       break;
                   default:
                       // unhandled error
                       response.StatusCode = (int)HttpStatusCode.InternalServerError;
                       break;
               }*/