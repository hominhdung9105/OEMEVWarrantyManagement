using OEMEVWarrantyManagement.Share.Models.Response;

namespace OEMEVWarrantyManagement.Share.Exceptions
{
    public class ApiException : Exception
    {
        public ResponseError Error { get; }

        public ApiException(ResponseError error) : base(error.GetAttr().Message)
        {
            Error = error;
        }
    }
}
