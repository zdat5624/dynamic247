using MailKit.Net.Smtp;
using MimeKit;

namespace NewsPage.helpers
{
    public class MailHelper
    {
        private readonly IConfiguration _configuration;
        private string subject;
        private string body;

        public MailHelper(IConfiguration configuration)
        {
            _configuration = configuration;
            body = "<h1>Xác thực bắc buộc</h1><p>Mã OTP của bạn là: <strong>{0}</strong></p><p>Lưu ý mã có hiệu lực trong vòng 1 phút</p>";
        }

        public void ConfigEmail(string subject, string toEmail, string otp)
        {
            this.subject = subject;
            SendEmailAsync(toEmail, otp).Wait();
        }

        public async Task SendEmailAsync(string toEmail, string otp)
        {
            var emailSettings = _configuration.GetSection("EmailSettings");
            var message = new MimeMessage();

            message.From.Add(new MailboxAddress("Dynamic247 supporter", emailSettings["SenderEmail"]));
            message.To.Add(new MailboxAddress("", toEmail));
            message.Subject = subject;

            var bodyBuilder = new BodyBuilder { HtmlBody = string.Format(body, otp) };
            message.Body = bodyBuilder.ToMessageBody();

            using (var client = new SmtpClient())
            {
                await client.ConnectAsync(emailSettings["SmtpServer"], int.Parse(emailSettings["Port"]), MailKit.Security.SecureSocketOptions.StartTls);
                await client.AuthenticateAsync(emailSettings["SenderEmail"], emailSettings["SenderPassword"]);
                await client.SendAsync(message);
                await client.DisconnectAsync(true);
            }
        }
    }
}
