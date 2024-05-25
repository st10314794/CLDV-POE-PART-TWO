
using System.Net.Mail;
using System.Net;

namespace CLDV_POE_PART_TWO
{
    public class EmailSender : IEmailSender
    {
        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            var mail = "KhumaloCrafts@outlook.com";
            var pw = "bhnccjwkmbrtwdfi";

            var client = new SmtpClient("smtp.outlook.com", 587)
            {
                EnableSsl = true,
                Credentials = new NetworkCredential(mail, pw)
            };

            var mailMessage = new MailMessage(from: mail, to: email,
                subject, htmlMessage)
            {
                IsBodyHtml = true
            };
            return client.SendMailAsync(mailMessage);
        }
    }
}
