using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Finix.IPDC.Infrastructure;
using Finix.IPDC.Infrastructure.Models;

namespace Finix.IPDC.DTO
{
    public class ProductDto 
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string ShortName { get; set; }
        public string Prefix { get; set; }
        public ProductType? ProductType { get; set; }
        public string ProductTypeName { get; set; }
        public DepositType? DepositType { get; set; }
        public string DepositTypeName { get; set; }
        public int MinTerm { get; set; }
        public int MaxTerm { get; set; }
        public decimal? MinAmount { get; set; }
        public decimal? MaxAmount { get; set; }
        public  List<ProductRatesDto> ProductRates { get; set; }
        public  List<ProductSpecialRateDto> ProductSpecialRate { get; set; }
        public  List<DPSMaturityScheduleDto> DPSMaturitySchedule { get; set; }
        public  List<DocumentSetupDto> DocumentSetups { get; set; }

        //for loan products
        public decimal? ApplicationFee { get; set; }
        public decimal? MaxProcessingFeeRate { get; set; }
        public decimal? MaxProcessingFeeAmount { get; set; }
        public decimal? MaxDocFeeRate { get; set; }
        public decimal? MaxDocFeeAmount { get; set; }
        public decimal? MinCIBCharge { get; set; }
        public  List<ProductSecurityDto> ProductSecurity { get; set; }
        public bool JointAccountAllowed { get; set; }
        //for deposit produt
        public bool FlexAmountAllowed { get; set; }
        public ProposalProduct? ProposalProduct { get; set; }
        public ProposalFacilityType? FacilityType { get; set; }
        public string FacilityTypeName { get; set; }
        public DateTime? CreateDate { get; set; }
        public long? CreatedBy { get; set; }
        public DateTime? EditDate { get; set; }
        public long? EditedBy { get; set; }
        public EntityStatus? Status { get; set; }
        public List<long> RemovedSecurities { get; set; }
        public List<long> RemovedProductRates { get; set; }
        public List<long> RemovedSpclProductRates { get; set; }
        public List<long> RemovedDPSMaturitySchedule { get; set; }
        public List<long> RemovedDocumentSetup { get; set; }

    }
}
