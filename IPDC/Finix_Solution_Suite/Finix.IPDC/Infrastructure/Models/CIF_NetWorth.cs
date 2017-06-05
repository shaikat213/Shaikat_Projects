using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finix.IPDC.Infrastructure.Models
{
    [Table("CIF_NetWorth")]
    public class CIF_NetWorth : Entity
    {
        public long CIF_PersonalId { get; set; }
        //details of assets and savings
        public decimal? CashAtHand { get; set; }
        public virtual ICollection<CIF_SavingsInBank> SavingsInBank { get; set; }
        public virtual ICollection<CIF_Investment> Investments { get; set; }
        public decimal? OtherSavings { get; set; }
        public virtual ICollection<CIF_NW_Property> Properties { get; set; }
        public virtual ICollection<CIF_BusinessShares> BusinessShares { get; set; }        
        public decimal? TotalAsset { get; set; }
        //details of borrowing and liabilities
        public virtual ICollection<CIF_Liability> Liabilities { get; set; }
        public decimal? OfficeLoan { get; set; }
        public decimal? UnpaidTaxes { get; set; }
        public decimal? OtherLiabilities { get; set; }
        public decimal? TotalLiabilities { get; set; }
        public decimal? NetWorth { get; set; }//asset-liability
        [ForeignKey("CIF_PersonalId"), InverseProperty("NetWorths")]
        public virtual CIF_Personal CIF_Personal { get; set; }

        public virtual ICollection<NetWorthVerification> NetWorthVerifications { get; set; }
    }

    [Table("CIF_SavingsInBank")]
    public class CIF_SavingsInBank : Entity
    {
        public long CIF_NetWorthId { get; set; }

        public string BankName { get; set; }
        public BankDepositType? BankDepositType { get; set; }
        public decimal? CurrentBalance { get; set; }
        [ForeignKey("CIF_NetWorthId"), InverseProperty("SavingsInBank")]
        public virtual CIF_NetWorth CIF_NetWorth { get; set; }
    }
    [Table("CIF_Investment")]
    public class CIF_Investment : Entity
    {
        public long CIF_NetWorthId { get; set; }
        public InvestmentType InvestmentType { get; set; }
        public decimal Amount { get; set; }
        [ForeignKey("CIF_NetWorthId"), InverseProperty("Investments")]
        public virtual CIF_NetWorth CIF_NetWorth { get; set; }
    }
    [Table("CIF_NW_Property")]
    public class CIF_NW_Property : Entity
    {
        public long CIF_NetWorthId { get; set; }
        public string Description { get; set; }
        public decimal MarketValue { get; set; }
        public bool? Encumbered { get; set; }
        [ForeignKey("CIF_NetWorthId"), InverseProperty("Properties")]
        public virtual CIF_NetWorth CIF_NetWorth { get; set; }
    }
    [Table("CIF_BusinessShares")]
    public class CIF_BusinessShares : Entity
    {
        public long CIF_NetWorthId { get; set; }
        public BusinessShareType BusinessShareType { get; set; }
        public decimal ValueOfOwnership { get; set; }
        [ForeignKey("CIF_NetWorthId"), InverseProperty("BusinessShares")]
        public virtual CIF_NetWorth CIF_NetWorth { get; set; }
    }
    [Table("CIF_Liability")]
    public class CIF_Liability : Entity
    {
        public long CIF_NetWorthId { get; set; }
        public LoanType? LoanType { get; set; }
        public decimal? LoanAmountOrLimit { get; set; }
        public decimal? InstallmentAmount { get; set; }
        public int? Term { get; set; }//in months
        public int? RemainingTerm { get; set; }
        public decimal? OutstandingAmount { get; set; }
        public string BankOrFIName { get; set; }
        public LiabilityType LiabilityType { get; set; }

        [ForeignKey("CIF_NetWorthId"), InverseProperty("Liabilities")]
        public virtual CIF_NetWorth CIF_NetWorth { get; set; }
    }

    
}
