using System.Linq;
using System.Threading.Tasks;
using DaHo.SephirWatcher.Models;
using DaHo.SephirWatcher.Web.Data;
using DaHo.SephirWatcher.Web.Interfaces;
using DaHo.SephirWatcher.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DaHo.SephirWatcher.Web.Controllers
{
    [Authorize]
    public class HomeController : BaseController
    {
        private readonly SephirContext _context;
        private readonly IStringCipher _stringCipher;
        private readonly UserManager<IdentityUser> _userManager;

        public HomeController(SephirContext context, IStringCipher stringCipher, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _stringCipher = stringCipher;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            if (await SephirAccountForCurrentUserIsSaved())
            {
                return View("Overview");
            }

            return View("AddSephirAccount");
        }

        public IActionResult Overview()
        {
            return View("Overview");
        }

        [HttpPost]
        public async Task<IActionResult> SaveSephirLogin(SephirLoginViewModel sephirLogin)
        {
            if (!ModelState.IsValid)
                return View("AddSephirAccount", sephirLogin);

            if (await _context.SephirLogins.AnyAsync(x => x.EmailAdress.Equals(sephirLogin.EmailAdress)))
            {
                ModelState.AddModelError(string.Empty,  "A Sephir account with this email-address was already saved");
                return View("AddSephirAccount", sephirLogin);
            }

            if (!await ValidateSephirLogin(sephirLogin))
            {
                ModelState.AddModelError(string.Empty, "The Sephir credentials are invalid (could not login in with them)");
                return View("AddSephirAccount", sephirLogin);
            }

            var login = new SephirLogin
            {
                EmailAdress = sephirLogin.EmailAdress,
                EncryptedPassword = _stringCipher.Encrypt(sephirLogin.Password),
                IdentityUserId = _userManager.GetUserId(HttpContext.User)
            };
            await _context.SephirLogins.AddAsync(login);

            await _context.SaveChangesAsync();

            await GetAndSaveSephirTests(login);

            return RedirectToAction("Overview");
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

        private async Task<bool> SephirAccountForCurrentUserIsSaved()
        {
            return await _context.SephirLogins.AnyAsync(x =>
                x.IdentityUserId == _userManager.GetUserId(HttpContext.User));
        }

        private async Task GetAndSaveSephirTests(SephirLogin sephirLogin)
        {
            var sephirWatcher = new SephirWatcher(new SephirAccount
            {
                AccountEmail = sephirLogin.EmailAdress,
                AccountPassword = _stringCipher.Decrypt(sephirLogin.EncryptedPassword)
            });

            var tests = (await sephirWatcher.GetSephirExamsForAllClasses())
                .Where(x => x.Mark.HasValue)
                .Select(x => new SephirTest
                {
                    ExamDate = x.ExamDate,
                    ExamState = x.ExamState,
                    ExamTitle = x.ExamTitle,
                    Mark = x.Mark,
                    MarkType = x.MarkType,
                    MarkWeighting = x.MarkWeighting,
                    SchoolSubject = x.SchoolSubject,
                    SephirLoginId = sephirLogin.Id
                });

            await _context.SephirTests.AddRangeAsync(tests);
            await _context.SaveChangesAsync();
        }
    }
}
