using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finix.IPDC.Infrastructure.Models
{
    [Table("CIF_IncomeStatement")]
    public class CIF_IncomeStatement : Entity
    {
        public long CIF_PersonalId { get; set; }
        public decimal? MonthlySalaryDeclared { get; set; }
        public decimal? MonthlyInterestIncomeDeclared { get; set; }
        public decimal? MonthlyRentalIncomeDeclared { get; set; }
        public virtual ICollection<CIF_AdditionalIncomeDeclared> MonthlyOtherIncomesDeclared { get; set; }
        public decimal? MonthlyBusinessIncomeDeclared { get; set; }
        public decimal? MonthlyIncomeTotalDeclared { get; set; }
        //expense fields
        public decimal? MonthlyExpenseFoodAndClothingDeclared { get; set; }
        public decimal? MonthlyExpenseTransportationDeclared { get; set; }
        public decimal? MonthlyExpenseEducationDeclared { get; set; }
        public decimal? MonthlyExpenseRentAndUtilityDeclared { get; set; }
        public decimal? MonthlyExpenseInstallmentsDeclared { get; set; }
        public decimal? MonthlyExpenseOthersDeclared { get; set; }
        public decimal? MonthlyExpenseTotalDeclared { get; set; }
        //loan installment
        public decimal? MonthlyPayableLoanInstallmentDeclared { get; set; }
        [ForeignKey("CIF_PersonalId"), InverseProperty("IncomeStatements")]
        public virtual CIF_Personal CIF_Personal { get; set; }
        public virtual ICollection<IncomeVerification> Verifications { get; set; }
    }

    [Table("CIF_AdditionalIncomeDeclared")]
    public class CIF_AdditionalIncomeDeclared : Entity
    {
        public long CIF_IncomeStatementId { get; set; }
        [ForeignKey("CIF_IncomeStatementId"), InverseProperty("MonthlyOtherIncomesDeclared")]
        public virtual CIF_IncomeStatement IncomeStatement { get; set; }
        public string SourceOfIncome { get; set; }
        public decimal? IncomeAmount { get; set; }
    }
}
