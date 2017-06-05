using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Finix.IPDC.Infrastructure;
using Finix.IPDC.Infrastructure.Models;

namespace Finix.IPDC.DTO
{
    public class LPPrimarySecurityDto
    {
        public long Id { get; set; }
        public long LoanApplicationId { get; set; }
        //public virtual LoanApplication LoanApplication { get; set; }
        public LandedPropertyLoanType? LandedPropertyLoanType { get; set; }
        public string LandedPropertyLoanTypeName { get; set; }
        public decimal? TotalCost { get; set; }
        public decimal? AmountPaid { get; set; }
        public decimal? RemainingClientContribution { get; set; }//total cost - amound paid - loan amount applied
        public string SourceOfRemainingFund { get; set; }
        public DateTime? FirstDisbursementExpDate { get; set; }
        public string FirstDisbursementExpDateText { get; set; }
        public LandedPropertySellertype LandedPropertySellertype { get; set; }
        public LandType? LandType { get; set; }
        public string LandTypeName { get; set; }
        public string SellerName { get; set; }
        public string SellerPhone { get; set; }
        public long? PropertyAddressId { get; set; }
        public AddressDto PropertyAddress { get; set; }
        public AddressDto ProjectAddress { get; set; }
        public long? DeveloperId { get; set; }
        public string DeveloperName { get; set; }
        public string ContactPerson { get; set; }
        public string ContactPersonDesignation { get; set; }
        public string ContactPersonPhone { get; set; }
        //public virtual Developer Developer { get; set; }
        public long? ProjectId { get; set; }
        public string ProjectName { get; set; }
        //public virtual Project Project { get; set; }
        public decimal? FlatSize { get; set; }
        public int? FloorNo { get; set; }
        public string FlatSide { get; set; }


        //takeover
        public string TakeoverFrom { get; set; }
        public decimal? PrevCompanyApprovedLoan { get; set; }
        public decimal? PrevCompanyOutstandingLoan { get; set; }
        public decimal? TopUpLoan { get; set; }
        public decimal? CurrentInterestRate { get; set; }

        //home equity
        public string Owner { get; set; }
        public decimal? TotalPropertyValue { get; set; }

        //others
        public string CurrentWorkingStage { get; set; }
        public int? CompletedFloors { get; set; }
        public int? ProposedFloors { get; set; }
        public decimal? EstimatedConstructionCost { get; set; }
        public DateTime? CreateDate { get; set; }
        public long? CreatedBy { get; set; }
        public DateTime? EditDate { get; set; }
        public long? EditedBy { get; set; }
        public EntityStatus? Status { get; set; }
        public List<LPPrimarySecurityValuationDto> Valuations { get; set; }
    }
}
