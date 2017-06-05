using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finix.IPDC.Infrastructure.Models
{
    [Table("NetWorthVerification")]
    public class NetWorthVerification : Entity
    {
        public long NetWorthId { get; set; }
        [ForeignKey("NetWorthId"), InverseProperty("NetWorthVerifications")]
        public virtual CIF_NetWorth CIF_NetWorth { get; set; }
        public long? CIF_PersonalId { get; set; }
        [ForeignKey("CIF_PersonalId")]
        public virtual CIF_Personal CIF { get; set; }
        public long? ApplicationId { get; set; }
        [ForeignKey("ApplicationId")]
        public virtual Application Application { get; set; }
        public VerificationAs VerificationPersonRole { get; set; }
        public decimal? CashAtHand { get; set; }
        public virtual ICollection<NWV_SavingsInBank> SavingsInBank { get; set; }
        public virtual ICollection<NWV_Investment> Investments { get; set; }
        public decimal? OtherSavings { get; set; }
        public virtual ICollection<NWV_Property> Properties { get; set; }
        public virtual ICollection<NWV_BusinessShares> BusinessShares { get; set; }
        public decimal? TotalAsset { get; set; }
        public virtual ICollection<NWV_Liability> Liabilities { get; set; }
        public decimal? OfficeLoan { get; set; }
        public decimal? UnpaidTaxes { get; set; }
        public decimal? OtherLiabilities { get; set; }
        public decimal? TotalLiabilities { get; set; }
        public decimal? NetWorth { get; set; }
        public DateTime? VerificationDate { get; set; }
        public long? VerifiedByUserId { get; set; }
        public long? VerifiedByEmpDegMapId { get; set; }
        public VerificationState VerificationState { get; set; }
        public string Remarks { get; set; }
        public long? EmpId { get; set; }
        [ForeignKey("EmpId")]
        public virtual Employee Employee { get; set; }
    }

    [Table("NWV_SavingsInBank")]
    public class NWV_SavingsInBank : Entity
    {
        public long NWV_NetWorthId { get; set; }

        public string BankName { get; set; }
        public BankDepositType? BankDepositType { get; set; }
        public decimal? CurrentBalance { get; set; }
        [ForeignKey("NWV_NetWorthId"), InverseProperty("SavingsInBank")]
        public virtual NetWorthVerification NetWorthVerification { get; set; }
    }
    [Table("NWV_Investment")]
    public class NWV_Investment : Entity
    {
        public long NWV_NetWorthId { get; set; }
        public InvestmentType InvestmentType { get; set; }
        public decimal Amount { get; set; }
        [ForeignKey("NWV_NetWorthId"), InverseProperty("Investments")]
        public virtual NetWorthVerification NetWorthVerification { get; set; }
    }
    [Table("NWV_Property")]
    public class NWV_Property : Entity
    {
        public long NWV_NetWorthId { get; set; }
        public string Description { get; set; }
        public decimal MarketValue { get; set; }
        public bool? Encumbered { get; set; }
        [ForeignKey("NWV_NetWorthId"), InverseProperty("Properties")]
        public virtual NetWorthVerification NetWorthVerification { get; set; }
    }
    [Table("NWV_BusinessShares")]
    public class NWV_BusinessShares : Entity
    {
        public long NWV_NetWorthId { get; set; }
        public BusinessShareType BusinessShareType { get; set; }
        public decimal ValueOfOwnership { get; set; }
        [ForeignKey("NWV_NetWorthId"), InverseProperty("BusinessShares")]
        public virtual NetWorthVerification NetWorthVerification { get; set; }
    }
    [Table("NWV_Liability")]
    public class NWV_Liability : Entity
    {
        public long NWV_NetWorthId { get; set; }
        public LoanType? LoanType { get; set; }
        public decimal? LoanAmountOrLimit { get; set; }
        public decimal? InstallmentAmount { get; set; }
        public int? Term { get; set; }//in months
        public int? RemainingTerm { get; set; }
        public decimal? OutstandingAmount { get; set; }
        public string BankOrFIName { get; set; }
        public LiabilityType LiabilityType { get; set; }

        [ForeignKey("NWV_NetWorthId"), InverseProperty("Liabilities")]
        public virtual NetWorthVerification NetWorthVerification { get; set; }
    }
}
