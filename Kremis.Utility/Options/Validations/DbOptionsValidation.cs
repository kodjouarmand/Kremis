using System.Text;
using Kremis.Utility.Enum;
using Microsoft.Extensions.Options;

namespace Kremis.Utility.Options.Validations
{
    public class DbOptionsValidation : IValidateOptions<DbOptions>
    {
        public ValidateOptionsResult Validate(string name, DbOptions settings)
        {
            if (string.IsNullOrWhiteSpace(settings.ServerType)
                ||settings.ServerType != DbServerTypeEnum.Sqlite.ToString()  
                || settings.ServerType != DbServerTypeEnum.SqlServer.ToString())
            {
                return ValidateOptionsResult.Fail("\"ServerType\" setting is required in the Db settings of configuration file, and should be" +
                    "of \"Sqlite\" or \"SqlServer\".");
            }
            if (settings.ServerType == DbServerTypeEnum.SqlServer.ToString()
                && string.IsNullOrWhiteSpace(settings.SqlServerConnectionStringName))
            {
                return ValidateOptionsResult.Fail("\"SqlServerConnectionStringName\" setting is required in the Db settings of configuration file.");
            }

            if (settings.ServerType == DbServerTypeEnum.Sqlite.ToString()
                 && string.IsNullOrWhiteSpace(settings.SqliteConnectionStringName))
            {
                return ValidateOptionsResult.Fail("\"SqlServerConnectionStringName\" setting is required in the Db settings of configuration file.");
            }

            return ValidateOptionsResult.Success;
        }
    }
}
