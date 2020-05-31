using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Net.Mail;
using UnityEngine;

namespace OneDevApp
{
    // delegate Declaration
    public delegate void OnMailSendDelegate(bool isMailSend, string error);

    public class SMTPManager : MonoInstance<SMTPManager>
    {
        // event Declaration
        public event OnMailSendDelegate OnMailSendEvent;

        [SerializeField]
        [Tooltip("Sender Mail ID")]
        private string SenderMailId;
        [SerializeField]
        [Tooltip("Sender Mail ID Password")]
        private string SenderPassword;
        [SerializeField]
        [Tooltip("Sender SMTP port")]
        private string SmtpPort;
        [SerializeField]
        [Tooltip("Sender SMTP ID")]
        private string SmtpId;

        private void SendCompletedCallback(object sender, AsyncCompletedEventArgs e)
        {
            // Get the unique identifier for this asynchronous operation.
            String token = (string)e.UserState;

            if (e.Cancelled)
            {
                OnMailSendEvent(false, e.Error.ToString());
                Console.WriteLine("[{0}] Send canceled.", token);
            }
            if (e.Error != null)
            {
                OnMailSendEvent(false, e.Error.ToString());
                Console.WriteLine("[{0}] {1}", token, e.Error.ToString());
            }
            else
            {
                OnMailSendEvent(true, "Message sent.");
                Console.WriteLine("Message sent.");
            }
        }

        public void SendEmail(string toMailId, string mailSubject, string mailBody, string[] mailAttachmentFilePath)
        {
            if (string.IsNullOrEmpty(SmtpId))
            {
                OnMailSendEvent(false, "SMTP ID cannot be empty");
                return;
            }
            else
            if (string.IsNullOrEmpty(SmtpPort))
            {
                OnMailSendEvent(false, "SMPT Port cannot be empty");
                return;
            }
            else
            if (string.IsNullOrEmpty(toMailId))
            {
                OnMailSendEvent(false, "Mail sender ID cannot be empty");
                return;
            }
            else
            if (string.IsNullOrEmpty(toMailId))
            {
                OnMailSendEvent(false, "To Mail ID cannot be empty");
                return;
            }
            else
            if (string.IsNullOrEmpty(mailBody))
            {
                OnMailSendEvent(false, "Mail body cannot be empty");
                return;
            }

            MailMessage mail = new MailMessage();
            mail.From = new MailAddress(SenderMailId);
            mail.To.Add(toMailId);
            mail.Subject = mailSubject;
            mail.Body = mailBody;

            List<Attachment> fileAttachments = new List<Attachment>();

            if (mailAttachmentFilePath != null)
            {
                foreach (string filePath in mailAttachmentFilePath)
                {
                    if (File.Exists(filePath))
                    {
                        Attachment fileData = new Attachment(filePath);
                        mail.Attachments.Add(fileData);
                        fileAttachments.Add(fileData);
                    }
                }

            }

            try
            {
                SmtpClient smtpServer = new SmtpClient(SmtpId);
                smtpServer.Port = Int32.Parse(SmtpPort);
                if (string.IsNullOrEmpty(SenderPassword))
                {
                    smtpServer.UseDefaultCredentials = false;
                }
                else
                {
                    smtpServer.UseDefaultCredentials = true;

                    NetworkCredential nc = new NetworkCredential(SenderMailId, SenderPassword);

                    smtpServer.EnableSsl = true;
                }

                // Set the method that is called back when the send operation ends.
                smtpServer.SendCompleted += new SendCompletedEventHandler(SendCompletedCallback);
                smtpServer.Send(mail);
                mail.Dispose();

                foreach (Attachment fileData in fileAttachments)
                {
                    fileData.Dispose();
                }

            }
            catch (Exception e)
            {
                OnMailSendEvent(false, e.GetBaseException().ToString());
                return;
            }
            finally
            {
                mail.Dispose();
            }
        }
    }
}
