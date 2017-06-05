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

    public class CIF_NetWorthDto
    {
        public long Id { get; set; }
        public long? CIF_PersonalId { get; set; }
        //details of assets and savings
        public decimal? CashAtHand { get; set; }
        public List<CIF_SavingsInBankDto> SavingsInBank { get; set; }
        public List<CIF_InvestmentDto> Investments { get; set; }
        public decimal? OtherSavings { get; set; }        
        public List<CIF_NW_PropertyDto> Properties { get; set; }
        public List<CIF_BusinessSharesDto> BusinessShares { get; set; }

        public decimal? TotalAsset { get; set; }

        //details of borrowing and liabilities
        public List<CIF_LiabilityDto> Liabilities { get; set; }
        public decimal? OfficeLoan { get; set; }
        public decimal? UnpaidTaxes { get; set; }
        public decimal? OtherLiabilities { get; set; }
        public decimal? TotalLiabilities { get; set; }
        public decimal? NetWorth { get; set; }//asset-liability
        public List<long> RemovedSavings { get; set; }
        public List<long> RemovedShareinBusines { get; set; }
        public List<long> RemovedProperties { get; set; }
        public List<long> RemovedInvestments { get; set; }
        public List<long> RemovedLiabilities { get; set; }
        public CIF_PersonalDto CIF_Personal { get; set; }
        public DateTime? CreateDate { get; set; }
        public long? CreatedBy { get; set; }
        public DateTime? EditDate { get; set; }
        public long? EditedBy { get; set; }
        public EntityStatus Status { get; set; }
    }

    public class CIF_SavingsInBankDto
    {
        public long Id { get; set; }
        public long? CIF_NetWorthId { get; set; }
        public string BankName { get; set; }
        public BankDepositType? BankDepositType { get; set; }
        public decimal? CurrentBalance { get; set; }        
        //public CIF_NetWorthDto CIF_NetWorth { get; set; }
        public long? CreatedBy { get; set; }
        public EntityStatus? Status { get; set; }
    }
    public class CIF_InvestmentDto
    {
        public long Id { get; set; }
        public long? CIF_NetWorthId { get; set; }
        public InvestmentType InvestmentType { get; set; }
        public decimal? Amount { get; set; }
        
        //public CIF_NetWorthDto CIF_NetWorth { get; set; }
        public long? CreatedBy { get; set; }
        public EntityStatus? Status { get; set; }

    }

    public class CIF_NW_PropertyDto
    {
        public long Id { get; set; }
        public long? CIF_NetWorthId { get; set; }
        public string Description { get; set; }
        public decimal? MarketValue { get; set; }
        public bool? Encumbered { get; set; }
        //public CIF_NetWorthDto CIF_NetWorth { get; set; }
        public long? CreatedBy { get; set; }
        public EntityStatus? Status { get; set; }

    }

    public class CIF_BusinessSharesDto
    {
        public long Id { get; set; }
        public long? CIF_NetWorthId { get; set; }
        public BusinessShareType BusinessShareType { get; set; }
        public decimal? ValueOfOwnership { get; set; }        
        //public CIF_NetWorthDto CIF_NetWorth { get; set; }
        public long? CreatedBy { get; set; }
        public EntityStatus? Status { get; set; }
    }

    public class CIF_LiabilityDto
    {
        public long Id { get; set; }
        public long? CIF_NetWorthId { get; set; }
        public LoanType? LoanType { get; set; }
        public decimal? LoanAmountOrLimit { get; set; }
        public decimal? InstallmentAmount { get; set; }
        public int? Term { get; set; }//in months
        public int? RemainingTerm { get; set; }
        public decimal? OutstandingAmount { get; set; }
        public string BankOrFIName { get; set; }
        public LiabilityType LiabilityType { get; set; }
        //public CIF_NetWorthDto CIF_NetWorth { get; set; }
        public long? CreatedBy { get; set; }
        public EntityStatus? Status { get; set; }
    }


}
