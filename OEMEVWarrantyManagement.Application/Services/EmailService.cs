using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OEMEVWarrantyManagement.Application.IServices;
using OEMEVWarrantyManagement.Application.IRepository;
using OEMEVWarrantyManagement.Share.Configs;
using System.Net;
using System.Net.Mail;
using System.Text;

namespace OEMEVWarrantyManagement.Application.Services
{
    public class EmailService : IEmailService
    {
        private readonly EmailSettings _settings;
        private readonly EmailUrlSettings _urlSettings;
        private readonly ILogger<EmailService> _logger;
        private readonly IVehicleRepository _vehicleRepository;

        public EmailService(
            IOptions<EmailSettings> options, 
            IOptions<EmailUrlSettings> urlOptions,
            ILogger<EmailService> logger, 
            IVehicleRepository vehicleRepository)
        {
            _settings = options.Value;
            _urlSettings = urlOptions.Value;
            _logger = logger;
            _vehicleRepository = vehicleRepository;
        }

        public async Task SendEmailAsync(string to, string subject, string body)
        {
            if (string.IsNullOrWhiteSpace(to)) throw new ArgumentException("Recipient is required", nameof(to));
            if (string.IsNullOrWhiteSpace(subject)) subject = string.Empty;
            if (string.IsNullOrWhiteSpace(body)) body = string.Empty;

            using var smtpClient = new SmtpClient(_settings.SmtpServer)
            {
                Port = _settings.Port,
                EnableSsl = _settings.EnableSsl,
                UseDefaultCredentials = _settings.UseDefaultCredentials,
                Credentials = new NetworkCredential(_settings.SenderEmail, _settings.Password),
                DeliveryMethod = SmtpDeliveryMethod.Network
            };

            using var mailMessage = new MailMessage
            {
                From = new MailAddress(_settings.SenderEmail, _settings.SenderName),
                Subject = subject,
                Body = body,
                IsBodyHtml = true,
                BodyEncoding = Encoding.UTF8,
                SubjectEncoding = Encoding.UTF8
            };

            mailMessage.To.Add(to);

            try
            {
                await smtpClient.SendMailAsync(mailMessage);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending email to {Recipient}", to);
                throw;
            }
        }

        // 1. Appointment Confirmation
        public async Task SendAppointmentConfirmationEmailAsync(string to, string customerName, string vin, DateOnly date, string slot, string time, string confirmUrl)
        {
            var subject = "Appointment Confirmation";

            var details = new Dictionary<string, string>
            {
                { "VIN", vin },
                { "Date", date.ToString("dd/MM/yyyy") },
                { "Slot", $"{slot} ({time})" }
            };

            var mainMessage = $"You have successfully booked a service appointment for your vehicle <strong>{WebUtility.HtmlEncode(vin)}</strong>. Please confirm your attendance below.";

            var body = GenerateEmailTemplate(
                title: "Appointment Scheduled",
                customerName: customerName,
                message: mainMessage,
                details: details,
                ctaUrl: confirmUrl,
                ctaText: "Confirm Appointment"
            );

            await SendEmailAsync(to, subject, body);
        }

        // 2. Appointment Cancelled
        public async Task SendAppointmentCancelledEmailAsync(string to, string customerName, string vin)
        {
            var subject = "Appointment Cancellation Notice";

            var details = new Dictionary<string, string>
            {
                { "VIN", vin },
                { "Status", "Cancelled" }
            };

            var mainMessage = $"Your service appointment for vehicle <strong>{WebUtility.HtmlEncode(vin)}</strong> has been <strong>cancelled</strong>.<br/><br/>If you would like to reschedule, please contact our service center or book a new appointment via our system.";

            var body = GenerateEmailTemplate(
                title: "Appointment Cancelled",
                customerName: customerName,
                message: mainMessage,
                details: details
            );

            await SendEmailAsync(to, subject, body);
        }

        // 3. Appointment No-Show
        public async Task SendAppointmentNoShowEmailAsync(string to, string customerName, string vin, DateOnly date, string slot, string time)
        {
            var subject = "Missed Appointment Notice";

            var details = new Dictionary<string, string>
            {
                { "VIN", vin },
                { "Missed Date", date.ToString("dd/MM/yyyy") },
                { "Slot", $"{slot} ({time})" }
            };

            var mainMessage = $"We missed you at your scheduled service appointment. If you still require service for vehicle <strong>{WebUtility.HtmlEncode(vin)}</strong>, please book a new appointment.";

            // Use GeneralBookingUrl from settings instead of hardcoded "#"
            var bookingUrl = !string.IsNullOrWhiteSpace(_urlSettings.GeneralBookingUrl) 
                ? _urlSettings.GeneralBookingUrl 
                : null;

            var body = GenerateEmailTemplate(
                title: "We Missed You",
                customerName: customerName,
                message: mainMessage,
                details: details,
                ctaText: "Book New Appointment",
                ctaUrl: bookingUrl
            );

            await SendEmailAsync(to, subject, body);
        }

        // 4. Appointment Rescheduled
        public async Task SendAppointmentRescheduledEmailAsync(string to, string customerName, string vin, DateOnly oldDate, string oldSlot, DateOnly newDate, string newSlot, string newTime, string confirmUrl)
        {
            var subject = "Appointment Rescheduled";

            var details = new Dictionary<string, string>
            {
                { "VIN", vin },
                { "Old Schedule", $"{oldDate:dd/MM/yyyy} - Slot {oldSlot}" },
                { "New Schedule", $"{newDate:dd/MM/yyyy} - Slot {newSlot} ({newTime})" }
            };

            var mainMessage = $"Your service appointment for vehicle <strong>{WebUtility.HtmlEncode(vin)}</strong> has been <strong>rescheduled</strong>. Please verify the new details below.";

            var body = GenerateEmailTemplate(
                title: "Schedule Change",
                customerName: customerName,
                message: mainMessage,
                details: details,
                ctaUrl: confirmUrl,
                ctaText: "Confirm New Time"
            );

            await SendEmailAsync(to, subject, body);
        }

        // 5. Warranty Claim Approved
        public async Task SendWarrantyClaimApprovedEmailAsync(string to, string customerName, string vin, Guid claimId, string? policyName)
        {
            var subject = "Warranty Claim Approved";

            var details = new Dictionary<string, string>
            {
                { "Claim ID", claimId.ToString() },
                { "VIN", vin }
            };

            if (!string.IsNullOrEmpty(policyName))
            {
                details.Add("Policy", policyName);
            }

            var mainMessage = $"Good news! Your warranty claim request has been <strong>approved</strong>. We will contact you shortly to arrange the repair schedule.";

            var body = GenerateEmailTemplate(
                title: "Claim Approved",
                customerName: customerName,
                message: mainMessage,
                details: details
            );

            await SendEmailAsync(to, subject, body);
        }

        // 6. Warranty Claim Denied
        public async Task SendWarrantyClaimDeniedEmailAsync(string to, string customerName, string vin, Guid claimId)
        {
            var subject = "Warranty Claim Status Update";

            var details = new Dictionary<string, string>
            {
                { "Claim ID", claimId.ToString() },
                { "VIN", vin },
                { "Status", "Denied" }
            };

            var mainMessage = $"We regret to inform you that your warranty claim request for vehicle <strong>{WebUtility.HtmlEncode(vin)}</strong> has been <strong>denied</strong>. Please contact the service center for more details.";

            var body = GenerateEmailTemplate(
                title: "Claim Denied",
                customerName: customerName,
                message: mainMessage,
                details: details
            );

            await SendEmailAsync(to, subject, body);
        }

        // 7. Repair Completed
        public async Task SendWarrantyRepairCompletedEmailAsync(string to, string customerName, string vin, Guid claimId, DateTime? completedAt = null, string? note = null)
        {
            var subject = "Repair Completed";
            var timeStr = completedAt.HasValue ? completedAt.Value.ToString("dd/MM/yyyy HH:mm") : DateTime.Now.ToString("dd/MM/yyyy HH:mm");

            var details = new Dictionary<string, string>
            {
                { "Claim ID", claimId.ToString() },
                { "VIN", vin },
                { "Completion Time", timeStr }
            };

            if (!string.IsNullOrWhiteSpace(note))
            {
                details.Add("Technician Note", note);
            }

            var mainMessage = $"Your vehicle repair is complete. You can now visit our service center to pick up your vehicle.";

            var body = GenerateEmailTemplate(
                title: "Ready for Pickup",
                customerName: customerName,
                message: mainMessage,
                details: details
            );

            await SendEmailAsync(to, subject, body);
        }

        // 8. Campaign Part Issue (Updated to use the shared generator for consistency)
        public async Task SendCampaignPartIssueEmailAsync(string to, string customerName, string vin, string partModel, string? campaignTitle = null, string? note = null, string? bookingUrl = null)
        {
            var subject = string.IsNullOrWhiteSpace(campaignTitle)
                ? "Service campaign notice for your vehicle"
                : $"{campaignTitle} - Service Campaign Notice";

            // Fetch vehicle info if needed, or just display VIN
            var vehicle = await _vehicleRepository.GetVehicleByVinAsync(vin);
            var modelInfo = vehicle != null ? $"{vehicle.Model} ({vehicle.Year})" : "";

            var details = new Dictionary<string, string>
            {
                { "VIN", vin },
                { "Affected Part", partModel }
            };

            if (!string.IsNullOrEmpty(modelInfo)) details.Add("Vehicle Model", modelInfo);
            if (!string.IsNullOrEmpty(campaignTitle)) details.Add("Campaign", campaignTitle);
            if (!string.IsNullOrEmpty(note)) details.Add("Note", note);

            var mainMessage = $"We are reaching out regarding a service campaign that affects your vehicle. Our records indicate that the part <strong>{WebUtility.HtmlEncode(partModel)}</strong> requires inspection.";

            // Use bookingUrl parameter if provided, otherwise use CampaignBookingUrl from settings
            var finalBookingUrl = bookingUrl ?? _urlSettings.GeneralBookingUrl;
            if (string.IsNullOrWhiteSpace(finalBookingUrl))
            {
                finalBookingUrl = null; // Don't show button if no URL configured
            }

            var body = GenerateEmailTemplate(
                title: string.IsNullOrWhiteSpace(campaignTitle) ? "Service Campaign" : campaignTitle,
                customerName: customerName,
                message: mainMessage,
                details: details,
                ctaUrl: finalBookingUrl,
                ctaText: "Book an Inspection"
            );

            await SendEmailAsync(to, subject, body);
        }

        private string GenerateEmailTemplate(string title, string customerName, string message, Dictionary<string, string> details, string? ctaUrl = null, string? ctaText = null)
        {
            var brandName = WebUtility.HtmlEncode(_settings?.SenderName ?? "OEM EV Warranty");
            var safeName = WebUtility.HtmlEncode(customerName ?? "Customer");
            var safeEmail = WebUtility.HtmlEncode(_settings?.SenderEmail ?? string.Empty);
            var year = DateTime.UtcNow.Year;

            // Build the details table rows
            var tableRows = new StringBuilder();
            foreach (var item in details)
            {
                var safeKey = WebUtility.HtmlEncode(item.Key);
                var safeValue = WebUtility.HtmlEncode(item.Value);
                tableRows.Append($@"
                    <tr>
                        <td style='background:#f9fafb;padding:10px 12px;width:40%;font-weight:600;color:#475569;border-bottom:1px solid #f1f5f9;'>{safeKey}</td>
                        <td style='padding:10px 12px;color:#1e293b;border-bottom:1px solid #f1f5f9;'>{safeValue}</td>
                    </tr>");
            }

            // Build CTA Button if URL is present
            var ctaSection = string.Empty;
            if (!string.IsNullOrWhiteSpace(ctaUrl))
            {
                ctaSection = $@"
                    <div style='margin:24px 0 0 0;text-align:center;'>
                        <a href='{ctaUrl}' style='display:inline-block;background:#0d6efd;color:#ffffff;text-decoration:none;padding:12px 24px;border-radius:6px;font-weight:600;font-size:16px;'>{WebUtility.HtmlEncode(ctaText ?? "View Details")}</a>
                        <p style='margin:12px 0 0 0;font-size:12px;color:#94a3b8;'>If the button above doesn't work, copy and paste this link:<br/><span style='color:#0d6efd;'>{WebUtility.HtmlEncode(ctaUrl)}</span></p>
                    </div>";
            }

            // Full HTML Structure
            return $@"<!doctype html>
<html>
  <head>
    <meta name='viewport' content='width=device-width, initial-scale=1.0'/>
    <meta http-equiv='Content-Type' content='text/html; charset=UTF-8' />
    <title>{WebUtility.HtmlEncode(title)}</title>
    <style>
      body {{ font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif; background-color: #f5f7fb; margin: 0; padding: 0; }}
      .container {{ width: 100%; max-width: 600px; margin: 0 auto; background-color: #ffffff; border-radius: 8px; overflow: hidden; box-shadow: 0 4px 6px rgba(0,0,0,0.05); border: 1px solid #e2e8f0; }}
      .header {{ background-color: #0d6efd; padding: 24px; color: #ffffff; text-align: center; }}
      .content {{ padding: 32px 24px; color: #334155; line-height: 1.6; }}
      .footer {{ background-color: #f8fafc; padding: 20px; text-align: center; font-size: 12px; color: #94a3b8; border-top: 1px solid #e2e8f0; }}
      h1 {{ margin: 0; font-size: 24px; font-weight: 600; }}
      h2 {{ font-size: 18px; color: #1e293b; margin-top: 0; }}
      .details-table {{ width: 100%; border-collapse: separate; border-spacing: 0; border: 1px solid #e2e8f0; border-radius: 8px; overflow: hidden; margin-top: 16px; font-size: 14px; }}
      .details-table tr:last-child td {{ border-bottom: none; }}
    </style>
  </head>
  <body>
    <div style='padding: 40px 0;'>
      <table role='presentation' class='container' cellspacing='0' cellpadding='0' border='0' align='center'>
        <tr>
          <td class='header'>
            <h1>{brandName}</h1>
            <p style='margin: 4px 0 0 0; opacity: 0.9; font-size: 14px;'>{WebUtility.HtmlEncode(title)}</p>
          </td>
        </tr>
        
        <tr>
          <td class='content'>
            <h2>Hello {safeName},</h2>
            <p style='margin-bottom: 20px;'>{message}</p>
            
            <table class='details-table' role='presentation'>
                {tableRows}
            </table>

            {ctaSection}

            <div style='margin-top: 32px; padding-top: 20px; border-top: 1px dashed #cbd5e1; font-size: 14px; color: #64748b;'>
                <p style='margin: 0;'>Best Regards,</p>
                <p style='margin: 4px 0 0 0; font-weight: 600;'>{brandName} Team</p>
                <p style='margin: 4px 0 0 0; font-size: 12px;'>Contact: <a href='mailto:{safeEmail}' style='color:#0d6efd;text-decoration:none;'>{safeEmail}</a></p>
            </div>
          </td>
        </tr>

        <tr>
          <td class='footer'>
            <p style='margin: 0;'>&copy; {year} {brandName}. All rights reserved.</p>
            <p style='margin: 8px 0 0 0;'>This is an automated message, please do not reply directly to this email.</p>
          </td>
        </tr>
      </table>
    </div>
  </body>
</html>";
        }
    }
}

//using Microsoft.Extensions.Logging;
//using Microsoft.Extensions.Options;
//using OEMEVWarrantyManagement.Application.IServices;
//using OEMEVWarrantyManagement.Application.IRepository;
//using OEMEVWarrantyManagement.Share.Configs;
//using System.Net;
//using System.Net.Mail;
//using System.Text;

//namespace OEMEVWarrantyManagement.Application.Services
//{
//    public class EmailService : IEmailService
//    {
//        private readonly EmailSettings _settings;
//        private readonly ILogger<EmailService> _logger;
//        private readonly IVehicleRepository _vehicleRepository;

//        public EmailService(IOptions<EmailSettings> options, ILogger<EmailService> logger, IVehicleRepository vehicleRepository)
//        {
//            _settings = options.Value;
//            _logger = logger;
//            _vehicleRepository = vehicleRepository;
//        }

//        public async Task SendEmailAsync(string to, string subject, string body)
//        {
//            if (string.IsNullOrWhiteSpace(to)) throw new ArgumentException("Recipient is required", nameof(to));
//            if (string.IsNullOrWhiteSpace(subject)) subject = string.Empty;
//            if (string.IsNullOrWhiteSpace(body)) body = string.Empty;

//            using var smtpClient = new SmtpClient(_settings.SmtpServer)
//            {
//                Port = _settings.Port,
//                EnableSsl = _settings.EnableSsl,
//                UseDefaultCredentials = _settings.UseDefaultCredentials,
//                Credentials = new NetworkCredential(_settings.SenderEmail, _settings.Password),
//                DeliveryMethod = SmtpDeliveryMethod.Network
//            };

//            using var mailMessage = new MailMessage
//            {
//                From = new MailAddress(_settings.SenderEmail, _settings.SenderName),
//                Subject = subject,
//                Body = body,
//                IsBodyHtml = true,
//                BodyEncoding = Encoding.UTF8,
//                SubjectEncoding = Encoding.UTF8
//            };

//            mailMessage.To.Add(to);

//            try
//            {
//                await smtpClient.SendMailAsync(mailMessage);
//            }
//            catch (SmtpFailedRecipientException ex)
//            {
//                _logger.LogError(ex, "Failed to send email to recipient {Recipient}", to);
//                throw;
//            }
//            catch (SmtpException ex)
//            {
//                _logger.LogError(ex, "SMTP error while sending email to {Recipient}", to);
//                throw;
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, "Unexpected error while sending email to {Recipient}", to);
//                throw;
//            }
//        }

//        public async Task SendAppointmentConfirmationEmailAsync(string to, string customerName, string vin, DateOnly date, string slot, string time, string confirmUrl)
//        {
//            var subject = "Xác nhận lịch hẹn bảo hành";
//            var safeName = System.Net.WebUtility.HtmlEncode(customerName ?? string.Empty);
//            var safeVin = System.Net.WebUtility.HtmlEncode(vin ?? string.Empty);
//            var safeSlot = System.Net.WebUtility.HtmlEncode(slot ?? string.Empty);
//            var safeTime = System.Net.WebUtility.HtmlEncode(time ?? string.Empty);
//            var safeUrl = confirmUrl ?? string.Empty;

//            var body = $@"
//                <div style='font-family: Arial, sans-serif;'>
//                    <h2>Xin chào {safeName},</h2>
//                    <p>Bạn đã đặt lịch hẹn bảo hành cho xe VIN <strong>{safeVin}</strong>.</p>
//                    <p>Thời gian: <strong>{date:dd/MM/yyyy}</strong> - Slot <strong>{safeSlot}</strong> ({safeTime}).</p>
//                    <p>Vui lòng nhấn nút dưới đây để xác nhận lịch hẹn.</p>
//                    <p>
//                        <a href='{safeUrl}' style='display:inline-block;padding:12px 20px;background:#0d6efd;color:#fff;text-decoration:none;border-radius:6px;'>Xác nhận lịch hẹn</a>
//                    </p>
//                    <p>Nếu bạn không đặt lịch hẹn này, vui lòng bỏ qua email.</p>
//                </div>";

//            await SendEmailAsync(to, subject, body);
//        }

//        public async Task SendAppointmentCancelledEmailAsync(string to, string customerName, string vin)
//        {
//            var subject = "Thông báo hủy lịch hẹn bảo hành";
//            var safeName = System.Net.WebUtility.HtmlEncode(customerName ?? string.Empty);
//            var safeVin = System.Net.WebUtility.HtmlEncode(vin ?? string.Empty);

//            var body = $@"
//                <div style='font-family: Arial, sans-serif;'>
//                    <h2>Xin chào {safeName},</h2>
//                    <p>Lịch hẹn bảo hành của bạn cho xe VIN <strong>{safeVin}</strong> đã được <strong>hủy</strong>.</p>
//                    <p>Nếu bạn muốn đặt lịch hẹn mới, vui lòng liên hệ với trung tâm dịch vụ hoặc đặt lịch qua hệ thống của chúng tôi.</p>
//                    <p>Cảm ơn bạn đã sử dụng dịch vụ của chúng tôi.</p>
//                </div>";

//            await SendEmailAsync(to, subject, body);
//        }

//        public async Task SendAppointmentNoShowEmailAsync(string to, string customerName, string vin, DateOnly date, string slot, string time)
//        {
//            var subject = "Thông báo vắng mặt lịch hẹn bảo hành";
//            var safeName = System.Net.WebUtility.HtmlEncode(customerName ?? string.Empty);
//            var safeVin = System.Net.WebUtility.HtmlEncode(vin ?? string.Empty);
//            var safeSlot = System.Net.WebUtility.HtmlEncode(slot ?? string.Empty);
//            var safeTime = System.Net.WebUtility.HtmlEncode(time ?? string.Empty);

//            var body = $@"
//                <div style='font-family: Arial, sans-serif;'>
//                    <h2>Xin chào {safeName},</h2>
//                    <p>Chúng tôi nhận thấy bạn đã không đến theo lịch hẹn bảo hành cho xe VIN <strong>{safeVin}</strong>.</p>
//                    <p>Thời gian hẹn: <strong>{date:dd/MM/yyyy}</strong> - Slot <strong>{safeSlot}</strong> ({safeTime}).</p>
//                    <p>Nếu bạn cần đặt lại lịch hẹn, vui lòng liên hệ với trung tâm dịch vụ hoặc đặt lịch mới qua hệ thống của chúng tôi.</p>
//                    <p>Cảm ơn bạn đã quan tâm đến dịch vụ của chúng tôi.</p>
//                </div>";

//            await SendEmailAsync(to, subject, body);
//        }

//        public async Task SendAppointmentRescheduledEmailAsync(string to, string customerName, string vin, DateOnly oldDate, string oldSlot, DateOnly newDate, string newSlot, string newTime, string confirmUrl)
//        {
//            var subject = "Thông báo thay đổi lịch hẹn bảo hành";
//            var safeName = System.Net.WebUtility.HtmlEncode(customerName ?? string.Empty);
//            var safeVin = System.Net.WebUtility.HtmlEncode(vin ?? string.Empty);
//            var safeOldSlot = System.Net.WebUtility.HtmlEncode(oldSlot ?? string.Empty);
//            var safeNewSlot = System.Net.WebUtility.HtmlEncode(newSlot ?? string.Empty);
//            var safeNewTime = System.Net.WebUtility.HtmlEncode(newTime ?? string.Empty);
//            var safeUrl = confirmUrl ?? string.Empty;

//            var body = $@"
//                <div style='font-family: Arial, sans-serif;'>
//                    <h2>Xin chào {safeName},</h2>
//                    <p>Lịch hẹn bảo hành của bạn cho xe VIN <strong>{safeVin}</strong> đã được <strong>thay đổi</strong>.</p>
//                    <p><strong>Lịch cũ:</strong> {oldDate:dd/MM/yyyy} - Slot {safeOldSlot}</p>
//                    <p><strong>Lịch mới:</strong> {newDate:dd/MM/yyyy} - Slot {safeNewSlot} ({safeNewTime})</p>
//                    <p>Vui lòng nhấn nút dưới đây để xác nhận lịch hẹn mới.</p>
//                    <p>
//                        <a href='{safeUrl}' style='display:inline-block;padding:12px 20px;background:#0d6efd;color:#fff;text-decoration:none;border-radius:6px;'>Xác nhận lịch hẹn mới</a>
//                    </p>
//                    <p>Nếu bạn có thắc mắc, vui lòng liên hệ với trung tâm dịch vụ.</p>
//                </div>";

//            await SendEmailAsync(to, subject, body);
//        }

//        public async Task SendWarrantyClaimApprovedEmailAsync(string to, string customerName, string vin, Guid claimId, string? policyName)
//        {
//            var subject = "Chấp nhận yêu cầu bảo hành";
//            var safeName = System.Net.WebUtility.HtmlEncode(customerName ?? string.Empty);
//            var safeVin = System.Net.WebUtility.HtmlEncode(vin ?? string.Empty);
//            var safePolicy = System.Net.WebUtility.HtmlEncode(policyName ?? "");

//            var body = $@"
//                <div style='font-family: Arial, sans-serif;'>
//                    <h2>Xin chào {safeName},</h2>
//                    <p>Yêu cầu bảo hành của bạn cho xe VIN <strong>{safeVin}</strong> đã được <strong>chấp nhận</strong>.</p>
//                    {(string.IsNullOrWhiteSpace(safePolicy) ? string.Empty : $"<p>Chính sách áp dụng: <strong>{safePolicy}</strong></p>")}
//                    <p>Mã yêu cầu: <strong>{claimId}</strong></p>
//                    <p>Chúng tôi sẽ sớm liên hệ để sắp xếp lịch sửa chữa.</p>
//                </div>";

//            await SendEmailAsync(to, subject, body);
//        }

//        public async Task SendWarrantyClaimDeniedEmailAsync(string to, string customerName, string vin, Guid claimId)
//        {
//            var subject = "Từ chối yêu cầu bảo hành";
//            var safeName = System.Net.WebUtility.HtmlEncode(customerName ?? string.Empty);
//            var safeVin = System.Net.WebUtility.HtmlEncode(vin ?? string.Empty);

//            var body = $@"
//                <div style='font-family: Arial, sans-serif;'>
//                    <h2>Xin chào {safeName},</h2>
//                    <p>Rất tiếc, yêu cầu bảo hành của bạn cho xe VIN <strong>{safeVin}</strong> đã bị <strong>từ chối</strong>.</p>
//                    <p>Mã yêu cầu: <strong>{claimId}</strong></p>
//                    <p>Nếu cần hỗ trợ thêm, vui lòng liên hệ trung tâm dịch vụ.</p>
//                </div>";

//            await SendEmailAsync(to, subject, body);
//        }

//        public async Task SendWarrantyRepairCompletedEmailAsync(string to, string customerName, string vin, Guid claimId, DateTime? completedAt = null, string? note = null)
//        {
//            var subject = "Xe của bạn đã sửa xong";
//            var safeName = System.Net.WebUtility.HtmlEncode(customerName ?? string.Empty);
//            var safeVin = System.Net.WebUtility.HtmlEncode(vin ?? string.Empty);
//            var timeStr = completedAt.HasValue ? completedAt.Value.ToString("dd/MM/yyyy HH:mm") : DateTime.Now.ToString("dd/MM/yyyy HH:mm");
//            var safeNote = System.Net.WebUtility.HtmlEncode(note ?? string.Empty);

//            var body = $@"
//                <div style='font-family: Arial, sans-serif;'>
//                    <h2>Xin chào {safeName},</h2>
//                    <p>Xe VIN <strong>{safeVin}</strong> trong yêu cầu bảo hành <strong>{claimId}</strong> đã được sửa xong.</p>
//                    <p>Thời gian hoàn tất: <strong>{timeStr}</strong></p>
//                    {(string.IsNullOrWhiteSpace(safeNote) ? string.Empty : $"<p>Ghi chú: {safeNote}</p>")}
//                    <p>Vui lòng liên hệ trung tâm dịch vụ để nhận xe.</p>
//                </div>";

//            await SendEmailAsync(to, subject, body);
//        }

//        public async Task SendCampaignPartIssueEmailAsync(string to, string customerName, string vin, string partModel, string? campaignTitle = null, string? note = null, string? bookingUrl = null)
//        {
//            // Subject in English
//            var subject = string.IsNullOrWhiteSpace(campaignTitle)
//                ? "Service campaign notice for your vehicle"
//                : $"{campaignTitle} - Service Campaign Notice";

//            // Safe values for HTML
//            var safeName = System.Net.WebUtility.HtmlEncode(customerName ?? string.Empty);
//            var safeVin = System.Net.WebUtility.HtmlEncode(vin ?? string.Empty);
//            var safeModel = System.Net.WebUtility.HtmlEncode(partModel ?? string.Empty);
//            var safeTitle = System.Net.WebUtility.HtmlEncode(campaignTitle ?? string.Empty);
//            var safeNote = System.Net.WebUtility.HtmlEncode(note ?? string.Empty);
//            var safeEmail = System.Net.WebUtility.HtmlEncode(to ?? string.Empty);
//            var safeUrl = bookingUrl ?? string.Empty; // don't HTML encode URLs used in href
//            var brandName = System.Net.WebUtility.HtmlEncode(_settings?.SenderName ?? "OEM EV Warranty");

//            // Vehicle details by VIN
//            var vehicle = await _vehicleRepository.GetVehicleByVinAsync(vin);
//            var safeVehicleModel = System.Net.WebUtility.HtmlEncode(vehicle?.Model ?? string.Empty);
//            var safeVehicleYear = System.Net.WebUtility.HtmlEncode((vehicle?.Year.ToString()) ?? string.Empty);

//            // Polished, responsive-ish HTML email with inline styles
//            var body = $@"<!doctype html>
//<html>
//  <head>
//    <meta name='viewport' content='width=device-width, initial-scale=1.0'/>
//    <meta http-equiv='Content-Type' content='text/html; charset=UTF-8' />
//    <title>{subject}</title>
//  </head>
//  <body style='margin:0;background:#f5f7fb;font-family:Segoe UI,Arial,Helvetica,sans-serif;color:#0f172a;'>
//    <div style='width:100%;padding:24px 0;'>
//      <table role='presentation' cellspacing='0' cellpadding='0' border='0' align='center' style='width:100%;max-width:640px;margin:0 auto;'>
//        <tr>
//          <td style='padding:0 16px;'>
//            <div style='background:#ffffff;border-radius:12px;overflow:hidden;border:1px solid #e5e7eb;'>
//              <div style='background:#0d6efd;color:#ffffff;padding:20px 24px;'>
//                <h1 style='margin:0;font-size:20px;font-weight:600;'>{brandName}</h1>
//                <p style='margin:4px 0 0 0;font-size:12px;opacity:.9;'>Customer safety and quality matter to us</p>
//              </div>

//              <div style='padding:24px;'>
//                <p style='margin:0 0 12px 0;font-size:16px;'>Hello <strong>{safeName}</strong>,</p>
//                <p style='margin:0 0 16px 0;line-height:1.6;'>
//                  We are reaching out regarding a service campaign/recall that may affect your vehicle. Our records indicate that the part
//                  <strong>{safeModel}</strong> on your vehicle with VIN <strong>{safeVin}</strong> may require inspection and, if necessary, service.
//                </p>
//                {(string.IsNullOrWhiteSpace(safeTitle) ? string.Empty : $"<p style='margin:0 0 8px 0;'><strong>Campaign:</strong> {safeTitle}</p>")}
//                {(string.IsNullOrWhiteSpace(safeNote) ? string.Empty : $"<p style='margin:0 0 16px 0;white-space:pre-line;'><strong>Note:</strong> {safeNote}</p>")}

//                <div style='margin:16px 0 0 0;'>
//                  <table role='presentation' cellpadding='0' cellspacing='0' border='0' style='width:100%;border-collapse:separate;border-spacing:0;'>
//                    <tr>
//                      <td style='padding:0 0 12px 0;'>
//                        <h3 style='margin:0 0 8px 0;font-size:16px;'>Customer details</h3>
//                        <table role='presentation' cellpadding='0' cellspacing='0' border='0' style='width:100%;border:1px solid #e5e7eb;border-radius:8px;overflow:hidden;'>
//                          <tr>
//                            <td style='background:#f9fafb;padding:10px 12px;width:40%;font-weight:600;'>Name</td>
//                            <td style='padding:10px 12px;'>{safeName}</td>
//                          </tr>
//                          <tr>
//                            <td style='background:#f9fafb;padding:10px 12px;width:40%;font-weight:600;'>Email</td>
//                            <td style='padding:10px 12px;'>{safeEmail}</td>
//                          </tr>
//                          <tr>
//                            <td style='background:#f9fafb;padding:10px 12px;width:40%;font-weight:600;'>VIN</td>
//                            <td style='padding:10px 12px;'>{safeVin}</td>
//                          </tr>
//                          {(string.IsNullOrWhiteSpace(safeVehicleModel) ? string.Empty : $"<tr><td style='background:#f9fafb;padding:10px 12px;width:40%;font-weight:600;'>Model</td><td style='padding:10px 12px;'>{safeVehicleModel}</td></tr>")}
//                          {(string.IsNullOrWhiteSpace(safeVehicleYear) ? string.Empty : $"<tr><td style='background:#f9fafb;padding:10px 12px;width:40%;font-weight:600;'>Year</td><td style='padding:10px 12px;'>{safeVehicleYear}</td></tr>")}
//                        </table>
//                      </td>
//                    </tr>

//                    <tr>
//                      <td style='padding:4px 0 0 0;'>
//                        <h3 style='margin:0 0 8px 0;font-size:16px;'>Campaign details</h3>
//                        <table role='presentation' cellpadding='0' cellspacing='0' border='0' style='width:100%;border:1px solid #e5e7eb;border-radius:8px;overflow:hidden;'>
//                          <tr>
//                            <td style='background:#f9fafb;padding:10px 12px;width:40%;font-weight:600;'>Campaign</td>
//                            <td style='padding:10px 12px;'>{(string.IsNullOrWhiteSpace(safeTitle) ? "Service Campaign" : safeTitle)}</td>
//                          </tr>
//                          <tr>
//                            <td style='background:#f9fafb;padding:10px 12px;width:40%;font-weight:600;'>Affected part</td>
//                            <td style='padding:10px 12px;'>{safeModel}</td>
//                          </tr>
//                          {(string.IsNullOrWhiteSpace(safeNote) ? string.Empty : $"<tr><td style='background:#f9fafb;padding:10px 12px;width:40%;font-weight:600;'>Note</td><td style='padding:10px 12px;'>{safeNote}</td></tr>")}
//                        </table>
//                      </td>
//                    </tr>
//                  </table>
//                </div>

//                {(string.IsNullOrWhiteSpace(safeUrl)
//                    ? "<p style='margin:16px 0 0 0;'>Please contact your nearest service center to arrange an inspection at your earliest convenience.</p>"
//                    : $"<div style='margin:20px 0 0 0;'>\n                      <a href='{safeUrl}' style='display:inline-block;background:#0d6efd;color:#ffffff;text-decoration:none;padding:12px 18px;border-radius:8px;font-weight:600;'>Book an inspection</a>\n                      <p style='margin:8px 0 0 0;font-size:12px;color:#475569;'>If the button does not work, copy and paste this link into your browser: <br/><span style='word-break:break-all;color:#0d6efd;'>{System.Net.WebUtility.HtmlEncode(safeUrl)}</span></p>\n                    </div>")}

//                <div style='margin:24px 0 0 0;padding:16px;border:1px dashed #e5e7eb;border-radius:8px;background:#fafafa;'>
//                  <p style='margin:0 0 4px 0;'>Best regards,</p>
//                  <p style='margin:0;font-weight:600'>{brandName}</p>
//                  <p style='margin:4px 0 0 0;font-size:12px;color:#64748b;'>Contact: {System.Net.WebUtility.HtmlEncode(_settings?.SenderEmail ?? string.Empty)}</p>
//                </div>

//                <p style='margin:20px 0 0 0;font-size:12px;color:#64748b;'>
//                  If you did not request or expect this message, you can safely ignore this email.
//                </p>
//              </div>

//              <div style='border-top:1px solid #e5e7eb;padding:16px 24px;background:#fafafa;color:#64748b;font-size:12px;text-align:center;'>
//                <p style='margin:0;'>&copy; {DateTime.UtcNow.Year} {brandName}. All rights reserved.</p>
//                <p style='margin:4px 0 0 0;'>This is an automated message. Please do not reply. For assistance, contact our support team.</p>
//              </div>
//            </div>
//          </td>
//        </tr>
//      </table>
//    </div>
//  </body>
//</html>";

//            await SendEmailAsync(to, subject, body);
//        }
//    }
//}
