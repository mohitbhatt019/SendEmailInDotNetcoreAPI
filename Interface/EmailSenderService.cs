using Microsoft.Extensions.Options;
using MimeKit;
using System.Net.Mail;
using MailKit.Net.Smtp;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.AspNetCore.Identity.UI.Services;
using System.Net;

namespace SendEmail.Interface
{
    public class EmailSenderService : IEmailSender
    {
        private EmailSettings _emailSettings { get; }
        public EmailSenderService(IOptions<EmailSettings> emailSettings)
        {
            _emailSettings = emailSettings.Value;
        }
        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            Execute(email, subject, htmlMessage).Wait();
            return Task.FromResult(0);
        }
        public async Task Execute(string email, string subject, string message)
        {
            try
            {
                string toEmail = string.IsNullOrEmpty(email) ? _emailSettings.ToEmail : email;
                MailMessage mail = new MailMessage()
                {
                    From = new MailAddress(_emailSettings.UsernameEmail, "My Email Name")
                };
                mail.To.Add(new MailAddress(toEmail));
                mail.CC.Add(new MailAddress(_emailSettings.CcEmail));
                mail.Subject = "Invitiation :" + subject;

                // Construct the email body with the invitation link
                string invitationLink = "https://example.com/invitation"; // Replace with your actual invitation link
                string emailBody = message + "<br/><br/>" + "Click <a href=\"" + invitationLink + "\">here</a> to accept the invitation.";


                mail.Body = email;
                mail.IsBodyHtml = true;
                mail.Priority = MailPriority.High;
                using (System.Net.Mail.SmtpClient smtp = new System.Net.Mail.SmtpClient(_emailSettings.PrimaryDomain,
                    _emailSettings.PrimaryPort))
                {
                    smtp.Credentials = new NetworkCredential(_emailSettings.UsernameEmail,
                        _emailSettings.UsernamePassword);
                    smtp.EnableSsl = true;
                    await smtp.SendMailAsync(mail);
                }
            }
            catch (Exception ex)
            {
                string str = ex.Message;
            }
        }
    }
}
