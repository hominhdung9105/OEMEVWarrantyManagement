namespace OEMEVWarrantyManagement.Share.Configs
{
    public class EmailSettings
    {
        public string SmtpServer { get; set; } = string.Empty;
        public int Port { get; set; } = 587;
        public string SenderEmail { get; set; } = string.Empty;
        public string? SenderName { get; set; }
        public string Password { get; set; } = string.Empty;
        public bool EnableSsl { get; set; } = true;
        public bool UseDefaultCredentials { get; set; } = false;
    }
}
