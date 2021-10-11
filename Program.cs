using CommandLine;
using Microsoft.Extensions.CommandLineUtils;
using System;
using System.Collections.Generic;
using System.Net.Mail;

namespace RebootNotification
{
    class Program
    {
        class Options
        {
            [Option('s', "sender", Required = false, Default ="jz248@duke.edu", HelpText = "Notifications sent from.")]
            public string Sender { get; set; }

            [Option('r', "receiver", Required = true, HelpText = "Notifications sent to.")]
            public IEnumerable<string> Receivers { get; set; }

            [Option('c', "cc", Required = false, HelpText = "Notifications sent to.")]
            public IEnumerable<string> Copies { get; set; }

            // Omitting long name, defaults to name of property, ie "--verbose"
            [Option(Default = false, HelpText = "Prints all messages to standard output.")]
            public bool Verbose { get; set; }
        }

        static void Main(string[] args)
        {
            CommandLine.Parser.Default.ParseArguments<Options>(args)
              .WithParsed(RunOptions)
              .WithNotParsed(HandleParseError);
        }
        static void RunOptions(Options opts)
        {
            //handle options
            SendMails(opts.Sender, opts.Receivers, opts.Copies);
        }
        static void HandleParseError(IEnumerable<Error> errs)
        {
            //handle errors
            Console.WriteLine("Parse Arguments Failed.");
        }

        static void SendMails(string sender, IEnumerable<string> receivers, IEnumerable<string> copies)
        {
            SmtpClient client = new SmtpClient();
            client.Host = "smtp.duke.edu";
            client.Port = 25;
            //client.Credentials = new System.Net.NetworkCredential("jh642@duke.edu", "");
            MailMessage message = new MailMessage();
            MailAddress from = new MailAddress(sender);//Sender
            foreach (var item in receivers)//Receiver
            {
                message.To.Add(new MailAddress(item));
            }
            foreach (var item in copies)//Carbon Copy
            {
                message.CC.Add(new MailAddress(item));
            }

            string serverInfo = Environment.MachineName;
            message.Subject = string.Format("Reboot Notification - {0}", serverInfo);
            message.Body = string.Format("Your server {0} has been rebooted on {1}, please check the status.", serverInfo, DateTime.Now);
            message.From = from;
            client.Send(message);
        }
    }
}
