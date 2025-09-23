using OEMEVWarrantyManagement.API.Models.Response;

namespace OEMEVWarrantyManagement.API
{
    public class ApiException : Exception
    {
        public ResponseError Error { get; }

        public ApiException(ResponseError error)
            : base(error.GetAttribute<MessageAttribute>()?.Message ?? error.ToString())
        {
            Error = error;
        }
    }
}
