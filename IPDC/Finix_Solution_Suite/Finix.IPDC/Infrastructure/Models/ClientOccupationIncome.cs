using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finix.IPDC.Infrastructure.Models
{
    public class ClientOccupationIncome : Entity
    {
        public string NameOfCompany { get; set; }
        public long? DesignationId  { get; set; }
        public long? ThanaId { get; set; }
        public long? DistrictId { get; set; }
        public string OfficeAddress { get; set; }
        public string PostalCode { get; set; }
        public string Phone { get; set; }
        public int MonthsCurrentJob { get; set; }
        public long? PreviousEmployerId { get; set; }
        public long? YearOfExprWithPrevEmployer { get; set; }
        public long? TotalYearOfExprInCurrentOrganization { get; set; }
        public string OfficePhone { get; set; }
        public OccupationType? OccupationType { get; set; }
        public EmploymentStatus? EmploymentStatus { get; set; }
        public bool? HasDirectorship  { get; set; }
        public long? BankId { get; set; } //todo-Bank Model
        public decimal? PercentageOfShare { get; set; }
        public string MobileNo { get; set; }
        public string OfficeEmail { get; set; }
        public ContactAddress ContactAddress { get; set; }
        public int RetirementAge { get; set; }
        [ForeignKey("DesignationId")]
        public virtual Designation  Designation { get; set; }
        [ForeignKey("ThanaId")]
        public virtual Thana Thana { get; set; }
        [ForeignKey("DistrictId")]
        public virtual District District { get; set; }
        [ForeignKey("PreviousEmployerId")]
        public virtual Organization PreviousOrganization { get; set; }
    }
}
