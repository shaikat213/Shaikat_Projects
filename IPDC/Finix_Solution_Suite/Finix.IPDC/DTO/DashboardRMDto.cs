using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finix.IPDC.DTO
{
    public class DashboardRMDto
    {
        public DashboardRMDto()
        {
            this.Schedules = new List<ScheduleMessageDto>();
            this.MTDLoanAmount = 0;
            this.LMTDLoanAmount = 0;
            this.MTDDepositAmount = 0;
            this.LMTDDepositAmount = 0;
        }
        public long? CallsInProgress { get; set; }
        public decimal? MTDLoanAmount { get; set; }
        public decimal? LMTDLoanAmount { get; set; }
        public decimal? MTDDepositAmount { get; set; }
        public decimal? LMTDDepositAmount { get; set; }
        //loan summary
        public long? ActiveLeadsLoan { get; set; }
        public long? UnsubmittedApplicationsLoan { get; set; }
        public long? SubmittedToCRMApplications { get; set; }
        public long? CRMApprovedApplications { get; set; }
        public long? ReadyForDisbursementApplications { get; set; }
        //deposit summary
        public long? ActiveLeadsDeposit { get; set; }
        public long? SubmittedApplicationsDeposit { get; set; }
        //MTD loan
        public long? TotalLeadsLoanMTD { get; set; }
        public long? TotalSubmittedApplicationsLoanMTD { get; set; }
        public long? DisbursementCountMTD { get; set; }//application wise
        public long? TotalRejectedApplicationsMTD { get; set; }
        //LMTD loan
        public long? TotalLeadsLoanLMTD { get; set; }
        public long? TotalSubmittedApplicationsLoanLMTD { get; set; }
        public long? DisbursementCountLMTD { get; set; }//application wise
        public long? TotalRejectedApplicationsLMTD { get; set; }
        //MTD Deposit
        public long? TotalLeadsDepositMTD { get; set; }
        public long? TotalAccountsOpenedMTD { get; set; }
        //LMTD Deposit
        public long? TotalLeadsDepositLMTD { get; set; }
        public long? TotalAccountsOpenedLMTD { get; set; }
        public List<ScheduleMessageDto> Schedules { get; set; }

    }

    public class ScheduleMessageDto
    {
        public long? Id { get; set; }
        public string HeaderText { get; set; }
        public string PhoneNo { get; set; }
        public DateTime? Schedule { get; set; }
        public string Description { get; set; }
        public string Url { get; set; }
    }
}
