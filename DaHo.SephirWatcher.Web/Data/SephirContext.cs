using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DaHo.SephirWatcher.Web.Data
{
    public class SephirContext : IdentityDbContext
    {
        public SephirContext(DbContextOptions<SephirContext> options)
            : base(options)
        {

        }
    }
}
