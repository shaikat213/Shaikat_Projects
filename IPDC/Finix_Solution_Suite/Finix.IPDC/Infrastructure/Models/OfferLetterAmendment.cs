using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Finix.IPDC.Infrastructure.Models
{
    [Table("OfferLetterAmendment")]
    public class OfferLetterAmendment : Entity
    {
        public long OfferLetterId { get; set; }
        [ForeignKey("OfferLetterId"), InverseProperty("Amendments")]
        public virtual OfferLetter OfferLetter { get; set; }
        public bool IsApproved { get; set; }
        public DateTime ApprovalDate { get; set; }
        public decimal LoanAmount { get; set; }
        public virtual ICollection<OfferLetterAmendmentTexts> Conditions { get; set; }

    }
    [Table("OfferLetterAmendmentTexts")]
    public class OfferLetterAmendmentTexts : Entity
    {
        public long OLAId { get; set; }
        [ForeignKey("OLAId"), InverseProperty("Conditions")]
        public virtual OfferLetterAmendment OfferLetterAmendment { get; set; }
        public string Particular { get; set; }
        public string ExistingContition { get; set; }
        public string RevisedCondition { get; set; }
    }
}
