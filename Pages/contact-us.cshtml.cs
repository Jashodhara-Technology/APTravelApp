using APTravelApp.Models;
using APTravelApp.Services.IServices;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;
using System.IO;
using System.Threading.Tasks;
using System;
using Microsoft.AspNetCore.Http;

namespace APTravelApp.Pages
{
    public class contact_usModel : PageModel
    {
        private readonly IMailService mailSrv;
        private readonly MailSettings _mailSettings;
        private readonly IWebHostEnvironment _hostEnvironment;
        public contact_usModel(IMailService _mailSrv, IOptions<MailSettings> mailSettings, IWebHostEnvironment hostEnvironment)
        {
            mailSrv = _mailSrv;
            _mailSettings = mailSettings.Value;
            this._hostEnvironment = hostEnvironment;
        }

        [BindProperty]
        public Customer SaleModel { get; set; }

        public async Task<IActionResult> OnPostSales()
        {
            string wwwRootPath = _hostEnvironment.WebRootPath;
            try
            {
                ModelState.Clear();
                if (TryValidateModel(SaleModel))
                {
                    string body = string.Empty;
                    string path = Path.Combine(wwwRootPath + "/template/", "emailer-query.html");
                    using (StreamReader reader = new StreamReader(path))
                    {
                        body = reader.ReadToEnd();
                    }
                    body = body.Replace("{Name}", SaleModel.Name);
                    body = body.Replace("{Email}", SaleModel.Email);
                    body = body.Replace("{PhoneNo}", SaleModel.PhoneNo);
                    body = body.Replace("{Message}", SaleModel.Message);
                    MailRequest _mail = new MailRequest();
                    _mail.Subject = "Sales Query";
                    _mail.ToEmail = _mailSettings.Mail;
                    _mail.Body = body;
                    _mail.SourcePath = _hostEnvironment.WebRootPath + "/Exception/";
                    await mailSrv.SendEmailAsync(_mail);
                    await ThanksMail(SaleModel.Name, SaleModel.Email);
                    return RedirectToPage("/Thanks");
                }
                else
                {
                    return Page();
                }
            }
            catch (Exception ex)
            {
                string path = _hostEnvironment.WebRootPath + "/Exception/";
                mailSrv.WriteException(path, ex);
                return RedirectToPage("/Error");
            }
        }

        public async Task ThanksMail(string Name, string Email)
        {
            string body = string.Empty;
            string wwwRootPath = _hostEnvironment.WebRootPath;
            string path = Path.Combine(wwwRootPath + "/template/", "emailer-thanyou.html");
            using (StreamReader reader = new StreamReader(path))
            {
                body = reader.ReadToEnd();
            }
            body = body.Replace("{Name}", Name);
            MailRequest _mail = new MailRequest();
            _mail.Subject = "Thank you for Contacting Renewable Energy Systems Ltd";
            _mail.ToEmail = Email;
            _mail.Body = body;
            await mailSrv.SendEmailAsync(_mail);
        }



        public void OnGet()
        {
        }
        public IActionResult OnGetLoadImageFile(string name)
        {
            int width = 100;
            int height = 36;
            var captchaCode = Captcha.GenerateCaptchaCode();
            var result = Captcha.GenerateCaptchaImage(width, height, captchaCode);
            HttpContext.Session.SetString("CaptchaCode", result.CaptchaCode);
            Stream s = new MemoryStream(result.CaptchaByteData);
            return new FileStreamResult(s, "image/png");
        }


    }
}
