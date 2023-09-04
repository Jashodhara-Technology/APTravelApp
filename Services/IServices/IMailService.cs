using APTravelApp.Models;
using System;
using System.Threading.Tasks;

namespace APTravelApp.Services.IServices
{
    public interface IMailService
    {
        Task SendEmailAsync(MailRequest mailRequest);
        void WriteException(string filePath, Exception ex);

    }
}
