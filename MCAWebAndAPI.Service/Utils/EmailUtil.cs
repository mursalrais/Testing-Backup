using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MCAWebAndAPI.Service.Utils
{
    public class EmailUtil
    {
        public async static Task SendAsync(string emailAddress, string subject, string emailMessage)
        {
            Send(emailAddress, subject, emailMessage);
        }

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

            foreach (var item in mail.To)
            {
                smtp.SendAsync(mail.From.Address, item.Address, mail.Subject, mail.Body, DateTime.Now);
            }
        }

        public static void SendMultiple(List<String> lstEmail, string subject, string emailMessage)
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
            foreach (var item in lstEmail)
            {
                mail.To.Add(new MailAddress(item));
            }

            mail.Subject = subject;
            mail.Body = emailMessage;
            mail.IsBodyHtml = true;

           
            smtp.Send(mail);

            //foreach (var item in mail.To)
            //{
            //    smtp.SendAsync(mail.From.Address, item.Address, mail.Subject, mail.Body, DateTime.Now);
            //}
        }

        public static bool IsValidEmailId(string InputEmail)
        {
            Regex regex = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");
            Match match = regex.Match(InputEmail);
            if (match.Success)
                return true;
            else
                return false;
        }
    }
}