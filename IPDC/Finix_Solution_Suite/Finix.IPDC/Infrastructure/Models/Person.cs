using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Finix.Auth.Infrastructure;

namespace Finix.IPDC.Infrastructure.Models
{
    [Table("Person")]
    public class Person : Entity
    {
        public Person()
        {
            Children = new List<Person>();
        }
        // basic info
        public long Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public Gender? Gender { get; set; }
        public MaritalStatus? MaritalStatus { get; set; }
        public Religion? Religion { get; set; }
        public BloodGroup? BloodGroup { get; set; }

        public long? FatherId { get; set; }
        [ForeignKey("FatherId")]
        public virtual Person Father { get; set; }

        public long? MotherId { get; set; }
        [ForeignKey("MotherId")]
        public virtual Person Mother { get; set; }

        public long? SpouseId { get; set; }
        [ForeignKey("SpouseId")]
        public virtual Person Spouse { get; set; }

        public long? ParentId { get; set; }
        [ForeignKey("ParentId")]
        public virtual ICollection<Person> Children { get; set; }

        
        public string Nationality { get; set; }

        public long? PermanentAddressId { get; set; }
        [ForeignKey("PermanentAddressId")]
        public virtual Address PermanentAddress { get; set; }

        public long? PresentAddressId { get; set; }
        [ForeignKey("PresentAddressId")]
        public virtual Address PresentAddress { get; set; }

        public string NID { get; set; }
        public string PassportNo { get; set; }
        public string DrivingLicense { get; set; }
        public string Photo { get; set; }

        public long? EmergencyContactId { get; set; }
        [ForeignKey("EmergencyContactId")]
        public virtual Person EmergencyContactPerson { get; set; }
        public string EmergencyContactPersonPhone { get; set; }
        public string EmergencyContactPersonRelation { get; set; }
        public string Email { get; set; }
        public string PhoneNo { get; set; }

        //not mapped.
        [NotMapped]
        public string Name
        {
            get { return string.Format("{0} {1}", this.FirstName, this.LastName); }
        }
        [NotMapped]
        public string FatherName
        {
            get
            {
                try
                {
                    return string.Format("{0} {1}", this.Father.FirstName, this.Father.LastName);
                }
                catch
                {
                    return "";
                }
            }
        }
        [NotMapped]
        public string MotherName
        {
            get
            {
                try
                {
                    return string.Format("{0} {1}", this.Mother.FirstName, this.Mother.LastName);
                }
                catch
                {
                    return "";
                }
            }
        }

    }
}
