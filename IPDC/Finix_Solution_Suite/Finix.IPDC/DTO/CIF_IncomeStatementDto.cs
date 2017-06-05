using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Finix.IPDC.Infrastructure;
using Finix.IPDC.Infrastructure.Models;

namespace Finix.IPDC.DTO
{

    public class CIF_IncomeStatementDto
    {
        public long CIF_PersonalId { get; set; }
        //income fields
        public long Id { get; set; }
        public long? ApplicationId { get; set; }
        public decimal? MonthlySalaryDeclared { get; set; }
        public decimal? MonthlyInterestIncomeDeclared { get; set; }
        public decimal? MonthlyRentalIncomeDeclared { get; set; }
        public decimal? MonthlyBusinessIncomeDeclared { get; set; }
        public decimal? MonthlyIncomeTotalDeclared { get; set; }
        public decimal? MonthlyExpenseFoodAndClothingDeclared { get; set; }
        public decimal? MonthlyExpenseTransportationDeclared { get; set; }
        public decimal? MonthlyExpenseEducationDeclared { get; set; }
        public decimal? MonthlyExpenseEducationAssessed { get; set; }
        public decimal? MonthlyExpenseRentAndUtilityDeclared { get; set; }
        public decimal? MonthlyExpenseInstallmentsDeclared { get; set; }
        public decimal? MonthlyExpenseOthersDeclared { get; set; }
        public decimal? MonthlyExpenseTotalDeclared { get; set; }
        public decimal? MonthlyPayableLoanInstallmentDeclared { get; set; }
        public List<CIF_AdditionalIncomeDeclaredDto> MonthlyOtherIncomesDeclared { get; set; }
        public List<long> RemovedAdditionalIncomeDeclared { get; set; }
        public DateTime? CreateDate { get; set; }
        public long? CreatedBy { get; set; }
        public DateTime? EditDate { get; set; }
        public long? EditedBy { get; set; }
        public EntityStatus Status { get; set; }
    }
}
