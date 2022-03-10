using Microsoft.Extensions.DependencyInjection;
using Kremis.DataAccess.Repositories;
using Kremis.DataAccess.Repositories.Contracts;

namespace Kremis.Infrastructure
{
    public static class DbServiceExtention
    {        
        public static void ConfigureUnitOfWork(this IServiceCollection services)
        {
            services.AddScoped<IUnitOfWork, UnitOfWork>();
        }
    }
}
