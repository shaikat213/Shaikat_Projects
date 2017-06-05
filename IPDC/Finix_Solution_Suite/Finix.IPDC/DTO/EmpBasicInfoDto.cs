using Finix.IPDC.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finix.IPDC.DTO
{
    public class EmpBasicInfoDto
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        //public string Name { get; set; }
        public string FatherFirstName { get; set; }
        public string FatherLastName { get; set; }
        public string MotherFirstName { get; set; }
        public string MotherLastName { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string DateOfBirthTxt { get; set; }
        public int? Gender { get; set; }
        //public string GenderName { get; set; }
        public int? MaritalStatus { get; set; }
        //public string MaritalStatusName { get; set; }
        public int? Religion { get; set; }
        //public string ReligionName { get; set; }
        public int? BloodGroup { get; set; }
        //public string BloodGroupName { get; set; }
        public long? CountryId { get; set; }
        //public string CountryName { get; set; }
        public string NID { get; set; }
        public DateTime? JoiningDate { get; set; }
        public EmployeeType? EmployeeType { get; set; }
        public string EmployeeTypeName { get; set; }
        public int? EmployeeStatus { get; set; }
        public string EmployeeStatusName { get; set; }
        public long? TrainingId { get; set; }
        public string TrainingName { get; set; }
        public long? DegreeId { get; set; }
        public string DegreeName { get; set; }
        public string BloodGroupName { get; set; }
        public string DesignationName { get; set; }
        public string Nationality { get; set; }
        public string ContactNo { get; set; }
        public string Photo { get; set; }
        public string IMEINo { get; set; }
    }
}
