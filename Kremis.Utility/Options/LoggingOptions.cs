
using System;
using System.IO;
using Microsoft.AspNetCore.Hosting;

namespace Kremis.Utility.Options
{
    public class LoggingOptions
    {
        public const string ConfigSectionName = "LoggingOptions";
        public string LogFileName { get; set; }
    }
}
