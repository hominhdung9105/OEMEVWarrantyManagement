namespace OEMEVWarrantyManagement.Application.IServices
{
    public interface IEmailService
    {
        Task SendEmailAsync(string to, string subject, string body);
        // Appointment specific email
        Task SendAppointmentConfirmationEmailAsync(string to, string customerName, string vin, DateOnly date, string slot, string time, string confirmUrl);
        Task SendAppointmentCancelledEmailAsync(string to, string customerName, string vin);
        Task SendAppointmentNoShowEmailAsync(string to, string customerName, string vin, DateOnly date, string slot, string time);
        Task SendAppointmentRescheduledEmailAsync(string to, string customerName, string vin, DateOnly oldDate, string oldSlot, DateOnly newDate, string newSlot, string newTime, string confirmUrl);
        Task SendWarrantyClaimApprovedEmailAsync(string to, string customerName, string vin, Guid claimId, string? policyName);
        Task SendWarrantyClaimDeniedEmailAsync(string to, string customerName, string vin, Guid claimId, string? denialReason = null, string? denialReasonDetail = null);
        Task SendWarrantyRepairCompletedEmailAsync(string to, string customerName, string vin, Guid claimId, DateTime? completedAt = null, string? note = null);
        Task SendCampaignPartIssueEmailAsync(string to, string customerName, string vin, string partModel, string? campaignTitle = null, string? note = null, string? bookingUrl = null);
        Task SendWarrantyClaimStatusChangedEmailAsync(string to, string customerName, string vin, Guid claimId, string newStatus, string? notes = null);
    }
}
