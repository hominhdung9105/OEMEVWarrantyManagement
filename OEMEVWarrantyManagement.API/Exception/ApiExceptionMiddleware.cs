using Microsoft.AspNetCore.Hosting.Server;
using OEMEVWarrantyManagement.API.Models.Response;

namespace OEMEVWarrantyManagement.API
{
    public class ApiExceptionMiddleware
    {
        private readonly RequestDelegate _next;

        public ApiExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
            ResponseError error;

            if (ex is ApiException apiEx)
            {
                error = apiEx.Error;
                context.Response.StatusCode = error.GetAttribute<HttpStatusAttribute>()?.Status ?? 500;
            }
            else
            {
                error = ResponseError.InternalServerError;
                context.Response.StatusCode = 500; // Internal Server Error
            }

            var response = ApiResponse<object>.ErrorResponse(error);

            
            context.Response.ContentType = "application/json";
            return context.Response.WriteAsJsonAsync(response);
        }
    }
}
