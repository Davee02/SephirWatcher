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
    public class EditSephirAccountController : BaseController
    {
        private readonly SephirContext _context;
        private readonly IStringCipher _stringCipher;
        private readonly UserManager<IdentityUser> _userManager;

        public EditSephirAccountController(SephirContext context, IStringCipher stringCipher, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _stringCipher = stringCipher;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var existingSephirLogin = await GetExistingSephirAccountForCurrentUser();
            if (existingSephirLogin != null)
            {
                return View("Index", new SephirLoginViewModel { EmailAdress = existingSephirLogin.EmailAdress });
            }
            else
            {
                return RedirectToAction("Overview", "Home");
            }
        }

        public IActionResult Saved()
        {
            return View("Saved");
        }

        [HttpPost]
        public async Task<IActionResult> SaveSephirLogin(SephirLoginViewModel newLogin)
        {
            if (!ModelState.IsValid)
                return View("Index", newLogin);

            if (!await ValidateSephirLogin(newLogin))
            {
                ModelState.AddModelError(string.Empty, "The Sephir credentials are invalid (could not login in with them)");
                return View("Index", newLogin);
            }

            var existingLogin = await GetExistingSephirAccountForCurrentUser();
            existingLogin.EmailAdress = newLogin.EmailAdress;
            existingLogin.EncryptedPassword = _stringCipher.Encrypt(newLogin.Password);

            _context.Update(existingLogin);

            await _context.SaveChangesAsync();


            return RedirectToAction("Saved");
        }

        private static async Task<bool> ValidateSephirLogin(SephirLoginViewModel login)
        {
            var sephirWatcher = new SephirWatcher(new SephirAccount
            {
                AccountEmail = login.EmailAdress,
                AccountPassword = login.Password
            });
            return await sephirWatcher.AreCredentialsValid();
        }

        private async Task<SephirLogin> GetExistingSephirAccountForCurrentUser()
        {
            return await _context.SephirLogins.FirstOrDefaultAsync(x =>
                x.IdentityUserId == _userManager.GetUserId(HttpContext.User));
        }
    }
}
