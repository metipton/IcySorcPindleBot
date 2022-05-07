using IcySorcPindleBot.Helpers.ItemHelper;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;


namespace IcySorcPindleBot.Helpers
{
    public static class Mailer
    {
        private static readonly MailAddress fromAddress = new MailAddress("icysorcpindlebot@gmail.com", "From Name");
        private static readonly MailAddress toAddress = new MailAddress("4432541679@vtext.com", "To Name");
        private const string fromPassword = "tipton13!#";
        private const int HeartBeatInterval = 25;
        public static void SendItemFind(List<Item> items)
        {
            if (items.Count == 1 && items[0].ItemType == "White")
            {
                return;
            }

            var body = GetItemMessageBody(items);

            SendMessage(body);
        }

        public static void SendHeartBeatMessage()
        {
            if(Globals.RUN_NUMBER % HeartBeatInterval != 0)
            {
                return;
            }

            var body = GetHeartBeatMessage();

            SendMessage(body);
        }

        private static void SendMessage(string body) 
        {
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

        private static string GetHeartBeatMessage()
        {

            return $"{DateTime.Now} Heart Beat - Commencing Run number: {Globals.RUN_NUMBER}";
        }

        private static string GetItemMessageBody(List<Item> items)
        {
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

            return body;
        }
    }
}