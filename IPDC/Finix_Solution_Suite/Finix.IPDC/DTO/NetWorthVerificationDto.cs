using Finix.IPDC.Infrastructure;
using System;
using System.Collections.Generic;
using Finix.IPDC.Infrastructure.Models;

namespace Finix.IPDC.DTO
{
    public class NetWorthVerificationDto
    {
        public long? Id { get; set; }
        public long? NetWorthId { get; set; }
        //public CIF_NetWorthDto CifNetWorth { get; set; }
        public long? CIF_PersonalId { get; set; }
        public string CIFNo { get; set; }
        public string CIFName { get; set; }
        public long? ApplicationId { get; set; }
        public string ApplicationNo { get; set; }
        public string AccountTitle { get; set; }
        public VerificationAs? VerificationPersonRole { get; set; }
        public string VerificationPersonRoleName { get; set; }

        public decimal? CashAtHand { get; set; }
        public List<NWV_SavingsInBankDto> SavingsInBank { get; set; }
        public List<NWV_InvestmentDto> Investments { get; set; }
        public decimal? OtherSavings { get; set; }
        public List<NWV_PropertyDto> Properties { get; set; }
        public List<NWV_BusinessSharesDto> BusinessShares { get; set; }
        public decimal? TotalAsset { get; set; }
        //details of borrowing and liabilities
        public List<NWV_LiabilityDto> Liabilities { get; set; }
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

        public DateTime? VerificationDate { get; set; }
        public string VerificationDateTxt { get; set; }
        public long? VerifiedByUserId { get; set; }
        public long? VerifiedByEmpDegMapId { get; set; }
        public VerificationState VerificationState { get; set; }
        public string VerificationStateName { get; set; }
        public string Remarks { get; set; }
        public long? EmpId { get; set; }
        public DateTime? CreateDate { get; set; }
        public long? CreatedBy { get; set; }
        public DateTime? EditDate { get; set; }
        public long? EditedBy { get; set; }
        public EntityStatus Status { get; set; }
    }
}
