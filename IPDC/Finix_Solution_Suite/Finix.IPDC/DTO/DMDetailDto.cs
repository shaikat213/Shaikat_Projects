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
    public class DMDetailDto
    {
        public long? Id { get; set; }
        public long DisbursementMemoId { get; set; }
        //public virtual DisbursementMemo DisbursementMemo { get; set; }
        public DisbursementMode DisbursementMode { get; set; }
        public LoanChequeDeliveryOptions? ChequeDeliveryOption { get; set; }
        public decimal DisbursementAmount { get; set; }
        public long? IPDCBankAccountId { get; set; }
        public string BankName { get; set; }    // disbursed to
        public string BranchName { get; set; }    // disbursed to
        public string RoutingNo { get; set; }    // disbursed to
        public string AccountName { get; set; }    // disbursed to
        public string AccountNo { get; set; }    // disbursed to
        public string ChequeNo { get; set; }    //ipdc cheque no
        public string ChequeIssuedTo { get; set; }
        public DateTime? ChequeDate { get; set; }
        public string ChequeDateTxt { get; set; }
        public string ClientAccountNo { get; set; }
        public DateTime? CreateDate { get; set; }
        public long? CreatedBy { get; set; }
        public DateTime? EditDate { get; set; }
        public long? EditedBy { get; set; }
        public EntityStatus? Status { get; set; }
    }
}
