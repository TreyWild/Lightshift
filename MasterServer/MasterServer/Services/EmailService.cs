using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace MasterServer
{
    public class EmailService
    {
        public static void Send(string reciever, string username, string subject, string msg)
        {
            try
            {
                var fromAddress = new MailAddress("contact.lightshift@gmail.com", "Lightshift");
                //var toAddress = new MailAddress("address", "displayName");
                const string fromPassword = "Vel99Cer99!";
                //const string subject = "Attention!";

                var toAddress = new MailAddress(reciever, username);

                var smtp = new SmtpClient
                {
                    Host = "smtp.gmail.com",
                    Port = 587,
                    EnableSsl = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(fromAddress.Address, fromPassword)
                };
                using (var message = new MailMessage(fromAddress, toAddress)
                {

                    Subject = subject,
                    Body = msg,
                    From = new MailAddress("verify@lightshift.net", "Lightshift")
                })


                {
                    smtp.Send(message);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
    }
}
