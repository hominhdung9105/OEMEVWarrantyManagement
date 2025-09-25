using OEMEVWarrantyManagement.Share.Models.Response;

namespace OEMEVWarrantyManagement.Share.Exceptions
{
    public class ApiException : System.Exception
    {
        public ResponseError Error { get; }

        public ApiException(ResponseError error)
            : base(error.GetAttribute<MessageAttribute>()?.Message ?? error.ToString())
        {
            Error = error;
        }
    }
}
