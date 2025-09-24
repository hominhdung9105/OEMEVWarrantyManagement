namespace OEMEVWarrantyManagement.Exception
{
    public class ApiException : Exception
    {
        public ResponseError Error { get; }
        public int StatusCode { get; }

        public ApiException(ResponseError error, int statusCode = 400)
            : base(error.GetAttribute<MessageAttribute>()?.Message ?? error.ToString())
        {
            Error = error;
            StatusCode = statusCode;
        }
    }

    public enum ResponseError
    {
        [Message("Invalid username or password.")]
        [Code(1)]
        InvalidAccount,
        [Message("Nay la` vi du.")]
        [Code(2)]
        Example
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
}
