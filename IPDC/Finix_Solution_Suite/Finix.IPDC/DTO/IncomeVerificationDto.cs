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
    public class IncomeVerificationDto
    {
        public long? Id { get; set; }
        public long ISId { get; set; }
        public  CIF_IncomeStatementDto IncomeStatement { get; set; }
        public long? ApplicationId { get; set; }
        public string ApplicationNo { get; set; }
        public long? CifId { get; set; }
        public string CifNo { get; set; }
        public string CifName { get; set; }
        public CIF_PersonalDto CifPersonal { get; set; }
        //public ApplicationDto Application { get; set; }
        public VerificationAs VerificationPersonRole { get; set; }
        public string VerificationPersonRoleName { get; set; }
        public decimal? MonthlySalaryAssessed { get; set; }
        public decimal? MonthlyInterestIncomeAssessed { get; set; }
        public decimal? MonthlyRentalIncomeAssessed { get; set; }
        public decimal? MonthlyOtherIncomeAssessed { get; set; }
        public decimal? MonthlyBusinessIncomeAssessed { get; set; }
        public decimal? MonthlyIncomeTotalAssessed { get; set; }
        public List<IncomeVerificationAdditionalIncomeAssessedDto> MonthlyOtherIncomesAssessed { get; set; }
        //expense fields
        public decimal? MonthlyExpenseFoodAndClothingAssessed { get; set; }
        public decimal? MonthlyExpenseTransportationAssessed { get; set; }
        public decimal? MonthlyExpenseEducationAssessed { get; set; }
        public decimal? MonthlyExpenseRentAndUtilityAssessed { get; set; }
        public decimal? MonthlyExpenseInstallmentsAssessed { get; set; }
        public decimal? MonthlyExpenseOthersAssessed { get; set; }
        public decimal? MonthlyExpenseTotalAssessed { get; set; }
        //loan installment
        public decimal? MonthlyPayableLoanInstallmentAssessed { get; set; }
        public DateTime? IncomeAssessmentDate { get; set; }
        public string IncomeAssessmentDateTxt { get; set; }
        public VerificationState VerificationState { get; set; }
        public string VerificationStateName { get; set; }
        public DateTime? CreateDate { get; set; }
        public long? CreatedBy { get; set; }
        public DateTime? EditDate { get; set; }
        public long? EditedBy { get; set; }
        public EntityStatus Status { get; set; }
        public bool MonthlySalaryAssessedConsidered { get; set; }
        public bool MonthlyInterestIncomeAssessedConsidered { get; set; }
        public bool MonthlyRentalIncomeAssessedConsidered { get; set; }
        public bool MonthlyBusinessIncomeAssessedConsidered { get; set; }
        public bool MonthlyIncomeTotalAssessedConsidered { get; set; }
        public long? EmpId { get; set; }
    }
}
