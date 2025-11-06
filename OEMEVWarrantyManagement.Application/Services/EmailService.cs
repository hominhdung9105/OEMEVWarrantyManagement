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
        private readonly ILogger<EmailService> _logger;
        private readonly IVehicleRepository _vehicleRepository;

        public EmailService(IOptions<EmailSettings> options, ILogger<EmailService> logger, IVehicleRepository vehicleRepository)
        {
            _settings = options.Value;
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
            catch (SmtpFailedRecipientException ex)
            {
                _logger.LogError(ex, "Failed to send email to recipient {Recipient}", to);
                throw;
            }
            catch (SmtpException ex)
            {
                _logger.LogError(ex, "SMTP error while sending email to {Recipient}", to);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while sending email to {Recipient}", to);
                throw;
            }
        }

        public async Task SendAppointmentConfirmationEmailAsync(string to, string customerName, string vin, DateOnly date, string slot, string time, string confirmUrl)
        {
            var subject = "Xác nhận lịch hẹn bảo hành";
            var safeName = System.Net.WebUtility.HtmlEncode(customerName ?? string.Empty);
            var safeVin = System.Net.WebUtility.HtmlEncode(vin ?? string.Empty);
            var safeSlot = System.Net.WebUtility.HtmlEncode(slot ?? string.Empty);
            var safeTime = System.Net.WebUtility.HtmlEncode(time ?? string.Empty);
            var safeUrl = confirmUrl ?? string.Empty;

            var body = $@"
                <div style='font-family: Arial, sans-serif;'>
                    <h2>Xin chào {safeName},</h2>
                    <p>Bạn đã đặt lịch hẹn bảo hành cho xe VIN <strong>{safeVin}</strong>.</p>
                    <p>Thời gian: <strong>{date:dd/MM/yyyy}</strong> - Slot <strong>{safeSlot}</strong> ({safeTime}).</p>
                    <p>Vui lòng nhấn nút dưới đây để xác nhận lịch hẹn.</p>
                    <p>
                        <a href='{safeUrl}' style='display:inline-block;padding:12px 20px;background:#0d6efd;color:#fff;text-decoration:none;border-radius:6px;'>Xác nhận lịch hẹn</a>
                    </p>
                    <p>Nếu bạn không đặt lịch hẹn này, vui lòng bỏ qua email.</p>
                </div>";

            await SendEmailAsync(to, subject, body);
        }

        public async Task SendWarrantyClaimApprovedEmailAsync(string to, string customerName, string vin, Guid claimId, string? policyName)
        {
            var subject = "Chấp nhận yêu cầu bảo hành";
            var safeName = System.Net.WebUtility.HtmlEncode(customerName ?? string.Empty);
            var safeVin = System.Net.WebUtility.HtmlEncode(vin ?? string.Empty);
            var safePolicy = System.Net.WebUtility.HtmlEncode(policyName ?? "");

            var body = $@"
                <div style='font-family: Arial, sans-serif;'>
                    <h2>Xin chào {safeName},</h2>
                    <p>Yêu cầu bảo hành của bạn cho xe VIN <strong>{safeVin}</strong> đã được <strong>chấp nhận</strong>.</p>
                    {(string.IsNullOrWhiteSpace(safePolicy) ? string.Empty : $"<p>Chính sách áp dụng: <strong>{safePolicy}</strong></p>")}
                    <p>Mã yêu cầu: <strong>{claimId}</strong></p>
                    <p>Chúng tôi sẽ sớm liên hệ để sắp xếp lịch sửa chữa.</p>
                </div>";

            await SendEmailAsync(to, subject, body);
        }

        public async Task SendWarrantyClaimDeniedEmailAsync(string to, string customerName, string vin, Guid claimId)
        {
            var subject = "Từ chối yêu cầu bảo hành";
            var safeName = System.Net.WebUtility.HtmlEncode(customerName ?? string.Empty);
            var safeVin = System.Net.WebUtility.HtmlEncode(vin ?? string.Empty);

            var body = $@"
                <div style='font-family: Arial, sans-serif;'>
                    <h2>Xin chào {safeName},</h2>
                    <p>Rất tiếc, yêu cầu bảo hành của bạn cho xe VIN <strong>{safeVin}</strong> đã bị <strong>từ chối</strong>.</p>
                    <p>Mã yêu cầu: <strong>{claimId}</strong></p>
                    <p>Nếu cần hỗ trợ thêm, vui lòng liên hệ trung tâm dịch vụ.</p>
                </div>";

            await SendEmailAsync(to, subject, body);
        }

        public async Task SendWarrantyRepairCompletedEmailAsync(string to, string customerName, string vin, Guid claimId, DateTime? completedAt = null, string? note = null)
        {
            var subject = "Xe của bạn đã sửa xong";
            var safeName = System.Net.WebUtility.HtmlEncode(customerName ?? string.Empty);
            var safeVin = System.Net.WebUtility.HtmlEncode(vin ?? string.Empty);
            var timeStr = completedAt.HasValue ? completedAt.Value.ToString("dd/MM/yyyy HH:mm") : DateTime.Now.ToString("dd/MM/yyyy HH:mm");
            var safeNote = System.Net.WebUtility.HtmlEncode(note ?? string.Empty);

            var body = $@"
                <div style='font-family: Arial, sans-serif;'>
                    <h2>Xin chào {safeName},</h2>
                    <p>Xe VIN <strong>{safeVin}</strong> trong yêu cầu bảo hành <strong>{claimId}</strong> đã được sửa xong.</p>
                    <p>Thời gian hoàn tất: <strong>{timeStr}</strong></p>
                    {(string.IsNullOrWhiteSpace(safeNote) ? string.Empty : $"<p>Ghi chú: {safeNote}</p>")}
                    <p>Vui lòng liên hệ trung tâm dịch vụ để nhận xe.</p>
                </div>";

            await SendEmailAsync(to, subject, body);
        }

        public async Task SendCampaignPartIssueEmailAsync(string to, string customerName, string vin, string partModel, string? campaignTitle = null, string? note = null, string? bookingUrl = null)
        {
            // Subject in English
            var subject = string.IsNullOrWhiteSpace(campaignTitle)
                ? "Service campaign notice for your vehicle"
                : $"{campaignTitle} - Service Campaign Notice";

            // Safe values for HTML
            var safeName = System.Net.WebUtility.HtmlEncode(customerName ?? string.Empty);
            var safeVin = System.Net.WebUtility.HtmlEncode(vin ?? string.Empty);
            var safeModel = System.Net.WebUtility.HtmlEncode(partModel ?? string.Empty);
            var safeTitle = System.Net.WebUtility.HtmlEncode(campaignTitle ?? string.Empty);
            var safeNote = System.Net.WebUtility.HtmlEncode(note ?? string.Empty);
            var safeEmail = System.Net.WebUtility.HtmlEncode(to ?? string.Empty);
            var safeUrl = bookingUrl ?? string.Empty; // don't HTML encode URLs used in href
            var brandName = System.Net.WebUtility.HtmlEncode(_settings?.SenderName ?? "OEM EV Warranty");

            // Vehicle details by VIN
            var vehicle = await _vehicleRepository.GetVehicleByVinAsync(vin);
            var safeVehicleModel = System.Net.WebUtility.HtmlEncode(vehicle?.Model ?? string.Empty);
            var safeVehicleYear = System.Net.WebUtility.HtmlEncode((vehicle?.Year.ToString()) ?? string.Empty);

            // Polished, responsive-ish HTML email with inline styles
            var body = $@"<!doctype html>
<html>
  <head>
    <meta name='viewport' content='width=device-width, initial-scale=1.0'/>
    <meta http-equiv='Content-Type' content='text/html; charset=UTF-8' />
    <title>{subject}</title>
  </head>
  <body style='margin:0;background:#f5f7fb;font-family:Segoe UI,Arial,Helvetica,sans-serif;color:#0f172a;'>
    <div style='width:100%;padding:24px 0;'>
      <table role='presentation' cellspacing='0' cellpadding='0' border='0' align='center' style='width:100%;max-width:640px;margin:0 auto;'>
        <tr>
          <td style='padding:0 16px;'>
            <div style='background:#ffffff;border-radius:12px;overflow:hidden;border:1px solid #e5e7eb;'>
              <div style='background:#0d6efd;color:#ffffff;padding:20px 24px;'>
                <h1 style='margin:0;font-size:20px;font-weight:600;'>{brandName}</h1>
                <p style='margin:4px 0 0 0;font-size:12px;opacity:.9;'>Customer safety and quality matter to us</p>
              </div>

              <div style='padding:24px;'>
                <p style='margin:0 0 12px 0;font-size:16px;'>Hello <strong>{safeName}</strong>,</p>
                <p style='margin:0 0 16px 0;line-height:1.6;'>
                  We are reaching out regarding a service campaign/recall that may affect your vehicle. Our records indicate that the part
                  <strong>{safeModel}</strong> on your vehicle with VIN <strong>{safeVin}</strong> may require inspection and, if necessary, service.
                </p>
                {(string.IsNullOrWhiteSpace(safeTitle) ? string.Empty : $"<p style='margin:0 0 8px 0;'><strong>Campaign:</strong> {safeTitle}</p>")}
                {(string.IsNullOrWhiteSpace(safeNote) ? string.Empty : $"<p style='margin:0 0 16px 0;white-space:pre-line;'><strong>Note:</strong> {safeNote}</p>")}

                <div style='margin:16px 0 0 0;'>
                  <table role='presentation' cellpadding='0' cellspacing='0' border='0' style='width:100%;border-collapse:separate;border-spacing:0;'>
                    <tr>
                      <td style='padding:0 0 12px 0;'>
                        <h3 style='margin:0 0 8px 0;font-size:16px;'>Customer details</h3>
                        <table role='presentation' cellpadding='0' cellspacing='0' border='0' style='width:100%;border:1px solid #e5e7eb;border-radius:8px;overflow:hidden;'>
                          <tr>
                            <td style='background:#f9fafb;padding:10px 12px;width:40%;font-weight:600;'>Name</td>
                            <td style='padding:10px 12px;'>{safeName}</td>
                          </tr>
                          <tr>
                            <td style='background:#f9fafb;padding:10px 12px;width:40%;font-weight:600;'>Email</td>
                            <td style='padding:10px 12px;'>{safeEmail}</td>
                          </tr>
                          <tr>
                            <td style='background:#f9fafb;padding:10px 12px;width:40%;font-weight:600;'>VIN</td>
                            <td style='padding:10px 12px;'>{safeVin}</td>
                          </tr>
                          {(string.IsNullOrWhiteSpace(safeVehicleModel) ? string.Empty : $"<tr><td style='background:#f9fafb;padding:10px 12px;width:40%;font-weight:600;'>Model</td><td style='padding:10px 12px;'>{safeVehicleModel}</td></tr>")}
                          {(string.IsNullOrWhiteSpace(safeVehicleYear) ? string.Empty : $"<tr><td style='background:#f9fafb;padding:10px 12px;width:40%;font-weight:600;'>Year</td><td style='padding:10px 12px;'>{safeVehicleYear}</td></tr>")}
                        </table>
                      </td>
                    </tr>

                    <tr>
                      <td style='padding:4px 0 0 0;'>
                        <h3 style='margin:0 0 8px 0;font-size:16px;'>Campaign details</h3>
                        <table role='presentation' cellpadding='0' cellspacing='0' border='0' style='width:100%;border:1px solid #e5e7eb;border-radius:8px;overflow:hidden;'>
                          <tr>
                            <td style='background:#f9fafb;padding:10px 12px;width:40%;font-weight:600;'>Campaign</td>
                            <td style='padding:10px 12px;'>{(string.IsNullOrWhiteSpace(safeTitle) ? "Service Campaign" : safeTitle)}</td>
                          </tr>
                          <tr>
                            <td style='background:#f9fafb;padding:10px 12px;width:40%;font-weight:600;'>Affected part</td>
                            <td style='padding:10px 12px;'>{safeModel}</td>
                          </tr>
                          {(string.IsNullOrWhiteSpace(safeNote) ? string.Empty : $"<tr><td style='background:#f9fafb;padding:10px 12px;width:40%;font-weight:600;'>Note</td><td style='padding:10px 12px;'>{safeNote}</td></tr>")}
                        </table>
                      </td>
                    </tr>
                  </table>
                </div>

                {(string.IsNullOrWhiteSpace(safeUrl)
                    ? "<p style='margin:16px 0 0 0;'>Please contact your nearest service center to arrange an inspection at your earliest convenience.</p>"
                    : $"<div style='margin:20px 0 0 0;'>\n                      <a href='{safeUrl}' style='display:inline-block;background:#0d6efd;color:#ffffff;text-decoration:none;padding:12px 18px;border-radius:8px;font-weight:600;'>Book an inspection</a>\n                      <p style='margin:8px 0 0 0;font-size:12px;color:#475569;'>If the button does not work, copy and paste this link into your browser: <br/><span style='word-break:break-all;color:#0d6efd;'>{System.Net.WebUtility.HtmlEncode(safeUrl)}</span></p>\n                    </div>")}

                <div style='margin:24px 0 0 0;padding:16px;border:1px dashed #e5e7eb;border-radius:8px;background:#fafafa;'>
                  <p style='margin:0 0 4px 0;'>Best regards,</p>
                  <p style='margin:0;font-weight:600'>{brandName}</p>
                  <p style='margin:4px 0 0 0;font-size:12px;color:#64748b;'>Contact: {System.Net.WebUtility.HtmlEncode(_settings?.SenderEmail ?? string.Empty)}</p>
                </div>

                <p style='margin:20px 0 0 0;font-size:12px;color:#64748b;'>
                  If you did not request or expect this message, you can safely ignore this email.
                </p>
              </div>

              <div style='border-top:1px solid #e5e7eb;padding:16px 24px;background:#fafafa;color:#64748b;font-size:12px;text-align:center;'>
                <p style='margin:0;'>&copy; {DateTime.UtcNow.Year} {brandName}. All rights reserved.</p>
                <p style='margin:4px 0 0 0;'>This is an automated message. Please do not reply. For assistance, contact our support team.</p>
              </div>
            </div>
          </td>
        </tr>
      </table>
    </div>
  </body>
</html>";

            await SendEmailAsync(to, subject, body);
        }
    }
}
