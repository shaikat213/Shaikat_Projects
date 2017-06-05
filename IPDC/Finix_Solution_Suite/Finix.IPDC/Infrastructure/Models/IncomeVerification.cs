using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finix.IPDC.Infrastructure.Models
{
    [Table("IncomeVerification")]
    public class IncomeVerification : Entity
    {
        public long ISId { get; set; }
        [ForeignKey("ISId"), InverseProperty("Verifications")]
        public virtual CIF_IncomeStatement IncomeStatement { get; set; }
        public long? ApplicationId { get; set; }
        [ForeignKey("ApplicationId")]
        public virtual Application Application { get; set; }
        public long? CifId { get; set; }
        [ForeignKey("CifId")]
        public virtual CIF_Personal CfPersonal { get; set; }
        public VerificationAs VerificationPersonRole { get; set; }
        public decimal? MonthlySalaryAssessed { get; set; }
        public bool MonthlySalaryAssessedConsidered { get; set; }
        public decimal? MonthlyInterestIncomeAssessed { get; set; }
        public bool MonthlyInterestIncomeAssessedConsidered { get; set; }
        public decimal? MonthlyRentalIncomeAssessed { get; set; }
        public bool MonthlyRentalIncomeAssessedConsidered { get; set; }
        public virtual ICollection<IncomeVerificationAdditionalIncomeAssessed> MonthlyOtherIncomesAssessed { get; set; }
        public decimal? MonthlyBusinessIncomeAssessed { get; set; }
        public bool MonthlyBusinessIncomeAssessedConsidered { get; set; }
        public decimal? MonthlyIncomeTotalAssessed { get; set; }
        public bool MonthlyIncomeTotalAssessedConsidered { get; set; }
        public decimal? MonthlyExpenseFoodAndClothingAssessed { get; set; }
        public decimal? MonthlyExpenseTransportationAssessed { get; set; }
        public decimal? MonthlyExpenseEducationAssessed { get; set; }
        public decimal? MonthlyExpenseRentAndUtilityAssessed { get; set; }
        public decimal? MonthlyExpenseInstallmentsAssessed { get; set; }
        public decimal? MonthlyExpenseOthersAssessed { get; set; }
        public decimal? MonthlyExpenseTotalAssessed { get; set; }
        public decimal? MonthlyPayableLoanInstallmentAssessed { get; set; }
        public DateTime? IncomeAssessmentDate { get; set; }
        public VerificationState VerificationState { get; set; }
        public long? EmpId { get; set; }
        [ForeignKey("EmpId")]
        public virtual Employee Employee { get; set; }
    }

    [Table("IncomeVerificationAdditionalIncomeAssessed")]
    public class IncomeVerificationAdditionalIncomeAssessed : Entity
    {
        public long IncomeVerificationId { get; set; }
        [ForeignKey("IncomeVerificationId"), InverseProperty("MonthlyOtherIncomesAssessed")]
        public virtual IncomeVerification IncomeVerification { get; set; }
        public long? AdditionalIncomeDeclaredId { get; set; }
        [ForeignKey("AdditionalIncomeDeclaredId")]
        public virtual CIF_AdditionalIncomeDeclared AdditionalIncomeDeclared { get; set; }
        public bool IsConsidered { get; set; }
        public string SourceOfIncome { get; set; }
        public decimal? IncomeAmount { get; set; }
    }
}
