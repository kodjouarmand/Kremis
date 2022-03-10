using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Kremis.Domain.Contexts;
using Kremis.Domain.Entities;
using Kremis.Infrastructure.Contracts;
using Kremis.Utility.Helpers;
using Kremis.Utility.Options;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace Kremis.Api.Initializer
{
    public class DbInitializer : IDbInitializer
    {
        private readonly ILoggerService _logger;
        private readonly ApplicationDbContext _dbContext;
        private readonly IWebHostEnvironment _hostEnvironment;
        private readonly IOptions<SuperAministratorOptions> _superAministratorOptions;

        public DbInitializer(ILoggerService logger, ApplicationDbContext dbContext, IWebHostEnvironment hostEnvironment,
             IOptions<SuperAministratorOptions> superAministratorOptions)
        {
            _logger = logger;
            _dbContext = dbContext;
            _hostEnvironment = hostEnvironment;
            _superAministratorOptions = superAministratorOptions;
        }

        public void Initialize(bool ensureDeleted = false)
        {
            try
            {
                Migrate(ensureDeleted);

                _logger.LogInfo($"Database initialization start ...");
                InitializeSuperAdministrator(_superAministratorOptions);
                InitializeApplicationUser();
                InitializeApplicationRole();
                InitializeApplicationUserRole();
                InitializeCity();
                InitializeDocumentType();
                InitializeIdentityDocumentType();
                if (_hostEnvironment.IsDevelopment())
                {

                }
                SaveChanges();
                _logger.LogInfo($"Database initialization end with success ...");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Database initialization failed...");
                _logger.LogError(ex);
                _logger.LogDebug($"Database initialization failed : {ex.Message}");
            }
        }

        private void Migrate(bool ensureDeleted)
        {
            if (ensureDeleted)
            {
                _dbContext.Database.EnsureDeleted();
            }
            if (_dbContext.Database.GetPendingMigrations().Any())
            {
                _logger.LogInfo($"Database migration start ...");
                _dbContext.Database.Migrate();
                _logger.LogInfo($"Database migration end with success ...");
            }
        }

        private void InitializeCity()
        {
            var citys = GetInitData<City>(nameof(City)).OrderBy(u => u.Name).ToList();
            if (!_dbContext.Cities.Any())
            {
                _dbContext.AddRange(citys);
            }
            else
            {
                foreach (var city in citys)
                {
                    if (!_dbContext.Cities.Any(u => u.Name == city.Name))
                    {
                        _dbContext.Add(city);
                    }
                }
            }
        }

        private void InitializeDocumentType()
        {
            var documentTypes = GetInitData<DocumentType>(nameof(DocumentType)).OrderBy(u => u.Name).ToList();
            if (!_dbContext.DocumentTypes.Any())
            {
                _dbContext.AddRange(documentTypes);
            }
            else
            {
                foreach (var documentType in documentTypes)
                {
                    if (!_dbContext.DocumentTypes.Any(u => u.Name == documentType.Name))
                    {
                        _dbContext.Add(documentType);
                    }
                }
            }
        }

        private void InitializeIdentityDocumentType()
        {
            var identitityDocumentTypes = GetInitData<IdentityDocumentType>(nameof(IdentityDocumentType)).OrderBy(u => u.Name).ToList();
            if (!_dbContext.IdentityDocumentTypes.Any())
            {
                _dbContext.AddRange(identitityDocumentTypes);
            }
            else
            {
                foreach (var identitityDocumentType in identitityDocumentTypes)
                {
                    if (!_dbContext.IdentityDocumentTypes.Any(u => u.Name == identitityDocumentType.Name))
                    {
                        _dbContext.Add(identitityDocumentType);
                    }
                }
            }
        }

        private void InitializeApplicationRole()
        {
            if (!_dbContext.ApplicationRoles.Any())
            {
                var roles = GetInitData<ApplicationRole>(nameof(ApplicationRole));
                _dbContext.AddRange(roles);
            }
        }

        private void InitializeApplicationUser()
        {
            if (!_dbContext.ApplicationUsers.Any())
            {
                var users = GetInitData<ApplicationUser>(nameof(ApplicationUser));
                foreach (var user in users)
                {
                    user.NormalizedUserName = user.UserName.ToUpper();
                    user.NormalizedEmail = user.Email.ToUpper();
                    user.SecurityStamp = Guid.NewGuid().ToString("D");
                    user.PasswordHash = GeneratePassword(user, "Password123!");
                }
                _dbContext.AddRange(users);
            }
        }

        private void InitializeApplicationUserRole()
        {
            if (!_dbContext.ApplicationUserRoles.Any())
            {
                var userRoles = GetInitData<ApplicationUserRole>(nameof(ApplicationUserRole));
                _dbContext.AddRange(userRoles);
            }
        }

        private void InitializeSuperAdministrator(IOptions<SuperAministratorOptions> superAministratorOptions)
        {
            var superAministratorSettings = superAministratorOptions.Value;
            if (!_dbContext.ApplicationUsers.Any(u => u.Email.Equals(superAministratorSettings.Email)))
            {
                ApplicationUser superAdmin = new()
                {
                    Id = new Guid(superAministratorSettings.UserId),
                    UserName = superAministratorSettings.UserName,
                    NormalizedUserName = superAministratorSettings.UserName.ToUpper(),
                    Name = superAministratorSettings.Name,
                    Email = superAministratorSettings.Email,
                    NormalizedEmail = superAministratorSettings.Email.ToUpper(),
                    SecurityStamp = Guid.NewGuid().ToString("D"),
                    EmailConfirmed = true,
                    PhoneNumberConfirmed = true
                };
                superAdmin.PasswordHash = GeneratePassword(superAdmin, superAministratorSettings.Password);

                _dbContext.Add(superAdmin);
            }

            if (!_dbContext.ApplicationRoles.Any(u => u.Name.Equals(superAministratorSettings.RoleName)))
            {
                ApplicationRole superAdminRole = new()
                {
                    Id = new Guid(superAministratorSettings.RoleId),
                    Name = superAministratorSettings.RoleName,
                    NormalizedName = superAministratorSettings.RoleName.ToUpper()
                };
                _dbContext.Add(superAdminRole);
            }

            if (!_dbContext.ApplicationUserRoles.Any(u => u.UserId.ToString() == superAministratorSettings.UserId
                                                        && u.RoleId.ToString() == superAministratorSettings.RoleId))
            {
                ApplicationUserRole superAdminUserRole = new()
                {
                    RoleId = new Guid(superAministratorSettings.RoleId),
                    UserId = new Guid(superAministratorSettings.UserId)
                };
                _dbContext.Add(superAdminUserRole);
            }
        }

        private void SaveChanges()
        {
            _dbContext.SaveChanges();
        }

        private static string GeneratePassword(ApplicationUser user, string password)
        {
            var passHash = new PasswordHasher<ApplicationUser>();
            return passHash.HashPassword(user, password);
        }

        private List<T> GetInitData<T>(string fileName)
        {
            string fullFileName = GetInitDataDirectory(fileName);
            return FileHelper.GetJsonData<T>(fullFileName);
        }

        private string GetInitDataDirectory(string fileName)
        {
            if (_hostEnvironment.IsDevelopment())
            {
                fileName = $"{fileName}.Development.json";
            }
            else
            {
                fileName = $"{fileName}.Production.json";
            }
            string webRootPath = _hostEnvironment.WebRootPath;
            string directoryName = Path.Combine(webRootPath, ConstantHelper.INIT_DATA_PATH);
            if (!Directory.Exists(directoryName))
            {
                Directory.CreateDirectory(directoryName);
            }
            return Path.Combine(directoryName, fileName);
        }
    }
}
