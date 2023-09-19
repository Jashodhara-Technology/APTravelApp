//using MimeKit;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using System.Net.Mail;
using System.Text;
using System;

using Microsoft.AspNetCore.Http;
using APTravelApp.Services.IServices;
using APTravelApp.Models;
using System.Net;

namespace APTravelApp.Services
{
    public class MailService : IMailService
    {

        private readonly MailSettings _mailSettings;
        public MailService(IOptions<MailSettings> mailSettings)
        {
            _mailSettings = mailSettings.Value;
        }



        public async Task SendEmailAsync(MailRequest mailRequest)
        {
            try
            {


                MailMessage message = new MailMessage();
                message.From = new MailAddress(_mailSettings.Mail, _mailSettings.DisplayName);
                message.To.Add(new MailAddress(mailRequest.ToEmail));

                if (!string.IsNullOrEmpty(_mailSettings.ToCC))
                {
                    message.CC.Add(new MailAddress(_mailSettings.ToCC));
                }

                message.Subject = mailRequest.Subject;
                message.Body = mailRequest.Body;
                message.BodyEncoding = Encoding.UTF8;
                message.IsBodyHtml = true;
                message.Priority = MailPriority.Normal;

                SmtpClient client = new SmtpClient();

                //client.Credentials = new System.Net.NetworkCredential(_mailSettings.Mail.Trim(), _mailSettings.Password.Trim(), _mailSettings.DisplayName);
                client.DeliveryMethod = SmtpDeliveryMethod.Network;


                //client.Credentials = new System.Net.NetworkCredential(_mailSettings.Mail.Trim(), _mailSettings.Password.Trim(),  _mailSettings.DisplayName);
                
                client.UseDefaultCredentials = false;

                client.Host = _mailSettings.Host;
                client.Port = _mailSettings.Port;
                client.EnableSsl = _mailSettings.EnableSsl;

                if (mailRequest.Attachments != null)
                {
                    foreach (var file in mailRequest.Attachments)
                    {
                        Random rnd = new Random();
                        string fileName = Path.Combine(mailRequest.SourcePath, file.FileName + rnd.Next(1, 20).ToString() + DateTime.Now.ToString());
                        long length = file.Length;
                        if (length > 0)
                        {
                            FileStream fileStream = new FileStream(fileName, FileMode.OpenOrCreate);
                            byte[] bytes = new byte[length];
                            fileStream.Read(bytes, 0, (int)file.Length);
                            message.Attachments.Add(new Attachment(fileStream, fileName));

                        }
                    }
                }


                //call this line, before you call .Send
                client.Credentials = CredentialCache.DefaultNetworkCredentials;
                await client.SendMailAsync(message);

            }

            catch (Exception ex)
            {
                throw ex;
            }
        }





        public void WriteException(string filePath, Exception ex)
        {
            string path = Path.Combine(filePath, "Error.txt");
            using (StreamWriter writer = new StreamWriter(path, true))
            {
                writer.WriteLine("-----------------------------------------------------------------------------");
                writer.WriteLine("Date : " + DateTime.Now.ToString());
                writer.WriteLine();

                while (ex != null)
                {
                    writer.WriteLine(ex.GetType().FullName);
                    writer.WriteLine("Message : " + ex.Message);
                    writer.WriteLine("StackTrace : " + ex.StackTrace);
                    ex = ex.InnerException;
                }
            }

        }












    }
}
