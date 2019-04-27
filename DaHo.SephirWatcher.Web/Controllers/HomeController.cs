using DaHo.SephirWatcher.Web.Data;
using DaHo.SephirWatcher.Web.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DaHo.SephirWatcher.Web.Controllers
{
    [Authorize]
    public class HomeController : BaseController
    {
        private readonly SephirContext _context;
        private readonly IPasswordCipher _passwordCipher;

        public HomeController(SephirContext context, IPasswordCipher passwordCipher)
        {
            _context = context;
            _passwordCipher = passwordCipher;
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}
