using MailKit.Net.Smtp;
using MimeKit;
using MimeKit.Text;
using shopping_bag.Models;
using shopping_bag.Models.Email;

namespace shopping_bag.Services
{
    public class EmailService : IEmailService
    {
        private readonly string _fromAddress;
        private readonly string _serverAddress;
        private readonly int _serverPort;
        private readonly string _serverUser;
        private readonly string _serverPassword;

        public EmailService(IConfiguration config)
        {
            var smtpSettings = config.GetSection("Smtp");

            _fromAddress = smtpSettings.GetValue<string>("FromAddress");
            _serverAddress = smtpSettings.GetValue<string>("ServerAddress");
            _serverPort = smtpSettings.GetValue<int>("ServerPort");
            _serverUser = smtpSettings.GetValue<string>("ServerUser");
            _serverPassword = smtpSettings.GetValue<string>("ServerPassword");
        }

        public ServiceResponse<bool> SendEmail(Email email)
        {
            try
            {
                var newEmail = new MimeMessage();
                newEmail.From.Add(MailboxAddress.Parse(_fromAddress));
                newEmail.To.Add(MailboxAddress.Parse(email.To));
                newEmail.Subject = email.Subject;
                newEmail.Body = new TextPart(TextFormat.Html)
                {
                    Text = email.Body
                };
                using (var smtp = new SmtpClient())
                {
                    smtp.Connect(_serverAddress, _serverPort, MailKit.Security.SecureSocketOptions.StartTls);
                    smtp.Authenticate(_serverUser, _serverPassword);
                    smtp.Send(newEmail);
                    smtp.Disconnect(true);
                }
                return new ServiceResponse<bool>(data: true);

            }
            catch (Exception ex)
            {
                return new ServiceResponse<bool>(error: ex.Message);
            }
        }
    }
}
