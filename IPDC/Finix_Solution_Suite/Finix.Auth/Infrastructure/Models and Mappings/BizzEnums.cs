using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finix.Auth.Infrastructure
{

    public enum RoleEnum { Admin = 3, SuperUser = -2 }
    public enum LCType { None = 0, Irecoverable = 1 }
    public enum BankAccountType { None = 0, Savings = 1, Current = 2, STD }
    public enum CompanyType { Company = 1, Branch = 2, Project = 3 }
    public enum CurrencyType { BDT = 1, USD = 2 }
    public enum PurchaseOrderStatus { Ordered = 1, PartialReceived, Received, PartialCancelled, Cancelled }
    public enum TranType { Receive = 1, Return = 2, Cancel = 3 }
    //public enum AccTranType { Receive = 1, Payment = 2, BankTran = 3, Adjustment = 4 }
    public enum StockTranType { Receive = 1, Return = 2, TranferIn = 3, Tranferout = 4 }
    public enum Gender { Male = 1, Female = 2 }
    public enum MaritalStatus { Married = 1, UnMarried = 2, Other }
    public enum BloodGroup { O_Positive=1, O_Negative, A_Positive, A_Negative, AB_Positive, AB_Negetive, B_Positive, B_Negative }
    public enum Religion { Islam = 1, Hindu = 2, Buddist, Christian, Other }
    public enum OfficeType { HO = 1, Regional, Area }
    public enum UnitType { Wing = 1, Dept, Section }
    public enum EmployeeType { Contractual, Permanent }
    public enum TrainingType { InHouse, External, Foreign }
    public enum DegreeLevel { PhD, Master, Batchelor, HSC, SSC, UnderSSC, MBA, Diploma, Other }
    public enum DegreeMajor { Science, Arts, Commerce, Humanities }
    public enum Workshift { Day, Night, Both }
    public enum UploadedFileType { EmployeePhoto, TrainingDocument }
    public enum LocationType { GeneralStructure, MarketStructure }
    /// <summary>
    ///  Outside BD: Division=State; District=City;
    /// </summary>
    public enum LocationLevel { Division = 1, District, Thana, Area }
    public enum LeaveApplicableTo { Male, Female, All }
    public enum LeaveApplicationStatus { Draft, Applied, ApprovedByDept, ApprovedByHr, Halted, Rejected ,Forward}

    public enum CurrentYear { True = 1, False = 0 }
   
    public enum HolidayType
    {
        Public = 1,
        Festival = 2
    }
    public enum WFTypes { LeaveApplication, LoanApplication, Appraisal, PaymentVoucher }
    public enum WFStatus { Pending, Executed, Postponed, Deadlocked }
    public enum WFActorType { Initiator, Middleman, Finisher }
    public enum LeaveSupervisionType { Approved = 1, Cancelled, Modified,Forward,Applied }

    public enum AppraisalType
    {
        Regular = 1,
        Special = 1
    }
    public enum TourType { OfficialTour, CompanyTour, Other }
    public enum TourStatus { Planned, Ongoing, Completed, Cancelled, Halted }

    public enum AppraisalStatus
    {
        NotStarted = 1,
        Started = 2,
        Postponed = 3,
        Finished = 4,
        Cancelled = 5
    }
    public enum Month { January=1,February,March,April,May,June,July,August,September,October,November,December}
    public enum SalaryItemType
    {
        Basic, Allowance, PF, OverTime, Loan, SalaryAdvance
    }
    public enum EmployeeHistoryEnum { transfer=1,promotion,resignation,join}
    public enum SalaryProcessStatus { Draft, Edited, Finalized }
    public enum SalaryItemStatus { Active,Inactive}
    public enum GradeStepSalaryStatus { Active,Inactive}
    public enum SalaryItemContributionType { Add,Deduct,Other}
    public enum EmployeeStatus { Active,Inactive}
}