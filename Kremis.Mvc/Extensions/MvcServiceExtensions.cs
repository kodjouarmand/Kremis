﻿using AspNetCoreRateLimit;
using Kremis.Domain.Entities;
using Marvin.Cache.Headers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Text;
using Kremis.Infrastructure;
using Kremis.Infrastructure.Contracts;
using Kremis.Domain.Contexts;
using Kremis.Mvc.ActionFilters;
using Kremis.BusinessLogic.Queries.Contracts;
using Kremis.BusinessLogic.Commands.Contracts;
using Microsoft.EntityFrameworkCore;
using Kremis.BusinessLogic.Queries;
using Kremis.Utility.Enum;
using Microsoft.AspNetCore.Identity.UI.Services;
using Kremis.Utility.Options;
using Microsoft.Extensions.Options;
using Kremis.Utility.Options.Validations;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace Kremis.Mvc.Extensions
{
    public static class MvcServiceExtensions
    {
        public static void ConfigureOptions(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<LoggingOptions>(configuration.GetSection(LoggingOptions.ConfigSectionName));

            services.Configure<EmailOptions>(configuration.GetSection(EmailOptions.ConfigSectionName));
            services.TryAddSingleton<IValidateOptions<EmailOptions>, EmailOptionsValidation>();

            services.Configure<DbOptions>(configuration.GetSection(DbOptions.ConfigSectionName));
            services.TryAddSingleton<IValidateOptions<DbOptions>, DbOptionsValidation>();

            services.Configure<SecurityOptions>(configuration.GetSection(SecurityOptions.ConfigSectionName));
            services.TryAddSingleton<IValidateOptions<SecurityOptions>, SecurityOptionsValidation>();

            services.Configure<CompanySettingsOptions>(configuration.GetSection(CompanySettingsOptions.ConfigSectionName));
        }

        public static void ConfigureSecurity(this IServiceCollection services, IConfiguration configuration)
        {
            SecurityOptions securityOptions = new();
            configuration.Bind(SecurityOptions.ConfigSectionName, securityOptions);

            services.AddIdentity<ApplicationUser, ApplicationRole>(opt =>
            {
                opt.Password.RequiredLength = securityOptions.RequiredLength.GetValueOrDefault();
                opt.Password.RequireDigit = securityOptions.RequireDigit;
                opt.Password.RequireUppercase = securityOptions.RequireUppercase;
                opt.Password.RequireLowercase = securityOptions.RequireLowercase;
                opt.Password.RequireNonAlphanumeric = securityOptions.RequireNonAlphanumeric;
                opt.Password.RequiredUniqueChars = securityOptions.RequiredUniqueChars.GetValueOrDefault();
                opt.User.RequireUniqueEmail = securityOptions.RequireUniqueEmail;
                opt.Lockout.MaxFailedAccessAttempts = securityOptions.MaxFailedAccessAttempts.GetValueOrDefault();
                opt.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromHours(securityOptions.DefaultLockoutTimeSpan.GetValueOrDefault());
                opt.SignIn.RequireConfirmedAccount = securityOptions.RequireConfirmedAccount;
                opt.SignIn.RequireConfirmedEmail = securityOptions.RequireConfirmedEmail;
                opt.SignIn.RequireConfirmedPhoneNumber = securityOptions.RequireConfirmedPhoneNumber;
            })
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();//Enabled generation of token for Password's reset and more

            services.Configure<DataProtectionTokenProviderOptions>(opt =>
            {
                opt.TokenLifespan = TimeSpan.FromHours(2);//Validity of a generated token
            });

        }

        public static void ConfigureDbContexts(this IServiceCollection services, IConfiguration configuration)
        {
            DbOptions dbOptions = new();
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

        public static void ConfigureBusinessServices(this IServiceCollection services)
        {
            services.AddScoped<ICityQuery, CityQuery>();

            services.AddScoped<IPaymentModeQuery, PaymentModeQuery>();

            services.AddScoped<ILocalityQuery, LocalityQuery>();
            services.AddScoped<ILocalityCommand, LocalityCommand>();

            services.AddScoped<ICustomerQuery, CustomerQuery>();
            services.AddScoped<ICustomerCommand, CustomerCommand>();

            services.AddScoped<IParcelQuery, ParcelQuery>();
            services.AddScoped<IParcelCommand, ParcelCommand>();

            services.AddScoped<ILandTitleQuery, LandTitleQuery>();
            services.AddScoped<ILandTitleCommand, LandTitleCommand>();

            services.AddScoped<IBusinessPartnerQuery, BusinessPartnerQuery>();
            services.AddScoped<IBusinessPartnerCommand, BusinessPartnerCommand>();

            services.AddScoped<IApplicationUserQuery, ApplicationUserQuery>();

            services.AddScoped<ILandTitleDocumentQuery, LandTitleDocumentQuery>();
            services.AddScoped<ILandTitleDocumentCommand, LandTitleDocumentCommand>();

            services.AddScoped<IDocumentTypeQuery, DocumentTypeQuery>();

            services.AddScoped<IIdentityDocumentTypeQuery, IdentityDocumentTypeQuery>();

            services.AddScoped<ICustomerDocumentQuery, CustomerDocumentQuery>();
            services.AddScoped<ICustomerDocumentCommand, CustomerDocumentCommand>();

            services.AddScoped<IInvoiceDetailQuery, InvoiceDetailQuery>();
            services.AddScoped<IInvoiceDetailCommand, InvoiceDetailCommand>();

            services.AddScoped<IInvoiceHeaderQuery, InvoiceHeaderQuery>();
            services.AddScoped<IInvoiceHeaderCommand, InvoiceHeaderCommand>();

            services.AddScoped<IInvoicePaymentQuery, InvoicePaymentQuery>();
            services.AddScoped<IInvoicePaymentCommand, InvoicePaymentCommand>();

            services.AddScoped<ICommissionPaymentQuery, CommissionPaymentQuery>();
            services.AddScoped<ICommissionPaymentCommand, CommissionPaymentCommand>();

            services.AddScoped<IParcelDocumentQuery, ParcelDocumentQuery>();
            services.AddScoped<IParcelDocumentCommand, ParcelDocumentCommand>();
        }

        public static void ConfigureInfrastructureServices(this IServiceCollection services)
        {
            services.AddScoped<ILoggerService, LoggerService>();
            services.AddScoped<IEmailSender, EmailService>();
        }

        public static void ConfigureControllers(this IServiceCollection services)
        {
            services.AddControllersWithViews(options =>
                    options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute()));
            services.AddRazorPages();
        }

        public static void ConfigureCors(this IServiceCollection services) =>
            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy", builder =>
                      builder.AllowAnyOrigin()
                      .AllowAnyMethod()
                      .AllowAnyHeader());
            });

        public static void ConfigureIISIntegration(this IServiceCollection services) =>
            services.Configure<IISOptions>(options => { });

        public static void ConfigureAutoMapper(this IServiceCollection services) =>
            services.AddAutoMapper(typeof(Startup));

        public static void ConfigureActionFilters(this IServiceCollection services) =>
            services.AddScoped<ValidationFilterAttribute>();

        public static void ConfigureVersioning(this IServiceCollection services)
        {
            services.AddApiVersioning(opt =>
            {

                opt.ReportApiVersions = true;
                opt.AssumeDefaultVersionWhenUnspecified = true;
                opt.DefaultApiVersion = new ApiVersion(1, 0);
                opt.ApiVersionReader = new HeaderApiVersionReader("api-version");
            });
        }

        public static void ConfigureResponseCaching(this IServiceCollection services)
            => services.AddResponseCaching();

        public static void ConfigureHttpCacheHeaders(this IServiceCollection services)
            => services.AddHttpCacheHeaders(
                (expirationOpt) =>
                {
                    expirationOpt.MaxAge = 65;
                    expirationOpt.CacheLocation = CacheLocation.Private;
                },
                (validationOpt) =>
                {
                    validationOpt.MustRevalidate = true;
                }
                );

        public static void ConfigureRateLimitingOptions(this IServiceCollection services)
        {
            List<RateLimitRule> rateLimitRules = new()
            {
                                    new RateLimitRule
                                    {
                                        Endpoint = "*",
                                        Limit= 30,
                                        Period = "5m"
                                    }
                                };
            services.Configure<IpRateLimitOptions>(opt =>
            {
                opt.GeneralRules = rateLimitRules;
            });
            services.AddSingleton<IRateLimitCounterStore, MemoryCacheRateLimitCounterStore>();
            services.AddSingleton<IIpPolicyStore, MemoryCacheIpPolicyStore>();
            services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();
        }

        public static void ConfigureJWT(this IServiceCollection services, IConfiguration configuration)
        {
            IConfigurationSection jwtSettings = configuration.GetSection("JwtSettings");
            string secretKey = "KremisSecretKey";// Environment.GetEnvironmentVariable("SECRET");
            services.AddAuthentication(opt =>
            {
                opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtSettings.GetSection("validIssuer").Value,
                    ValidAudience = jwtSettings.GetSection("validAudience").Value,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
                };
            });
        }

        public static void ConfigureSwagger(this IServiceCollection services)
        {
            services.AddSwaggerGen(s =>
            {
                s.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Kremis API",
                    Version = "v1",
                    Description = "Kremis.Mvc",
                    TermsOfService = new Uri("https://example.com/terms"),
                    Contact = new OpenApiContact
                    {
                        Name = "Armand K",
                        Email = "armand@gmail.com",
                        Url = new Uri("https://twitter.com/johndoe"),
                    },
                    License = new OpenApiLicense
                    {
                        Name = "Kremis.Mvc API LICX",
                        Url = new Uri("https://example.com/license"),
                    }
                });
                s.SwaggerDoc("v2", new OpenApiInfo
                {
                    Title = "Kremis API",
                    Version = "v2"
                });
                s.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Description = "Place to add JWT with Bearer",
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });
                s.AddSecurityRequirement(new OpenApiSecurityRequirement()
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            },
                            Name = "Bearer",
                        },
                        new List<string>()
                    }
                });
            });
        }

        public static void ConfigureSessions(this IServiceCollection services)
        {
            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30);
            });
        }

        public static void ConfigureFacebookAuthentication(this IServiceCollection services)
        {
            services.AddAuthentication().AddFacebook(options =>
            {
                options.AppId = "285941236666819";
                options.AppSecret = "22abe406faa151c5a05af61b620a1376";
            });
        }

        public static void ConfigureGoogleAuthentication(this IServiceCollection services)
        {
            services.AddAuthentication().AddGoogle(options =>
            {
                options.ClientId = "910515816196-jve1ijo4k4kpg9tmte45qehb7qn0o79d.apps.googleusercontent.com";
                options.ClientSecret = "0fklW-vlEDB4h117DCDYsjMX";

            });
        }

        public static void ConfigureApplicationCookie(this IServiceCollection services)
        {
            services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = $"/Identity/Account/Login";
                options.LogoutPath = $"/Identity/Account/Logout";
                options.AccessDeniedPath = $"/Identity/Account/AccessDenied";
            });
        }

        public static void ConfigureTempData(this IServiceCollection services)
        {
            services.AddSingleton<ITempDataProvider, CookieTempDataProvider>();
        }
    }
}
