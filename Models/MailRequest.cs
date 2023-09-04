using Microsoft.AspNetCore.Http;
using System.Collections.Generic;

namespace APTravelApp.Models
{

    public class MailRequest
    {
        public string ToEmail { get; set; }
        public string ToCC { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public List<IFormFile> Attachments { get; set; }
        public string SourcePath { get; set; }
    }
    public class MailSettings
    {
        public string Mail { get; set; }
        public string DisplayName { get; set; }
        public string Password { get; set; }
        public string Host { get; set; }
        public int Port { get; set; }
        public string ToCC { get; set; }     
        public bool EnableSsl { get; set; }
        
    }

}
