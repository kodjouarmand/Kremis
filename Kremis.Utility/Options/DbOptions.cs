using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kremis.Utility.Options
{
    public class DbOptions
    {
        public const string ConfigSectionName = "DbOptions";

        public string ServerType { get; set; }
        public string SqliteConnectionStringName { get; set; }
        public string SqlServerConnectionStringName { get; set; }
    }
}
