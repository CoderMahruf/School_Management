using System.Net;
using System.Net.Mail;

namespace CrudMVC.Services
{
    public class EmailSender : IEmailSender
    {
        public async Task<bool> SendEmailAsync(string email, string subject, string htmlMessage)
        {
            try
            {
                var smtpClient = new SmtpClient("smtp.gmail.com")
                {
                    Port = 587,
                    Credentials = new NetworkCredential(
                        "mahruf.dev@gmail.com",
                        "kwdn pmli qjlg bjhv"
                    ),
                    EnableSsl = true,
                };

                var mailMessage = new MailMessage
                {
                    From = new MailAddress("mahruf.dev@gmail.com"),
                    Subject = subject,
                    Body = htmlMessage,
                    IsBodyHtml = true,
                };

                mailMessage.To.Add(email);

                await smtpClient.SendMailAsync(mailMessage);

                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
