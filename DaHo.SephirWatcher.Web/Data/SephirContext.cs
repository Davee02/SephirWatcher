using DaHo.SephirWatcher.Web.Models;
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

        public DbSet<SephirLogin> SephirLogins { get; set; }

        public DbSet<SephirTest> SephirTests { get; set; }
    }
}
