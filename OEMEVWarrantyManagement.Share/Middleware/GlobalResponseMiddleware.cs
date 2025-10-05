using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using OEMEVWarrantyManagement.Share.Exceptions;
using OEMEVWarrantyManagement.Share.Models.Response;

namespace OEMEVWarrantyManagement.Share.Middleware
{
    public class GlobalResponseMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalResponseMiddleware> _logger;

        public GlobalResponseMiddleware(RequestDelegate next, ILogger<GlobalResponseMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                // Kiểm tra request body JSON hợp lệ trước khi vào controller
                if (IsJsonRequest(context.Request))
                {
                    context.Request.EnableBuffering();

                    using var reader = new StreamReader(context.Request.Body, leaveOpen: true);
                    var body = await reader.ReadToEndAsync();
                    context.Request.Body.Position = 0;

                    if (!string.IsNullOrWhiteSpace(body))
                    {
                        try
                        {
                            JsonDocument.Parse(body);
                        }
                        catch (JsonException)
                        {
                            await WriteJson(context,
                                ApiResponse<object>.Fail(ResponseError.InvalidJsonFormat),
                                StatusCodes.Status400BadRequest);
                            return;
                        }
                    }
                }

                // Tiếp tục pipeline
                await _next(context);
            }
            catch (ApiException ex)
            {
                var attr = ex.Error.GetAttr();

                await WriteJson(context, ApiResponse<object>.Fail(ex.Error), attr.HttpStatus);
            }
            catch (JsonException)
            {
                await WriteJson(context, ApiResponse<object>.Fail(ResponseError.InvalidJsonFormat), StatusCodes.Status400BadRequest);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception");

                var attr = ResponseError.InternalServerError.GetAttr();

                var response = new ApiResponse<object>
                {
                    Success = false,
                    Code = attr.Code,
                    Message = ex.Message, // TODO - ra san pham doi thanh attr.Message
                    Data = null
                };

                await WriteJson(context, response, attr.HttpStatus);
            }
        }

        private static bool IsJsonRequest(HttpRequest request)
        {
            if (request.ContentType == null) return false;

            return request.ContentType.Contains("application/json", StringComparison.OrdinalIgnoreCase)
                   && (request.Method == HttpMethods.Post
                       || request.Method == HttpMethods.Put
                       || request.Method == HttpMethods.Patch);
        }

        private static async Task WriteJson(HttpContext context, object response, int statusCode)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = statusCode;
            await context.Response.WriteAsJsonAsync(response);
        }
    }

}