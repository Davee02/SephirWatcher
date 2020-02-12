using DaHo.SephirWatcher.Web.Configuration;
using DaHo.SephirWatcher.Web.Data;
using DaHo.SephirWatcher.Web.Services;
using DaHo.SephirWatcher.Web.Services.Abstraction;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using DaHo.Library.AspNetCore;

namespace DaHo.SephirWatcher.Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.AddMvc(options => options.EnableEndpointRouting = false)
                .SetCompatibilityVersion(CompatibilityVersion.Version_3_0)
                .AddRazorRuntimeCompilation();

            services.AddDbContext<SephirContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            services.AddTransient<IEmailSender, EmailSender>();
            services.AddTransient<IStringCipher, StringCipher>();

            services.AddSingleton<IHostedService, SephirMarkWatcherService>();

            services.AddDefaultIdentity<IdentityUser>()
                .AddDefaultUI()
                .AddEntityFrameworkStores<SephirContext>();

            services.Configure<EmailSenderOptions>(Configuration);
            services.Configure<PasswordCipherOptions>(Configuration);

            services.AddDataProtection();

            services.AddAuthentication()
                .AddGoogle(googleOptions =>
                    {
                        googleOptions.ClientId = Configuration["Authentication:Google:ClientId"];
                        googleOptions.ClientSecret = Configuration["Authentication:Google:ClientSecret"];
                    });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (!env.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseDeveloperExceptionPage();
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseAuthentication();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });

            app.MigrateDatabase<SephirContext>();
        }
    }
}
