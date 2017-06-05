using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finix.IPDC.Infrastructure.Models
{
    [Table("DocumentCheckList")]
    public class DocumentCheckList : Entity
    {
        public string DCLNo { get; set; }
        public DateTime? DCLDate { get; set; }
        public long ApplicationId { get; set; }
        [ForeignKey("ApplicationId")]
        public virtual Application Application { get; set; }
        public long? ProposalId { get; set; }
        [ForeignKey("ProposalId")]
        public virtual Proposal Proposal { get; set; }
        public string ApplicationTitle { get; set; }
        public ProposalFacilityType? FacilityType { get; set; }
        public long? ProductId { get; set; }
        [ForeignKey("ProductId")]
        public virtual Product Product { get; set; }
        public int? Term { get; set; }
        public bool? IsApproved { get; set; }
        public virtual ICollection<DocumentCheckListDetail> Documents { get; set; }
        public virtual ICollection<DocumentCheckListException> Exceptions { get; set; }
        public virtual ICollection<DocumentSecurities> Securities { get; set; }
        public virtual ICollection<DCL_Signatory> Signatories { get; set; }
        public long? ApprovedByDegId { get; set; }
        [ForeignKey("ApprovedByDegId")]
        public virtual OfficeDesignationSetting OfficeDesignationSetting { get; set; }
        public long? ApprovedByEmpId { get; set; }
        [ForeignKey("ApprovedByEmpId")]
        public virtual Employee Employee { get; set; }
        public DateTime? ApprovalDate { get; set; }
        public string PreparedBy { get; set; }
    }
}
