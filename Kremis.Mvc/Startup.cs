using Kremis.BusinessLogic.Queries.Contracts;
using Kremis.Infrastructure;
using Kremis.Infrastructure.Contracts;
using Kremis.Mvc.Extensions;
using Kremis.Utility.Helpers;
using Kremis.Utility.Options;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Kremis.Domain.Contexts;

namespace Kremis.Mvc
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
            services.ConfigureDbContexts(Configuration);
            services.ConfigureUnitOfWork();
            services.ConfigureBusinessServices();
            services.ConfigureOptions(Configuration);
            services.ConfigureInfrastructureServices();
            services.ConfigureIISIntegration();
            services.ConfigureAutoMapper();
            services.ConfigureControllers();
            services.ConfigureSessions();
            services.ConfigureSecurity(Configuration);
            services.ConfigureApplicationCookie();
            services.ConfigureFacebookAuthentication();
            services.ConfigureGoogleAuthentication();
            services.ConfigureTempData();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                //app.UseDeveloperExceptionPage();
                app.UseMigrationsEndPoint();
            }
            else
            {
                //app.UseExceptionHandler(ConstantHelper.DEFAULT_ERROR_PAGE);
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            //app.UseExceptionHandler(ConstantHelper.DEFAULT_ERROR_PAGE);
            app.UseDeveloperExceptionPage();
            app.UseSession();
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{area=Home}/{controller=Home}/{action=Index}/{id?}");
                endpoints.MapRazorPages();
            });
        }        
    }
}
