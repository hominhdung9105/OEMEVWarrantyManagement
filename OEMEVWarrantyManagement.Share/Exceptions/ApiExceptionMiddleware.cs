using Microsoft.AspNetCore.Http;
using OEMEVWarrantyManagement.Share.Models.Response;
using System.Text.Json;

namespace OEMEVWarrantyManagement.Share.Exceptions
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
            catch (ApiException ex)
            {
                var attr = ex.Error.GetAttr();

                context.Response.StatusCode = attr.HttpStatus;
                context.Response.ContentType = "application/json";

                var response = new ApiResponse<object>
                {
                    Success = false,
                    Code = attr.Code,
                    Message = attr.Message,
                    Data = null
                };

                var json = JsonSerializer.Serialize(response);
                await context.Response.WriteAsync(json);
            }
            catch (Exception)
            {
                var attr = ResponseError.InternalServerError.GetAttr();

                context.Response.StatusCode = attr.HttpStatus;
                context.Response.ContentType = "application/json";

                var response = new ApiResponse<object>
                {
                    Success = false,
                    Code = attr.Code,
                    Message = attr.Message,
                    Data = null
                };

                var json = JsonSerializer.Serialize(response);
                await context.Response.WriteAsync(json);
            }
        }
    }
}
