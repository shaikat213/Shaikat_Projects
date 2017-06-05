using Finix.IPDC.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finix.IPDC.DTO
{
    public class EmployeeDto
    {
        public EmployeeDto()
        {
            BasicInfo = new EmpBasicInfoDto();
            ContactInformation = new ContactInformationDto();
            //Education = new DegreeInfoDto();
            //TrainingInformation = new TrainingInformationDto();
            //JoiningInformation = new JoiningInformationDto();
            UserInformation = new UserInformationDto();
        }
        public long? Id { get; set; }
        public string Name { get; set; }
        public string EmpCode { get; set; }
        public string RmCode { get; set; }
        public string IMEINo { get; set; }
        public DateTime? JoiningDate { get; set; }
        public string JoiningDateText { get; set; }
        public EmployeeType? EmployeeType { get; set; }
        public string EmployeeTypeName { get; set; }
        public string Photo { get; set; }
        public long? CompanyProfileId { get; set; }
        public EmpBasicInfoDto BasicInfo { get; set; }
        public ContactInformationDto ContactInformation { get; set; }
        //public DegreeInfoDto Education { get; set; }
        //public TrainingInformationDto TrainingInformation { get; set; }
        public UserInformationDto UserInformation { get; set; }
        //public JoiningInformationDto JoiningInformation { get; set; }

    }

    public class ContactInformationDto
    {
        public long? Id { get; set; }
        public AddressDto PresentAddress { get; set; }
        public AddressDto ParmanentAddress { get; set; }
        public string PhoneNo { get; set; }
        public string Email { get; set; }
        public string EmergencyContactPerson { get; set; }
        public string EmergencyContactPhone { get; set; }
        public string EmergencyContactRelation { get; set; }
        public long? EmployeeId { get; set; }

    }

    public class UserInformationDto
    {
        public long? Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public bool? IsEmployee { get; set; }
        public long? EmployeeId { get; set; }

    }
}
