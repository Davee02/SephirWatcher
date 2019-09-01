using Microsoft.AspNetCore.Hosting;

[assembly: HostingStartup(typeof(DaHo.SephirWatcher.Web.Areas.Identity.IdentityHostingStartup))]
namespace DaHo.SephirWatcher.Web.Areas.Identity
{
    public class IdentityHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices((context, services) => {
            });
        }
    }
}