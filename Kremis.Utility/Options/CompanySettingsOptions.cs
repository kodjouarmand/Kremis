using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kremis.Utility.Options
{
    public class CompanySettingsOptions
    {
        public const string ConfigSectionName = "CompanySettingsOptions";

        public double? ParcellingCosts { get; set; }
        public double? TechnicalFileCosts { get; set; }
        public double? BoundaryCosts { get; set; }
        public double? DownPaymentMinimumRate { get; set; }
        public int? PaymentDelay { get; set; }
    }
}
