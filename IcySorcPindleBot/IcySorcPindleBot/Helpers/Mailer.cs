using IcySorcPindleBot.Helpers.ItemHelper;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;


namespace IcySorcPindleBot.Helpers
{
    public static class Mailer
    {
        public static void SendItemFind(List<Item> items)
        {
            if (items.Count == 1 && items[0].ItemType == "White")
            {
                return;
            }

            var fromAddress = new MailAddress("icysorcpindlebot@gmail.com", "From Name");
            var toAddress = new MailAddress("4432541679@vtext.com", "To Name");
            const string fromPassword = "tipton13!#";
            var body = $"{DateTime.Now} Run number: {Globals.RUN_NUMBER} - ";

            foreach (var item in items)
            {
                if (item.ItemType == "Rune")
                {
                    body += $" You just found a {item.ItemName}.";

                }
                else if (item.ItemType != "White")
                {
                    body += $" You just found a {item.ItemType} {item.ItemName}.";
                }
            }

            var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromAddress.Address, fromPassword)
            };

            try
            {
                using (var message = new MailMessage(fromAddress, toAddress)
                {
                    Subject = body,
                    Body = ""
                })
                {
                    smtp.Send(message);
                    Console.WriteLine("Items found. Email sent.");
                }
            }
            catch
            {
                Console.WriteLine("Failed to send text.");
            }

        }
    }
}