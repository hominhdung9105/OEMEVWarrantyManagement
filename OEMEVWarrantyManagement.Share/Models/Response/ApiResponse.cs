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
        NotEnoughStock,

        [ResponseErrorAttr("Not found appointment.", 400, 40)]
        NotFoundAppointment,

        // New specific errors
        [ResponseErrorAttr("Campaign not found.", 404, 41)]
        NotFoundCampaign,

        [ResponseErrorAttr("Campaign vehicle not found.", 404, 42)]
        NotFoundCampaignVehicle,

        [ResponseErrorAttr("Invalid campaign type.", 400, 43)]
        InvalidCampaignType,

        [ResponseErrorAttr("Campaign vehicle already exists.", 400, 44)]
        DuplicateCampaignVehicle,

        [ResponseErrorAttr("Invalid campaign vehicle status.", 400, 45)]
        InvalidCampaignVehicleStatus,

        [ResponseErrorAttr("Invalid technician list.", 400, 46)]
        InvalidTechnicianList,

        [ResponseErrorAttr("Invalid warranty claim status for creating work orders.", 400, 47)]
        InvalidWarrantyClaimStatus,

        [ResponseErrorAttr("Invalid work order target.", 400, 48)]
        InvalidWorkOrderTarget,

        // New error for duplicate active warranty claim (VIN already has non-done claim)
        [ResponseErrorAttr("This VIN already has an active warranty claim.", 400, 49)]
        DuplicateActiveWarrantyClaim,

        // Google login errors
        [ResponseErrorAttr("Invalid Google token.", 401, 50)]
        InvalidGoogleToken,

        [ResponseErrorAttr("Google login failed.", 500, 51)]
        GoogleLoginFailed,

        [ResponseErrorAttr("Employee not found.", 404, 52)]
        EmployeeNotFound,

        // Warranty policy errors
        [ResponseErrorAttr("Warranty policy not found.", 404, 53)]
        NotFoundWarrantyPolicy,

        [ResponseErrorAttr("Not Found that part.", 404, 54)]
        NotFoundThatPart,

        // Part Order Shipment errors
        [ResponseErrorAttr("Part order quantity exceeds maximum allowed per part.", 400, 55)]
        PartOrderQuantityExceedsMax,

        [ResponseErrorAttr("Invalid CSV file format.", 400, 56)]
        InvalidCsvFormat,

        [ResponseErrorAttr("Shipment validation failed. Check errors for details.", 400, 57)]
        ShipmentValidationFailed,

        [ResponseErrorAttr("Serial number not found in stock.", 400, 58)]
        SerialNotInStock,

        [ResponseErrorAttr("Serial number does not match the part model.", 400, 59)]
        SerialModelMismatch,

        [ResponseErrorAttr("Duplicate serial number in shipment.", 400, 60)]
        DuplicateSerial,

        [ResponseErrorAttr("Part order has not been confirmed yet.", 400, 61)]
        OrderNotConfirmed,

        [ResponseErrorAttr("Part order shipment has not been validated yet.", 400, 62)]
        ShipmentNotValidated,

        [ResponseErrorAttr("Receipt validation failed. Check errors for details.", 400, 63)]
        ReceiptValidationFailed,

        [ResponseErrorAttr("Serial number was not shipped in this order.", 400, 64)]
        SerialNotShipped,

        [ResponseErrorAttr("Part order has not been shipped yet.", 400, 65)]
        OrderNotShipped,

        [ResponseErrorAttr("Receipt has not been validated yet.", 400, 66)]
        ReceiptNotValidated,

        [ResponseErrorAttr("Part order is not in transit status.", 400, 67)]
        OrderNotInTransit,

        [ResponseErrorAttr("Shipment file has already been uploaded for this order.", 400, 68)]
        ShipmentAlreadyUploaded,

        // Part Order Issue errors
        [ResponseErrorAttr("Invalid issue reason.", 400, 69)]
        InvalidIssueReason,

        [ResponseErrorAttr("Reason detail is required when reason is 'Other'.", 400, 70)]
        ReasonDetailRequired,

        [ResponseErrorAttr("Order cannot be cancelled in current status.", 400, 71)]
        CannotCancelOrder,

        [ResponseErrorAttr("Order cannot be returned in current status.", 400, 72)]
        CannotReturnOrder,

        [ResponseErrorAttr("Service center not found.", 404, 73)]
        ServiceCenterNotFound,

        [ResponseErrorAttr("Order has no discrepancy to resolve.", 400, 74)]
        NoDiscrepancyToResolve,

        [ResponseErrorAttr("Invalid responsible party.", 400, 75)]
        InvalidResponsibleParty,

        [ResponseErrorAttr("Order is not in return inspection status.", 400, 76)]
        OrderNotInReturnInspection,

        [ResponseErrorAttr("Cannot create order item. Order must be in Pending status.", 400, 77)]
        CannotCreateOrderItemInvalidStatus,

        [ResponseErrorAttr("Invalid status value.", 400, 78)]
        InvalidStatus,

        // Warranty Claim Denial errors
        [ResponseErrorAttr("Invalid denial reason.", 400, 79)]
        InvalidDenialReason,

        [ResponseErrorAttr("Reason detail is required when reason is 'Other'.", 400, 80)]
        DenialReasonDetailRequired,

        // Technician Reassignment errors
        [ResponseErrorAttr("Technician count must match the current assigned count.", 400, 81)]
        TechnicianCountMismatch,

        [ResponseErrorAttr("Technician must be from the same service center.", 403, 82)]
        TechnicianNotInSameServiceCenter,

        [ResponseErrorAttr("Part order is not deliveried.", 400, 83)]
        OrderNotDeliveried,
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

        public static ApiResponse<T> Fail(ResponseError error, string customMessage)
        {
            var attr = error.GetAttr();
            return new ApiResponse<T>
            {
                Success = false,
                Code = attr.Code,
                Message = customMessage,
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
