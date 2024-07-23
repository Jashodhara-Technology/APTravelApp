using APTravelApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Hosting;
using System.IO;
using System.Threading.Tasks;
using System;
using APTravelApp.Services.IServices;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Options;

namespace APTravelApp.Pages
{
    public class EnquiryModel : PageModel
    {


        private readonly IMailService mailSrv;
        private readonly MailSettings _mailSettings;
        private readonly IWebHostEnvironment _hostEnvironment;
        public EnquiryModel(IMailService _mailSrv, IOptions<MailSettings> mailSettings, IWebHostEnvironment hostEnvironment)
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
                if (CaptchaHelper.ValidateCaptchaCode(SaleModel.CaptchaCode, HttpContext))
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
                else
                {
                    ModelState.AddModelError("SaleModel.CaptchaCode", "captcha code entered is invalid.");
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
    }
}
