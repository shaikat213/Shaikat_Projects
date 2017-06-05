using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Finix.IPDC.Infrastructure.Models;

namespace Finix.IPDC.DTO
{
    public class FDRPrimarySecurityDto
    {
        public long Id { get; set; }
        public long LoanApplicationId { get; set; }
        //public virtual LoanApplication LoanApplication { get; set; }
        public List<FDRPSDetailDto> FDRPSDetails { get; set; }
    }
}
