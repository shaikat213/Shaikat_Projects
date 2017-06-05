using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finix.IPDC.Infrastructure.Models
{
    [Table("Occupation")]
    public class Occupation : Entity
    {
        public OccupationType? OccupationType { get; set; }
        public EmploymentStatus? EmploymentStatus { get; set; }
        public long? ProfessionId { get; set; }
        [ForeignKey("ProfessionId")]
        public virtual Profession Profession { get; set; }
        public string Designation { get; set; }
        public long? OrganizationId { get; set; }
        public bool? EnlistedOrganization { get; set; }
        public string OrganizationName { get; set; }

        [ForeignKey("OrganizationId")]
        public virtual Organization Organization { get; set; }

        public long? OfficeAddressId { get; set; }
        [ForeignKey("OfficeAddressId")]
        public virtual Address OfficeAddress { get; set; }

        public int? MonthsInCurrentJob { get; set; }
        public string NameOfPreviousEmployeer { get; set; }
        public int? YearsOfExpInPrevEmp { get; set; }
        public int? TotalYearOfServieExp { get; set; }
        public int? CompanyRetirementAge { get; set; }

        public CompanyLegalStatus? LegalStatus { get; set; }
        public int? NumberOfEmployees { get; set; }
        public decimal? EquityOrShare { get; set; }
        public string MainProduct { get; set; }
        public string MainClient { get; set; }

        public string PrimaryIncomeSource { get; set; }
        public string OtherIncomeSource { get; set; }

        public virtual ICollection<LandOwnerProperty> LandOwnerProperties { get; set; }

        public string DescriptionOfOccupation { get; set; }

        public bool? IsDirectorOfAnyBankOrFL { get; set; }
        public string BankOrFLName { get; set; }
        public RoleInBankOrFL? RoleInBankOrFL { get; set; }
        public bool? RelatedPartyWithIPDC { get; set; }
        public RelationshipWithIPDC? RelationshipWithIPDC { get; set; }

    }
}
