using System.Net;
using System.Net.Mail;

namespace MCAWebAndAPI.Service.Utils
{
    public class EmailUtil
    {
        public static void Send(string emailAddress, string subject, string emailMessage)
        {
            var smtp = new SmtpClient();
            {
                smtp.Host = "smtp-mail.outlook.com";
                smtp.Port = 587;
                smtp.EnableSsl = true;
                smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtp.Credentials = new NetworkCredential("auto_mcaindonesia@outlook.com", "MCAIMS_Indoneisa");
                smtp.Timeout = 20000;
            }

            MailMessage mail = new MailMessage();

            mail.From = new MailAddress("no-reply@mca-indonesia.go.id", "ims.mca-indonesia.go.id");
            mail.To.Add(new MailAddress(emailAddress));
            mail.Subject = subject;
            mail.Body = emailMessage;
            mail.IsBodyHtml = true;

            smtp.Send(mail);
        }
    }
}
