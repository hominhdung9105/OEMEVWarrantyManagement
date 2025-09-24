
namespace OEMEVWarrantyManagement.API.Models.Response
{
    public enum ResponseError
    {
        [Message("Invalid username or password.")]
        [HttpStatus(400)]
        [Code(1)]
        InvalidAccount,
        [Message("Internal server error.")]
        [HttpStatus(500)]
        [Code(2)]
        InternalServerError,
        [Message("Invalid token.")]
        [HttpStatus(401)]
        [Code(3)]
        AuthenticationFailed,
        [Message("Access token is required.")]
        [HttpStatus(400)]
        [Code(4)]
        AccessTokenRequired,
        [Message("Refresh token is required.")]
        [HttpStatus(400)]
        [Code(5)]
        RefreshTokenRequired,
        [Message("Invalid request token.")]
        [HttpStatus(400)]
        [Code(6)]
        InvalidRequestToken,
        [Message("Token has expired.")]
        [HttpStatus(401)]
        [Code(7)]
        TokenExpired,
        [Message("You do not have permission to access this resource.")]
        [HttpStatus(403)]
        [Code(8)]
        Forbidden,
        [Message("Invalid user id.")]
        [HttpStatus(400)]
        [Code(9)]
        InvalidUserId
    }

    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public int Code { get; set; }
        public T? Data { get; set; }
        public ApiResponse() { }
        public ApiResponse(bool success, string message, int code, T? data)
        {
            Success = success;
            Message = message;
            Code = code;
            Data = data;
        }

        public static ApiResponse<T> SuccessResponse(T? data, string message)
        {
            return new ApiResponse<T>(true, message, 0, data);
        }

        public static ApiResponse<T> ErrorResponse(ResponseError res)
        {
            var msg = res.GetAttribute<MessageAttribute>().Message;
            var code = res.GetAttribute<CodeAttribute>().Code;

            return new ApiResponse<T>(false, msg, code, default);
        }
    }

    public static class EnumExtensions
    {
        public static TAttribute GetAttribute<TAttribute>(this Enum value)
        where TAttribute : Attribute
        {
            var enumType = value.GetType();
            var name = Enum.GetName(enumType, value);
            return enumType.GetField(name).GetCustomAttributes(false).OfType<TAttribute>().SingleOrDefault();
        }
    }

    class MessageAttribute : Attribute
    {
        public string Message { get; private set; }

        public MessageAttribute(string message)
        {
            this.Message = message;
        }
    }

    class CodeAttribute : Attribute
    {
        public int Code { get; private set; }

        public CodeAttribute(int code)
        {
            this.Code = code;
        }
    }

    class HttpStatusAttribute : Attribute
    {
        public int Status { get; private set; }

        public HttpStatusAttribute(int status)
        {
            this.Status = status;
        }
    }
}
