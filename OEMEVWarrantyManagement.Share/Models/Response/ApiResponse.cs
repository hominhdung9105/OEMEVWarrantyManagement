namespace OEMEVWarrantyManagement.Share.Models.Response
{
    public enum ResponseError
    {
        [ResponseErrorAttr("Invalid username or password.", 400, 1)]
        InvalidAccount,

        [ResponseErrorAttr("Internal server error.", 500, 2)]
        InternalServerError,

        [ResponseErrorAttr("Invalid token.", 401, 3)]
        AuthenticationFailed,

        [ResponseErrorAttr("Access token is required.", 401, 4)]
        AccessTokenRequired,

        [ResponseErrorAttr("Refresh token is required.", 401, 5)]
        RefreshTokenRequired,

        [ResponseErrorAttr("Invalid refresh token.", 400, 6)]
        InvalidRefreshToken,

        [ResponseErrorAttr("Employee not found.", 404, 7)]
        NotFoundEmployee,

        [ResponseErrorAttr("You do not have permission to access this resource.", 403, 8)]
        Forbidden,

        [ResponseErrorAttr("Invalid user id.", 400, 9)]
        InvalidUserId,

        [ResponseErrorAttr("Not found Vin.", 404, 10)]
        NotfoundVin,

        [ResponseErrorAttr("Not found Warranty Request.", 404, 11)]
        NotfoundWarrantyRequest,

        [ResponseErrorAttr("Not found Warranty Record.", 404, 12)]
        NotfoundWarrantyRecord,

        [ResponseErrorAttr("Username already exists.", 400, 13)]
        UsernameAlreadyExists,

        [ResponseErrorAttr("Car condition was created.", 400, 14)]
        DuplicateCarCondition,

        [ResponseErrorAttr("Invalid update request.", 400, 15)]
        InvalidUpdateCarCondition,

        [ResponseErrorAttr("Car condition not found.", 404, 16)]
        NotFoundCarCondition,

        [ResponseErrorAttr("Employee is technical.", 400, 17)]
        EmployeeNotTech,

        [ResponseErrorAttr("Not Found Warranty Claim.", 404, 18)]
        NotFoundWarrantyClaim,

        [ResponseErrorAttr("Invalid Warranty Claim Id.", 400, 19)]
        InvalidWarrantyClaimId,

        [ResponseErrorAttr("You are not assigned to this Warranty Claim.", 403, 20)]
        NotYourWarrantyClaim,

        [ResponseErrorAttr("Invalid image file.", 400, 21)]
        InvalidImage,

        [ResponseErrorAttr("Image size exceeds the limit of 5MB.", 400, 22)]
        ImageSizeToLarge,

        [ResponseErrorAttr("Failed to delete image.", 400, 23)]
        DeleteImageFail,

        [ResponseErrorAttr("Claim attachment not found.", 404, 24)]
        NotFoundClaimAttachment,

        [ResponseErrorAttr("ImageKit service error.", 500, 25)]
        ImageKitError,

        [ResponseErrorAttr("Failed to upload image.", 500, 26)]
        UploadImageFail,

        [ResponseErrorAttr("Not found any part in here.", 404, 27)]
        NotFoundPartHere,

        [ResponseErrorAttr("Invalid description.", 400, 28)]
        InvalidDescription,

        [ResponseErrorAttr("Invalid body", 400, 29)]
        InvalidJsonFormat,

        [ResponseErrorAttr("Task not found.", 404, 30)]
        NotFoundWorkOrder,

        [ResponseErrorAttr("Invalid category.", 400, 31)]
        InvalidPartCategory,

        [ResponseErrorAttr("Vehicle part not found.", 404, 32)]
        NotFoundVehiclePart,

        [ResponseErrorAttr("Claim part not found.", 404, 33)]
        NotFoundClaimPart,

        [ResponseErrorAttr("Invalid part model.", 400, 34)]
        InvalidPartModel,

        [ResponseErrorAttr("Invalid vehicle policy id.", 400, 35)]
        InvalidVehiclePolicyId,

        [ResponseErrorAttr("Invalid policy", 400, 36)]
        InvalidPolicy,

        [ResponseErrorAttr("Invalid claim part id.", 400, 37)]
        InvalidClaimPartId,

        [ResponseErrorAttr("Invalid orderId.", 400, 38)]
        InvalidOrderId,

        [ResponseErrorAttr("Not enough stock.", 400, 39)]
        NotEnoughStock
    }

    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public int Code { get; set; }
        public string Message { get; set; }
        public T? Data { get; set; }

        public static ApiResponse<T> Ok(T data, string message)
        {
            return new ApiResponse<T>
            {
                Success = true,
                Code = 0,
                Message = message,
                Data = data
            };
        }

        public static ApiResponse<T> Fail(ResponseError error)
        {
            var attr = error.GetAttr();
            return new ApiResponse<T>
            {
                Success = false,
                Code = attr.Code,
                Message = attr.Message,
                Data = default
            };
        }
    }

    public static class ResponseErrorExtensions
    {
        public static ResponseErrorAttr GetAttr(this ResponseError error)
        {
            var memberInfo = typeof(ResponseError).GetField(error.ToString());
            return (ResponseErrorAttr)Attribute.GetCustomAttribute(memberInfo, typeof(ResponseErrorAttr));
        }
    }

    [AttributeUsage(AttributeTargets.Field)]
    public class ResponseErrorAttr(string message, int httpStatus, int code) : Attribute
    {
        public string Message { get; } = message;
        public int HttpStatus { get; } = httpStatus;
        public int Code { get; } = code;
    }
}
