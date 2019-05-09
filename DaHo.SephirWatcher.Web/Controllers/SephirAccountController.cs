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

namespace DaHo.SephirWatcher.Web.Controllers
{
    [Authorize]
    public class SephirAccountController : BaseController
    {
        private readonly SephirContext _context;
        private readonly IStringCipher _stringCipher;
        private readonly UserManager<IdentityUser> _userManager;

        public SephirAccountController(SephirContext context, IStringCipher stringCipher, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _stringCipher = stringCipher;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var existingSephirAccount = await GetExistingSephirAccountForCurrentUser();
            if (existingSephirAccount != null)
            {
                return View("Edit",
                    new SephirLoginViewModel
                    {
                        EmailAdress = existingSephirAccount.EmailAdress,
                        Id = existingSephirAccount.Id
                    });
            }

            return View("Create");
        }

        public IActionResult Saved()
        {
            return View("Saved");
        }

        [HttpPost]
        public async Task<IActionResult> EditSephirLogin(long id, SephirLoginViewModel newLogin)
        {
            if (id != newLogin.Id)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
                return View("Edit", newLogin);

            if (!await ValidateSephirLogin(newLogin))
            {
                ModelState.AddModelError(string.Empty, "The Sephir credentials are invalid (could not login in with them)");
                return View("Edit", newLogin);
            }

            var existingLogin = await _context.SephirLogins.FindAsync(id);
            existingLogin.EmailAdress = newLogin.EmailAdress;
            existingLogin.EncryptedPassword = _stringCipher.Encrypt(newLogin.Password);

            _context.Update(existingLogin);

            await _context.SaveChangesAsync();

            return RedirectToAction("Saved");
        }

        [HttpPost]
        public async Task<IActionResult> CreateSephirLogin(SephirLoginViewModel sephirLogin)
        {
            if (!ModelState.IsValid)
                return View("Create", sephirLogin);

            if (await _context.SephirLogins.AnyAsync(x => x.EmailAdress.Equals(sephirLogin.EmailAdress)))
            {
                ModelState.AddModelError(string.Empty, "A Sephir account with this email-address was already saved");
                return View("Create", sephirLogin);
            }

            if (!await ValidateSephirLogin(sephirLogin))
            {
                ModelState.AddModelError(string.Empty, "The Sephir credentials are invalid (could not login in with them)");
                return View("Create", sephirLogin);
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
                    ExamTitle = x.ExamTitle,
                    EncryptedMark = _stringCipher.Encrypt(x.Mark.ToString()),
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
