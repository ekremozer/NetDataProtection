using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NetDataProtection.Web.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Microsoft.AspNetCore.DataProtection;
using NetDataProtection.Web.Core;

namespace NetDataProtection.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly IDataProtector _dataProtector;
        private readonly List<string> _emailList = new List<string> { "ekrem@gmail.com", "hakan@gmail.com", "murat@gmail.com" };

        public HomeController(IDataProtectionProvider dataProtector)
        {
            _dataProtector = dataProtector.CreateProtector(DataProtectorDefaults.EmailValidation);
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult InputEmail()
        {
            return View();
        }

        [HttpPost]
        public IActionResult InputEmail(string email)
        {
            var timeLimited = _dataProtector.ToTimeLimitedDataProtector();
            var encryptedEmail = timeLimited.Protect(email, TimeSpan.FromHours(24));
            //var encryptedEmail = _dataProtector.Protect(email);
            var validationLink = $"/home/EmailValidation?email={encryptedEmail}";
            ViewBag.ValidationLink = validationLink;
            return View();
        }

        public IActionResult EmailValidation(string email)
        {
            try
            {
                var decryptedEmail = _dataProtector.Unprotect(email);
                var isValid = _emailList.Contains(decryptedEmail);
                ViewBag.Message = isValid
                    ? $"{decryptedEmail} email adresiniz doğrulandı."
                    : $"{decryptedEmail} email adresiniz doğrulanamadı.";
            }
            catch (CryptographicException ex)
            {
                ViewBag.Message = $"Hata oluştu: {ex.Message}";
            }
            return View();
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }


    }
}
