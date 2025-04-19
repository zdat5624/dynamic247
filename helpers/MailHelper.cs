using MailKit.Net.Smtp;
using MimeKit;

namespace NewsPage.helpers
{
    public class MailHelper
    {
        private readonly IConfiguration _configuration;
        private string _subject;
        private string _body;
        private readonly string _otpTemplate;

        public MailHelper(IConfiguration configuration)
        {
            _configuration = configuration;
            _otpTemplate = "<h1>Xác thực bắc buộc</h1><p>Mã OTP của bạn là: <strong>{0}</strong></p><p>Lưu ý mã có hiệu lực trong vòng 1 phút</p>";
        }

        public void ConfigEmail(string subject, string toEmail, string otp)
        {
            _subject = subject;
            _body = string.Format(_otpTemplate, otp);
            SendEmailAsync(toEmail).Wait();
        }

        public async Task SendEmailAsync(string toEmail)
        {
            var emailSettings = _configuration.GetSection("EmailSettings");
            var message = new MimeMessage();
            
            message.From.Add(new MailboxAddress("Dynamic247 supporter", emailSettings["SenderEmail"]));
            message.To.Add(new MailboxAddress("", toEmail));
            message.Subject = _subject;
            
            var bodyBuilder = new BodyBuilder { HtmlBody = _body };
            message.Body = bodyBuilder.ToMessageBody();
            
            using (var client = new SmtpClient())
            {
                await client.ConnectAsync(emailSettings["SmtpServer"], int.Parse(emailSettings["Port"]), MailKit.Security.SecureSocketOptions.StartTls);
                await client.AuthenticateAsync(emailSettings["SenderEmail"], emailSettings["SenderPassword"]);
                await client.SendAsync(message);
                await client.DisconnectAsync(true);
            }
        }

        public async Task SendOtpEmailAsync(string toEmail, string subject, string otp)
        {
            _subject = subject;
            _body = string.Format(_otpTemplate, otp);
            await SendEmailAsync(toEmail);
        }

        public async Task SendEmailToMultipleRecipientsAsync(List<string> userEmails, string subject, string body)
        {
            _subject = subject;
            _body = body;
            
            foreach (string email in userEmails)
            {
                await SendEmailAsync(email);
            }
        }
    }
}