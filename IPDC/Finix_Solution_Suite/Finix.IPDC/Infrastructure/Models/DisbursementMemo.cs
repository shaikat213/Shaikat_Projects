using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finix.IPDC.Infrastructure.Models
{
    [Table("DisbursementMemo")]
    public class DisbursementMemo : Entity
    {
        public string DMNo { get; set; }
        public DateTime? DMDate { get; set; }
        public long? ApplicationId { get; set; }
        [ForeignKey("ApplicationId")]
        public virtual Application Application { get; set; }
        public long? ProposalId { get; set; }
        [ForeignKey("ProposalId")]
        public virtual Proposal Proposal { get; set; }
        public long? ParentId { get; set; }
        public virtual DisbursementMemo ParentMemo { get; set; }
        public bool? IsPartial { get; set; }
        public string TrenchNo { get; set; }
        public decimal TotalLoanAmount { get; set; }
        public decimal CurrentDisbursementAmount { get; set; }
        public decimal TotalDisbursedAmount { get; set; }
        public virtual ICollection<DMText> Texts { get; set; }
        public bool? IsApproved { get; set; }
        public DateTime? ApprovalDate { get; set; }
        public long? ApprovedByEmpId { get; set; }
        [ForeignKey("ApprovedByEmpId")]
        public virtual Employee ApprovedByEmp { get; set; }
        public bool? IsDisbursed { get; set; }
        public DateTime? DisbursedDate { get; set; }
        public long? DisbursedByEmpId { get; set; }
        [ForeignKey("DisbursedByEmpId")]
        public virtual Employee DisbursedByEmp { get; set; }
        public virtual ICollection<Disbursment_Signatory> Signatories { get; set; }
        public virtual ICollection<DMDetail> DisbursementDetails { get; set; }

    }
    [Table("DMText")]
    public class DMText : Entity
    {
        public long DMId { get; set; }
        [ForeignKey("DMId"), InverseProperty("Texts")]
        public virtual DisbursementMemo DM { get; set; }
        public DisbursementTextType DisbursementTextType { get; set; }
        public string Text { get; set; }
    }
    [Table("DMDetail")]
    public class DMDetail : Entity
    {
        public long DisbursementMemoId { get; set; }
        [ForeignKey("DisbursementMemoId"), InverseProperty("DisbursementDetails")]
        public virtual DisbursementMemo DisbursementMemo { get; set; }
        public DisbursementMode DisbursementMode { get; set; }
        public LoanChequeDeliveryOptions? ChequeDeliveryOption { get; set; }
        public decimal DisbursementAmount { get; set; }
        public long? IPDCBankAccountId { get; set; }
        [ForeignKey("IPDCBankAccountId")]
        public IPDCBankAccounts IPDCBankAccounts { get; set; } // disbursed from
        public string BankName { get; set; }    // disbursed to
        public string BranchName { get; set; }    // disbursed to
        public string RoutingNo { get; set; }    // disbursed to
        public string AccountName { get; set; }    // disbursed to
        public string AccountNo { get; set; }    // disbursed to
        public string ChequeNo { get; set; }    //ipdc cheque no
        public string ChequeIssuedTo { get; set; }
        public DateTime? ChequeDate { get; set; }
        public string ClientAccountNo { get; set; }
    }
}
