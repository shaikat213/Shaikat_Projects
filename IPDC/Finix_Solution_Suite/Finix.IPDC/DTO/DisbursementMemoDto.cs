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
    public class DisbursementMemoDto
    {
        public long? Id { get; set; }
        public string DMNo { get; set; }
        public DateTime? DMDate { get; set; }
        public string DMDateTxt { get; set; }
        public long? ApplicationId { get; set; }
        public string ApplicationNo { get; set; }
        public string ProductName { get; set; }
        public long? ProposalId { get; set; }
        public long? ParentId { get; set; }
        public bool? IsPartial { get; set; }
        public string TrenchNo { get; set; }
        public decimal TotalLoanAmount { get; set; }
        public decimal CurrentDisbursementAmount { get; set; }
        public decimal TotalDisbursedAmount { get; set; }
        public List<DMTextDto> Texts { get; set; }
        public List<Disbursment_SignatoryDto> Signatories { get; set; }
        public List<long> RemovedTexts { get; set; }
        public List<long> RemovedSignatories { get; set; }
        public string AccountTitle { get; set; }
        public ProposalFacilityType? FacilityType { get; set; }
        public string FacilityTypeName { get; set; }
        public string BranchName { get; set; }
        public DateTime? ApplicattionDate { get; set; }
        public DateTime? CRMApprovalDate { get; set; }
        public decimal? LoanAmount { get; set; }
        public string DevelopersName { get; set; }
        public string ProjectName { get; set; }
        public bool? IsApproved { get; set; }
        public DateTime? ApprovalDate { get; set; }
        public string ApprovalDateTxt { get; set; }
        public long? ApprovedByEmpId { get; set; }
        public DateTime? CreateDate { get; set; }
        public long? CreatedBy { get; set; }
        public string CreatedByName { get; set; }
        public DateTime? EditDate { get; set; }
        public long? EditedBy { get; set; }
        public bool? IsDisbursed { get; set; }
        public DateTime? DisbursedDate { get; set; }
        public  List<DMDetailDto> DisbursementDetails { get; set; }
        public List<long> RemovedDMDetails { get; set; }
        public string DisbursedDateTxt { get; set; }
        public long? DisbursedByEmpId { get; set; }
        public EntityStatus Status { get; set; }
        
        //for reporting purpose only
        public string CreditMemoNo { get; set; }
        public string DCLNo { get; set; }
        public string CBSAccNo { get; set; }
        public string EmployeerName { get; set; }

    }
}
