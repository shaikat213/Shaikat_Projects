using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finix.IPDC.Util
{
    public class IPDCSystem
    {
        public static string SeedDataPath { get; set; }

        public static bool DropDB
        {
            get
            {
                var v = ConfigReader.GetAppSetting("drop_db_on_seed_data_exception");
                return !string.IsNullOrWhiteSpace(v) && v.ToLower() == "true";
            }
        }

        public static DateTime SystemDate
        {
            get { return DateTime.Now; }
        }
    }
}
