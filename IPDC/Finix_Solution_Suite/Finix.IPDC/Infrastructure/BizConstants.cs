using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finix.IPDC.Infrastructure
{
    public static class BizConstants
    {
        //public const string CashInHand = "1020101001";
        public const long Bangladesh = 1;
        public const long BmRoleId = 7;
        public const string CardRateChart = "D:\\IPDC_published_2017_02_02\\IPDC\\UploadedFiles\\RateChart\\Rate_5_April.compressed.pdf";//"C:\\IPDC\\Finix_Solution_Suite\\Finix.UI\\UploadedFiles\\ProjectTechical\\Project for HL Test\\Project for HL Test_2017_04_15_001.sql";
        //public const string CardRateChart = "C:\\IPDC\\Finix_Solution_Suite\\Finix.UI\\UploadedFiles\\ProjectTechical\\Project for HL Test\\Project for HL Test_2017_04_15_001.sql";
        public const string ApiKey = "abcd123";

        public const long IPDCHeadOfficeId = 1;
        public static readonly List<long> ProductHeadRoles = new List<long>(new List<long> {
            39, 40, 41
        });
    }
}
