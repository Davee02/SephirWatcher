using System.Threading.Tasks;
using DaHo.SephirWatcher.Models;
using DaHo.SephirWatcher.Web.Data;
using DaHo.SephirWatcher.Web.Interfaces;
using DaHo.SephirWatcher.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DaHo.SephirWatcher.Web.Controllers
{
    [Authorize]
    public class HomeController : BaseController
    {
        private readonly SephirContext _context;
        private readonly IPasswordCipher _passwordCipher;
        private readonly UserManager<IdentityUser> _userManager;

        public HomeController(SephirContext context, IPasswordCipher passwordCipher, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _passwordCipher = passwordCipher;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SaveSephirLogin(SephirLoginViewModel sephirLogin)
        {
            if (!ModelState.IsValid)
                return View("Index", sephirLogin);

            if (await _context.SephirLogins.AnyAsync(x => x.EmailAdress.Equals(sephirLogin.EmailAdress)))
            {
                ModelState.AddModelError(string.Empty, "Ein Sephir-Account mit dieser E-Mail Adresse wurde bereits gespeichert");
                return View("Index", sephirLogin);
            }

            if (!await ValidateSephirLogin(sephirLogin))
            {
                ModelState.AddModelError(string.Empty, "Die eingegebenen Sephir-Accountdaten stimmen nicht (das Einloggen damit funktioniert nicht)");
                return View("Index", sephirLogin);
            }

            await _context.SephirLogins.AddAsync(new SephirLogin
            {
                EmailAdress = sephirLogin.EmailAdress,
                EncryptedPassword = _passwordCipher.Encrypt(sephirLogin.Password),
                IdentityUserId = _userManager.GetUserId(HttpContext.User)
            });

            await _context.SaveChangesAsync();

            return View("Index");
        }

        private async Task<bool> ValidateSephirLogin(SephirLoginViewModel login)
        {
            var sephirWatcher = new SephirWatcher(new SephirAccount
            {
                AccountEmail = login.EmailAdress,
                AccountPassword = login.Password
            });
            return await sephirWatcher.AreCredentialsValid();
        }
    }
}
