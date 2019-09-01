using System.Threading.Tasks;
using DaHo.SephirWatcher.Web.Data;
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
        private readonly UserManager<IdentityUser> _userManager;

        public HomeController(SephirContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [AllowAnonymous]
        public IActionResult Index()
        {
            return View("Index");
        }

        public async Task<IActionResult> Overview()
        {
            var sephirAccountExists = (await GetExistingSephirAccountForCurrentUser()) != null;

            return View("Overview", new OverviewViewModel { SephirAccountIsSaved = sephirAccountExists });
        }

        private async Task<SephirLogin> GetExistingSephirAccountForCurrentUser()
        {
            return await _context.SephirLogins.FirstOrDefaultAsync(x =>
                x.IdentityUserId == _userManager.GetUserId(HttpContext.User));
        }
    }
}
