using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

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
        // Warranty claim approved email
        Task SendWarrantyClaimApprovedEmailAsync(string to, string customerName, string vin, Guid claimId, string? policyName);
        // Warranty claim denied email
        Task SendWarrantyClaimDeniedEmailAsync(string to, string customerName, string vin, Guid claimId);
        // Warranty repair completed email
        Task SendWarrantyRepairCompletedEmailAsync(string to, string customerName, string vin, Guid claimId, DateTime? completedAt = null, string? note = null);
        // Campaign part issue / recall notification email
        Task SendCampaignPartIssueEmailAsync(string to, string customerName, string vin, string partModel, string? campaignTitle = null, string? note = null, string? bookingUrl = null);
        // New: Generic warranty claim status changed email (for statuses without dedicated template)
        Task SendWarrantyClaimStatusChangedEmailAsync(string to, string customerName, string vin, Guid claimId, string newStatus, string? notes = null);
    }
}
