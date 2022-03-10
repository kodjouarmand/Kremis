using Microsoft.AspNetCore.Hosting;

[assembly: HostingStartup(typeof(Kremis.Areas.Identity.IdentityHostingStartup))]
namespace Kremis.Areas.Identity
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