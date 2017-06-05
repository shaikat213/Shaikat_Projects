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
    public class OccupationDto
    {
        public long? Id { get; set; }
        public long CIF_PersonalId { get; set; }
        public OccupationType? OccupationType { get; set; }
        public EmploymentStatus? EmploymentStatus { get; set; }
        public long? ProfessionId { get; set; }
        public string ProfessionName { get; set; }
        public string Designation { get; set; }
        public long? OrganizationId { get; set; }
        public bool? EnlistedOrganization { get; set; }
        public string OrganizationName { get; set; }
        public long? OfficeAddressId { get; set; }
        public string OfficeAddressName { get; set; }
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
        public List<LandOwnerPropertyDto> LandOwnerProperties { get; set; }
        public string DescriptionOfOccupation { get; set; }
        public bool? IsDirectorOfAnyBankOrFL { get; set; }
        public string BankOrFLName { get; set; }
        public RoleInBankOrFL? RoleInBankOrFL { get; set; }
        public string RoleInBankName { get; set; }
        public bool? RelatedPartyWithIPDC { get; set; }
        public RelationshipWithIPDC? RelationshipWithIPDC { get; set; }
        public string RelationshipWithIPDCName { get; set; }
        public AddressDto OfficeAddress { get; set; }
        public List<long> RemovedLandOwnerProp { get; set; }
        public DateTime? CreateDate { get; set; }
        public long? CreatedBy { get; set; }
        public DateTime? EditDate { get; set; }
        public long? EditedBy { get; set; }
        public EntityStatus? Status { get; set; }
    }
}
