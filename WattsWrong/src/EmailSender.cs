using System;
using System.Net;
using System.Net.Mail;
using System.IO;
using System.Windows.Forms;

namespace WattsWrong
{
    internal class EmailSender
    {
        private readonly string smtpServer;
        private readonly int port;
        private readonly string senderEmail;
        private readonly string senderPassword;

        // Constructor to initialize SMTP settings
        public EmailSender(string smtpServer, int port, string senderEmail, string senderPassword)
        {
            this.smtpServer = smtpServer;
            this.port = port;
            this.senderEmail = senderEmail;
            this.senderPassword = senderPassword;
        }

        
        public void SendEmail(string recipientEmail, string subject, string body, string pdfFilePath , bool show_msg)
        {
            try
            {
                // Configure the SMTP client
                SmtpClient smtpClient = new SmtpClient(smtpServer)
                {
                    Port = port,
                    Credentials = new NetworkCredential(senderEmail, senderPassword),
                    EnableSsl = true,
                };

                // Create the email message
                MailMessage mailMessage = new MailMessage
                {
                    From = new MailAddress(senderEmail),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true, // Enable HTML content in the body
                };

                // Add the recipient
                mailMessage.To.Add(recipientEmail);

                // Attach the PDF file if it exists
                if (File.Exists(pdfFilePath))
                {
                    Attachment attachment = new Attachment(pdfFilePath);
                    mailMessage.Attachments.Add(attachment);
                }
                else
                {
                    Console.WriteLine("The specified PDF file does not exist.");
                    if(pdfFilePath != "ALERT")return;
                }

                // Send the email
                smtpClient.Send(mailMessage);
                Console.WriteLine("Email sent successfully!");
               if(show_msg) MessageBox.Show("Email Sent", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred while sending the email: " + ex.Message);
                MessageBox.Show(ex.Message);
            }
        }
    }
}
