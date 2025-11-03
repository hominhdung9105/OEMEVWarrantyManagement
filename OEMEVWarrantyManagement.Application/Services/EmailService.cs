using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OEMEVWarrantyManagement.Application.IServices;
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

        public EmailService(IOptions<EmailSettings> options, ILogger<EmailService> logger)
        {
            _settings = options.Value;
            _logger = logger;
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
            var subject = "Thông báo kiểm tra xe theo chiến dịch";
            var safeName = System.Net.WebUtility.HtmlEncode(customerName ?? string.Empty);
            var safeVin = System.Net.WebUtility.HtmlEncode(vin ?? string.Empty);
            var safeModel = System.Net.WebUtility.HtmlEncode(partModel ?? string.Empty);
            var safeTitle = System.Net.WebUtility.HtmlEncode(campaignTitle ?? string.Empty);
            var safeNote = System.Net.WebUtility.HtmlEncode(note ?? string.Empty);
            var safeUrl = bookingUrl ?? string.Empty;

            var body = $@"
                <div style='font-family: Arial, sans-serif;'>
                    <h2>Xin chào {safeName},</h2>
                    {(string.IsNullOrWhiteSpace(safeTitle) ? string.Empty : $"<p><strong>{safeTitle}</strong></p>")}
                    <p>Hệ thống ghi nhận bộ phận <strong>{safeModel}</strong> trên xe VIN <strong>{safeVin}</strong> có khả năng gặp lỗi theo chiến dịch/recall.</p>
                    {(string.IsNullOrWhiteSpace(safeNote) ? string.Empty : $"<p>{safeNote}</p>")}
                    {(string.IsNullOrWhiteSpace(safeUrl) ? "<p>Vui lòng liên hệ trung tâm dịch vụ để đặt lịch kiểm tra sớm nhất.</p>" : $"<p>Vui lòng đặt lịch kiểm tra tại đây: <a href='{safeUrl}'>Đặt lịch</a></p>")}
                </div>";

            await SendEmailAsync(to, subject, body);
        }
    }
}
