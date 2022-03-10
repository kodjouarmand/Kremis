using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Kremis.Domain.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.UI.Services;
using Kremis.Utility.Options;
using Kremis.Infrastructure.Contracts;
using Kremis.Infrastructure;
using Kremis.Utility.Enum;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Kremis.Utility.Options.Validations;
using Kremis.Api.Initializer;

namespace Kremis.Api.Extensions
{
    public static class ApiServiceExtensions
    {
        public static void ConfigureDbInitializer(this IServiceCollection services)
        {
            services.AddScoped<IDbInitializer, DbInitializer>();
        }

        public static void ConfigureOptions(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<LoggingOptions>(configuration.GetSection(LoggingOptions.ConfigSectionName));

            //services.Configure<DbOptions>(configuration.GetSection(DbOptions.ConfigSectionName));
            //services.TryAddSingleton<IValidateOptions<DbOptions>, DbOptionsValidation>();

            services.Configure<SuperAministratorOptions>(configuration.GetSection(SuperAministratorOptions.ConfigSectionName));
            services.TryAddSingleton<IValidateOptions<SuperAministratorOptions>, SuperAministratorOptionsValidation>();
        }

        public static void ConfigureDbContexts(this IServiceCollection services, IConfiguration configuration)
        {
            var dbOptions = new DbOptions();
            configuration.Bind(DbOptions.ConfigSectionName, dbOptions);

            if (dbOptions.ServerType == DbServerTypeEnum.Sqlite.ToString())
            {
                services.AddDbContext<ApplicationDbContext>(opts =>
                   opts.UseSqlite(configuration.GetConnectionString(dbOptions.SqliteConnectionStringName)));
            }
            else if (dbOptions.ServerType == DbServerTypeEnum.SqlServer.ToString())
            {
                services.AddDbContext<ApplicationDbContext>(opts =>
                opts.UseSqlServer(configuration.GetConnectionString(dbOptions.SqlServerConnectionStringName)));
            }
        }

        public static void ConfigureInfrastructureServices(this IServiceCollection services)
        {
            services.AddScoped<ILoggerService, LoggerService>();
        }

        public static void ConfigureControllers(this IServiceCollection services)
        {
            services.AddControllers();
        }

        public static void ConfigureSwagger(this IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Kremis.API", Version = "v1" });
            });
        }
    }
}
