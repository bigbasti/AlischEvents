using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net.Mail;
using log4net;

namespace AlischEvents.Web.Helpers {
    public class EMailHelper {

        static ILog logger = LogManager.GetLogger(typeof(EMailHelper));

        public static bool SendMail(string from, string to, string subject, string body, bool html) {

            MailMessage message = new MailMessage(from, to, subject, body);
            message.IsBodyHtml = html;

            try {
                SmtpClient client = new SmtpClient();

                logger.Debug("Using email settings: Host: " + client.Host + " | Port: " + client.Port);

                client.Send(message);
            } catch (Exception ex) {
                logger.Error("Could not send Email", ex);
                return false;
            }

            return true;
        }
    }
}