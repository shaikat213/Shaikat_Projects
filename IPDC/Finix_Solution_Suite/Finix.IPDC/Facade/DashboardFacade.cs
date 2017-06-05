using AutoMapper;
using Finix.Auth.Facade;
using Finix.IPDC.DTO;
using Finix.IPDC.Infrastructure;
using Finix.IPDC.Infrastructure.Models;
using Finix.IPDC.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Finix.Auth.DTO;
using Finix.Auth.Infrastructure;
using Finix.Auth.Util;
using Microsoft.Practices.ObjectBuilder2;
using Application = Finix.IPDC.Infrastructure.Models.Application;
using EntityStatus = Finix.IPDC.Infrastructure.EntityStatus;
using UiUtil = Finix.IPDC.Util.UiUtil;


namespace Finix.IPDC.Facade
{
    public class DashboardFacade : BaseFacade
    {
        private readonly UserFacade _user = new UserFacade();
        private readonly EmployeeFacade _employee = new EmployeeFacade();
        private readonly SalesLeadFacade _salesLead = new SalesLeadFacade();
        private readonly OfficeDesignationSettingFacade _office = new OfficeDesignationSettingFacade();

        public DashboardRMDto GetRMDashboardData(long UserId)
        {
            var data = new DashboardRMDto();

            #region data population
            var today = new DateRangeDto(TimeLine.Today);
            var MTD = new DateRangeDto(TimeLine.MTD);
            var LMTD = new DateRangeDto(TimeLine.LMTD);

            long employeeId = _user.GetEmployeeIdByUserId(UserId);
            long offDegSettingId = _employee.GetOfficeDesignationIdOfEmployee(employeeId);
            long assigned = 0;
            if (offDegSettingId > 0)
            {
                assigned = offDegSettingId;
            }

            var leadIds = GenService.GetAll<SalesLeadAssignment>()
                            .Where(s => s.AssignedToEmpId == employeeId && s.Status == EntityStatus.Active).Select(s => s.SalesLeadId).ToList();
            var allLeads = GenService.GetAll<SalesLead>().Where(s => leadIds.Contains(s.Id) && s.Status == EntityStatus.Active);
            var activeLeads = allLeads.Where(s => s.LeadStatus == LeadStatus.Drafted);

            var unsubmittedApplicaionsQuery = GenService.GetAll<SalesLead>().Where(s => leadIds.Contains(s.Id) && s.Status == EntityStatus.Active && s.LeadStatus == LeadStatus.Prospective).ToList();
            var notAcceptableLeadIds =
                GenService.GetAll<Application>()
                    .Where(s => s.ApplicationStage != ApplicationStage.Drafted)
                    .Select(s => s.SalesLeadId)
                    .ToList();
            var unSubmittedApplications = unsubmittedApplicaionsQuery.Where(q => !notAcceptableLeadIds.Contains(q.Id)).ToList();
            var submittedApplications = GenService.GetAll<Application>().Where(a => a.RMId == employeeId && a.ApplicationStage != ApplicationStage.Drafted);
            var submittedApplicationIds = submittedApplications.Select(s => s.Id).ToList();
            var readyForDisbursement = GenService.GetAll<DisbursementMemo>().Where(d => d.IsApproved != null &&
                                                                                        d.IsApproved == true &&
                                                                                        d.IsDisbursed == null &&
                                                                                        d.Status == EntityStatus.Active &&
                                                                                        d.ApplicationId != null &&
                                                                                        submittedApplicationIds.Contains((long)d.ApplicationId))
                                                                            .Select(d => d.ApplicationId).Distinct().Count();

            //var successfulLeads = GenService.GetAll<SalesLead>().Where(s=>)
            var applicationLog = GenService.GetAll<ApplicationLog>().Where(l => submittedApplicationIds.Contains(l.ApplicationId));
            var submittedAppLog = applicationLog.Where(l => l.FromStage == ApplicationStage.Drafted && l.ToStage == ApplicationStage.SentToTL);

            var rejectedAppLog = applicationLog.Where(l => l.ToStage < ApplicationStage.ApplicationDisposed);
            var accountOpenedAppLog = applicationLog.Where(l => l.ToStage == ApplicationStage.AccountOpened);
            var appLogGrouped = (from log in applicationLog.OrderBy(a => a.Id)
                                 where (log.ToStage == ApplicationStage.PartialDisbursementComplete || log.ToStage == ApplicationStage.DisbursementComplete)
                                 group log by log.ApplicationId into logGroup
                                 from appLog in logGroup.DefaultIfEmpty()
                                 select new
                                 {
                                     ApplicationId = logGroup.FirstOrDefault().ApplicationId,
                                     CreateDate = logGroup.FirstOrDefault().CreateDate
                                 }).Distinct();
            var disbursementMemos = GenService.GetAll<DisbursementMemo>().Where(d => d.ApplicationId != null &&
                                                                                     submittedApplicationIds.Contains((long)d.ApplicationId) &&
                                                                                     d.Status == EntityStatus.Active &&
                                                                                     d.IsApproved != null &&
                                                                                     d.IsApproved == true &&
                                                                                     d.IsDisbursed != null &&
                                                                                     d.IsDisbursed == true);
            var fundReceived = GenService.GetAll<FundConfirmationDetail>().Where(f => submittedApplicationIds.Contains((long)f.FundConfirmation.ApplicationId) &&
                                                                                      f.FundConfirmation.Status == EntityStatus.Active &&
                                                                                      f.Status == EntityStatus.Active &&
                                                                                      f.FundConfirmation.FundReceived != null &&
                                                                                      f.FundConfirmation.FundReceived == true &&
                                                                                      f.CreditDate != null);
            var disbursementMemosForMtd = disbursementMemos.Where(d => d.DisbursementDetails.FirstOrDefault().CreateDate <= MTD.ToDate && d.DisbursementDetails.FirstOrDefault().CreateDate >= MTD.FromDate);
            var disbursementMemosForLmtd = disbursementMemos.Where(d => d.DisbursementDetails.FirstOrDefault().CreateDate <= LMTD.ToDate && d.DisbursementDetails.FirstOrDefault().CreateDate >= LMTD.FromDate);
            var fundsReceivedForMtd = fundReceived.Where(f => f.CreditDate <= MTD.ToDate && f.CreditDate >= MTD.FromDate);
            var fundsReceivedForLmtd = fundReceived.Where(f => f.CreditDate <= LMTD.ToDate && f.CreditDate >= LMTD.FromDate);

            var callsInProgress = GenService.GetAll<Call>().Where(s => s.Status == EntityStatus.Active &&
                                                                        s.CallStatus == CallStatus.Unfinished &&
                                                                        ((s.CallType == CallType.Self && s.CreatedBy == UserId) ||
                                                                         (s.CallType != CallType.Self && s.AssignedTo == assigned)));
            var todaysScheduledLeads = activeLeads.Where(l => l.FollowupTime < today.ToDate).ToList();
            #endregion

            data.CallsInProgress = callsInProgress.Count();
            if (disbursementMemosForMtd.Count() > 0)
            {
                data.MTDLoanAmount = (from memo in disbursementMemosForMtd
                                          //group memo by memo.ApplicationId into memoGroup
                                      select memo//memoGroup.Sum(m=>m.CurrentDisbursementAmount)
                                      ).Sum(m => m.CurrentDisbursementAmount);
            }
            if (disbursementMemosForLmtd.Count() > 0)
            {
                data.LMTDLoanAmount = (from memo in disbursementMemosForLmtd
                                       select memo).Sum(m => m.CurrentDisbursementAmount);
            }
            if (fundsReceivedForMtd.Count() > 0)
            {
                data.MTDDepositAmount = (from funds in fundsReceivedForMtd
                                         select funds).Sum(f => f.Amount);
            }
            if (fundsReceivedForLmtd.Count() > 0)
            {
                data.LMTDDepositAmount = (from funds in fundsReceivedForLmtd
                                          select funds).Sum(f => f.Amount);
            }


            ////loan summary
            data.ActiveLeadsLoan = activeLeads.Where(a => a.ProductId != null && a.Product.ProductType == ProductType.Loan).Count();
            data.UnsubmittedApplicationsLoan = unSubmittedApplications.Where(u => u.ProductId != null && u.Product.ProductType == ProductType.Loan).Count();
            data.SubmittedToCRMApplications = submittedApplications.Where(s => s.ApplicationStage == ApplicationStage.SentToCRM || s.ApplicationStage == ApplicationStage.UnderProcessAtCRM).Count();
            data.CRMApprovedApplications = submittedApplications.Where(s => s.ApplicationStage == ApplicationStage.SentToOperations || s.ApplicationStage == ApplicationStage.UnderProcessAtOperations).Count();
            data.ReadyForDisbursementApplications = readyForDisbursement;

            ////deposit summary
            data.ActiveLeadsDeposit = activeLeads.Where(a => a.ProductId != null && a.Product.ProductType == ProductType.Deposit).Count();
            data.SubmittedApplicationsDeposit = submittedApplications.Where(s => s.Product.ProductType == ProductType.Deposit).Count();

            ////MTD loan
            data.TotalLeadsLoanMTD = allLeads.Where(l => l.ProductId != null && l.Product.ProductType == ProductType.Loan && l.CreateDate != null && l.CreateDate <= MTD.ToDate && l.CreateDate >= MTD.FromDate).Count();
            data.TotalSubmittedApplicationsLoanMTD = submittedAppLog.Where(l => l.Application.Product.ProductType == ProductType.Loan && l.CreateDate <= MTD.ToDate && l.CreateDate >= MTD.FromDate).Select(l => new { l.ApplicationId }).Distinct().Count();

            data.DisbursementCountMTD = appLogGrouped.Where(l => l.CreateDate <= MTD.ToDate && l.CreateDate >= MTD.FromDate).Count();
            //application wise
            data.TotalRejectedApplicationsMTD = rejectedAppLog.Where(l => l.Application.Product.ProductType == ProductType.Loan && l.CreateDate <= MTD.ToDate && l.CreateDate >= MTD.FromDate).Select(l => new { l.ApplicationId }).Distinct().Count();

            ////LMTD loan
            data.TotalLeadsLoanLMTD = allLeads.Where(l => l.ProductId != null && l.Product.ProductType == ProductType.Loan && l.CreateDate != null && l.CreateDate <= LMTD.ToDate && l.CreateDate >= LMTD.FromDate).Count();
            data.TotalSubmittedApplicationsLoanLMTD = submittedAppLog.Where(l => l.Application.Product.ProductType == ProductType.Loan && l.CreateDate <= LMTD.ToDate && l.CreateDate >= LMTD.FromDate).Select(l => new { l.ApplicationId }).Distinct().Count();
            data.DisbursementCountLMTD = appLogGrouped.Where(l => l.CreateDate <= LMTD.ToDate && l.CreateDate >= LMTD.FromDate).Count();//application wise
            data.TotalRejectedApplicationsLMTD = rejectedAppLog.Where(l => l.Application.Product.ProductType == ProductType.Loan && l.CreateDate <= LMTD.ToDate && l.CreateDate >= LMTD.FromDate).Select(l => new { l.ApplicationId }).Distinct().Count();

            ////MTD Deposit
            data.TotalLeadsDepositMTD = allLeads.Where(l => l.ProductId != null && l.Product.ProductType == ProductType.Deposit && l.CreateDate != null && l.CreateDate <= MTD.ToDate && l.CreateDate >= MTD.FromDate).Count();
            data.TotalAccountsOpenedMTD = accountOpenedAppLog.Where(l => l.Application.Product.ProductType == ProductType.Deposit && l.CreateDate <= MTD.ToDate && l.CreateDate >= MTD.FromDate).Select(l => new { l.ApplicationId }).Distinct().Count();

            ////LMTD Deposit
            data.TotalLeadsDepositLMTD = allLeads.Where(l => l.ProductId != null && l.Product.ProductType == ProductType.Deposit && l.CreateDate != null && l.CreateDate <= LMTD.ToDate && l.CreateDate >= LMTD.FromDate).Count();
            data.TotalAccountsOpenedLMTD = accountOpenedAppLog.Where(l => l.Application.Product.ProductType == ProductType.Deposit && l.CreateDate <= LMTD.ToDate && l.CreateDate >= LMTD.FromDate).Select(l => new { l.ApplicationId }).Distinct().Count();

            if (todaysScheduledLeads != null && todaysScheduledLeads.Count() > 0)
            {
                data.Schedules.AddRange(todaysScheduledLeads.Select(l => new ScheduleMessageDto
                {
                    Id = l.Id,
                    HeaderText = "Call " + l.CustomerName,
                    Description = "regarding lead followup",
                    Schedule = l.FollowupTime,
                    PhoneNo = l.CustomerPhone,
                    Url = "/IPDC/SalesLead/SalesLeadEntry"
                }));
            }
            //GetBMDashboardHighlights(52, TimeLine.MTD);
            return data;
        }

        public object GetBMDashboardHighlights(long UserId, TimeLine? timeLine)
        {
            DateRangeDto dateRange;
            if (timeLine == null)
                dateRange = new DateRangeDto();
            else
                dateRange = new DateRangeDto((TimeLine)timeLine);

            #region data population
            long employeeId = _user.GetEmployeeIdByUserId(UserId);
            long offDegSettingId = _employee.GetOfficeDesignationIdOfEmployee(employeeId);
            var subordinateEmpList = _employee.GetEmployeeWiseBM(employeeId);
            var subordinateEmpIdList = subordinateEmpList.Where(s => s.EmployeeId != null).Select(s => s.EmployeeId).ToList();
            var subordinateUserList = new List<long>();
            foreach (var sub in subordinateEmpList.Where(s => s.EmployeeId != null))
            {
                subordinateUserList.Add((long)_user.GetUserByEmployeeId((long)sub.EmployeeId).Id);
            }

            List<long> existingthanaIds = new List<long>();
            var thanaOfEmployee = _employee.GetThanaOfEmployee(employeeId);
            if (thanaOfEmployee.Any())
            {
                existingthanaIds = thanaOfEmployee.Select(d => d.Id).ToList();
            }
            var allApplications = GenService.GetAll<Application>().Where(a => subordinateEmpIdList.Contains(a.RMId));

            #region call data population
            IQueryable<Call> allCalls = GenService.GetAll<Call>().Where(c => c.CreateDate <= dateRange.ToDate && c.CreateDate >= dateRange.FromDate && c.Status == EntityStatus.Active);
            var ct = allCalls.Count();
            IQueryable<Call> callQueryableAreaWise = allCalls
                .Where(s => s.Status == EntityStatus.Active &&
                            s.CallStatus == CallStatus.Unfinished &&
                            s.CallType == CallType.Auto_assign &&
                            existingthanaIds.Contains((long)s.CustomerAddress.ThanaId));
            IQueryable<Call> callQueryableAsReferred = allCalls
                .Where(c => c.Status == EntityStatus.Active &&
                            c.CallStatus == CallStatus.Unfinished &&
                            c.ReferredTo == offDegSettingId);
            IQueryable<Call> rmSelfGeneratedCalls = allCalls
                .Where(c => c.Status == EntityStatus.Active &&
                            c.CallType == CallType.Self &&
                            subordinateUserList.Contains((long)c.CreatedBy));
            var allCallsUnited = callQueryableAreaWise.Union(callQueryableAsReferred).Union(rmSelfGeneratedCalls).Distinct();
            #endregion

            #region lead data population
            var allLeads = GenService.GetAll<SalesLeadAssignment>()
                .Where(s => s.Status == EntityStatus.Active
                            && s.SalesLead.CreateDate <= dateRange.ToDate
                            && s.SalesLead.CreateDate >= dateRange.FromDate
                            && s.SalesLead.Status == EntityStatus.Active &&
                            subordinateEmpIdList.Contains(s.AssignedToEmpId));
            var notAcceptableLeadIds = GenService.GetAll<ApplicationLog>()
                .Where(s => s.ToStage < ApplicationStage.Drafted && s.ToStage > ApplicationStage.SentToBM)
                .Select(s => s.ApplicationId);
            var assignedLeads = allLeads
                .Where(s => !notAcceptableLeadIds.Contains(s.Id) &&
                s.Status == EntityStatus.Active &&
                (s.SalesLead.LeadStatus == LeadStatus.Drafted || s.SalesLead.LeadStatus == LeadStatus.Prospective))
                .Select(s => s.SalesLead).OrderBy(s => s.Id).Distinct();
            #endregion

            #region application data population
            var submittedApplications = allApplications
                .Where(a => (((a.ApplicationStage == ApplicationStage.SentToCRM || a.ApplicationStage == ApplicationStage.UnderProcessAtCRM) && a.Product.ProductType == ProductType.Loan) ||
                            ((a.ApplicationStage == ApplicationStage.SentToOperations || a.ApplicationStage == ApplicationStage.UnderProcessAtOperations) && a.Product.ProductType == ProductType.Deposit)));
            var submittedApplicationIds = submittedApplications.Select(s => s.Id).ToList();
            var acceptableLogs = GenService.GetAll<ApplicationLog>()
                .Where(l => l.Status == EntityStatus.Active &&
                submittedApplicationIds.Contains(l.ApplicationId) &&
                l.CreateDate <= dateRange.ToDate &&
                l.CreateDate >= dateRange.FromDate &&
                (((l.ToStage == ApplicationStage.SentToCRM || l.ToStage == ApplicationStage.UnderProcessAtCRM) &&
                 l.Application.Product.ProductType == ProductType.Loan) ||
                ((l.ToStage == ApplicationStage.SentToOperations || l.ToStage == ApplicationStage.UnderProcessAtOperations) &&
                 l.Application.Product.ProductType == ProductType.Deposit)));
            var submittedApplicationsWithLog = (from appLog in acceptableLogs
                                                orderby appLog.Id
                                                select new
                                                {
                                                    ApplicationId = appLog.ApplicationId,
                                                    Product = appLog.Application.Product
                                                }).Distinct();
            #endregion

            #region approved/disapproved
            //var loanApplications = allApplications.Where(a => a.Product.ProductType == ProductType.Loan).ToList();
            var loanApplicationsIds = allApplications.Where(a => a.Product.ProductType == ProductType.Loan).Select(a => a.Id);
            var approvedLoanApplicationLogs = GenService.GetAll<ApplicationLog>()
                .Where(l => l.Status == EntityStatus.Active &&
                            l.ToStage == ApplicationStage.SentToOperations &&
                            loanApplicationsIds.Contains(l.ApplicationId) &&
                            l.CreateDate <= dateRange.ToDate &&
                            l.CreateDate >= dateRange.FromDate)
                .OrderBy(l => l.Id)
                .Select(l => new { ApplicationId = l.ApplicationId, Product = l.Application.Product, Amount = l.Application.LoanApplication.LoanAmountApplied })
                .Distinct();
            var approvedLoanApplicationLogsWithAmount = (from logs in approvedLoanApplicationLogs
                                                         join proposal in GenService.GetAll<Proposal>().Where(p => p.Status == EntityStatus.Active) on logs.ApplicationId equals proposal.ApplicationId into proposals
                                                         from p in proposals.DefaultIfEmpty()
                                                         select new
                                                         {
                                                             ApplicationId = logs.ApplicationId,
                                                             Product = logs.Product,
                                                             Amount = p != null && p.RecomendedLoanAmountFromIPDC != null ? p.RecomendedLoanAmountFromIPDC : logs.Amount
                                                         }).Distinct();

            var disapporvedLoanApplicationLogs = GenService.GetAll<ApplicationLog>()
                .Where(l => l.Status == EntityStatus.Active &&
                            l.ToStage < ApplicationStage.Drafted &&
                            loanApplicationsIds.Contains(l.ApplicationId) &&
                            l.CreateDate <= dateRange.ToDate &&
                            l.CreateDate >= dateRange.FromDate)
                .OrderBy(l => l.Id)
                .Select(l => new { ApplicationId = l.ApplicationId, Product = l.Application.Product, Amount = l.Application.LoanApplication.LoanAmountApplied })
                .Distinct();
            var disapprovedLoanApplicationLogsWithAmount = (from logs in disapporvedLoanApplicationLogs
                                                            join proposal in GenService.GetAll<Proposal>().Where(p => p.Status == EntityStatus.Active) on logs.ApplicationId equals proposal.ApplicationId into proposals
                                                            from p in proposals.DefaultIfEmpty()
                                                            select new
                                                            {
                                                                ApplicationId = logs.ApplicationId,
                                                                Product = logs.Product,
                                                                Amount = p != null && p.RecomendedLoanAmountFromIPDC != null ? p.RecomendedLoanAmountFromIPDC : logs.Amount
                                                            }).Distinct();
            #endregion

            #region Amount disbursed / received
            var disbursementMemos = GenService.GetAll<DisbursementMemo>().Where(d => d.ApplicationId != null &&
                                                                                     submittedApplicationIds.Contains((long)d.ApplicationId) &&
                                                                                     d.Status == EntityStatus.Active &&
                                                                                     d.IsApproved != null &&
                                                                                     d.IsApproved == true &&
                                                                                     d.IsDisbursed != null &&
                                                                                     d.IsDisbursed == true &&
                                                                                     d.DisbursementDetails.FirstOrDefault().CreateDate <= dateRange.ToDate &&
                                                                                     d.DisbursementDetails.FirstOrDefault().CreateDate >= dateRange.FromDate);
            var fundReceived = GenService.GetAll<FundConfirmationDetail>().Where(f => submittedApplicationIds.Contains((long)f.FundConfirmation.ApplicationId) &&
                                                                                      f.FundConfirmation.Status == EntityStatus.Active &&
                                                                                      f.Status == EntityStatus.Active &&
                                                                                      f.FundConfirmation.FundReceived != null &&
                                                                                      f.FundConfirmation.FundReceived == true &&
                                                                                      f.CreditDate != null &&
                                                                                      f.CreditDate <= dateRange.ToDate &&
                                                                                      f.CreditDate >= dateRange.FromDate);
            var TopListLoan = (from disbursement in disbursementMemos.Select(d => new
            {
                ApplicationId = d.ApplicationId,
                Amount = d.CurrentDisbursementAmount,
                RM = d.Application.RMEmp.Person.FirstName + " " + d.Application.RMEmp.Person.LastName,
                FacilityType = d.Proposal.FacilityType
            })
                               group disbursement by new { disbursement.RM, disbursement.FacilityType } into grouped
                               select new
                               {
                                   RM = grouped.Key.RM,
                                   FacilityType = grouped.Key.FacilityType,
                                   Amount = grouped.Sum(s => s.Amount),
                                   Applications = grouped.Count()
                               }).OrderBy(o => o.FacilityType).ThenBy(o => o.Amount);
            var TopListDeposit = (from funds in fundReceived.Select(d => new
            {
                Amount = d.Amount,
                ApplicationId = d.FundConfirmation.ApplicationId,
                RM = d.FundConfirmation.Application.RMEmp.Person.FirstName + " " + d.FundConfirmation.Application.RMEmp.Person.LastName
            })
                                  group funds by funds.RM into grouped
                                  select new
                                  {
                                      Amount = grouped.Sum(s => s.Amount) ?? 0,
                                      RM = grouped.Key,
                                      Applications = grouped.Count()
                                  }).OrderBy(d => d.Amount).ThenBy(d => d.Applications);
            //var Disbursement_TopRMAmount = 0;
            #endregion

            #endregion

            var data = new DashBoardHighlightsDto();
            #region initializations
            data.Disbursed_HomeLoanAmount = 0;
            data.Disbursed_PersonalLoanAmount = 0;
            data.Disbursed_AutoLoanAmount = 0;
            data.RMHomeLoan1 = "";
            data.RMHomeLoan1Amount = 0;
            data.RMHomeLoan1Count = 0;
            data.RMHomeLoan2 = "";
            data.RMHomeLoan2Amount = 0;
            data.RMHomeLoan2Count = 0;
            data.RMPersonalLoan = "";
            data.RMPersonalLoanAmount = 0;
            data.RMPersonalLoanCount = 0;
            data.RMAutoLoan = "";
            data.RMAutoLoanAmount = 0;
            data.RMAutoLoanCount = 0;
            data.RMLiability1 = "";
            data.RMLiability1Amount = 0;
            data.RMLiability1Count = 0;
            data.RMLiability2 = "";
            data.RMLiability2Amount = 0;
            data.RMLiability2Count = 0;
            #endregion
            data.Call_HomeLoan = allCallsUnited.Where(c => c.ProductId != null && c.Product.FacilityType == ProposalFacilityType.Home_Loan).Count();
            data.Call_PersonalLoan = allCallsUnited.Where(c => c.ProductId != null && c.Product.FacilityType == ProposalFacilityType.Personal_Loan).Count();
            data.Call_AutoLoan = allCallsUnited.Where(c => c.ProductId != null && c.Product.FacilityType == ProposalFacilityType.Auto_Loan).Count();
            data.Call_Liability = allCallsUnited.Where(c => c.ProductId != null && c.Product.ProductType == ProductType.Deposit).Count();
            data.Call_Undefined = allCallsUnited.Where(c => c.ProductId == null).Count();

            data.Lead_HomeLoan = assignedLeads.Where(l => l.ProductId != null && l.Product.FacilityType == ProposalFacilityType.Home_Loan).Count();
            data.Lead_PersonalLoan = assignedLeads.Where(l => l.ProductId != null && l.Product.FacilityType == ProposalFacilityType.Personal_Loan).Count();
            data.Lead_AutoLoan = assignedLeads.Where(l => l.ProductId != null && l.Product.FacilityType == ProposalFacilityType.Auto_Loan).Count();
            data.Lead_Liability = assignedLeads.Where(l => l.ProductId != null && l.Product.ProductType == ProductType.Deposit).Count();

            data.Application_HomeLoan = submittedApplicationsWithLog.Where(a => a.Product.FacilityType == ProposalFacilityType.Home_Loan).Count();
            data.Application_PersonalLoan = submittedApplicationsWithLog.Where(a => a.Product.FacilityType == ProposalFacilityType.Personal_Loan).Count();
            data.Application_AutoLoan = submittedApplicationsWithLog.Where(a => a.Product.FacilityType == ProposalFacilityType.Auto_Loan).Count();
            data.Application_Liability = submittedApplicationsWithLog.Where(a => a.Product.ProductType == ProductType.Deposit).Count();

            data.Approved_HomeLoan = approvedLoanApplicationLogsWithAmount.Where(a => a.Product.FacilityType == ProposalFacilityType.Home_Loan).Count();
            data.Approved_PersonalLoan = approvedLoanApplicationLogsWithAmount.Where(a => a.Product.FacilityType == ProposalFacilityType.Personal_Loan).Count();
            data.Approved_AutoLoan = approvedLoanApplicationLogsWithAmount.Where(a => a.Product.FacilityType == ProposalFacilityType.Auto_Loan).Count();
            data.Approved_HomeLoanAmount = (approvedLoanApplicationLogsWithAmount.Where(a => a.Product.FacilityType == ProposalFacilityType.Home_Loan).Sum(a => a.Amount != null ? a.Amount : 0) ?? 0);
            data.Approved_PersonalLoanAmount = (approvedLoanApplicationLogsWithAmount.Where(a => a.Product.FacilityType == ProposalFacilityType.Personal_Loan).Sum(a => a.Amount != null ? a.Amount : 0) ?? 0);
            data.Approved_AutoLoanAmount = (approvedLoanApplicationLogsWithAmount.Where(a => a.Product.FacilityType == ProposalFacilityType.Auto_Loan).Sum(a => a.Amount != null ? a.Amount : 0) ?? 0);
            data.Disapproved_HomeLoan = disapprovedLoanApplicationLogsWithAmount.Where(a => a.Product.FacilityType == ProposalFacilityType.Home_Loan).Count();
            data.Disapproved_PersonalLoan = disapprovedLoanApplicationLogsWithAmount.Where(a => a.Product.FacilityType == ProposalFacilityType.Personal_Loan).Count();
            data.Disapproved_AutoLoan = disapprovedLoanApplicationLogsWithAmount.Where(a => a.Product.FacilityType == ProposalFacilityType.Auto_Loan).Count();
            data.Disapproved_HomeLoanAmount = (disapprovedLoanApplicationLogsWithAmount.Where(a => a.Product.FacilityType == ProposalFacilityType.Home_Loan).Sum(a => a.Amount != null ? a.Amount : 0) ?? 0);
            data.Disapproved_PersonalLoanAmount = (disapprovedLoanApplicationLogsWithAmount.Where(a => a.Product.FacilityType == ProposalFacilityType.Personal_Loan).Sum(a => a.Amount != null ? a.Amount : 0) ?? 0);
            data.Disapproved_AutoLoanAmount = (disapprovedLoanApplicationLogsWithAmount.Where(a => a.Product.FacilityType == ProposalFacilityType.Auto_Loan).Sum(a => a.Amount != null ? a.Amount : 0) ?? 0);


            if (disbursementMemos.Where(d => d.Proposal.FacilityType == ProposalFacilityType.Home_Loan).Count() > 0)
                data.Disbursed_HomeLoanAmount = disbursementMemos.Where(d => d.Proposal.FacilityType == ProposalFacilityType.Home_Loan).Sum(d => d.CurrentDisbursementAmount);
            if (disbursementMemos.Where(d => d.Proposal.FacilityType == ProposalFacilityType.Personal_Loan).Count() > 0)
                data.Disbursed_PersonalLoanAmount = disbursementMemos.Where(d => d.Proposal.FacilityType == ProposalFacilityType.Personal_Loan).Sum(d => d.CurrentDisbursementAmount);
            if (disbursementMemos.Where(d => d.Proposal.FacilityType == ProposalFacilityType.Auto_Loan).Count() > 0)
                data.Disbursed_AutoLoanAmount = disbursementMemos.Where(d => d.Proposal.FacilityType == ProposalFacilityType.Auto_Loan).Sum(d => d.CurrentDisbursementAmount);

            data.ReceivedDepositAmount = (fundReceived.Sum(f => f.Amount) ?? 0);

            if (TopListLoan.Where(t => t.FacilityType == ProposalFacilityType.Home_Loan).Count() > 0)
            {
                data.RMHomeLoan1 = TopListLoan.Where(t => t.FacilityType == ProposalFacilityType.Home_Loan).FirstOrDefault().RM;
                data.RMHomeLoan1Amount = TopListLoan.Where(t => t.FacilityType == ProposalFacilityType.Home_Loan).FirstOrDefault().Amount;
                data.RMHomeLoan1Count = TopListLoan.Where(t => t.FacilityType == ProposalFacilityType.Home_Loan).FirstOrDefault().Applications;
            }
            if (TopListLoan.Where(t => t.FacilityType == ProposalFacilityType.Home_Loan).Count() > 1)
            {
                data.RMHomeLoan2 = TopListLoan.Where(t => t.FacilityType == ProposalFacilityType.Home_Loan).Skip(1).FirstOrDefault().RM;
                data.RMHomeLoan2Amount = TopListLoan.Where(t => t.FacilityType == ProposalFacilityType.Home_Loan).Skip(1).FirstOrDefault().Amount;
                data.RMHomeLoan2Count = TopListLoan.Where(t => t.FacilityType == ProposalFacilityType.Home_Loan).Skip(1).FirstOrDefault().Applications;
            }
            if (TopListLoan.Where(t => t.FacilityType == ProposalFacilityType.Personal_Loan).Count() > 0)
            {
                data.RMPersonalLoan = TopListLoan.Where(t => t.FacilityType == ProposalFacilityType.Personal_Loan).FirstOrDefault().RM;
                data.RMPersonalLoanAmount = TopListLoan.Where(t => t.FacilityType == ProposalFacilityType.Personal_Loan).FirstOrDefault().Amount;
                data.RMPersonalLoanCount = TopListLoan.Where(t => t.FacilityType == ProposalFacilityType.Personal_Loan).FirstOrDefault().Applications;
            }
            if (TopListLoan.Where(t => t.FacilityType == ProposalFacilityType.Auto_Loan).Count() > 0)
            {
                data.RMAutoLoan = TopListLoan.Where(t => t.FacilityType == ProposalFacilityType.Auto_Loan).FirstOrDefault().RM;
                data.RMAutoLoanAmount = TopListLoan.Where(t => t.FacilityType == ProposalFacilityType.Auto_Loan).FirstOrDefault().Amount;
                data.RMAutoLoanCount = TopListLoan.Where(t => t.FacilityType == ProposalFacilityType.Auto_Loan).FirstOrDefault().Applications;
            }
            if (TopListDeposit.Count() > 0)
            {
                data.RMLiability1 = TopListDeposit.FirstOrDefault().RM;
                data.RMLiability1Amount = TopListDeposit.FirstOrDefault().Amount;
                data.RMLiability1Count = TopListDeposit.FirstOrDefault().Applications;
            }
            if (TopListDeposit.Count() > 1)
            {
                data.RMLiability2 = TopListDeposit.FirstOrDefault().RM;
                data.RMLiability2Amount = TopListDeposit.FirstOrDefault().Amount;
                data.RMLiability2Count = TopListDeposit.FirstOrDefault().Applications;
            }

            return data;
        }

        public object GetNSMBMDashboardHighlights(TimeLine? timeLine, long? branchId)
        {
            var data = new DashBoardHighlightsDto();
            DateRangeDto dateRange;
            if (timeLine == null)
                dateRange = new DateRangeDto();
            else
                dateRange = new DateRangeDto((TimeLine)timeLine);

            var callListToday = new List<Call>();

            var leadListToday = new List<SalesLead>();

            var applicationListToDay = new List<ApplicationLog>();

            var approvedApplicationListToDay = new List<ApplicationLog>();

            var disApprovedApplicationListToDay = new List<ApplicationLog>();
            var disbursementMemosToday = new List<DisbursementMemo>();
            var fundReceivedToday = new List<FundConfirmation>();

            //var TopListLoan = new List<DisbursementMemo>();
            //var TopListDeposit = new List<FundConfirmation>();

            var disbursedMemodetails = new List<DMDetail>();
            disbursedMemodetails = GenService.GetAll<DMDetail>().Where(dm => dm.Status == EntityStatus.Active).ToList();
            var fundConfirmdetails = new List<FundConfirmationDetail>();
            fundConfirmdetails = GenService.GetAll<FundConfirmationDetail>().Where(fc => fc.Status == EntityStatus.Active).ToList();

            #region data population

            if (branchId > 0)
            {
                List<EmployeeDto> subordinateEmpList = new List<EmployeeDto>();

                subordinateEmpList = _office.GetEmployeesByOfficeId((long)branchId);
                var subordinateEmpIdList = subordinateEmpList.Select(s => s.Id).ToList();
                var subordinateUserList = new List<long>();
                foreach (var sub in subordinateEmpList)
                {
                    var subUser = _user.GetUserByEmployeeId((long)sub.Id);
                    if (subUser != null)
                        subordinateUserList.Add((long)subUser.Id);
                }

                var allApplications = GenService.GetAll<Application>().Where(a => subordinateEmpIdList.Contains(a.RMId));

                #region Call

                callListToday = GenService.GetAll<Call>().Where(c => c.CreateDate <= dateRange.ToDate && c.CreateDate >= dateRange.FromDate && c.Amount != null && subordinateUserList.Contains((long)c.CreatedBy) && c.Status == EntityStatus.Active).ToList();

                #endregion Call

                #region Lead

                leadListToday = GenService.GetAll<SalesLead>().Where(c => c.CreateDate <= dateRange.ToDate && c.CreateDate >= dateRange.FromDate && c.Amount != null && subordinateEmpIdList.Contains(c.CreatedBy) && c.Status == EntityStatus.Active).ToList();

                #endregion Lead

                #region File Submited Apps.

                //&& submittedApplicationIds.Contains(l.ApplicationId) &&
                var acceptableLogsToday = GenService.GetAll<ApplicationLog>()
                    .Where(l => allApplications.Any(a => a.Id == l.ApplicationId)
                    && (l.Application.LoanApplicationId != null || l.Application.DepositApplicationId != null)
                    && l.Status == EntityStatus.Active
                    && l.CreateDate <= dateRange.ToDate
                    && l.CreateDate >= dateRange.FromDate
                    && (((l.ToStage == ApplicationStage.SentToCRM || l.ToStage == ApplicationStage.UnderProcessAtCRM)
                    && l.Application.Product.ProductType == ProductType.Loan)
                    || ((l.ToStage == ApplicationStage.SentToOperations || l.ToStage == ApplicationStage.UnderProcessAtOperations)
                    && l.Application.Product.ProductType == ProductType.Deposit)));
                var allApplicationsToday = (from appLog in acceptableLogsToday
                                            orderby appLog.Id
                                            select appLog).Distinct().ToList();
                if (allApplicationsToday.Any())
                    applicationListToDay.AddRange(allApplicationsToday);

                #endregion File Submited Apps.

                #region File Approved/DisApproved ToDay Apps.

                var loanApplicationsIds = allApplications.Where(a => a.Product.ProductType == ProductType.Loan && a.LoanApplication.LoanAmountApplied != null).Select(a => a.Id);

                var approvedLoanApplicationLogsToday = GenService.GetAll<ApplicationLog>()
                    .Where(l => l.Status == EntityStatus.Active &&
                                l.ToStage == ApplicationStage.SentToOperations &&
                                loanApplicationsIds.Contains(l.ApplicationId) &&
                                l.CreateDate <= dateRange.ToDate &&
                                l.CreateDate >= dateRange.FromDate).ToList();
                if (approvedLoanApplicationLogsToday.Count > 0)
                    approvedApplicationListToDay.AddRange(approvedLoanApplicationLogsToday);

                var disApporvedLoanApplicationLogsToday = GenService.GetAll<ApplicationLog>()
                .Where(l => l.Status == EntityStatus.Active &&
                            l.ToStage < ApplicationStage.Drafted &&
                            loanApplicationsIds.Contains(l.ApplicationId) &&
                            l.CreateDate <= dateRange.ToDate &&
                            l.CreateDate >= dateRange.FromDate).ToList();

                disApprovedApplicationListToDay.AddRange(disApporvedLoanApplicationLogsToday);


                #endregion File Approved/DisApproved ToDay Apps.

                #region Amount disbursed / received Today

                var acceptableAppIds = applicationListToDay.Select(a => a.ApplicationId).Distinct().ToList();
                var TodaydisbursementMemos = GenService.GetAll<DMDetail>().Where(d => d.DisbursementMemo.ApplicationId != null
                            && d.DisbursementMemo.Application.BranchId != null
                            && d.DisbursementMemo.Application.BranchId == branchId
                            && d.DisbursementMemo.Application.LoanApplicationId != null
                            && d.DisbursementMemo.Application.Product.FacilityType != null
                            && (d.DisbursementMemo.Application.Product.FacilityType == ProposalFacilityType.Home_Loan
                                || d.DisbursementMemo.Application.Product.FacilityType == ProposalFacilityType.Auto_Loan
                                || d.DisbursementMemo.Application.Product.FacilityType == ProposalFacilityType.Personal_Loan)
                            && d.CreateDate <= dateRange.ToDate
                            && d.CreateDate >= dateRange.FromDate
                            && d.DisbursementMemo.IsApproved != null
                            && d.DisbursementMemo.IsApproved == true
                            && d.DisbursementMemo.IsDisbursed != null
                            && d.DisbursementMemo.IsDisbursed == true
                            && d.Status == EntityStatus.Active).ToList();
                var acceptableDMidstoday = TodaydisbursementMemos.Select(dm => dm.DisbursementMemoId).ToList();
                var disMemotoday = GenService.GetAll<DisbursementMemo>().Where(d => acceptableDMidstoday.Contains(d.Id)).ToList();
                disbursementMemosToday = disMemotoday.ToList();

                var TodayfundReceivedToday = GenService.GetAll<FundConfirmationDetail>().Where(d => d.FundConfirmation.ApplicationId != null
                            && d.FundConfirmation.Application.BranchId != null
                            && d.FundConfirmation.Application.BranchId == branchId
                            && d.FundConfirmation.Application.DepositApplicationId != null
                            && d.FundConfirmation.Application.Product.DepositType != null
                            && (d.FundConfirmation.Application.Product.DepositType == DepositType.Fixed
                                || d.FundConfirmation.Application.Product.DepositType == DepositType.Recurring)
                            && d.CreateDate <= dateRange.ToDate
                            && d.CreateDate >= dateRange.FromDate
                            && d.FundConfirmation.FundReceived != null
                            && d.FundConfirmation.FundReceived == true
                            && d.Status == EntityStatus.Active).ToList();
                var acceptableFCidstoday = TodayfundReceivedToday.Select(dm => dm.FundConfirmationId).ToList();
                var fundRectoday = GenService.GetAll<FundConfirmation>().Where(d => acceptableFCidstoday.Contains(d.Id)).ToList();
                fundReceivedToday = fundRectoday.ToList();

                #endregion Amount disbursed / received Today

                var TopListLoan = (from disbursement in disbursementMemosToday.Select(d => new
                {
                    ApplicationId = d.ApplicationId,
                    Amount = d.CurrentDisbursementAmount,
                    RM = d.Application.RMEmp.Person.FirstName + " " + d.Application.RMEmp.Person.LastName,
                    FacilityType = d.Proposal.FacilityType
                })
                                   group disbursement by new { disbursement.RM, disbursement.FacilityType } into grouped
                                   select new
                                   {
                                       RM = grouped.Key.RM,
                                       FacilityType = grouped.Key.FacilityType,
                                       Amount = grouped.Sum(s => s.Amount),
                                       Applications = grouped.Count()
                                   }).OrderBy(o => o.FacilityType).ThenBy(o => o.Amount);

                var TopListDeposit = (from funds in fundReceivedToday.Select(d => new
                {
                    Amount = d.Application.DepositApplication.TotalDepositAmount,
                    ApplicationId = d.ApplicationId,
                    RM = d.Application.RMEmp.Person.FirstName + " " + d.Application.RMEmp.Person.LastName
                })
                                      group funds by funds.RM into grouped
                                      select new
                                      {
                                          Amount = grouped.Sum(s => s.Amount),
                                          RM = grouped.Key,
                                          Applications = grouped.Count()
                                      }).OrderBy(d => d.Amount).ThenBy(d => d.Applications);

                if (TopListLoan.Where(t => t.FacilityType == ProposalFacilityType.Home_Loan).Count() > 0)
                {
                    data.RMHomeLoan1 = TopListLoan.Where(t => t.FacilityType == ProposalFacilityType.Home_Loan).FirstOrDefault().RM;
                    data.RMHomeLoan1Amount = TopListLoan.Where(t => t.FacilityType == ProposalFacilityType.Home_Loan).FirstOrDefault().Amount;
                    data.RMHomeLoan1Count = TopListLoan.Where(t => t.FacilityType == ProposalFacilityType.Home_Loan).FirstOrDefault().Applications;
                }
                if (TopListLoan.Where(t => t.FacilityType == ProposalFacilityType.Home_Loan).Count() > 1)
                {
                    data.RMHomeLoan2 = TopListLoan.Where(t => t.FacilityType == ProposalFacilityType.Home_Loan).Skip(1).FirstOrDefault().RM;
                    data.RMHomeLoan2Amount = TopListLoan.Where(t => t.FacilityType == ProposalFacilityType.Home_Loan).Skip(1).FirstOrDefault().Amount;
                    data.RMHomeLoan2Count = TopListLoan.Where(t => t.FacilityType == ProposalFacilityType.Home_Loan).Skip(1).FirstOrDefault().Applications;
                }
                if (TopListLoan.Where(t => t.FacilityType == ProposalFacilityType.Personal_Loan).Count() > 0)
                {
                    data.RMPersonalLoan = TopListLoan.Where(t => t.FacilityType == ProposalFacilityType.Personal_Loan).FirstOrDefault().RM;
                    data.RMPersonalLoanAmount = TopListLoan.Where(t => t.FacilityType == ProposalFacilityType.Personal_Loan).FirstOrDefault().Amount;
                    data.RMPersonalLoanCount = TopListLoan.Where(t => t.FacilityType == ProposalFacilityType.Personal_Loan).FirstOrDefault().Applications;
                }
                if (TopListLoan.Where(t => t.FacilityType == ProposalFacilityType.Auto_Loan).Count() > 0)
                {
                    data.RMAutoLoan = TopListLoan.Where(t => t.FacilityType == ProposalFacilityType.Auto_Loan).FirstOrDefault().RM;
                    data.RMAutoLoanAmount = TopListLoan.Where(t => t.FacilityType == ProposalFacilityType.Auto_Loan).FirstOrDefault().Amount;
                    data.RMAutoLoanCount = TopListLoan.Where(t => t.FacilityType == ProposalFacilityType.Auto_Loan).FirstOrDefault().Applications;
                }
                if (TopListDeposit.Count() > 0)
                {
                    data.RMLiability1 = TopListDeposit.FirstOrDefault().RM;
                    data.RMLiability1Amount = TopListDeposit.FirstOrDefault().Amount;
                    data.RMLiability1Count = TopListDeposit.FirstOrDefault().Applications;
                }
                if (TopListDeposit.Count() > 1)
                {
                    data.RMLiability2 = TopListDeposit.FirstOrDefault().RM;
                    data.RMLiability2Amount = TopListDeposit.FirstOrDefault().Amount;
                    data.RMLiability2Count = TopListDeposit.FirstOrDefault().Applications;
                }
            }
            else
            {
                #region Call/Lead
                callListToday = GenService.GetAll<Call>().Where(c => c.CreateDate <= dateRange.ToDate && c.CreateDate >= dateRange.FromDate && c.Amount != null && c.Status == EntityStatus.Active).ToList();

                leadListToday = GenService.GetAll<SalesLead>().Where(c => c.CreateDate <= dateRange.ToDate && c.CreateDate >= dateRange.FromDate && c.Amount != null && c.Status == EntityStatus.Active).ToList();

                #endregion Call/Lead

                #region Submited Apps
                var acceptableLogsToday = GenService.GetAll<ApplicationLog>()
                    .Where(l => (l.Application.LoanApplicationId != null || l.Application.DepositApplicationId != null)
                    && l.Status == EntityStatus.Active
                    && l.CreateDate <= dateRange.ToDate
                    && l.CreateDate >= dateRange.FromDate
                    && (((l.ToStage == ApplicationStage.SentToCRM || l.ToStage == ApplicationStage.UnderProcessAtCRM)
                    && l.Application.Product.ProductType == ProductType.Loan)
                    || ((l.ToStage == ApplicationStage.SentToOperations || l.ToStage == ApplicationStage.UnderProcessAtOperations)
                    && l.Application.Product.ProductType == ProductType.Deposit)));
                var allApplicationsToday = (from appLog in acceptableLogsToday
                                            orderby appLog.Id
                                            select appLog).Distinct().ToList();
                if (allApplicationsToday.Any())
                    applicationListToDay.AddRange(allApplicationsToday);

                #endregion Submited Apps

                #region File Approved/DisApproved ToDay Apps.

                var loanApplicationsIds = GenService.GetAll<Application>().Where(a => a.Product.ProductType == ProductType.Loan).Select(a => a.Id);

                var approvedLoanApplicationLogsToday = GenService.GetAll<ApplicationLog>()
                .Where(l => l.Status == EntityStatus.Active &&
                                l.ToStage == ApplicationStage.SentToOperations &&
                                loanApplicationsIds.Contains(l.ApplicationId) &&
                l.CreateDate <= dateRange.ToDate &&
                                l.CreateDate >= dateRange.FromDate).ToList();
                if (approvedLoanApplicationLogsToday.Count > 0)
                    approvedApplicationListToDay.AddRange(approvedLoanApplicationLogsToday);

                var disApporvedLoanApplicationLogsToday = GenService.GetAll<ApplicationLog>()
                .Where(l => l.Status == EntityStatus.Active &&
                            l.ToStage < ApplicationStage.Drafted &&
                            loanApplicationsIds.Contains(l.ApplicationId) &&
                            l.CreateDate <= dateRange.ToDate &&
                            l.CreateDate >= dateRange.FromDate).ToList();

                disApprovedApplicationListToDay.AddRange(disApporvedLoanApplicationLogsToday);


                #endregion File Approved/DisApproved ToDay Apps.

                #region File disbursed / received Today
                var TodaydisbursementMemos = GenService.GetAll<DMDetail>().Where(d => d.DisbursementMemo.ApplicationId != null

                            && d.DisbursementMemo.Application.LoanApplicationId != null
                            && d.DisbursementMemo.Application.Product.FacilityType != null
                            && (d.DisbursementMemo.Application.Product.FacilityType == ProposalFacilityType.Home_Loan
                                || d.DisbursementMemo.Application.Product.FacilityType == ProposalFacilityType.Auto_Loan
                                || d.DisbursementMemo.Application.Product.FacilityType == ProposalFacilityType.Personal_Loan)
                            && d.CreateDate <= dateRange.ToDate
                            && d.CreateDate >= dateRange.FromDate
                            && d.DisbursementMemo.IsApproved != null
                            && d.DisbursementMemo.IsApproved == true
                            && d.DisbursementMemo.IsDisbursed != null
                            && d.DisbursementMemo.IsDisbursed == true
                            && d.Status == EntityStatus.Active).ToList();
                var acceptableDMids = TodaydisbursementMemos.Select(dm => dm.DisbursementMemoId).ToList();
                var disMemotoday = GenService.GetAll<DisbursementMemo>().Where(d => acceptableDMids.Contains(d.Id)).ToList();
                disbursementMemosToday = disMemotoday.ToList();

                var TodayfundReceived = GenService.GetAll<FundConfirmationDetail>().Where(d => d.FundConfirmation.ApplicationId != null

                            && d.FundConfirmation.Application.DepositApplicationId != null
                            && d.FundConfirmation.Application.Product.DepositType != null
                            && (d.FundConfirmation.Application.Product.DepositType == DepositType.Fixed
                                || d.FundConfirmation.Application.Product.DepositType == DepositType.Recurring)
                            && d.CreateDate <= dateRange.ToDate
                            && d.CreateDate >= dateRange.FromDate
                            && d.FundConfirmation.FundReceived != null
                            && d.FundConfirmation.FundReceived == true
                            && d.Status == EntityStatus.Active).ToList();
                var acceptableFCids = TodayfundReceived.Select(dm => dm.FundConfirmationId).ToList();
                var fundRectoday = GenService.GetAll<FundConfirmation>().Where(d => acceptableFCids.Contains(d.Id)).ToList();
                fundReceivedToday = fundRectoday.ToList();

                #endregion File disbursed / received Today
                var TopListLoan = (from disbursement in disbursementMemosToday.Select(d => new
                {
                    ApplicationId = d.ApplicationId,
                    Amount = d.CurrentDisbursementAmount,
                    RM = d.Application.RMEmp.Person.FirstName + " " + d.Application.RMEmp.Person.LastName,
                    FacilityType = d.Proposal.FacilityType
                })
                                   group disbursement by new { disbursement.RM, disbursement.FacilityType } into grouped
                                   select new
                                   {
                                       RM = grouped.Key.RM,
                                       FacilityType = grouped.Key.FacilityType,
                                       Amount = grouped.Sum(s => s.Amount),
                                       Applications = grouped.Count()
                                   }).OrderBy(o => o.FacilityType).ThenBy(o => o.Amount);

                var TopListDeposit = (from funds in fundReceivedToday.Select(d => new
                {
                    Amount = d.Application.DepositApplication.TotalDepositAmount,
                    ApplicationId = d.ApplicationId,
                    RM = d.Application.RMEmp.Person.FirstName + " " + d.Application.RMEmp.Person.LastName
                })
                                      group funds by funds.RM into grouped
                                      select new
                                      {
                                          Amount = grouped.Sum(s => s.Amount),
                                          RM = grouped.Key,
                                          Applications = grouped.Count()
                                      }).OrderBy(d => d.Amount).ThenBy(d => d.Applications);

                if (TopListLoan.Where(t => t.FacilityType == ProposalFacilityType.Home_Loan).Count() > 0)
                {
                    data.RMHomeLoan1 = TopListLoan.Where(t => t.FacilityType == ProposalFacilityType.Home_Loan).FirstOrDefault().RM;
                    data.RMHomeLoan1Amount = TopListLoan.Where(t => t.FacilityType == ProposalFacilityType.Home_Loan).FirstOrDefault().Amount;
                    data.RMHomeLoan1Count = TopListLoan.Where(t => t.FacilityType == ProposalFacilityType.Home_Loan).FirstOrDefault().Applications;
                }
                if (TopListLoan.Where(t => t.FacilityType == ProposalFacilityType.Home_Loan).Count() > 1)
                {
                    data.RMHomeLoan2 = TopListLoan.Where(t => t.FacilityType == ProposalFacilityType.Home_Loan).Skip(1).FirstOrDefault().RM;
                    data.RMHomeLoan2Amount = TopListLoan.Where(t => t.FacilityType == ProposalFacilityType.Home_Loan).Skip(1).FirstOrDefault().Amount;
                    data.RMHomeLoan2Count = TopListLoan.Where(t => t.FacilityType == ProposalFacilityType.Home_Loan).Skip(1).FirstOrDefault().Applications;
                }
                if (TopListLoan.Where(t => t.FacilityType == ProposalFacilityType.Personal_Loan).Count() > 0)
                {
                    data.RMPersonalLoan = TopListLoan.Where(t => t.FacilityType == ProposalFacilityType.Personal_Loan).FirstOrDefault().RM;
                    data.RMPersonalLoanAmount = TopListLoan.Where(t => t.FacilityType == ProposalFacilityType.Personal_Loan).FirstOrDefault().Amount;
                    data.RMPersonalLoanCount = TopListLoan.Where(t => t.FacilityType == ProposalFacilityType.Personal_Loan).FirstOrDefault().Applications;
                }
                if (TopListLoan.Where(t => t.FacilityType == ProposalFacilityType.Auto_Loan).Count() > 0)
                {
                    data.RMAutoLoan = TopListLoan.Where(t => t.FacilityType == ProposalFacilityType.Auto_Loan).FirstOrDefault().RM;
                    data.RMAutoLoanAmount = TopListLoan.Where(t => t.FacilityType == ProposalFacilityType.Auto_Loan).FirstOrDefault().Amount;
                    data.RMAutoLoanCount = TopListLoan.Where(t => t.FacilityType == ProposalFacilityType.Auto_Loan).FirstOrDefault().Applications;
                }
                if (TopListDeposit.Count() > 0)
                {
                    data.RMLiability1 = TopListDeposit.FirstOrDefault().RM;
                    data.RMLiability1Amount = TopListDeposit.FirstOrDefault().Amount;
                    data.RMLiability1Count = TopListDeposit.FirstOrDefault().Applications;
                }
                if (TopListDeposit.Count() > 1)
                {
                    data.RMLiability2 = TopListDeposit.FirstOrDefault().RM;
                    data.RMLiability2Amount = TopListDeposit.FirstOrDefault().Amount;
                    data.RMLiability2Count = TopListDeposit.FirstOrDefault().Applications;
                }

            }


            #endregion


            #region initializations
            data.Disbursed_HomeLoanAmount = 0;
            data.Disbursed_PersonalLoanAmount = 0;
            data.Disbursed_AutoLoanAmount = 0;
            data.RMHomeLoan1 = "";
            data.RMHomeLoan1Amount = 0;
            data.RMHomeLoan1Count = 0;
            data.RMHomeLoan2 = "";
            data.RMHomeLoan2Amount = 0;
            data.RMHomeLoan2Count = 0;
            data.RMPersonalLoan = "";
            data.RMPersonalLoanAmount = 0;
            data.RMPersonalLoanCount = 0;
            data.RMAutoLoan = "";
            data.RMAutoLoanAmount = 0;
            data.RMAutoLoanCount = 0;
            data.RMLiability1 = "";
            data.RMLiability1Amount = 0;
            data.RMLiability1Count = 0;
            data.RMLiability2 = "";
            data.RMLiability2Amount = 0;
            data.RMLiability2Count = 0;
            #endregion
            data.Call_HomeLoan = callListToday.Where(c => c.ProductId != null && c.Product.FacilityType == ProposalFacilityType.Home_Loan).Count();
            data.Call_PersonalLoan = callListToday.Where(c => c.ProductId != null && c.Product.FacilityType == ProposalFacilityType.Personal_Loan).Count();
            data.Call_AutoLoan = callListToday.Where(c => c.ProductId != null && c.Product.FacilityType == ProposalFacilityType.Auto_Loan).Count();
            data.Call_Liability = callListToday.Where(c => c.ProductId != null && c.Product.ProductType == ProductType.Deposit).Count();
            data.Call_Undefined = callListToday.Where(c => c.ProductId == null).Count();

            data.Lead_HomeLoan = leadListToday.Where(l => l.ProductId != null && l.Product.FacilityType == ProposalFacilityType.Home_Loan).Count();
            data.Lead_PersonalLoan = leadListToday.Where(l => l.ProductId != null && l.Product.FacilityType == ProposalFacilityType.Personal_Loan).Count();
            data.Lead_AutoLoan = leadListToday.Where(l => l.ProductId != null && l.Product.FacilityType == ProposalFacilityType.Auto_Loan).Count();
            data.Lead_Liability = leadListToday.Where(l => l.ProductId != null && l.Product.ProductType == ProductType.Deposit).Count();

            data.Application_HomeLoan = applicationListToDay.Where(a => a.Application.Product.FacilityType == ProposalFacilityType.Home_Loan).Count();
            data.Application_PersonalLoan = applicationListToDay.Where(a => a.Application.Product.FacilityType == ProposalFacilityType.Personal_Loan).Count();
            data.Application_AutoLoan = applicationListToDay.Where(a => a.Application.Product.FacilityType == ProposalFacilityType.Auto_Loan).Count();
            data.Application_Liability = applicationListToDay.Where(a => a.Application.Product.ProductType == ProductType.Deposit).Count();

            #region File Approved Apps.
            var approvedApplicationListTodayHome = approvedApplicationListToDay.Where(l => l.Application.LoanApplicationId != null && l.Application.ProductId != null && l.Application.Product.FacilityType != null && l.Application.Product.FacilityType == ProposalFacilityType.Home_Loan).ToList();

            var approvedHomeLoanLogsWithAmountToday = new List<Proposal>();

            if (approvedApplicationListTodayHome.Count > 0)
            {
                //approvedHomeLoanLogsWithAmountToday = approvedHomeLoanLogsWithAmountToday.Where(p => approvedApplicationListTodayHome.Any(a => a.ApplicationId == p.ApplicationId)).ToList();
                approvedHomeLoanLogsWithAmountToday = (from logs in approvedApplicationListTodayHome
                                                       join proposal in GenService.GetAll<Proposal>().Where(p => p.Status == EntityStatus.Active) on
                                                           logs.ApplicationId equals proposal.ApplicationId into proposals
                                                       from p in proposals.DefaultIfEmpty()
                                                       select p).ToList();
            }

            var approvedApplicationListTodayAuto = approvedApplicationListToDay.Where(l => l.Application.LoanApplicationId != null && l.Application.ProductId != null && l.Application.Product.FacilityType != null && l.Application.Product.FacilityType == ProposalFacilityType.Auto_Loan).ToList();

            var approvedAutoLoanLogsWithAmountToday = new List<Proposal>();

            if (approvedApplicationListTodayAuto.Count > 0)
            {
                //approvedAutoLoanLogsWithAmountToday = GenService.GetAll<Proposal>().Where(p => approvedApplicationListTodayAuto.Any(a => a.ApplicationId == p.ApplicationId)).ToList();
                approvedAutoLoanLogsWithAmountToday = (from logs in approvedApplicationListTodayAuto
                                                       join proposal in GenService.GetAll<Proposal>().Where(p => p.Status == EntityStatus.Active) on
                                                           logs.ApplicationId equals proposal.ApplicationId into proposals
                                                       from p in proposals.DefaultIfEmpty()
                                                       select p).ToList();
            }

            var approvedApplicationListTodayPersonal = approvedApplicationListToDay.Where(l => l.Application.LoanApplicationId != null && l.Application.ProductId != null && l.Application.Product.FacilityType != null && l.Application.Product.FacilityType == ProposalFacilityType.Personal_Loan).ToList();
            var approvedPersonalLoanLogsWithAmountToday = new List<Proposal>();

            if (approvedApplicationListTodayPersonal.Count > 0)
            {
                //approvedPersonalLoanLogsWithAmountToday = GenService.GetAll<Proposal>().Where(p => approvedApplicationListTodayPersonal.Any(a => a.ApplicationId == p.ApplicationId)).ToList();
                approvedPersonalLoanLogsWithAmountToday = (from logs in approvedApplicationListTodayPersonal
                                                           join proposal in GenService.GetAll<Proposal>().Where(p => p.Status == EntityStatus.Active) on
                                                               logs.ApplicationId equals proposal.ApplicationId into proposals
                                                           from p in proposals.DefaultIfEmpty()
                                                           select p).ToList();
            }

            var disApprovedApplicationListTodayHome = disApprovedApplicationListToDay.Where(l => l.Application.LoanApplicationId != null && l.Application.ProductId != null && l.Application.Product.FacilityType != null && l.Application.Product.FacilityType == ProposalFacilityType.Home_Loan).ToList();

            var disApprovedHomeLoanLogsWithAmountToday = new List<Proposal>();

            if (disApprovedApplicationListTodayHome.Count > 0)
            {
                //disApprovedHomeLoanLogsWithAmountToday = GenService.GetAll<Proposal>().Where(p => disApprovedApplicationListTodayHome.Any(a => a.ApplicationId == p.ApplicationId)).ToList();
                disApprovedHomeLoanLogsWithAmountToday = (from logs in disApprovedApplicationListTodayHome
                                                          join proposal in GenService.GetAll<Proposal>().Where(p => p.Status == EntityStatus.Active) on
                                                              logs.ApplicationId equals proposal.ApplicationId into proposals
                                                          from p in proposals.DefaultIfEmpty()
                                                          select p).ToList();
            }


            var disApprovedApplicationListTodayAuto = disApprovedApplicationListToDay.Where(l => l.Application.LoanApplicationId != null && l.Application.ProductId != null && l.Application.Product.FacilityType != null && l.Application.Product.FacilityType == ProposalFacilityType.Auto_Loan).ToList();

            var disApprovedAutoLoanLogsWithAmountToday = new List<Proposal>();

            if (disApprovedApplicationListTodayAuto.Count > 0)
            {
                //disApprovedAutoLoanLogsWithAmountToday = GenService.GetAll<Proposal>().Where(p => disApprovedApplicationListTodayAuto.Any(a => a.ApplicationId == p.ApplicationId)).ToList();
                disApprovedAutoLoanLogsWithAmountToday = (from logs in disApprovedApplicationListTodayAuto
                                                          join proposal in GenService.GetAll<Proposal>().Where(p => p.Status == EntityStatus.Active) on
                                                              logs.ApplicationId equals proposal.ApplicationId into proposals
                                                          from p in proposals.DefaultIfEmpty()
                                                          select p).ToList();
            }


            var disApprovedApplicationListTodayPersonal = disApprovedApplicationListToDay.Where(l => l.Application.LoanApplicationId != null && l.Application.ProductId != null && l.Application.Product.FacilityType != null && l.Application.Product.FacilityType == ProposalFacilityType.Personal_Loan).ToList();
            var disApprovedPersonalLoanLogsWithAmountToday = new List<Proposal>();

            if (disApprovedApplicationListTodayPersonal.Count > 0)
            {
                //disApprovedPersonalLoanLogsWithAmountToday = GenService.GetAll<Proposal>().Where(p => disApprovedApplicationListTodayPersonal.Any(a => a.ApplicationId == p.ApplicationId)).ToList();
                disApprovedPersonalLoanLogsWithAmountToday = (from logs in disApprovedApplicationListTodayPersonal
                                                              join proposal in GenService.GetAll<Proposal>().Where(p => p.Status == EntityStatus.Active) on
                                                                  logs.ApplicationId equals proposal.ApplicationId into proposals
                                                              from p in proposals.DefaultIfEmpty()
                                                              select p).ToList();
            }


            #endregion File Approved Apps.

            data.Approved_HomeLoan = approvedHomeLoanLogsWithAmountToday.Where(a => a.FacilityType == ProposalFacilityType.Home_Loan).Count();
            data.Approved_PersonalLoan = approvedPersonalLoanLogsWithAmountToday.Where(a => a.FacilityType == ProposalFacilityType.Personal_Loan).Count();
            data.Approved_AutoLoan = approvedAutoLoanLogsWithAmountToday.Where(a => a.FacilityType == ProposalFacilityType.Auto_Loan).Count();
            data.Approved_HomeLoanAmount = (approvedHomeLoanLogsWithAmountToday.Where(a => a.FacilityType == ProposalFacilityType.Home_Loan).Sum(a => a.RecomendedLoanAmountFromIPDC != null ? a.RecomendedLoanAmountFromIPDC : 0) ?? 0);
            data.Approved_PersonalLoanAmount = (approvedPersonalLoanLogsWithAmountToday.Where(a => a.FacilityType == ProposalFacilityType.Home_Loan).Sum(a => a.RecomendedLoanAmountFromIPDC != null ? a.RecomendedLoanAmountFromIPDC : 0) ?? 0);
            data.Approved_AutoLoanAmount = (approvedAutoLoanLogsWithAmountToday.Where(a => a.FacilityType == ProposalFacilityType.Home_Loan).Sum(a => a.RecomendedLoanAmountFromIPDC != null ? a.RecomendedLoanAmountFromIPDC : 0) ?? 0);

            data.Disapproved_HomeLoan = disApprovedHomeLoanLogsWithAmountToday.Where(a => a.FacilityType == ProposalFacilityType.Home_Loan).Count();
            data.Disapproved_PersonalLoan = disApprovedPersonalLoanLogsWithAmountToday.Where(a => a.FacilityType == ProposalFacilityType.Personal_Loan).Count();
            data.Disapproved_AutoLoan = disApprovedAutoLoanLogsWithAmountToday.Where(a => a.FacilityType == ProposalFacilityType.Auto_Loan).Count();
            data.Disapproved_HomeLoanAmount = (disApprovedHomeLoanLogsWithAmountToday.Where(a => a.FacilityType == ProposalFacilityType.Home_Loan).Sum(a => a.RecomendedLoanAmountFromIPDC != null ? a.RecomendedLoanAmountFromIPDC : 0) ?? 0);
            data.Disapproved_PersonalLoanAmount = (disApprovedPersonalLoanLogsWithAmountToday.Where(a => a.FacilityType == ProposalFacilityType.Personal_Loan).Sum(a => a.RecomendedLoanAmountFromIPDC != null ? a.RecomendedLoanAmountFromIPDC : 0) ?? 0);
            data.Disapproved_AutoLoanAmount = (disApprovedAutoLoanLogsWithAmountToday.Where(a => a.FacilityType == ProposalFacilityType.Auto_Loan).Sum(a => a.RecomendedLoanAmountFromIPDC != null ? a.RecomendedLoanAmountFromIPDC : 0) ?? 0);

            if (disbursementMemosToday.Where(d => d.Proposal.FacilityType == ProposalFacilityType.Home_Loan).Count() > 0)
                data.Disbursed_HomeLoanAmount = disbursementMemosToday.Where(d => d.Proposal.FacilityType == ProposalFacilityType.Home_Loan).Sum(d => d.CurrentDisbursementAmount);
            if (disbursementMemosToday.Where(d => d.Proposal.FacilityType == ProposalFacilityType.Personal_Loan).Count() > 0)
                data.Disbursed_PersonalLoanAmount = disbursementMemosToday.Where(d => d.Proposal.FacilityType == ProposalFacilityType.Personal_Loan).Sum(d => d.CurrentDisbursementAmount);
            if (disbursementMemosToday.Where(d => d.Proposal.FacilityType == ProposalFacilityType.Auto_Loan).Count() > 0)
                data.Disbursed_AutoLoanAmount = disbursementMemosToday.Where(d => d.Proposal.FacilityType == ProposalFacilityType.Auto_Loan).Sum(d => d.CurrentDisbursementAmount);

            data.ReceivedDepositAmount = fundReceivedToday.Sum(f => f.Application.DepositApplication.TotalDepositAmount);



            return data;
        }

        public object GetNSMBMDashboardHighlights(TimeLine? timeLine, List<long?> branchId)
        {
            var data = new DashBoardHighlightsDto();
            DateRangeDto dateRange;
            if (timeLine == null)
                dateRange = new DateRangeDto();
            else
                dateRange = new DateRangeDto((TimeLine)timeLine);

            var callListToday = new List<Call>();

            var leadListToday = new List<SalesLead>();

            var applicationListToDay = new List<ApplicationLog>();

            var approvedApplicationListToDay = new List<ApplicationLog>();

            var disApprovedApplicationListToDay = new List<ApplicationLog>();
            var disbursementMemosToday = new List<DisbursementMemo>();
            var fundReceivedToday = new List<FundConfirmation>();

            //var TopListLoan = new List<DisbursementMemo>();
            //var TopListDeposit = new List<FundConfirmation>();

            var disbursedMemodetails = new List<DMDetail>();
            disbursedMemodetails = GenService.GetAll<DMDetail>().Where(dm => dm.Status == EntityStatus.Active).ToList();
            var fundConfirmdetails = new List<FundConfirmationDetail>();
            fundConfirmdetails = GenService.GetAll<FundConfirmationDetail>().Where(fc => fc.Status == EntityStatus.Active).ToList();

            #region data population

            if (branchId != null && branchId.Count > 0)
            {
                List<EmployeeDto> subordinateEmpList = new List<EmployeeDto>();
                foreach (var eachBranch in branchId)
                {
                    if (eachBranch > 0)
                    {
                        var temp = _office.GetEmployeesByOfficeId((long)eachBranch);
                        if (temp != null)
                            subordinateEmpList.AddRange(temp);
                    }
                }
                var subordinateEmpIdList = subordinateEmpList.Select(s => s.Id).ToList();
                var subordinateUserList = new List<long>();
                foreach (var sub in subordinateEmpList)
                {
                    var subUser = _user.GetUserByEmployeeId((long)sub.Id);
                    if (subUser != null)
                        subordinateUserList.Add((long)subUser.Id);
                }

                var allApplications = GenService.GetAll<Application>().Where(a => subordinateEmpIdList.Contains(a.RMId));

                #region Call
                callListToday = GenService.GetAll<Call>().Where(c => c.CreateDate <= dateRange.ToDate && c.CreateDate >= dateRange.FromDate && c.Amount != null && subordinateUserList.Contains((long)c.CreatedBy) && c.Status == EntityStatus.Active).ToList();
                #endregion Call

                #region Lead

                leadListToday = GenService.GetAll<SalesLead>().Where(c => c.CreateDate <= dateRange.ToDate && c.CreateDate >= dateRange.FromDate && c.Amount != null && subordinateEmpIdList.Contains(c.CreatedBy) && c.Status == EntityStatus.Active).ToList();

                #endregion Lead

                #region File Submited Apps.

                //&& submittedApplicationIds.Contains(l.ApplicationId) &&
                var acceptableLogsToday = GenService.GetAll<ApplicationLog>()
                    .Where(l => allApplications.Any(a => a.Id == l.ApplicationId)
                    && (l.Application.LoanApplicationId != null || l.Application.DepositApplicationId != null)
                    && l.Status == EntityStatus.Active
                    && l.CreateDate <= dateRange.ToDate
                    && l.CreateDate >= dateRange.FromDate
                    && (((l.ToStage == ApplicationStage.SentToCRM || l.ToStage == ApplicationStage.UnderProcessAtCRM)
                    && l.Application.Product.ProductType == ProductType.Loan)
                    || ((l.ToStage == ApplicationStage.SentToOperations || l.ToStage == ApplicationStage.UnderProcessAtOperations)
                    && l.Application.Product.ProductType == ProductType.Deposit)));
                var allApplicationsToday = (from appLog in acceptableLogsToday
                                            orderby appLog.Id
                                            select appLog).Distinct().ToList();
                if (allApplicationsToday.Any())
                    applicationListToDay.AddRange(allApplicationsToday);

                #endregion File Submited Apps.

                #region File Approved/DisApproved ToDay Apps.

                var loanApplicationsIds = allApplications.Where(a => a.Product.ProductType == ProductType.Loan).Select(a => a.Id);

                var approvedLoanApplicationLogsToday = GenService.GetAll<ApplicationLog>()
                    .Where(l => l.Status == EntityStatus.Active &&
                                l.ToStage == ApplicationStage.SentToOperations &&
                                loanApplicationsIds.Contains(l.ApplicationId) &&
                                l.CreateDate <= dateRange.ToDate &&
                                l.CreateDate >= dateRange.FromDate).ToList();
                if (approvedLoanApplicationLogsToday.Count > 0)
                    approvedApplicationListToDay.AddRange(approvedLoanApplicationLogsToday);

                var disApporvedLoanApplicationLogsToday = GenService.GetAll<ApplicationLog>()
                .Where(l => l.Status == EntityStatus.Active &&
                            l.ToStage < ApplicationStage.Drafted &&
                            loanApplicationsIds.Contains(l.ApplicationId) &&
                            l.CreateDate <= dateRange.ToDate &&
                            l.CreateDate >= dateRange.FromDate).ToList();

                disApprovedApplicationListToDay.AddRange(disApporvedLoanApplicationLogsToday);


                #endregion File Approved/DisApproved ToDay Apps.

                #region Amount disbursed / received Today

                var acceptableAppIds = applicationListToDay.Select(a => a.ApplicationId).Distinct().ToList();
                var TodaydisbursementMemos = GenService.GetAll<DMDetail>().Where(d => d.DisbursementMemo.ApplicationId != null
                            && d.DisbursementMemo.Application.BranchId != null
                            && branchId.Contains(d.DisbursementMemo.Application.BranchId)
                            && d.DisbursementMemo.Application.LoanApplicationId != null
                            && d.DisbursementMemo.Application.Product.FacilityType != null
                            && (d.DisbursementMemo.Application.Product.FacilityType == ProposalFacilityType.Home_Loan
                                || d.DisbursementMemo.Application.Product.FacilityType == ProposalFacilityType.Auto_Loan
                                || d.DisbursementMemo.Application.Product.FacilityType == ProposalFacilityType.Personal_Loan)
                            && d.CreateDate <= dateRange.ToDate
                            && d.CreateDate >= dateRange.FromDate
                            && d.DisbursementMemo.IsApproved != null
                            && d.DisbursementMemo.IsApproved == true
                            && d.DisbursementMemo.IsDisbursed != null
                            && d.DisbursementMemo.IsDisbursed == true
                            && d.Status == EntityStatus.Active).ToList();
                var acceptableDMidstoday = TodaydisbursementMemos.Select(dm => dm.DisbursementMemoId).ToList();
                var disMemotoday = GenService.GetAll<DisbursementMemo>().Where(d => acceptableDMidstoday.Contains(d.Id)).ToList();
                disbursementMemosToday = disMemotoday.ToList();

                var TodayfundReceivedToday = GenService.GetAll<FundConfirmationDetail>().Where(d => d.FundConfirmation.ApplicationId != null
                            && d.FundConfirmation.Application.BranchId != null
                            && branchId.Contains(d.FundConfirmation.Application.BranchId)
                            && d.FundConfirmation.Application.DepositApplicationId != null
                            && d.FundConfirmation.Application.Product.DepositType != null
                            && (d.FundConfirmation.Application.Product.DepositType == DepositType.Fixed
                                || d.FundConfirmation.Application.Product.DepositType == DepositType.Recurring)
                            && d.CreateDate <= dateRange.ToDate
                            && d.CreateDate >= dateRange.FromDate
                            && d.FundConfirmation.FundReceived != null
                            && d.FundConfirmation.FundReceived == true
                            && d.Status == EntityStatus.Active).ToList();
                var acceptableFCidstoday = TodayfundReceivedToday.Select(dm => dm.FundConfirmationId).ToList();
                var fundRectoday = GenService.GetAll<FundConfirmation>().Where(d => acceptableFCidstoday.Contains(d.Id)).ToList();
                fundReceivedToday = fundRectoday.ToList();

                #endregion Amount disbursed / received Today

                var TopListLoan = (from disbursement in disbursementMemosToday.Select(d => new
                {
                    ApplicationId = d.ApplicationId,
                    Amount = d.CurrentDisbursementAmount,
                    RM = d.Application.RMEmp.Person.FirstName + " " + d.Application.RMEmp.Person.LastName,
                    FacilityType = d.Proposal.FacilityType
                })
                                   group disbursement by new { disbursement.RM, disbursement.FacilityType } into grouped
                                   select new
                                   {
                                       RM = grouped.Key.RM,
                                       FacilityType = grouped.Key.FacilityType,
                                       Amount = grouped.Sum(s => s.Amount),
                                       Applications = grouped.Count()
                                   }).OrderBy(o => o.FacilityType).ThenBy(o => o.Amount);

                var TopListDeposit = (from funds in fundReceivedToday.Select(d => new
                {
                    Amount = d.Application.DepositApplication.TotalDepositAmount,
                    ApplicationId = d.ApplicationId,
                    RM = d.Application.RMEmp.Person.FirstName + " " + d.Application.RMEmp.Person.LastName
                })
                                      group funds by funds.RM into grouped
                                      select new
                                      {
                                          Amount = grouped.Sum(s => s.Amount),
                                          RM = grouped.Key,
                                          Applications = grouped.Count()
                                      }).OrderBy(d => d.Amount).ThenBy(d => d.Applications);

                if (TopListLoan.Where(t => t.FacilityType == ProposalFacilityType.Home_Loan).Count() > 0)
                {
                    data.RMHomeLoan1 = TopListLoan.Where(t => t.FacilityType == ProposalFacilityType.Home_Loan).FirstOrDefault().RM;
                    data.RMHomeLoan1Amount = TopListLoan.Where(t => t.FacilityType == ProposalFacilityType.Home_Loan).FirstOrDefault().Amount;
                    data.RMHomeLoan1Count = TopListLoan.Where(t => t.FacilityType == ProposalFacilityType.Home_Loan).FirstOrDefault().Applications;
                }
                if (TopListLoan.Where(t => t.FacilityType == ProposalFacilityType.Home_Loan).Count() > 1)
                {
                    data.RMHomeLoan2 = TopListLoan.Where(t => t.FacilityType == ProposalFacilityType.Home_Loan).Skip(1).FirstOrDefault().RM;
                    data.RMHomeLoan2Amount = TopListLoan.Where(t => t.FacilityType == ProposalFacilityType.Home_Loan).Skip(1).FirstOrDefault().Amount;
                    data.RMHomeLoan2Count = TopListLoan.Where(t => t.FacilityType == ProposalFacilityType.Home_Loan).Skip(1).FirstOrDefault().Applications;
                }
                if (TopListLoan.Where(t => t.FacilityType == ProposalFacilityType.Personal_Loan).Count() > 0)
                {
                    data.RMPersonalLoan = TopListLoan.Where(t => t.FacilityType == ProposalFacilityType.Personal_Loan).FirstOrDefault().RM;
                    data.RMPersonalLoanAmount = TopListLoan.Where(t => t.FacilityType == ProposalFacilityType.Personal_Loan).FirstOrDefault().Amount;
                    data.RMPersonalLoanCount = TopListLoan.Where(t => t.FacilityType == ProposalFacilityType.Personal_Loan).FirstOrDefault().Applications;
                }
                if (TopListLoan.Where(t => t.FacilityType == ProposalFacilityType.Auto_Loan).Count() > 0)
                {
                    data.RMAutoLoan = TopListLoan.Where(t => t.FacilityType == ProposalFacilityType.Auto_Loan).FirstOrDefault().RM;
                    data.RMAutoLoanAmount = TopListLoan.Where(t => t.FacilityType == ProposalFacilityType.Auto_Loan).FirstOrDefault().Amount;
                    data.RMAutoLoanCount = TopListLoan.Where(t => t.FacilityType == ProposalFacilityType.Auto_Loan).FirstOrDefault().Applications;
                }
                if (TopListDeposit.Count() > 0)
                {
                    data.RMLiability1 = TopListDeposit.FirstOrDefault().RM;
                    data.RMLiability1Amount = TopListDeposit.FirstOrDefault().Amount;
                    data.RMLiability1Count = TopListDeposit.FirstOrDefault().Applications;
                }
                if (TopListDeposit.Count() > 1)
                {
                    data.RMLiability2 = TopListDeposit.FirstOrDefault().RM;
                    data.RMLiability2Amount = TopListDeposit.FirstOrDefault().Amount;
                    data.RMLiability2Count = TopListDeposit.FirstOrDefault().Applications;
                }
            }
            else
            {
                #region Call/Lead
                callListToday = GenService.GetAll<Call>().Where(c => c.CreateDate <= dateRange.ToDate && c.CreateDate >= dateRange.FromDate && c.Amount != null && c.Status == EntityStatus.Active).ToList();

                leadListToday = GenService.GetAll<SalesLead>().Where(c => c.CreateDate <= dateRange.ToDate && c.CreateDate >= dateRange.FromDate && c.Amount != null && c.Status == EntityStatus.Active).ToList();

                #endregion Call/Lead

                #region Submited Apps
                var acceptableLogsToday = GenService.GetAll<ApplicationLog>()
                    .Where(l => (l.Application.LoanApplicationId != null || l.Application.DepositApplicationId != null)
                    && l.Status == EntityStatus.Active
                    && l.CreateDate <= dateRange.ToDate
                    && l.CreateDate >= dateRange.FromDate
                    && (((l.ToStage == ApplicationStage.SentToCRM || l.ToStage == ApplicationStage.UnderProcessAtCRM)
                    && l.Application.Product.ProductType == ProductType.Loan)
                    || ((l.ToStage == ApplicationStage.SentToOperations || l.ToStage == ApplicationStage.UnderProcessAtOperations)
                    && l.Application.Product.ProductType == ProductType.Deposit)));
                var allApplicationsToday = (from appLog in acceptableLogsToday
                                            orderby appLog.Id
                                            select appLog).Distinct().ToList();
                if (allApplicationsToday.Any())
                    applicationListToDay.AddRange(allApplicationsToday);

                #endregion Submited Apps

                #region File Approved/DisApproved ToDay Apps.

                var loanApplicationsIds = GenService.GetAll<Application>().Where(a => a.Product.ProductType == ProductType.Loan).Select(a => a.Id);

                var approvedLoanApplicationLogsToday = GenService.GetAll<ApplicationLog>()
                .Where(l => l.Status == EntityStatus.Active &&
                                l.ToStage == ApplicationStage.SentToOperations &&
                                loanApplicationsIds.Contains(l.ApplicationId) &&
                l.CreateDate <= dateRange.ToDate &&
                                l.CreateDate >= dateRange.FromDate).ToList();
                if (approvedLoanApplicationLogsToday.Count > 0)
                    approvedApplicationListToDay.AddRange(approvedLoanApplicationLogsToday);

                var disApporvedLoanApplicationLogsToday = GenService.GetAll<ApplicationLog>()
                .Where(l => l.Status == EntityStatus.Active &&
                            l.ToStage < ApplicationStage.Drafted &&
                            loanApplicationsIds.Contains(l.ApplicationId) &&
                            l.CreateDate <= dateRange.ToDate &&
                            l.CreateDate >= dateRange.FromDate).ToList();

                disApprovedApplicationListToDay.AddRange(disApporvedLoanApplicationLogsToday);


                #endregion File Approved/DisApproved ToDay Apps.

                #region File disbursed / received Today
                var TodaydisbursementMemos = GenService.GetAll<DMDetail>().Where(d => d.DisbursementMemo.ApplicationId != null

                            && d.DisbursementMemo.Application.LoanApplicationId != null
                            && d.DisbursementMemo.Application.Product.FacilityType != null
                            && (d.DisbursementMemo.Application.Product.FacilityType == ProposalFacilityType.Home_Loan
                                || d.DisbursementMemo.Application.Product.FacilityType == ProposalFacilityType.Auto_Loan
                                || d.DisbursementMemo.Application.Product.FacilityType == ProposalFacilityType.Personal_Loan)
                            && d.CreateDate <= dateRange.ToDate
                            && d.CreateDate >= dateRange.FromDate
                            && d.DisbursementMemo.IsApproved != null
                            && d.DisbursementMemo.IsApproved == true
                            && d.DisbursementMemo.IsDisbursed != null
                            && d.DisbursementMemo.IsDisbursed == true
                            && d.Status == EntityStatus.Active).ToList();
                var acceptableDMids = TodaydisbursementMemos.Select(dm => dm.DisbursementMemoId).ToList();
                var disMemotoday = GenService.GetAll<DisbursementMemo>().Where(d => acceptableDMids.Contains(d.Id)).ToList();
                disbursementMemosToday = disMemotoday.ToList();

                var TodayfundReceived = GenService.GetAll<FundConfirmationDetail>().Where(d => d.FundConfirmation.ApplicationId != null

                            && d.FundConfirmation.Application.DepositApplicationId != null
                            && d.FundConfirmation.Application.Product.DepositType != null
                            && (d.FundConfirmation.Application.Product.DepositType == DepositType.Fixed
                                || d.FundConfirmation.Application.Product.DepositType == DepositType.Recurring)
                            && d.CreateDate <= dateRange.ToDate
                            && d.CreateDate >= dateRange.FromDate
                            && d.FundConfirmation.FundReceived != null
                            && d.FundConfirmation.FundReceived == true
                            && d.Status == EntityStatus.Active).ToList();
                var acceptableFCids = TodayfundReceived.Select(dm => dm.FundConfirmationId).ToList();
                var fundRectoday = GenService.GetAll<FundConfirmation>().Where(d => acceptableFCids.Contains(d.Id)).ToList();
                fundReceivedToday = fundRectoday.ToList();

                #endregion File disbursed / received Today
                var TopListLoan = (from disbursement in disbursementMemosToday.Select(d => new
                {
                    ApplicationId = d.ApplicationId,
                    Amount = d.CurrentDisbursementAmount,
                    RM = d.Application.RMEmp.Person.FirstName + " " + d.Application.RMEmp.Person.LastName,
                    FacilityType = d.Proposal.FacilityType
                })
                                   group disbursement by new { disbursement.RM, disbursement.FacilityType } into grouped
                                   select new
                                   {
                                       RM = grouped.Key.RM,
                                       FacilityType = grouped.Key.FacilityType,
                                       Amount = grouped.Sum(s => s.Amount),
                                       Applications = grouped.Count()
                                   }).OrderBy(o => o.FacilityType).ThenBy(o => o.Amount);

                var TopListDeposit = (from funds in fundReceivedToday.Select(d => new
                {
                    Amount = d.Application.DepositApplication.TotalDepositAmount,
                    ApplicationId = d.ApplicationId,
                    RM = d.Application.RMEmp.Person.FirstName + " " + d.Application.RMEmp.Person.LastName
                })
                                      group funds by funds.RM into grouped
                                      select new
                                      {
                                          Amount = grouped.Sum(s => s.Amount),
                                          RM = grouped.Key,
                                          Applications = grouped.Count()
                                      }).OrderBy(d => d.Amount).ThenBy(d => d.Applications);

                if (TopListLoan.Where(t => t.FacilityType == ProposalFacilityType.Home_Loan).Count() > 0)
                {
                    data.RMHomeLoan1 = TopListLoan.Where(t => t.FacilityType == ProposalFacilityType.Home_Loan).FirstOrDefault().RM;
                    data.RMHomeLoan1Amount = TopListLoan.Where(t => t.FacilityType == ProposalFacilityType.Home_Loan).FirstOrDefault().Amount;
                    data.RMHomeLoan1Count = TopListLoan.Where(t => t.FacilityType == ProposalFacilityType.Home_Loan).FirstOrDefault().Applications;
                }
                if (TopListLoan.Where(t => t.FacilityType == ProposalFacilityType.Home_Loan).Count() > 1)
                {
                    data.RMHomeLoan2 = TopListLoan.Where(t => t.FacilityType == ProposalFacilityType.Home_Loan).Skip(1).FirstOrDefault().RM;
                    data.RMHomeLoan2Amount = TopListLoan.Where(t => t.FacilityType == ProposalFacilityType.Home_Loan).Skip(1).FirstOrDefault().Amount;
                    data.RMHomeLoan2Count = TopListLoan.Where(t => t.FacilityType == ProposalFacilityType.Home_Loan).Skip(1).FirstOrDefault().Applications;
                }
                if (TopListLoan.Where(t => t.FacilityType == ProposalFacilityType.Personal_Loan).Count() > 0)
                {
                    data.RMPersonalLoan = TopListLoan.Where(t => t.FacilityType == ProposalFacilityType.Personal_Loan).FirstOrDefault().RM;
                    data.RMPersonalLoanAmount = TopListLoan.Where(t => t.FacilityType == ProposalFacilityType.Personal_Loan).FirstOrDefault().Amount;
                    data.RMPersonalLoanCount = TopListLoan.Where(t => t.FacilityType == ProposalFacilityType.Personal_Loan).FirstOrDefault().Applications;
                }
                if (TopListLoan.Where(t => t.FacilityType == ProposalFacilityType.Auto_Loan).Count() > 0)
                {
                    data.RMAutoLoan = TopListLoan.Where(t => t.FacilityType == ProposalFacilityType.Auto_Loan).FirstOrDefault().RM;
                    data.RMAutoLoanAmount = TopListLoan.Where(t => t.FacilityType == ProposalFacilityType.Auto_Loan).FirstOrDefault().Amount;
                    data.RMAutoLoanCount = TopListLoan.Where(t => t.FacilityType == ProposalFacilityType.Auto_Loan).FirstOrDefault().Applications;
                }
                if (TopListDeposit.Count() > 0)
                {
                    data.RMLiability1 = TopListDeposit.FirstOrDefault().RM;
                    data.RMLiability1Amount = TopListDeposit.FirstOrDefault().Amount;
                    data.RMLiability1Count = TopListDeposit.FirstOrDefault().Applications;
                }
                if (TopListDeposit.Count() > 1)
                {
                    data.RMLiability2 = TopListDeposit.FirstOrDefault().RM;
                    data.RMLiability2Amount = TopListDeposit.FirstOrDefault().Amount;
                    data.RMLiability2Count = TopListDeposit.FirstOrDefault().Applications;
                }

            }


            #endregion


            #region initializations
            data.Disbursed_HomeLoanAmount = 0;
            data.Disbursed_PersonalLoanAmount = 0;
            data.Disbursed_AutoLoanAmount = 0;
            data.RMHomeLoan1 = "";
            data.RMHomeLoan1Amount = 0;
            data.RMHomeLoan1Count = 0;
            data.RMHomeLoan2 = "";
            data.RMHomeLoan2Amount = 0;
            data.RMHomeLoan2Count = 0;
            data.RMPersonalLoan = "";
            data.RMPersonalLoanAmount = 0;
            data.RMPersonalLoanCount = 0;
            data.RMAutoLoan = "";
            data.RMAutoLoanAmount = 0;
            data.RMAutoLoanCount = 0;
            data.RMLiability1 = "";
            data.RMLiability1Amount = 0;
            data.RMLiability1Count = 0;
            data.RMLiability2 = "";
            data.RMLiability2Amount = 0;
            data.RMLiability2Count = 0;
            #endregion
            data.Call_HomeLoan = callListToday.Where(c => c.ProductId != null && c.Product.FacilityType == ProposalFacilityType.Home_Loan).Count();
            data.Call_PersonalLoan = callListToday.Where(c => c.ProductId != null && c.Product.FacilityType == ProposalFacilityType.Personal_Loan).Count();
            data.Call_AutoLoan = callListToday.Where(c => c.ProductId != null && c.Product.FacilityType == ProposalFacilityType.Auto_Loan).Count();
            data.Call_Liability = callListToday.Where(c => c.ProductId != null && c.Product.ProductType == ProductType.Deposit).Count();
            data.Call_Undefined = callListToday.Where(c => c.ProductId == null).Count();

            data.Lead_HomeLoan = leadListToday.Where(l => l.ProductId != null && l.Product.FacilityType == ProposalFacilityType.Home_Loan).Count();
            data.Lead_PersonalLoan = leadListToday.Where(l => l.ProductId != null && l.Product.FacilityType == ProposalFacilityType.Personal_Loan).Count();
            data.Lead_AutoLoan = leadListToday.Where(l => l.ProductId != null && l.Product.FacilityType == ProposalFacilityType.Auto_Loan).Count();
            data.Lead_Liability = leadListToday.Where(l => l.ProductId != null && l.Product.ProductType == ProductType.Deposit).Count();

            data.Application_HomeLoan = applicationListToDay.Where(a => a.Application.Product.FacilityType == ProposalFacilityType.Home_Loan).Count();
            data.Application_PersonalLoan = applicationListToDay.Where(a => a.Application.Product.FacilityType == ProposalFacilityType.Personal_Loan).Count();
            data.Application_AutoLoan = applicationListToDay.Where(a => a.Application.Product.FacilityType == ProposalFacilityType.Auto_Loan).Count();
            data.Application_Liability = applicationListToDay.Where(a => a.Application.Product.ProductType == ProductType.Deposit).Count();

            #region File Approved Apps.
            var approvedApplicationListTodayHome = approvedApplicationListToDay.Where(l => l.Application.LoanApplicationId != null && l.Application.ProductId != null && l.Application.Product.FacilityType != null && l.Application.Product.FacilityType == ProposalFacilityType.Home_Loan).ToList();

            var approvedHomeLoanLogsWithAmountToday = new List<Proposal>();

            if (approvedApplicationListTodayHome.Count > 0)
            {
                //approvedHomeLoanLogsWithAmountToday = approvedHomeLoanLogsWithAmountToday.Where(p => approvedApplicationListTodayHome.Any(a => a.ApplicationId == p.ApplicationId)).ToList();
                approvedHomeLoanLogsWithAmountToday = (from logs in approvedApplicationListTodayHome
                                                       join proposal in GenService.GetAll<Proposal>().Where(p => p.Status == EntityStatus.Active) on
                                                           logs.ApplicationId equals proposal.ApplicationId into proposals
                                                       from p in proposals.DefaultIfEmpty()
                                                       select p).ToList();
            }

            var approvedApplicationListTodayAuto = approvedApplicationListToDay.Where(l => l.Application.LoanApplicationId != null && l.Application.ProductId != null && l.Application.Product.FacilityType != null && l.Application.Product.FacilityType == ProposalFacilityType.Auto_Loan).ToList();

            var approvedAutoLoanLogsWithAmountToday = new List<Proposal>();

            if (approvedApplicationListTodayAuto.Count > 0)
            {
                //approvedAutoLoanLogsWithAmountToday = GenService.GetAll<Proposal>().Where(p => approvedApplicationListTodayAuto.Any(a => a.ApplicationId == p.ApplicationId)).ToList();
                approvedAutoLoanLogsWithAmountToday = (from logs in approvedApplicationListTodayAuto
                                                       join proposal in GenService.GetAll<Proposal>().Where(p => p.Status == EntityStatus.Active) on
                                                           logs.ApplicationId equals proposal.ApplicationId into proposals
                                                       from p in proposals.DefaultIfEmpty()
                                                       select p).ToList();
            }

            var approvedApplicationListTodayPersonal = approvedApplicationListToDay.Where(l => l.Application.LoanApplicationId != null && l.Application.ProductId != null && l.Application.Product.FacilityType != null && l.Application.Product.FacilityType == ProposalFacilityType.Personal_Loan).ToList();
            var approvedPersonalLoanLogsWithAmountToday = new List<Proposal>();

            if (approvedApplicationListTodayPersonal.Count > 0)
            {
                //approvedPersonalLoanLogsWithAmountToday = GenService.GetAll<Proposal>().Where(p => approvedApplicationListTodayPersonal.Any(a => a.ApplicationId == p.ApplicationId)).ToList();
                approvedPersonalLoanLogsWithAmountToday = (from logs in approvedApplicationListTodayPersonal
                                                           join proposal in GenService.GetAll<Proposal>().Where(p => p.Status == EntityStatus.Active) on
                                                               logs.ApplicationId equals proposal.ApplicationId into proposals
                                                           from p in proposals.DefaultIfEmpty()
                                                           select p).ToList();
            }

            var disApprovedApplicationListTodayHome = disApprovedApplicationListToDay.Where(l => l.Application.LoanApplicationId != null && l.Application.ProductId != null && l.Application.Product.FacilityType != null && l.Application.Product.FacilityType == ProposalFacilityType.Home_Loan).ToList();

            var disApprovedHomeLoanLogsWithAmountToday = new List<Proposal>();

            if (disApprovedApplicationListTodayHome.Count > 0)
            {
                //disApprovedHomeLoanLogsWithAmountToday = GenService.GetAll<Proposal>().Where(p => disApprovedApplicationListTodayHome.Any(a => a.ApplicationId == p.ApplicationId)).ToList();
                disApprovedHomeLoanLogsWithAmountToday = (from logs in disApprovedApplicationListTodayHome
                                                          join proposal in GenService.GetAll<Proposal>().Where(p => p.Status == EntityStatus.Active) on
                                                              logs.ApplicationId equals proposal.ApplicationId into proposals
                                                          from p in proposals.DefaultIfEmpty()
                                                          select p).ToList();
            }


            var disApprovedApplicationListTodayAuto = disApprovedApplicationListToDay.Where(l => l.Application.LoanApplicationId != null && l.Application.ProductId != null && l.Application.Product.FacilityType != null && l.Application.Product.FacilityType == ProposalFacilityType.Auto_Loan).ToList();

            var disApprovedAutoLoanLogsWithAmountToday = new List<Proposal>();

            if (disApprovedApplicationListTodayAuto.Count > 0)
            {
                //disApprovedAutoLoanLogsWithAmountToday = GenService.GetAll<Proposal>().Where(p => disApprovedApplicationListTodayAuto.Any(a => a.ApplicationId == p.ApplicationId)).ToList();
                disApprovedAutoLoanLogsWithAmountToday = (from logs in disApprovedApplicationListTodayAuto
                                                          join proposal in GenService.GetAll<Proposal>().Where(p => p.Status == EntityStatus.Active) on
                                                              logs.ApplicationId equals proposal.ApplicationId into proposals
                                                          from p in proposals.DefaultIfEmpty()
                                                          select p).ToList();
            }


            var disApprovedApplicationListTodayPersonal = disApprovedApplicationListToDay.Where(l => l.Application.LoanApplicationId != null && l.Application.ProductId != null && l.Application.Product.FacilityType != null && l.Application.Product.FacilityType == ProposalFacilityType.Personal_Loan).ToList();
            var disApprovedPersonalLoanLogsWithAmountToday = new List<Proposal>();

            if (disApprovedApplicationListTodayPersonal.Count > 0)
            {
                //disApprovedPersonalLoanLogsWithAmountToday = GenService.GetAll<Proposal>().Where(p => disApprovedApplicationListTodayPersonal.Any(a => a.ApplicationId == p.ApplicationId)).ToList();
                disApprovedPersonalLoanLogsWithAmountToday = (from logs in disApprovedApplicationListTodayPersonal
                                                              join proposal in GenService.GetAll<Proposal>().Where(p => p.Status == EntityStatus.Active) on
                                                                  logs.ApplicationId equals proposal.ApplicationId into proposals
                                                              from p in proposals.DefaultIfEmpty()
                                                              select p).ToList();
            }


            #endregion File Approved Apps.

            data.Approved_HomeLoan = approvedHomeLoanLogsWithAmountToday.Where(a => a.FacilityType == ProposalFacilityType.Home_Loan).Count();
            data.Approved_PersonalLoan = approvedPersonalLoanLogsWithAmountToday.Where(a => a.FacilityType == ProposalFacilityType.Personal_Loan).Count();
            data.Approved_AutoLoan = approvedAutoLoanLogsWithAmountToday.Where(a => a.FacilityType == ProposalFacilityType.Auto_Loan).Count();
            data.Approved_HomeLoanAmount = (approvedHomeLoanLogsWithAmountToday.Where(a => a.FacilityType == ProposalFacilityType.Home_Loan).Sum(a => a.RecomendedLoanAmountFromIPDC != null ? a.RecomendedLoanAmountFromIPDC : 0) ?? 0);
            data.Approved_PersonalLoanAmount = (approvedPersonalLoanLogsWithAmountToday.Where(a => a.FacilityType == ProposalFacilityType.Home_Loan).Sum(a => a.RecomendedLoanAmountFromIPDC != null ? a.RecomendedLoanAmountFromIPDC : 0) ?? 0);
            data.Approved_AutoLoanAmount = (approvedAutoLoanLogsWithAmountToday.Where(a => a.FacilityType == ProposalFacilityType.Home_Loan).Sum(a => a.RecomendedLoanAmountFromIPDC != null ? a.RecomendedLoanAmountFromIPDC : 0) ?? 0);

            data.Disapproved_HomeLoan = disApprovedHomeLoanLogsWithAmountToday.Where(a => a.FacilityType == ProposalFacilityType.Home_Loan).Count();
            data.Disapproved_PersonalLoan = disApprovedPersonalLoanLogsWithAmountToday.Where(a => a.FacilityType == ProposalFacilityType.Personal_Loan).Count();
            data.Disapproved_AutoLoan = disApprovedAutoLoanLogsWithAmountToday.Where(a => a.FacilityType == ProposalFacilityType.Auto_Loan).Count();
            data.Disapproved_HomeLoanAmount = (disApprovedHomeLoanLogsWithAmountToday.Where(a => a.FacilityType == ProposalFacilityType.Home_Loan).Sum(a => a.RecomendedLoanAmountFromIPDC != null ? a.RecomendedLoanAmountFromIPDC : 0) ?? 0);
            data.Disapproved_PersonalLoanAmount = (disApprovedPersonalLoanLogsWithAmountToday.Where(a => a.FacilityType == ProposalFacilityType.Personal_Loan).Sum(a => a.RecomendedLoanAmountFromIPDC != null ? a.RecomendedLoanAmountFromIPDC : 0) ?? 0);
            data.Disapproved_AutoLoanAmount = (disApprovedAutoLoanLogsWithAmountToday.Where(a => a.FacilityType == ProposalFacilityType.Auto_Loan).Sum(a => a.RecomendedLoanAmountFromIPDC != null ? a.RecomendedLoanAmountFromIPDC : 0) ?? 0);

            if (disbursementMemosToday.Where(d => d.Proposal.FacilityType == ProposalFacilityType.Home_Loan).Count() > 0)
                data.Disbursed_HomeLoanAmount = disbursementMemosToday.Where(d => d.Proposal.FacilityType == ProposalFacilityType.Home_Loan).Sum(d => d.CurrentDisbursementAmount);
            if (disbursementMemosToday.Where(d => d.Proposal.FacilityType == ProposalFacilityType.Personal_Loan).Count() > 0)
                data.Disbursed_PersonalLoanAmount = disbursementMemosToday.Where(d => d.Proposal.FacilityType == ProposalFacilityType.Personal_Loan).Sum(d => d.CurrentDisbursementAmount);
            if (disbursementMemosToday.Where(d => d.Proposal.FacilityType == ProposalFacilityType.Auto_Loan).Count() > 0)
                data.Disbursed_AutoLoanAmount = disbursementMemosToday.Where(d => d.Proposal.FacilityType == ProposalFacilityType.Auto_Loan).Sum(d => d.CurrentDisbursementAmount);

            data.ReceivedDepositAmount = fundReceivedToday.Sum(f => f.Application.DepositApplication.TotalDepositAmount);



            return data;
        }

        public object GetNSMDashboardHighlights(long UserId, TimeLine? timeLine, long? branchId)
        {
            DateRangeDto dateRange;
            if (timeLine == null)
                dateRange = new DateRangeDto();
            else
                dateRange = new DateRangeDto((TimeLine)timeLine);

            #region data population

            long employeeId = _user.GetEmployeeIdByUserId(UserId);
            long offDegSettingId = _employee.GetOfficeDesignationIdOfEmployee(employeeId);
            List<EmployeeDto> subordinateEmpList = new List<EmployeeDto>();
            if (branchId > 0)
                subordinateEmpList = _office.GetEmployeesByOfficeId((long)branchId);
            var subordinateEmpIdList = subordinateEmpList.Select(s => s.Id).ToList();
            var subordinateUserList = new List<long>();
            foreach (var sub in subordinateEmpList)
            {
                subordinateUserList.Add((long)_user.GetUserByEmployeeId((long)sub.Id).Id);
            }

            List<long> existingthanaIds = new List<long>();
            var thanaOfEmployee = _employee.GetThanaOfEmployee(employeeId);
            if (thanaOfEmployee.Any())
            {
                existingthanaIds = thanaOfEmployee.Select(d => d.Id).ToList();
            }
            var allApplications = GenService.GetAll<Application>().Where(a => subordinateEmpIdList.Contains(a.RMId));

            #region call data population
            IQueryable<Call> allCalls = GenService.GetAll<Call>().Where(c => c.CreateDate <= dateRange.ToDate && c.CreateDate >= dateRange.FromDate && c.Status == EntityStatus.Active);

            IQueryable<Call> callQueryableAreaWise = allCalls
                .Where(s => s.Status == EntityStatus.Active &&
                            s.CallStatus == CallStatus.Unfinished &&
                            s.CallType == CallType.Auto_assign &&
                            existingthanaIds.Contains((long)s.CustomerAddress.ThanaId));
            IQueryable<Call> callQueryableAsReferred = allCalls
                .Where(c => c.Status == EntityStatus.Active &&
                            c.CallStatus == CallStatus.Unfinished &&
                            c.ReferredTo == offDegSettingId);
            IQueryable<Call> rmSelfGeneratedCalls = allCalls
                .Where(c => c.Status == EntityStatus.Active &&
                            c.CallType == CallType.Self &&
                            subordinateUserList.Contains((long)c.CreatedBy));
            var allCallsUnited = callQueryableAreaWise.Union(callQueryableAsReferred).Union(rmSelfGeneratedCalls).Distinct();
            #endregion

            #region lead data population
            IQueryable<SalesLead> allLeads = GenService.GetAll<SalesLead>()
                .Where(s => s.Status == EntityStatus.Active && s.CreateDate <= dateRange.ToDate && s.CreateDate >= dateRange.FromDate);
            var leadIds = GenService.GetAll<SalesLeadAssignment>()
                .Where(s => subordinateEmpIdList.Contains(s.AssignedToEmpId) &&
                s.Status == EntityStatus.Active).Select(s => s.SalesLeadId).ToList();
            var notAcceptableLeadIds = allApplications
                .Where(s => s.ApplicationStage < ApplicationStage.Drafted && s.ApplicationStage > ApplicationStage.SentToBM)
                .Select(s => s.SalesLeadId);
            var assignedLeads = allLeads
                .Where(s => leadIds.Contains(s.Id) &&
                !notAcceptableLeadIds.Contains(s.Id) &&
                s.Status == EntityStatus.Active &&
                (s.LeadStatus == LeadStatus.Drafted || s.LeadStatus == LeadStatus.Prospective));
            #endregion

            #region application data population
            var submittedApplications = allApplications
                .Where(a => (((a.ApplicationStage == ApplicationStage.SentToCRM || a.ApplicationStage == ApplicationStage.UnderProcessAtCRM) && a.Product.ProductType == ProductType.Loan) ||
                            ((a.ApplicationStage == ApplicationStage.SentToOperations || a.ApplicationStage == ApplicationStage.UnderProcessAtOperations) && a.Product.ProductType == ProductType.Deposit)));
            var submittedApplicationIds = submittedApplications.Select(s => s.Id).ToList();
            var acceptableLogs = GenService.GetAll<ApplicationLog>()
                .Where(l => l.Status == EntityStatus.Active &&
                submittedApplicationIds.Contains(l.ApplicationId) &&
                l.CreateDate <= dateRange.ToDate &&
                l.CreateDate >= dateRange.FromDate &&
                (((l.ToStage == ApplicationStage.SentToCRM || l.ToStage == ApplicationStage.UnderProcessAtCRM) &&
                 l.Application.Product.ProductType == ProductType.Loan) ||
                ((l.ToStage == ApplicationStage.SentToOperations || l.ToStage == ApplicationStage.UnderProcessAtOperations) &&
                 l.Application.Product.ProductType == ProductType.Deposit)));
            var submittedApplicationsWithLog = (from appLog in acceptableLogs
                                                orderby appLog.Id
                                                select new
                                                {
                                                    ApplicationId = appLog.ApplicationId,
                                                    Product = appLog.Application.Product
                                                }).Distinct();
            #endregion

            #region approved/disapproved
            //var loanApplications = allApplications.Where(a => a.Product.ProductType == ProductType.Loan).ToList();
            var loanApplicationsIds = allApplications.Where(a => a.Product.ProductType == ProductType.Loan).Select(a => a.Id);
            var approvedLoanApplicationLogs = GenService.GetAll<ApplicationLog>()
                .Where(l => l.Status == EntityStatus.Active &&
                            l.ToStage == ApplicationStage.SentToOperations &&
                            loanApplicationsIds.Contains(l.ApplicationId) &&
                            l.CreateDate <= dateRange.ToDate &&
                            l.CreateDate >= dateRange.FromDate)
                .OrderBy(l => l.Id)
                .Select(l => new { ApplicationId = l.ApplicationId, Product = l.Application.Product, Amount = l.Application.LoanApplication.LoanAmountApplied })
                .Distinct();
            var approvedLoanApplicationLogsWithAmount = (from logs in approvedLoanApplicationLogs
                                                         join proposal in GenService.GetAll<Proposal>().Where(p => p.Status == EntityStatus.Active) on logs.ApplicationId equals proposal.ApplicationId into proposals
                                                         from p in proposals.DefaultIfEmpty()
                                                         select new
                                                         {
                                                             ApplicationId = logs.ApplicationId,
                                                             Product = logs.Product,
                                                             Amount = p != null && p.RecomendedLoanAmountFromIPDC != null ? p.RecomendedLoanAmountFromIPDC : logs.Amount
                                                         }).Distinct();

            var disapporvedLoanApplicationLogs = GenService.GetAll<ApplicationLog>()
                .Where(l => l.Status == EntityStatus.Active &&
                            l.ToStage < ApplicationStage.Drafted &&
                            loanApplicationsIds.Contains(l.ApplicationId) &&
                            l.CreateDate <= dateRange.ToDate &&
                            l.CreateDate >= dateRange.FromDate)
                .OrderBy(l => l.Id)
                .Select(l => new { ApplicationId = l.ApplicationId, Product = l.Application.Product, Amount = l.Application.LoanApplication.LoanAmountApplied })
                .Distinct();
            var disapprovedLoanApplicationLogsWithAmount = (from logs in disapporvedLoanApplicationLogs
                                                            join proposal in GenService.GetAll<Proposal>().Where(p => p.Status == EntityStatus.Active) on logs.ApplicationId equals proposal.ApplicationId into proposals
                                                            from p in proposals.DefaultIfEmpty()
                                                            select new
                                                            {
                                                                ApplicationId = logs.ApplicationId,
                                                                Product = logs.Product,
                                                                Amount = p != null && p.RecomendedLoanAmountFromIPDC != null ? p.RecomendedLoanAmountFromIPDC : logs.Amount
                                                            }).Distinct();
            #endregion

            #region Amount disbursed / received
            var disbursementMemos = GenService.GetAll<DisbursementMemo>().Where(d => d.ApplicationId != null &&
                                                                                     submittedApplicationIds.Contains((long)d.ApplicationId) &&
                                                                                     d.Status == EntityStatus.Active &&
                                                                                     d.IsApproved != null &&
                                                                                     d.IsApproved == true &&
                                                                                     d.IsDisbursed != null &&
                                                                                     d.IsDisbursed == true &&
                                                                                     d.DisbursementDetails.FirstOrDefault().CreateDate <= dateRange.ToDate &&
                                                                                     d.DisbursementDetails.FirstOrDefault().CreateDate >= dateRange.FromDate);
            var fundReceived = GenService.GetAll<FundConfirmationDetail>().Where(f => submittedApplicationIds.Contains((long)f.FundConfirmation.ApplicationId) &&
                                                                                      f.FundConfirmation.Status == EntityStatus.Active &&
                                                                                      f.Status == EntityStatus.Active &&
                                                                                      f.FundConfirmation.FundReceived != null &&
                                                                                      f.FundConfirmation.FundReceived == true &&
                                                                                      f.CreditDate != null &&
                                                                                      f.CreditDate <= dateRange.ToDate &&
                                                                                      f.CreditDate >= dateRange.FromDate);
            var TopListLoan = (from disbursement in disbursementMemos.Select(d => new
            {
                ApplicationId = d.ApplicationId,
                Amount = d.CurrentDisbursementAmount,
                RM = d.Application.RMEmp.Person.FirstName + " " + d.Application.RMEmp.Person.LastName,
                FacilityType = d.Proposal.FacilityType
            })
                               group disbursement by new { disbursement.RM, disbursement.FacilityType } into grouped
                               select new
                               {
                                   RM = grouped.Key.RM,
                                   FacilityType = grouped.Key.FacilityType,
                                   Amount = grouped.Sum(s => s.Amount),
                                   Applications = grouped.Count()
                               }).OrderBy(o => o.FacilityType).ThenBy(o => o.Amount);
            var TopListDeposit = (from funds in fundReceived.Select(d => new
            {
                Amount = d.Amount,
                ApplicationId = d.FundConfirmation.ApplicationId,
                RM = d.FundConfirmation.Application.RMEmp.Person.FirstName + " " + d.FundConfirmation.Application.RMEmp.Person.LastName
            })
                                  group funds by funds.RM into grouped
                                  select new
                                  {
                                      Amount = grouped.Sum(s => s.Amount) ?? 0,
                                      RM = grouped.Key,
                                      Applications = grouped.Count()
                                  }).OrderBy(d => d.Amount).ThenBy(d => d.Applications);
            #endregion

            #endregion

            var data = new DashBoardHighlightsDto();
            #region initializations
            data.Disbursed_HomeLoanAmount = 0;
            data.Disbursed_PersonalLoanAmount = 0;
            data.Disbursed_AutoLoanAmount = 0;
            data.RMHomeLoan1 = "";
            data.RMHomeLoan1Amount = 0;
            data.RMHomeLoan1Count = 0;
            data.RMHomeLoan2 = "";
            data.RMHomeLoan2Amount = 0;
            data.RMHomeLoan2Count = 0;
            data.RMPersonalLoan = "";
            data.RMPersonalLoanAmount = 0;
            data.RMPersonalLoanCount = 0;
            data.RMAutoLoan = "";
            data.RMAutoLoanAmount = 0;
            data.RMAutoLoanCount = 0;
            data.RMLiability1 = "";
            data.RMLiability1Amount = 0;
            data.RMLiability1Count = 0;
            data.RMLiability2 = "";
            data.RMLiability2Amount = 0;
            data.RMLiability2Count = 0;
            #endregion
            data.Call_HomeLoan = allCallsUnited.Where(c => c.ProductId != null && c.Product.FacilityType == ProposalFacilityType.Home_Loan).Count();
            data.Call_PersonalLoan = allCallsUnited.Where(c => c.ProductId != null && c.Product.FacilityType == ProposalFacilityType.Personal_Loan).Count();
            data.Call_AutoLoan = allCallsUnited.Where(c => c.ProductId != null && c.Product.FacilityType == ProposalFacilityType.Auto_Loan).Count();
            data.Call_Liability = allCallsUnited.Where(c => c.ProductId != null && c.Product.ProductType == ProductType.Deposit).Count();
            data.Call_Undefined = allCallsUnited.Where(c => c.ProductId == null).Count();

            data.Lead_HomeLoan = assignedLeads.Where(l => l.ProductId != null && l.Product.FacilityType == ProposalFacilityType.Home_Loan).Count();
            data.Lead_PersonalLoan = assignedLeads.Where(l => l.ProductId != null && l.Product.FacilityType == ProposalFacilityType.Personal_Loan).Count();
            data.Lead_AutoLoan = assignedLeads.Where(l => l.ProductId != null && l.Product.FacilityType == ProposalFacilityType.Auto_Loan).Count();
            data.Lead_Liability = assignedLeads.Where(l => l.ProductId != null && l.Product.ProductType == ProductType.Deposit).Count();

            data.Application_HomeLoan = submittedApplicationsWithLog.Where(a => a.Product.FacilityType == ProposalFacilityType.Home_Loan).Count();
            data.Application_PersonalLoan = submittedApplicationsWithLog.Where(a => a.Product.FacilityType == ProposalFacilityType.Personal_Loan).Count();
            data.Application_AutoLoan = submittedApplicationsWithLog.Where(a => a.Product.FacilityType == ProposalFacilityType.Auto_Loan).Count();
            data.Application_Liability = submittedApplicationsWithLog.Where(a => a.Product.ProductType == ProductType.Deposit).Count();

            data.Approved_HomeLoan = approvedLoanApplicationLogsWithAmount.Where(a => a.Product.FacilityType == ProposalFacilityType.Home_Loan).Count();
            data.Approved_PersonalLoan = approvedLoanApplicationLogsWithAmount.Where(a => a.Product.FacilityType == ProposalFacilityType.Personal_Loan).Count();
            data.Approved_AutoLoan = approvedLoanApplicationLogsWithAmount.Where(a => a.Product.FacilityType == ProposalFacilityType.Auto_Loan).Count();
            data.Approved_HomeLoanAmount = (approvedLoanApplicationLogsWithAmount.Where(a => a.Product.FacilityType == ProposalFacilityType.Home_Loan).Sum(a => a.Amount != null ? a.Amount : 0) ?? 0);
            data.Approved_PersonalLoanAmount = (approvedLoanApplicationLogsWithAmount.Where(a => a.Product.FacilityType == ProposalFacilityType.Personal_Loan).Sum(a => a.Amount != null ? a.Amount : 0) ?? 0);
            data.Approved_AutoLoanAmount = (approvedLoanApplicationLogsWithAmount.Where(a => a.Product.FacilityType == ProposalFacilityType.Auto_Loan).Sum(a => a.Amount != null ? a.Amount : 0) ?? 0);
            data.Disapproved_HomeLoan = disapprovedLoanApplicationLogsWithAmount.Where(a => a.Product.FacilityType == ProposalFacilityType.Home_Loan).Count();
            data.Disapproved_PersonalLoan = disapprovedLoanApplicationLogsWithAmount.Where(a => a.Product.FacilityType == ProposalFacilityType.Personal_Loan).Count();
            data.Disapproved_AutoLoan = disapprovedLoanApplicationLogsWithAmount.Where(a => a.Product.FacilityType == ProposalFacilityType.Auto_Loan).Count();
            data.Disapproved_HomeLoanAmount = (disapprovedLoanApplicationLogsWithAmount.Where(a => a.Product.FacilityType == ProposalFacilityType.Home_Loan).Sum(a => a.Amount != null ? a.Amount : 0) ?? 0);
            data.Disapproved_PersonalLoanAmount = (disapprovedLoanApplicationLogsWithAmount.Where(a => a.Product.FacilityType == ProposalFacilityType.Personal_Loan).Sum(a => a.Amount != null ? a.Amount : 0) ?? 0);
            data.Disapproved_AutoLoanAmount = (disapprovedLoanApplicationLogsWithAmount.Where(a => a.Product.FacilityType == ProposalFacilityType.Auto_Loan).Sum(a => a.Amount != null ? a.Amount : 0) ?? 0);


            if (disbursementMemos.Where(d => d.Proposal.FacilityType == ProposalFacilityType.Home_Loan).Count() > 0)
                data.Disbursed_HomeLoanAmount = disbursementMemos.Where(d => d.Proposal.FacilityType == ProposalFacilityType.Home_Loan).Sum(d => d.CurrentDisbursementAmount);
            if (disbursementMemos.Where(d => d.Proposal.FacilityType == ProposalFacilityType.Personal_Loan).Count() > 0)
                data.Disbursed_PersonalLoanAmount = disbursementMemos.Where(d => d.Proposal.FacilityType == ProposalFacilityType.Personal_Loan).Sum(d => d.CurrentDisbursementAmount);
            if (disbursementMemos.Where(d => d.Proposal.FacilityType == ProposalFacilityType.Auto_Loan).Count() > 0)
                data.Disbursed_AutoLoanAmount = disbursementMemos.Where(d => d.Proposal.FacilityType == ProposalFacilityType.Auto_Loan).Sum(d => d.CurrentDisbursementAmount);

            data.ReceivedDepositAmount = (fundReceived.Sum(f => f.Amount) ?? 0);

            if (TopListLoan.Where(t => t.FacilityType == ProposalFacilityType.Home_Loan).Count() > 0)
            {
                data.RMHomeLoan1 = TopListLoan.Where(t => t.FacilityType == ProposalFacilityType.Home_Loan).FirstOrDefault().RM;
                data.RMHomeLoan1Amount = TopListLoan.Where(t => t.FacilityType == ProposalFacilityType.Home_Loan).FirstOrDefault().Amount;
                data.RMHomeLoan1Count = TopListLoan.Where(t => t.FacilityType == ProposalFacilityType.Home_Loan).FirstOrDefault().Applications;
            }
            if (TopListLoan.Where(t => t.FacilityType == ProposalFacilityType.Home_Loan).Count() > 1)
            {
                data.RMHomeLoan2 = TopListLoan.Where(t => t.FacilityType == ProposalFacilityType.Home_Loan).Skip(1).FirstOrDefault().RM;
                data.RMHomeLoan2Amount = TopListLoan.Where(t => t.FacilityType == ProposalFacilityType.Home_Loan).Skip(1).FirstOrDefault().Amount;
                data.RMHomeLoan2Count = TopListLoan.Where(t => t.FacilityType == ProposalFacilityType.Home_Loan).Skip(1).FirstOrDefault().Applications;
            }
            if (TopListLoan.Where(t => t.FacilityType == ProposalFacilityType.Personal_Loan).Count() > 0)
            {
                data.RMPersonalLoan = TopListLoan.Where(t => t.FacilityType == ProposalFacilityType.Personal_Loan).FirstOrDefault().RM;
                data.RMPersonalLoanAmount = TopListLoan.Where(t => t.FacilityType == ProposalFacilityType.Personal_Loan).FirstOrDefault().Amount;
                data.RMPersonalLoanCount = TopListLoan.Where(t => t.FacilityType == ProposalFacilityType.Personal_Loan).FirstOrDefault().Applications;
            }
            if (TopListLoan.Where(t => t.FacilityType == ProposalFacilityType.Auto_Loan).Count() > 0)
            {
                data.RMAutoLoan = TopListLoan.Where(t => t.FacilityType == ProposalFacilityType.Auto_Loan).FirstOrDefault().RM;
                data.RMAutoLoanAmount = TopListLoan.Where(t => t.FacilityType == ProposalFacilityType.Auto_Loan).FirstOrDefault().Amount;
                data.RMAutoLoanCount = TopListLoan.Where(t => t.FacilityType == ProposalFacilityType.Auto_Loan).FirstOrDefault().Applications;
            }
            if (TopListDeposit.Count() > 0)
            {
                data.RMLiability1 = TopListDeposit.FirstOrDefault().RM;
                data.RMLiability1Amount = TopListDeposit.FirstOrDefault().Amount;
                data.RMLiability1Count = TopListDeposit.FirstOrDefault().Applications;
            }
            if (TopListDeposit.Count() > 1)
            {
                data.RMLiability2 = TopListDeposit.FirstOrDefault().RM;
                data.RMLiability2Amount = TopListDeposit.FirstOrDefault().Amount;
                data.RMLiability2Count = TopListDeposit.FirstOrDefault().Applications;
            }

            return data;
        }

        public object GetMDDashboardCallHightlidhts(TimeLine? timeLine, Criteria? criteria, List<long?> branchIds)
        {
            //long UserId,
            var data = new DashBoardHighlightsDto();
            DateRangeDto dateRange;
            if (timeLine == null)
                timeLine = TimeLine.Today;
            dateRange = new DateRangeDto((TimeLine)timeLine);
            TimeLine seconderyTimeLine;
            switch (timeLine)
            {
                case TimeLine.MTD:
                    seconderyTimeLine = TimeLine.LMTD;
                    break;
                case TimeLine.YTD:
                    seconderyTimeLine = TimeLine.LYTD;
                    break;
                case TimeLine.QTD:
                    seconderyTimeLine = TimeLine.LYQTD;
                    break;
                default:
                    seconderyTimeLine = TimeLine.Yesterday;
                    break;
            }
            var secondaryDateRange = new DateRangeDto(seconderyTimeLine);


            var callListToday = new List<Call>();
            var callListLastDay = new List<Call>();

            var leadListToday = new List<SalesLead>();
            var leadListLastDay = new List<SalesLead>();

            var applicationListToDay = new List<ApplicationLog>();
            var applicationListLastDay = new List<ApplicationLog>();

            var approvedApplicationListToDay = new List<ApplicationLog>();
            var approvedApplicationListLastDay = new List<ApplicationLog>();

            var disApprovedApplicationListToDay = new List<ApplicationLog>();
            var disApprovedApplicationListLastDay = new List<ApplicationLog>();
            var disbursementMemosToday = new List<DisbursementMemo>();
            var disbursementMemosLastday = new List<DisbursementMemo>();
            var fundReceivedToday = new List<FundConfirmation>();
            var fundReceivedLastday = new List<FundConfirmation>();

            var disbursedMemodetails = new List<DMDetail>();
            disbursedMemodetails = GenService.GetAll<DMDetail>().Where(dm => dm.Status == EntityStatus.Active).ToList();
            var fundConfirmdetails = new List<FundConfirmationDetail>();
            fundConfirmdetails = GenService.GetAll<FundConfirmationDetail>().Where(fc => fc.Status == EntityStatus.Active).ToList();


            var disMemo = new List<DisbursementMemo>();

            var fundRec = new List<FundConfirmation>();

            //long userId = 0;

            if (branchIds != null && branchIds.Count > 0)
            {
                //var employeeList = _office.GetEmployeesByOfficeId((long)branchId);

                List<EmployeeDto> subordinateEmpList = new List<EmployeeDto>();
                foreach (var branchId in branchIds)
                {
                    if (branchId > 0)
                        subordinateEmpList = _office.GetEmployeesByOfficeId((long)branchId);
                }
                var subordinateEmpIdList = subordinateEmpList.Select(s => s.Id).ToList();
                var subordinateUserList = new List<long>();
                foreach (var sub in subordinateEmpList)
                {
                    subordinateUserList.Add((long)_user.GetUserByEmployeeId((long)sub.Id).Id);
                }

                var allApplications = GenService.GetAll<Application>().Where(a => subordinateEmpIdList.Contains(a.RMId));

                #region Call

                IQueryable<Call> allCallsToday = GenService.GetAll<Call>().Where(c => c.CreateDate <= dateRange.ToDate && c.CreateDate >= dateRange.FromDate && c.Amount != null && subordinateUserList.Contains((long)c.CreatedBy) && c.Status == EntityStatus.Active);
                if (allCallsToday.Any())
                    callListToday.AddRange(allCallsToday);

                IQueryable<Call> allCallsLastday = GenService.GetAll<Call>().Where(c => c.CreateDate <= secondaryDateRange.ToDate && c.CreateDate >= secondaryDateRange.FromDate && c.Amount != null && subordinateUserList.Contains((long)c.CreatedBy) && c.Status == EntityStatus.Active);
                if (allCallsLastday.Any())
                    callListLastDay.AddRange(allCallsLastday);
                #endregion Call

                #region Lead

                IQueryable<SalesLead> allLeadsToday = GenService.GetAll<SalesLead>().Where(c => c.CreateDate <= dateRange.ToDate && c.CreateDate >= dateRange.FromDate && c.Amount != null && subordinateEmpIdList.Contains(c.CreatedBy) && c.Status == EntityStatus.Active);
                if (allLeadsToday.Count() > 0)
                    leadListToday.AddRange(allLeadsToday);

                IQueryable<SalesLead> allLeadsLastday = GenService.GetAll<SalesLead>().Where(c => c.CreateDate <= secondaryDateRange.ToDate && c.CreateDate >= secondaryDateRange.FromDate && c.Amount != null && subordinateEmpIdList.Contains(c.CreatedBy) && c.Status == EntityStatus.Active);
                if (allLeadsLastday.Count() > 0)
                    leadListLastDay.AddRange(allLeadsLastday);

                #endregion Lead

                #region File Submited Apps.

                //&& submittedApplicationIds.Contains(l.ApplicationId) &&
                var acceptableLogsToday = GenService.GetAll<ApplicationLog>()
                    .Where(l => allApplications.Any(a => a.Id == l.ApplicationId)
                    && (l.Application.LoanApplicationId != null || l.Application.DepositApplicationId != null)
                    && l.Status == EntityStatus.Active
                    && l.CreateDate <= dateRange.ToDate
                    && l.CreateDate >= dateRange.FromDate
                    && (((l.ToStage == ApplicationStage.SentToCRM || l.ToStage == ApplicationStage.UnderProcessAtCRM)
                    && l.Application.Product.ProductType == ProductType.Loan)
                    || ((l.ToStage == ApplicationStage.SentToOperations || l.ToStage == ApplicationStage.UnderProcessAtOperations)
                    && l.Application.Product.ProductType == ProductType.Deposit)));
                var allApplicationsToday = (from appLog in acceptableLogsToday
                                            orderby appLog.Id
                                            select appLog).Distinct().ToList();
                if (allApplicationsToday.Any())
                    applicationListToDay.AddRange(allApplicationsToday);
                //if (acceptableLogsToday.Any())
                //    applicationListToDay.AddRange(acceptableLogsToday);


                var acceptableLogsLastday = GenService.GetAll<ApplicationLog>()
                    .Where(l => allApplications.Any(a => a.Id == l.ApplicationId)
                    && (l.Application.LoanApplicationId != null || l.Application.DepositApplicationId != null)
                    && l.Status == EntityStatus.Active
                    && l.CreateDate <= secondaryDateRange.ToDate
                    && l.CreateDate >= secondaryDateRange.FromDate
                    && (((l.ToStage == ApplicationStage.SentToCRM || l.ToStage == ApplicationStage.UnderProcessAtCRM)
                    && l.Application.Product.ProductType == ProductType.Loan)
                    || ((l.ToStage == ApplicationStage.SentToOperations || l.ToStage == ApplicationStage.UnderProcessAtOperations)
                    && l.Application.Product.ProductType == ProductType.Deposit)));
                var allApplicationsLastday = (from appLog in acceptableLogsLastday
                                              orderby appLog.Id
                                              select appLog).Distinct().ToList();

                if (allApplicationsLastday.Any())
                    applicationListLastDay.AddRange(allApplicationsLastday);

                #endregion File Submited Apps.

                #region File Approved/DisApproved ToDay Apps.

                var loanApplicationsIds = allApplications.Where(a => a.Product.ProductType == ProductType.Loan && a.LoanApplication.LoanAmountApplied != null).Select(a => a.Id);

                var approvedLoanApplicationLogsToday = GenService.GetAll<ApplicationLog>()
                    .Where(l => l.Status == EntityStatus.Active &&
                                l.ToStage == ApplicationStage.SentToOperations &&
                                loanApplicationsIds.Contains(l.ApplicationId) &&
                                l.CreateDate <= dateRange.ToDate &&
                                l.CreateDate >= dateRange.FromDate).ToList();
                if (approvedLoanApplicationLogsToday.Count > 0)
                    approvedApplicationListToDay.AddRange(approvedLoanApplicationLogsToday);

                var disApporvedLoanApplicationLogsToday = GenService.GetAll<ApplicationLog>()
                .Where(l => l.Status == EntityStatus.Active &&
                            l.ToStage < ApplicationStage.Drafted &&
                            loanApplicationsIds.Contains(l.ApplicationId) &&
                            l.CreateDate <= dateRange.ToDate &&
                            l.CreateDate >= dateRange.FromDate).ToList();

                disApprovedApplicationListToDay.AddRange(disApporvedLoanApplicationLogsToday);


                #endregion File Approved/DisApproved ToDay Apps.

                #region File Approved/DisApproved LastDay Apps.

                var approvedLoanApplicationLogsLastday = GenService.GetAll<ApplicationLog>()
                        .Where(l => l.Status == EntityStatus.Active &&
                                    l.ToStage == ApplicationStage.SentToOperations &&
                                    loanApplicationsIds.Contains(l.ApplicationId) &&
                                    l.CreateDate <= secondaryDateRange.ToDate &&
                                    l.CreateDate >= secondaryDateRange.FromDate).ToList();
                approvedApplicationListLastDay.AddRange(approvedLoanApplicationLogsLastday);

                var disApporvedLoanApplicationLogsLastday = GenService.GetAll<ApplicationLog>()
                    .Where(l => l.Status == EntityStatus.Active &&
                                l.ToStage < ApplicationStage.Drafted &&
                                loanApplicationsIds.Contains(l.ApplicationId) &&
                                l.CreateDate <= secondaryDateRange.ToDate &&
                                l.CreateDate >= secondaryDateRange.FromDate).ToList();

                disApprovedApplicationListLastDay.AddRange(disApporvedLoanApplicationLogsLastday);

                #endregion File Approved/DisApproved LastDay Apps.

                #region Amount disbursed / received Today

                var acceptableAppIds = applicationListToDay.Select(a => a.ApplicationId).Distinct().ToList();
                var TodaydisbursementMemos = GenService.GetAll<DMDetail>().Where(d => d.DisbursementMemo.ApplicationId != null
                            && d.DisbursementMemo.Application.BranchId != null
                            && branchIds.Contains(d.DisbursementMemo.Application.BranchId)
                            && d.DisbursementMemo.Application.LoanApplicationId != null
                            && d.DisbursementMemo.Application.Product.FacilityType != null
                            && (d.DisbursementMemo.Application.Product.FacilityType == ProposalFacilityType.Home_Loan
                                || d.DisbursementMemo.Application.Product.FacilityType == ProposalFacilityType.Auto_Loan
                                || d.DisbursementMemo.Application.Product.FacilityType == ProposalFacilityType.Personal_Loan)
                            && d.CreateDate <= dateRange.ToDate
                            && d.CreateDate >= dateRange.FromDate
                            && d.DisbursementMemo.IsApproved != null
                            && d.DisbursementMemo.IsApproved == true
                            && d.DisbursementMemo.IsDisbursed != null
                            && d.DisbursementMemo.IsDisbursed == true
                            && d.Status == EntityStatus.Active).ToList();
                var acceptableDMidstoday = TodaydisbursementMemos.Select(dm => dm.DisbursementMemoId).ToList();
                var disMemotoday = GenService.GetAll<DisbursementMemo>().Where(d => acceptableDMidstoday.Contains(d.Id)).ToList();
                disbursementMemosToday = disMemotoday.ToList();

                var TodayfundReceivedToday = GenService.GetAll<FundConfirmationDetail>().Where(d => d.FundConfirmation.ApplicationId != null
                            && d.FundConfirmation.Application.BranchId != null
                            && branchIds.Contains(d.FundConfirmation.Application.BranchId)
                            && d.FundConfirmation.Application.DepositApplicationId != null
                            && d.FundConfirmation.Application.Product.DepositType != null
                            && (d.FundConfirmation.Application.Product.DepositType == DepositType.Fixed
                                || d.FundConfirmation.Application.Product.DepositType == DepositType.Recurring)
                            && d.CreateDate <= dateRange.ToDate
                            && d.CreateDate >= dateRange.FromDate
                            && d.FundConfirmation.FundReceived != null
                            && d.FundConfirmation.FundReceived == true
                            && d.Status == EntityStatus.Active).ToList();
                var acceptableFCidstoday = TodayfundReceivedToday.Select(dm => dm.FundConfirmationId).ToList();
                var fundRectoday = GenService.GetAll<FundConfirmation>().Where(d => acceptableFCidstoday.Contains(d.Id)).ToList();
                fundReceivedToday = fundRectoday.ToList();

                #endregion Amount disbursed / received Today

                #region Amount disbursed / received Lastday

                var LastdaydisbursementMemos = GenService.GetAll<DMDetail>().Where(d => d.DisbursementMemo.ApplicationId != null
                            && d.DisbursementMemo.Application.BranchId != null
                            && branchIds.Contains(d.DisbursementMemo.Application.BranchId)
                            && d.DisbursementMemo.Application.LoanApplicationId != null
                            && d.DisbursementMemo.Application.Product.FacilityType != null
                            && (d.DisbursementMemo.Application.Product.FacilityType == ProposalFacilityType.Home_Loan
                                || d.DisbursementMemo.Application.Product.FacilityType == ProposalFacilityType.Auto_Loan
                                || d.DisbursementMemo.Application.Product.FacilityType == ProposalFacilityType.Personal_Loan)
                            && d.CreateDate <= secondaryDateRange.ToDate
                            && d.CreateDate >= secondaryDateRange.FromDate
                            && d.DisbursementMemo.IsApproved != null
                            && d.DisbursementMemo.IsApproved == true
                            && d.DisbursementMemo.IsDisbursed != null
                            && d.DisbursementMemo.IsDisbursed == true
                            && d.Status == EntityStatus.Active).ToList();
                var acceptableDMidslastday = LastdaydisbursementMemos.Select(dm => dm.DisbursementMemoId).ToList();
                var disMemolastday = GenService.GetAll<DisbursementMemo>().Where(d => acceptableDMidslastday.Contains(d.Id)).ToList();
                disbursementMemosLastday = disMemolastday.ToList();


                var LastdayfundReceived = GenService.GetAll<FundConfirmationDetail>().Where(d => d.FundConfirmation.ApplicationId != null
                            && d.FundConfirmation.Application.BranchId != null
                            && branchIds.Contains(d.FundConfirmation.Application.BranchId)
                            && d.FundConfirmation.Application.DepositApplicationId != null
                            && d.FundConfirmation.Application.Product.DepositType != null
                            && (d.FundConfirmation.Application.Product.DepositType == DepositType.Fixed
                                || d.FundConfirmation.Application.Product.DepositType == DepositType.Recurring)
                            && d.CreateDate <= secondaryDateRange.ToDate
                            && d.CreateDate >= secondaryDateRange.FromDate
                            && d.FundConfirmation.FundReceived != null
                            && d.FundConfirmation.FundReceived == true
                            && d.Status == EntityStatus.Active).ToList();
                var acceptableFCidslastday = LastdayfundReceived.Select(dm => dm.FundConfirmationId).ToList();
                var fundReclastday = GenService.GetAll<FundConfirmation>().Where(d => acceptableFCidslastday.Contains(d.Id)).ToList();
                fundReceivedLastday = fundReclastday.ToList();

                #endregion Amount disbursed / received Lastday

            }
            else
            {
                #region Call/Lead
                callListToday = GenService.GetAll<Call>().Where(c => c.CreateDate <= dateRange.ToDate && c.CreateDate >= dateRange.FromDate && c.Amount != null && c.Status == EntityStatus.Active).ToList();
                callListLastDay = GenService.GetAll<Call>().Where(c => c.CreateDate <= secondaryDateRange.ToDate && c.CreateDate >= secondaryDateRange.FromDate && c.Amount != null && c.Status == EntityStatus.Active).ToList();

                leadListToday = GenService.GetAll<SalesLead>().Where(c => c.CreateDate <= dateRange.ToDate && c.CreateDate >= dateRange.FromDate && c.Amount != null && c.Status == EntityStatus.Active).ToList();
                leadListLastDay = GenService.GetAll<SalesLead>().Where(c => c.CreateDate <= secondaryDateRange.ToDate && c.CreateDate >= secondaryDateRange.FromDate && c.Amount != null && c.Status == EntityStatus.Active).ToList();
                #endregion Call/Lead

                #region Submited Apps
                var acceptableLogsToday = GenService.GetAll<ApplicationLog>()
                    .Where(l => (l.Application.LoanApplicationId != null || l.Application.DepositApplicationId != null)
                    && (l.Application.LoanApplication.LoanAmountApplied != null || l.Application.DepositApplication.TotalDepositAmount != null)
                    && l.Status == EntityStatus.Active
                    && l.CreateDate <= dateRange.ToDate
                    && l.CreateDate >= dateRange.FromDate
                    && (((l.ToStage == ApplicationStage.SentToCRM || l.ToStage == ApplicationStage.UnderProcessAtCRM)
                    && l.Application.Product.ProductType == ProductType.Loan)
                    || ((l.ToStage == ApplicationStage.SentToOperations || l.ToStage == ApplicationStage.UnderProcessAtOperations)
                    && l.Application.Product.ProductType == ProductType.Deposit)));
                var allApplicationsToday = (from appLog in acceptableLogsToday
                                            orderby appLog.Id
                                            select appLog).Distinct().ToList();
                if (allApplicationsToday.Any())
                    applicationListToDay.AddRange(allApplicationsToday);


                var acceptableLogsLastday = GenService.GetAll<ApplicationLog>()
                    .Where(l => (l.Application.LoanApplicationId != null || l.Application.DepositApplicationId != null)
                    && (l.Application.LoanApplication.LoanAmountApplied != null || l.Application.DepositApplication.TotalDepositAmount != null)
                    && l.Status == EntityStatus.Active
                    && l.CreateDate <= secondaryDateRange.ToDate
                    && l.CreateDate >= secondaryDateRange.FromDate
                    && (((l.ToStage == ApplicationStage.SentToCRM || l.ToStage == ApplicationStage.UnderProcessAtCRM)
                    && l.Application.Product.ProductType == ProductType.Loan)
                    || ((l.ToStage == ApplicationStage.SentToOperations || l.ToStage == ApplicationStage.UnderProcessAtOperations)
                    && l.Application.Product.ProductType == ProductType.Deposit)));
                var allApplicationsLastday = (from appLog in acceptableLogsLastday
                                              orderby appLog.Id
                                              select appLog).Distinct().ToList();

                if (allApplicationsLastday.Any())
                    applicationListLastDay.AddRange(allApplicationsLastday);
                #endregion Submited Apps

                #region File Approved/DisApproved ToDay Apps.

                var loanApplicationsIds = GenService.GetAll<Application>().Where(a => a.Product.ProductType == ProductType.Loan).Select(a => a.Id);

                var approvedLoanApplicationLogsToday = GenService.GetAll<ApplicationLog>()
                .Where(l => l.Status == EntityStatus.Active &&
                                l.ToStage == ApplicationStage.SentToOperations &&
                                loanApplicationsIds.Contains(l.ApplicationId) &&
                l.CreateDate <= dateRange.ToDate &&
                                l.CreateDate >= dateRange.FromDate).ToList();
                if (approvedLoanApplicationLogsToday.Count > 0)
                    approvedApplicationListToDay.AddRange(approvedLoanApplicationLogsToday);

                var disApporvedLoanApplicationLogsToday = GenService.GetAll<ApplicationLog>()
                .Where(l => l.Status == EntityStatus.Active &&
                            l.ToStage < ApplicationStage.Drafted &&
                            loanApplicationsIds.Contains(l.ApplicationId) &&
                            l.CreateDate <= dateRange.ToDate &&
                            l.CreateDate >= dateRange.FromDate).ToList();

                disApprovedApplicationListToDay.AddRange(disApporvedLoanApplicationLogsToday);


                #endregion File Approved/DisApproved ToDay Apps.

                #region File Approved/DisApproved LastDay Apps.

                var approvedLoanApplicationLogsLastday = GenService.GetAll<ApplicationLog>()
                .Where(l => l.Status == EntityStatus.Active &&
                            l.ToStage == ApplicationStage.SentToOperations &&
                            loanApplicationsIds.Contains(l.ApplicationId) &&
                                    l.CreateDate <= secondaryDateRange.ToDate &&
                                    l.CreateDate >= secondaryDateRange.FromDate).ToList();
                approvedApplicationListLastDay.AddRange(approvedLoanApplicationLogsLastday);

                var disApporvedLoanApplicationLogsLastday = GenService.GetAll<ApplicationLog>()
                .Where(l => l.Status == EntityStatus.Active &&
                            l.ToStage < ApplicationStage.Drafted &&
                            loanApplicationsIds.Contains(l.ApplicationId) &&
                                l.CreateDate <= secondaryDateRange.ToDate &&
                                l.CreateDate >= secondaryDateRange.FromDate).ToList();

                disApprovedApplicationListLastDay.AddRange(disApporvedLoanApplicationLogsLastday);

                #endregion File Approved/DisApproved LastDay Apps.

                #region File disbursed / received Today
                var TodaydisbursementMemos = GenService.GetAll<DMDetail>().Where(d => d.DisbursementMemo.ApplicationId != null

                            && d.DisbursementMemo.Application.LoanApplicationId != null
                            && d.DisbursementMemo.Application.Product.FacilityType != null
                            && (d.DisbursementMemo.Application.Product.FacilityType == ProposalFacilityType.Home_Loan
                                || d.DisbursementMemo.Application.Product.FacilityType == ProposalFacilityType.Auto_Loan
                                || d.DisbursementMemo.Application.Product.FacilityType == ProposalFacilityType.Personal_Loan)
                            && d.CreateDate <= dateRange.ToDate
                            && d.CreateDate >= dateRange.FromDate
                            && d.DisbursementMemo.IsApproved != null
                            && d.DisbursementMemo.IsApproved == true
                            && d.DisbursementMemo.IsDisbursed != null
                            && d.DisbursementMemo.IsDisbursed == true
                            && d.Status == EntityStatus.Active).ToList();
                var acceptableDMids = TodaydisbursementMemos.Select(dm => dm.DisbursementMemoId).ToList();
                var disMemotoday = GenService.GetAll<DisbursementMemo>().Where(d => acceptableDMids.Contains(d.Id)).ToList();
                disbursementMemosToday = disMemotoday.ToList();

                var TodayfundReceived = GenService.GetAll<FundConfirmationDetail>().Where(d => d.FundConfirmation.ApplicationId != null

                            && d.FundConfirmation.Application.DepositApplicationId != null
                            && d.FundConfirmation.Application.Product.DepositType != null
                            && (d.FundConfirmation.Application.Product.DepositType == DepositType.Fixed
                                || d.FundConfirmation.Application.Product.DepositType == DepositType.Recurring)
                            && d.CreateDate <= dateRange.ToDate
                            && d.CreateDate >= dateRange.FromDate
                            && d.FundConfirmation.FundReceived != null
                            && d.FundConfirmation.FundReceived == true
                            && d.Status == EntityStatus.Active).ToList();
                var acceptableFCids = TodayfundReceived.Select(dm => dm.FundConfirmationId).ToList();
                var fundRectoday = GenService.GetAll<FundConfirmation>().Where(d => acceptableFCids.Contains(d.Id)).ToList();
                fundReceivedToday = fundRectoday.ToList();

                #endregion File disbursed / received Today

                #region File disbursed / received Lastday

                var LastdaydisbursementMemos = GenService.GetAll<DMDetail>().Where(d => d.DisbursementMemo.ApplicationId != null

                            && d.DisbursementMemo.Application.LoanApplicationId != null
                            && d.DisbursementMemo.Application.Product.FacilityType != null
                            && (d.DisbursementMemo.Application.Product.FacilityType == ProposalFacilityType.Home_Loan
                                || d.DisbursementMemo.Application.Product.FacilityType == ProposalFacilityType.Auto_Loan
                                || d.DisbursementMemo.Application.Product.FacilityType == ProposalFacilityType.Personal_Loan)
                            && d.CreateDate <= secondaryDateRange.ToDate
                            && d.CreateDate >= secondaryDateRange.FromDate
                            && d.DisbursementMemo.IsApproved != null
                            && d.DisbursementMemo.IsApproved == true
                            && d.DisbursementMemo.IsDisbursed != null
                            && d.DisbursementMemo.IsDisbursed == true
                            && d.Status == EntityStatus.Active).ToList();
                var acceptableDMidslastday = LastdaydisbursementMemos.Select(dm => dm.DisbursementMemoId).ToList();
                var disMemolastday = GenService.GetAll<DisbursementMemo>().Where(d => acceptableDMidslastday.Contains(d.Id)).ToList();
                disbursementMemosLastday = disMemolastday.ToList();


                var LastdayfundReceived = GenService.GetAll<FundConfirmationDetail>().Where(d => d.FundConfirmation.ApplicationId != null

                            && d.FundConfirmation.Application.DepositApplicationId != null
                            && d.FundConfirmation.Application.Product.DepositType != null
                            && (d.FundConfirmation.Application.Product.DepositType == DepositType.Fixed
                                || d.FundConfirmation.Application.Product.DepositType == DepositType.Recurring)
                            && d.CreateDate <= secondaryDateRange.ToDate
                            && d.CreateDate >= secondaryDateRange.FromDate
                            && d.FundConfirmation.FundReceived != null
                            && d.FundConfirmation.FundReceived == true
                            && d.Status == EntityStatus.Active).ToList();
                var acceptableFCidslastday = LastdayfundReceived.Select(dm => dm.FundConfirmationId).ToList();
                var fundReclastday = GenService.GetAll<FundConfirmation>().Where(d => acceptableFCidslastday.Contains(d.Id)).ToList();
                fundReceivedLastday = fundReclastday.ToList();


                #endregion File disbursed / received Lastday
            }

            #region initializations

            data.Disbursed_HomeLoanAmount = 0;
            data.Disbursed_PersonalLoanAmount = 0;
            data.Disbursed_AutoLoanAmount = 0;
            data.RMHomeLoan1 = "";
            data.RMHomeLoan1Amount = 0;
            data.RMHomeLoan1Count = 0;
            data.RMHomeLoan2 = "";
            data.RMHomeLoan2Amount = 0;
            data.RMHomeLoan2Count = 0;
            data.RMPersonalLoan = "";
            data.RMPersonalLoanAmount = 0;
            data.RMPersonalLoanCount = 0;
            data.RMAutoLoan = "";
            data.RMAutoLoanAmount = 0;
            data.RMAutoLoanCount = 0;
            data.RMLiability1 = "";
            data.RMLiability1Amount = 0;
            data.RMLiability1Count = 0;
            data.RMLiability2 = "";
            data.RMLiability2Amount = 0;
            data.RMLiability2Count = 0;
            #endregion

            #region Call
            var callListTodayHome = callListToday.Where(c => c.ProductId != null && c.Product.FacilityType == ProposalFacilityType.Home_Loan).ToList();
            var callListLastHome = callListLastDay.Where(c => c.ProductId != null && c.Product.FacilityType == ProposalFacilityType.Home_Loan).ToList();

            var callListTodayAuto = callListToday.Where(c => c.ProductId != null && c.Product.FacilityType == ProposalFacilityType.Auto_Loan).ToList();
            var callListLastAuto = callListLastDay.Where(c => c.ProductId != null && c.Product.FacilityType == ProposalFacilityType.Auto_Loan).ToList();

            var callListTodayPersonal = callListToday.Where(c => c.ProductId != null && c.Product.FacilityType == ProposalFacilityType.Personal_Loan).ToList();
            var callListLastPersonal = callListLastDay.Where(c => c.ProductId != null && c.Product.FacilityType == ProposalFacilityType.Personal_Loan).ToList();

            var callListTodayFixedDeposit = callListToday.Where(c => c.ProductId != null && c.Product.DepositType == DepositType.Fixed).ToList();
            var callListLastFixedDeposit = callListLastDay.Where(c => c.ProductId != null && c.Product.DepositType == DepositType.Fixed).ToList();

            var callListTodayRecurrentDeposit = callListToday.Where(c => c.ProductId != null && c.Product.DepositType == DepositType.Recurring).ToList();
            var callListLastRecurrentDeposit = callListLastDay.Where(c => c.ProductId != null && c.Product.DepositType == DepositType.Recurring).ToList();

            var callListTodayUndefined = callListToday.Where(c => c.ProductId == null).ToList();
            var callListLastUndefined = callListLastDay.Where(c => c.ProductId == null).ToList();
            #endregion Call

            #region Lead
            var leadListTodayHome = leadListToday.Where(l => l.ProductId != null && l.Product.FacilityType == ProposalFacilityType.Home_Loan).ToList();
            var leadListLastHome = leadListLastDay.Where(l => l.ProductId != null && l.Product.FacilityType == ProposalFacilityType.Home_Loan).ToList();

            var leadListTodayAuto = leadListToday.Where(l => l.ProductId != null && l.Product.FacilityType == ProposalFacilityType.Auto_Loan).ToList();
            var leadListLastAuto = leadListLastDay.Where(l => l.ProductId != null && l.Product.FacilityType == ProposalFacilityType.Auto_Loan).ToList();

            var leadListTodayPersonal = leadListToday.Where(l => l.ProductId != null && l.Product.FacilityType == ProposalFacilityType.Personal_Loan).ToList();
            var leadListLastPersonal = leadListLastDay.Where(l => l.ProductId != null && l.Product.FacilityType == ProposalFacilityType.Personal_Loan).ToList();

            var leadListTodayFixedDeposit = leadListToday.Where(l => l.ProductId != null && l.Product.DepositType == DepositType.Fixed).ToList();
            var leadListLastFixedDeposit = leadListLastDay.Where(l => l.ProductId != null && l.Product.DepositType == DepositType.Fixed).ToList();

            var leadListTodayRecurrentDeposit = leadListToday.Where(l => l.ProductId != null && l.Product.DepositType == DepositType.Recurring).ToList();
            var leadListLastRecurrentDeposit = leadListLastDay.Where(l => l.ProductId != null && l.Product.DepositType == DepositType.Recurring).ToList();
            #endregion Lead

            #region File Submited Apps.
            var applicationListTodayHome = applicationListToDay.Where(l => l.Application.LoanApplicationId != null && l.Application.ProductId != null && l.Application.Product.FacilityType == ProposalFacilityType.Home_Loan).ToList();
            var applicationListLastHome = applicationListLastDay.Where(l => l.Application.LoanApplicationId != null && l.Application.ProductId != null && l.Application.Product.FacilityType == ProposalFacilityType.Home_Loan).ToList();

            var applicationListTodayAuto = applicationListToDay.Where(l => l.Application.LoanApplicationId != null && l.Application.ProductId != null && l.Application.Product.FacilityType == ProposalFacilityType.Auto_Loan).ToList();
            var applicationListLastAuto = applicationListLastDay.Where(l => l.Application.LoanApplicationId != null && l.Application.ProductId != null && l.Application.Product.FacilityType == ProposalFacilityType.Auto_Loan).ToList();

            var applicationListTodayPersonal = applicationListToDay.Where(l => l.Application.LoanApplicationId != null && l.Application.ProductId != null && l.Application.Product.FacilityType == ProposalFacilityType.Personal_Loan).ToList();
            var applicationListLastPersonal = applicationListLastDay.Where(l => l.Application.LoanApplicationId != null && l.Application.ProductId != null && l.Application.Product.FacilityType == ProposalFacilityType.Personal_Loan).ToList();

            var applicationListTodayFixedDeposit = applicationListToDay.Where(l => l.Application.DepositApplicationId != null && l.Application.ProductId != null && l.Application.Product.DepositType == DepositType.Fixed).ToList();
            var applicationListLastFixedDeposit = applicationListLastDay.Where(l => l.Application.DepositApplicationId != null && l.Application.ProductId != null && l.Application.Product.DepositType == DepositType.Fixed).ToList();

            var applicationListTodayRecurrentDeposit = applicationListToDay.Where(l => l.Application.DepositApplicationId != null && l.Application.ProductId != null && l.Application.Product.DepositType == DepositType.Recurring).ToList();
            var applicationListLastRecurrentDeposit = applicationListLastDay.Where(l => l.Application.DepositApplicationId != null && l.Application.ProductId != null && l.Application.Product.DepositType == DepositType.Recurring).ToList();
            #endregion File Submited Apps.

            #region File Approved Apps.
            var approvedApplicationListTodayHome = approvedApplicationListToDay.Where(l => l.Application.LoanApplicationId != null && l.Application.ProductId != null && l.Application.Product.FacilityType != null && l.Application.Product.FacilityType == ProposalFacilityType.Home_Loan).ToList();

            var approvedHomeLoanLogsWithAmountToday = new List<Proposal>();

            if (approvedApplicationListTodayHome.Count > 0)
            {
                //approvedHomeLoanLogsWithAmountToday = approvedHomeLoanLogsWithAmountToday.Where(p => approvedApplicationListTodayHome.Any(a => a.ApplicationId == p.ApplicationId)).ToList();
                approvedHomeLoanLogsWithAmountToday = (from logs in approvedApplicationListTodayHome
                                                       join proposal in GenService.GetAll<Proposal>().Where(p => p.Status == EntityStatus.Active) on
                                                           logs.ApplicationId equals proposal.ApplicationId into proposals
                                                       from p in proposals.DefaultIfEmpty()
                                                       select p).ToList();
            }

            var approvedApplicationListLastHome = approvedApplicationListLastDay.Where(l => l.Application.LoanApplicationId != null && l.Application.ProductId != null && l.Application.Product.FacilityType != null && l.Application.Product.FacilityType == ProposalFacilityType.Home_Loan).ToList();

            var approvedHomeLoanLogsWithAmountLastday = new List<Proposal>();

            if (approvedApplicationListLastHome.Count > 0)
            {
                //approvedHomeLoanLogsWithAmountLastday = GenService.GetAll<Proposal>().Where(p => approvedApplicationListLastHome.Any(a => a.ApplicationId == p.ApplicationId)).ToList();

                approvedHomeLoanLogsWithAmountLastday = (from logs in approvedApplicationListLastHome
                                                         join proposal in GenService.GetAll<Proposal>().Where(p => p.Status == EntityStatus.Active) on
                                                             logs.ApplicationId equals proposal.ApplicationId into proposals
                                                         from p in proposals.DefaultIfEmpty()
                                                         select p).ToList();
            }

            var approvedApplicationListTodayAuto = approvedApplicationListToDay.Where(l => l.Application.LoanApplicationId != null && l.Application.ProductId != null && l.Application.Product.FacilityType != null && l.Application.Product.FacilityType == ProposalFacilityType.Auto_Loan).ToList();

            var approvedAutoLoanLogsWithAmountToday = new List<Proposal>();

            if (approvedApplicationListTodayAuto.Count > 0)
            {
                //approvedAutoLoanLogsWithAmountToday = GenService.GetAll<Proposal>().Where(p => approvedApplicationListTodayAuto.Any(a => a.ApplicationId == p.ApplicationId)).ToList();
                approvedAutoLoanLogsWithAmountToday = (from logs in approvedApplicationListTodayAuto
                                                       join proposal in GenService.GetAll<Proposal>().Where(p => p.Status == EntityStatus.Active) on
                                                           logs.ApplicationId equals proposal.ApplicationId into proposals
                                                       from p in proposals.DefaultIfEmpty()
                                                       select p).ToList();
            }

            var approvedApplicationListLastAuto = approvedApplicationListLastDay.Where(l => l.Application.LoanApplicationId != null && l.Application.ProductId != null && l.Application.Product.FacilityType != null && l.Application.Product.FacilityType == ProposalFacilityType.Auto_Loan).ToList();

            var approvedAutoLoanLogsWithAmountLastday = new List<Proposal>();

            if (approvedApplicationListLastAuto.Count > 0)
            {
                //approvedAutoLoanLogsWithAmountLastday = GenService.GetAll<Proposal>().Where(p => approvedApplicationListLastAuto.Any(a => a.ApplicationId == p.ApplicationId)).ToList();
                approvedAutoLoanLogsWithAmountLastday = (from logs in approvedApplicationListLastAuto
                                                         join proposal in GenService.GetAll<Proposal>().Where(p => p.Status == EntityStatus.Active) on
                                                             logs.ApplicationId equals proposal.ApplicationId into proposals
                                                         from p in proposals.DefaultIfEmpty()
                                                         select p).ToList();
            }

            var approvedApplicationListTodayPersonal = approvedApplicationListToDay.Where(l => l.Application.LoanApplicationId != null && l.Application.ProductId != null && l.Application.Product.FacilityType != null && l.Application.Product.FacilityType == ProposalFacilityType.Personal_Loan).ToList();
            var approvedPersonalLoanLogsWithAmountToday = new List<Proposal>();

            if (approvedApplicationListTodayPersonal.Count > 0)
            {
                //approvedPersonalLoanLogsWithAmountToday = GenService.GetAll<Proposal>().Where(p => approvedApplicationListTodayPersonal.Any(a => a.ApplicationId == p.ApplicationId)).ToList();
                approvedPersonalLoanLogsWithAmountToday = (from logs in approvedApplicationListTodayPersonal
                                                           join proposal in GenService.GetAll<Proposal>().Where(p => p.Status == EntityStatus.Active) on
                                                               logs.ApplicationId equals proposal.ApplicationId into proposals
                                                           from p in proposals.DefaultIfEmpty()
                                                           select p).ToList();
            }

            var approvedApplicationListLastPersonal = approvedApplicationListLastDay.Where(l => l.Application.LoanApplicationId != null && l.Application.ProductId != null && l.Application.Product.FacilityType != null && l.Application.Product.FacilityType == ProposalFacilityType.Personal_Loan).ToList();
            var approvedPersonalLoanLogsWithAmountLastday = new List<Proposal>();

            if (approvedApplicationListLastPersonal.Count > 0)
            {
                //approvedPersonalLoanLogsWithAmountLastday = GenService.GetAll<Proposal>().Where(p => approvedApplicationListLastPersonal.Any(a => a.ApplicationId == p.ApplicationId)).ToList();
                approvedPersonalLoanLogsWithAmountLastday = (from logs in approvedApplicationListLastPersonal
                                                             join proposal in GenService.GetAll<Proposal>().Where(p => p.Status == EntityStatus.Active) on
                                                                 logs.ApplicationId equals proposal.ApplicationId into proposals
                                                             from p in proposals.DefaultIfEmpty()
                                                             select p).ToList();
            }

            #endregion File Approved Apps.

            #region File DisApproved Apps.
            var disApprovedApplicationListTodayHome = disApprovedApplicationListToDay.Where(l => l.Application.LoanApplicationId != null && l.Application.ProductId != null && l.Application.Product.FacilityType != null && l.Application.Product.FacilityType == ProposalFacilityType.Home_Loan).ToList();

            var disApprovedHomeLoanLogsWithAmountToday = new List<Proposal>();

            if (disApprovedApplicationListTodayHome.Count > 0)
            {
                //disApprovedHomeLoanLogsWithAmountToday = GenService.GetAll<Proposal>().Where(p => disApprovedApplicationListTodayHome.Any(a => a.ApplicationId == p.ApplicationId)).ToList();
                disApprovedHomeLoanLogsWithAmountToday = (from logs in disApprovedApplicationListTodayHome
                                                          join proposal in GenService.GetAll<Proposal>().Where(p => p.Status == EntityStatus.Active) on
                                                              logs.ApplicationId equals proposal.ApplicationId into proposals
                                                          from p in proposals.DefaultIfEmpty()
                                                          select p).ToList();
            }

            var disApprovedApplicationListLastHome = disApprovedApplicationListLastDay.Where(l => l.Application.LoanApplicationId != null && l.Application.ProductId != null && l.Application.Product.FacilityType != null && l.Application.Product.FacilityType == ProposalFacilityType.Home_Loan).ToList();

            var disApprovedHomeLoanLogsWithAmountLastday = new List<Proposal>();

            if (disApprovedApplicationListLastHome.Count > 0)
            {
                //disApprovedHomeLoanLogsWithAmountLastday = GenService.GetAll<Proposal>().Where(p => disApprovedApplicationListLastHome.Any(a => a.ApplicationId == p.ApplicationId)).ToList();
                disApprovedHomeLoanLogsWithAmountLastday = (from logs in disApprovedApplicationListLastHome
                                                            join proposal in GenService.GetAll<Proposal>().Where(p => p.Status == EntityStatus.Active) on
                                                                logs.ApplicationId equals proposal.ApplicationId into proposals
                                                            from p in proposals.DefaultIfEmpty()
                                                            select p).ToList();
            }

            var disApprovedApplicationListTodayAuto = disApprovedApplicationListToDay.Where(l => l.Application.LoanApplicationId != null && l.Application.ProductId != null && l.Application.Product.FacilityType != null && l.Application.Product.FacilityType == ProposalFacilityType.Auto_Loan).ToList();

            var disApprovedAutoLoanLogsWithAmountToday = new List<Proposal>();

            if (disApprovedApplicationListTodayAuto.Count > 0)
            {
                //disApprovedAutoLoanLogsWithAmountToday = GenService.GetAll<Proposal>().Where(p => disApprovedApplicationListTodayAuto.Any(a => a.ApplicationId == p.ApplicationId)).ToList();
                disApprovedAutoLoanLogsWithAmountToday = (from logs in disApprovedApplicationListTodayAuto
                                                          join proposal in GenService.GetAll<Proposal>().Where(p => p.Status == EntityStatus.Active) on
                                                              logs.ApplicationId equals proposal.ApplicationId into proposals
                                                          from p in proposals.DefaultIfEmpty()
                                                          select p).ToList();
            }

            var disApprovedApplicationListLastAuto = disApprovedApplicationListLastDay.Where(l => l.Application.LoanApplicationId != null && l.Application.ProductId != null && l.Application.Product.FacilityType != null && l.Application.Product.FacilityType == ProposalFacilityType.Auto_Loan).ToList();

            var disApprovedAutoLoanLogsWithAmountLastday = new List<Proposal>();

            if (disApprovedApplicationListLastAuto.Count > 0)
            {
                //disApprovedAutoLoanLogsWithAmountLastday = GenService.GetAll<Proposal>().Where(p => disApprovedApplicationListLastAuto.Any(a => a.ApplicationId == p.ApplicationId)).ToList();
                disApprovedAutoLoanLogsWithAmountLastday = (from logs in disApprovedApplicationListLastAuto
                                                            join proposal in GenService.GetAll<Proposal>().Where(p => p.Status == EntityStatus.Active) on
                                                                logs.ApplicationId equals proposal.ApplicationId into proposals
                                                            from p in proposals.DefaultIfEmpty()
                                                            select p).ToList();
            }

            var disApprovedApplicationListTodayPersonal = disApprovedApplicationListToDay.Where(l => l.Application.LoanApplicationId != null && l.Application.ProductId != null && l.Application.Product.FacilityType != null && l.Application.Product.FacilityType == ProposalFacilityType.Personal_Loan).ToList();
            var disApprovedPersonalLoanLogsWithAmountToday = new List<Proposal>();

            if (disApprovedApplicationListTodayPersonal.Count > 0)
            {
                //disApprovedPersonalLoanLogsWithAmountToday = GenService.GetAll<Proposal>().Where(p => disApprovedApplicationListTodayPersonal.Any(a => a.ApplicationId == p.ApplicationId)).ToList();
                disApprovedPersonalLoanLogsWithAmountToday = (from logs in disApprovedApplicationListTodayPersonal
                                                              join proposal in GenService.GetAll<Proposal>().Where(p => p.Status == EntityStatus.Active) on
                                                                  logs.ApplicationId equals proposal.ApplicationId into proposals
                                                              from p in proposals.DefaultIfEmpty()
                                                              select p).ToList();
            }

            var disApprovedApplicationListLastPersonal = disApprovedApplicationListLastDay.Where(l => l.Application.LoanApplicationId != null && l.Application.ProductId != null && l.Application.Product.FacilityType != null && l.Application.Product.FacilityType == ProposalFacilityType.Personal_Loan).ToList();
            var disApprovedPersonalLoanLogsWithAmountLastday = new List<Proposal>();

            if (disApprovedApplicationListLastPersonal.Count > 0)
            {
                //disApprovedPersonalLoanLogsWithAmountLastday = GenService.GetAll<Proposal>().Where(p => disApprovedApplicationListLastPersonal.Any(a => a.ApplicationId == p.ApplicationId)).ToList();
                disApprovedPersonalLoanLogsWithAmountLastday = (from logs in disApprovedApplicationListLastPersonal
                                                                join proposal in GenService.GetAll<Proposal>().Where(p => p.Status == EntityStatus.Active) on
                                                                    logs.ApplicationId equals proposal.ApplicationId into proposals
                                                                from p in proposals.DefaultIfEmpty()
                                                                select p).ToList();
            }

            #endregion File DisApproved Apps.

            if (criteria == Criteria.Amount)
            {
                #region Call

                data.TodayHomeLoanAmount = (decimal)callListTodayHome.Where(a => a.Product.FacilityType == ProposalFacilityType.Home_Loan).Sum(a => a.Amount != null ? a.Amount : 0);
                data.LastdayHomeLoanAmount = (decimal)callListLastHome.Where(a => a.Product.FacilityType == ProposalFacilityType.Home_Loan).Sum(a => a.Amount != null ? a.Amount : 0);

                data.TodayAutoLoanAmount = (decimal)callListTodayAuto.Where(a => a.Product.FacilityType == ProposalFacilityType.Auto_Loan).Sum(a => a.Amount != null ? a.Amount : 0);
                data.LastdayAutoLoanAmount = (decimal)callListLastAuto.Where(a => a.Product.FacilityType == ProposalFacilityType.Auto_Loan).Sum(a => a.Amount != null ? a.Amount : 0);

                data.TodayPersonalLoanAmount = (decimal)callListTodayPersonal.Where(a => a.Product.FacilityType == ProposalFacilityType.Personal_Loan).Sum(a => a.Amount != null ? a.Amount : 0);
                data.LastdayPersonalLoanAmount = (decimal)callListLastAuto.Where(a => a.Product.FacilityType == ProposalFacilityType.Auto_Loan).Sum(a => a.Amount != null ? a.Amount : 0);

                data.TodayFixedDepositAmount = (decimal)callListTodayPersonal.Where(a => a.Product.FacilityType == ProposalFacilityType.Personal_Loan).Sum(a => a.Amount != null ? a.Amount : 0);
                data.LastdayFixedDepositAmount = (decimal)callListLastPersonal.Where(a => a.Product.FacilityType == ProposalFacilityType.Personal_Loan).Sum(a => a.Amount != null ? a.Amount : 0);

                data.TodayRecurrentDepositAmount = (decimal)callListTodayRecurrentDeposit.Where(a => a.Product.DepositType == DepositType.Recurring).Sum(a => a.Amount != null ? a.Amount : 0);
                data.LastdayRecurrentDepositAmount = (decimal)callListLastRecurrentDeposit.Where(a => a.Product.DepositType == DepositType.Recurring).Sum(a => a.Amount != null ? a.Amount : 0);

                data.TodayUndefinedAmount = (decimal)callListTodayUndefined.Where(a => a.ProductId == null).Sum(a => a.Amount != null ? a.Amount : 0);
                data.LastdayUndefinedAmount = (decimal)callListLastUndefined.Where(a => a.ProductId == null).Sum(a => a.Amount != null ? a.Amount : 0);

                #endregion Call

                #region Lead

                data.TodayLeadHomeLoanAmount = (decimal)leadListTodayHome.Where(a => a.Product.FacilityType == ProposalFacilityType.Home_Loan).Sum(a => a.Amount != null ? a.Amount : 0);
                data.LastdayLeadHomeLoanAmount = (decimal)leadListLastHome.Where(a => a.Product.FacilityType == ProposalFacilityType.Home_Loan).Sum(a => a.Amount != null ? a.Amount : 0);

                data.TodayLeadAutoLoanAmount = (decimal)leadListTodayAuto.Where(a => a.Product.FacilityType == ProposalFacilityType.Auto_Loan).Sum(a => a.Amount != null ? a.Amount : 0);
                data.LastdayLeadAutoLoanAmount = (decimal)leadListLastAuto.Where(a => a.Product.FacilityType == ProposalFacilityType.Auto_Loan).Sum(a => a.Amount != null ? a.Amount : 0);

                data.TodayLeadPersonalLoanAmount = (decimal)leadListTodayPersonal.Where(a => a.Product.FacilityType == ProposalFacilityType.Personal_Loan).Sum(a => a.Amount != null ? a.Amount : 0);
                data.LastdayLeadPersonalLoanAmount = (decimal)leadListLastPersonal.Where(a => a.Product.FacilityType == ProposalFacilityType.Personal_Loan).Sum(a => a.Amount != null ? a.Amount : 0);

                data.TodayLeadFixedDepositAmount = (decimal)leadListTodayFixedDeposit.Where(a => a.Product.DepositType == DepositType.Fixed).Sum(a => a.Amount != null ? a.Amount : 0);
                data.LastdayLeadFixedDepositAmount = (decimal)leadListLastFixedDeposit.Where(a => a.Product.DepositType == DepositType.Fixed).Sum(a => a.Amount != null ? a.Amount : 0);

                data.TodayLeadRecurrentDepositAmount = (decimal)leadListTodayRecurrentDeposit.Where(a => a.Product.DepositType == DepositType.Recurring).Sum(a => a.Amount != null ? a.Amount : 0);
                data.LastdayLeadRecurrentDepositAmount = (decimal)leadListLastRecurrentDeposit.Where(a => a.Product.DepositType == DepositType.Recurring).Sum(a => a.Amount != null ? a.Amount : 0);
                #endregion Lead

                #region File Submited Apps.

                data.TodayApplicationHomeLoanAmount = applicationListTodayHome.Where(a => a.Application.Product.FacilityType == ProposalFacilityType.Home_Loan).Sum(a => a.Application.LoanApplication.LoanAmountApplied != null ? a.Application.LoanApplication.LoanAmountApplied : 0);
                data.LastdayApplicationHomeLoanAmount = applicationListLastHome.Where(a => a.Application.Product.FacilityType == ProposalFacilityType.Home_Loan).Sum(a => a.Application.LoanApplication.LoanAmountApplied != null ? a.Application.LoanApplication.LoanAmountApplied : 0);

                data.TodayApplicationAutoLoanAmount = applicationListTodayAuto.Where(a => a.Application.Product.FacilityType == ProposalFacilityType.Auto_Loan).Sum(a => a.Application.LoanApplication.LoanAmountApplied != null ? a.Application.LoanApplication.LoanAmountApplied : 0); ;
                data.LastdayApplicationAutoLoanAmount = applicationListLastAuto.Where(a => a.Application.Product.FacilityType == ProposalFacilityType.Auto_Loan).Sum(a => a.Application.LoanApplication.LoanAmountApplied != null ? a.Application.LoanApplication.LoanAmountApplied : 0); ;

                data.TodayApplicationPersonalLoanAmount = applicationListTodayPersonal.Where(a => a.Application.Product.FacilityType == ProposalFacilityType.Personal_Loan).Sum(a => a.Application.LoanApplication.LoanAmountApplied != null ? a.Application.LoanApplication.LoanAmountApplied : 0); ;
                data.LastdayApplicationPersonalLoanAmount = applicationListLastPersonal.Where(a => a.Application.Product.FacilityType == ProposalFacilityType.Personal_Loan).Sum(a => a.Application.LoanApplication.LoanAmountApplied != null ? a.Application.LoanApplication.LoanAmountApplied : 0); ;

                data.TodayApplicationFixedDepositAmount = applicationListTodayFixedDeposit.Where(a => a.Application.Product.DepositType == DepositType.Fixed).Sum(a => a.Application.DepositApplication.TotalDepositAmount != null ? a.Application.DepositApplication.TotalDepositAmount : 0); ;
                data.LastdayApplicationFixedDepositAmount = applicationListLastFixedDeposit.Where(a => a.Application.Product.DepositType == DepositType.Fixed).Sum(a => a.Application.DepositApplication.TotalDepositAmount != null ? a.Application.DepositApplication.TotalDepositAmount : 0); ;

                data.TodayApplicationRecurrentDepositAmount = applicationListTodayRecurrentDeposit.Where(a => a.Application.Product.DepositType == DepositType.Recurring).Sum(a => a.Application.DepositApplication.TotalDepositAmount != null ? a.Application.DepositApplication.TotalDepositAmount : 0); ;
                data.LastdayApplicationRecurrentDepositAmount = applicationListLastRecurrentDeposit.Where(a => a.Application.Product.DepositType == DepositType.Recurring).Sum(a => a.Application.DepositApplication.TotalDepositAmount != null ? a.Application.DepositApplication.TotalDepositAmount : 0); ;

                #endregion File Submited Apps.

                #region File Approved Apps.

                data.TodayApprovedApplicationHomeLoanAmount = approvedHomeLoanLogsWithAmountToday.Where(a => a.FacilityType == ProposalFacilityType.Home_Loan).Sum(a => a.RecomendedLoanAmountFromIPDC != null ? a.RecomendedLoanAmountFromIPDC : 0) ?? 0;
                data.LastdayApprovedApplicationHomeLoanAmount = approvedHomeLoanLogsWithAmountLastday.Where(a => a.FacilityType == ProposalFacilityType.Home_Loan).Sum(a => a.RecomendedLoanAmountFromIPDC != null ? a.RecomendedLoanAmountFromIPDC : 0) ?? 0;
                data.TodayApprovedApplicationAutoLoanAmount = approvedAutoLoanLogsWithAmountToday.Where(a => a.FacilityType == ProposalFacilityType.Auto_Loan).Sum(a => a.RecomendedLoanAmountFromIPDC != null ? a.RecomendedLoanAmountFromIPDC : 0) ?? 0;
                data.LastdayApprovedApplicationAutoLoanAmount = approvedAutoLoanLogsWithAmountLastday.Where(a => a.FacilityType == ProposalFacilityType.Auto_Loan).Sum(a => a.RecomendedLoanAmountFromIPDC != null ? a.RecomendedLoanAmountFromIPDC : 0) ?? 0;
                data.TodayApprovedApplicationPersonalLoanAmount = approvedPersonalLoanLogsWithAmountToday.Where(a => a.FacilityType == ProposalFacilityType.Personal_Loan).Sum(a => a.RecomendedLoanAmountFromIPDC != null ? a.RecomendedLoanAmountFromIPDC : 0) ?? 0;
                data.LastdayApprovedApplicationPersonalLoanAmount = approvedPersonalLoanLogsWithAmountLastday.Where(a => a.FacilityType == ProposalFacilityType.Personal_Loan).Sum(a => a.RecomendedLoanAmountFromIPDC != null ? a.RecomendedLoanAmountFromIPDC : 0) ?? 0;

                #endregion File Approved Apps.

                #region File DisApproved Apps.
                data.TodayDisApprovedApplicationHomeLoanAmount = disApprovedHomeLoanLogsWithAmountToday.Where(a => a.FacilityType == ProposalFacilityType.Home_Loan).Sum(a => a.RecomendedLoanAmountFromIPDC != null ? a.RecomendedLoanAmountFromIPDC : 0) ?? 0;
                data.LastdayDisApprovedApplicationHomeLoanAmount = disApprovedHomeLoanLogsWithAmountLastday.Where(a => a.FacilityType == ProposalFacilityType.Home_Loan).Sum(a => a.RecomendedLoanAmountFromIPDC != null ? a.RecomendedLoanAmountFromIPDC : 0) ?? 0;
                data.TodayDisApprovedApplicationAutoLoanAmount = disApprovedAutoLoanLogsWithAmountToday.Where(a => a.FacilityType != null && a.FacilityType == ProposalFacilityType.Auto_Loan).Sum(a => a.RecomendedLoanAmountFromIPDC != null ? a.RecomendedLoanAmountFromIPDC : 0) ?? 0;
                data.LastdayDisApprovedApplicationAutoLoanAmount = disApprovedAutoLoanLogsWithAmountLastday.Where(a => a.FacilityType == ProposalFacilityType.Auto_Loan).Sum(a => a.RecomendedLoanAmountFromIPDC != null ? a.RecomendedLoanAmountFromIPDC : 0) ?? 0;
                data.TodayDisApprovedApplicationPersonalLoanAmount = disApprovedPersonalLoanLogsWithAmountToday.Where(a => a.FacilityType == ProposalFacilityType.Personal_Loan).Sum(a => a.RecomendedLoanAmountFromIPDC != null ? a.RecomendedLoanAmountFromIPDC : 0) ?? 0;
                data.LastdayDisApprovedApplicationPersonalLoanAmount = disApprovedPersonalLoanLogsWithAmountLastday.Where(a => a.FacilityType == ProposalFacilityType.Personal_Loan).Sum(a => a.RecomendedLoanAmountFromIPDC != null ? a.RecomendedLoanAmountFromIPDC : 0) ?? 0;

                #endregion File DisApproved Apps.

                #region Disbursed/Received

                if (disbursementMemosToday.Where(d => d.Application.Product.FacilityType == ProposalFacilityType.Home_Loan).Count() > 0)
                    data.Disbursed_HomeLoanAmountToday = disbursedMemodetails.Where(d => disbursementMemosToday.Any(dis => dis.Id == d.DisbursementMemoId && dis.Application.Product.FacilityType == ProposalFacilityType.Home_Loan)).Sum(d => d.DisbursementAmount);

                if (disbursementMemosLastday.Where(d => d.Application.Product.FacilityType == ProposalFacilityType.Home_Loan).Count() > 0)
                    data.Disbursed_HomeLoanAmountLastday = disbursedMemodetails.Where(d => disbursementMemosLastday.Any(dis => dis.Id == d.DisbursementMemoId && dis.Application.Product.FacilityType == ProposalFacilityType.Home_Loan)).Sum(d => d.DisbursementAmount);

                if (disbursementMemosToday.Where(d => d.Application.Product.FacilityType == ProposalFacilityType.Auto_Loan).Count() > 0)
                    data.Disbursed_AutoLoanAmountToday = disbursedMemodetails.Where(d => disbursementMemosToday.Any(dis => dis.Id == d.DisbursementMemoId && dis.Application.Product.FacilityType == ProposalFacilityType.Auto_Loan)).Sum(d => d.DisbursementAmount);

                if (disbursementMemosLastday.Where(d => d.Application.Product.FacilityType == ProposalFacilityType.Auto_Loan).Count() > 0)
                    data.Disbursed_AutoLoanAmountLastday = disbursedMemodetails.Where(d => disbursementMemosLastday.Any(dis => dis.Id == d.DisbursementMemoId && dis.Application.Product.FacilityType == ProposalFacilityType.Auto_Loan)).Sum(d => d.DisbursementAmount);

                if (disbursementMemosToday.Where(d => d.Application.Product.FacilityType == ProposalFacilityType.Personal_Loan).Count() > 0)
                    data.Disbursed_PersonalLoanAmountToday = disbursedMemodetails.Where(d => disbursementMemosToday.Any(dis => dis.Id == d.DisbursementMemoId && dis.Application.Product.FacilityType == ProposalFacilityType.Personal_Loan)).Sum(d => d.DisbursementAmount);

                if (disbursementMemosLastday.Where(d => d.Application.Product.FacilityType == ProposalFacilityType.Personal_Loan).Count() > 0)
                    data.Disbursed_PersonalLoanAmountLastday = disbursedMemodetails.Where(d => disbursementMemosLastday.Any(dis => dis.Id == d.DisbursementMemoId && dis.Application.Product.FacilityType == ProposalFacilityType.Personal_Loan)).Sum(d => d.DisbursementAmount);

                if (fundReceivedToday.Where(d => d.Application.Product.DepositType == DepositType.Fixed).Count() > 0)
                    data.FundReceived_FixedAmountToday = (decimal)fundConfirmdetails.Where(d => fundReceivedToday.Any(dis => dis.Id == d.FundConfirmationId && dis.Application.Product.DepositType == DepositType.Fixed)).Sum(d => d.Amount);

                if (fundReceivedLastday.Where(d => d.Application.Product.DepositType == DepositType.Fixed).Count() > 0)
                    data.FundReceived_FixedAmountLastday = (decimal)fundConfirmdetails.Where(d => fundReceivedLastday.Any(dis => dis.Id == d.FundConfirmationId && dis.Application.Product.DepositType == DepositType.Fixed)).Sum(d => d.Amount);

                if (fundReceivedToday.Where(d => d.Application.Product.DepositType == DepositType.Recurring).Count() > 0)
                    data.FundReceived_RecurrentAmountToday = (decimal)fundConfirmdetails.Where(d => fundReceivedToday.Any(dis => dis.Id == d.FundConfirmationId && dis.Application.Product.DepositType == DepositType.Recurring)).Sum(d => d.Amount);

                if (fundReceivedLastday.Where(d => d.Application.Product.DepositType == DepositType.Recurring).Count() > 0)
                    data.FundReceived_RecurrentAmountLastday = (decimal)fundConfirmdetails.Where(d => fundReceivedLastday.Any(dis => dis.Id == d.FundConfirmationId && dis.Application.Product.DepositType == DepositType.Recurring)).Sum(d => d.Amount);
                #endregion Disbursed/Received
            }
            else
            {
                #region Call
                data.Call_HomeLoanToDay = callListTodayHome.Count;
                data.Call_HomeLoanLastDay = callListLastHome.Count;

                data.Call_AutoLoanToDay = callListTodayAuto.Count;
                data.Call_AutoLoanLastDay = callListLastAuto.Count;

                data.Call_PersonalLoanToDay = callListTodayPersonal.Count;
                data.Call_PersonalLoanLastDay = callListLastPersonal.Count;

                data.Call_FixedDepositToDay = callListTodayFixedDeposit.Count;
                data.Call_FixedDepositLastDay = callListLastFixedDeposit.Count;

                data.Call_RecurrentDepositToDay = callListTodayRecurrentDeposit.Count;
                data.Call_RecurrentDepositLastDay = callListLastRecurrentDeposit.Count;

                data.Call_UndefinedToDay = callListTodayUndefined.Count;
                data.Call_UndefinedLastDay = callListLastUndefined.Count;
                #endregion Call

                #region Lead
                data.Lead_HomeLoanToDay = leadListTodayHome.Count;
                data.Lead_HomeLoanLastDay = leadListLastHome.Count;

                data.Lead_AutoLoanToDay = leadListTodayAuto.Count;
                data.Lead_AutoLoanLastDay = leadListLastAuto.Count;

                data.Lead_PersonalLoanToDay = leadListTodayPersonal.Count;
                data.Lead_PersonalLoanLastDay = leadListLastPersonal.Count;

                data.Lead_FixedDepositToDay = leadListTodayFixedDeposit.Count;
                data.Lead_FixedDepositLastDay = leadListLastFixedDeposit.Count;

                data.Lead_RecurrentDepositToDay = leadListTodayRecurrentDeposit.Count;
                data.Lead_RecurrentDepositLastDay = leadListLastRecurrentDeposit.Count;
                #endregion Lead

                #region File Submited Apps
                data.Application_HomeLoanToDay = applicationListTodayHome.Count;
                data.Application_HomeLoanLastDay = applicationListLastHome.Count;

                data.Application_AutoLoanToDay = applicationListTodayAuto.Count;
                data.Application_AutoLoanLastDay = applicationListLastAuto.Count;

                data.Application_PersonalLoanToDay = applicationListTodayPersonal.Count;
                data.Application_PersonalLoanLastDay = applicationListLastPersonal.Count;

                data.Application_FixedDepositToDay = applicationListTodayFixedDeposit.Count;
                data.Application_FixedDepositLastDay = applicationListLastFixedDeposit.Count;

                data.Application_RecurrentDepositToDay = applicationListTodayRecurrentDeposit.Count;
                data.Application_RecurrentDepositLastDay = applicationListLastRecurrentDeposit.Count;
                #endregion File Submited Apps

                #region File Approved Apps
                data.Approved_HomeLoanToDay = approvedApplicationListTodayHome.Count;
                data.Approved_HomeLoanLastDay = approvedApplicationListLastHome.Count;

                data.Approved_AutoLoanToDay = approvedApplicationListTodayAuto.Count;
                data.Approved_AutoLoanLastDay = approvedApplicationListLastAuto.Count;

                data.Approved_PersonalLoanToDay = approvedApplicationListTodayPersonal.Count;
                data.Approved_PersonalLoanLastDay = approvedApplicationListLastPersonal.Count;

                #endregion File Approved Apps

                #region File DisApproved Apps
                data.DisApproved_HomeLoanToDay = disApprovedApplicationListTodayHome.Count;
                data.DisApproved_HomeLoanLastDay = disApprovedApplicationListLastHome.Count;

                data.DisApproved_AutoLoanToDay = disApprovedApplicationListTodayAuto.Count;
                data.DisApproved_AutoLoanLastDay = disApprovedApplicationListLastAuto.Count;

                data.DisApproved_PersonalLoanToDay = disApprovedApplicationListTodayPersonal.Count;
                data.DisApproved_PersonalLoanLastDay = disApprovedApplicationListLastPersonal.Count;

                #endregion File DisApproved Apps

                #region Disbursed/Received

                data.Disbursed_HomeLoanToDay = disbursementMemosToday.Where(d => d.Application.Product.FacilityType == ProposalFacilityType.Home_Loan).Count();
                data.Disbursed_HomeLoanLastDay = disbursementMemosLastday.Where(d => d.Application.Product.FacilityType == ProposalFacilityType.Home_Loan).Count();

                data.Disbursed_AutoLoanToDay = disbursementMemosToday.Where(d => d.Application.Product.FacilityType == ProposalFacilityType.Auto_Loan).Count();
                data.Disbursed_AutoLoanLastDay = disbursementMemosLastday.Where(d => d.Application.Product.FacilityType == ProposalFacilityType.Auto_Loan).Count();

                data.Disbursed_PersonalLoanToDay = disbursementMemosToday.Where(d => d.Application.Product.FacilityType == ProposalFacilityType.Personal_Loan).Count();
                data.Disbursed_PersonalLoanLastDay = disbursementMemosLastday.Where(d => d.Application.Product.FacilityType == ProposalFacilityType.Personal_Loan).Count();

                data.Received_FixedToDay = fundReceivedToday.Where(d => d.Application.Product.DepositType == DepositType.Fixed).Count();
                data.Received_FixedLastDay = fundReceivedLastday.Where(d => d.Application.Product.DepositType == DepositType.Fixed).Count();

                data.Received_RecurrentToDay = fundReceivedToday.Where(d => d.Application.Product.DepositType == DepositType.Recurring).Count();
                data.Received_RecurrentLastDay = fundReceivedLastday.Where(d => d.Application.Product.DepositType == DepositType.Recurring).Count();

                #endregion Disbursed/Received
            }

            return data;
        }

        public object GetMDDashboardHighlightsRight(TimeLine? timeLine, List<long?> branchId)
        {
            var data = new DashBoardHighlightsDto();
            DateRangeDto dateRange;
            if (timeLine == null)
                dateRange = new DateRangeDto();
            else
                dateRange = new DateRangeDto((TimeLine)timeLine);

            var disbursementMemos = new List<DMDetail>();
            var disMemo = new List<DisbursementMemo>();
            var fundReceived = new List<FundConfirmationDetail>();
            var fundRec = new List<FundConfirmation>();

            //long userId = 0;

            #region WithBranch Pie Chart
            if (branchId != null && branchId.Count > 0)
            {
                #region File Submited Apps.
                disbursementMemos = GenService.GetAll<DMDetail>().Where(d => d.DisbursementMemo.ApplicationId != null
                            && d.DisbursementMemo.Application.BranchId != null
                            && branchId.Contains(d.DisbursementMemo.Application.BranchId)
                            && d.DisbursementMemo.Application.LoanApplicationId != null
                            && d.DisbursementMemo.Application.Product.FacilityType != null
                            && (d.DisbursementMemo.Application.Product.FacilityType == ProposalFacilityType.Home_Loan
                                || d.DisbursementMemo.Application.Product.FacilityType == ProposalFacilityType.Auto_Loan
                                || d.DisbursementMemo.Application.Product.FacilityType == ProposalFacilityType.Personal_Loan)
                            && d.CreateDate <= dateRange.ToDate
                            && d.CreateDate >= dateRange.FromDate
                            && d.DisbursementMemo.IsApproved != null
                            && d.DisbursementMemo.IsApproved == true
                            && d.DisbursementMemo.IsDisbursed != null
                            && d.DisbursementMemo.IsDisbursed == true
                            && d.Status == EntityStatus.Active).ToList();
                var acceptableDMids = disbursementMemos.Select(dm => dm.DisbursementMemoId).ToList();
                disMemo = GenService.GetAll<DisbursementMemo>().Where(d => acceptableDMids.Contains(d.Id)).ToList();

                fundReceived = GenService.GetAll<FundConfirmationDetail>().Where(d => d.FundConfirmation.ApplicationId != null
                            && d.FundConfirmation.Application.BranchId != null
                            && branchId.Contains(d.FundConfirmation.Application.BranchId)
                            && d.FundConfirmation.Application.DepositApplicationId != null
                            && d.FundConfirmation.Application.Product.DepositType != null
                            && (d.FundConfirmation.Application.Product.DepositType == DepositType.Fixed
                                || d.FundConfirmation.Application.Product.DepositType == DepositType.Recurring)
                            && d.CreateDate <= dateRange.ToDate
                            && d.CreateDate >= dateRange.FromDate
                            && d.FundConfirmation.FundReceived != null
                            && d.FundConfirmation.FundReceived == true
                            && d.Status == EntityStatus.Active).ToList();
                var acceptableFCids = fundReceived.Select(dm => dm.FundConfirmationId).ToList();
                fundRec = GenService.GetAll<FundConfirmation>().Where(d => acceptableFCids.Contains(d.Id)).ToList();
                #endregion File Submited Apps.
            }
            #endregion WithBranch
            #region WithoutBrach Pie Chart
            else
            {
                disbursementMemos = GenService.GetAll<DMDetail>().Where(d => d.DisbursementMemo.ApplicationId != null

                            && d.DisbursementMemo.Application.LoanApplicationId != null
                            && d.DisbursementMemo.Application.Product.FacilityType != null
                            && (d.DisbursementMemo.Application.Product.FacilityType == ProposalFacilityType.Home_Loan
                                || d.DisbursementMemo.Application.Product.FacilityType == ProposalFacilityType.Auto_Loan
                                || d.DisbursementMemo.Application.Product.FacilityType == ProposalFacilityType.Personal_Loan)
                            && d.CreateDate <= dateRange.ToDate
                            && d.CreateDate >= dateRange.FromDate
                            && d.DisbursementMemo.IsApproved != null
                            && d.DisbursementMemo.IsApproved == true
                            && d.DisbursementMemo.IsDisbursed != null
                            && d.DisbursementMemo.IsDisbursed == true
                            && d.Status == EntityStatus.Active).ToList();
                var acceptableDMids = disbursementMemos.Select(dm => dm.DisbursementMemoId).ToList();
                disMemo = GenService.GetAll<DisbursementMemo>().Where(d => acceptableDMids.Contains(d.Id)).ToList();
                fundReceived = GenService.GetAll<FundConfirmationDetail>().Where(d => d.FundConfirmation.ApplicationId != null
                            && d.FundConfirmation.Application.DepositApplicationId != null
                            && d.FundConfirmation.Application.Product.DepositType != null
                            && (d.FundConfirmation.Application.Product.DepositType == DepositType.Fixed
                                || d.FundConfirmation.Application.Product.DepositType == DepositType.Recurring)
                            && d.CreateDate <= dateRange.ToDate
                            && d.CreateDate >= dateRange.FromDate
                            && d.FundConfirmation.FundReceived != null
                            && d.FundConfirmation.FundReceived == true
                            && d.Status == EntityStatus.Active).ToList();
                var acceptableFCids = fundReceived.Select(dm => dm.FundConfirmationId).ToList();
                fundRec = GenService.GetAll<FundConfirmation>().Where(d => acceptableFCids.Contains(d.Id)).ToList();

            }
            #endregion WithoutBrach Pie Chart

            data.AssetCount = disMemo.Count;
            data.LiabilityCount = fundRec.Count;

            return data;
        }

        public object GetMDDashboardHighlightsRightBranch(TimeLine? timeLine)
        {
            var data = new DashBoardHighlightsDto();
            DateRangeDto dateRange;
            if (timeLine == null)
                dateRange = new DateRangeDto();
            else
                dateRange = new DateRangeDto((TimeLine)timeLine);

            data.BusinessConAsset = new List<BusinessContributionUnitDto>();
            data.BusinessConLiability = new List<BusinessContributionUnitDto>();

            var disbursedLoansBr = GenService.GetAll<DMDetail>()
                .Where(d => d.DisbursementMemo.ApplicationId != null && d.DisbursementMemo.Application.BranchId != null
                            && d.DisbursementMemo.Application.LoanApplicationId != null
                            && d.DisbursementMemo.Application.Product.FacilityType != null
                            && (d.DisbursementMemo.Application.Product.FacilityType == ProposalFacilityType.Home_Loan
                                || d.DisbursementMemo.Application.Product.FacilityType == ProposalFacilityType.Auto_Loan
                                || d.DisbursementMemo.Application.Product.FacilityType == ProposalFacilityType.Personal_Loan)
                            && d.CreateDate <= dateRange.ToDate
                            && d.CreateDate >= dateRange.FromDate
                            && d.DisbursementMemo.IsApproved != null
                            && d.DisbursementMemo.IsApproved == true
                            && d.DisbursementMemo.IsDisbursed != null
                            && d.DisbursementMemo.IsDisbursed == true
                            && d.Status == EntityStatus.Active).GroupBy(d => d.DisbursementMemo.Application.BranchId)
                .Select(d => new BusinessContributionUnitDto
                {
                    BranchId = d.Key,
                    BranchName = d.FirstOrDefault().DisbursementMemo.Application.BranchOffice.Name,
                    ProductType = d.FirstOrDefault().DisbursementMemo.Application.Product.ProductType,
                    Count = d.Count(),
                    Amount = d.Sum(e => e.DisbursementAmount)
                }).ToList();
            if (disbursedLoansBr != null)

                data.BusinessConAsset = disbursedLoansBr;

            var disbursedDepositsBr = GenService.GetAll<FundConfirmationDetail>()
                .Where(d => d.FundConfirmation.ApplicationId != null && d.FundConfirmation.Application.BranchId != null
                            && d.FundConfirmation.Application.DepositApplicationId != null
                            && d.FundConfirmation.Application.Product.DepositType != null
                            && (d.FundConfirmation.Application.Product.DepositType == DepositType.Fixed
                                || d.FundConfirmation.Application.Product.DepositType == DepositType.Recurring)
                            && d.CreateDate <= dateRange.ToDate
                            && d.CreateDate >= dateRange.FromDate
                            && d.FundConfirmation.FundReceived != null
                            && d.FundConfirmation.FundReceived == true
                            && d.Status == EntityStatus.Active).GroupBy(d => d.FundConfirmation.Application.BranchId)
                .Select(d => new BusinessContributionUnitDto
                {
                    BranchId = d.Key,
                    BranchName = d.FirstOrDefault().FundConfirmation.Application.BranchOffice.Name,
                    ProductType = d.FirstOrDefault().FundConfirmation.Application.Product.ProductType,
                    Count = d.Count(),
                    Amount = d.Sum(e => e.Amount)
                }).ToList();

            if (disbursedDepositsBr != null)
                data.BusinessConLiability = disbursedDepositsBr;
            return data;
        }

        public List<ActivitySummaryBMDto> GetActivitySummaryOfBm(long userId, long? productId, TimeLine? timeLine)
        {
            #region data population
            long employeeId = _user.GetEmployeeIdByUserId(userId);
            var subordinateEmpList = _employee.GetEmployeeWiseSubordinate(employeeId);
            List<ActivitySummaryBMDto> SubordinateOfBM = new List<ActivitySummaryBMDto>();
            var subordinateEmpIdList = subordinateEmpList.Where(s => s.EmployeeId != null).Select(s => s.EmployeeId).ToList();

            //var allApplications = GenService.GetAll<Application>().Where(a => subordinateEmpIdList.Contains(a.RMId));
            if (subordinateEmpList.Any())
            {
                foreach (var officeDesignationSettingDto in subordinateEmpList)
                {
                    var subList = _employee.GetEmployeeWiseSubordinate((long)officeDesignationSettingDto.EmployeeId);
                    var secondStep = GetEmployeeActivity(officeDesignationSettingDto, subList, timeLine, productId);
                    SubordinateOfBM.AddRange(secondStep);
                }
            }
            #endregion
            return SubordinateOfBM;
        }

        public List<ActivitySummaryBMDto> GetEmployeeActivity(OfficeDesignationSettingDto parent, List<OfficeDesignationSettingDto> subordinateList, TimeLine? timeLine, long? productId)
        {
            DateRangeDto dateRange;
            if (timeLine == null)
                dateRange = new DateRangeDto();
            else
                dateRange = new DateRangeDto((TimeLine)timeLine);
            var activity = new List<ActivitySummaryBMDto>();
            //activity.ParentEmployeeId = parentId;
            //activity.Children = new List<ActivitySummaryBMDto>();
            var subordinateEmpIdList = subordinateList.Where(s => s.EmployeeId != null).Select(s => s.EmployeeId).ToList();
            var allApplications = GenService.GetAll<Application>().Where(a => a.RMId == parent.EmployeeId);
            if (productId != null)
            {
                allApplications = allApplications.Where(p => p.ProductId == productId);
            }
            var subordinateUserList = new List<long>();
            foreach (var sub in subordinateList.Where(s => s.EmployeeId != null))
            {
                subordinateUserList.Add((long)_user.GetUserByEmployeeId((long)sub.EmployeeId).Id);
            }
            var user = _user.GetUserByEmployeeId((long)parent.EmployeeId);
            long offDegSettingId = _employee.GetOfficeDesignationIdOfEmployee((long)parent.EmployeeId);
            long assigned = 0;
            long uId = 0;
            if (offDegSettingId > 0)
            {
                assigned = offDegSettingId;
            }
            if (user.Id != null)
            {
                uId = (long)user.Id;
            }
            List<long> existingthanaIds = new List<long>();
            var thanaOfEmployee = _employee.GetThanaOfEmployee((long)parent.EmployeeId);
            if (thanaOfEmployee.Any())
            {
                existingthanaIds = thanaOfEmployee.Select(d => d.Id).ToList();
            }
            //var isSubordinateExist = subordinateList.Where(r => r.ParentEmployeeId != parentId && r.ParentEmployeeId == (long)officeDesignationSettingDto.EmployeeId);
            //if (isSubordinateExist.Any())
            //{
            //    var secondStep = GetEmployeeActivity((long)officeDesignationSettingDto.EmployeeId, subList, timeLine);
            //    activity.AddRange(secondStep);
            //}
            var aSubordinate = new ActivitySummaryBMDto();
            aSubordinate.EmployeeId = parent.EmployeeId;
            aSubordinate.Name = parent.Name;
            aSubordinate.ParentEmployeeId = parent.ParentEmployeeId;
            aSubordinate.ParentEmployeeName = parent.ParentEmployeeName;
            #region lead data population
            IQueryable<SalesLead> allLeads = GenService.GetAll<SalesLead>()
        .Where(s => s.Status == EntityStatus.Active && s.CreateDate <= dateRange.ToDate && s.CreateDate >= dateRange.FromDate);
            var leadIds = GenService.GetAll<SalesLeadAssignment>()
               .Where(s => s.AssignedToEmpId == parent.EmployeeId && //subordinateEmpIdList.Contains(s.AssignedToEmpId) &&
               s.Status == EntityStatus.Active).Select(s => s.SalesLeadId).ToList();
            var notAcceptableLeadIds = allApplications.Where(r => r.RMId == aSubordinate.EmployeeId)
                .Where(s => s.ApplicationStage < ApplicationStage.Drafted && s.ApplicationStage > ApplicationStage.SentToBM)
                .Select(s => s.SalesLeadId);
            var assignedLeads = allLeads
            .Where(s => leadIds.Contains(s.Id) &&
                !notAcceptableLeadIds.Contains(s.Id) &&
                s.Status == EntityStatus.Active &&
                (s.LeadStatus == LeadStatus.Drafted || s.LeadStatus == LeadStatus.Prospective));
            if (productId != null)
            {
                assignedLeads = assignedLeads.Where(r => r.ProductId == productId);
            }
            aSubordinate.LeadSubmittedQty = assignedLeads.Count();
            aSubordinate.LeadSubmittedAmt = assignedLeads.Sum(r => r.Amount);
            #endregion
            //#region Call 
            ////-todo error in call generation
            //var callsInProgress = GenService.GetAll<Call>().Where(s => s.Status == EntityStatus.Active &&
            //                                                      s.CallStatus == CallStatus.Unfinished &&
            //                                                      ((s.CallType == CallType.Self && s.CreatedBy == uId) ||
            //                                                       (s.CallType != CallType.Self && s.AssignedTo == assigned)));

            //#endregion
            #region call data population
            IQueryable<Call> allCalls = GenService.GetAll<Call>().Where(c => c.CreateDate <= dateRange.ToDate && c.CreateDate >= dateRange.FromDate && c.Status == EntityStatus.Active);
            var ct = allCalls.Count();
            IQueryable<Call> callQueryableAreaWise = allCalls
                .Where(s => s.Status == EntityStatus.Active &&
                            s.CallStatus == CallStatus.Unfinished &&
                            s.CallType == CallType.Auto_assign &&
                            existingthanaIds.Contains((long)s.CustomerAddress.ThanaId));
            IQueryable<Call> callQueryableAsReferred = allCalls
                .Where(c => c.Status == EntityStatus.Active &&
                            c.CallStatus == CallStatus.Unfinished &&
                            c.ReferredTo == assigned);
            IQueryable<Call> rmSelfGeneratedCalls = allCalls
                .Where(c => c.Status == EntityStatus.Active &&
                            c.CallType == CallType.Self &&
                            c.CreatedBy == uId); //subordinateUserList.Contains((long)c.CreatedBy
            var allCallsUnited = callQueryableAreaWise.Union(callQueryableAsReferred).Union(rmSelfGeneratedCalls).Distinct();
            if (productId != null)
            {
                allCallsUnited = allCallsUnited.Where(r => r.ProductId == productId);
            }
            aSubordinate.CallQty = allCallsUnited.Count();
            #endregion
            #region application data population
            var submittedApplications = allApplications
                    .Where(a => (((a.ApplicationStage == ApplicationStage.SentToCRM || a.ApplicationStage == ApplicationStage.UnderProcessAtCRM) && a.Product.ProductType == ProductType.Loan) ||
                                ((a.ApplicationStage == ApplicationStage.SentToOperations || a.ApplicationStage == ApplicationStage.UnderProcessAtOperations) && a.Product.ProductType == ProductType.Deposit)));
            var submittedApplicationIds = submittedApplications.Select(s => s.Id).ToList();
            if (submittedApplicationIds.Count > 0)
            {
                var acceptableLogs = GenService.GetAll<ApplicationLog>()
                    .Where(l => l.Status == EntityStatus.Active &&
                    submittedApplicationIds.Contains(l.ApplicationId) &&
                    l.CreateDate <= dateRange.ToDate &&
                    l.CreateDate >= dateRange.FromDate &&
                    (((l.ToStage == ApplicationStage.SentToCRM || l.ToStage == ApplicationStage.UnderProcessAtCRM) &&
                     l.Application.Product.ProductType == ProductType.Loan) ||
                    ((l.ToStage == ApplicationStage.SentToOperations || l.ToStage == ApplicationStage.UnderProcessAtOperations) &&
     l.Application.Product.ProductType == ProductType.Deposit))).ToList();
                var submittedApplicationsWithLog = (from appLog in acceptableLogs
                                                    orderby appLog.Id
                                                    select new
                                                    {
                                                        ApplicationId = appLog.ApplicationId,
                                                        Product = appLog.Application.Product,
                                                        Amount = appLog.Application.LoanApplication.LoanAmountApplied
                                                    }).Distinct();
                if (submittedApplicationsWithLog != null)
                {
                    aSubordinate.FileSubmittedQty = submittedApplicationsWithLog.Count();
                    aSubordinate.FileSubmittedAmt = 0;
                    aSubordinate.FileSubmittedAmt = submittedApplicationsWithLog.Sum(s => s.Amount);
                }
            }
            #endregion
            #region Amount disbursed / received
            if (submittedApplicationIds.Count > 0)
            {
                var disbursementMemos = GenService.GetAll<DisbursementMemo>().Where(d => d.ApplicationId != null &&
                                                                                         submittedApplicationIds.Contains((long)d.ApplicationId) &&
                                                                                         d.Status == EntityStatus.Active &&
                                                                                         d.IsApproved != null &&
                                                                                         d.IsApproved == true &&
                                                                                         d.IsDisbursed != null &&
                                                                                         d.IsDisbursed == true &&
                                                                                         d.DisbursementDetails.FirstOrDefault().CreateDate <= dateRange.ToDate &&
                                                                                         d.DisbursementDetails.FirstOrDefault().CreateDate >= dateRange.FromDate);
                if (disbursementMemos.Count() > 0)
                {
                    aSubordinate.FileDisbursedQty = disbursementMemos.Count();
                    aSubordinate.FileDisbursedAmt = disbursementMemos.Sum(t => t.CurrentDisbursementAmount);
                }
            }
            #endregion
            #region approved/disapproved

            var loanApplicationsIds = allApplications.Where(a => a.Product.ProductType == ProductType.Loan && a.RMId == aSubordinate.EmployeeId).Select(a => a.Id);
            var disapporvedLoanApplicationLogs = GenService.GetAll<ApplicationLog>()
                .Where(l => l.Status == EntityStatus.Active &&
                            l.ToStage < ApplicationStage.Drafted &&
                            loanApplicationsIds.Contains(l.ApplicationId) &&
                            l.CreateDate <= dateRange.ToDate &&
                            l.CreateDate >= dateRange.FromDate)
                .OrderBy(l => l.Id)
                .Select(l => new { ApplicationId = l.ApplicationId, Product = l.Application.Product, Amount = l.Application.LoanApplication.LoanAmountApplied })
                .Distinct();
            var disapprovedLoanApplicationLogsWithAmount = (from logs in disapporvedLoanApplicationLogs
                                                            join proposal in GenService.GetAll<Proposal>().Where(p => p.Status == EntityStatus.Active) on logs.ApplicationId equals proposal.ApplicationId into proposals
                                                            from p in proposals.DefaultIfEmpty()
                                                            select new
                                                            {
                                                                ApplicationId = logs.ApplicationId,
                                                                Product = logs.Product,
                                                                Amount = p != null && p.RecomendedLoanAmountFromIPDC != null ? p.RecomendedLoanAmountFromIPDC : logs.Amount
                                                            }).Distinct();
            aSubordinate.FileRejectedQty = disapprovedLoanApplicationLogsWithAmount.Count();
            aSubordinate.FileRejectedAmt = disapprovedLoanApplicationLogsWithAmount.Sum(l => l.Amount);
            #endregion
            activity.Add(aSubordinate);
            //var dat = activity.Where(r => r.EmployeeId == aSubordinate.ParentEmployeeId);
            //activity.Where(r => r.EmployeeId == aSubordinate.ParentEmployeeId).ForEach(s => s.CallQty = s.CallQty + aSubordinate.CallQty);
            //activity.Where(r => r.EmployeeId == aSubordinate.ParentEmployeeId).ForEach(s => s.LeadSubmittedQty = s.LeadSubmittedQty + aSubordinate.LeadSubmittedQty);
            //activity.Where(r => r.EmployeeId == aSubordinate.ParentEmployeeId).ForEach(s => s.LeadSubmittedAmt = s.LeadSubmittedAmt + aSubordinate.LeadSubmittedAmt);
            //activity.Where(r => r.EmployeeId == aSubordinate.ParentEmployeeId).ForEach(s => s.FileSubmittedQty = s.FileSubmittedQty + aSubordinate.FileSubmittedQty);
            //activity.Where(r => r.EmployeeId == aSubordinate.ParentEmployeeId).ForEach(s => s.FileSubmittedAmt = s.FileSubmittedAmt + aSubordinate.FileSubmittedAmt);
            //activity.Where(r => r.EmployeeId == aSubordinate.ParentEmployeeId).ForEach(s => s.FileDisbursedQty = s.FileDisbursedQty + aSubordinate.FileDisbursedQty);
            //activity.Where(r => r.EmployeeId == aSubordinate.ParentEmployeeId).ForEach(s => s.FileDisbursedAmt = s.FileDisbursedAmt + aSubordinate.FileDisbursedAmt);
            //activity.Where(r => r.EmployeeId == aSubordinate.ParentEmployeeId).ForEach(s => s.FileRejectedQty = s.FileRejectedQty + aSubordinate.FileRejectedQty);
            //activity.Where(r => r.EmployeeId == aSubordinate.ParentEmployeeId).ForEach(s => s.FileRejectedAmt = s.FileRejectedAmt + aSubordinate.FileRejectedAmt);
            var secondStep = new List<ActivitySummaryBMDto>();
            foreach (var officeDesignationSettingDto in subordinateList)
            {
                var subList = _employee.GetEmployeeWiseSubordinate((long)officeDesignationSettingDto.EmployeeId);
                secondStep = GetEmployeeActivity(officeDesignationSettingDto, subList, timeLine, productId);
                if (secondStep.Count() > 0)
                {
                    foreach (var activitySummaryBmDto in secondStep)
                    {
                        var dat = activity.Where(r => r.EmployeeId == activitySummaryBmDto.ParentEmployeeId);
                        activity.Where(r => r.EmployeeId == activitySummaryBmDto.ParentEmployeeId).ForEach(s => s.CallQty = s.CallQty + activitySummaryBmDto.CallQty);
                        activity.Where(r => r.EmployeeId == activitySummaryBmDto.ParentEmployeeId).ForEach(s => s.LeadSubmittedQty = s.LeadSubmittedQty + activitySummaryBmDto.LeadSubmittedQty);
                        activity.Where(r => r.EmployeeId == activitySummaryBmDto.ParentEmployeeId).ForEach(s => s.LeadSubmittedAmt = s.LeadSubmittedAmt != null ? s.LeadSubmittedAmt : 0 + activitySummaryBmDto.LeadSubmittedAmt);
                        activity.Where(r => r.EmployeeId == activitySummaryBmDto.ParentEmployeeId).ForEach(s => s.FileSubmittedQty = s.FileSubmittedQty != null ? s.FileSubmittedQty : 0 + activitySummaryBmDto.FileSubmittedQty);
                        activity.Where(r => r.EmployeeId == activitySummaryBmDto.ParentEmployeeId).ForEach(s => s.FileSubmittedAmt = s.FileSubmittedAmt != null ? s.FileSubmittedAmt : 0 + activitySummaryBmDto.FileSubmittedAmt);
                        activity.Where(r => r.EmployeeId == activitySummaryBmDto.ParentEmployeeId).ForEach(s => s.FileDisbursedQty = s.FileDisbursedQty != null ? s.FileDisbursedQty : 0 + activitySummaryBmDto.FileDisbursedQty);
                        activity.Where(r => r.EmployeeId == activitySummaryBmDto.ParentEmployeeId).ForEach(s => s.FileDisbursedAmt = s.FileDisbursedAmt != null ? s.FileDisbursedAmt : 0 + activitySummaryBmDto.FileDisbursedAmt);
                        activity.Where(r => r.EmployeeId == activitySummaryBmDto.ParentEmployeeId).ForEach(s => s.FileRejectedQty = s.FileRejectedQty + activitySummaryBmDto.FileRejectedQty);
                        activity.Where(r => r.EmployeeId == activitySummaryBmDto.ParentEmployeeId).ForEach(s => s.FileRejectedAmt = s.FileRejectedAmt != null ? s.FileRejectedAmt : 0 + activitySummaryBmDto.FileRejectedAmt);
                    }
                }
                activity.AddRange(secondStep);
            }
            return activity;
        }

        public LeaderBoardDto GetLeaderboardInfo(TimeLine? timeLine, long? stageId, long? criteriaId, long? centerId, long? productId, long? branchId)
        {
            var result = new LeaderBoardDto();
            DateRangeDto dateRange;
            if (timeLine == null)
                timeLine = TimeLine.Today;
            dateRange = new DateRangeDto((TimeLine)timeLine);
            TimeLine seconderyTimeLine;
            switch (timeLine)
            {
                case TimeLine.MTD:
                    seconderyTimeLine = TimeLine.LMTD;
                    break;
                case TimeLine.YTD:
                    seconderyTimeLine = TimeLine.LYTD;
                    break;
                case TimeLine.QTD:
                    seconderyTimeLine = TimeLine.LYQTD;
                    break;
                default:
                    seconderyTimeLine = TimeLine.Yesterday;
                    break;
            }
            var secondaryDateRange = new DateRangeDto(seconderyTimeLine);
            #region Call
            if (stageId != null && stageId == (long)Stage.Call)
            {
                var allCalls = GenService.GetAll<Call>();
                var employeeOffDegSet = GenService.GetAll<EmployeeDesignationMapping>();
                //var culprint = employeeOffDegSet.Where(r => r.Employee.Person == null);
                //culprint = employeeOffDegSet.Where(r => r.Employee == null);
                var allOffices = GenService.GetAll<Office>();
                result.TopFirst = new List<PerformerDto>();
                var data = (from lb in allCalls
                            join offDeg in employeeOffDegSet on lb.AssignedTo equals offDeg.OfficeDesignationSettingId
                            join ofc in allOffices on offDeg.OfficeDesignationSetting.OfficeId equals ofc.Id
                            //group lb by new { lb.ProductId, lb.AssignedTo, offDeg.EmployeeId, offDeg.Employee.Person.Name, ofc.Id, officeName = ofc.Name } into grp
                            select new PerformerDto
                            {
                                //Id = lb.Id,
                                RMId = offDeg.EmployeeId,
                                RMName = offDeg.Employee.Person.FirstName,
                                BranchId = ofc.Id,
                                BranchName = ofc.Name,
                                Number = 0,
                                OfficeDesignationSettingId = offDeg.OfficeDesignationSettingId,
                                ProductId = lb.ProductId,
                                ProductName = lb.Product != null ? lb.Product.Name : "",
                                CreateDate = lb.CreateDate,
                                CreatedBy = lb.CreatedBy,
                                Amount = lb.Amount,
                                Rate = 0,
                                WAR = 0
                            }).Where(c => c.CreateDate <= dateRange.ToDate && c.CreateDate >= dateRange.FromDate);
                if (productId != null)
                {
                    data = data.Where(r => r.ProductId == productId);

                }
                if (branchId != null)
                {
                    data = data.Where(r => r.BranchId == branchId);

                }

                result.TopFirst = (from dt in data
                                   group dt by new { dt.OfficeDesignationSettingId, dt.RMId }
                    into grp
                                   select new PerformerDto
                                   {
                                       RMId = grp.Key.RMId,
                                       RMName = grp.FirstOrDefault() != null ? grp.FirstOrDefault().RMName : null,
                                       BranchId = grp.FirstOrDefault() != null ? grp.FirstOrDefault().BranchId : null,
                                       BranchName = grp.FirstOrDefault() != null ? grp.FirstOrDefault().BranchName : "",
                                       Number = grp.Count(),
                                       OfficeDesignationSettingId = grp.Key.OfficeDesignationSettingId,
                                       ProductId = grp.FirstOrDefault() != null ? grp.FirstOrDefault().ProductId : null,
                                       ProductName = grp.FirstOrDefault() != null ? grp.FirstOrDefault().ProductName : "",
                                       CreateDate = grp.FirstOrDefault() != null ? grp.FirstOrDefault().CreateDate : DateTime.Now,
                                       CreatedBy = null,
                                       Amount = grp.Sum(r => r.Amount),
                                       Rate = 0,
                                       WAR = 0

                                   }).ToList();
                ////////////////////////Second///////////////////////////
                var dataSecondTimeline = (from lb in allCalls
                                          join offDeg in employeeOffDegSet on lb.AssignedTo equals offDeg.OfficeDesignationSettingId
                                          join ofc in allOffices on offDeg.OfficeDesignationSetting.OfficeId equals ofc.Id
                                          //group lb by new { lb.ProductId, lb.AssignedTo, offDeg.EmployeeId, offDeg.Employee.Person.Name, ofc.Id, officeName = ofc.Name } into grp
                                          select new PerformerDto
                                          {
                                              //Id = lb.Id,
                                              RMId = offDeg.EmployeeId,
                                              RMName = offDeg.Employee.Person.FirstName,
                                              BranchId = ofc.Id,
                                              BranchName = ofc.Name,
                                              Number = 0,
                                              OfficeDesignationSettingId = offDeg.OfficeDesignationSettingId,
                                              ProductId = lb.ProductId,
                                              ProductName = lb.Product != null ? lb.Product.Name : "",
                                              CreateDate = lb.CreateDate,
                                              CreatedBy = lb.CreatedBy,
                                              Amount = lb.Amount,
                                              Rate = 0,
                                              WAR = 0

                                          }).Where(c => c.CreateDate <= secondaryDateRange.ToDate && c.CreateDate >= secondaryDateRange.FromDate);
                if (productId != null)
                {
                    dataSecondTimeline = dataSecondTimeline.Where(r => r.ProductId == productId);
                }
                if (branchId != null)
                {
                    dataSecondTimeline = dataSecondTimeline.Where(r => r.BranchId == branchId);
                }
                result.TopSecond = new List<PerformerDto>();
                result.TopSecond = (from dt in dataSecondTimeline
                                    group dt by new { dt.OfficeDesignationSettingId, dt.RMId }
                    into grp
                                    select new PerformerDto
                                    {
                                        RMId = grp.Key.RMId,
                                        RMName = grp.FirstOrDefault() != null ? grp.FirstOrDefault().RMName : null,
                                        BranchId = grp.FirstOrDefault() != null ? grp.FirstOrDefault().BranchId : null,
                                        BranchName = grp.FirstOrDefault() != null ? grp.FirstOrDefault().BranchName : "",
                                        Number = grp.Count(),
                                        OfficeDesignationSettingId = grp.Key.OfficeDesignationSettingId,
                                        ProductId = grp.FirstOrDefault() != null ? grp.FirstOrDefault().ProductId : null,
                                        ProductName = grp.FirstOrDefault() != null ? grp.FirstOrDefault().ProductName : "",
                                        CreateDate = grp.FirstOrDefault() != null ? grp.FirstOrDefault().CreateDate : DateTime.Now,
                                        CreatedBy = null,
                                        Amount = grp.Sum(r => r.Amount),
                                        Rate = 0,
                                        WAR = 0

                                    }).ToList();
                if (criteriaId != null && criteriaId == (long)Criteria.Number)
                {
                    result.TopFirst = result.TopFirst.OrderByDescending(r => r.Number).Take(10).ToList();
                    result.BottomFirst = result.TopFirst.OrderBy(r => r.Number).Take(10).ToList();
                    result.TopSecond = result.TopSecond.OrderByDescending(r => r.Number).Take(10).ToList();
                    result.BottomSecond = result.TopSecond.OrderBy(r => r.Number).Take(10).ToList();
                }
                if (criteriaId != null && criteriaId == (long)Criteria.Amount)
                {
                    result.TopFirst = result.TopFirst.OrderByDescending(r => r.Amount).Take(10).ToList();
                    result.BottomFirst = result.TopFirst.OrderBy(r => r.Amount).Take(10).ToList();
                    result.TopSecond = result.TopSecond.OrderByDescending(r => r.Amount).Take(10).ToList();
                    result.BottomSecond = result.TopSecond.OrderBy(r => r.Amount).Take(10).ToList();
                }
            }
            #endregion
            #region Lead 

            if (stageId != null && stageId == (long)Stage.Lead)
            {
                var allLeads = GenService.GetAll<SalesLead>();
                //var allCalls = GenService.GetAll<Call>();
                var leadAssignment = GenService.GetAll<SalesLeadAssignment>();
                var employeeOffDegSet = GenService.GetAll<EmployeeDesignationMapping>();
                //var culprint = employeeOffDegSet.Where(r => r.Employee.Person == null);
                //culprint = employeeOffDegSet.Where(r => r.Employee == null);
                var allOffices = GenService.GetAll<Office>();
                result.TopFirst = new List<PerformerDto>();
                var data = (from ld in allLeads
                            join asgn in leadAssignment on ld.Id equals asgn.SalesLeadId
                            join offDeg in employeeOffDegSet on asgn.AssignedToEmpId equals offDeg.EmployeeId
                            join ofc in allOffices on offDeg.OfficeDesignationSetting.OfficeId equals ofc.Id
                            //group lb by new { lb.ProductId, lb.AssignedTo, offDeg.EmployeeId, offDeg.Employee.Person.Name, ofc.Id, officeName = ofc.Name } into grp
                            select new PerformerDto
                            {
                                //Id = lb.Id,
                                RMId = offDeg.EmployeeId,
                                RMName = offDeg.Employee.Person.FirstName,
                                BranchId = ofc.Id,
                                BranchName = ofc.Name,
                                Number = 0,
                                OfficeDesignationSettingId = offDeg.OfficeDesignationSettingId,
                                ProductId = ld.ProductId,
                                ProductName = ld.Product != null ? ld.Product.Name : "",
                                CreateDate = ld.CreateDate,
                                CreatedBy = ld.CreatedBy,
                                Amount = ld.Amount,
                                WAR = 0,
                                Rate = 0
                            }).Where(c => c.CreateDate <= dateRange.ToDate && c.CreateDate >= dateRange.FromDate);

                if (productId != null)
                {
                    data = data.Where(r => r.ProductId == productId);
                }
                if (branchId != null)
                {
                    data = data.Where(r => r.BranchId == branchId);
                }
                result.TopFirst = (from dt in data
                                   group dt by new { dt.OfficeDesignationSettingId, dt.RMId }
                    into grp
                                   select new PerformerDto
                                   {
                                       RMId = grp.Key.RMId,
                                       RMName = grp.FirstOrDefault() != null ? grp.FirstOrDefault().RMName : null,
                                       BranchId = grp.FirstOrDefault() != null ? grp.FirstOrDefault().BranchId : null,
                                       BranchName = grp.FirstOrDefault() != null ? grp.FirstOrDefault().BranchName : "",
                                       Number = grp.Count(),
                                       OfficeDesignationSettingId = grp.Key.OfficeDesignationSettingId,
                                       ProductId = grp.FirstOrDefault() != null ? grp.FirstOrDefault().ProductId : null,
                                       ProductName = grp.FirstOrDefault() != null ? grp.FirstOrDefault().ProductName : "",
                                       CreateDate = grp.FirstOrDefault() != null ? grp.FirstOrDefault().CreateDate : DateTime.Now,
                                       CreatedBy = null,
                                       Amount = grp.Sum(r => r.Amount),
                                       WAR = 0,
                                       Rate = 0
                                   }).ToList();
                ////////////////////////Second///////////////////////////
                var dataSecondTimeline = (from ld in allLeads
                                          join asgn in leadAssignment on ld.Id equals asgn.SalesLeadId
                                          join offDeg in employeeOffDegSet on asgn.AssignedToEmpId equals offDeg.EmployeeId
                                          join ofc in allOffices on offDeg.OfficeDesignationSetting.OfficeId equals ofc.Id
                                          select new PerformerDto
                                          {
                                              //Id = lb.Id,
                                              RMId = offDeg.EmployeeId,
                                              RMName = offDeg.Employee.Person.FirstName,
                                              BranchId = ofc.Id,
                                              BranchName = ofc.Name,
                                              Number = 0,
                                              OfficeDesignationSettingId = offDeg.OfficeDesignationSettingId,
                                              ProductId = ld.ProductId,
                                              ProductName = ld.Product != null ? ld.Product.Name : "",
                                              CreateDate = ld.CreateDate,
                                              CreatedBy = ld.CreatedBy,
                                              Amount = ld.Amount,
                                              WAR = 0,
                                              Rate = 0

                                          }).Where(c => c.CreateDate <= secondaryDateRange.ToDate && c.CreateDate >= secondaryDateRange.FromDate);
                if (productId != null)
                {
                    dataSecondTimeline = dataSecondTimeline.Where(r => r.ProductId == productId);
                }
                if (branchId != null)
                {
                    dataSecondTimeline = dataSecondTimeline.Where(r => r.BranchId == branchId);
                }
                result.TopSecond = new List<PerformerDto>();
                result.TopSecond = (from dt in dataSecondTimeline
                                    group dt by new { dt.OfficeDesignationSettingId, dt.RMId }
                    into grp
                                    select new PerformerDto
                                    {
                                        RMId = grp.Key.RMId,
                                        RMName = grp.FirstOrDefault() != null ? grp.FirstOrDefault().RMName : null,
                                        BranchId = grp.FirstOrDefault() != null ? grp.FirstOrDefault().BranchId : null,
                                        BranchName = grp.FirstOrDefault() != null ? grp.FirstOrDefault().BranchName : "",
                                        Number = grp.Count(),
                                        OfficeDesignationSettingId = grp.Key.OfficeDesignationSettingId,
                                        ProductId = grp.FirstOrDefault() != null ? grp.FirstOrDefault().ProductId : null,
                                        ProductName = grp.FirstOrDefault() != null ? grp.FirstOrDefault().ProductName : "",
                                        CreateDate = grp.FirstOrDefault() != null ? grp.FirstOrDefault().CreateDate : DateTime.Now,
                                        CreatedBy = null,
                                        Amount = grp.Sum(r => r.Amount),
                                        WAR = 0,
                                        Rate = 0
                                    }).ToList();
                if (criteriaId != null && criteriaId == (long)Criteria.Number)
                {
                    result.TopFirst = result.TopFirst.OrderByDescending(r => r.Number).Take(10).ToList();
                    result.BottomFirst = result.TopFirst.OrderBy(r => r.Number).Take(10).ToList();
                    result.TopSecond = result.TopSecond.OrderByDescending(r => r.Number).Take(10).ToList();
                    result.BottomSecond = result.TopSecond.OrderBy(r => r.Number).Take(10).ToList();
                }
                if (criteriaId != null && criteriaId == (long)Criteria.Amount)
                {
                    result.TopFirst = result.TopFirst.OrderByDescending(r => r.Amount).Take(10).ToList();
                    result.BottomFirst = result.TopFirst.OrderBy(r => r.Amount).Take(10).ToList();
                    result.TopSecond = result.TopSecond.OrderByDescending(r => r.Amount).Take(10).ToList();
                    result.BottomSecond = result.TopSecond.OrderBy(r => r.Amount).Take(10).ToList();
                }
            }

            #endregion
            #region Application
            if (stageId != null && stageId == (long)Stage.Application)
            {
                var allApplication = GenService.GetAll<Application>().Where(a => (((a.ApplicationStage == ApplicationStage.SentToCRM || a.ApplicationStage == ApplicationStage.UnderProcessAtCRM) && a.Product.ProductType == ProductType.Loan) ||
                            ((a.ApplicationStage == ApplicationStage.SentToOperations || a.ApplicationStage == ApplicationStage.UnderProcessAtOperations) && a.Product.ProductType == ProductType.Deposit)));

                if (allApplication.Any() && centerId > 0)
                {
                    allApplication = allApplication.Where(r => r.CostCenterId == centerId);
                }
                var employeeOffDegSet = GenService.GetAll<EmployeeDesignationMapping>();
                var allOffices = GenService.GetAll<Office>();
                result.TopFirst = new List<PerformerDto>();
                var data = (from app in allApplication
                            join offDeg in employeeOffDegSet on app.RMId equals offDeg.EmployeeId
                            join ofc in allOffices on offDeg.OfficeDesignationSetting.OfficeId equals ofc.Id
                            select new PerformerDto
                            {
                                RMId = offDeg.EmployeeId,
                                RMName = offDeg.Employee.Person.FirstName,
                                BranchId = ofc.Id,
                                BranchName = ofc.Name,
                                Number = 0,
                                OfficeDesignationSettingId = offDeg.OfficeDesignationSettingId,
                                ProductId = app.ProductId,
                                ProductName = app.Product != null ? app.Product.Name : "",
                                CreateDate = app.CreateDate,
                                CreatedBy = app.CreatedBy,
                                Amount = app.LoanApplicationId > 0 ? app.LoanApplication.LoanAmountApplied : app.DepositApplicationId > 0 ? app.DepositApplication.TotalDepositAmount : 0,
                                Rate = 0,
                                WAR = 0
                            }).Where(c => c.CreateDate <= dateRange.ToDate && c.CreateDate >= dateRange.FromDate);

                if (productId != null)
                {
                    data = data.Where(r => r.ProductId == productId);
                }
                if (branchId != null)
                {
                    data = data.Where(r => r.BranchId == branchId);
                }

                result.TopFirst = (from dt in data
                                   group dt by new { dt.OfficeDesignationSettingId, dt.RMId }
                                   into grp
                                   select new PerformerDto
                                   {
                                       RMId = grp.Key.RMId,
                                       RMName = grp.FirstOrDefault() != null ? grp.FirstOrDefault().RMName : null,
                                       BranchId = grp.FirstOrDefault() != null ? grp.FirstOrDefault().BranchId : null,
                                       BranchName = grp.FirstOrDefault() != null ? grp.FirstOrDefault().BranchName : "",
                                       Number = grp.Count(),
                                       OfficeDesignationSettingId = grp.Key.OfficeDesignationSettingId,
                                       ProductId = grp.FirstOrDefault() != null ? grp.FirstOrDefault().ProductId : null,
                                       ProductName = grp.FirstOrDefault() != null ? grp.FirstOrDefault().ProductName : "",
                                       CreateDate = grp.FirstOrDefault() != null ? grp.FirstOrDefault().CreateDate : DateTime.Now,
                                       CreatedBy = null,
                                       Amount = grp.Sum(r => r.Amount),
                                       Rate = 0,
                                       WAR = 0
                                   }).ToList();
                ////////////////////////Second///////////////////////////
                var dataSecondTimeline = (from app in allApplication
                                          join offDeg in employeeOffDegSet on app.RMId equals offDeg.EmployeeId
                                          join ofc in allOffices on offDeg.OfficeDesignationSetting.OfficeId equals ofc.Id
                                          select new PerformerDto
                                          {
                                              //Id = lb.Id,
                                              RMId = offDeg.EmployeeId,
                                              RMName = offDeg.Employee.Person.FirstName,
                                              BranchId = ofc.Id,
                                              BranchName = ofc.Name,
                                              Number = 0,
                                              OfficeDesignationSettingId = offDeg.OfficeDesignationSettingId,
                                              ProductId = app.ProductId,
                                              ProductName = app.Product != null ? app.Product.Name : "",
                                              CreateDate = app.CreateDate,
                                              CreatedBy = app.CreatedBy,
                                              Amount = app.LoanApplicationId > 0 ? app.LoanApplication.LoanAmountApplied : app.DepositApplicationId > 0 ? app.DepositApplication.TotalDepositAmount : 0,
                                              Rate = 0,
                                              WAR = 0
                                          }).Where(c => c.CreateDate <= secondaryDateRange.ToDate && c.CreateDate >= secondaryDateRange.FromDate);
                if (productId != null)
                {
                    dataSecondTimeline = dataSecondTimeline.Where(r => r.ProductId == productId);
                }
                if (branchId != null)
                {
                    dataSecondTimeline = dataSecondTimeline.Where(r => r.BranchId == branchId);
                }
                result.TopSecond = new List<PerformerDto>();
                result.TopSecond = (from dt in dataSecondTimeline
                                    group dt by new { dt.OfficeDesignationSettingId, dt.RMId }
                    into grp
                                    select new PerformerDto
                                    {
                                        RMId = grp.Key.RMId,
                                        RMName = grp.FirstOrDefault() != null ? grp.FirstOrDefault().RMName : null,
                                        BranchId = grp.FirstOrDefault() != null ? grp.FirstOrDefault().BranchId : null,
                                        BranchName = grp.FirstOrDefault() != null ? grp.FirstOrDefault().BranchName : "",
                                        Number = grp.Count(),
                                        OfficeDesignationSettingId = grp.Key.OfficeDesignationSettingId,
                                        ProductId = grp.FirstOrDefault() != null ? grp.FirstOrDefault().ProductId : null,
                                        ProductName = grp.FirstOrDefault() != null ? grp.FirstOrDefault().ProductName : "",
                                        CreateDate = grp.FirstOrDefault() != null ? grp.FirstOrDefault().CreateDate : DateTime.Now,
                                        CreatedBy = null,
                                        Amount = grp.Sum(r => r.Amount),
                                        Rate = 0,
                                        WAR = 0
                                    }).ToList();
                if (criteriaId != null && criteriaId == (long)Criteria.Number)
                {
                    result.TopFirst = result.TopFirst.OrderByDescending(r => r.Number).Take(10).ToList();
                    result.BottomFirst = result.TopFirst.OrderBy(r => r.Number).Take(10).ToList();
                    result.TopSecond = result.TopSecond.OrderByDescending(r => r.Number).Take(10).ToList();
                    result.BottomSecond = result.TopSecond.OrderBy(r => r.Number).Take(10).ToList();
                }
                if (criteriaId != null && criteriaId == (long)Criteria.Amount)
                {
                    result.TopFirst = result.TopFirst.OrderByDescending(r => r.Amount).Take(10).ToList();
                    result.BottomFirst = result.TopFirst.OrderBy(r => r.Amount).Take(10).ToList();
                    result.TopSecond = result.TopSecond.OrderByDescending(r => r.Amount).Take(10).ToList();
                    result.BottomSecond = result.TopSecond.OrderBy(r => r.Amount).Take(10).ToList();
                }
            }
            #endregion
            #region Approval
            if (stageId != null && stageId == (long)Stage.Approval)
            {
                var allApplication = GenService.GetAll<Application>();
                if (allApplication.Any() && centerId > 0)
                {
                    allApplication = allApplication.Where(r => r.CostCenterId == centerId);
                }
                var loanApplicationsIds = allApplication.Where(a => a.Product.ProductType == ProductType.Loan).Select(a => a.Id);
                var approvedLoanApplicationLogs = GenService.GetAll<ApplicationLog>()
                    .Where(l => l.Status == EntityStatus.Active &&
                                l.ToStage == ApplicationStage.SentToOperations &&
                                loanApplicationsIds.Contains(l.ApplicationId)) //&& l.CreateDate <= dateRange.ToDate && l.CreateDate >= dateRange.FromDate)
                    .OrderBy(l => l.Id)
                    .Select(l => new { ApplicationId = l.ApplicationId, Product = l.Application.Product, Amount = l.Application.LoanApplication.LoanAmountApplied })
                    .Distinct();
                var approvedLoanApplicationLogsIds = approvedLoanApplicationLogs.Select(r => r.ApplicationId);
                var proposal = GenService.GetAll<Proposal>().Where(l => approvedLoanApplicationLogsIds.Contains(l.ApplicationId) && l.Status == EntityStatus.Active);

                var employeeOffDegSet = GenService.GetAll<EmployeeDesignationMapping>();
                var allOffices = GenService.GetAll<Office>();
                result.TopFirst = new List<PerformerDto>();
                var data = (from log in approvedLoanApplicationLogs
                            join app in allApplication on log.ApplicationId equals app.Id
                            join prsl in proposal on app.Id equals prsl.ApplicationId
                            join offDeg in employeeOffDegSet on app.RMId equals offDeg.EmployeeId
                            join ofc in allOffices on offDeg.OfficeDesignationSetting.OfficeId equals ofc.Id
                            select new PerformerDto
                            {
                                RMId = offDeg.EmployeeId,
                                RMName = offDeg.Employee.Person.FirstName,
                                BranchId = ofc.Id,
                                BranchName = ofc.Name,
                                Number = 0,
                                OfficeDesignationSettingId = offDeg.OfficeDesignationSettingId,
                                ProductId = app.ProductId,
                                ProductName = app.Product != null ? app.Product.Name : "",
                                CreateDate = app.CreateDate,
                                CreatedBy = app.CreatedBy,
                                Amount = prsl.RecomendedLoanAmountFromIPDC > 0 ? prsl.RecomendedLoanAmountFromIPDC : 0,
                                Rate = prsl.InterestRateOffered,
                                WAR = (int)(((prsl.RecomendedLoanAmountFromIPDC != null ? prsl.RecomendedLoanAmountFromIPDC : 0) * prsl.InterestRateOffered) / (prsl.RecomendedLoanAmountFromIPDC != null ? prsl.RecomendedLoanAmountFromIPDC : 0))
                            }).Where(c => c.CreateDate <= dateRange.ToDate && c.CreateDate >= dateRange.FromDate);

                if (productId != null)
                {
                    data = data.Where(r => r.ProductId == productId);
                }
                if (branchId != null)
                {
                    data = data.Where(r => r.BranchId == branchId);
                }

                result.TopFirst = (from dt in data
                                   group dt by new { dt.OfficeDesignationSettingId, dt.RMId }
                                   into grp
                                   select new PerformerDto
                                   {
                                       RMId = grp.Key.RMId,
                                       RMName = grp.FirstOrDefault() != null ? grp.FirstOrDefault().RMName : null,
                                       BranchId = grp.FirstOrDefault() != null ? grp.FirstOrDefault().BranchId : null,
                                       BranchName = grp.FirstOrDefault() != null ? grp.FirstOrDefault().BranchName : "",
                                       Number = grp.Count(),
                                       OfficeDesignationSettingId = grp.Key.OfficeDesignationSettingId,
                                       ProductId = grp.FirstOrDefault() != null ? grp.FirstOrDefault().ProductId : null,
                                       ProductName = grp.FirstOrDefault() != null ? grp.FirstOrDefault().ProductName : "",
                                       CreateDate = grp.FirstOrDefault() != null ? grp.FirstOrDefault().CreateDate : DateTime.Now,
                                       CreatedBy = null,
                                       Amount = grp.Sum(r => r.Amount),
                                       Rate = grp.Sum(r => r.Rate),
                                       WAR = grp.Sum(r => r.WAR)
                                   }).ToList();
                var product_Group_Approve = (from dt in data
                                             group dt by new { dt.OfficeDesignationSettingId, dt.RMId, dt.ProductId }
                                  into grp
                                             select new PerformerDto
                                             {
                                                 RMId = grp.Key.RMId,
                                                 RMName = grp.FirstOrDefault() != null ? grp.FirstOrDefault().RMName : null,
                                                 BranchId = grp.FirstOrDefault() != null ? grp.FirstOrDefault().BranchId : null,
                                                 BranchName = grp.FirstOrDefault() != null ? grp.FirstOrDefault().BranchName : "",
                                                 Number = grp.Count(),
                                                 OfficeDesignationSettingId = grp.Key.OfficeDesignationSettingId,
                                                 ProductId = grp.FirstOrDefault() != null ? grp.FirstOrDefault().ProductId : null,
                                                 ProductName = grp.FirstOrDefault() != null ? grp.FirstOrDefault().ProductName : "",
                                                 CreateDate = grp.FirstOrDefault() != null ? grp.FirstOrDefault().CreateDate : DateTime.Now,
                                                 CreatedBy = null,
                                                 Amount = grp.Sum(r => r.Amount),
                                                 Rate = grp.Sum(r => r.Rate),
                                                 WAR = grp.Sum(r => r.WAR)
                                             }).ToList();
                foreach (var performerDto in product_Group_Approve)
                {
                    int totalAmount = 0;
                    var collectAmt = data.Where(r => r.RMId == performerDto.RMId).Sum(r => r.Amount);
                    var collectRate = data.Where(r => r.RMId == performerDto.RMId).Sum(r => r.Rate);
                    totalAmount = (int)((collectAmt * collectRate) / (collectAmt != null ? collectAmt : 1));
                    result.TopFirst.Where(t => t.RMId == performerDto.RMId).ForEach(l => l.WAR = totalAmount);
                }
                ////////////////////////Second///////////////////////////
                var dataSecondTimeline = (from log in approvedLoanApplicationLogs
                                          join app in allApplication on log.ApplicationId equals app.Id
                                          join prsl in proposal on app.Id equals prsl.ApplicationId
                                          join offDeg in employeeOffDegSet on app.RMId equals offDeg.EmployeeId
                                          join ofc in allOffices on offDeg.OfficeDesignationSetting.OfficeId equals ofc.Id
                                          select new PerformerDto
                                          {
                                              //Id = lb.Id,
                                              RMId = offDeg.EmployeeId,
                                              RMName = offDeg.Employee.Person.FirstName,
                                              BranchId = ofc.Id,
                                              BranchName = ofc.Name,
                                              Number = 0,
                                              OfficeDesignationSettingId = offDeg.OfficeDesignationSettingId,
                                              ProductId = app.ProductId,
                                              ProductName = app.Product != null ? app.Product.Name : "",
                                              CreateDate = app.CreateDate,
                                              CreatedBy = app.CreatedBy,
                                              Amount = app.LoanApplicationId > 0 ? app.LoanApplication.LoanAmountApplied : app.DepositApplicationId > 0 ? app.DepositApplication.TotalDepositAmount : 0,
                                              Rate = prsl.InterestRateOffered,
                                              WAR = (int)(((prsl.RecomendedLoanAmountFromIPDC != null ? prsl.RecomendedLoanAmountFromIPDC : 0) * prsl.InterestRateOffered) / (prsl.RecomendedLoanAmountFromIPDC != null ? prsl.RecomendedLoanAmountFromIPDC : 0)),

                                          }).Where(c => c.CreateDate <= secondaryDateRange.ToDate && c.CreateDate >= secondaryDateRange.FromDate);
                if (productId != null)
                {
                    dataSecondTimeline = dataSecondTimeline.Where(r => r.ProductId == productId);
                }
                if (branchId != null)
                {
                    dataSecondTimeline = dataSecondTimeline.Where(r => r.BranchId == branchId);
                }
                result.TopSecond = new List<PerformerDto>();
                result.TopSecond = (from dt in dataSecondTimeline
                                    group dt by new { dt.OfficeDesignationSettingId, dt.RMId }
                    into grp
                                    select new PerformerDto
                                    {
                                        RMId = grp.Key.RMId,
                                        RMName = grp.FirstOrDefault() != null ? grp.FirstOrDefault().RMName : null,
                                        BranchId = grp.FirstOrDefault() != null ? grp.FirstOrDefault().BranchId : null,
                                        BranchName = grp.FirstOrDefault() != null ? grp.FirstOrDefault().BranchName : "",
                                        Number = grp.Count(),
                                        OfficeDesignationSettingId = grp.Key.OfficeDesignationSettingId,
                                        ProductId = grp.FirstOrDefault() != null ? grp.FirstOrDefault().ProductId : null,
                                        ProductName = grp.FirstOrDefault() != null ? grp.FirstOrDefault().ProductName : "",
                                        CreateDate = grp.FirstOrDefault() != null ? grp.FirstOrDefault().CreateDate : DateTime.Now,
                                        CreatedBy = null,
                                        Amount = grp.Sum(r => r.Amount),
                                        Rate = grp.Sum(r => r.Rate),
                                        WAR = grp.Sum(r => r.WAR)
                                    }).ToList();
                var product_GroupSecond_Approve = (from dt in dataSecondTimeline
                                                   group dt by new { dt.OfficeDesignationSettingId, dt.RMId, dt.ProductId }
                                into grp
                                                   select new PerformerDto
                                                   {
                                                       RMId = grp.Key.RMId,
                                                       RMName = grp.FirstOrDefault() != null ? grp.FirstOrDefault().RMName : null,
                                                       BranchId = grp.FirstOrDefault() != null ? grp.FirstOrDefault().BranchId : null,
                                                       BranchName = grp.FirstOrDefault() != null ? grp.FirstOrDefault().BranchName : "",
                                                       Number = grp.Count(),
                                                       OfficeDesignationSettingId = grp.Key.OfficeDesignationSettingId,
                                                       ProductId = grp.FirstOrDefault() != null ? grp.FirstOrDefault().ProductId : null,
                                                       ProductName = grp.FirstOrDefault() != null ? grp.FirstOrDefault().ProductName : "",
                                                       CreateDate = grp.FirstOrDefault() != null ? grp.FirstOrDefault().CreateDate : DateTime.Now,
                                                       CreatedBy = null,
                                                       Amount = grp.Sum(r => r.Amount),
                                                       Rate = grp.Sum(r => r.Rate),
                                                       WAR = grp.Sum(r => r.WAR)
                                                   }).ToList();
                foreach (var performerDto in product_GroupSecond_Approve)
                {
                    int totalAmount = 0;
                    var collectAmt = data.Where(r => r.RMId == performerDto.RMId).Sum(r => r.Amount);
                    var collectRate = data.Where(r => r.RMId == performerDto.RMId).Sum(r => r.Rate);
                    totalAmount = (int)((collectAmt * collectRate) / (collectAmt != null ? collectAmt : 1));
                    result.TopSecond.Where(t => t.RMId == performerDto.RMId).ForEach(l => l.WAR = totalAmount);
                }
                //-todo WAR Calculation
                //foreach (var performerDto in result.TopSecond)
                //{
                //    performerDto.WAR = performerDto.
                //}
                if (criteriaId != null && criteriaId == (long)Criteria.Number)
                {
                    result.TopFirst = result.TopFirst.OrderByDescending(r => r.Number).Take(10).ToList();
                    result.BottomFirst = result.TopFirst.OrderBy(r => r.Number).Take(10).ToList();
                    result.TopSecond = result.TopSecond.OrderByDescending(r => r.Number).Take(10).ToList();
                    result.BottomSecond = result.TopSecond.OrderBy(r => r.Number).Take(10).ToList();
                }
                if (criteriaId != null && criteriaId == (long)Criteria.Amount)
                {
                    result.TopFirst = result.TopFirst.OrderByDescending(r => r.Amount).Take(10).ToList();
                    result.BottomFirst = result.TopFirst.OrderBy(r => r.Amount).Take(10).ToList();
                    result.TopSecond = result.TopSecond.OrderByDescending(r => r.Amount).Take(10).ToList();
                    result.BottomSecond = result.TopSecond.OrderBy(r => r.Amount).Take(10).ToList();
                }
            }
            #endregion
            #region Rejection
            if (stageId != null && stageId == (long)Stage.Rejection)
            {
                var allApplication = GenService.GetAll<Application>();
                if (allApplication.Any() && centerId > 0)
                {
                    allApplication = allApplication.Where(r => r.CostCenterId == centerId);
                }
                var loanApplicationsIds = allApplication.Where(a => a.Product.ProductType == ProductType.Loan).Select(a => a.Id);
                var disapporvedLoanApplicationLogs = GenService.GetAll<ApplicationLog>()
                .Where(l => l.Status == EntityStatus.Active &&
                  l.ToStage < ApplicationStage.Drafted &&
                  loanApplicationsIds.Contains(l.ApplicationId)) // && l.CreateDate <= dateRange.ToDate && l.CreateDate >= dateRange.FromDate
      .OrderBy(l => l.Id)
      .Select(l => new { ApplicationId = l.ApplicationId, Product = l.Application.Product, Amount = l.Application.LoanApplication.LoanAmountApplied })
      .Distinct();
                var disapporvedLoanApplicationLogsIds = disapporvedLoanApplicationLogs.Select(r => r.ApplicationId);
                var proposal = GenService.GetAll<Proposal>().Where(l => disapporvedLoanApplicationLogsIds.Contains(l.ApplicationId) && l.Status == EntityStatus.Active);

                var employeeOffDegSet = GenService.GetAll<EmployeeDesignationMapping>();
                var allOffices = GenService.GetAll<Office>();
                result.TopFirst = new List<PerformerDto>();
                var data = (from log in disapporvedLoanApplicationLogs
                            join app in allApplication on log.ApplicationId equals app.Id
                            join prsl in proposal on app.Id equals prsl.ApplicationId
                            join offDeg in employeeOffDegSet on app.RMId equals offDeg.EmployeeId
                            join ofc in allOffices on offDeg.OfficeDesignationSetting.OfficeId equals ofc.Id
                            select new PerformerDto
                            {
                                RMId = offDeg.EmployeeId,
                                RMName = offDeg.Employee.Person.FirstName,
                                BranchId = ofc.Id,
                                BranchName = ofc.Name,
                                Number = 0,
                                OfficeDesignationSettingId = offDeg.OfficeDesignationSettingId,
                                ProductId = app.ProductId,
                                ProductName = app.Product != null ? app.Product.Name : "",
                                CreateDate = app.CreateDate,
                                CreatedBy = app.CreatedBy,
                                Amount = prsl.RecomendedLoanAmountFromIPDC > 0 ? prsl.RecomendedLoanAmountFromIPDC : 0,
                                Rate = prsl.InterestRateOffered,
                                WAR = (int)(((prsl.RecomendedLoanAmountFromIPDC != null ? prsl.RecomendedLoanAmountFromIPDC : 0) * prsl.InterestRateOffered) / (prsl.RecomendedLoanAmountFromIPDC != null ? prsl.RecomendedLoanAmountFromIPDC : 0))
                            }).Where(c => c.CreateDate <= dateRange.ToDate && c.CreateDate >= dateRange.FromDate);


                if (productId != null)
                {
                    data = data.Where(r => r.ProductId == productId);
                }
                if (branchId != null)
                {
                    data = data.Where(r => r.BranchId == branchId);
                }

                result.TopFirst = (from dt in data
                                   group dt by new { dt.OfficeDesignationSettingId, dt.RMId }
                                   into grp
                                   select new PerformerDto
                                   {
                                       RMId = grp.Key.RMId,
                                       RMName = grp.FirstOrDefault() != null ? grp.FirstOrDefault().RMName : null,
                                       BranchId = grp.FirstOrDefault() != null ? grp.FirstOrDefault().BranchId : null,
                                       BranchName = grp.FirstOrDefault() != null ? grp.FirstOrDefault().BranchName : "",
                                       Number = grp.Count(),
                                       OfficeDesignationSettingId = grp.Key.OfficeDesignationSettingId,
                                       ProductId = grp.FirstOrDefault() != null ? grp.FirstOrDefault().ProductId : null,
                                       ProductName = grp.FirstOrDefault() != null ? grp.FirstOrDefault().ProductName : "",
                                       CreateDate = grp.FirstOrDefault() != null ? grp.FirstOrDefault().CreateDate : DateTime.Now,
                                       CreatedBy = null,
                                       Amount = grp.Sum(r => r.Amount),
                                       Rate = grp.Sum(r => r.Rate),
                                       WAR = grp.Sum(r => r.WAR)
                                   }).ToList();
                var product_Group = (from dt in data
                                     group dt by new { dt.OfficeDesignationSettingId, dt.RMId, dt.ProductId }
                                  into grp
                                     select new PerformerDto
                                     {
                                         RMId = grp.Key.RMId,
                                         RMName = grp.FirstOrDefault() != null ? grp.FirstOrDefault().RMName : null,
                                         BranchId = grp.FirstOrDefault() != null ? grp.FirstOrDefault().BranchId : null,
                                         BranchName = grp.FirstOrDefault() != null ? grp.FirstOrDefault().BranchName : "",
                                         Number = grp.Count(),
                                         OfficeDesignationSettingId = grp.Key.OfficeDesignationSettingId,
                                         ProductId = grp.FirstOrDefault() != null ? grp.FirstOrDefault().ProductId : null,
                                         ProductName = grp.FirstOrDefault() != null ? grp.FirstOrDefault().ProductName : "",
                                         CreateDate = grp.FirstOrDefault() != null ? grp.FirstOrDefault().CreateDate : DateTime.Now,
                                         CreatedBy = null,
                                         Amount = grp.Sum(r => r.Amount),
                                         Rate = grp.Sum(r => r.Rate),
                                         WAR = grp.Sum(r => r.WAR)
                                     }).ToList();
                foreach (var performerDto in product_Group)
                {
                    int totalAmount = 0;
                    var collectAmt = data.Where(r => r.RMId == performerDto.RMId).Sum(r => r.Amount);
                    var collectRate = data.Where(r => r.RMId == performerDto.RMId).Sum(r => r.Rate);
                    totalAmount = (int)((collectAmt * collectRate) / (collectAmt != null ? collectAmt : 1));
                    result.TopFirst.Where(t => t.RMId == performerDto.RMId).ForEach(l => l.WAR = totalAmount);
                }
                ////////////////////////Second///////////////////////////
                var dataSecondTimeline = (from log in disapporvedLoanApplicationLogs
                                          join app in allApplication on log.ApplicationId equals app.Id
                                          join prsl in proposal on app.Id equals prsl.ApplicationId
                                          join offDeg in employeeOffDegSet on app.RMId equals offDeg.EmployeeId
                                          join ofc in allOffices on offDeg.OfficeDesignationSetting.OfficeId equals ofc.Id
                                          select new PerformerDto
                                          {
                                              //Id = lb.Id,
                                              RMId = offDeg.EmployeeId,
                                              RMName = offDeg.Employee.Person.FirstName,
                                              BranchId = ofc.Id,
                                              BranchName = ofc.Name,
                                              Number = 0,
                                              OfficeDesignationSettingId = offDeg.OfficeDesignationSettingId,
                                              ProductId = app.ProductId,
                                              ProductName = app.Product != null ? app.Product.Name : "",
                                              CreateDate = app.CreateDate,
                                              CreatedBy = app.CreatedBy,
                                              Amount = app.LoanApplicationId > 0 ? app.LoanApplication.LoanAmountApplied : app.DepositApplicationId > 0 ? app.DepositApplication.TotalDepositAmount : 0,
                                              Rate = prsl.InterestRateOffered,
                                              WAR = (int)(((prsl.RecomendedLoanAmountFromIPDC != null ? prsl.RecomendedLoanAmountFromIPDC : 0) * prsl.InterestRateOffered) / (prsl.RecomendedLoanAmountFromIPDC != null ? prsl.RecomendedLoanAmountFromIPDC : 0)),

                                          }).Where(c => c.CreateDate <= secondaryDateRange.ToDate && c.CreateDate >= secondaryDateRange.FromDate);
                if (productId != null)
                {
                    dataSecondTimeline = dataSecondTimeline.Where(r => r.ProductId == productId);
                }
                if (branchId != null)
                {
                    dataSecondTimeline = dataSecondTimeline.Where(r => r.BranchId == branchId);
                }
                result.TopSecond = new List<PerformerDto>();
                result.TopSecond = (from dt in dataSecondTimeline
                                    group dt by new { dt.OfficeDesignationSettingId, dt.RMId }
                    into grp
                                    select new PerformerDto
                                    {
                                        RMId = grp.Key.RMId,
                                        RMName = grp.FirstOrDefault() != null ? grp.FirstOrDefault().RMName : null,
                                        BranchId = grp.FirstOrDefault() != null ? grp.FirstOrDefault().BranchId : null,
                                        BranchName = grp.FirstOrDefault() != null ? grp.FirstOrDefault().BranchName : "",
                                        Number = grp.Count(),
                                        OfficeDesignationSettingId = grp.Key.OfficeDesignationSettingId,
                                        ProductId = grp.FirstOrDefault() != null ? grp.FirstOrDefault().ProductId : null,
                                        ProductName = grp.FirstOrDefault() != null ? grp.FirstOrDefault().ProductName : "",
                                        CreateDate = grp.FirstOrDefault() != null ? grp.FirstOrDefault().CreateDate : DateTime.Now,
                                        CreatedBy = null,
                                        Amount = grp.Sum(r => r.Amount),
                                        Rate = grp.Sum(r => r.Rate),
                                        WAR = grp.Sum(r => r.WAR)
                                    }).ToList();
                var product_Grouping = (from dt in dataSecondTimeline
                                        group dt by new { dt.OfficeDesignationSettingId, dt.RMId }
                   into grp
                                        select new PerformerDto
                                        {
                                            RMId = grp.Key.RMId,
                                            RMName = grp.FirstOrDefault() != null ? grp.FirstOrDefault().RMName : null,
                                            BranchId = grp.FirstOrDefault() != null ? grp.FirstOrDefault().BranchId : null,
                                            BranchName = grp.FirstOrDefault() != null ? grp.FirstOrDefault().BranchName : "",
                                            Number = grp.Count(),
                                            OfficeDesignationSettingId = grp.Key.OfficeDesignationSettingId,
                                            ProductId = grp.FirstOrDefault() != null ? grp.FirstOrDefault().ProductId : null,
                                            ProductName = grp.FirstOrDefault() != null ? grp.FirstOrDefault().ProductName : "",
                                            CreateDate = grp.FirstOrDefault() != null ? grp.FirstOrDefault().CreateDate : DateTime.Now,
                                            CreatedBy = null,
                                            Amount = grp.Sum(r => r.Amount),
                                            Rate = grp.Sum(r => r.Rate),
                                            WAR = grp.Sum(r => r.WAR)
                                        }).ToList();
                foreach (var performerDto in product_Grouping)
                {
                    int totalAmount = 0;
                    var collectAmt = data.Where(r => r.RMId == performerDto.RMId).Sum(r => r.Amount);
                    var collectRate = data.Where(r => r.RMId == performerDto.RMId).Sum(r => r.Rate);
                    totalAmount = (int)((collectAmt * collectRate) / (collectAmt != null ? collectAmt : 1));
                    result.TopSecond.Where(t => t.RMId == performerDto.RMId).ForEach(l => l.WAR = totalAmount);
                }
                //-todo WAR Calculation
                //foreach (var performerDto in result.TopSecond)
                //{
                //    performerDto.WAR = performerDto.
                //}
                if (criteriaId != null && criteriaId == (long)Criteria.Number)
                {
                    result.TopFirst = result.TopFirst.OrderByDescending(r => r.Number).Take(10).ToList();
                    result.BottomFirst = result.TopFirst.OrderBy(r => r.Number).Take(10).ToList();
                    result.TopSecond = result.TopSecond.OrderByDescending(r => r.Number).Take(10).ToList();
                    result.BottomSecond = result.TopSecond.OrderBy(r => r.Number).Take(10).ToList();
                }
                if (criteriaId != null && criteriaId == (long)Criteria.Amount)
                {
                    result.TopFirst = result.TopFirst.OrderByDescending(r => r.Amount).Take(10).ToList();
                    result.BottomFirst = result.TopFirst.OrderBy(r => r.Amount).Take(10).ToList();
                    result.TopSecond = result.TopSecond.OrderByDescending(r => r.Amount).Take(10).ToList();
                    result.BottomSecond = result.TopSecond.OrderBy(r => r.Amount).Take(10).ToList();
                }
            }
            #endregion
            #region Disbursment
            if (stageId != null && stageId == (long)Stage.Disbursement)
            {
                var allApplication = GenService.GetAll<Application>();
                if (allApplication.Any() && centerId > 0)
                {
                    allApplication = allApplication.Where(r => r.CostCenterId == centerId);
                }
                var submittedApplications = allApplication
                  .Where(a => (((a.ApplicationStage == ApplicationStage.SentToCRM || a.ApplicationStage == ApplicationStage.UnderProcessAtCRM) && a.Product.ProductType == ProductType.Loan) ||
                              ((a.ApplicationStage == ApplicationStage.SentToOperations || a.ApplicationStage == ApplicationStage.UnderProcessAtOperations) && a.Product.ProductType == ProductType.Deposit)));
                var submittedApplicationIds = submittedApplications.Select(s => s.Id).ToList();
                var disbursementMemos = GenService.GetAll<DisbursementMemo>().Where(d => d.ApplicationId != null &&
                                                                                  submittedApplicationIds.Contains((long)d.ApplicationId) &&
                                                                                  d.Status == EntityStatus.Active &&
                                                                                  d.IsApproved != null &&
                                                                                  d.IsApproved == true &&
                                                                                  d.IsDisbursed != null &&
                                                                                  d.IsDisbursed == true);
                var proposal = GenService.GetAll<Proposal>().Where(l => submittedApplicationIds.Contains(l.ApplicationId) && l.Status == EntityStatus.Active);

                var employeeOffDegSet = GenService.GetAll<EmployeeDesignationMapping>();
                var allOffices = GenService.GetAll<Office>();
                result.TopFirst = new List<PerformerDto>();
                var data = (from log in disbursementMemos
                            join app in allApplication on log.ApplicationId equals app.Id
                            join prsl in proposal on app.Id equals prsl.ApplicationId
                            join offDeg in employeeOffDegSet on app.RMId equals offDeg.EmployeeId
                            join ofc in allOffices on offDeg.OfficeDesignationSetting.OfficeId equals ofc.Id
                            select new PerformerDto
                            {
                                RMId = offDeg.EmployeeId,
                                RMName = offDeg.Employee.Person.FirstName,
                                BranchId = ofc.Id,
                                BranchName = ofc.Name,
                                Number = 0,
                                OfficeDesignationSettingId = offDeg.OfficeDesignationSettingId,
                                ProductId = app.ProductId,
                                ProductName = app.Product != null ? app.Product.Name : "",
                                CreateDate = app.CreateDate,
                                CreatedBy = app.CreatedBy,
                                Amount = prsl.RecomendedLoanAmountFromIPDC > 0 ? prsl.RecomendedLoanAmountFromIPDC : 0,
                                Rate = prsl.InterestRateOffered,
                                WAR = (int)(((prsl.RecomendedLoanAmountFromIPDC != null ? prsl.RecomendedLoanAmountFromIPDC : 0) * prsl.InterestRateOffered) / (prsl.RecomendedLoanAmountFromIPDC != null ? prsl.RecomendedLoanAmountFromIPDC : 1))
                            }).Where(c => c.CreateDate <= dateRange.ToDate && c.CreateDate >= dateRange.FromDate);

                if (productId != null)
                {
                    data = data.Where(r => r.ProductId == productId);
                }
                if (branchId != null)
                {
                    data = data.Where(r => r.BranchId == branchId);
                }

                result.TopFirst = (from dt in data
                                   group dt by new { dt.OfficeDesignationSettingId, dt.RMId }
                                   into grp
                                   select new PerformerDto
                                   {
                                       RMId = grp.Key.RMId,
                                       RMName = grp.FirstOrDefault() != null ? grp.FirstOrDefault().RMName : null,
                                       BranchId = grp.FirstOrDefault() != null ? grp.FirstOrDefault().BranchId : null,
                                       BranchName = grp.FirstOrDefault() != null ? grp.FirstOrDefault().BranchName : "",
                                       Number = grp.Count(),
                                       OfficeDesignationSettingId = grp.Key.OfficeDesignationSettingId,
                                       ProductId = grp.FirstOrDefault() != null ? grp.FirstOrDefault().ProductId : null,
                                       ProductName = grp.FirstOrDefault() != null ? grp.FirstOrDefault().ProductName : "",
                                       CreateDate = grp.FirstOrDefault() != null ? grp.FirstOrDefault().CreateDate : DateTime.Now,
                                       CreatedBy = null,
                                       Amount = grp.Sum(r => r.Amount),
                                       Rate = grp.Sum(r => r.Rate),
                                       WAR = grp.Sum(r => r.WAR)
                                   }).ToList();
                var product_Group = (from dt in data
                                     group dt by new { dt.OfficeDesignationSettingId, dt.RMId, dt.ProductId }
                                        into grp
                                     select new PerformerDto
                                     {
                                         RMId = grp.Key.RMId,
                                         RMName = grp.FirstOrDefault() != null ? grp.FirstOrDefault().RMName : null,
                                         BranchId = grp.FirstOrDefault() != null ? grp.FirstOrDefault().BranchId : null,
                                         BranchName = grp.FirstOrDefault() != null ? grp.FirstOrDefault().BranchName : "",
                                         Number = grp.Count(),
                                         OfficeDesignationSettingId = grp.Key.OfficeDesignationSettingId,
                                         ProductId = grp.Key.ProductId,
                                         ProductName = grp.FirstOrDefault() != null ? grp.FirstOrDefault().ProductName : "",
                                         CreateDate = grp.FirstOrDefault() != null ? grp.FirstOrDefault().CreateDate : DateTime.Now,
                                         CreatedBy = null,
                                         Amount = grp.Sum(r => r.Amount),
                                         Rate = grp.Sum(r => r.Rate),
                                         WAR = grp.Sum(r => r.WAR)
                                     }).ToList();

                foreach (var performerDto in product_Group)
                {
                    int totalAmount = 0;
                    var collectAmt = data.Where(r => r.RMId == performerDto.RMId).Sum(r => r.Amount);
                    var collectRate = data.Where(r => r.RMId == performerDto.RMId).Sum(r => r.Rate);
                    totalAmount = (int)((collectAmt * collectRate) / (collectAmt != null ? collectAmt : 1));
                    result.TopFirst.Where(t => t.RMId == performerDto.RMId).ForEach(l => l.WAR = totalAmount);
                }
                ////////////////////////Second///////////////////////////
                var dataSecondTimeline = (from log in disbursementMemos
                                          join app in allApplication on log.ApplicationId equals app.Id
                                          join prsl in proposal on app.Id equals prsl.ApplicationId
                                          join offDeg in employeeOffDegSet on app.RMId equals offDeg.EmployeeId
                                          join ofc in allOffices on offDeg.OfficeDesignationSetting.OfficeId equals ofc.Id
                                          select new PerformerDto
                                          {
                                              //Id = lb.Id,
                                              RMId = offDeg.EmployeeId,
                                              RMName = offDeg.Employee.Person.FirstName,
                                              BranchId = ofc.Id,
                                              BranchName = ofc.Name,
                                              Number = 0,
                                              OfficeDesignationSettingId = offDeg.OfficeDesignationSettingId,
                                              ProductId = app.ProductId,
                                              ProductName = app.Product != null ? app.Product.Name : "",
                                              CreateDate = app.CreateDate,
                                              CreatedBy = app.CreatedBy,
                                              Amount = app.LoanApplicationId > 0 ? app.LoanApplication.LoanAmountApplied : app.DepositApplicationId > 0 ? app.DepositApplication.TotalDepositAmount : 0,
                                              Rate = prsl.InterestRateOffered,
                                              WAR = (int)(((prsl.RecomendedLoanAmountFromIPDC != null ? prsl.RecomendedLoanAmountFromIPDC : 0) * prsl.InterestRateOffered) / (prsl.RecomendedLoanAmountFromIPDC != null ? prsl.RecomendedLoanAmountFromIPDC : 1)),

                                          }).Where(c => c.CreateDate <= secondaryDateRange.ToDate && c.CreateDate >= secondaryDateRange.FromDate);
                if (productId != null)
                {
                    dataSecondTimeline = dataSecondTimeline.Where(r => r.ProductId == productId);
                }
                if (branchId != null)
                {
                    dataSecondTimeline = dataSecondTimeline.Where(r => r.BranchId == branchId);
                }
                result.TopSecond = new List<PerformerDto>();
                result.TopSecond = (from dt in dataSecondTimeline
                                    group dt by new { dt.OfficeDesignationSettingId, dt.RMId }
                    into grp
                                    select new PerformerDto
                                    {
                                        RMId = grp.Key.RMId,
                                        RMName = grp.FirstOrDefault() != null ? grp.FirstOrDefault().RMName : null,
                                        BranchId = grp.FirstOrDefault() != null ? grp.FirstOrDefault().BranchId : null,
                                        BranchName = grp.FirstOrDefault() != null ? grp.FirstOrDefault().BranchName : "",
                                        Number = grp.Count(),
                                        OfficeDesignationSettingId = grp.Key.OfficeDesignationSettingId,
                                        ProductId = grp.FirstOrDefault() != null ? grp.FirstOrDefault().ProductId : null,
                                        ProductName = grp.FirstOrDefault() != null ? grp.FirstOrDefault().ProductName : "",
                                        CreateDate = grp.FirstOrDefault() != null ? grp.FirstOrDefault().CreateDate : DateTime.Now,
                                        CreatedBy = null,
                                        Amount = grp.Sum(r => r.Amount),
                                        Rate = grp.Sum(r => r.Rate),
                                        WAR = grp.Sum(r => r.WAR)
                                    }).ToList();
                var product_Grouping = (from dt in dataSecondTimeline
                                        group dt by new { dt.OfficeDesignationSettingId, dt.RMId, dt.ProductId }
                    into grp
                                        select new PerformerDto
                                        {
                                            RMId = grp.Key.RMId,
                                            RMName = grp.FirstOrDefault() != null ? grp.FirstOrDefault().RMName : null,
                                            BranchId = grp.FirstOrDefault() != null ? grp.FirstOrDefault().BranchId : null,
                                            BranchName = grp.FirstOrDefault() != null ? grp.FirstOrDefault().BranchName : "",
                                            Number = grp.Count(),
                                            OfficeDesignationSettingId = grp.Key.OfficeDesignationSettingId,
                                            ProductId = grp.Key.ProductId,
                                            ProductName = grp.FirstOrDefault() != null ? grp.FirstOrDefault().ProductName : "",
                                            CreateDate = grp.FirstOrDefault() != null ? grp.FirstOrDefault().CreateDate : DateTime.Now,
                                            CreatedBy = null,
                                            Amount = grp.Sum(r => r.Amount),
                                            Rate = grp.Sum(r => r.Rate),
                                            WAR = grp.Sum(r => r.WAR)
                                        }).ToList();

                foreach (var performerDto in product_Grouping)
                {
                    int totalAmount = 0;
                    var collectAmt = dataSecondTimeline.Where(r => r.RMId == performerDto.RMId).Sum(r => r.Amount);
                    var collectRate = dataSecondTimeline.Where(r => r.RMId == performerDto.RMId).Sum(r => r.Rate);
                    totalAmount = (int)((collectAmt * collectRate) / (collectAmt != null ? collectAmt : 1));
                    result.TopSecond.Where(t => t.RMId == performerDto.RMId).ForEach(l => l.WAR = totalAmount);
                }

                if (criteriaId != null && criteriaId == (long)Criteria.Number)
                {
                    result.TopFirst = result.TopFirst.OrderByDescending(r => r.Number).Take(10).ToList();
                    result.BottomFirst = result.TopFirst.OrderBy(r => r.Number).Take(10).ToList();
                    result.TopSecond = result.TopSecond.OrderByDescending(r => r.Number).Take(10).ToList();
                    result.BottomSecond = result.TopSecond.OrderBy(r => r.Number).Take(10).ToList();
                }
                if (criteriaId != null && criteriaId == (long)Criteria.Amount)
                {
                    result.TopFirst = result.TopFirst.OrderByDescending(r => r.Amount).Take(10).ToList();
                    result.BottomFirst = result.TopFirst.OrderBy(r => r.Amount).Take(10).ToList();
                    result.TopSecond = result.TopSecond.OrderByDescending(r => r.Amount).Take(10).ToList();
                    result.BottomSecond = result.TopSecond.OrderBy(r => r.Amount).Take(10).ToList();
                }
            }
            #endregion
            return result;
            //throw new NotImplementedException();
        }

        //public object GetMDDashboardHighlightsRight(TimeLine? timeLine, long? branchId)
        //{
        //    var data = new DashBoardHighlightsDto();
        //    DateRangeDto dateRange;
        //    if (timeLine == null)
        //        dateRange = new DateRangeDto();
        //    else
        //        dateRange = new DateRangeDto((TimeLine)timeLine);

        //    var applicationListToDay = new List<ApplicationLog>();

        //    var approvedApplicationListToDay = new List<ApplicationLog>();

        //    var disbursementMemosToday = new List<DisbursementMemo>();

        //    var fundReceivedToday = new List<FundConfirmationDetail>();


        //    long userId = 0;

        //    if (branchId != null || branchId > 0)
        //    {
        //        //var employeeList = _office.GetEmployeesByOfficeId((long)branchId);

        //        List<EmployeeDto> subordinateEmpList = new List<EmployeeDto>();
        //        if (branchId > 0)
        //            subordinateEmpList = _office.GetEmployeesByOfficeId((long)branchId);
        //        var subordinateEmpIdList = subordinateEmpList.Select(s => s.Id).ToList();
        //        var subordinateUserList = new List<long>();
        //        foreach (var sub in subordinateEmpList)
        //        {
        //            subordinateUserList.Add((long)_user.GetUserByEmployeeId((long)sub.Id).Id);
        //        }

        //        var allApplications = GenService.GetAll<Application>().Where(a => subordinateEmpIdList.Contains(a.RMId));

        //        #region File Submited Apps.

        //        var acceptableLogsToday = GenService.GetAll<ApplicationLog>()
        //            .Where(l => allApplications.Any(a => a.Id == l.ApplicationId)
        //            && (l.Application.LoanApplicationId != null || l.Application.DepositApplicationId != null)
        //            && (l.Application.LoanApplication.LoanAmountApplied != null || l.Application.DepositApplication.TotalDepositAmount != null)
        //            && l.Status == EntityStatus.Active
        //            && l.CreateDate <= dateRange.ToDate
        //            && l.CreateDate >= dateRange.FromDate
        //            && (((l.ToStage == ApplicationStage.SentToCRM || l.ToStage == ApplicationStage.UnderProcessAtCRM)
        //            && l.Application.Product.ProductType == ProductType.Loan)
        //            || ((l.ToStage == ApplicationStage.SentToOperations || l.ToStage == ApplicationStage.UnderProcessAtOperations)
        //            && l.Application.Product.ProductType == ProductType.Deposit)));
        //        var allApplicationsToday = (from appLog in acceptableLogsToday
        //                                    orderby appLog.Id
        //                                    select appLog).Distinct().ToList();
        //        if (allApplicationsToday.Any())
        //            applicationListToDay.AddRange(allApplicationsToday);

        //        #endregion File Submited Apps.

        //        #region File Approved/DisApproved ToDay Apps.

        //        var loanApplicationsIds = allApplications.Where(a => a.Product.ProductType == ProductType.Loan && a.LoanApplication.LoanAmountApplied != null).Select(a => a.Id);

        //        var approvedLoanApplicationLogsToday = GenService.GetAll<ApplicationLog>()
        //            .Where(l => l.Status == EntityStatus.Active &&
        //                        l.ToStage == ApplicationStage.SentToOperations &&
        //                        loanApplicationsIds.Contains(l.ApplicationId) &&
        //                        l.CreateDate <= dateRange.ToDate &&
        //                        l.CreateDate >= dateRange.FromDate).ToList();
        //        if (approvedLoanApplicationLogsToday.Count > 0)
        //            approvedApplicationListToDay.AddRange(approvedLoanApplicationLogsToday);




        //        #endregion File Approved/DisApproved ToDay Apps.

        //        #region Amount disbursed / received Today

        //        var acceptableAppIds = applicationListToDay.Select(a => a.ApplicationId).Distinct().ToList();
        //        var TodaydisbursementMemos = GenService.GetAll<DisbursementMemo>().Where(d => d.ApplicationId != null
        //                                                                            && acceptableAppIds.Contains((long)d.ApplicationId)
        //                                                                            && d.Status == EntityStatus.Active
        //                                                                            && d.IsApproved != null
        //                                                                            && d.IsApproved == true
        //                                                                            && d.IsDisbursed != null
        //                                                                            && d.IsDisbursed == true
        //                                                                            && d.DisbursementDetails.FirstOrDefault().CreateDate <= dateRange.ToDate
        //                                                                            && d.DisbursementDetails.FirstOrDefault().CreateDate >= dateRange.FromDate);
        //        disbursementMemosToday = TodaydisbursementMemos.ToList();

        //        var TodayfundReceivedToday = GenService.GetAll<FundConfirmationDetail>().Where(f => acceptableAppIds.Contains((long)f.FundConfirmation.ApplicationId)
        //                                                                                && f.FundConfirmation.Status == EntityStatus.Active
        //                                                                                && f.Status == EntityStatus.Active
        //                                                                                && f.FundConfirmation.FundReceived != null
        //                                                                                && f.FundConfirmation.FundReceived == true
        //                                                                                && f.CreditDate != null
        //                                                                                && f.CreditDate <= dateRange.ToDate
        //                                                                                && f.CreditDate >= dateRange.FromDate);


        //        fundReceivedToday = TodayfundReceivedToday.ToList();

        //        #endregion Amount disbursed / received Today



        //    }
        //    else
        //    {
        //        #region Submited Apps
        //        var acceptableLogsToday = GenService.GetAll<ApplicationLog>()
        //            .Where(l => (l.Application.LoanApplicationId != null || l.Application.DepositApplicationId != null)
        //            && (l.Application.LoanApplication.LoanAmountApplied != null || l.Application.DepositApplication.TotalDepositAmount != null)
        //            && l.Status == EntityStatus.Active
        //            && l.CreateDate <= dateRange.ToDate
        //            && l.CreateDate >= dateRange.FromDate
        //            && (((l.ToStage == ApplicationStage.SentToCRM || l.ToStage == ApplicationStage.UnderProcessAtCRM)
        //            && l.Application.Product.ProductType == ProductType.Loan)
        //            || ((l.ToStage == ApplicationStage.SentToOperations || l.ToStage == ApplicationStage.UnderProcessAtOperations)
        //            && l.Application.Product.ProductType == ProductType.Deposit)));
        //        var allApplicationsToday = (from appLog in acceptableLogsToday
        //                                    orderby appLog.Id
        //                                    select appLog).Distinct().ToList();
        //        if (allApplicationsToday.Any())
        //            applicationListToDay.AddRange(allApplicationsToday);
        //        #endregion Submited Apps

        //        #region File Approved/DisApproved ToDay Apps.

        //        var loanApplicationsIds = GenService.GetAll<Application>().Where(a => a.Product.ProductType == ProductType.Loan).Select(a => a.Id);

        //        var approvedLoanApplicationLogsToday = GenService.GetAll<ApplicationLog>()
        //        .Where(l => l.Status == EntityStatus.Active &&
        //                        l.ToStage == ApplicationStage.SentToOperations &&
        //                        loanApplicationsIds.Contains(l.ApplicationId) &&
        //        l.CreateDate <= dateRange.ToDate &&
        //                        l.CreateDate >= dateRange.FromDate).ToList();
        //        if (approvedLoanApplicationLogsToday.Count > 0)
        //            approvedApplicationListToDay.AddRange(approvedLoanApplicationLogsToday);




        //        #endregion File Approved/DisApproved ToDay Apps.

        //        #region File disbursed / received Today
        //        var TodaydisbursementMemos = GenService.GetAll<DisbursementMemo>().Where(d => d.ApplicationId != null
        //                                                                            && d.Status == EntityStatus.Active
        //                                                                            && d.IsApproved != null
        //                                                                            && d.IsApproved == true
        //                                                                            && d.IsDisbursed != null
        //                                                                            && d.IsDisbursed == true
        //                                                                            && d.DisbursementDetails.FirstOrDefault().CreateDate <= dateRange.ToDate
        //                                                                            && d.DisbursementDetails.FirstOrDefault().CreateDate >= dateRange.FromDate);

        //        disbursementMemosToday = TodaydisbursementMemos.ToList();

        //        var TodayfundReceived = GenService.GetAll<FundConfirmationDetail>().Where(f => f.FundConfirmation.ApplicationId != null
        //                                                                                && f.FundConfirmation.Status == EntityStatus.Active
        //                                                                                && f.Status == EntityStatus.Active
        //                                                                                && f.FundConfirmation.FundReceived != null
        //                                                                                && f.FundConfirmation.FundReceived == true
        //                                                                                && f.CreditDate != null
        //                                                                                && f.CreditDate <= dateRange.ToDate
        //                                                                                && f.CreditDate >= dateRange.FromDate);


        //        fundReceivedToday = TodayfundReceived.ToList();

        //        #endregion File disbursed / received Today
        //    }

        //    #region initializations

        //    data.Disbursed_HomeLoanAmount = 0;
        //    data.Disbursed_PersonalLoanAmount = 0;
        //    data.Disbursed_AutoLoanAmount = 0;
        //    data.RMHomeLoan1 = "";
        //    data.RMHomeLoan1Amount = 0;
        //    data.RMHomeLoan1Count = 0;
        //    data.RMHomeLoan2 = "";
        //    data.RMHomeLoan2Amount = 0;
        //    data.RMHomeLoan2Count = 0;
        //    data.RMPersonalLoan = "";
        //    data.RMPersonalLoanAmount = 0;
        //    data.RMPersonalLoanCount = 0;
        //    data.RMAutoLoan = "";
        //    data.RMAutoLoanAmount = 0;
        //    data.RMAutoLoanCount = 0;
        //    data.RMLiability1 = "";
        //    data.RMLiability1Amount = 0;
        //    data.RMLiability1Count = 0;
        //    data.RMLiability2 = "";
        //    data.RMLiability2Amount = 0;
        //    data.RMLiability2Count = 0;
        //    #endregion


        //    #region Disbursed/Received

        //    data.Disbursed_HomeLoanToDay = disbursementMemosToday.Where(d => d.Proposal.FacilityType == ProposalFacilityType.Home_Loan).Count();

        //    data.Disbursed_AutoLoanToDay = disbursementMemosToday.Where(d => d.Proposal.FacilityType == ProposalFacilityType.Auto_Loan).Count();

        //    data.Disbursed_PersonalLoanToDay = disbursementMemosToday.Where(d => d.Proposal.FacilityType == ProposalFacilityType.Personal_Loan).Count();

        //    data.Received_FixedToDay = fundReceivedToday.Where(d => d.FundConfirmation.Application.Product.DepositType == DepositType.Fixed).Count();

        //    data.Received_RecurrentToDay = fundReceivedToday.Where(d => d.FundConfirmation.Application.Product.DepositType == DepositType.Recurring).Count();

        //    #endregion Disbursed/Received

        //    long DisbursedHome = data.Disbursed_HomeLoanToDay;
        //    long DisbursedAuto = data.Disbursed_AutoLoanToDay;
        //    long DisbursedPersonal = data.Disbursed_PersonalLoanToDay;

        //    long receivedFixed = data.Received_FixedToDay;
        //    long ReceivedRecurrent = data.Received_RecurrentToDay;

        //    data.LiabilityCount = receivedFixed + ReceivedRecurrent;
        //    data.AssetCount = DisbursedHome + DisbursedAuto + DisbursedPersonal;

        //    return data;
        //}


        public object GetPMDashboard(DateTime? fromDate, DateTime? toDate, Stages? stage, Criteria? criteria, long? costCenterId, ProductSelection? product, long? branchId)
        {
            //long UserId,
            var data = new DashBoardHighlightsDto();

            var allApplications = GenService.GetAll<Application>().Where(a => a.Status == EntityStatus.Active);

            var loanApplicationsIds = allApplications.Where(a => a.Product.ProductType == ProductType.Loan && a.LoanApplication.LoanAmountApplied != null).Select(a => a.Id);

            var callListToday = new List<Call>();

            var leadListToday = new List<SalesLead>();

            var applicationListToDay = new List<ApplicationLog>();

            var approvedApplicationListToDay = new List<ApplicationLog>();

            var disbursementMemosToday = new List<DisbursementMemo>();

            var fundReceivedToday = new List<FundConfirmation>();

            var disbursedMemodetails = new List<DMDetail>();
            disbursedMemodetails = GenService.GetAll<DMDetail>().Where(dm => dm.Status == EntityStatus.Active).ToList();
            var fundConfirmdetails = new List<FundConfirmationDetail>();
            fundConfirmdetails = GenService.GetAll<FundConfirmationDetail>().Where(fc => fc.Status == EntityStatus.Active).ToList();

            #region Call

            IQueryable<Call> allCallsToday = GenService.GetAll<Call>().Where(c =>
            c.CreateDate <= toDate
            && c.CreateDate >= fromDate
            && c.Amount != null && c.Status == EntityStatus.Active);

            #endregion Call

            #region Lead

            IQueryable<SalesLead> allLeadsToday = GenService.GetAll<SalesLead>().Where(c =>
            c.CreateDate <= toDate && c.CreateDate >= fromDate &&
            c.Amount != null && c.Status == EntityStatus.Active);

            #endregion Lead

            #region File Submited Apps

            var acceptableLogsToday = GenService.GetAll<ApplicationLog>()
                .Where(l => (l.Application.LoanApplicationId != null || l.Application.DepositApplicationId != null)
                && (l.Application.LoanApplication.LoanAmountApplied != null || l.Application.DepositApplication.TotalDepositAmount != null)
                && l.Status == EntityStatus.Active
                && l.CreateDate <= toDate
                && l.CreateDate >= fromDate
                && (((l.ToStage == ApplicationStage.SentToCRM || l.ToStage == ApplicationStage.UnderProcessAtCRM)
                && l.Application.Product.ProductType == ProductType.Loan)
                || ((l.ToStage == ApplicationStage.SentToOperations || l.ToStage == ApplicationStage.UnderProcessAtOperations)
                && l.Application.Product.ProductType == ProductType.Deposit)));


            #endregion File Submited Apps

            #region File Approved

            var approvedLoanApplicationLogsToday = GenService.GetAll<ApplicationLog>()
                .Where(l => l.Status == EntityStatus.Active &&
                            l.ToStage == ApplicationStage.SentToOperations
                            && l.CreateDate <= toDate
                            && l.CreateDate >= fromDate
                            ).ToList();


            #endregion File Approved

            #region Amount disbursed / received Today


            var TodaydisbursementMemos = GenService.GetAll<DMDetail>().Where(d => d.DisbursementMemo.ApplicationId != null
                        && d.DisbursementMemo.Status == EntityStatus.Active
                        && d.DisbursementMemo.Application.LoanApplicationId != null
                        && d.DisbursementMemo.Application.Product.FacilityType != null
                        && (d.DisbursementMemo.Application.Product.FacilityType == ProposalFacilityType.Home_Loan
                            || d.DisbursementMemo.Application.Product.FacilityType == ProposalFacilityType.Auto_Loan
                            || d.DisbursementMemo.Application.Product.FacilityType == ProposalFacilityType.Personal_Loan)
                        && d.CreateDate <= toDate
                        && d.CreateDate >= fromDate
                        && d.DisbursementMemo.IsApproved != null
                        && d.DisbursementMemo.IsApproved == true
                        && d.DisbursementMemo.IsDisbursed != null
                        && d.DisbursementMemo.IsDisbursed == true
                        && d.Status == EntityStatus.Active).ToList();

            var TodayfundReceivedToday = GenService.GetAll<FundConfirmationDetail>().Where(d => d.FundConfirmation.ApplicationId != null
                        && d.FundConfirmation.Application.DepositApplicationId != null
                        && d.FundConfirmation.Application.Product.DepositType != null
                        && (d.FundConfirmation.Application.Product.DepositType == DepositType.Fixed
                            || d.FundConfirmation.Application.Product.DepositType == DepositType.Recurring)
                        && d.CreateDate <= toDate
                        && d.CreateDate >= fromDate
                        && d.FundConfirmation.FundReceived != null
                        && d.FundConfirmation.FundReceived == true
                        && d.Status == EntityStatus.Active).ToList();


            #endregion Amount disbursed / received Today

            #region With Brach and cost center

            if ((branchId != null || branchId > 0) && (costCenterId != null || costCenterId > 0))
            {
                List<EmployeeDto> subordinateEmpList = new List<EmployeeDto>();
                if (branchId > 0)
                    subordinateEmpList = _office.GetEmployeesByOfficeId((long)branchId);

                var subordinateUserList = new List<long>();
                foreach (var sub in subordinateEmpList)
                {
                    subordinateUserList.Add((long)_user.GetUserByEmployeeId((long)sub.Id).Id);
                }

                //allApplications = allApplications.Where(a => subordinateEmpIdList.Contains(a.RMId));
                #region Call
                if (stage == Stages.Call)
                {
                    allCallsToday = allCallsToday.Where(c => subordinateUserList.Contains((long)c.CreatedBy));
                    if (allCallsToday.Any())
                        callListToday.AddRange(allCallsToday);
                }
                #endregion Call

                #region Lead

                else if (stage == Stages.Lead)
                {

                    allLeadsToday = allLeadsToday.Where(l => subordinateUserList.Contains((long)l.CreatedBy));
                    if (allLeadsToday.Any())
                        leadListToday.AddRange(allLeadsToday);
                }

                #endregion Lead

                #region File Submitted

                if (stage == Stages.Files_Submitted)
                {
                    //acceptableLogsToday = acceptableLogsToday.Where(l => allApplications.Any(a => a.Id == l.ApplicationId));
                    acceptableLogsToday = acceptableLogsToday.Where(l => (l.Application.BranchId != null && l.Application.BranchId == branchId) && (l.Application.CostCenterId != null && l.Application.CostCenterId == costCenterId));
                    var allApplicationsToday = (from appLog in acceptableLogsToday
                                                orderby appLog.Id
                                                select appLog).Distinct().ToList();
                    if (allApplicationsToday.Any())
                        applicationListToDay.AddRange(allApplicationsToday);
                }

                #endregion File Submitted

                #region File Approved

                //var loanApplicationsIds = allApplications.Where(a => a.Product.ProductType == ProductType.Loan && a.LoanApplication.LoanAmountApplied != null).Select(a => a.Id);
                loanApplicationsIds = allApplications.Where(a => (a.BranchId != null && a.BranchId == branchId) && (a.CostCenterId != null && a.CostCenterId == costCenterId)).Select(a => a.Id);
                if (stage == Stages.Files_Approved)
                {
                    approvedLoanApplicationLogsToday = approvedLoanApplicationLogsToday.Where(l => loanApplicationsIds.Contains(l.ApplicationId)).ToList();
                    if (approvedLoanApplicationLogsToday.Count > 0)
                        approvedApplicationListToDay.AddRange(approvedLoanApplicationLogsToday);
                }

                #endregion File Approved

                #region Disbursed/Received

                TodaydisbursementMemos = TodaydisbursementMemos.Where(d => (d.DisbursementMemo.Application.BranchId != null && d.DisbursementMemo.Application.BranchId == branchId) && (d.DisbursementMemo.Application.CostCenterId != null && d.DisbursementMemo.Application.CostCenterId == costCenterId)).ToList();
                TodayfundReceivedToday = TodayfundReceivedToday.Where(d => (d.FundConfirmation.Application.BranchId != null && d.FundConfirmation.Application.BranchId == branchId) && (d.FundConfirmation.Application.CostCenterId != null && d.FundConfirmation.Application.CostCenterId == costCenterId)).ToList();

                if (stage == Stages.Files_Disbursed)
                {
                    var acceptableDMidstoday = TodaydisbursementMemos.Select(dm => dm.DisbursementMemoId).ToList();
                    var disMemotoday = GenService.GetAll<DisbursementMemo>().Where(d => acceptableDMidstoday.Contains(d.Id)).ToList();
                    disbursementMemosToday = disMemotoday.ToList();

                    var acceptableFCidstoday = TodayfundReceivedToday.Select(dm => dm.FundConfirmationId).ToList();
                    var fundRectoday = GenService.GetAll<FundConfirmation>().Where(d => acceptableFCidstoday.Contains(d.Id)).ToList();
                    fundReceivedToday = fundRectoday.ToList();
                }

                #endregion Disbursed/Received

            }

            #endregion

            #region With Brach
            else if (branchId != null || branchId > 0)
            {
                List<EmployeeDto> subordinateEmpList = new List<EmployeeDto>();
                if (branchId > 0)
                    subordinateEmpList = _office.GetEmployeesByOfficeId((long)branchId);

                var subordinateUserList = new List<long>();
                foreach (var sub in subordinateEmpList)
                {
                    subordinateUserList.Add((long)_user.GetUserByEmployeeId((long)sub.Id).Id);
                }

                //allApplications = allApplications.Where(a => subordinateEmpIdList.Contains(a.RMId));
                #region Call
                if (stage == Stages.Call)
                {
                    allCallsToday = allCallsToday.Where(c => subordinateUserList.Contains((long)c.CreatedBy));
                    if (allCallsToday.Any())
                        callListToday.AddRange(allCallsToday);
                }
                #endregion Call

                #region Lead

                else if (stage == Stages.Lead)
                {

                    allLeadsToday = allLeadsToday.Where(l => subordinateUserList.Contains((long)l.CreatedBy));
                    if (allLeadsToday.Any())
                        leadListToday.AddRange(allLeadsToday);
                }

                #endregion Lead

                #region File Submitted

                if (stage == Stages.Files_Submitted)
                {
                    //acceptableLogsToday = acceptableLogsToday.Where(l => allApplications.Any(a => a.Id == l.ApplicationId));
                    acceptableLogsToday = acceptableLogsToday.Where(l => l.Application.BranchId != null && l.Application.BranchId == branchId);
                    var allApplicationsToday = (from appLog in acceptableLogsToday
                                                orderby appLog.Id
                                                select appLog).Distinct().ToList();
                    if (allApplicationsToday.Any())
                        applicationListToDay.AddRange(allApplicationsToday);
                }

                #endregion File Submitted

                #region File Approved
                //var loanApplicationsIds = allApplications.Where(a => a.Product.ProductType == ProductType.Loan && a.LoanApplication.LoanAmountApplied != null).Select(a => a.Id);
                loanApplicationsIds = allApplications.Where(a => a.BranchId != null && a.BranchId == branchId).Select(a => a.Id);
                if (stage == Stages.Files_Approved)
                {
                    approvedLoanApplicationLogsToday = approvedLoanApplicationLogsToday.Where(l => loanApplicationsIds.Contains(l.ApplicationId)).ToList();
                    if (approvedLoanApplicationLogsToday.Count > 0)
                        approvedApplicationListToDay.AddRange(approvedLoanApplicationLogsToday);
                }

                #endregion File Approved

                #region Disbursed/Received

                TodaydisbursementMemos = TodaydisbursementMemos.Where(d => d.DisbursementMemo.Application.BranchId != null && d.DisbursementMemo.Application.BranchId == branchId).ToList();
                TodayfundReceivedToday = TodayfundReceivedToday.Where(d => d.FundConfirmation.Application.BranchId != null && d.FundConfirmation.Application.BranchId == branchId).ToList();

                if (stage == Stages.Files_Disbursed)
                {
                    var acceptableDMidstoday = TodaydisbursementMemos.Select(dm => dm.DisbursementMemoId).ToList();
                    var disMemotoday = GenService.GetAll<DisbursementMemo>().Where(d => acceptableDMidstoday.Contains(d.Id)).ToList();
                    disbursementMemosToday = disMemotoday.ToList();

                    var acceptableFCidstoday = TodayfundReceivedToday.Select(dm => dm.FundConfirmationId).ToList();
                    var fundRectoday = GenService.GetAll<FundConfirmation>().Where(d => acceptableFCidstoday.Contains(d.Id)).ToList();
                    fundReceivedToday = fundRectoday.ToList();
                }

                #endregion Disbursed/Received
            }
            #endregion

            #region cost center
            else if (costCenterId != null && costCenterId > 0)
            {
                #region Call

                if (stage == Stages.Call)
                {
                    if (allCallsToday.Any())
                        callListToday.AddRange(allCallsToday);
                }
                #endregion Call

                #region Lead

                if (stage == Stages.Lead)
                {
                    if (allLeadsToday.Any())
                        leadListToday.AddRange(allLeadsToday);
                }
                #endregion Lead

                #region File Submitted

                if (stage == Stages.Files_Submitted)
                {
                    //acceptableLogsToday = acceptableLogsToday.Where(l => allApplications.Any(a => a.Id == l.ApplicationId));
                    acceptableLogsToday = acceptableLogsToday.Where(l => (l.Application.CostCenterId != null && l.Application.CostCenterId == costCenterId));
                    var allApplicationsToday = (from appLog in acceptableLogsToday
                                                orderby appLog.Id
                                                select appLog).Distinct().ToList();
                    if (allApplicationsToday.Any())
                        applicationListToDay.AddRange(allApplicationsToday);
                }

                #endregion File Submitted

                #region File Approved

                //var loanApplicationsIds = allApplications.Where(a => a.Product.ProductType == ProductType.Loan && a.LoanApplication.LoanAmountApplied != null).Select(a => a.Id);
                loanApplicationsIds = allApplications.Where(a => (a.CostCenterId != null && a.CostCenterId == costCenterId)).Select(a => a.Id);
                if (stage == Stages.Files_Approved)
                {
                    approvedLoanApplicationLogsToday = approvedLoanApplicationLogsToday.Where(l => loanApplicationsIds.Contains(l.ApplicationId)).ToList();
                    if (approvedLoanApplicationLogsToday.Count > 0)
                        approvedApplicationListToDay.AddRange(approvedLoanApplicationLogsToday);
                }

                #endregion File Approved

                #region Disbursed/Received

                TodaydisbursementMemos = TodaydisbursementMemos.Where(d => (d.DisbursementMemo.Application.CostCenterId != null && d.DisbursementMemo.Application.CostCenterId == costCenterId)).ToList();
                TodayfundReceivedToday = TodayfundReceivedToday.Where(d => (d.FundConfirmation.Application.CostCenterId != null && d.FundConfirmation.Application.CostCenterId == costCenterId)).ToList();

                if (stage == Stages.Files_Disbursed)
                {
                    var acceptableDMidstoday = TodaydisbursementMemos.Select(dm => dm.DisbursementMemoId).ToList();
                    var disMemotoday = GenService.GetAll<DisbursementMemo>().Where(d => acceptableDMidstoday.Contains(d.Id)).ToList();
                    disbursementMemosToday = disMemotoday.ToList();

                    var acceptableFCidstoday = TodayfundReceivedToday.Select(dm => dm.FundConfirmationId).ToList();
                    var fundRectoday = GenService.GetAll<FundConfirmation>().Where(d => acceptableFCidstoday.Contains(d.Id)).ToList();
                    fundReceivedToday = fundRectoday.ToList();
                }

                #endregion Disbursed/Received
            }
            #endregion

            else
            {
                #region Call

                if (stage == Stages.Call)
                {
                    if (allCallsToday.Any())
                        callListToday.AddRange(allCallsToday);

                    if (allLeadsToday.Any())
                        leadListToday.AddRange(allLeadsToday);
                }
                #endregion Call

                #region Lead

                if (stage == Stages.Lead)
                {
                    if (allLeadsToday.Any())
                        leadListToday.AddRange(allLeadsToday);

                    if (allCallsToday.Any())
                        callListToday.AddRange(allCallsToday);
                }
                #endregion Lead

                #region File Submitted

                var allApplicationsToday = (from appLog in acceptableLogsToday
                                            orderby appLog.Id
                                            select appLog).Distinct().ToList();
                if (allApplicationsToday.Any())
                    applicationListToDay.AddRange(allApplicationsToday);

                #endregion File Submitted

                #region File Approved

                loanApplicationsIds = GenService.GetAll<Application>().Where(a => a.Product.ProductType == ProductType.Loan).Select(a => a.Id);

                if (stage == Stages.Files_Approved)
                {
                    approvedLoanApplicationLogsToday = approvedLoanApplicationLogsToday.Where(l => loanApplicationsIds.Contains(l.ApplicationId)).ToList();
                    if (approvedLoanApplicationLogsToday.Count > 0)
                        approvedApplicationListToDay.AddRange(approvedLoanApplicationLogsToday);
                }

                #endregion File Approved

                #region Disbursed/Received

                if (stage == Stages.Files_Disbursed)
                {
                    var acceptableDMidstoday = TodaydisbursementMemos.Select(dm => dm.DisbursementMemoId).ToList();
                    var disMemotoday = GenService.GetAll<DisbursementMemo>().Where(d => acceptableDMidstoday.Contains(d.Id)).ToList();
                    disbursementMemosToday = disMemotoday.ToList();

                    var acceptableFCidstoday = TodayfundReceivedToday.Select(dm => dm.FundConfirmationId).ToList();
                    var fundRectoday = GenService.GetAll<FundConfirmation>().Where(d => acceptableFCidstoday.Contains(d.Id)).ToList();
                    fundReceivedToday = fundRectoday.ToList();
                }

                #endregion Disbursed/Received
            }

            #region initializations

            data.Disbursed_HomeLoanAmount = 0;
            data.Disbursed_PersonalLoanAmount = 0;
            data.Disbursed_AutoLoanAmount = 0;
            data.RMHomeLoan1 = "";
            data.RMHomeLoan1Amount = 0;
            data.RMHomeLoan1Count = 0;
            data.RMHomeLoan2 = "";
            data.RMHomeLoan2Amount = 0;
            data.RMHomeLoan2Count = 0;
            data.RMPersonalLoan = "";
            data.RMPersonalLoanAmount = 0;
            data.RMPersonalLoanCount = 0;
            data.RMAutoLoan = "";
            data.RMAutoLoanAmount = 0;
            data.RMAutoLoanCount = 0;
            data.RMLiability1 = "";
            data.RMLiability1Amount = 0;
            data.RMLiability1Count = 0;
            data.RMLiability2 = "";
            data.RMLiability2Amount = 0;
            data.RMLiability2Count = 0;

            var callListTodayHome = new List<Call>();
            var callListTodayAuto = new List<Call>();
            var callListTodayPersonal = new List<Call>();
            var callListTodayFixedDeposit = new List<Call>();
            var callListTodayRecurrentDeposit = new List<Call>();
            var callListTodayUndefined = new List<Call>();


            var leadListTodayHome = new List<SalesLead>();
            var leadListTodayAuto = new List<SalesLead>();
            var leadListTodayPersonal = new List<SalesLead>();
            var leadListTodayFixedDeposit = new List<SalesLead>();
            var leadListTodayRecurrentDeposit = new List<SalesLead>();

            var applicationListTodayHome = new List<ApplicationLog>();
            var applicationListTodayAuto = new List<ApplicationLog>();
            var applicationListTodayPersonal = new List<ApplicationLog>();
            var applicationListTodayFixedDeposit = new List<ApplicationLog>();
            var applicationListTodayRecurrentDeposit = new List<ApplicationLog>();

            var approvedApplicationListTodayHome = new List<ApplicationLog>();
            var approvedApplicationListTodayAuto = new List<ApplicationLog>();
            var approvedApplicationListTodayPersonal = new List<ApplicationLog>();

            var approvedHomeLoanLogsWithAmountToday = new List<ProductivityMatrixDataUnit>();
            var approvedAutoLoanLogsWithAmountToday = new List<ProductivityMatrixDataUnit>();
            var approvedPersonalLoanLogsWithAmountToday = new List<ProductivityMatrixDataUnit>();

            var disbursementMemosTodayHome = new List<DisbursementMemo>();
            var disbursementMemosTodayAuto = new List<DisbursementMemo>();
            var disbursementMemosTodayPersonal = new List<DisbursementMemo>();
            var fundReceivedTodayFixed = new List<FundConfirmation>();
            var fundReceivedTodayRecurrent = new List<FundConfirmation>();
            #endregion

            #region Call

            if (product == ProductSelection.Home_Loan)
                callListTodayHome = callListToday.Where(c => c.ProductId != null && c.Product.FacilityType == ProposalFacilityType.Home_Loan).ToList();
            else if (product == ProductSelection.Auto_Loan)
                callListTodayAuto = callListToday.Where(c => c.ProductId != null && c.Product.FacilityType == ProposalFacilityType.Auto_Loan).ToList();
            else if (product == ProductSelection.Personal_Loan)
                callListTodayPersonal = callListToday.Where(c => c.ProductId != null && c.Product.FacilityType == ProposalFacilityType.Personal_Loan).ToList();
            else if (product == ProductSelection.Fixed)
                callListTodayFixedDeposit = callListToday.Where(c => c.ProductId != null && c.Product.DepositType == DepositType.Fixed).ToList();
            else if (product == ProductSelection.Recurring)
                callListTodayRecurrentDeposit = callListToday.Where(c => c.ProductId != null && c.Product.DepositType == DepositType.Recurring).ToList();
            else
                callListTodayUndefined = callListToday.ToList();

            #endregion Call

            #region Lead

            if (product == ProductSelection.Home_Loan)
                leadListTodayHome = leadListToday.Where(l => l.ProductId != null && l.Product.FacilityType == ProposalFacilityType.Home_Loan).ToList();
            else if (product == ProductSelection.Auto_Loan)
                leadListTodayAuto = leadListToday.Where(l => l.ProductId != null && l.Product.FacilityType == ProposalFacilityType.Auto_Loan).ToList();
            else if (product == ProductSelection.Personal_Loan)
                leadListTodayPersonal = leadListToday.Where(l => l.ProductId != null && l.Product.FacilityType == ProposalFacilityType.Personal_Loan).ToList();
            else if (product == ProductSelection.Fixed)
                leadListTodayFixedDeposit = leadListToday.Where(l => l.ProductId != null && l.Product.DepositType == DepositType.Fixed).ToList();
            else if (product == ProductSelection.Recurring)
                leadListTodayRecurrentDeposit = leadListToday.Where(l => l.ProductId != null && l.Product.DepositType == DepositType.Recurring).ToList();

            #endregion Lead

            #region File Submited Apps.

            if (product == ProductSelection.Home_Loan)
                applicationListTodayHome = applicationListToDay.Where(l => l.Application.LoanApplicationId != null && l.Application.ProductId != null && l.Application.Product.FacilityType == ProposalFacilityType.Home_Loan).ToList();
            else if (product == ProductSelection.Auto_Loan)
                applicationListTodayAuto = applicationListToDay.Where(l => l.Application.LoanApplicationId != null && l.Application.ProductId != null && l.Application.Product.FacilityType == ProposalFacilityType.Auto_Loan).ToList();
            else if (product == ProductSelection.Personal_Loan)
                applicationListTodayPersonal = applicationListToDay.Where(l => l.Application.LoanApplicationId != null && l.Application.ProductId != null && l.Application.Product.FacilityType == ProposalFacilityType.Personal_Loan).ToList();
            else if (product == ProductSelection.Fixed)
                applicationListTodayFixedDeposit = applicationListToDay.Where(l => l.Application.DepositApplicationId != null && l.Application.ProductId != null && l.Application.Product.DepositType == DepositType.Fixed).ToList();
            else if (product == ProductSelection.Recurring)
                applicationListTodayRecurrentDeposit = applicationListToDay.Where(l => l.Application.DepositApplicationId != null && l.Application.ProductId != null && l.Application.Product.DepositType == DepositType.Recurring).ToList();

            #endregion File Submited Apps.

            #region File Approved Apps.

            if (product == ProductSelection.Home_Loan)
            {

                approvedApplicationListTodayHome = approvedApplicationListToDay.Where(l => l.Application.LoanApplicationId != null && l.Application.ProductId != null && l.Application.Product.FacilityType != null && l.Application.Product.FacilityType == ProposalFacilityType.Home_Loan).ToList();

                if (approvedApplicationListTodayHome.Count > 0)
                {
                    //approvedHomeLoanLogsWithAmountToday = approvedHomeLoanLogsWithAmountToday.Where(p => approvedApplicationListTodayHome.Any(a => a.ApplicationId == p.ApplicationId)).ToList();
                    approvedHomeLoanLogsWithAmountToday = (from logs in approvedApplicationListTodayHome
                                                           join proposal in GenService.GetAll<Proposal>().Where(p => p.Status == EntityStatus.Active) on
                                                               logs.ApplicationId equals proposal.ApplicationId into proposals
                                                           from p in proposals.DefaultIfEmpty()
                                                           select new ProductivityMatrixDataUnit
                                                           {
                                                               Date = logs.CreateDate,
                                                               Value = p.RecomendedLoanAmountFromIPDC
                                                           }).ToList();
                }
            }

            if (product == ProductSelection.Auto_Loan)
            {
                approvedApplicationListTodayAuto = approvedApplicationListToDay.Where(l => l.Application.LoanApplicationId != null && l.Application.ProductId != null && l.Application.Product.FacilityType != null && l.Application.Product.FacilityType == ProposalFacilityType.Auto_Loan).ToList();

                if (approvedApplicationListTodayAuto.Count > 0)
                {
                    //approvedAutoLoanLogsWithAmountToday = GenService.GetAll<Proposal>().Where(p => approvedApplicationListTodayAuto.Any(a => a.ApplicationId == p.ApplicationId)).ToList();
                    approvedAutoLoanLogsWithAmountToday = (from logs in approvedApplicationListTodayAuto
                                                           join proposal in GenService.GetAll<Proposal>().Where(p => p.Status == EntityStatus.Active) on
                                                               logs.ApplicationId equals proposal.ApplicationId into proposals
                                                           from p in proposals.DefaultIfEmpty()
                                                           select new ProductivityMatrixDataUnit
                                                           {
                                                               Date = logs.CreateDate,
                                                               Value = p.RecomendedLoanAmountFromIPDC
                                                           }).ToList();
                }
            }

            if (product == ProductSelection.Auto_Loan)
            {

                approvedApplicationListTodayPersonal = approvedApplicationListToDay.Where(l => l.Application.LoanApplicationId != null && l.Application.ProductId != null && l.Application.Product.FacilityType != null && l.Application.Product.FacilityType == ProposalFacilityType.Personal_Loan).ToList();

                if (approvedApplicationListTodayPersonal.Count > 0)
                {
                    //approvedPersonalLoanLogsWithAmountToday = GenService.GetAll<Proposal>().Where(p => approvedApplicationListTodayPersonal.Any(a => a.ApplicationId == p.ApplicationId)).ToList();
                    approvedPersonalLoanLogsWithAmountToday = (from logs in approvedApplicationListTodayPersonal
                                                               join proposal in GenService.GetAll<Proposal>().Where(p => p.Status == EntityStatus.Active) on
                                                                   logs.ApplicationId equals proposal.ApplicationId into proposals
                                                               from p in proposals.DefaultIfEmpty()
                                                               select new ProductivityMatrixDataUnit
                                                               {
                                                                   Date = logs.CreateDate,
                                                                   Value = p.RecomendedLoanAmountFromIPDC
                                                               }).ToList();
                }
            }


            #endregion File Approved Apps.

            #region Disbursed/Received
            if (product == ProductSelection.Home_Loan)
            {

                disbursementMemosTodayHome = disbursementMemosToday.Where(d => d.Application.Product.FacilityType == ProposalFacilityType.Home_Loan).ToList();
            }

            if (product == ProductSelection.Auto_Loan)
            {

                disbursementMemosTodayAuto = disbursementMemosToday.Where(d => d.Application.Product.FacilityType == ProposalFacilityType.Auto_Loan).ToList();
            }

            if (product == ProductSelection.Personal_Loan)
            {

                disbursementMemosTodayPersonal = disbursementMemosToday.Where(d => d.Application.Product.FacilityType == ProposalFacilityType.Personal_Loan).ToList();
            }

            if (product == ProductSelection.Fixed)
            {

                fundReceivedTodayFixed = fundReceivedToday.Where(d => d.Application.Product.DepositType == DepositType.Fixed).ToList();
            }
            if (product == ProductSelection.Recurring)
            {

                fundReceivedTodayRecurrent = fundReceivedToday.Where(d => d.Application.Product.DepositType == DepositType.Recurring).ToList();
            }
            #endregion
            List<ProductivityMatrixDataUnit> ChartData = new List<ProductivityMatrixDataUnit>();

            if (criteria == Criteria.Amount)
            {
                #region Call
                if (stage == Stages.Call)
                {
                    if (product == ProductSelection.Home_Loan)
                        callListToday = callListToday.Where(c => c.ProductId != null && c.Product.FacilityType == ProposalFacilityType.Home_Loan).ToList();
                    else if (product == ProductSelection.Auto_Loan)
                        callListToday = callListToday.Where(c => c.ProductId != null && c.Product.FacilityType == ProposalFacilityType.Auto_Loan).ToList();
                    else if (product == ProductSelection.Personal_Loan)
                        callListToday = callListToday.Where(c => c.ProductId != null && c.Product.FacilityType == ProposalFacilityType.Personal_Loan).ToList();
                    else if (product == ProductSelection.Fixed)
                        callListToday = callListToday.Where(c => c.ProductId != null && c.Product.DepositType == DepositType.Fixed).ToList();
                    else if (product == ProductSelection.Recurring)
                        callListToday = callListToday.Where(c => c.ProductId != null && c.Product.DepositType == DepositType.Recurring).ToList();


                    ChartData = callListToday.GroupBy(c => new { Month = c.CreateDate.Value.Month, Year = c.CreateDate.Value.Year })
                        .Select(c => new ProductivityMatrixDataUnit
                        {
                            Month = c.FirstOrDefault().CreateDate.Value.ToString("MMM"),
                            Year = c.FirstOrDefault().CreateDate.Value.ToString("yyyy"),
                            Date = c.FirstOrDefault().CreateDate.Value.AddDays(1 - c.FirstOrDefault().CreateDate.Value.Day),
                            Value = c.Sum(s => s.Amount != null ? s.Amount : 0),
                            CompareValue = c.Where(s => s.CallStatus == CallStatus.Successful).Sum(s => s.Amount != null ? s.Amount : 0)
                        }).ToList();
                }
                #endregion Call

                #region Lead
                else if (stage == Stages.Lead)
                {
                    if (product == ProductSelection.Home_Loan)
                        leadListToday = leadListToday.Where(c => c.ProductId != null && c.Product.FacilityType == ProposalFacilityType.Home_Loan).ToList();
                    else if (product == ProductSelection.Auto_Loan)
                        leadListToday = leadListToday.Where(c => c.ProductId != null && c.Product.FacilityType == ProposalFacilityType.Auto_Loan).ToList();
                    else if (product == ProductSelection.Personal_Loan)
                        leadListToday = leadListToday.Where(c => c.ProductId != null && c.Product.FacilityType == ProposalFacilityType.Personal_Loan).ToList();
                    else if (product == ProductSelection.Fixed)
                        leadListToday = leadListToday.Where(c => c.ProductId != null && c.Product.DepositType == DepositType.Fixed).ToList();
                    else if (product == ProductSelection.Recurring)
                        leadListToday = leadListToday.Where(c => c.ProductId != null && c.Product.DepositType == DepositType.Recurring).ToList();


                    ChartData = leadListToday.GroupBy(c => new { Month = c.CreateDate.Value.Month, Year = c.CreateDate.Value.Year })
                        .Select(c => new ProductivityMatrixDataUnit
                        {
                            Month = c.FirstOrDefault().CreateDate.Value.ToString("MMM"),
                            Year = c.FirstOrDefault().CreateDate.Value.ToString("yyyy"),
                            Date = c.FirstOrDefault().CreateDate.Value.AddDays(1 - c.FirstOrDefault().CreateDate.Value.Day),
                            Value = c.Sum(s => s.Amount != null ? s.Amount : 0),
                            CompareValue = c.Where(s => s.CallStatus == CallStatus.Successful).Sum(s => s.Amount != null ? s.Amount : 0)
                        }).ToList();
                }

                #endregion Lead

                #region File Submited Apps.
                else if (stage == Stages.Files_Submitted)
                {
                    if (product == ProductSelection.Home_Loan)
                        applicationListToDay = applicationListToDay.Where(l => l.Application.LoanApplicationId != null && l.Application.Product.FacilityType == ProposalFacilityType.Home_Loan).ToList();
                    else if (product == ProductSelection.Auto_Loan)
                        applicationListToDay = applicationListToDay.Where(l => l.Application.LoanApplicationId != null && l.Application.Product.FacilityType == ProposalFacilityType.Auto_Loan).ToList();
                    else if (product == ProductSelection.Personal_Loan)
                        applicationListToDay = applicationListToDay.Where(l => l.Application.LoanApplicationId != null && l.Application.Product.FacilityType == ProposalFacilityType.Personal_Loan).ToList();
                    else if (product == ProductSelection.Fixed)
                        applicationListToDay = applicationListToDay.Where(l => l.Application.DepositApplicationId != null && l.Application.Product.DepositType == DepositType.Fixed).ToList();
                    else if (product == ProductSelection.Recurring)
                        applicationListToDay = applicationListToDay.Where(l => l.Application.DepositApplicationId != null && l.Application.Product.DepositType == DepositType.Recurring).ToList();


                    ChartData = applicationListToDay.GroupBy(c => new { Month = c.CreateDate.Value.Month, Year = c.CreateDate.Value.Year })
                        .Select(c => new ProductivityMatrixDataUnit
                        {
                            Month = c.FirstOrDefault().CreateDate.Value.ToString("MMM"),
                            Year = c.FirstOrDefault().CreateDate.Value.ToString("yyyy"),
                            Date = c.FirstOrDefault().CreateDate.Value.AddDays(1 - c.FirstOrDefault().CreateDate.Value.Day),
                            Value = c.Sum(s => s.Application.LoanApplication.LoanAmountApplied),
                            CompareValue = 0// c.Where(s => s.CallStatus == CallStatus.Successful).Sum(s => s.Amount != null ? s.Amount : 0)
                        }).ToList();
                }
                #endregion File Submited Apps.

                #region File Approved Apps.
                else if (stage == Stages.Files_Approved)
                {
                    if (product == ProductSelection.Home_Loan)
                    {
                        ChartData = approvedHomeLoanLogsWithAmountToday.GroupBy(c => new { Month = c.Date.Value.Month, Year = c.Date.Value.Year })
                        .Select(c => new ProductivityMatrixDataUnit
                        {
                            Month = c.FirstOrDefault().Date.Value.ToString("MMM"),
                            Year = c.FirstOrDefault().Date.Value.ToString("yyyy"),
                            Date = c.FirstOrDefault().Date.Value.AddDays(1 - c.FirstOrDefault().Date.Value.Day),
                            Value = c.Sum(s => s.Value != null ? s.Value : 0),
                            CompareValue = 0//c.Where(s => s.CallStatus == CallStatus.Successful).Sum(s => s.Amount != null ? s.Amount : 0)
                        }).ToList();
                    }
                    else if (product == ProductSelection.Auto_Loan)
                    {
                        ChartData = approvedAutoLoanLogsWithAmountToday.GroupBy(c => new { Month = c.Date.Value.Month, Year = c.Date.Value.Year })
                        .Select(c => new ProductivityMatrixDataUnit
                        {
                            Month = c.FirstOrDefault().Date.Value.ToString("MMM"),
                            Year = c.FirstOrDefault().Date.Value.ToString("yyyy"),
                            Date = c.FirstOrDefault().Date.Value.AddDays(1 - c.FirstOrDefault().Date.Value.Day),
                            Value = c.Sum(s => s.Value != null ? s.Value : 0),
                            CompareValue = 0//c.Where(s => s.CallStatus == CallStatus.Successful).Sum(s => s.Amount != null ? s.Amount : 0)
                        }).ToList();
                    }
                    else if (product == ProductSelection.Personal_Loan)
                    {
                        ChartData = approvedPersonalLoanLogsWithAmountToday.GroupBy(c => new { Month = c.Date.Value.Month, Year = c.Date.Value.Year })
                        .Select(c => new ProductivityMatrixDataUnit
                        {
                            Month = c.FirstOrDefault().Date.Value.ToString("MMM"),
                            Year = c.FirstOrDefault().Date.Value.ToString("yyyy"),
                            Date = c.FirstOrDefault().Date.Value.AddDays(1 - c.FirstOrDefault().Date.Value.Day),
                            Value = c.Sum(s => s.Value != null ? s.Value : 0),
                            CompareValue = 0//c.Where(s => s.CallStatus == CallStatus.Successful).Sum(s => s.Amount != null ? s.Amount : 0)
                        }).ToList();
                    }
                }
                #endregion File Approved Apps.

                #region Disbursed/Received
                else if (stage == Stages.Files_Disbursed)
                {
                    if (product == ProductSelection.Home_Loan)
                    {
                        ChartData = disbursedMemodetails.Where(d => disbursementMemosToday.Any(dis => dis.Id == d.DisbursementMemoId && dis.Application.Product.FacilityType == ProposalFacilityType.Home_Loan))
                            .GroupBy(c => new { Month = c.CreateDate.Value.Month, Year = c.CreateDate.Value.Year })
                        .Select(c => new ProductivityMatrixDataUnit
                        {
                            Month = c.FirstOrDefault().CreateDate.Value.ToString("MMM"),
                            Year = c.FirstOrDefault().CreateDate.Value.ToString("yyyy"),
                            Date = c.FirstOrDefault().CreateDate.Value.AddDays(1 - c.FirstOrDefault().CreateDate.Value.Day),
                            Value = c.Sum(s => s.DisbursementAmount),
                            CompareValue = 0//c.Where(s => s.CallStatus == CallStatus.Successful).Sum(s => s.Amount != null ? s.Amount : 0)
                        }).ToList();
                    }
                    else if (product == ProductSelection.Auto_Loan)
                    {
                        ChartData = disbursedMemodetails.Where(d => disbursementMemosToday.Any(dis => dis.Id == d.DisbursementMemoId && dis.Application.Product.FacilityType == ProposalFacilityType.Home_Loan))
                            .GroupBy(c => new { Month = c.CreateDate.Value.Month, Year = c.CreateDate.Value.Year })
                        .Select(c => new ProductivityMatrixDataUnit
                        {
                            Month = c.FirstOrDefault().CreateDate.Value.ToString("MMM"),
                            Year = c.FirstOrDefault().CreateDate.Value.ToString("yyyy"),
                            Date = c.FirstOrDefault().CreateDate.Value.AddDays(1 - c.FirstOrDefault().CreateDate.Value.Day),
                            Value = c.Sum(s => s.DisbursementAmount),
                            CompareValue = 0//c.Where(s => s.CallStatus == CallStatus.Successful).Sum(s => s.Amount != null ? s.Amount : 0)
                        }).ToList();
                    }
                    else if (product == ProductSelection.Personal_Loan)
                    {
                        ChartData = disbursedMemodetails.Where(d => disbursementMemosToday.Any(dis => dis.Id == d.DisbursementMemoId && dis.Application.Product.FacilityType == ProposalFacilityType.Home_Loan))
                            .GroupBy(c => new { Month = c.CreateDate.Value.Month, Year = c.CreateDate.Value.Year })
                        .Select(c => new ProductivityMatrixDataUnit
                        {
                            Month = c.FirstOrDefault().CreateDate.Value.ToString("MMM"),
                            Year = c.FirstOrDefault().CreateDate.Value.ToString("yyyy"),
                            Date = c.FirstOrDefault().CreateDate.Value.AddDays(1 - c.FirstOrDefault().CreateDate.Value.Day),
                            Value = c.Sum(s => s.DisbursementAmount),
                            CompareValue = 0// c.Where(s => s.CallStatus == CallStatus.Successful).Sum(s => s.Amount != null ? s.Amount : 0)
                        }).ToList();
                    }
                }
                //if (disbursementMemosTodayHome.Count() > 0)
                //    data.Disbursed_HomeLoanAmountToday = disbursedMemodetails.Where(d => disbursementMemosToday.Any(dis => dis.Id == d.DisbursementMemoId && dis.Application.Product.FacilityType == ProposalFacilityType.Home_Loan)).Sum(d => d.DisbursementAmount);


                //if (disbursementMemosTodayAuto.Count() > 0)
                //    data.Disbursed_AutoLoanAmountToday = disbursedMemodetails.Where(d => disbursementMemosToday.Any(dis => dis.Id == d.DisbursementMemoId && dis.Application.Product.FacilityType == ProposalFacilityType.Auto_Loan)).Sum(d => d.DisbursementAmount);


                //if (disbursementMemosTodayPersonal.Count() > 0)
                //    data.Disbursed_PersonalLoanAmountToday = disbursedMemodetails.Where(d => disbursementMemosToday.Any(dis => dis.Id == d.DisbursementMemoId && dis.Application.Product.FacilityType == ProposalFacilityType.Personal_Loan)).Sum(d => d.DisbursementAmount);


                //if (fundReceivedTodayFixed.Count() > 0)
                //    data.FundReceived_FixedAmountToday = (decimal)fundConfirmdetails.Where(d => fundReceivedToday.Any(dis => dis.Id == d.FundConfirmationId && dis.Application.Product.DepositType == DepositType.Fixed)).Sum(d => d.Amount);


                //if (fundReceivedTodayRecurrent.Count() > 0)
                //    data.FundReceived_RecurrentAmountToday = (decimal)fundConfirmdetails.Where(d => fundReceivedToday.Any(dis => dis.Id == d.FundConfirmationId && dis.Application.Product.DepositType == DepositType.Recurring)).Sum(d => d.Amount);

                #endregion Disbursed/Received
            }
            else
            {
                #region Call
                if (stage == Stages.Call)
                {
                    if (product == ProductSelection.Home_Loan)
                        callListToday = callListToday.Where(c => c.ProductId != null && c.Product.FacilityType == ProposalFacilityType.Home_Loan).ToList();
                    else if (product == ProductSelection.Auto_Loan)
                        callListToday = callListToday.Where(c => c.ProductId != null && c.Product.FacilityType == ProposalFacilityType.Auto_Loan).ToList();
                    else if (product == ProductSelection.Personal_Loan)
                        callListToday = callListToday.Where(c => c.ProductId != null && c.Product.FacilityType == ProposalFacilityType.Personal_Loan).ToList();
                    else if (product == ProductSelection.Fixed)
                        callListToday = callListToday.Where(c => c.ProductId != null && c.Product.DepositType == DepositType.Fixed).ToList();
                    else if (product == ProductSelection.Recurring)
                        callListToday = callListToday.Where(c => c.ProductId != null && c.Product.DepositType == DepositType.Recurring).ToList();


                    ChartData = callListToday.GroupBy(c => new { Month = c.CreateDate.Value.Month, Year = c.CreateDate.Value.Year })
                        .Select(c => new ProductivityMatrixDataUnit
                        {
                            Month = c.FirstOrDefault().CreateDate.Value.ToString("MMM"),
                            Year = c.FirstOrDefault().CreateDate.Value.ToString("yyyy"),
                            Date = c.FirstOrDefault().CreateDate.Value.AddDays(1 - c.FirstOrDefault().CreateDate.Value.Day),
                            Value = c.Count(),
                            CompareValue = c.Where(s => s.CallStatus == CallStatus.Successful).Count()
                        }).ToList();
                }
                #endregion Call

                #region Lead
                else if (stage == Stages.Lead)
                {
                    if (product == ProductSelection.Home_Loan)
                        leadListToday = leadListToday.Where(c => c.ProductId != null && c.Product.FacilityType == ProposalFacilityType.Home_Loan).ToList();
                    else if (product == ProductSelection.Auto_Loan)
                        leadListToday = leadListToday.Where(c => c.ProductId != null && c.Product.FacilityType == ProposalFacilityType.Auto_Loan).ToList();
                    else if (product == ProductSelection.Personal_Loan)
                        leadListToday = leadListToday.Where(c => c.ProductId != null && c.Product.FacilityType == ProposalFacilityType.Personal_Loan).ToList();
                    else if (product == ProductSelection.Fixed)
                        leadListToday = leadListToday.Where(c => c.ProductId != null && c.Product.DepositType == DepositType.Fixed).ToList();
                    else if (product == ProductSelection.Recurring)
                        leadListToday = leadListToday.Where(c => c.ProductId != null && c.Product.DepositType == DepositType.Recurring).ToList();


                    ChartData = leadListToday.GroupBy(c => new { Month = c.CreateDate.Value.Month, Year = c.CreateDate.Value.Year })
                        .Select(c => new ProductivityMatrixDataUnit
                        {
                            Month = c.FirstOrDefault().CreateDate.Value.ToString("MMM"),
                            Year = c.FirstOrDefault().CreateDate.Value.ToString("yyyy"),
                            Date = c.FirstOrDefault().CreateDate.Value.AddDays(1 - c.FirstOrDefault().CreateDate.Value.Day),
                            Value = c.Count(),
                            CompareValue = c.Where(s => s.CallStatus == CallStatus.Successful).Count()
                        }).ToList();
                }
                #endregion Lead

                #region File Submited Apps
                else if (stage == Stages.Files_Submitted)
                {
                    if (product == ProductSelection.Home_Loan)
                        applicationListToDay = applicationListToDay.Where(l => l.Application.LoanApplicationId != null && l.Application.Product.FacilityType == ProposalFacilityType.Home_Loan).ToList();
                    else if (product == ProductSelection.Auto_Loan)
                        applicationListToDay = applicationListToDay.Where(l => l.Application.LoanApplicationId != null && l.Application.Product.FacilityType == ProposalFacilityType.Auto_Loan).ToList();
                    else if (product == ProductSelection.Personal_Loan)
                        applicationListToDay = applicationListToDay.Where(l => l.Application.LoanApplicationId != null && l.Application.Product.FacilityType == ProposalFacilityType.Personal_Loan).ToList();
                    else if (product == ProductSelection.Fixed)
                        applicationListToDay = applicationListToDay.Where(l => l.Application.DepositApplicationId != null && l.Application.Product.DepositType == DepositType.Fixed).ToList();
                    else if (product == ProductSelection.Recurring)
                        applicationListToDay = applicationListToDay.Where(l => l.Application.DepositApplicationId != null && l.Application.Product.DepositType == DepositType.Recurring).ToList();


                    ChartData = applicationListToDay.GroupBy(c => new { Month = c.CreateDate.Value.Month, Year = c.CreateDate.Value.Year })
                        .Select(c => new ProductivityMatrixDataUnit
                        {
                            Month = c.FirstOrDefault().CreateDate.Value.ToString("MMM"),
                            Year = c.FirstOrDefault().CreateDate.Value.ToString("yyyy"),
                            Date = c.FirstOrDefault().CreateDate.Value.AddDays(1 - c.FirstOrDefault().CreateDate.Value.Day),
                            Value = c.Count(),
                            CompareValue = 0//c.Where(s => s.CallStatus == CallStatus.Successful).Sum(s => s.Amount != null ? s.Amount : 0)
                        }).ToList();
                }
                #endregion File Submited Apps

                #region File Approved Apps
                else if (stage == Stages.Files_Approved)
                {
                    if (product == ProductSelection.Home_Loan)
                    {
                        ChartData = approvedApplicationListTodayHome.GroupBy(c => new { Month = c.CreateDate.Value.Month, Year = c.CreateDate.Value.Year })
                        .Select(c => new ProductivityMatrixDataUnit
                        {
                            Month = c.FirstOrDefault().CreateDate.Value.ToString("MMM"),
                            Year = c.FirstOrDefault().CreateDate.Value.ToString("yyyy"),
                            Date = c.FirstOrDefault().CreateDate.Value.AddDays(1 - c.FirstOrDefault().CreateDate.Value.Day),
                            Value = c.Count(),
                            CompareValue = 0//c.Where(s => s.CallStatus == CallStatus.Successful).Sum(s => s.Amount != null ? s.Amount : 0)
                        }).ToList();
                    }
                    else if (product == ProductSelection.Auto_Loan)
                    {
                        ChartData = approvedApplicationListTodayAuto.GroupBy(c => new { Month = c.CreateDate.Value.Month, Year = c.CreateDate.Value.Year })
                        .Select(c => new ProductivityMatrixDataUnit
                        {
                            Month = c.FirstOrDefault().CreateDate.Value.ToString("MMM"),
                            Year = c.FirstOrDefault().CreateDate.Value.ToString("yyyy"),
                            Date = c.FirstOrDefault().CreateDate.Value.AddDays(1 - c.FirstOrDefault().CreateDate.Value.Day),
                            Value = c.Count(),
                            CompareValue = 0//c.Where(s => s.CallStatus == CallStatus.Successful).Sum(s => s.Amount != null ? s.Amount : 0)
                        }).ToList();
                    }
                    else if (product == ProductSelection.Personal_Loan)
                    {
                        ChartData = approvedApplicationListTodayPersonal.GroupBy(c => new { Month = c.CreateDate.Value.Month, Year = c.CreateDate.Value.Year })
                        .Select(c => new ProductivityMatrixDataUnit
                        {
                            Month = c.FirstOrDefault().CreateDate.Value.ToString("MMM"),
                            Year = c.FirstOrDefault().CreateDate.Value.ToString("yyyy"),
                            Date = c.FirstOrDefault().CreateDate.Value.AddDays(1 - c.FirstOrDefault().CreateDate.Value.Day),
                            Value = c.Count(),
                            CompareValue = 0//c.Where(s => s.CallStatus == CallStatus.Successful).Sum(s => s.Amount != null ? s.Amount : 0)
                        }).ToList();
                    }
                }

                #endregion File Approved Apps

                #region Disbursed/Received
                else if (stage == Stages.Files_Disbursed)
                {
                    if (product == ProductSelection.Home_Loan)
                    {
                        ChartData = disbursedMemodetails.Where(d => disbursementMemosToday.Any(dis => dis.Id == d.DisbursementMemoId && dis.Application.Product.FacilityType == ProposalFacilityType.Home_Loan))
                            .GroupBy(c => new { Month = c.CreateDate.Value.Month, Year = c.CreateDate.Value.Year, c.DisbursementMemoId })
                        .Select(c => new ProductivityMatrixDataUnit
                        {
                            Month = c.FirstOrDefault().CreateDate.Value.ToString("MMM"),
                            Year = c.FirstOrDefault().CreateDate.Value.ToString("yyyy"),
                            Date = c.FirstOrDefault().CreateDate.Value.AddDays(1 - c.FirstOrDefault().CreateDate.Value.Day),
                            Value = c.Count(),
                            CompareValue = 0//c.Where(s => s.CallStatus == CallStatus.Successful).Sum(s => s.Amount != null ? s.Amount : 0)
                        }).ToList();
                    }
                    else if (product == ProductSelection.Auto_Loan)
                    {
                        ChartData = disbursedMemodetails.Where(d => disbursementMemosToday.Any(dis => dis.Id == d.DisbursementMemoId && dis.Application.Product.FacilityType == ProposalFacilityType.Home_Loan))
                            .GroupBy(c => new { Month = c.CreateDate.Value.Month, Year = c.CreateDate.Value.Year })
                        .Select(c => new ProductivityMatrixDataUnit
                        {
                            Month = c.FirstOrDefault().CreateDate.Value.ToString("MMM"),
                            Year = c.FirstOrDefault().CreateDate.Value.ToString("yyyy"),
                            Date = c.FirstOrDefault().CreateDate.Value.AddDays(1 - c.FirstOrDefault().CreateDate.Value.Day),
                            Value = c.Count(),
                            CompareValue = 0//c.Where(s => s.CallStatus == CallStatus.Successful).Sum(s => s.Amount != null ? s.Amount : 0)
                        }).ToList();
                    }
                    else if (product == ProductSelection.Personal_Loan)
                    {
                        ChartData = disbursedMemodetails.Where(d => disbursementMemosToday.Any(dis => dis.Id == d.DisbursementMemoId && dis.Application.Product.FacilityType == ProposalFacilityType.Home_Loan))
                            .GroupBy(c => new { Month = c.CreateDate.Value.Month, Year = c.CreateDate.Value.Year })
                        .Select(c => new ProductivityMatrixDataUnit
                        {
                            Month = c.FirstOrDefault().CreateDate.Value.ToString("MMM"),
                            Year = c.FirstOrDefault().CreateDate.Value.ToString("yyyy"),
                            Date = c.FirstOrDefault().CreateDate.Value.AddDays(1 - c.FirstOrDefault().CreateDate.Value.Day),
                            Value = c.Count(),
                            CompareValue = 0//c.Where(s => s.CallStatus == CallStatus.Successful).Sum(s => s.Amount != null ? s.Amount : 0)
                        }).ToList();
                    }
                }
                //data.Disbursed_HomeLoanToDay = disbursementMemosTodayHome.Count();

                //data.Disbursed_AutoLoanToDay = disbursementMemosTodayAuto.Count();

                //data.Disbursed_PersonalLoanToDay = disbursementMemosTodayPersonal.Count();

                //data.Received_FixedToDay = fundReceivedTodayFixed.Count();

                //data.Received_RecurrentToDay = fundReceivedTodayRecurrent.Count();

                #endregion Disbursed/Received
            }
            data.ChartData = ChartData;
            return data;
        }
        public object GetPMDashboard(DateTime? fromDate, DateTime? toDate, Stages? stage, Criteria? criteria, long? costCenterId, List<ProductSelection?> product, List<long?> branchId)
        {
            //long UserId,
            var data = new DashBoardHighlightsDto();

            var allApplications = GenService.GetAll<Application>().Where(a => a.Status == EntityStatus.Active);

            var loanApplicationsIds = allApplications.Where(a => a.Product.ProductType == ProductType.Loan).Select(a => a.Id);

            var callListToday = new List<Call>();

            var leadListToday = new List<SalesLead>();

            var applicationListToDay = new List<ApplicationLog>();

            var approvedApplicationListToDay = new List<ApplicationLog>();

            var disbursementMemosToday = new List<DisbursementMemo>();

            var fundReceivedToday = new List<FundConfirmation>();

            var disbursedMemodetails = new List<DMDetail>();
            disbursedMemodetails = GenService.GetAll<DMDetail>().Where(dm => dm.Status == EntityStatus.Active).ToList();
            var fundConfirmdetails = new List<FundConfirmationDetail>();
            fundConfirmdetails = GenService.GetAll<FundConfirmationDetail>().Where(fc => fc.Status == EntityStatus.Active).ToList();

            #region Call

            IQueryable<Call> allCallsToday = GenService.GetAll<Call>().Where(c =>
            c.CreateDate <= toDate
            && c.CreateDate >= fromDate
            && c.Amount != null && c.Status == EntityStatus.Active);

            #endregion Call

            #region Lead

            IQueryable<SalesLead> allLeadsToday = GenService.GetAll<SalesLead>().Where(c =>
            c.CreateDate <= toDate && c.CreateDate >= fromDate &&
            c.Amount != null && c.Status == EntityStatus.Active);

            #endregion Lead

            #region File Submited Apps

            var acceptableLogsToday = GenService.GetAll<ApplicationLog>()
                .Where(l => (l.Application.LoanApplicationId != null || l.Application.DepositApplicationId != null)
                && l.Status == EntityStatus.Active
                && l.CreateDate <= toDate
                && l.CreateDate >= fromDate
                && (((l.ToStage == ApplicationStage.SentToCRM || l.ToStage == ApplicationStage.UnderProcessAtCRM)
                && l.Application.Product.ProductType == ProductType.Loan)
                || ((l.ToStage == ApplicationStage.SentToOperations || l.ToStage == ApplicationStage.UnderProcessAtOperations)
                && l.Application.Product.ProductType == ProductType.Deposit)));


            #endregion File Submited Apps

            #region File Approved

            var approvedLoanApplicationLogsToday = GenService.GetAll<ApplicationLog>()
                .Where(l => l.Status == EntityStatus.Active &&
                            l.ToStage == ApplicationStage.SentToOperations
                            && l.CreateDate <= toDate
                            && l.CreateDate >= fromDate
                            ).ToList();


            #endregion File Approved

            #region Amount disbursed / received Today


            var TodaydisbursementMemos = GenService.GetAll<DMDetail>().Where(d => d.DisbursementMemo.ApplicationId != null
                        && d.DisbursementMemo.Status == EntityStatus.Active
                        && d.DisbursementMemo.Application.LoanApplicationId != null
                        && d.DisbursementMemo.Application.Product.FacilityType != null
                        && (d.DisbursementMemo.Application.Product.FacilityType == ProposalFacilityType.Home_Loan
                            || d.DisbursementMemo.Application.Product.FacilityType == ProposalFacilityType.Auto_Loan
                            || d.DisbursementMemo.Application.Product.FacilityType == ProposalFacilityType.Personal_Loan)
                        && d.CreateDate <= toDate
                        && d.CreateDate >= fromDate
                        && d.DisbursementMemo.IsApproved != null
                        && d.DisbursementMemo.IsApproved == true
                        && d.DisbursementMemo.IsDisbursed != null
                        && d.DisbursementMemo.IsDisbursed == true
                        && d.Status == EntityStatus.Active).ToList();

            var TodayfundReceivedToday = GenService.GetAll<FundConfirmationDetail>().Where(d => d.FundConfirmation.ApplicationId != null
                        && d.FundConfirmation.Application.DepositApplicationId != null
                        && d.FundConfirmation.Application.Product.DepositType != null
                        && (d.FundConfirmation.Application.Product.DepositType == DepositType.Fixed
                            || d.FundConfirmation.Application.Product.DepositType == DepositType.Recurring)
                        && d.CreateDate <= toDate
                        && d.CreateDate >= fromDate
                        && d.FundConfirmation.FundReceived != null
                        && d.FundConfirmation.FundReceived == true
                        && d.Status == EntityStatus.Active).ToList();


            #endregion Amount disbursed / received Today

            #region With Brach and cost center

            if ((branchId != null && branchId.Count > 0) && (costCenterId != null || costCenterId > 0))
            {
                List<EmployeeDto> subordinateEmpList = new List<EmployeeDto>();
                foreach (var eachBranch in branchId)
                {
                    if (eachBranch > 0)
                    {
                        var temp = _office.GetEmployeesByOfficeId((long)eachBranch);
                        if (temp != null)
                            subordinateEmpList.AddRange(temp);
                    }
                }
                var subordinateUserList = new List<long>();
                foreach (var sub in subordinateEmpList)
                {
                    subordinateUserList.Add((long)_user.GetUserByEmployeeId((long)sub.Id).Id);
                }

                //allApplications = allApplications.Where(a => subordinateEmpIdList.Contains(a.RMId));
                #region Call
                if (stage == Stages.Call)
                {
                    allCallsToday = allCallsToday.Where(c => subordinateUserList.Contains((long)c.CreatedBy));
                    if (allCallsToday.Any())
                        callListToday.AddRange(allCallsToday);
                }
                #endregion Call

                #region Lead

                else if (stage == Stages.Lead)
                {

                    allLeadsToday = allLeadsToday.Where(l => subordinateUserList.Contains((long)l.CreatedBy));
                    if (allLeadsToday.Any())
                        leadListToday.AddRange(allLeadsToday);
                }

                #endregion Lead

                #region File Submitted

                if (stage == Stages.Files_Submitted)
                {
                    //acceptableLogsToday = acceptableLogsToday.Where(l => allApplications.Any(a => a.Id == l.ApplicationId));
                    acceptableLogsToday = acceptableLogsToday.Where(l => (l.Application.BranchId != null && branchId.Contains(l.Application.BranchId)) && (l.Application.CostCenterId != null && l.Application.CostCenterId == costCenterId));
                    var allApplicationsToday = (from appLog in acceptableLogsToday
                                                orderby appLog.Id
                                                select appLog).Distinct().ToList();
                    if (allApplicationsToday.Any())
                        applicationListToDay.AddRange(allApplicationsToday);
                }

                #endregion File Submitted

                #region File Approved

                //var loanApplicationsIds = allApplications.Where(a => a.Product.ProductType == ProductType.Loan && a.LoanApplication.LoanAmountApplied != null).Select(a => a.Id);
                loanApplicationsIds = allApplications.Where(a => (a.BranchId != null && branchId.Contains(a.BranchId)) && (a.CostCenterId != null && a.CostCenterId == costCenterId)).Select(a => a.Id);
                if (stage == Stages.Files_Approved)
                {
                    approvedLoanApplicationLogsToday = approvedLoanApplicationLogsToday.Where(l => loanApplicationsIds.Contains(l.ApplicationId)).ToList();
                    if (approvedLoanApplicationLogsToday.Count > 0)
                        approvedApplicationListToDay.AddRange(approvedLoanApplicationLogsToday);
                }

                #endregion File Approved

                #region Disbursed/Received

                TodaydisbursementMemos = TodaydisbursementMemos.Where(d => (d.DisbursementMemo.Application.BranchId != null && branchId.Contains(d.DisbursementMemo.Application.BranchId)) && (d.DisbursementMemo.Application.CostCenterId != null && d.DisbursementMemo.Application.CostCenterId == costCenterId)).ToList();
                TodayfundReceivedToday = TodayfundReceivedToday.Where(d => (d.FundConfirmation.Application.BranchId != null && branchId.Contains(d.FundConfirmation.Application.BranchId)) && (d.FundConfirmation.Application.CostCenterId != null && d.FundConfirmation.Application.CostCenterId == costCenterId)).ToList();

                if (stage == Stages.Files_Disbursed)
                {
                    var acceptableDMidstoday = TodaydisbursementMemos.Select(dm => dm.DisbursementMemoId).ToList();
                    var disMemotoday = GenService.GetAll<DisbursementMemo>().Where(d => acceptableDMidstoday.Contains(d.Id)).ToList();
                    disbursementMemosToday = disMemotoday.ToList();

                    var acceptableFCidstoday = TodayfundReceivedToday.Select(dm => dm.FundConfirmationId).ToList();
                    var fundRectoday = GenService.GetAll<FundConfirmation>().Where(d => acceptableFCidstoday.Contains(d.Id)).ToList();
                    fundReceivedToday = fundRectoday.ToList();
                }

                #endregion Disbursed/Received

            }

            #endregion

            #region With Brach
            else if (branchId != null && branchId.Count > 0)
            {
                List<EmployeeDto> subordinateEmpList = new List<EmployeeDto>();
                foreach (var eachBranch in branchId)
                {
                    if (eachBranch > 0)
                    {
                        var temp = _office.GetEmployeesByOfficeId((long)eachBranch);
                        if (temp != null)
                            subordinateEmpList.AddRange(temp);
                    }
                }
                var subordinateUserList = new List<long>();
                foreach (var sub in subordinateEmpList)
                {
                    var user = _user.GetUserByEmployeeId((long)sub.Id);
                    if(user != null)
                        subordinateUserList.Add((long)user.Id);
                }

                //allApplications = allApplications.Where(a => subordinateEmpIdList.Contains(a.RMId));
                #region Call
                if (stage == Stages.Call)
                {
                    allCallsToday = allCallsToday.Where(c => subordinateUserList.Contains((long)c.CreatedBy));
                    if (allCallsToday.Any())
                        callListToday.AddRange(allCallsToday);
                }
                #endregion Call

                #region Lead

                else if (stage == Stages.Lead)
                {

                    allLeadsToday = allLeadsToday.Where(l => subordinateUserList.Contains((long)l.CreatedBy));
                    if (allLeadsToday.Any())
                        leadListToday.AddRange(allLeadsToday);
                }

                #endregion Lead

                #region File Submitted

                if (stage == Stages.Files_Submitted)
                {
                    //acceptableLogsToday = acceptableLogsToday.Where(l => allApplications.Any(a => a.Id == l.ApplicationId));
                    acceptableLogsToday = acceptableLogsToday.Where(l => l.Application.BranchId != null && branchId.Contains(l.Application.BranchId));
                    var allApplicationsToday = (from appLog in acceptableLogsToday
                                                orderby appLog.Id
                                                select appLog).Distinct().ToList();
                    if (allApplicationsToday.Any())
                        applicationListToDay.AddRange(allApplicationsToday);
                }

                #endregion File Submitted

                #region File Approved
                //var loanApplicationsIds = allApplications.Where(a => a.Product.ProductType == ProductType.Loan && a.LoanApplication.LoanAmountApplied != null).Select(a => a.Id);
                loanApplicationsIds = allApplications.Where(a => a.BranchId != null && branchId.Contains(a.BranchId)).Select(a => a.Id);
                if (stage == Stages.Files_Approved)
                {
                    approvedLoanApplicationLogsToday = approvedLoanApplicationLogsToday.Where(l => loanApplicationsIds.Contains(l.ApplicationId)).ToList();
                    if (approvedLoanApplicationLogsToday.Count > 0)
                        approvedApplicationListToDay.AddRange(approvedLoanApplicationLogsToday);
                }

                #endregion File Approved

                #region Disbursed/Received

                TodaydisbursementMemos = TodaydisbursementMemos.Where(d => d.DisbursementMemo.Application.BranchId != null && branchId.Contains(d.DisbursementMemo.Application.BranchId)).ToList();
                TodayfundReceivedToday = TodayfundReceivedToday.Where(d => d.FundConfirmation.Application.BranchId != null && branchId.Contains(d.FundConfirmation.Application.BranchId)).ToList();

                if (stage == Stages.Files_Disbursed)
                {
                    var acceptableDMidstoday = TodaydisbursementMemos.Select(dm => dm.DisbursementMemoId).ToList();
                    var disMemotoday = GenService.GetAll<DisbursementMemo>().Where(d => acceptableDMidstoday.Contains(d.Id)).ToList();
                    disbursementMemosToday = disMemotoday.ToList();

                    var acceptableFCidstoday = TodayfundReceivedToday.Select(dm => dm.FundConfirmationId).ToList();
                    var fundRectoday = GenService.GetAll<FundConfirmation>().Where(d => acceptableFCidstoday.Contains(d.Id)).ToList();
                    fundReceivedToday = fundRectoday.ToList();
                }

                #endregion Disbursed/Received
            }
            #endregion

            #region cost center
            else if (costCenterId != null && costCenterId > 0)
            {
                #region Call

                if (stage == Stages.Call)
                {
                    if (allCallsToday.Any())
                        callListToday.AddRange(allCallsToday);
                }
                #endregion Call

                #region Lead

                if (stage == Stages.Lead)
                {
                    if (allLeadsToday.Any())
                        leadListToday.AddRange(allLeadsToday);
                }
                #endregion Lead

                #region File Submitted

                if (stage == Stages.Files_Submitted)
                {
                    //acceptableLogsToday = acceptableLogsToday.Where(l => allApplications.Any(a => a.Id == l.ApplicationId));
                    acceptableLogsToday = acceptableLogsToday.Where(l => (l.Application.CostCenterId != null && l.Application.CostCenterId == costCenterId));
                    var allApplicationsToday = (from appLog in acceptableLogsToday
                                                orderby appLog.Id
                                                select appLog).Distinct().ToList();
                    if (allApplicationsToday.Any())
                        applicationListToDay.AddRange(allApplicationsToday);
                }

                #endregion File Submitted

                #region File Approved

                //var loanApplicationsIds = allApplications.Where(a => a.Product.ProductType == ProductType.Loan && a.LoanApplication.LoanAmountApplied != null).Select(a => a.Id);
                loanApplicationsIds = allApplications.Where(a => (a.CostCenterId != null && a.CostCenterId == costCenterId)).Select(a => a.Id);
                if (stage == Stages.Files_Approved)
                {
                    approvedLoanApplicationLogsToday = approvedLoanApplicationLogsToday.Where(l => loanApplicationsIds.Contains(l.ApplicationId)).ToList();
                    if (approvedLoanApplicationLogsToday.Count > 0)
                        approvedApplicationListToDay.AddRange(approvedLoanApplicationLogsToday);
                }

                #endregion File Approved

                #region Disbursed/Received

                TodaydisbursementMemos = TodaydisbursementMemos.Where(d => (d.DisbursementMemo.Application.CostCenterId != null && d.DisbursementMemo.Application.CostCenterId == costCenterId)).ToList();
                TodayfundReceivedToday = TodayfundReceivedToday.Where(d => (d.FundConfirmation.Application.CostCenterId != null && d.FundConfirmation.Application.CostCenterId == costCenterId)).ToList();

                if (stage == Stages.Files_Disbursed)
                {
                    var acceptableDMidstoday = TodaydisbursementMemos.Select(dm => dm.DisbursementMemoId).ToList();
                    var disMemotoday = GenService.GetAll<DisbursementMemo>().Where(d => acceptableDMidstoday.Contains(d.Id)).ToList();
                    disbursementMemosToday = disMemotoday.ToList();

                    var acceptableFCidstoday = TodayfundReceivedToday.Select(dm => dm.FundConfirmationId).ToList();
                    var fundRectoday = GenService.GetAll<FundConfirmation>().Where(d => acceptableFCidstoday.Contains(d.Id)).ToList();
                    fundReceivedToday = fundRectoday.ToList();
                }

                #endregion Disbursed/Received
            }
            #endregion

            else
            {
                #region Call

                if (stage == Stages.Call)
                {
                    if (allCallsToday.Any())
                        callListToday.AddRange(allCallsToday);

                    if (allLeadsToday.Any())
                        leadListToday.AddRange(allLeadsToday);
                }
                #endregion Call

                #region Lead

                if (stage == Stages.Lead)
                {
                    if (allLeadsToday.Any())
                        leadListToday.AddRange(allLeadsToday);

                    if (allCallsToday.Any())
                        callListToday.AddRange(allCallsToday);
                }
                #endregion Lead

                #region File Submitted

                var allApplicationsToday = (from appLog in acceptableLogsToday
                                            orderby appLog.Id
                                            select appLog).Distinct().ToList();
                if (allApplicationsToday.Any())
                    applicationListToDay.AddRange(allApplicationsToday);

                #endregion File Submitted

                #region File Approved

                loanApplicationsIds = GenService.GetAll<Application>().Where(a => a.Product.ProductType == ProductType.Loan).Select(a => a.Id);

                if (stage == Stages.Files_Approved)
                {
                    approvedLoanApplicationLogsToday = approvedLoanApplicationLogsToday.Where(l => loanApplicationsIds.Contains(l.ApplicationId)).ToList();
                    if (approvedLoanApplicationLogsToday.Count > 0)
                        approvedApplicationListToDay.AddRange(approvedLoanApplicationLogsToday);
                }

                #endregion File Approved

                #region Disbursed/Received

                if (stage == Stages.Files_Disbursed)
                {
                    var acceptableDMidstoday = TodaydisbursementMemos.Select(dm => dm.DisbursementMemoId).ToList();
                    var disMemotoday = GenService.GetAll<DisbursementMemo>().Where(d => acceptableDMidstoday.Contains(d.Id)).ToList();
                    disbursementMemosToday = disMemotoday.ToList();

                    var acceptableFCidstoday = TodayfundReceivedToday.Select(dm => dm.FundConfirmationId).ToList();
                    var fundRectoday = GenService.GetAll<FundConfirmation>().Where(d => acceptableFCidstoday.Contains(d.Id)).ToList();
                    fundReceivedToday = fundRectoday.ToList();
                }

                #endregion Disbursed/Received
            }

            #region initializations

            data.Disbursed_HomeLoanAmount = 0;
            data.Disbursed_PersonalLoanAmount = 0;
            data.Disbursed_AutoLoanAmount = 0;
            data.RMHomeLoan1 = "";
            data.RMHomeLoan1Amount = 0;
            data.RMHomeLoan1Count = 0;
            data.RMHomeLoan2 = "";
            data.RMHomeLoan2Amount = 0;
            data.RMHomeLoan2Count = 0;
            data.RMPersonalLoan = "";
            data.RMPersonalLoanAmount = 0;
            data.RMPersonalLoanCount = 0;
            data.RMAutoLoan = "";
            data.RMAutoLoanAmount = 0;
            data.RMAutoLoanCount = 0;
            data.RMLiability1 = "";
            data.RMLiability1Amount = 0;
            data.RMLiability1Count = 0;
            data.RMLiability2 = "";
            data.RMLiability2Amount = 0;
            data.RMLiability2Count = 0;

            var callListTodayHome = new List<Call>();
            var callListTodayAuto = new List<Call>();
            var callListTodayPersonal = new List<Call>();
            var callListTodayFixedDeposit = new List<Call>();
            var callListTodayRecurrentDeposit = new List<Call>();
            var callListTodayUndefined = new List<Call>();


            var leadListTodayHome = new List<SalesLead>();
            var leadListTodayAuto = new List<SalesLead>();
            var leadListTodayPersonal = new List<SalesLead>();
            var leadListTodayFixedDeposit = new List<SalesLead>();
            var leadListTodayRecurrentDeposit = new List<SalesLead>();

            var applicationListTodayHome = new List<ApplicationLog>();
            var applicationListTodayAuto = new List<ApplicationLog>();
            var applicationListTodayPersonal = new List<ApplicationLog>();
            var applicationListTodayFixedDeposit = new List<ApplicationLog>();
            var applicationListTodayRecurrentDeposit = new List<ApplicationLog>();

            var approvedApplicationListTodayHome = new List<ApplicationLog>();
            var approvedApplicationListTodayAuto = new List<ApplicationLog>();
            var approvedApplicationListTodayPersonal = new List<ApplicationLog>();

            var approvedHomeLoanLogsWithAmountToday = new List<ProductivityMatrixDataUnit>();
            var approvedAutoLoanLogsWithAmountToday = new List<ProductivityMatrixDataUnit>();
            var approvedPersonalLoanLogsWithAmountToday = new List<ProductivityMatrixDataUnit>();

            var disbursementMemosTodayHome = new List<DisbursementMemo>();
            var disbursementMemosTodayAuto = new List<DisbursementMemo>();
            var disbursementMemosTodayPersonal = new List<DisbursementMemo>();
            var fundReceivedTodayFixed = new List<FundConfirmation>();
            var fundReceivedTodayRecurrent = new List<FundConfirmation>();
            #endregion

            #region Call

            if (product.Contains(ProductSelection.Home_Loan))
                callListTodayHome = callListToday.Where(c => c.ProductId != null && c.Product.FacilityType == ProposalFacilityType.Home_Loan).ToList();
            if (product.Contains(ProductSelection.Auto_Loan))
                callListTodayAuto = callListToday.Where(c => c.ProductId != null && c.Product.FacilityType == ProposalFacilityType.Auto_Loan).ToList();
            if (product.Contains(ProductSelection.Personal_Loan))
                callListTodayPersonal = callListToday.Where(c => c.ProductId != null && c.Product.FacilityType == ProposalFacilityType.Personal_Loan).ToList();
            if (product.Contains(ProductSelection.Fixed))
                callListTodayFixedDeposit = callListToday.Where(c => c.ProductId != null && c.Product.DepositType == DepositType.Fixed).ToList();
            if (product.Contains(ProductSelection.Recurring))
                callListTodayRecurrentDeposit = callListToday.Where(c => c.ProductId != null && c.Product.DepositType == DepositType.Recurring).ToList();
            if (product.Count < 1)
                callListTodayUndefined = callListToday.ToList();

            #endregion Call

            #region Lead

            if (product.Contains(ProductSelection.Home_Loan))
                leadListTodayHome = leadListToday.Where(l => l.ProductId != null && l.Product.FacilityType == ProposalFacilityType.Home_Loan).ToList();
            if (product.Contains(ProductSelection.Auto_Loan))
                leadListTodayAuto = leadListToday.Where(l => l.ProductId != null && l.Product.FacilityType == ProposalFacilityType.Auto_Loan).ToList();
            if (product.Contains(ProductSelection.Personal_Loan))
                leadListTodayPersonal = leadListToday.Where(l => l.ProductId != null && l.Product.FacilityType == ProposalFacilityType.Personal_Loan).ToList();
            if (product.Contains(ProductSelection.Fixed))
                leadListTodayFixedDeposit = leadListToday.Where(l => l.ProductId != null && l.Product.DepositType == DepositType.Fixed).ToList();
            if (product.Contains(ProductSelection.Recurring))
                leadListTodayRecurrentDeposit = leadListToday.Where(l => l.ProductId != null && l.Product.DepositType == DepositType.Recurring).ToList();

            #endregion Lead

            #region File Submited Apps.

            if (product.Contains(ProductSelection.Home_Loan))
                applicationListTodayHome = applicationListToDay.Where(l => l.Application.LoanApplicationId != null && l.Application.Product.FacilityType == ProposalFacilityType.Home_Loan).ToList();
            if (product.Contains(ProductSelection.Auto_Loan))
                applicationListTodayAuto = applicationListToDay.Where(l => l.Application.LoanApplicationId != null && l.Application.Product.FacilityType == ProposalFacilityType.Auto_Loan).ToList();
            if (product.Contains(ProductSelection.Personal_Loan))
                applicationListTodayPersonal = applicationListToDay.Where(l => l.Application.LoanApplicationId != null && l.Application.Product.FacilityType == ProposalFacilityType.Personal_Loan).ToList();
            if (product.Contains(ProductSelection.Fixed))
                applicationListTodayFixedDeposit = applicationListToDay.Where(l => l.Application.DepositApplicationId != null && l.Application.Product.DepositType == DepositType.Fixed).ToList();
            if (product.Contains(ProductSelection.Recurring))
                applicationListTodayRecurrentDeposit = applicationListToDay.Where(l => l.Application.DepositApplicationId != null && l.Application.Product.DepositType == DepositType.Recurring).ToList();

            #endregion File Submited Apps.

            #region File Approved Apps.

            if (product.Contains(ProductSelection.Home_Loan))
            {

                approvedApplicationListTodayHome = approvedApplicationListToDay.Where(l => l.Application.LoanApplicationId != null && l.Application.ProductId != null && l.Application.Product.FacilityType != null && l.Application.Product.FacilityType == ProposalFacilityType.Home_Loan).ToList();

                if (approvedApplicationListTodayHome.Count > 0)
                {
                    //approvedHomeLoanLogsWithAmountToday = approvedHomeLoanLogsWithAmountToday.Where(p => approvedApplicationListTodayHome.Any(a => a.ApplicationId == p.ApplicationId)).ToList();
                    approvedHomeLoanLogsWithAmountToday = (from logs in approvedApplicationListTodayHome
                                                           join proposal in GenService.GetAll<Proposal>().Where(p => p.Status == EntityStatus.Active) on
                                                               logs.ApplicationId equals proposal.ApplicationId into proposals
                                                           from p in proposals.DefaultIfEmpty()
                                                           select new ProductivityMatrixDataUnit
                                                           {
                                                               Date = logs.CreateDate,
                                                               Value = p.RecomendedLoanAmountFromIPDC
                                                           }).ToList();
                }
            }

            if (product.Contains(ProductSelection.Auto_Loan))
            {
                approvedApplicationListTodayAuto = approvedApplicationListToDay.Where(l => l.Application.LoanApplicationId != null && l.Application.ProductId != null && l.Application.Product.FacilityType != null && l.Application.Product.FacilityType == ProposalFacilityType.Auto_Loan).ToList();

                if (approvedApplicationListTodayAuto.Count > 0)
                {
                    //approvedAutoLoanLogsWithAmountToday = GenService.GetAll<Proposal>().Where(p => approvedApplicationListTodayAuto.Any(a => a.ApplicationId == p.ApplicationId)).ToList();
                    approvedAutoLoanLogsWithAmountToday = (from logs in approvedApplicationListTodayAuto
                                                           join proposal in GenService.GetAll<Proposal>().Where(p => p.Status == EntityStatus.Active) on
                                                               logs.ApplicationId equals proposal.ApplicationId into proposals
                                                           from p in proposals.DefaultIfEmpty()
                                                           select new ProductivityMatrixDataUnit
                                                           {
                                                               Date = logs.CreateDate,
                                                               Value = p.RecomendedLoanAmountFromIPDC
                                                           }).ToList();
                }
            }

            if (product.Contains(ProductSelection.Personal_Loan))
            {

                approvedApplicationListTodayPersonal = approvedApplicationListToDay.Where(l => l.Application.LoanApplicationId != null && l.Application.ProductId != null && l.Application.Product.FacilityType != null && l.Application.Product.FacilityType == ProposalFacilityType.Personal_Loan).ToList();

                if (approvedApplicationListTodayPersonal.Count > 0)
                {
                    //approvedPersonalLoanLogsWithAmountToday = GenService.GetAll<Proposal>().Where(p => approvedApplicationListTodayPersonal.Any(a => a.ApplicationId == p.ApplicationId)).ToList();
                    approvedPersonalLoanLogsWithAmountToday = (from logs in approvedApplicationListTodayPersonal
                                                               join proposal in GenService.GetAll<Proposal>().Where(p => p.Status == EntityStatus.Active) on
                                                                   logs.ApplicationId equals proposal.ApplicationId into proposals
                                                               from p in proposals.DefaultIfEmpty()
                                                               select new ProductivityMatrixDataUnit
                                                               {
                                                                   Date = logs.CreateDate,
                                                                   Value = p.RecomendedLoanAmountFromIPDC
                                                               }).ToList();
                }
            }


            #endregion File Approved Apps.


            #region Disbursed/Received
            if (product.Contains(ProductSelection.Home_Loan))
            {
                disbursementMemosTodayHome = disbursementMemosToday.Where(d => d.Application.Product.FacilityType == ProposalFacilityType.Home_Loan).ToList();
            }

            if (product.Contains(ProductSelection.Auto_Loan))
            {
                disbursementMemosTodayAuto = disbursementMemosToday.Where(d => d.Application.Product.FacilityType == ProposalFacilityType.Auto_Loan).ToList();
            }

            if (product.Contains(ProductSelection.Personal_Loan))
            {
                disbursementMemosTodayPersonal = disbursementMemosToday.Where(d => d.Application.Product.FacilityType == ProposalFacilityType.Personal_Loan).ToList();
            }

            if (product.Contains(ProductSelection.Fixed))
            {
                fundReceivedTodayFixed = fundReceivedToday.Where(d => d.Application.Product.DepositType == DepositType.Fixed).ToList();
            }
            if (product.Contains(ProductSelection.Recurring))
            {
                fundReceivedTodayRecurrent = fundReceivedToday.Where(d => d.Application.Product.DepositType == DepositType.Recurring).ToList();
            }
            #endregion
            List<ProductivityMatrixDataUnit> ChartData = new List<ProductivityMatrixDataUnit>();

            #region Call
            if (stage == Stages.Call)
            {
                if (product.Contains(ProductSelection.Home_Loan))
                    callListToday = callListToday
                        .Where(c => c.ProductId != null &&
                                    ((product.Contains(ProductSelection.Home_Loan) && c.Product.FacilityType == ProposalFacilityType.Home_Loan) ||
                                     (product.Contains(ProductSelection.Auto_Loan) && c.Product.FacilityType == ProposalFacilityType.Auto_Loan) ||
                                     (product.Contains(ProductSelection.Personal_Loan) && c.Product.FacilityType == ProposalFacilityType.Personal_Loan) ||
                                     (product.Contains(ProductSelection.Fixed) && c.Product.DepositType == DepositType.Fixed) ||
                                     (product.Contains(ProductSelection.Recurring) && c.Product.DepositType == DepositType.Recurring)
                                    )).ToList();

                ChartData = callListToday.GroupBy(c => new { Month = c.CreateDate.Value.Month, Year = c.CreateDate.Value.Year })
                    .Select(c => new ProductivityMatrixDataUnit
                    {
                        Month = c.FirstOrDefault().CreateDate.Value.ToString("MMM"),
                        Year = c.FirstOrDefault().CreateDate.Value.ToString("yyyy"),
                        Date = c.FirstOrDefault().CreateDate.Value.AddDays(1 - c.FirstOrDefault().CreateDate.Value.Day),
                        Value = criteria == Criteria.Amount ? c.Sum(s => s.Amount != null ? s.Amount : 0) : c.Count(),
                        CompareValue = criteria == Criteria.Amount ? c.Where(s => s.CallStatus == CallStatus.Successful).Sum(s => s.Amount != null ? s.Amount : 0) : c.Where(s => s.CallStatus == CallStatus.Successful).Count()
                    }).ToList();
            }
            #endregion Call

            #region Lead
            else if (stage == Stages.Lead)
            {
                leadListToday = leadListToday.Where(c => c.ProductId != null &&
                                ((product.Contains(ProductSelection.Home_Loan) && c.Product.FacilityType == ProposalFacilityType.Home_Loan) ||
                                 (product.Contains(ProductSelection.Auto_Loan) && c.Product.FacilityType == ProposalFacilityType.Auto_Loan) ||
                                 (product.Contains(ProductSelection.Personal_Loan) && c.Product.FacilityType == ProposalFacilityType.Personal_Loan) ||
                                 (product.Contains(ProductSelection.Fixed) && c.Product.DepositType == DepositType.Fixed) ||
                                 (product.Contains(ProductSelection.Recurring) && c.Product.DepositType == DepositType.Recurring)
                                )).ToList();

                ChartData = leadListToday.GroupBy(c => new { Month = c.CreateDate.Value.Month, Year = c.CreateDate.Value.Year })
                    .Select(c => new ProductivityMatrixDataUnit
                    {
                        Month = c.FirstOrDefault().CreateDate.Value.ToString("MMM"),
                        Year = c.FirstOrDefault().CreateDate.Value.ToString("yyyy"),
                        Date = c.FirstOrDefault().CreateDate.Value.AddDays(1 - c.FirstOrDefault().CreateDate.Value.Day),
                        Value = criteria == Criteria.Amount ? c.Sum(s => s.Amount != null ? s.Amount : 0) : c.Count(),
                        CompareValue = criteria == Criteria.Amount ? c.Where(s => s.CallStatus == CallStatus.Successful).Sum(s => s.Amount != null ? s.Amount : 0) : c.Where(s => s.CallStatus == CallStatus.Successful).Count()
                    }).ToList();
            }

            #endregion Lead

            #region File Submited Apps.
            else if (stage == Stages.Files_Submitted)
            {
                applicationListToDay = applicationListToDay
                    .Where(l => (l.Application.LoanApplicationId != null &&
                                 (product.Contains(ProductSelection.Home_Loan) && l.Application.Product.FacilityType == ProposalFacilityType.Home_Loan) ||
                                 (product.Contains(ProductSelection.Auto_Loan) && l.Application.Product.FacilityType == ProposalFacilityType.Auto_Loan) ||
                                 (product.Contains(ProductSelection.Personal_Loan) && l.Application.Product.FacilityType == ProposalFacilityType.Personal_Loan)
                                ) ||
                                (l.Application.DepositApplicationId != null &&
                                ((product.Contains(ProductSelection.Fixed) && l.Application.Product.DepositType == DepositType.Fixed) ||
                                 (product.Contains(ProductSelection.Recurring) && l.Application.Product.DepositType == DepositType.Recurring))
                                )
                    ).ToList();

                ChartData = applicationListToDay.GroupBy(c => new { Month = c.CreateDate.Value.Month, Year = c.CreateDate.Value.Year })
                    .Select(c => new ProductivityMatrixDataUnit
                    {
                        Month = c.FirstOrDefault().CreateDate.Value.ToString("MMM"),
                        Year = c.FirstOrDefault().CreateDate.Value.ToString("yyyy"),
                        Date = c.FirstOrDefault().CreateDate.Value.AddDays(1 - c.FirstOrDefault().CreateDate.Value.Day),
                        Value = criteria == Criteria.Amount ? c.Sum(s => s.Application.LoanApplication.LoanAmountApplied) : c.Count(),
                        CompareValue = 0// c.Where(s => s.CallStatus == CallStatus.Successful).Sum(s => s.Amount != null ? s.Amount : 0)
                    }).ToList();
            }
            #endregion File Submited Apps.

            #region File Approved Apps.
            else if (stage == Stages.Files_Approved)
            {
                if (product.Contains(ProductSelection.Home_Loan))
                {

                    var test = approvedHomeLoanLogsWithAmountToday.GroupBy(c => new { Month = c.Date.Value.Month, Year = c.Date.Value.Year })
                    .Select(c => new ProductivityMatrixDataUnit
                    {
                        Month = c.FirstOrDefault().Date.Value.ToString("MMM"),
                        Year = c.FirstOrDefault().Date.Value.ToString("yyyy"),
                        Date = c.FirstOrDefault().Date.Value.AddDays(1 - c.FirstOrDefault().Date.Value.Day),
                        Value = criteria == Criteria.Amount ? c.Sum(s => s.Value != null ? s.Value : 0) : c.Count(),
                        CompareValue = 0//c.Where(s => s.CallStatus == CallStatus.Successful).Sum(s => s.Amount != null ? s.Amount : 0)
                    }).ToList();
                    if (test != null && test.Count > 0)
                        ChartData.AddRange(test);
                }
                if (product.Contains(ProductSelection.Auto_Loan))
                {
                    var test = approvedAutoLoanLogsWithAmountToday.GroupBy(c => new { Month = c.Date.Value.Month, Year = c.Date.Value.Year })
                    .Select(c => new ProductivityMatrixDataUnit
                    {
                        Month = c.FirstOrDefault().Date.Value.ToString("MMM"),
                        Year = c.FirstOrDefault().Date.Value.ToString("yyyy"),
                        Date = c.FirstOrDefault().Date.Value.AddDays(1 - c.FirstOrDefault().Date.Value.Day),
                        Value = criteria == Criteria.Amount ? c.Sum(s => s.Value != null ? s.Value : 0) : c.Count(),
                        CompareValue = 0//c.Where(s => s.CallStatus == CallStatus.Successful).Sum(s => s.Amount != null ? s.Amount : 0)
                    }).ToList();
                    if (test != null && test.Count > 0)
                        ChartData.AddRange(test);
                }
                if (product.Contains(ProductSelection.Personal_Loan))
                {
                    var test = approvedPersonalLoanLogsWithAmountToday.GroupBy(c => new { Month = c.Date.Value.Month, Year = c.Date.Value.Year })
                    .Select(c => new ProductivityMatrixDataUnit
                    {
                        Month = c.FirstOrDefault().Date.Value.ToString("MMM"),
                        Year = c.FirstOrDefault().Date.Value.ToString("yyyy"),
                        Date = c.FirstOrDefault().Date.Value.AddDays(1 - c.FirstOrDefault().Date.Value.Day),
                        Value = criteria == Criteria.Amount ? c.Sum(s => s.Value != null ? s.Value : 0) : c.Count(),
                        CompareValue = 0//c.Where(s => s.CallStatus == CallStatus.Successful).Sum(s => s.Amount != null ? s.Amount : 0)
                    }).ToList();
                    if (test != null && test.Count > 0)
                        ChartData.AddRange(test);
                }

            }
            #endregion File Approved Apps.

            #region Disbursed/Received
            else if (stage == Stages.Files_Disbursed)
            {
                if (product.Contains(ProductSelection.Home_Loan))
                {
                    var test = disbursedMemodetails.Where(d => disbursementMemosToday.Any(dis => dis.Id == d.DisbursementMemoId && dis.Application.Product.FacilityType == ProposalFacilityType.Home_Loan))
                        .GroupBy(c => new { Month = c.CreateDate.Value.Month, Year = c.CreateDate.Value.Year })
                    .Select(c => new ProductivityMatrixDataUnit
                    {
                        Month = c.FirstOrDefault().CreateDate.Value.ToString("MMM"),
                        Year = c.FirstOrDefault().CreateDate.Value.ToString("yyyy"),
                        Date = c.FirstOrDefault().CreateDate.Value.AddDays(1 - c.FirstOrDefault().CreateDate.Value.Day),
                        Value = criteria == Criteria.Amount ? c.Sum(s => s.DisbursementAmount) : c.Count(),
                        CompareValue = 0//c.Where(s => s.CallStatus == CallStatus.Successful).Sum(s => s.Amount != null ? s.Amount : 0)
                    }).ToList();
                    if (test != null && test.Count > 0)
                        ChartData.AddRange(test);
                }
                if (product.Contains(ProductSelection.Auto_Loan))
                {
                    var test = disbursedMemodetails.Where(d => disbursementMemosToday.Any(dis => dis.Id == d.DisbursementMemoId && dis.Application.Product.FacilityType == ProposalFacilityType.Home_Loan))
                        .GroupBy(c => new { Month = c.CreateDate.Value.Month, Year = c.CreateDate.Value.Year })
                    .Select(c => new ProductivityMatrixDataUnit
                    {
                        Month = c.FirstOrDefault().CreateDate.Value.ToString("MMM"),
                        Year = c.FirstOrDefault().CreateDate.Value.ToString("yyyy"),
                        Date = c.FirstOrDefault().CreateDate.Value.AddDays(1 - c.FirstOrDefault().CreateDate.Value.Day),
                        Value = criteria == Criteria.Amount ? c.Sum(s => s.DisbursementAmount) : c.Count(),
                        CompareValue = 0//c.Where(s => s.CallStatus == CallStatus.Successful).Sum(s => s.Amount != null ? s.Amount : 0)
                    }).ToList();
                    if (test != null && test.Count > 0)
                        ChartData.AddRange(test);
                }
                if (product.Contains(ProductSelection.Personal_Loan))
                {
                    var test = disbursedMemodetails.Where(d => disbursementMemosToday.Any(dis => dis.Id == d.DisbursementMemoId && dis.Application.Product.FacilityType == ProposalFacilityType.Home_Loan))
                        .GroupBy(c => new { Month = c.CreateDate.Value.Month, Year = c.CreateDate.Value.Year })
                    .Select(c => new ProductivityMatrixDataUnit
                    {
                        Month = c.FirstOrDefault().CreateDate.Value.ToString("MMM"),
                        Year = c.FirstOrDefault().CreateDate.Value.ToString("yyyy"),
                        Date = c.FirstOrDefault().CreateDate.Value.AddDays(1 - c.FirstOrDefault().CreateDate.Value.Day),
                        Value = criteria == Criteria.Amount ? c.Sum(s => s.DisbursementAmount) : c.Count(),
                        CompareValue = 0// c.Where(s => s.CallStatus == CallStatus.Successful).Sum(s => s.Amount != null ? s.Amount : 0)
                    }).ToList();
                    if (test != null && test.Count > 0)
                        ChartData.AddRange(test);
                }
            }
            #endregion Disbursed/Received

            ChartData = ChartData.GroupBy(c => new { Month = c.Month, Year = c.Year, Date = c.Date })
                    .Select(c => new ProductivityMatrixDataUnit
                    {
                        Month = c.Key.Month,
                        Year = c.Key.Year,
                        Date = c.Key.Date,
                        Value = c.Sum(s => s.Value),
                        CompareValue = c.Sum(s => s.CompareValue)
                    }).ToList();


            data.ChartData = ChartData;
            return data;
        }

        public object GetNSMDisiburesedReceivedMTD(TimeLine? timeLine, long? branchId)
        {
            //long UserId,
            var data = new DashBoardHighlightsDto();
            DateRangeDto dateRange;
            if (timeLine == null)
                timeLine = TimeLine.Today;
            dateRange = new DateRangeDto((TimeLine)timeLine);


            var disbursementMemosToday = new List<DisbursementMemo>();

            var fundReceivedToday = new List<FundConfirmation>();

            var disbursedMemodetails = new List<DMDetail>();
            disbursedMemodetails = GenService.GetAll<DMDetail>().Where(dm => dm.Status == EntityStatus.Active).ToList();
            var fundConfirmdetails = new List<FundConfirmationDetail>();
            fundConfirmdetails = GenService.GetAll<FundConfirmationDetail>().Where(fc => fc.Status == EntityStatus.Active).ToList();

            long userId = 0;

            if (branchId != null || branchId > 0)
            {
                //var employeeList = _office.GetEmployeesByOfficeId((long)branchId);

                List<EmployeeDto> subordinateEmpList = new List<EmployeeDto>();
                if (branchId > 0)
                    subordinateEmpList = _office.GetEmployeesByOfficeId((long)branchId);
                var subordinateUserList = new List<long>();
                foreach (var sub in subordinateEmpList)
                {
                    subordinateUserList.Add((long)_user.GetUserByEmployeeId((long)sub.Id).Id);
                }

                #region Amount disbursed / received Today

                var TodaydisbursementMemos = GenService.GetAll<DMDetail>().Where(d => d.DisbursementMemo.ApplicationId != null
                            && d.DisbursementMemo.Application.BranchId != null
                            && d.DisbursementMemo.Application.BranchId == branchId
                            && d.DisbursementMemo.Application.LoanApplicationId != null
                            && d.DisbursementMemo.Application.Product.FacilityType != null
                            && (d.DisbursementMemo.Application.Product.FacilityType == ProposalFacilityType.Home_Loan
                                || d.DisbursementMemo.Application.Product.FacilityType == ProposalFacilityType.Auto_Loan
                                || d.DisbursementMemo.Application.Product.FacilityType == ProposalFacilityType.Personal_Loan)
                            && d.CreateDate <= dateRange.ToDate
                            && d.CreateDate >= dateRange.FromDate
                            && d.DisbursementMemo.IsApproved != null
                            && d.DisbursementMemo.IsApproved == true
                            && d.DisbursementMemo.IsDisbursed != null
                            && d.DisbursementMemo.IsDisbursed == true
                            && d.Status == EntityStatus.Active).ToList();
                var acceptableDMidstoday = TodaydisbursementMemos.Select(dm => dm.DisbursementMemoId).ToList();
                var disMemotoday = GenService.GetAll<DisbursementMemo>().Where(d => acceptableDMidstoday.Contains(d.Id)).ToList();
                disbursementMemosToday = disMemotoday.ToList();

                var TodayfundReceivedToday = GenService.GetAll<FundConfirmationDetail>().Where(d => d.FundConfirmation.ApplicationId != null
                            && d.FundConfirmation.Application.BranchId != null
                            && d.FundConfirmation.Application.BranchId == branchId
                            && d.FundConfirmation.Application.DepositApplicationId != null
                            && d.FundConfirmation.Application.Product.DepositType != null
                            && (d.FundConfirmation.Application.Product.DepositType == DepositType.Fixed
                                || d.FundConfirmation.Application.Product.DepositType == DepositType.Recurring)
                            && d.CreateDate <= dateRange.ToDate
                            && d.CreateDate >= dateRange.FromDate
                            && d.FundConfirmation.FundReceived != null
                            && d.FundConfirmation.FundReceived == true
                            && d.Status == EntityStatus.Active).ToList();
                var acceptableFCidstoday = TodayfundReceivedToday.Select(dm => dm.FundConfirmationId).ToList();
                var fundRectoday = GenService.GetAll<FundConfirmation>().Where(d => acceptableFCidstoday.Contains(d.Id)).ToList();
                fundReceivedToday = fundRectoday.ToList();

                #endregion Amount disbursed / received Today

            }
            else
            {

                #region File disbursed / received Today
                var TodaydisbursementMemos = GenService.GetAll<DMDetail>().Where(d => d.DisbursementMemo.ApplicationId != null

                            && d.DisbursementMemo.Application.LoanApplicationId != null
                            && d.DisbursementMemo.Application.Product.FacilityType != null
                            && (d.DisbursementMemo.Application.Product.FacilityType == ProposalFacilityType.Home_Loan
                                || d.DisbursementMemo.Application.Product.FacilityType == ProposalFacilityType.Auto_Loan
                                || d.DisbursementMemo.Application.Product.FacilityType == ProposalFacilityType.Personal_Loan)
                            && d.CreateDate <= dateRange.ToDate
                            && d.CreateDate >= dateRange.FromDate
                            && d.DisbursementMemo.IsApproved != null
                            && d.DisbursementMemo.IsApproved == true
                            && d.DisbursementMemo.IsDisbursed != null
                            && d.DisbursementMemo.IsDisbursed == true
                            && d.Status == EntityStatus.Active).ToList();
                var acceptableDMids = TodaydisbursementMemos.Select(dm => dm.DisbursementMemoId).ToList();
                var disMemotoday = GenService.GetAll<DisbursementMemo>().Where(d => acceptableDMids.Contains(d.Id)).ToList();
                disbursementMemosToday = disMemotoday.ToList();

                var TodayfundReceived = GenService.GetAll<FundConfirmationDetail>().Where(d => d.FundConfirmation.ApplicationId != null

                            && d.FundConfirmation.Application.DepositApplicationId != null
                            && d.FundConfirmation.Application.Product.DepositType != null
                            && (d.FundConfirmation.Application.Product.DepositType == DepositType.Fixed
                                || d.FundConfirmation.Application.Product.DepositType == DepositType.Recurring)
                            && d.CreateDate <= dateRange.ToDate
                            && d.CreateDate >= dateRange.FromDate
                            && d.FundConfirmation.FundReceived != null
                            && d.FundConfirmation.FundReceived == true
                            && d.Status == EntityStatus.Active).ToList();
                var acceptableFCids = TodayfundReceived.Select(dm => dm.FundConfirmationId).ToList();
                var fundRectoday = GenService.GetAll<FundConfirmation>().Where(d => acceptableFCids.Contains(d.Id)).ToList();
                fundReceivedToday = fundRectoday.ToList();

                #endregion File disbursed / received Today


            }

            #region initializations

            data.Disbursed_HomeLoanAmount = 0;
            data.Disbursed_PersonalLoanAmount = 0;
            data.Disbursed_AutoLoanAmount = 0;
            data.RMHomeLoan1 = "";
            data.RMHomeLoan1Amount = 0;
            data.RMHomeLoan1Count = 0;
            data.RMHomeLoan2 = "";
            data.RMHomeLoan2Amount = 0;
            data.RMHomeLoan2Count = 0;
            data.RMPersonalLoan = "";
            data.RMPersonalLoanAmount = 0;
            data.RMPersonalLoanCount = 0;
            data.RMAutoLoan = "";
            data.RMAutoLoanAmount = 0;
            data.RMAutoLoanCount = 0;
            data.RMLiability1 = "";
            data.RMLiability1Amount = 0;
            data.RMLiability1Count = 0;
            data.RMLiability2 = "";
            data.RMLiability2Amount = 0;
            data.RMLiability2Count = 0;
            #endregion

            #region Disbursed/Received No. Of Files

            data.Disbursed_HomeLoanMTD = disbursementMemosToday.Where(d => d.Application.Product.FacilityType == ProposalFacilityType.Home_Loan).Count();


            data.Disbursed_AutoLoanMTD = disbursementMemosToday.Where(d => d.Application.Product.FacilityType == ProposalFacilityType.Auto_Loan).Count();


            data.Disbursed_PersonalLoanMTD = disbursementMemosToday.Where(d => d.Application.Product.FacilityType == ProposalFacilityType.Personal_Loan).Count();


            data.Received_FixedMTD = fundReceivedToday.Where(d => d.Application.Product.DepositType == DepositType.Fixed).Count();


            data.Received_RecurrentMTD = fundReceivedToday.Where(d => d.Application.Product.DepositType == DepositType.Recurring).Count();


            #endregion Disbursed/Received No. Of Files

            #region Disbursed/Received Amount/War

            if (disbursementMemosToday.Count(d => d.Application.Product.FacilityType == ProposalFacilityType.Home_Loan) > 0)
            {
                decimal totalwarHome = 0;
                decimal sumofAmoutHome = 0;
                decimal sumofRateAmoutProductHome = 0;

                data.Disbursed_HomeLoanAmountMTD = disbursedMemodetails.Where(d => disbursementMemosToday.Any(dis => dis.Id == d.DisbursementMemoId && dis.Application.Product.FacilityType == ProposalFacilityType.Home_Loan)).Sum(d => d.DisbursementAmount);

                foreach (var w in disbursementMemosToday.Where(d => d.Application.Product.FacilityType == ProposalFacilityType.Home_Loan).ToList())
                {
                    if (w.Proposal.RecomendedLoanAmountFromIPDC != null)
                    {
                        sumofAmoutHome += (decimal)w.Proposal.RecomendedLoanAmountFromIPDC;
                        sumofRateAmoutProductHome += (decimal)(w.Proposal.RecomendedLoanAmountFromIPDC * w.Proposal.InterestRateOffered);
                    }
                    totalwarHome = sumofRateAmoutProductHome / sumofAmoutHome;
                }
                data.HomeLoanWarMTD = totalwarHome;
            }

            if (disbursementMemosToday.Count(d => d.Application.Product.FacilityType == ProposalFacilityType.Auto_Loan) > 0)
            {
                data.Disbursed_AutoLoanAmountMTD = disbursedMemodetails.Where(d => disbursementMemosToday.Any(dis => dis.Id == d.DisbursementMemoId && dis.Application.Product.FacilityType == ProposalFacilityType.Auto_Loan)).Sum(d => d.DisbursementAmount);

                decimal totalwarAuto = 0;
                decimal sumofAmoutAuto = 0;
                decimal sumofRateAmoutProductAuto = 0;

                foreach (var w in disbursementMemosToday.Where(d => d.Application.Product.FacilityType == ProposalFacilityType.Auto_Loan).ToList())
                {
                    if (w.Proposal.RecomendedLoanAmountFromIPDC != null)
                    {
                        sumofAmoutAuto += (decimal)w.Proposal.RecomendedLoanAmountFromIPDC;
                        sumofRateAmoutProductAuto += (decimal)(w.Proposal.RecomendedLoanAmountFromIPDC * w.Proposal.InterestRateOffered);
                    }
                    totalwarAuto = sumofRateAmoutProductAuto / sumofAmoutAuto;
                }
                data.AutoLoanWarMTD = totalwarAuto;
            }

            if (disbursementMemosToday.Count(d => d.Application.Product.FacilityType == ProposalFacilityType.Personal_Loan) > 0)
            {
                data.Disbursed_PersonalLoanAmountMTD = disbursedMemodetails.Where(d => disbursementMemosToday.Any(dis => dis.Id == d.DisbursementMemoId && dis.Application.Product.FacilityType == ProposalFacilityType.Personal_Loan)).Sum(d => d.DisbursementAmount);

                decimal totalwarPersonal = 0;
                decimal sumofAmoutPersonal = 0;
                decimal sumofRateAmoutProductPersonal = 0;

                foreach (var w in disbursementMemosToday.Where(d => d.Application.Product.FacilityType == ProposalFacilityType.Personal_Loan).ToList())
                {
                    if (w.Proposal.RecomendedLoanAmountFromIPDC != null)
                    {
                        sumofAmoutPersonal += (decimal)w.Proposal.RecomendedLoanAmountFromIPDC;
                        sumofRateAmoutProductPersonal += (decimal)(w.Proposal.RecomendedLoanAmountFromIPDC * w.Proposal.InterestRateOffered);
                    }
                    totalwarPersonal = sumofRateAmoutProductPersonal / sumofAmoutPersonal;
                }
                data.PersonalLoanWarMTD = totalwarPersonal;
            }

            if (fundReceivedToday.Count(d => d.Application.Product.DepositType == DepositType.Fixed) > 0)
            {
                decimal totalwarFixed = 0;
                decimal sumofAmoutFixed = 0;
                decimal sumofRateAmoutProductFixed = 0;

                var sumFixed = fundConfirmdetails.Where(d => fundReceivedToday.Any(dis => dis.Id == d.FundConfirmationId && dis.Application.Product.DepositType == DepositType.Fixed)).Sum(d => d.Amount);
                if (sumFixed != null)
                    data.FundReceived_FixedAmountMTD = (decimal)sumFixed;

                foreach (var w in fundReceivedToday.Where(d => d.Application.Product.DepositType == DepositType.Fixed).ToList())
                {
                    if (w.Application.DepositApplicationId != null)
                    {
                        sumofAmoutFixed += w.Application.DepositApplication.TotalDepositAmount;
                        sumofRateAmoutProductFixed += (decimal)(w.Application.DepositApplication.TotalDepositAmount * w.Application.DepositApplication.OfferRate);
                    }

                    totalwarFixed = sumofRateAmoutProductFixed / sumofAmoutFixed;
                }
                data.FixedDepositWarMTD = totalwarFixed;
            }

            if (fundReceivedToday.Count(d => d.Application.Product.DepositType == DepositType.Recurring) > 0)
            {
                decimal totalwarRecurrent = 0;
                decimal sumofAmoutRecurrent = 0;
                decimal sumofRateAmoutProductRecurrent = 0;

                var sumRecurrent = fundConfirmdetails.Where(d => fundReceivedToday.Any(dis => dis.Id == d.FundConfirmationId && dis.Application.Product.DepositType == DepositType.Recurring)).Sum(d => d.Amount);
                if (sumRecurrent != null)
                    data.FundReceived_RecurrentAmountMTD = (decimal)sumRecurrent;

                foreach (var w in fundReceivedToday.Where(d => d.Application.Product.DepositType == DepositType.Recurring).ToList())
                {
                    if (w.Application.DepositApplicationId != null && w.Application.DepositApplication.OfferRate != null)
                    {
                        sumofAmoutRecurrent += w.Application.DepositApplication.TotalDepositAmount;
                        sumofRateAmoutProductRecurrent += (decimal)(w.Application.DepositApplication.TotalDepositAmount * w.Application.DepositApplication.OfferRate);
                    }

                    totalwarRecurrent = sumofRateAmoutProductRecurrent / sumofAmoutRecurrent;
                }
                data.ReccurentDepositWarMTD = totalwarRecurrent;
            }

            #endregion Disbursed/Received Amount/War

            return data;
        }

        public object GetNSMDisiburesedReceivedMTD(TimeLine? timeLine, List<long?> branchId)
        {
            //long UserId,
            var data = new DashBoardHighlightsDto();
            DateRangeDto dateRange;
            if (timeLine == null)
                timeLine = TimeLine.Today;
            dateRange = new DateRangeDto((TimeLine)timeLine);


            var disbursementMemosToday = new List<DisbursementMemo>();

            var fundReceivedToday = new List<FundConfirmation>();

            var disbursedMemodetails = new List<DMDetail>();
            disbursedMemodetails = GenService.GetAll<DMDetail>().Where(dm => dm.Status == EntityStatus.Active).ToList();
            var fundConfirmdetails = new List<FundConfirmationDetail>();
            fundConfirmdetails = GenService.GetAll<FundConfirmationDetail>().Where(fc => fc.Status == EntityStatus.Active).ToList();


            if (branchId != null && branchId.Count > 0)
            {
                List<EmployeeDto> subordinateEmpList = new List<EmployeeDto>();
                foreach (var eachBranch in branchId)
                {
                    if (eachBranch > 0)
                    {
                        var temp = _office.GetEmployeesByOfficeId((long)eachBranch);
                        if (temp != null)
                            subordinateEmpList.AddRange(temp);
                    }
                }
                var subordinateUserList = new List<long>();
                foreach (var sub in subordinateEmpList)
                {
                    var user = _user.GetUserByEmployeeId((long)sub.Id);
                    if(user != null)
                        subordinateUserList.Add((long)user.Id);
                }

                #region Amount disbursed / received Today

                var TodaydisbursementMemos = GenService.GetAll<DMDetail>().Where(d => d.DisbursementMemo.ApplicationId != null
                            && d.DisbursementMemo.Application.BranchId != null
                            && branchId.Contains(d.DisbursementMemo.Application.BranchId)
                            && d.DisbursementMemo.Application.LoanApplicationId != null
                            && d.DisbursementMemo.Application.Product.FacilityType != null
                            && (d.DisbursementMemo.Application.Product.FacilityType == ProposalFacilityType.Home_Loan
                                || d.DisbursementMemo.Application.Product.FacilityType == ProposalFacilityType.Auto_Loan
                                || d.DisbursementMemo.Application.Product.FacilityType == ProposalFacilityType.Personal_Loan)
                            && d.CreateDate <= dateRange.ToDate
                            && d.CreateDate >= dateRange.FromDate
                            && d.DisbursementMemo.IsApproved != null
                            && d.DisbursementMemo.IsApproved == true
                            && d.DisbursementMemo.IsDisbursed != null
                            && d.DisbursementMemo.IsDisbursed == true
                            && d.Status == EntityStatus.Active).ToList();
                var acceptableDMidstoday = TodaydisbursementMemos.Select(dm => dm.DisbursementMemoId).ToList();
                var disMemotoday = GenService.GetAll<DisbursementMemo>().Where(d => acceptableDMidstoday.Contains(d.Id)).ToList();
                disbursementMemosToday = disMemotoday.ToList();

                var TodayfundReceivedToday = GenService.GetAll<FundConfirmationDetail>().Where(d => d.FundConfirmation.ApplicationId != null
                            && d.FundConfirmation.Application.BranchId != null
                            && branchId.Contains(d.FundConfirmation.Application.BranchId)
                            && d.FundConfirmation.Application.DepositApplicationId != null
                            && d.FundConfirmation.Application.Product.DepositType != null
                            && (d.FundConfirmation.Application.Product.DepositType == DepositType.Fixed
                                || d.FundConfirmation.Application.Product.DepositType == DepositType.Recurring)
                            && d.CreateDate <= dateRange.ToDate
                            && d.CreateDate >= dateRange.FromDate
                            && d.FundConfirmation.FundReceived != null
                            && d.FundConfirmation.FundReceived == true
                            && d.Status == EntityStatus.Active).ToList();
                var acceptableFCidstoday = TodayfundReceivedToday.Select(dm => dm.FundConfirmationId).ToList();
                var fundRectoday = GenService.GetAll<FundConfirmation>().Where(d => acceptableFCidstoday.Contains(d.Id)).ToList();
                fundReceivedToday = fundRectoday.ToList();

                #endregion Amount disbursed / received Today

            }
            else
            {

                #region File disbursed / received Today
                var TodaydisbursementMemos = GenService.GetAll<DMDetail>().Where(d => d.DisbursementMemo.ApplicationId != null

                            && d.DisbursementMemo.Application.LoanApplicationId != null
                            && d.DisbursementMemo.Application.Product.FacilityType != null
                            && (d.DisbursementMemo.Application.Product.FacilityType == ProposalFacilityType.Home_Loan
                                || d.DisbursementMemo.Application.Product.FacilityType == ProposalFacilityType.Auto_Loan
                                || d.DisbursementMemo.Application.Product.FacilityType == ProposalFacilityType.Personal_Loan)
                            && d.CreateDate <= dateRange.ToDate
                            && d.CreateDate >= dateRange.FromDate
                            && d.DisbursementMemo.IsApproved != null
                            && d.DisbursementMemo.IsApproved == true
                            && d.DisbursementMemo.IsDisbursed != null
                            && d.DisbursementMemo.IsDisbursed == true
                            && d.Status == EntityStatus.Active).ToList();
                var acceptableDMids = TodaydisbursementMemos.Select(dm => dm.DisbursementMemoId).ToList();
                var disMemotoday = GenService.GetAll<DisbursementMemo>().Where(d => acceptableDMids.Contains(d.Id)).ToList();
                disbursementMemosToday = disMemotoday.ToList();

                var TodayfundReceived = GenService.GetAll<FundConfirmationDetail>().Where(d => d.FundConfirmation.ApplicationId != null

                            && d.FundConfirmation.Application.DepositApplicationId != null
                            && d.FundConfirmation.Application.Product.DepositType != null
                            && (d.FundConfirmation.Application.Product.DepositType == DepositType.Fixed
                                || d.FundConfirmation.Application.Product.DepositType == DepositType.Recurring)
                            && d.CreateDate <= dateRange.ToDate
                            && d.CreateDate >= dateRange.FromDate
                            && d.FundConfirmation.FundReceived != null
                            && d.FundConfirmation.FundReceived == true
                            && d.Status == EntityStatus.Active).ToList();
                var acceptableFCids = TodayfundReceived.Select(dm => dm.FundConfirmationId).ToList();
                var fundRectoday = GenService.GetAll<FundConfirmation>().Where(d => acceptableFCids.Contains(d.Id)).ToList();
                fundReceivedToday = fundRectoday.ToList();

                #endregion File disbursed / received Today


            }

            #region initializations

            data.Disbursed_HomeLoanAmount = 0;
            data.Disbursed_PersonalLoanAmount = 0;
            data.Disbursed_AutoLoanAmount = 0;
            data.RMHomeLoan1 = "";
            data.RMHomeLoan1Amount = 0;
            data.RMHomeLoan1Count = 0;
            data.RMHomeLoan2 = "";
            data.RMHomeLoan2Amount = 0;
            data.RMHomeLoan2Count = 0;
            data.RMPersonalLoan = "";
            data.RMPersonalLoanAmount = 0;
            data.RMPersonalLoanCount = 0;
            data.RMAutoLoan = "";
            data.RMAutoLoanAmount = 0;
            data.RMAutoLoanCount = 0;
            data.RMLiability1 = "";
            data.RMLiability1Amount = 0;
            data.RMLiability1Count = 0;
            data.RMLiability2 = "";
            data.RMLiability2Amount = 0;
            data.RMLiability2Count = 0;
            #endregion

            #region Disbursed/Received No. Of Files

            data.Disbursed_HomeLoanMTD = disbursementMemosToday.Where(d => d.Application.Product.FacilityType == ProposalFacilityType.Home_Loan).Count();


            data.Disbursed_AutoLoanMTD = disbursementMemosToday.Where(d => d.Application.Product.FacilityType == ProposalFacilityType.Auto_Loan).Count();


            data.Disbursed_PersonalLoanMTD = disbursementMemosToday.Where(d => d.Application.Product.FacilityType == ProposalFacilityType.Personal_Loan).Count();


            data.Received_FixedMTD = fundReceivedToday.Where(d => d.Application.Product.DepositType == DepositType.Fixed).Count();


            data.Received_RecurrentMTD = fundReceivedToday.Where(d => d.Application.Product.DepositType == DepositType.Recurring).Count();


            #endregion Disbursed/Received No. Of Files

            #region Disbursed/Received Amount/War

            if (disbursementMemosToday.Count(d => d.Application.Product.FacilityType == ProposalFacilityType.Home_Loan) > 0)
            {
                decimal totalwarHome = 0;
                decimal sumofAmoutHome = 0;
                decimal sumofRateAmoutProductHome = 0;

                data.Disbursed_HomeLoanAmountMTD = disbursedMemodetails.Where(d => disbursementMemosToday.Any(dis => dis.Id == d.DisbursementMemoId && dis.Application.Product.FacilityType == ProposalFacilityType.Home_Loan)).Sum(d => d.DisbursementAmount);

                foreach (var w in disbursementMemosToday.Where(d => d.Application.Product.FacilityType == ProposalFacilityType.Home_Loan).ToList())
                {
                    if (w.Proposal.RecomendedLoanAmountFromIPDC != null)
                    {
                        sumofAmoutHome += (decimal)w.Proposal.RecomendedLoanAmountFromIPDC;
                        sumofRateAmoutProductHome += (decimal)(w.Proposal.RecomendedLoanAmountFromIPDC * w.Proposal.InterestRateOffered);
                    }
                    totalwarHome = sumofRateAmoutProductHome / sumofAmoutHome;
                }
                data.HomeLoanWarMTD = totalwarHome;
            }

            if (disbursementMemosToday.Count(d => d.Application.Product.FacilityType == ProposalFacilityType.Auto_Loan) > 0)
            {
                data.Disbursed_AutoLoanAmountMTD = disbursedMemodetails.Where(d => disbursementMemosToday.Any(dis => dis.Id == d.DisbursementMemoId && dis.Application.Product.FacilityType == ProposalFacilityType.Auto_Loan)).Sum(d => d.DisbursementAmount);

                decimal totalwarAuto = 0;
                decimal sumofAmoutAuto = 0;
                decimal sumofRateAmoutProductAuto = 0;

                foreach (var w in disbursementMemosToday.Where(d => d.Application.Product.FacilityType == ProposalFacilityType.Auto_Loan).ToList())
                {
                    if (w.Proposal.RecomendedLoanAmountFromIPDC != null)
                    {
                        sumofAmoutAuto += (decimal)w.Proposal.RecomendedLoanAmountFromIPDC;
                        sumofRateAmoutProductAuto += (decimal)(w.Proposal.RecomendedLoanAmountFromIPDC * w.Proposal.InterestRateOffered);
                    }
                    totalwarAuto = sumofRateAmoutProductAuto / sumofAmoutAuto;
                }
                data.AutoLoanWarMTD = totalwarAuto;
            }

            if (disbursementMemosToday.Count(d => d.Application.Product.FacilityType == ProposalFacilityType.Personal_Loan) > 0)
            {
                data.Disbursed_PersonalLoanAmountMTD = disbursedMemodetails.Where(d => disbursementMemosToday.Any(dis => dis.Id == d.DisbursementMemoId && dis.Application.Product.FacilityType == ProposalFacilityType.Personal_Loan)).Sum(d => d.DisbursementAmount);

                decimal totalwarPersonal = 0;
                decimal sumofAmoutPersonal = 0;
                decimal sumofRateAmoutProductPersonal = 0;

                foreach (var w in disbursementMemosToday.Where(d => d.Application.Product.FacilityType == ProposalFacilityType.Personal_Loan).ToList())
                {
                    if (w.Proposal.RecomendedLoanAmountFromIPDC != null)
                    {
                        sumofAmoutPersonal += (decimal)w.Proposal.RecomendedLoanAmountFromIPDC;
                        sumofRateAmoutProductPersonal += (decimal)(w.Proposal.RecomendedLoanAmountFromIPDC * w.Proposal.InterestRateOffered);
                    }
                    totalwarPersonal = sumofRateAmoutProductPersonal / sumofAmoutPersonal;
                }
                data.PersonalLoanWarMTD = totalwarPersonal;
            }

            if (fundReceivedToday.Count(d => d.Application.Product.DepositType == DepositType.Fixed) > 0)
            {
                decimal totalwarFixed = 0;
                decimal sumofAmoutFixed = 0;
                decimal sumofRateAmoutProductFixed = 0;

                var sumFixed = fundConfirmdetails.Where(d => fundReceivedToday.Any(dis => dis.Id == d.FundConfirmationId && dis.Application.Product.DepositType == DepositType.Fixed)).Sum(d => d.Amount);
                if (sumFixed != null)
                    data.FundReceived_FixedAmountMTD = (decimal)sumFixed;

                foreach (var w in fundReceivedToday.Where(d => d.Application.Product.DepositType == DepositType.Fixed).ToList())
                {
                    if (w.Application.DepositApplicationId != null)
                    {
                        sumofAmoutFixed += w.Application.DepositApplication.TotalDepositAmount;
                        sumofRateAmoutProductFixed += (decimal)(w.Application.DepositApplication.TotalDepositAmount * w.Application.DepositApplication.OfferRate);
                    }

                    totalwarFixed = sumofRateAmoutProductFixed / sumofAmoutFixed;
                }
                data.FixedDepositWarMTD = totalwarFixed;
            }

            if (fundReceivedToday.Count(d => d.Application.Product.DepositType == DepositType.Recurring) > 0)
            {
                decimal totalwarRecurrent = 0;
                decimal sumofAmoutRecurrent = 0;
                decimal sumofRateAmoutProductRecurrent = 0;

                var sumRecurrent = fundConfirmdetails.Where(d => fundReceivedToday.Any(dis => dis.Id == d.FundConfirmationId && dis.Application.Product.DepositType == DepositType.Recurring)).Sum(d => d.Amount);
                if (sumRecurrent != null)
                    data.FundReceived_RecurrentAmountMTD = (decimal)sumRecurrent;

                foreach (var w in fundReceivedToday.Where(d => d.Application.Product.DepositType == DepositType.Recurring).ToList())
                {
                    if (w.Application.DepositApplicationId != null && w.Application.DepositApplication.OfferRate != null)
                    {
                        sumofAmoutRecurrent += w.Application.DepositApplication.TotalDepositAmount;
                        sumofRateAmoutProductRecurrent += (decimal)(w.Application.DepositApplication.TotalDepositAmount * w.Application.DepositApplication.OfferRate);
                    }
                    if (sumofAmoutRecurrent != 0)
                        totalwarRecurrent = sumofRateAmoutProductRecurrent / sumofAmoutRecurrent;
                }
                data.ReccurentDepositWarMTD = totalwarRecurrent;
            }

            #endregion Disbursed/Received Amount/War

            return data;
        }

        public object GetNSMDisiburesedReceivedLMTD(TimeLine? timeLine, long? branchId)
        {
            //long UserId,
            var data = new DashBoardHighlightsDto();
            DateRangeDto dateRange;
            if (timeLine == null)
                timeLine = TimeLine.Today;
            dateRange = new DateRangeDto((TimeLine)timeLine);


            var disbursementMemosToday = new List<DisbursementMemo>();

            var fundReceivedToday = new List<FundConfirmation>();

            var disbursedMemodetails = new List<DMDetail>();
            disbursedMemodetails = GenService.GetAll<DMDetail>().Where(dm => dm.Status == EntityStatus.Active).ToList();
            var fundConfirmdetails = new List<FundConfirmationDetail>();
            fundConfirmdetails = GenService.GetAll<FundConfirmationDetail>().Where(fc => fc.Status == EntityStatus.Active).ToList();

            long userId = 0;

            if (branchId != null || branchId > 0)
            {
                //var employeeList = _office.GetEmployeesByOfficeId((long)branchId);

                List<EmployeeDto> subordinateEmpList = new List<EmployeeDto>();
                if (branchId > 0)
                    subordinateEmpList = _office.GetEmployeesByOfficeId((long)branchId);
                var subordinateUserList = new List<long>();
                foreach (var sub in subordinateEmpList)
                {
                    subordinateUserList.Add((long)_user.GetUserByEmployeeId((long)sub.Id).Id);
                }

                #region Amount disbursed / received Today

                var TodaydisbursementMemos = GenService.GetAll<DMDetail>().Where(d => d.DisbursementMemo.ApplicationId != null
                            && d.DisbursementMemo.Application.BranchId != null
                            && d.DisbursementMemo.Application.BranchId == branchId
                            && d.DisbursementMemo.Application.LoanApplicationId != null
                            && d.DisbursementMemo.Application.Product.FacilityType != null
                            && (d.DisbursementMemo.Application.Product.FacilityType == ProposalFacilityType.Home_Loan
                                || d.DisbursementMemo.Application.Product.FacilityType == ProposalFacilityType.Auto_Loan
                                || d.DisbursementMemo.Application.Product.FacilityType == ProposalFacilityType.Personal_Loan)
                            && d.CreateDate <= dateRange.ToDate
                            && d.CreateDate >= dateRange.FromDate
                            && d.DisbursementMemo.IsApproved != null
                            && d.DisbursementMemo.IsApproved == true
                            && d.DisbursementMemo.IsDisbursed != null
                            && d.DisbursementMemo.IsDisbursed == true
                            && d.Status == EntityStatus.Active).ToList();
                var acceptableDMidstoday = TodaydisbursementMemos.Select(dm => dm.DisbursementMemoId).ToList();
                var disMemotoday = GenService.GetAll<DisbursementMemo>().Where(d => acceptableDMidstoday.Contains(d.Id)).ToList();
                disbursementMemosToday = disMemotoday.ToList();

                var TodayfundReceivedToday = GenService.GetAll<FundConfirmationDetail>().Where(d => d.FundConfirmation.ApplicationId != null
                            && d.FundConfirmation.Application.BranchId != null
                            && d.FundConfirmation.Application.BranchId == branchId
                            && d.FundConfirmation.Application.DepositApplicationId != null
                            && d.FundConfirmation.Application.Product.DepositType != null
                            && (d.FundConfirmation.Application.Product.DepositType == DepositType.Fixed
                                || d.FundConfirmation.Application.Product.DepositType == DepositType.Recurring)
                            && d.CreateDate <= dateRange.ToDate
                            && d.CreateDate >= dateRange.FromDate
                            && d.FundConfirmation.FundReceived != null
                            && d.FundConfirmation.FundReceived == true
                            && d.Status == EntityStatus.Active).ToList();
                var acceptableFCidstoday = TodayfundReceivedToday.Select(dm => dm.FundConfirmationId).ToList();
                var fundRectoday = GenService.GetAll<FundConfirmation>().Where(d => acceptableFCidstoday.Contains(d.Id)).ToList();
                fundReceivedToday = fundRectoday.ToList();

                #endregion Amount disbursed / received Today

            }
            else
            {

                #region File disbursed / received Today
                var TodaydisbursementMemos = GenService.GetAll<DMDetail>().Where(d => d.DisbursementMemo.ApplicationId != null

                            && d.DisbursementMemo.Application.LoanApplicationId != null
                            && d.DisbursementMemo.Application.Product.FacilityType != null
                            && (d.DisbursementMemo.Application.Product.FacilityType == ProposalFacilityType.Home_Loan
                                || d.DisbursementMemo.Application.Product.FacilityType == ProposalFacilityType.Auto_Loan
                                || d.DisbursementMemo.Application.Product.FacilityType == ProposalFacilityType.Personal_Loan)
                            && d.CreateDate <= dateRange.ToDate
                            && d.CreateDate >= dateRange.FromDate
                            && d.DisbursementMemo.IsApproved != null
                            && d.DisbursementMemo.IsApproved == true
                            && d.DisbursementMemo.IsDisbursed != null
                            && d.DisbursementMemo.IsDisbursed == true
                            && d.Status == EntityStatus.Active).ToList();
                var acceptableDMids = TodaydisbursementMemos.Select(dm => dm.DisbursementMemoId).ToList();
                var disMemotoday = GenService.GetAll<DisbursementMemo>().Where(d => acceptableDMids.Contains(d.Id)).ToList();
                disbursementMemosToday = disMemotoday.ToList();

                var TodayfundReceived = GenService.GetAll<FundConfirmationDetail>().Where(d => d.FundConfirmation.ApplicationId != null

                            && d.FundConfirmation.Application.DepositApplicationId != null
                            && d.FundConfirmation.Application.Product.DepositType != null
                            && (d.FundConfirmation.Application.Product.DepositType == DepositType.Fixed
                                || d.FundConfirmation.Application.Product.DepositType == DepositType.Recurring)
                            && d.CreateDate <= dateRange.ToDate
                            && d.CreateDate >= dateRange.FromDate
                            && d.FundConfirmation.FundReceived != null
                            && d.FundConfirmation.FundReceived == true
                            && d.Status == EntityStatus.Active).ToList();
                var acceptableFCids = TodayfundReceived.Select(dm => dm.FundConfirmationId).ToList();
                var fundRectoday = GenService.GetAll<FundConfirmation>().Where(d => acceptableFCids.Contains(d.Id)).ToList();
                fundReceivedToday = fundRectoday.ToList();

                #endregion File disbursed / received Today


            }

            #region initializations

            data.Disbursed_HomeLoanAmount = 0;
            data.Disbursed_PersonalLoanAmount = 0;
            data.Disbursed_AutoLoanAmount = 0;
            data.RMHomeLoan1 = "";
            data.RMHomeLoan1Amount = 0;
            data.RMHomeLoan1Count = 0;
            data.RMHomeLoan2 = "";
            data.RMHomeLoan2Amount = 0;
            data.RMHomeLoan2Count = 0;
            data.RMPersonalLoan = "";
            data.RMPersonalLoanAmount = 0;
            data.RMPersonalLoanCount = 0;
            data.RMAutoLoan = "";
            data.RMAutoLoanAmount = 0;
            data.RMAutoLoanCount = 0;
            data.RMLiability1 = "";
            data.RMLiability1Amount = 0;
            data.RMLiability1Count = 0;
            data.RMLiability2 = "";
            data.RMLiability2Amount = 0;
            data.RMLiability2Count = 0;
            #endregion

            #region Disbursed/Received No. Of Files

            data.Disbursed_HomeLoanLMTD = disbursementMemosToday.Where(d => d.Application.Product.FacilityType == ProposalFacilityType.Home_Loan).Count();


            data.Disbursed_AutoLoanLMTD = disbursementMemosToday.Where(d => d.Application.Product.FacilityType == ProposalFacilityType.Auto_Loan).Count();


            data.Disbursed_PersonalLoanLMTD = disbursementMemosToday.Where(d => d.Application.Product.FacilityType == ProposalFacilityType.Personal_Loan).Count();


            data.Received_FixedLMTD = fundReceivedToday.Where(d => d.Application.Product.DepositType == DepositType.Fixed).Count();


            data.Received_RecurrentLMTD = fundReceivedToday.Where(d => d.Application.Product.DepositType == DepositType.Recurring).Count();


            #endregion Disbursed/Received No. Of Files

            #region Disbursed/Received Amount/War

            if (disbursementMemosToday.Count(d => d.Application.Product.FacilityType == ProposalFacilityType.Home_Loan) > 0)
            {
                decimal totalwarHome = 0;
                decimal sumofAmoutHome = 0;
                decimal sumofRateAmoutProductHome = 0;

                data.Disbursed_HomeLoanAmountLMTD = disbursedMemodetails.Where(d => disbursementMemosToday.Any(dis => dis.Id == d.DisbursementMemoId && dis.Application.Product.FacilityType == ProposalFacilityType.Home_Loan)).Sum(d => d.DisbursementAmount);

                foreach (var w in disbursementMemosToday.Where(d => d.Application.Product.FacilityType == ProposalFacilityType.Home_Loan).ToList())
                {
                    if (w.Proposal.RecomendedLoanAmountFromIPDC != null)
                    {
                        sumofAmoutHome += (decimal)w.Proposal.RecomendedLoanAmountFromIPDC;
                        sumofRateAmoutProductHome += (decimal)(w.Proposal.RecomendedLoanAmountFromIPDC * w.Proposal.InterestRateOffered);
                    }
                    totalwarHome = sumofRateAmoutProductHome / sumofAmoutHome;
                }
                data.HomeLoanWarLMTD = totalwarHome;
            }

            if (disbursementMemosToday.Count(d => d.Application.Product.FacilityType == ProposalFacilityType.Auto_Loan) > 0)
            {
                data.Disbursed_AutoLoanAmountLMTD = disbursedMemodetails.Where(d => disbursementMemosToday.Any(dis => dis.Id == d.DisbursementMemoId && dis.Application.Product.FacilityType == ProposalFacilityType.Auto_Loan)).Sum(d => d.DisbursementAmount);

                decimal totalwarAuto = 0;
                decimal sumofAmoutAuto = 0;
                decimal sumofRateAmoutProductAuto = 0;

                foreach (var w in disbursementMemosToday.Where(d => d.Application.Product.FacilityType == ProposalFacilityType.Auto_Loan).ToList())
                {
                    if (w.Proposal.RecomendedLoanAmountFromIPDC != null)
                    {
                        sumofAmoutAuto += (decimal)w.Proposal.RecomendedLoanAmountFromIPDC;
                        sumofRateAmoutProductAuto += (decimal)(w.Proposal.RecomendedLoanAmountFromIPDC * w.Proposal.InterestRateOffered);
                    }
                    totalwarAuto = sumofRateAmoutProductAuto / sumofAmoutAuto;
                }
                data.AutoLoanWarLMTD = totalwarAuto;
            }

            if (disbursementMemosToday.Count(d => d.Application.Product.FacilityType == ProposalFacilityType.Personal_Loan) > 0)
            {
                data.Disbursed_PersonalLoanAmountLMTD = disbursedMemodetails.Where(d => disbursementMemosToday.Any(dis => dis.Id == d.DisbursementMemoId && dis.Application.Product.FacilityType == ProposalFacilityType.Personal_Loan)).Sum(d => d.DisbursementAmount);

                decimal totalwarPersonal = 0;
                decimal sumofAmoutPersonal = 0;
                decimal sumofRateAmoutProductPersonal = 0;

                foreach (var w in disbursementMemosToday.Where(d => d.Application.Product.FacilityType == ProposalFacilityType.Personal_Loan).ToList())
                {
                    if (w.Proposal.RecomendedLoanAmountFromIPDC != null)
                    {
                        sumofAmoutPersonal += (decimal)w.Proposal.RecomendedLoanAmountFromIPDC;
                        sumofRateAmoutProductPersonal += (decimal)(w.Proposal.RecomendedLoanAmountFromIPDC * w.Proposal.InterestRateOffered);
                    }
                    totalwarPersonal = sumofRateAmoutProductPersonal / sumofAmoutPersonal;
                }
                data.PersonalLoanWarLMTD = totalwarPersonal;
            }

            if (fundReceivedToday.Count(d => d.Application.Product.DepositType == DepositType.Fixed) > 0)
            {
                decimal totalwarFixed = 0;
                decimal sumofAmoutFixed = 0;
                decimal sumofRateAmoutProductFixed = 0;

                var sumFixed = fundConfirmdetails.Where(d => fundReceivedToday.Any(dis => dis.Id == d.FundConfirmationId && dis.Application.Product.DepositType == DepositType.Fixed)).Sum(d => d.Amount);
                if (sumFixed != null)
                    data.FundReceived_FixedAmountLMTD = (decimal)sumFixed;

                foreach (var w in fundReceivedToday.Where(d => d.Application.Product.DepositType == DepositType.Fixed).ToList())
                {
                    if (w.Application.DepositApplicationId != null)
                    {
                        sumofAmoutFixed += w.Application.DepositApplication.TotalDepositAmount;
                        sumofRateAmoutProductFixed += (decimal)(w.Application.DepositApplication.TotalDepositAmount * w.Application.DepositApplication.OfferRate);
                    }

                    totalwarFixed = sumofRateAmoutProductFixed / sumofAmoutFixed;
                }
                data.FixedDepositWarLMTD = totalwarFixed;
            }

            if (fundReceivedToday.Count(d => d.Application.Product.DepositType == DepositType.Recurring) > 0)
            {
                decimal totalwarRecurrent = 0;
                decimal sumofAmoutRecurrent = 0;
                decimal sumofRateAmoutProductRecurrent = 0;

                var sumRecurrent = fundConfirmdetails.Where(d => fundReceivedToday.Any(dis => dis.Id == d.FundConfirmationId && dis.Application.Product.DepositType == DepositType.Recurring)).Sum(d => d.Amount);
                if (sumRecurrent != null)
                    data.FundReceived_RecurrentAmountLMTD = (decimal)sumRecurrent;

                foreach (var w in fundReceivedToday.Where(d => d.Application.Product.DepositType == DepositType.Recurring).ToList())
                {
                    if (w.Application.DepositApplicationId != null && w.Application.DepositApplication.OfferRate != null)
                    {
                        sumofAmoutRecurrent += w.Application.DepositApplication.TotalDepositAmount;
                        sumofRateAmoutProductRecurrent += (decimal)(w.Application.DepositApplication.TotalDepositAmount * w.Application.DepositApplication.OfferRate);
                    }

                    totalwarRecurrent = sumofRateAmoutProductRecurrent / sumofAmoutRecurrent;
                }
                data.ReccurentDepositWarLMTD = totalwarRecurrent;
            }

            #endregion Disbursed/Received Amount/War


            return data;
        }
        public object GetNSMDisiburesedReceivedLMTD(TimeLine? timeLine, List<long?> branchId)
        {
            //long UserId,
            var data = new DashBoardHighlightsDto();
            DateRangeDto dateRange;
            if (timeLine == null)
                timeLine = TimeLine.Today;
            dateRange = new DateRangeDto((TimeLine)timeLine);


            var disbursementMemosToday = new List<DisbursementMemo>();

            var fundReceivedToday = new List<FundConfirmation>();

            var disbursedMemodetails = new List<DMDetail>();
            disbursedMemodetails = GenService.GetAll<DMDetail>().Where(dm => dm.Status == EntityStatus.Active).ToList();
            var fundConfirmdetails = new List<FundConfirmationDetail>();
            fundConfirmdetails = GenService.GetAll<FundConfirmationDetail>().Where(fc => fc.Status == EntityStatus.Active).ToList();

            if (branchId != null && branchId.Count > 0)
            {
                List<EmployeeDto> subordinateEmpList = new List<EmployeeDto>();
                foreach (var eachBranch in branchId)
                {
                    if (eachBranch > 0)
                        subordinateEmpList.AddRange(_office.GetEmployeesByOfficeId((long)eachBranch));
                }
                var subordinateUserList = new List<long>();
                foreach (var sub in subordinateEmpList)
                {
                    subordinateUserList.Add((long)_user.GetUserByEmployeeId((long)sub.Id).Id);
                }

                #region Amount disbursed / received Today

                var TodaydisbursementMemos = GenService.GetAll<DMDetail>().Where(d => d.DisbursementMemo.ApplicationId != null
                            && d.DisbursementMemo.Application.BranchId != null
                            && branchId.Contains(d.DisbursementMemo.Application.BranchId)
                            && d.DisbursementMemo.Application.LoanApplicationId != null
                            && d.DisbursementMemo.Application.Product.FacilityType != null
                            && (d.DisbursementMemo.Application.Product.FacilityType == ProposalFacilityType.Home_Loan
                                || d.DisbursementMemo.Application.Product.FacilityType == ProposalFacilityType.Auto_Loan
                                || d.DisbursementMemo.Application.Product.FacilityType == ProposalFacilityType.Personal_Loan)
                            && d.CreateDate <= dateRange.ToDate
                            && d.CreateDate >= dateRange.FromDate
                            && d.DisbursementMemo.IsApproved != null
                            && d.DisbursementMemo.IsApproved == true
                            && d.DisbursementMemo.IsDisbursed != null
                            && d.DisbursementMemo.IsDisbursed == true
                            && d.Status == EntityStatus.Active).ToList();
                var acceptableDMidstoday = TodaydisbursementMemos.Select(dm => dm.DisbursementMemoId).ToList();
                var disMemotoday = GenService.GetAll<DisbursementMemo>().Where(d => acceptableDMidstoday.Contains(d.Id)).ToList();
                disbursementMemosToday = disMemotoday.ToList();

                var TodayfundReceivedToday = GenService.GetAll<FundConfirmationDetail>().Where(d => d.FundConfirmation.ApplicationId != null
                            && d.FundConfirmation.Application.BranchId != null
                            && branchId.Contains(d.FundConfirmation.Application.BranchId)
                            && d.FundConfirmation.Application.DepositApplicationId != null
                            && d.FundConfirmation.Application.Product.DepositType != null
                            && (d.FundConfirmation.Application.Product.DepositType == DepositType.Fixed
                                || d.FundConfirmation.Application.Product.DepositType == DepositType.Recurring)
                            && d.CreateDate <= dateRange.ToDate
                            && d.CreateDate >= dateRange.FromDate
                            && d.FundConfirmation.FundReceived != null
                            && d.FundConfirmation.FundReceived == true
                            && d.Status == EntityStatus.Active).ToList();
                var acceptableFCidstoday = TodayfundReceivedToday.Select(dm => dm.FundConfirmationId).ToList();
                var fundRectoday = GenService.GetAll<FundConfirmation>().Where(d => acceptableFCidstoday.Contains(d.Id)).ToList();
                fundReceivedToday = fundRectoday.ToList();

                #endregion Amount disbursed / received Today

            }
            else
            {

                #region File disbursed / received Today
                var TodaydisbursementMemos = GenService.GetAll<DMDetail>().Where(d => d.DisbursementMemo.ApplicationId != null

                            && d.DisbursementMemo.Application.LoanApplicationId != null
                            && d.DisbursementMemo.Application.Product.FacilityType != null
                            && (d.DisbursementMemo.Application.Product.FacilityType == ProposalFacilityType.Home_Loan
                                || d.DisbursementMemo.Application.Product.FacilityType == ProposalFacilityType.Auto_Loan
                                || d.DisbursementMemo.Application.Product.FacilityType == ProposalFacilityType.Personal_Loan)
                            && d.CreateDate <= dateRange.ToDate
                            && d.CreateDate >= dateRange.FromDate
                            && d.DisbursementMemo.IsApproved != null
                            && d.DisbursementMemo.IsApproved == true
                            && d.DisbursementMemo.IsDisbursed != null
                            && d.DisbursementMemo.IsDisbursed == true
                            && d.Status == EntityStatus.Active).ToList();
                var acceptableDMids = TodaydisbursementMemos.Select(dm => dm.DisbursementMemoId).ToList();
                var disMemotoday = GenService.GetAll<DisbursementMemo>().Where(d => acceptableDMids.Contains(d.Id)).ToList();
                disbursementMemosToday = disMemotoday.ToList();

                var TodayfundReceived = GenService.GetAll<FundConfirmationDetail>().Where(d => d.FundConfirmation.ApplicationId != null

                            && d.FundConfirmation.Application.DepositApplicationId != null
                            && d.FundConfirmation.Application.Product.DepositType != null
                            && (d.FundConfirmation.Application.Product.DepositType == DepositType.Fixed
                                || d.FundConfirmation.Application.Product.DepositType == DepositType.Recurring)
                            && d.CreateDate <= dateRange.ToDate
                            && d.CreateDate >= dateRange.FromDate
                            && d.FundConfirmation.FundReceived != null
                            && d.FundConfirmation.FundReceived == true
                            && d.Status == EntityStatus.Active).ToList();
                var acceptableFCids = TodayfundReceived.Select(dm => dm.FundConfirmationId).ToList();
                var fundRectoday = GenService.GetAll<FundConfirmation>().Where(d => acceptableFCids.Contains(d.Id)).ToList();
                fundReceivedToday = fundRectoday.ToList();

                #endregion File disbursed / received Today


            }

            #region initializations

            data.Disbursed_HomeLoanAmount = 0;
            data.Disbursed_PersonalLoanAmount = 0;
            data.Disbursed_AutoLoanAmount = 0;
            data.RMHomeLoan1 = "";
            data.RMHomeLoan1Amount = 0;
            data.RMHomeLoan1Count = 0;
            data.RMHomeLoan2 = "";
            data.RMHomeLoan2Amount = 0;
            data.RMHomeLoan2Count = 0;
            data.RMPersonalLoan = "";
            data.RMPersonalLoanAmount = 0;
            data.RMPersonalLoanCount = 0;
            data.RMAutoLoan = "";
            data.RMAutoLoanAmount = 0;
            data.RMAutoLoanCount = 0;
            data.RMLiability1 = "";
            data.RMLiability1Amount = 0;
            data.RMLiability1Count = 0;
            data.RMLiability2 = "";
            data.RMLiability2Amount = 0;
            data.RMLiability2Count = 0;
            #endregion

            #region Disbursed/Received No. Of Files

            data.Disbursed_HomeLoanLMTD = disbursementMemosToday.Where(d => d.Application.Product.FacilityType == ProposalFacilityType.Home_Loan).Count();


            data.Disbursed_AutoLoanLMTD = disbursementMemosToday.Where(d => d.Application.Product.FacilityType == ProposalFacilityType.Auto_Loan).Count();


            data.Disbursed_PersonalLoanLMTD = disbursementMemosToday.Where(d => d.Application.Product.FacilityType == ProposalFacilityType.Personal_Loan).Count();


            data.Received_FixedLMTD = fundReceivedToday.Where(d => d.Application.Product.DepositType == DepositType.Fixed).Count();


            data.Received_RecurrentLMTD = fundReceivedToday.Where(d => d.Application.Product.DepositType == DepositType.Recurring).Count();


            #endregion Disbursed/Received No. Of Files

            #region Disbursed/Received Amount/War

            if (disbursementMemosToday.Count(d => d.Application.Product.FacilityType == ProposalFacilityType.Home_Loan) > 0)
            {
                decimal totalwarHome = 0;
                decimal sumofAmoutHome = 0;
                decimal sumofRateAmoutProductHome = 0;

                data.Disbursed_HomeLoanAmountLMTD = disbursedMemodetails.Where(d => disbursementMemosToday.Any(dis => dis.Id == d.DisbursementMemoId && dis.Application.Product.FacilityType == ProposalFacilityType.Home_Loan)).Sum(d => d.DisbursementAmount);

                foreach (var w in disbursementMemosToday.Where(d => d.Application.Product.FacilityType == ProposalFacilityType.Home_Loan).ToList())
                {
                    if (w.Proposal.RecomendedLoanAmountFromIPDC != null)
                    {
                        sumofAmoutHome += (decimal)w.Proposal.RecomendedLoanAmountFromIPDC;
                        sumofRateAmoutProductHome += (decimal)(w.Proposal.RecomendedLoanAmountFromIPDC * w.Proposal.InterestRateOffered);
                    }
                    totalwarHome = sumofRateAmoutProductHome / sumofAmoutHome;
                }
                data.HomeLoanWarLMTD = totalwarHome;
            }

            if (disbursementMemosToday.Count(d => d.Application.Product.FacilityType == ProposalFacilityType.Auto_Loan) > 0)
            {
                data.Disbursed_AutoLoanAmountLMTD = disbursedMemodetails.Where(d => disbursementMemosToday.Any(dis => dis.Id == d.DisbursementMemoId && dis.Application.Product.FacilityType == ProposalFacilityType.Auto_Loan)).Sum(d => d.DisbursementAmount);

                decimal totalwarAuto = 0;
                decimal sumofAmoutAuto = 0;
                decimal sumofRateAmoutProductAuto = 0;

                foreach (var w in disbursementMemosToday.Where(d => d.Application.Product.FacilityType == ProposalFacilityType.Auto_Loan).ToList())
                {
                    if (w.Proposal.RecomendedLoanAmountFromIPDC != null)
                    {
                        sumofAmoutAuto += (decimal)w.Proposal.RecomendedLoanAmountFromIPDC;
                        sumofRateAmoutProductAuto += (decimal)(w.Proposal.RecomendedLoanAmountFromIPDC * w.Proposal.InterestRateOffered);
                    }
                    totalwarAuto = sumofRateAmoutProductAuto / sumofAmoutAuto;
                }
                data.AutoLoanWarLMTD = totalwarAuto;
            }

            if (disbursementMemosToday.Count(d => d.Application.Product.FacilityType == ProposalFacilityType.Personal_Loan) > 0)
            {
                data.Disbursed_PersonalLoanAmountLMTD = disbursedMemodetails.Where(d => disbursementMemosToday.Any(dis => dis.Id == d.DisbursementMemoId && dis.Application.Product.FacilityType == ProposalFacilityType.Personal_Loan)).Sum(d => d.DisbursementAmount);

                decimal totalwarPersonal = 0;
                decimal sumofAmoutPersonal = 0;
                decimal sumofRateAmoutProductPersonal = 0;

                foreach (var w in disbursementMemosToday.Where(d => d.Application.Product.FacilityType == ProposalFacilityType.Personal_Loan).ToList())
                {
                    if (w.Proposal.RecomendedLoanAmountFromIPDC != null)
                    {
                        sumofAmoutPersonal += (decimal)w.Proposal.RecomendedLoanAmountFromIPDC;
                        sumofRateAmoutProductPersonal += (decimal)(w.Proposal.RecomendedLoanAmountFromIPDC * w.Proposal.InterestRateOffered);
                    }
                    totalwarPersonal = sumofRateAmoutProductPersonal / sumofAmoutPersonal;
                }
                data.PersonalLoanWarLMTD = totalwarPersonal;
            }

            if (fundReceivedToday.Count(d => d.Application.Product.DepositType == DepositType.Fixed) > 0)
            {
                decimal totalwarFixed = 0;
                decimal sumofAmoutFixed = 0;
                decimal sumofRateAmoutProductFixed = 0;

                var sumFixed = fundConfirmdetails.Where(d => fundReceivedToday.Any(dis => dis.Id == d.FundConfirmationId && dis.Application.Product.DepositType == DepositType.Fixed)).Sum(d => d.Amount);
                if (sumFixed != null)
                    data.FundReceived_FixedAmountLMTD = (decimal)sumFixed;

                foreach (var w in fundReceivedToday.Where(d => d.Application.Product.DepositType == DepositType.Fixed).ToList())
                {
                    if (w.Application.DepositApplicationId != null)
                    {
                        sumofAmoutFixed += w.Application.DepositApplication.TotalDepositAmount;
                        sumofRateAmoutProductFixed += (decimal)(w.Application.DepositApplication.TotalDepositAmount * w.Application.DepositApplication.OfferRate);
                    }

                    totalwarFixed = sumofRateAmoutProductFixed / sumofAmoutFixed;
                }
                data.FixedDepositWarLMTD = totalwarFixed;
            }

            if (fundReceivedToday.Count(d => d.Application.Product.DepositType == DepositType.Recurring) > 0)
            {
                decimal totalwarRecurrent = 0;
                decimal sumofAmoutRecurrent = 0;
                decimal sumofRateAmoutProductRecurrent = 0;

                var sumRecurrent = fundConfirmdetails.Where(d => fundReceivedToday.Any(dis => dis.Id == d.FundConfirmationId && dis.Application.Product.DepositType == DepositType.Recurring)).Sum(d => d.Amount);
                if (sumRecurrent != null)
                    data.FundReceived_RecurrentAmountLMTD = (decimal)sumRecurrent;

                foreach (var w in fundReceivedToday.Where(d => d.Application.Product.DepositType == DepositType.Recurring).ToList())
                {
                    if (w.Application.DepositApplicationId != null && w.Application.DepositApplication.OfferRate != null)
                    {
                        sumofAmoutRecurrent += w.Application.DepositApplication.TotalDepositAmount;
                        sumofRateAmoutProductRecurrent += (decimal)(w.Application.DepositApplication.TotalDepositAmount * w.Application.DepositApplication.OfferRate);
                    }

                    totalwarRecurrent = sumofRateAmoutProductRecurrent / sumofAmoutRecurrent;
                }
                data.ReccurentDepositWarLMTD = totalwarRecurrent;
            }

            #endregion Disbursed/Received Amount/War


            return data;
        }
        public List<NSMFileSubmissionDto> GetFileSubmissionForNSM(long? productId)//TimeLine? timeLine, long? stageId, long? criteriaId, long? centerId, long? productId, long? branchId
        {
            List<NSMFileSubmissionDto> aFileSubmissionDto = new List<NSMFileSubmissionDto>();
            var data = new LeaderBoardDto();
            DateRangeDto dateRange;
            dateRange = new DateRangeDto(TimeLine.MTD);
            var secondaryDateRange = new DateRangeDto(TimeLine.LMTD);

            data.TopFirst = GetFileSubmittedDataForTimeLine(dateRange, productId);
            data.TopSecond = GetFileSubmittedDataForTimeLine(secondaryDateRange, productId);
            if (data.TopFirst != null)
            {
                aFileSubmissionDto = (from fdt in data.TopFirst
                                      join sdt in data.TopSecond on fdt.BranchId equals sdt.BranchId into extra
                                      from extr in extra.DefaultIfEmpty()
                                      select new NSMFileSubmissionDto
                                      {
                                          BranchId = fdt != null ? (long)fdt.BranchId : extr != null ? (long)extr.BranchId : 0,
                                          BranchName = fdt != null ? fdt.BranchName : extr != null ? extr.BranchName : "",
                                          FirstNoOfFiles = fdt.Number,
                                          FirstAmount = fdt.Number,
                                          FirstWAR = fdt.WAR,
                                          SecondNoOfFiles = extr != null ? extr.Number : 0,
                                          SecondAmount = extr != null ? (decimal)extr.Amount : 0,
                                          SecondWAR = extr != null ? extr.WAR : 0,
                                          GrowthNoOfFiles = extr != null && extr.Number != 0 && fdt != null ? (((fdt.Number - extr.Number) / extr.Number) * 100) : 0,
                                          GrowthAmount = extr != null && extr.Amount != 0 && fdt != null ? (decimal)(((fdt.Amount - extr.Amount) / extr.Amount) * 100) : 0,
                                          GrowthWAR = extr != null && extr.Amount != 0 && fdt != null ? (((fdt.WAR - extr.WAR) / extr.WAR) * 100) : 0
                                      }).ToList();
            }
            return aFileSubmissionDto;
        }
        public List<PerformerDto> GetFileSubmittedDataForTimeLine(DateRangeDto dateRange, long? productId)
        {
            #region File Submited Apps
            var allApplication = GenService.GetAll<Application>().Where(r => r.Status == EntityStatus.Active);
            var acceptableLogsToday = GenService.GetAll<ApplicationLog>()
                .Where(l => (l.Application.LoanApplicationId != null || l.Application.DepositApplicationId != null)
                && l.Status == EntityStatus.Active
                && l.CreateDate <= dateRange.ToDate
                && l.CreateDate >= dateRange.FromDate
                && (((l.ToStage == ApplicationStage.SentToCRM || l.ToStage == ApplicationStage.UnderProcessAtCRM)
                && l.Application.Product.ProductType == ProductType.Loan)
                || ((l.ToStage == ApplicationStage.SentToOperations || l.ToStage == ApplicationStage.UnderProcessAtOperations)
                && l.Application.Product.ProductType == ProductType.Deposit))).ToList();
            if (productId != null)
            {
                acceptableLogsToday = acceptableLogsToday.Where(r => r.Application.ProductId == productId).ToList();
            }
            var allApplicationsToday = acceptableLogsToday.OrderBy(r => r.Id).Distinct().ToList();
            //(from appLog in acceptableLogsToday
            //                        orderby appLog.Id
            //                        select appLog).Distinct().ToList();
            var log = allApplicationsToday.Select(l => l.ApplicationId).ToList();
            var fundReceived = GenService.GetAll<FundConfirmationDetail>().Where(f => log.Contains((long)f.FundConfirmation.ApplicationId) &&
                                                                                     f.FundConfirmation.Status == EntityStatus.Active &&
                                                                                     f.Status == EntityStatus.Active &&
                                                                                     f.FundConfirmation.FundReceived != null &&
                                                                                     f.FundConfirmation.FundReceived == true &&
                                                                                      f.CreditDate != null).ToList();
            var fundsReceivedForMtd = fundReceived.Where(f => f.CreditDate <= dateRange.ToDate && f.CreditDate >= dateRange.FromDate).ToList();

            var proposal = GenService.GetAll<Proposal>().Where(l => log.Contains(l.ApplicationId) && l.Status == EntityStatus.Active);
            var dataFrom = (from appLog in allApplicationsToday
                            join prsl in proposal on appLog.ApplicationId equals prsl.ApplicationId into prslExtr
                            from prsldt in prslExtr.DefaultIfEmpty()
                            join fund in fundsReceivedForMtd on appLog.ApplicationId equals fund.FundConfirmation.ApplicationId into fundExtr
                            from funddt in fundExtr.DefaultIfEmpty()
                            select new PerformerDto
                            {
                                BranchId = appLog.Application.BranchId,
                                BranchName = appLog.Application.BranchOffice.Name,
                                Number = 0,
                                Amount = prsldt != null ? prsldt.RecomendedLoanAmountFromIPDC : funddt != null ? funddt.Amount : 0,
                                WAR = prsldt != null ? (int)(((prsldt.RecomendedLoanAmountFromIPDC != null ? prsldt.RecomendedLoanAmountFromIPDC : 0) * prsldt.InterestRateOffered) / (prsldt.RecomendedLoanAmountFromIPDC != null && prsldt.RecomendedLoanAmountFromIPDC != 0 ? prsldt.RecomendedLoanAmountFromIPDC : 1)) : 0
                            }).ToList();
            var grpDataFrom = (from dt in dataFrom
                               group dt by new { dt.BranchId } into grp
                               let firstOrDefault = grp.FirstOrDefault()
                               where firstOrDefault != null
                               select new PerformerDto
                               {
                                   BranchId = grp.Key.BranchId,
                                   BranchName = firstOrDefault.BranchName,
                                   Number = grp.Count(),
                                   Amount = grp.Sum(r => r.Amount),
                                   WAR = grp.Sum(r => r.WAR)
                               }).ToList();

            #endregion File Submited Apps

            return grpDataFrom;
        }


        public List<NSMFileSubmissionDto> GetFileApprovedForNSM(long? productId)//TimeLine? timeLine, long? stageId, long? criteriaId, long? centerId, long? productId, long? branchId
        {
            List<NSMFileSubmissionDto> aFileSubmissionDto = new List<NSMFileSubmissionDto>();
            var data = new LeaderBoardDto();
            DateRangeDto dateRange;
            dateRange = new DateRangeDto(TimeLine.MTD);
            var secondaryDateRange = new DateRangeDto(TimeLine.LMTD);

            data.TopFirst = GetFileApprovedDataForTimeLine(dateRange, productId);
            data.TopSecond = GetFileApprovedDataForTimeLine(secondaryDateRange, productId);
            if (data.TopFirst != null)
            {
                aFileSubmissionDto = (from fdt in data.TopFirst
                                      join sdt in data.TopSecond on fdt.BranchId equals sdt.BranchId into extra
                                      from extr in extra.DefaultIfEmpty()
                                      select new NSMFileSubmissionDto
                                      {
                                          BranchId = fdt != null ? (long)fdt.BranchId : extr != null ? (long)extr.BranchId : 0,
                                          BranchName = fdt != null ? fdt.BranchName : extr != null ? extr.BranchName : "",
                                          FirstNoOfFiles = fdt.Number,
                                          FirstAmount = fdt.Number,
                                          FirstContribution = (decimal)fdt.Contribution,
                                          SecondNoOfFiles = extr != null ? extr.Number : 0,
                                          SecondAmount = extr != null ? (decimal)extr.Amount : 0,
                                          SecondContribution = extr != null ? (decimal)extr.Contribution : 0,
                                          GrowthNoOfFiles = extr != null && fdt != null && extr.Number != 0 ? (((fdt.Number - extr.Number) / extr.Number) * 100) : 0,
                                          GrowthAmount = extr != null && fdt != null && extr.Amount != 0 ? (decimal)(((fdt.Amount - extr.Amount) / extr.Amount) * 100) : 0,
                                          GrowthContribution = extr != null && fdt != null && extr.Contribution != 0 ? (decimal)(((fdt.Contribution - extr.Contribution) / extr.Contribution) * 100) : 0
                                      }).ToList();
            }
            return aFileSubmissionDto;
        }
        public List<PerformerDto> GetFileApprovedDataForTimeLine(DateRangeDto dateRange, long? productId)
        {
            #region File Approved Apps
            var allApplication = GenService.GetAll<Application>().Where(r => r.Status == EntityStatus.Active);
            var loanApplicationsIds = allApplication.Where(a => a.Product.ProductType == ProductType.Loan && a.LoanApplication.LoanAmountApplied != null).Select(a => a.Id);
            var acceptableLogsToday = GenService.GetAll<ApplicationLog>()
                    .Where(l => l.Status == EntityStatus.Active &&
                                l.ToStage == ApplicationStage.SentToOperations &&
                                loanApplicationsIds.Contains(l.ApplicationId) &&
                                l.CreateDate <= dateRange.ToDate &&
                                l.CreateDate >= dateRange.FromDate);
            if (productId != null)
            {
                acceptableLogsToday = acceptableLogsToday.Where(r => r.Application.ProductId == productId);
            }
            var allApplicationsToday = acceptableLogsToday.OrderBy(r => r.Id).Distinct().ToList();
            var log = allApplicationsToday.Select(l => l.ApplicationId).ToList();
            //var allApplicationsToday = (from appLog in acceptableLogsToday
            //                            orderby appLog.Id
            //                            select appLog).Distinct().ToList();
            var fundReceived = GenService.GetAll<FundConfirmationDetail>().Where(f => log.Contains((long)f.FundConfirmation.ApplicationId) &&
                                                                                   f.FundConfirmation.Status == EntityStatus.Active &&
                                                                                   f.Status == EntityStatus.Active &&
                                                                                   f.FundConfirmation.FundReceived != null &&
                                                                                   f.FundConfirmation.FundReceived == true &&
                                                                                    f.CreditDate != null);
            var fundsReceivedForMtd = fundReceived.Where(f => f.CreditDate <= dateRange.ToDate && f.CreditDate >= dateRange.FromDate).ToList();
            var applicationsIds = allApplicationsToday.Select(r => r.ApplicationId).ToList();
            var proposal = GenService.GetAll<Proposal>().Where(l => applicationsIds.Contains(l.ApplicationId) && l.Status == EntityStatus.Active).ToList();
            var dataFrom = (from appLog in allApplicationsToday
                            join app in allApplication on appLog.ApplicationId equals app.Id
                            join prsl in proposal on appLog.ApplicationId equals prsl.ApplicationId into prslExtr
                            from prsldt in prslExtr.DefaultIfEmpty()
                            join fund in fundsReceivedForMtd on appLog.ApplicationId equals fund.FundConfirmation.ApplicationId into fundExtr
                            from funddt in fundExtr.DefaultIfEmpty()
                            select new PerformerDto
                            {
                                BranchId = app.BranchId,
                                BranchName = app.BranchOffice.Name,
                                Number = 0,
                                Amount = prsldt != null ? prsldt.RecomendedLoanAmountFromIPDC : funddt != null ? funddt.Amount : 0,
                                WAR = prsldt != null ? (int)(((prsldt.RecomendedLoanAmountFromIPDC != null ? prsldt.RecomendedLoanAmountFromIPDC : 0) * prsldt.InterestRateOffered) / (prsldt.RecomendedLoanAmountFromIPDC != null && prsldt.RecomendedLoanAmountFromIPDC != 0 ? prsldt.RecomendedLoanAmountFromIPDC : 1)) : 0,
                                Contribution = 0
                            }).ToList();
            var grpDataFrom = (from dt in dataFrom
                               group dt by new { dt.BranchId } into grp
                               let firstOrDefault = grp.FirstOrDefault()
                               where firstOrDefault != null
                               select new PerformerDto
                               {
                                   BranchId = grp.Key.BranchId,
                                   BranchName = firstOrDefault.BranchName,
                                   Number = grp.Count(),
                                   Amount = grp.Sum(r => r.Amount),
                                   WAR = grp.Sum(r => r.WAR),
                                   Contribution = 0
                               }).ToList();
            if (grpDataFrom != null)
            {
                var total = grpDataFrom.Sum(l => l.Amount);
                if (total > 0)
                {
                    grpDataFrom.ForEach(l => l.Contribution = (l.Amount / total) * 100);
                }
            }
            #endregion
            return grpDataFrom;
        }


        public List<NSMFileApprovalRatioDto> GetApprovalRatioNSM(long? productId)//TimeLine? timeLine, long? stageId, long? criteriaId, long? centerId, long? productId, long? branchId
        {
            List<NSMFileApprovalRatioDto> aFileSubmissionDto = new List<NSMFileApprovalRatioDto>();
            var data = new LeaderBoardDto();
            DateRangeDto dateRange;
            dateRange = new DateRangeDto(TimeLine.MTD);
            var secondaryDateRange = new DateRangeDto(TimeLine.LMTD);

            var dataFdt = GetApprovalRatioForTimeLine(dateRange, productId);
            var dataSdt = GetApprovalRatioForTimeLine(secondaryDateRange, productId);
            if (data.TopFirst != null)
            {
                aFileSubmissionDto = (from fdt in dataFdt
                                      join sdt in dataSdt on fdt.BranchId equals sdt.BranchId into extra
                                      from extr in extra.DefaultIfEmpty()
                                      select new NSMFileApprovalRatioDto
                                      {
                                          BranchId = fdt != null ? (long)fdt.BranchId : extr != null ? (long)extr.BranchId : 0,
                                          BranchName = fdt != null ? fdt.BranchName : extr != null ? extr.BranchName : "",
                                          FirstCallConverSionSuccessful = fdt != null ? fdt.CallConverSionSuccessful : 0,
                                          FirstCallConverSionUnSuccessful = fdt != null ? fdt.CallConverSionUnSuccessful : 0,
                                          FirstCallConverSionRate = fdt != null ? fdt.CallConverSionRate : 0,
                                          FirstLeadConverSionSuccessful = fdt != null ? fdt.LeadConverSionSuccessful : 0,
                                          FirstLeadConverSionUnSuccessful = fdt != null ? fdt.LeadConverSionUnSuccessful : 0,
                                          FirstLeadConverSionRate = fdt != null ? fdt.LeadConverSionRate : 0,
                                          FirstFileApproved = fdt != null ? fdt.FileApproved : 0,
                                          FirstFileDisapproved = fdt != null ? fdt.FileDisapproved : 0,
                                          FirstApplicationApprovalRate = fdt != null ? fdt.ApplicationApprovalRate : 0,
                                          ScdCallConverSionSuccessful = extr != null ? extr.CallConverSionSuccessful : 0,
                                          ScdCallConverSionUnSuccessful = extr != null ? extr.CallConverSionUnSuccessful : 0,
                                          ScdCallConverSionRate = extr != null ? extr.CallConverSionRate : 0,
                                          ScdLeadConverSionSuccessful = extr != null ? extr.LeadConverSionSuccessful : 0,
                                          ScdLeadConverSionUnSuccessful = extr != null ? extr.LeadConverSionUnSuccessful : 0,
                                          ScdLeadConverSionRate = extr != null ? extr.LeadConverSionRate : 0,
                                          ScdFileApproved = extr != null ? extr.FileApproved : 0,
                                          ScdFileDisapproved = extr != null ? extr.FileDisapproved : 0,
                                          ScdApplicationApprovalRate = extr != null ? extr.ApplicationApprovalRate : 0,
                                          GrowthCallConversationRate = (int)(((fdt.CallConverSionRate - extr.CallConverSionRate) / extr.CallConverSionRate) * 100),
                                          GrowthLeadConversationRate = (int)(((fdt.LeadConverSionRate - extr.LeadConverSionRate) / extr.LeadConverSionRate) * 100),
                                          GrowthAppApprovalConversationRate = (int)(((fdt.ApplicationApprovalRate - extr.ApplicationApprovalRate) / extr.ApplicationApprovalRate) * 100),

                                      }).ToList();
            }
            return aFileSubmissionDto;
        }
        //ApprovalRatioDto
        public List<ApprovalRatioTimelineWiseDto> GetApprovalRatioForTimeLine(DateRangeDto dateRange, long? productId)
        {
            //var result = new ApprovalRatioTimelineWiseDto();
            var empList = new List<Employee>();
            empList = GenService.GetAll<Employee>().Where(r => r.Status == EntityStatus.Active).ToList();
            //if (branchId > 0)
            //subordinateEmpList = _office.GetEmployeesByOfficeId((long)branchId);
            var empDesgList = GenService.GetAll<EmployeeDesignationMapping>()
                .Where(e => e.Status == EntityStatus.Active &&
                            e.OfficeDesignationSetting.Status == EntityStatus.Active &&
                            e.Employee.Status == EntityStatus.Active);
            var empIdList = empList.Select(s => s.Id).ToList();
            var userListId = new List<long>();
            var userList = new List<UserDto>();
            foreach (var sub in empList)
            {
                var udserId = _user.GetUserByEmployeeId((long)sub.Id) != null ? _user.GetUserByEmployeeId((long)sub.Id).Id : null;
                if (udserId != null)
                {
                    userListId.Add((long)udserId);
                    userList.Add(_user.GetUserByEmployeeId((long)sub.Id));
                }
            }
            var allApplications = GenService.GetAll<Application>().Where(a => empIdList.Contains((long)a.RMId));
            if (productId != null)
            {
                allApplications = allApplications.Where(r => r.ProductId == productId);
            }
            #region Call
            //IQueryable<Call> 
            var allCallsToday = GenService.GetAll<Call>().Where(c => c.CreateDate <= dateRange.ToDate && c.CreateDate >= dateRange.FromDate && c.Amount != null && userListId.Contains((long)c.CreatedBy) && c.Status == EntityStatus.Active &&
                                                          c.CallStatus == CallStatus.Successful);

            //IQueryable<Call> 
            var allUnsuccessfulCallsToday = GenService.GetAll<Call>().Where(c => c.CreateDate <= dateRange.ToDate && c.CreateDate >= dateRange.FromDate && c.Amount != null && userListId.Contains((long)c.CreatedBy) && c.Status == EntityStatus.Active &&
                                                         c.CallStatus == CallStatus.Unsuccessful);
            if (productId != null)
            {
                allCallsToday = allCallsToday.Where(r => r.ProductId == productId);
                allUnsuccessfulCallsToday = allUnsuccessfulCallsToday.Where(r => r.ProductId == productId);
            }
            var successfulCalls = (from emp in empList
                                   join usr in userList on emp.Id equals usr.EmployeeId
                                   join des in empDesgList on emp.Id equals des.EmployeeId
                                   join cl in allCallsToday on usr.Id equals cl.CreatedBy
                                   select new ApprovalRatioTimelineWiseDto
                                   {
                                       BranchId = des != null ? (long)des.OfficeDesignationSetting.OfficeId : 0,
                                       BranchName = des != null ? des.OfficeDesignationSetting != null ? des.OfficeDesignationSetting.Office.Name : null : "",
                                       CallConverSionSuccessful = 0,
                                       CallConverSionUnSuccessful = 0,
                                       CallConverSionRate = 0,
                                       LeadConverSionSuccessful = 0,
                                       LeadConverSionUnSuccessful = 0,
                                       LeadConverSionRate = 0,
                                       FileApproved = 0,
                                       FileDisapproved = 0,
                                       ApplicationApprovalRate = 0
                                   }).ToList();
            var unSuccessfulCalls = (from emp in empList
                                     join usr in userList on emp.Id equals usr.EmployeeId
                                     join des in empDesgList on emp.Id equals des.EmployeeId
                                     join cl in allUnsuccessfulCallsToday on usr.Id equals cl.CreatedBy
                                     select new ApprovalRatioTimelineWiseDto
                                     {
                                         BranchId = des != null ? (long)des.OfficeDesignationSetting.OfficeId : 0,
                                         BranchName = des != null ? des.OfficeDesignationSetting != null ? des.OfficeDesignationSetting.Office.Name : null : "",
                                         CallConverSionSuccessful = 0,
                                         CallConverSionUnSuccessful = 0,
                                         CallConverSionRate = 0,
                                         LeadConverSionSuccessful = 0,
                                         LeadConverSionUnSuccessful = 0,
                                         LeadConverSionRate = 0,
                                         FileApproved = 0,
                                         FileDisapproved = 0,
                                         ApplicationApprovalRate = 0
                                     }).ToList();
            var successfulCallBranchWise = (from sc in successfulCalls
                                            group sc by new { sc.BranchId, sc.BranchName } into grp
                                            select new ApprovalRatioTimelineWiseDto
                                            {
                                                BranchId = grp.Key.BranchId,
                                                BranchName = grp.Key.BranchName,
                                                CallConverSionSuccessful = grp.Count(),
                                                CallConverSionUnSuccessful = 0,
                                                CallConverSionRate = 0,
                                                LeadConverSionSuccessful = 0,
                                                LeadConverSionUnSuccessful = 0,
                                                LeadConverSionRate = 0,
                                                FileApproved = 0,
                                                FileDisapproved = 0,
                                                ApplicationApprovalRate = 0
                                            }).ToList();

            var unSuccessfulCallBranchWise = (from sc in unSuccessfulCalls
                                              group sc by new { sc.BranchId, sc.BranchName } into grp
                                              select new ApprovalRatioTimelineWiseDto
                                              {
                                                  BranchId = grp.Key.BranchId,
                                                  BranchName = grp.Key.BranchName,
                                                  CallConverSionSuccessful = 0,
                                                  CallConverSionUnSuccessful = grp.Count(),
                                                  CallConverSionRate = 0,
                                                  LeadConverSionSuccessful = 0,
                                                  LeadConverSionUnSuccessful = 0,
                                                  LeadConverSionRate = 0,
                                                  FileApproved = 0,
                                                  FileDisapproved = 0,
                                                  ApplicationApprovalRate = 0
                                              }).ToList();
            var additionUnsuccessfulCalls = (from sc in successfulCallBranchWise
                                             join uSc in unSuccessfulCallBranchWise on sc.BranchId equals uSc.BranchId into extra
                                             from extr in extra.DefaultIfEmpty()
                                             select new ApprovalRatioTimelineWiseDto
                                             {
                                                 BranchId = sc.BranchId,
                                                 BranchName = sc.BranchName,
                                                 CallConverSionSuccessful = sc.CallConverSionSuccessful,
                                                 CallConverSionUnSuccessful = extr != null ? extr.CallConverSionUnSuccessful : 0,
                                                 CallConverSionRate = 0,
                                                 LeadConverSionSuccessful = 0,
                                                 LeadConverSionUnSuccessful = 0,
                                                 LeadConverSionRate = 0,
                                                 FileApproved = 0,
                                                 FileDisapproved = 0,
                                                 ApplicationApprovalRate = 0
                                             }).ToList();
            additionUnsuccessfulCalls.ForEach(r => r.CallConverSionRate = r.CallConverSionUnSuccessful > 0 ? ((r.CallConverSionSuccessful - r.CallConverSionUnSuccessful) / r.CallConverSionUnSuccessful) * 100 : 0);

            #endregion Call
            #region Lead
            var allLeadsToday = GenService.GetAll<SalesLead>().Where(c => c.CreateDate <= dateRange.ToDate && c.CreateDate >= dateRange.FromDate && c.Amount != null && empIdList.Contains((long)c.CreatedBy) && c.Status == EntityStatus.Active && c.LeadStatus == LeadStatus.Prospective).ToList();
            var allUscLeadsToday = GenService.GetAll<SalesLead>().Where(c => c.CreateDate <= dateRange.ToDate && c.CreateDate >= dateRange.FromDate && c.Amount != null && empIdList.Contains((long)c.CreatedBy) && c.Status == EntityStatus.Active && c.LeadStatus == LeadStatus.Unsuccessful).ToList();
            if (productId != null)
            {
                allLeadsToday = allLeadsToday.Where(r => r.ProductId == productId).ToList();
                allUscLeadsToday = allUscLeadsToday.Where(r => r.ProductId == productId).ToList();
            }

            var successfulLeads = (from leads in allLeadsToday
                                   join usr in userList on leads.CreatedBy equals usr.Id into usrExtr
                                   from u in usrExtr.DefaultIfEmpty()
                                   join des in empDesgList on u.EmployeeId equals des.EmployeeId into desExtra
                                   from extrdes in desExtra.DefaultIfEmpty()
                                   join cl in additionUnsuccessfulCalls on extrdes.OfficeDesignationSetting.OfficeId equals cl.BranchId
                                   //from emp in empList
                                   //join usr in userList on emp.Id equals usr.EmployeeId into usrExtr
                                   //from extr in usrExtr.DefaultIfEmpty()
                                   //join des in empDesgList on emp.Id equals des.EmployeeId into desExtra
                                   //from extrdes in desExtra.DefaultIfEmpty()
                                   //join ld in allLeadsToday on extr.Id equals ld.CreatedBy into ldExtra
                                   //join cl in additionUnsuccessfulCalls on extrdes.OfficeDesignationSetting.OfficeId equals cl.BranchId
                                   select new ApprovalRatioTimelineWiseDto
                                   {
                                       BranchId = extrdes != null ? (long)extrdes.OfficeDesignationSetting.OfficeId : 0,
                                       BranchName = extrdes != null ? extrdes.OfficeDesignationSetting != null ? extrdes.OfficeDesignationSetting.Office.Name : null : "",
                                       CallConverSionSuccessful = cl != null ? cl.CallConverSionSuccessful : 0,
                                       CallConverSionUnSuccessful = cl != null ? cl.CallConverSionUnSuccessful : 0,
                                       CallConverSionRate = cl != null ? cl.CallConverSionRate : 0,
                                       LeadConverSionSuccessful = 0,
                                       LeadConverSionUnSuccessful = 0,
                                       LeadConverSionRate = 0,
                                       FileApproved = 0,
                                       FileDisapproved = 0,
                                       ApplicationApprovalRate = 0
                                   }).ToList();
            //var unSuccessfulLeads = (
            //    from emp in empList
            //                         from usr in userList// on emp.Id equals usr.EmployeeId into usrExtr
            //                         //from extr in usrExtr.DefaultIfEmpty()
            //                         join des in empDesgList on emp.Id equals des.EmployeeId into desExtra
            //                         from extrdes in desExtra.DefaultIfEmpty()
            //                         join ld in allUscLeadsToday on (long?)usr.Id equals ld.CreatedBy into ldExtr
            //                         from ldEtr  in ldExtr.DefaultIfEmpty()
            //                         join cl in additionUnsuccessfulCalls on extrdes.OfficeDesignationSetting.OfficeId equals cl.BranchId
            //                         select new ApprovalRatioTimelineWiseDto
            //                         {
            //                             BranchId = extrdes != null ? (long)extrdes.OfficeDesignationSetting.OfficeId : 0,
            //                             BranchName = extrdes != null ? extrdes.OfficeDesignationSetting != null ? extrdes.OfficeDesignationSetting.Office.Name : null : "",
            //                             CallConverSionSuccessful = cl != null ? cl.CallConverSionSuccessful : 0,
            //                             CallConverSionUnSuccessful = cl != null ? cl.CallConverSionUnSuccessful : 0,
            //                             CallConverSionRate = cl != null ? cl.CallConverSionRate : 0,
            //                             LeadConverSionSuccessful = 0,
            //                             LeadConverSionUnSuccessful = 0,
            //                             LeadConverSionRate = 0,
            //                             FileApproved = 0,
            //                             FileDisapproved = 0,
            //                             ApplicationApprovalRate = 0
            //                         });
            var unSuccessfulLeads = (from leads in allUscLeadsToday
                                     join usr in userList on leads.CreatedBy equals usr.Id into usrExtr
                                     from u in usrExtr.DefaultIfEmpty()
                                     join des in empDesgList on u.EmployeeId equals des.EmployeeId into desExtra
                                     from extrdes in desExtra.DefaultIfEmpty()
                                     join cl in additionUnsuccessfulCalls on extrdes.OfficeDesignationSetting.OfficeId equals cl.BranchId
                                     select new ApprovalRatioTimelineWiseDto
                                     {
                                         BranchId = extrdes != null ? (long)extrdes.OfficeDesignationSetting.OfficeId : 0,
                                         BranchName = extrdes != null ? extrdes.OfficeDesignationSetting != null ? extrdes.OfficeDesignationSetting.Office.Name : null : "",
                                         CallConverSionSuccessful = cl != null ? cl.CallConverSionSuccessful : 0,
                                         CallConverSionUnSuccessful = cl != null ? cl.CallConverSionUnSuccessful : 0,
                                         CallConverSionRate = cl != null ? cl.CallConverSionRate : 0,
                                         LeadConverSionSuccessful = 0,
                                         LeadConverSionUnSuccessful = 0,
                                         LeadConverSionRate = 0,
                                         FileApproved = 0,
                                         FileDisapproved = 0,
                                         ApplicationApprovalRate = 0
                                     }).ToList();
            var successfulLeadsBranchWise = (from sc in successfulLeads
                                             group sc by new { sc.BranchId, sc.BranchName } into grp
                                             select new ApprovalRatioTimelineWiseDto
                                             {
                                                 BranchId = grp.Key.BranchId,
                                                 BranchName = grp.Key.BranchName,
                                                 CallConverSionSuccessful = grp.FirstOrDefault().CallConverSionSuccessful,
                                                 CallConverSionUnSuccessful = grp.FirstOrDefault().CallConverSionUnSuccessful,
                                                 CallConverSionRate = grp.FirstOrDefault().CallConverSionRate,
                                                 LeadConverSionSuccessful = grp.Count(),
                                                 LeadConverSionUnSuccessful = 0,
                                                 LeadConverSionRate = 0,
                                                 FileApproved = 0,
                                                 FileDisapproved = 0,
                                                 ApplicationApprovalRate = 0
                                             }).ToList();
            var UnsuccessfulLeadsBranchWise = (from sc in unSuccessfulLeads
                                               group sc by new { sc.BranchId, sc.BranchName } into grp
                                               select new ApprovalRatioTimelineWiseDto
                                               {
                                                   BranchId = grp.Key.BranchId,
                                                   BranchName = grp.Key.BranchName,
                                                   CallConverSionSuccessful = grp.FirstOrDefault().CallConverSionSuccessful,
                                                   CallConverSionUnSuccessful = grp.FirstOrDefault().CallConverSionUnSuccessful,
                                                   CallConverSionRate = grp.FirstOrDefault().CallConverSionRate,
                                                   LeadConverSionSuccessful = grp.Count(),
                                                   LeadConverSionUnSuccessful = 0,
                                                   LeadConverSionRate = 0,
                                                   FileApproved = 0,
                                                   FileDisapproved = 0,
                                                   ApplicationApprovalRate = 0
                                               }).ToList();
            var additionLeads = (from sl in successfulLeadsBranchWise
                                 join uSl in UnsuccessfulLeadsBranchWise on sl.BranchId equals uSl.BranchId into extra
                                 from extr in extra.DefaultIfEmpty()
                                 select new ApprovalRatioTimelineWiseDto
                                 {
                                     BranchId = extr != null ? extr.BranchId : 0,
                                     BranchName = sl != null ? sl.BranchName : "",
                                     CallConverSionSuccessful = sl != null ? sl.CallConverSionSuccessful : 0,
                                     CallConverSionUnSuccessful = sl != null ? sl.CallConverSionUnSuccessful : 0,
                                     CallConverSionRate = sl != null ? sl.CallConverSionRate : 0,
                                     LeadConverSionSuccessful = sl != null ? sl.LeadConverSionSuccessful : 0,
                                     LeadConverSionUnSuccessful = extr != null ? extr.LeadConverSionUnSuccessful : 0,
                                     LeadConverSionRate = 0,
                                     FileApproved = 0,
                                     FileDisapproved = 0,
                                     ApplicationApprovalRate = 0
                                 }).Where(r => r.BranchId > 0).ToList();
            additionLeads.ForEach(r => r.CallConverSionRate = r.LeadConverSionUnSuccessful > 0 ? ((r.LeadConverSionSuccessful - r.LeadConverSionUnSuccessful) / r.LeadConverSionUnSuccessful) * 100 : 0);
            #endregion Lead
            #region File Approved/DisApproved ToDay Apps.

            var loanApplicationsIds = allApplications.Select(a => a.Id).Distinct().ToList(); //Where(a => a.Product.ProductType == ProductType.Loan && a.LoanApplication.LoanAmountApplied != null).

            var approvedLoanApplicationLogsToday = GenService.GetAll<ApplicationLog>()
                .Where(l => l.Status == EntityStatus.Active &&
                            l.ToStage == ApplicationStage.SentToOperations &&
                            loanApplicationsIds.Contains(l.ApplicationId) &&
                            l.CreateDate <= dateRange.ToDate &&
                            l.CreateDate >= dateRange.FromDate).ToList();

            var disApporvedLoanApplicationLogsToday = GenService.GetAll<ApplicationLog>()
            .Where(l => l.Status == EntityStatus.Active &&
                        l.ToStage < ApplicationStage.Drafted &&
                        loanApplicationsIds.Contains(l.ApplicationId) &&
                        l.CreateDate <= dateRange.ToDate &&
                        l.CreateDate >= dateRange.FromDate).ToList();


            var successfulFilesBranchWise = (from sc in approvedLoanApplicationLogsToday
                                             join app in allApplications on sc.ApplicationId equals app.Id
                                             group app by new { app.BranchId } into grp
                                             select new ApprovalRatioTimelineWiseDto
                                             {
                                                 BranchId = (long)grp.Key.BranchId,
                                                 BranchName = "",
                                                 CallConverSionSuccessful = 0,
                                                 CallConverSionUnSuccessful = 0,
                                                 CallConverSionRate = 0,
                                                 LeadConverSionSuccessful = 0,
                                                 LeadConverSionUnSuccessful = 0,
                                                 LeadConverSionRate = 0,
                                                 FileApproved = grp.Count(),
                                                 FileDisapproved = 0,
                                                 ApplicationApprovalRate = 0
                                             }).ToList();//additionLeads
            var unSuccessfulFilesBranchWise = (from sc in disApporvedLoanApplicationLogsToday
                                               join app in allApplications on sc.ApplicationId equals app.Id
                                               group app by new { app.BranchId } into grp
                                               select new ApprovalRatioTimelineWiseDto
                                               {
                                                   BranchId = (long)grp.Key.BranchId,
                                                   BranchName = "",
                                                   CallConverSionSuccessful = 0,
                                                   CallConverSionUnSuccessful = 0,
                                                   CallConverSionRate = 0,
                                                   LeadConverSionSuccessful = 0,
                                                   LeadConverSionUnSuccessful = 0,
                                                   LeadConverSionRate = 0,
                                                   FileApproved = grp.Count(),
                                                   FileDisapproved = 0,
                                                   ApplicationApprovalRate = 0
                                               }).ToList();
            var additionFiles = (from sl in successfulFilesBranchWise
                                 join uSl in unSuccessfulFilesBranchWise on sl.BranchId equals uSl.BranchId into extra
                                 join lead in additionLeads on sl.BranchId equals lead.BranchId into extraBrch
                                 from extr in extra.DefaultIfEmpty()
                                 from ext in extraBrch.DefaultIfEmpty()
                                 select new ApprovalRatioTimelineWiseDto
                                 {
                                     BranchId = ext != null ? ext.BranchId : 0,
                                     BranchName = ext != null ? ext.BranchName : "",
                                     CallConverSionSuccessful = ext != null ? ext.CallConverSionSuccessful : 0,
                                     CallConverSionUnSuccessful = ext != null ? ext.CallConverSionUnSuccessful : 0,
                                     CallConverSionRate = ext != null ? ext.CallConverSionRate : 0,
                                     LeadConverSionSuccessful = ext != null ? ext.LeadConverSionSuccessful : 0,
                                     LeadConverSionUnSuccessful = ext != null ? ext.LeadConverSionUnSuccessful : 0,
                                     LeadConverSionRate = ext != null ? ext.LeadConverSionRate : 0,
                                     FileApproved = sl.FileApproved,
                                     FileDisapproved = sl.FileDisapproved,
                                     ApplicationApprovalRate = 0
                                 }).ToList();
            additionFiles.ForEach(r => r.ApplicationApprovalRate = r.FileDisapproved > 0 ? ((r.FileApproved - r.FileDisapproved) / r.FileDisapproved) * 100 : 0);


            #endregion File Approved/DisApproved ToDay Apps.

            return additionFiles.ToList();
        }


        public List<NSMAgingDto> GetAgingForNSM(long? productId)//TimeLine? timeLine, long? stageId, long? criteriaId, long? centerId, long? productId, long? branchId
        {
            List<NSMAgingDto> aging = new List<NSMAgingDto>();
            var data = new LeaderBoardDto();
            DateRangeDto dateRange;
            dateRange = new DateRangeDto(TimeLine.MTD);
            var secondaryDateRange = new DateRangeDto(TimeLine.LMTD);
            var dataFdt = GetAgingForNSMForTimeLine(dateRange, productId);
            var dataSdt = GetAgingForNSMForTimeLine(secondaryDateRange, productId);
            //if (data.TopFirst != null)
            //{
            aging = (from fdt in dataFdt
                     join sdt in dataSdt on fdt.BranchId equals sdt.BranchId into extra
                     from extr in extra.DefaultIfEmpty()
                     select new NSMAgingDto
                     {
                         BranchId = fdt != null ? (long)fdt.BranchId : extr != null ? (long)extr.BranchId : 0,
                         BranchName = fdt != null ? fdt.BranchName : extr != null ? extr.BranchName : "",
                         FirstCall = fdt != null ? (decimal)fdt.Call : 0,
                         FirstLead = fdt != null ? (decimal)fdt.Lead : 0,
                         FirstDataCollection = fdt != null ? (decimal)fdt.DataCollection : 0,
                         FirstSanction = fdt != null ? (decimal)fdt.Sanction : 0,
                         FirstDisbursment = fdt != null ? (decimal)fdt.Disbursment : 0,
                         //FirstTotal = fdt != null ? fdt. : 0,
                         ScdCall = extr != null ? (decimal)extr.Call : 0,
                         ScdLead = extr != null ? (decimal)extr.Lead : 0,
                         ScdDataCollection = extr != null ? (decimal)extr.DataCollection : 0,
                         ScdSanction = extr != null ? (decimal)extr.Sanction : 0,
                         ScdDisbursment = extr != null ? (decimal)extr.Disbursment : 0,
                         //ScdTotal = extr != null ? extr. : 0,
                         GrthCall = extr.Call != 0 ? (int)(((fdt.Call - extr.Call) / extr.Call) * 100) : 0,
                         GrthLead = extr.Call != 0 ? (int)(((fdt.Lead - extr.Lead) / extr.Lead) * 100) : 0,
                         GrthDataCollection = extr.Call != 0 ? (int)(((fdt.DataCollection - extr.DataCollection) / extr.DataCollection) * 100) : 0,
                         GrthSanction = extr.Call != 0 ? (int)(((fdt.Sanction - extr.Sanction) / extr.Sanction) * 100) : 0,
                         GrthDisbursment = extr.Call != 0 ? (int)(((fdt.Disbursment - extr.Disbursment) / extr.Disbursment) * 100) : 0,
                         //GrthTotal = fdt != null ? fdt. : 0,
                         //ScdApplicationApprovalRate = extr != null ? extr.ApplicationApprovalRate : 0,
                         //GrowthCallConversationRate = (int)(((fdt.CallConverSionRate - extr.CallConverSionRate) / extr.CallConverSionRate) * 100),
                         //GrowthLeadConversationRate = (int)(((fdt.LeadConverSionRate - extr.LeadConverSionRate) / extr.LeadConverSionRate) * 100),
                         //GrowthAppApprovalConversationRate = (int)(((fdt.ApplicationApprovalRate - extr.ApplicationApprovalRate) / extr.ApplicationApprovalRate) * 100),

                     }).ToList();
            // }
            return aging;
        }
        public List<NSMAgingTimelineDto> GetAgingForNSMForTimeLine(DateRangeDto dateRange, long? productId)
        {
            List<NSMAgingTimelineDto> aList = new List<NSMAgingTimelineDto>();
            var allApplications = GenService.GetAll<Application>();
            if (productId != null)
            {
                allApplications = allApplications.Where(r => r.ProductId == productId);
            }
            var allApplicationsIds = allApplications.Select(r => r.Id).Distinct().ToList();
            var disbursementMemos = GenService.GetAll<DisbursementMemo>().Where(d => d.ApplicationId != null &&
                                                                                    allApplicationsIds.Contains((long)d.ApplicationId) &&
                                                                                    d.Status == EntityStatus.Active &&
                                                                                    d.IsApproved != null &&
                                                                                    d.IsApproved == true &&
                                                                                    d.IsDisbursed != null &&
                                                                                    d.IsDisbursed == true &&
                                                                                    d.DisbursementDetails.FirstOrDefault().CreateDate <= dateRange.ToDate &&
                                                                                    d.DisbursementDetails.FirstOrDefault().CreateDate >= dateRange.FromDate);
            var disbursmentMemoAppIds = disbursementMemos.Select(l => l.ApplicationId).Distinct().ToList();
            var app = allApplications.Where(y => disbursmentMemoAppIds.Contains(y.Id)).ToList();
            var appLeadIds = app.Select(l => l.SalesLeadId).Distinct().ToList();
            var leads = GenService.GetAll<SalesLead>().Where(r => appLeadIds.Contains(r.Id)).ToList();
            var callIds = leads.Select(l => l.Call_Id).Distinct().ToList();
            var calls = GenService.GetAll<Call>().Where(r => callIds.Contains(r.Id)).ToList();
            foreach (var call in calls)
            {
                NSMAgingTimelineDto aging = new NSMAgingTimelineDto();
                var lead = leads.Where(r => r.Call_Id == call.Id).OrderByDescending(t => t.Id).FirstOrDefault();
                var appByLead = app.Where(r => r.SalesLeadId == lead.Id).OrderByDescending(t => t.Id).FirstOrDefault();
                if (call.EditDate != null && call.CreateDate != null)
                    aging.Call = ((DateTime)call.EditDate.Value.Date - (DateTime)call.CreateDate.Value.Date).TotalDays;
                if (lead.EditDate != null && lead.CreateDate != null)
                    aging.Lead = ((DateTime)lead.EditDate.Value.Date - (DateTime)lead.CreateDate.Value.Date).TotalDays;
                if (lead.EditDate != null && appByLead.CreateDate != null)
                    aging.DataCollection = ((DateTime)appByLead.CreateDate.Value.Date - (DateTime)lead.EditDate.Value.Date).TotalDays;
                var applogSentToCrm = new ApplicationLog();
                var applogSentToCrmdata = GenService.GetAll<ApplicationLog>().Where(r => r.ApplicationId == appByLead.Id && r.ToStage == ApplicationStage.SentToCRM).ToList();
                if (applogSentToCrmdata.Any())
                {
                    applogSentToCrm = applogSentToCrmdata.OrderByDescending(r => r.Id).FirstOrDefault();
                }

                var applogCrmApprvl = GenService.GetAll<ApplicationLog>().Where(r => r.ApplicationId == appByLead.Id && r.ToStage == ApplicationStage.SentToOperations).OrderByDescending(r => r.Id).FirstOrDefault();
                var disbursed = disbursementMemos.Where(t => t.ApplicationId == appByLead.Id).OrderByDescending(t => t.Id).FirstOrDefault();
                if (applogCrmApprvl != null && applogCrmApprvl.CreateDate != null && applogSentToCrm != null && applogSentToCrm.CreateDate != null)
                {
                    aging.Sanction = ((DateTime)applogCrmApprvl.CreateDate.Value.Date - (DateTime)applogSentToCrm.CreateDate.Value.Date).TotalDays;
                }
                if (applogCrmApprvl != null && applogCrmApprvl.CreateDate != null && disbursed != null && disbursed.EditDate != null)
                {
                    aging.Disbursment = ((DateTime)disbursed.EditDate.Value.Date - (DateTime)applogCrmApprvl.CreateDate.Value.Date).TotalDays;
                }
                aging.BranchId = (long)appByLead.BranchId;
                aging.BranchName = appByLead.BranchOffice.Name;
                aList.Add(aging);
            }
            var data = (from dt in aList
                        group dt by new { dt.BranchId, dt.BranchName } into grp
                        select new NSMAgingTimelineDto
                        {
                            BranchId = grp.Key.BranchId,
                            BranchName = grp.Key.BranchName,
                            Call = grp.Sum(r => r.Call) / grp.Count(),
                            Lead = grp.Sum(r => r.Lead) / grp.Count(),
                            DataCollection = grp.Sum(r => r.DataCollection) / grp.Count(),
                            Sanction = grp.Sum(r => r.Sanction) / grp.Count(),
                            Disbursment = grp.Sum(r => r.Disbursment) / grp.Count()
                        }).ToList();
            //aList = (from cl in calls
            //         join ld in leads on cl.Id equals ld.Call_Id
            //         join ap in app on ld.Id equals ap.SalesLeadId
            //         join dm in disbursementMemos on ap.Id equals dm.ApplicationId
            //         select new NSMAgingTimelineDto
            //         {
            //             BranchId = 0,
            //             BranchName = "",
            //             Call = ((DateTime)cl.EditDate.Value.Date - (DateTime)cl.CreateDate.Value.Date).TotalDays,
            //             Lead = 0,
            //             DataCollection = 0,
            //             Sanction = 0,
            //             Disbursment = 0
            //         }).ToList();
            return data;
        }

        public object GetMobileDashboardData(long UserId)
        {
            #region data population

            var today = new DateRangeDto(TimeLine.Today);
            var MTD = new DateRangeDto(TimeLine.MTD);
            var LMTD = new DateRangeDto(TimeLine.LMTD);
            var YTD = new DateRangeDto(TimeLine.YTD);
            var LYTD = new DateRangeDto(TimeLine.LYTD);

            long employeeId = _user.GetEmployeeIdByUserId(UserId);
            long offDegSettingId = _employee.GetOfficeDesignationIdOfEmployee(employeeId);
            long assigned = 0;
            if (offDegSettingId > 0)
            {
                assigned = offDegSettingId;
            }
            List<long> employeeIds = new List<long>();
            List<long> userIds = new List<long>();
            userIds.Add(UserId);
            employeeIds = _employee.GetEmployeeWiseBM(employeeId).Select(e => (long)e.EmployeeId).ToList();
            employeeIds.Add(employeeId);
            var leadIds = GenService.GetAll<SalesLeadAssignment>()
                .Where(s => employeeIds.Contains((long)s.AssignedToEmpId) && s.Status == EntityStatus.Active)
                .Select(s => s.SalesLeadId)
                .ToList();
            var allLeads =
                GenService.GetAll<SalesLead>().Where(s => leadIds.Contains(s.Id) && s.Status == EntityStatus.Active);
            int activeLeads = allLeads.Where(s => s.LeadStatus == LeadStatus.Drafted).Count();
            ////////
            //long employeeId = _user.GetEmployeeIdByUserId(UserId);
            var inboxMessages = GenService.GetAll<IPDCMessaging>()
                .Where(s => s.Status == EntityStatus.Active && s.ToEmpId == employeeId &&
                            (s.IsDraft == false || s.IsDraft == null))
                            .GroupBy(x => x.ApplicationId, (k, each) => each.FirstOrDefault())
                            .Count();

            var notifications = GenService.GetAll<Notification>()
                .Where(x => x.UserId == UserId && x.NotificationStatusType == NotificationStatusType.New).Count();


            var allApp = GenService.GetAll<Application>()
                .Where(s => s.Status == EntityStatus.Active && employeeIds.Contains((long)s.RMId))
                .OrderByDescending(m => m.Id).Distinct();

            var LoanAppForSubmission = allApp
                .Where(a => a.ApplicationStage == ApplicationStage.Drafted && a.Product.ProductType == ProductType.Loan)
                .Count();
            var LoanAppUnderProcess = allApp
                .Where(a => a.Product.ProductType == ProductType.Loan &&
                            (a.ApplicationStage == ApplicationStage.SentToTL ||
                             a.ApplicationStage == ApplicationStage.SentToBM ||
                             a.ApplicationStage == ApplicationStage.SentToCRM ||
                             a.ApplicationStage == ApplicationStage.UnderProcessAtCRM))
                .Count();
            var LoanSanctionedApplications = allApp
                .Where(a => a.Product.ProductType == ProductType.Loan &&
                            (a.ApplicationStage == ApplicationStage.SentToOperations ||
                             a.ApplicationStage == ApplicationStage.UnderProcessAtOperations ||
                             a.ApplicationStage == ApplicationStage.DCLUnderProcess ||
                             a.ApplicationStage == ApplicationStage.DCLApproved ||
                             a.ApplicationStage == ApplicationStage.POUnderProcess ||
                             a.ApplicationStage == ApplicationStage.ApprovedByOperations ||
                             a.ApplicationStage == ApplicationStage.ReadyForDeisbursement))
                .Count();
            var LoanDisbursedApplications = allApp
                .Where(a => a.Product.ProductType == ProductType.Loan &&
                            (a.ApplicationStage == ApplicationStage.PartialDisbursementComplete ||
                             a.ApplicationStage == ApplicationStage.DisbursementComplete))
                .Count();

            var DepositAppForSubmission = allApp
                .Where(a => a.Product.ProductType == ProductType.Deposit &&
                            (a.ApplicationStage == ApplicationStage.Drafted ||
                             a.ApplicationStage == ApplicationStage.SentToTL ||
                             a.ApplicationStage == ApplicationStage.SentToBM))
                .Count();
            var DepositReceivedApplications = allApp
                .Where(a => a.Product.ProductType == ProductType.Deposit &&
                            (a.ApplicationStage == ApplicationStage.SentToOperations ||
                             a.ApplicationStage == ApplicationStage.UnderProcessAtOperations ||
                             a.ApplicationStage == ApplicationStage.FundReceived ||
                             a.ApplicationStage == ApplicationStage.DCLUnderProcess ||
                             a.ApplicationStage == ApplicationStage.DCLApproved ||
                             a.ApplicationStage == ApplicationStage.AccountOpeningUnderProcess))
                .Count();
            var DepositAccountOpened = allApp
                .Where(a => a.Product.ProductType == ProductType.Deposit &&
                            (a.ApplicationStage == ApplicationStage.AccountOpened ||
                             a.ApplicationStage == ApplicationStage.InstrumentReady ||
                             a.ApplicationStage == ApplicationStage.InsturmentDeliveredtoClient ||
                             a.ApplicationStage == ApplicationStage.InsturmentSentToRM ||
                             a.ApplicationStage == ApplicationStage.InsturmentSentToBranch ||
                             a.ApplicationStage == ApplicationStage.InsturmentKeptinFile ||
                             a.ApplicationStage == ApplicationStage.PendingIssue))
                .Count();

            ////////
            var submittedApplications =
                GenService.GetAll<Application>()
                    .Where(a => a.RMId == employeeId && a.ApplicationStage != ApplicationStage.Drafted);
            var submittedApplicationIds = submittedApplications.Select(s => s.Id).ToList();

            var disbursementMemos = GenService.GetAll<DisbursementMemo>().Where(d => d.ApplicationId != null &&
                                                                                     submittedApplicationIds.Contains(
                                                                                         (long)d.ApplicationId) &&
                                                                                     d.Status == EntityStatus.Active &&
                                                                                     d.IsApproved != null &&
                                                                                     d.IsApproved == true &&
                                                                                     d.IsDisbursed != null &&
                                                                                     d.IsDisbursed == true);
            var fundReceived =
                GenService.GetAll<FundConfirmationDetail>()
                    .Where(f => submittedApplicationIds.Contains((long)f.FundConfirmation.ApplicationId) &&
                                f.FundConfirmation.Status == EntityStatus.Active &&
                                f.Status == EntityStatus.Active &&
                                f.FundConfirmation.FundReceived != null &&
                                f.FundConfirmation.FundReceived == true &&
                                f.CreditDate != null);
            var disbursementMemosForMtd =
                disbursementMemos.Where(
                    d =>
                        d.DisbursementDetails.FirstOrDefault().CreateDate <= MTD.ToDate &&
                        d.DisbursementDetails.FirstOrDefault().CreateDate >= MTD.FromDate);
            var disbursementMemosForLmtd =
                disbursementMemos.Where(
                    d =>
                        d.DisbursementDetails.FirstOrDefault().CreateDate <= LMTD.ToDate &&
                        d.DisbursementDetails.FirstOrDefault().CreateDate >= LMTD.FromDate);
            var disbursementMemosForYtd =
                disbursementMemos.Where(
                    d =>
                        d.DisbursementDetails.FirstOrDefault().CreateDate <= YTD.ToDate &&
                        d.DisbursementDetails.FirstOrDefault().CreateDate >= YTD.FromDate);
            var disbursementMemosForLytd =
                disbursementMemos.Where(
                    d =>
                        d.DisbursementDetails.FirstOrDefault().CreateDate <= LYTD.ToDate &&
                        d.DisbursementDetails.FirstOrDefault().CreateDate >= LYTD.FromDate);

            var fundsReceivedForMtd = fundReceived.Where(f => f.CreditDate <= MTD.ToDate && f.CreditDate >= MTD.FromDate);
            var fundsReceivedForLmtd =
                fundReceived.Where(f => f.CreditDate <= LMTD.ToDate && f.CreditDate >= LMTD.FromDate);
            var fundsReceivedForYtd = fundReceived.Where(f => f.CreditDate <= YTD.ToDate && f.CreditDate >= YTD.FromDate);
            var fundsReceivedForLytd =
                fundReceived.Where(f => f.CreditDate <= LYTD.ToDate && f.CreditDate >= LYTD.FromDate);

            var callsInProgress = GenService.GetAll<Call>().Where(s => s.Status == EntityStatus.Active &&
                                                                       s.CallStatus == CallStatus.Unfinished &&
                                                                       ((s.CallType == CallType.Self &&
                                                                         s.CreatedBy == UserId) ||
                                                                        (s.CallType != CallType.Self &&
                                                                         s.AssignedTo == assigned)));
            //var todaysScheduledLeads = activeLeads.Where(l => l.FollowupTime < today.ToDate).ToList();

            #endregion

            var CallsInProgress = callsInProgress.Count();
            long InboxMessage = 0;
            long Notifications = 0;
            long MTDLoanCount = 0;
            long LMTDLoanCount = 0;
            long YTDLoanCount = 0;
            long LYTDLoanCount = 0;
            long MTDDepositCount = 0;
            long LMTDDepositCount = 0;
            long YTDDepositCount = 0;
            long LYTDDepositCount = 0;
            decimal MTDLoanAmount = 0;
            decimal LMTDLoanAmount = 0;
            decimal YTDLoanAmount = 0;
            decimal LYTDLoanAmount = 0;
            decimal? MTDDepositAmount = 0;
            decimal? LMTDDepositAmount = 0;
            decimal? YTDDepositAmount = 0;
            decimal? LYTDDepositAmount = 0;
            //
            if (disbursementMemosForMtd.Count() > 0)
            {
                MTDLoanAmount = (from memo in disbursementMemosForMtd
                                 select memo).Sum(m => m.CurrentDisbursementAmount);
                MTDLoanCount = disbursementMemosForMtd.Select(d => d.ApplicationId).Distinct().Count();
            }

            if (disbursementMemosForLmtd.Count() > 0)
            {
                LMTDLoanAmount = (from memo in disbursementMemosForLmtd
                                  select memo).Sum(m => m.CurrentDisbursementAmount);
                LMTDLoanCount = disbursementMemosForLmtd.Select(d => d.ApplicationId).Distinct().Count();
            }
            if (disbursementMemosForYtd.Count() > 0)
            {
                YTDLoanAmount = (from memo in disbursementMemosForYtd
                                 select memo).Sum(m => m.CurrentDisbursementAmount);
                YTDLoanCount = disbursementMemosForYtd.Select(d => d.ApplicationId).Distinct().Count();
            }
            if (disbursementMemosForLytd.Count() > 0)
            {
                LYTDLoanAmount = (from memo in disbursementMemosForLytd
                                  select memo).Sum(m => m.CurrentDisbursementAmount);
                LYTDLoanCount = disbursementMemosForLytd.Select(d => d.ApplicationId).Distinct().Count();
            }

            if (fundsReceivedForMtd.Count() > 0)
            {
                MTDDepositAmount = (from funds in fundsReceivedForMtd
                                    select funds).Sum(f => f.Amount);
                MTDDepositCount = fundsReceivedForMtd.Select(d => d.FundConfirmation.ApplicationId).Distinct().Count();
            }
            if (fundsReceivedForLmtd.Count() > 0)
            {
                LMTDDepositAmount = (from funds in fundsReceivedForLmtd
                                     select funds).Sum(f => f.Amount);
                LMTDDepositCount = fundsReceivedForLmtd.Select(d => d.FundConfirmation.ApplicationId).Distinct().Count();
            }
            if (fundsReceivedForYtd.Count() > 0)
            {
                YTDDepositAmount = (from funds in fundsReceivedForYtd
                                    select funds).Sum(f => f.Amount);
                YTDDepositCount = fundsReceivedForYtd.Select(d => d.FundConfirmation.ApplicationId).Distinct().Count();
            }
            if (fundsReceivedForLytd.Count() > 0)
            {
                LYTDDepositAmount = (from funds in fundsReceivedForLytd
                                     select funds).Sum(f => f.Amount);
                LYTDDepositCount = fundsReceivedForLytd.Select(d => d.FundConfirmation.ApplicationId).Distinct().Count();
            }
            if (inboxMessages > 0)
                InboxMessage = inboxMessages;
            if (notifications > 0)
                Notifications = notifications;

            return new
            {
                InboxMessage,
                Notifications,
                CallsInProgress = CallsInProgress,
                ActiveLeads = activeLeads,
                TotalLoanAmountMTD = MTDLoanAmount,
                TotalLoanCountMTD = MTDLoanCount,
                TotalLoanAmountLMTD = LMTDLoanAmount,
                TotalLoanCountLMTD = LMTDLoanCount,
                TotalLoanAmountYTD = YTDLoanAmount,
                TotalLoanCountYTD = YTDLoanCount,
                TotalLoanAmountLYTD = LYTDLoanAmount,
                TotalLoanCountLYTD = LYTDLoanCount,
                TotalDepositAmountMTD = MTDDepositAmount,
                TotalDepositCountMTD = MTDDepositCount,
                TotalDepositAmountLMTD = LMTDDepositAmount,
                TotalDepositCountLMTD = LMTDDepositCount,
                TotalDepositAmountYTD = YTDDepositAmount,
                TotalDepositCountYTD = YTDDepositCount,
                TotalDepositAmountLYTD = LYTDDepositAmount,
                TotalDepositCountLYTD = LYTDDepositCount,
                LoanAppForSubmission,
                LoanAppUnderProcess,
                LoanSanctionedApplications,
                LoanDisbursedApplications,
                DepositAppForSubmission,
                DepositReceivedApplications,
                DepositAccountOpened
            };
        }
        public object GetMessageAndNotificationCount(long UserId)
        {
            long employeeId = _user.GetEmployeeIdByUserId(UserId);
            long offDegSettingId = _employee.GetOfficeDesignationIdOfEmployee(employeeId);
            var inboxMessages = GenService.GetAll<IPDCMessaging>()
                .Where(s => s.Status == EntityStatus.Active && s.ToEmpId == employeeId &&
                            (s.IsDraft == false || s.IsDraft == null))
                            .GroupBy(x => x.ApplicationId, (k, each) => each.FirstOrDefault())
                            .Count();
            var notifications = GenService.GetAll<Notification>()
                .Where(x => x.UserId == UserId && x.NotificationStatusType == NotificationStatusType.New).Count();
            return new {
                InboxMessage = inboxMessages,
                Notifications = notifications
            };
        }

        public object GetDemoGraphicResidenceDashboard(DateTime? fromDate, DateTime? toDate, long? divisionId, long? districtId, long? thanaId, Stages? stage, Criteria? criteria, List<ProductSelection?> products, List<long?> branchIds)// long? branchId
        {
            //long UserId,
            var data = new DashBoardHighlightsDto();

            var allApplications = GenService.GetAll<Application>().Where(a => a.Status == EntityStatus.Active);
            var cifList = GenService.GetAll<ApplicationCIFs>().Where(cif => cif.ApplicantRole == ApplicantRole.Primary && cif.Status == EntityStatus.Active);

            var loanApplicationsIds = allApplications.Where(a => a.Product.ProductType == ProductType.Loan && a.LoanApplication.LoanAmountApplied != null).Select(a => a.Id);

            var callListToday = new List<Call>();

            var leadListToday = new List<SalesLead>();

            var applicationListToDay = new List<ApplicationLog>();

            var approvedApplicationListToDay = new List<ApplicationLog>();

            var disbursementMemosToday = new List<DisbursementMemo>();

            var fundReceivedToday = new List<FundConfirmation>();

            var disbursedMemodetails = new List<DMDetail>();
            disbursedMemodetails = GenService.GetAll<DMDetail>().Where(dm => dm.Status == EntityStatus.Active).ToList();
            var fundConfirmdetails = new List<FundConfirmationDetail>();
            fundConfirmdetails = GenService.GetAll<FundConfirmationDetail>().Where(fc => fc.Status == EntityStatus.Active).ToList();

            #region Call

            IQueryable<Call> allCallsToday = GenService.GetAll<Call>().Where(c =>
                                             c.CreateDate <= toDate && c.CreateDate >= fromDate
                                             && c.Amount != null && c.Status == EntityStatus.Active);

            #endregion Call

            #region Lead

            IQueryable<SalesLead> allLeadsToday = GenService.GetAll<SalesLead>().Where(c =>
                                                  c.CreateDate <= toDate && c.CreateDate >= fromDate &&
                                                  c.Amount != null && c.Status == EntityStatus.Active);

            #endregion Lead

            #region File Submited Apps

            var acceptableLogsToday = GenService.GetAll<ApplicationLog>()
                .Where(l => (l.Application.LoanApplicationId != null || l.Application.DepositApplicationId != null)
                && (l.Application.LoanApplication.LoanAmountApplied != null || l.Application.DepositApplication.TotalDepositAmount != null)
                && l.Status == EntityStatus.Active
                && l.CreateDate <= toDate
                && l.CreateDate >= fromDate
                && (((l.ToStage == ApplicationStage.SentToCRM || l.ToStage == ApplicationStage.UnderProcessAtCRM)
                && l.Application.Product.ProductType == ProductType.Loan)
                || ((l.ToStage == ApplicationStage.SentToOperations || l.ToStage == ApplicationStage.UnderProcessAtOperations)
                && l.Application.Product.ProductType == ProductType.Deposit)));

            var submitedappList = acceptableLogsToday.Select(a => a.ApplicationId).ToList();
            var cifListofsubmiteapp = cifList.Where(cif => submitedappList.Contains(cif.ApplicationId));

            #endregion File Submited Apps

            #region File Approved

            var approvedLoanApplicationLogsToday = GenService.GetAll<ApplicationLog>()
                .Where(l => l.Status == EntityStatus.Active
                            && l.Application.LoanApplicationId != null
                            && l.Application.Product.ProductType == ProductType.Loan
                            && l.ToStage == ApplicationStage.SentToOperations
                            && l.CreateDate <= toDate
                            && l.CreateDate >= fromDate);
            var approvedappList = approvedLoanApplicationLogsToday.Select(a => a.ApplicationId).ToList();
            var cifListofapprovedapp = cifList.Where(cif => approvedappList.Contains(cif.ApplicationId));


            #endregion File Approved

            #region Amount disbursed / received Today


            var disbursementMemos = GenService.GetAll<DMDetail>().Where(d => d.DisbursementMemo.ApplicationId != null
                        && d.DisbursementMemo.Application.LoanApplicationId != null
                        && d.DisbursementMemo.Application.Product.FacilityType != null
                        && (d.DisbursementMemo.Application.Product.FacilityType == ProposalFacilityType.Home_Loan
                            || d.DisbursementMemo.Application.Product.FacilityType == ProposalFacilityType.Auto_Loan
                            || d.DisbursementMemo.Application.Product.FacilityType == ProposalFacilityType.Personal_Loan)
                        && d.CreateDate <= toDate
                        && d.CreateDate >= fromDate
                        && d.DisbursementMemo.IsApproved != null
                        && d.DisbursementMemo.IsApproved == true
                        && d.DisbursementMemo.IsDisbursed != null
                        && d.DisbursementMemo.IsDisbursed == true
                        && d.Status == EntityStatus.Active).ToList();

            var disbursedLoanappList = disbursementMemos.Select(a => a.DisbursementMemo.Proposal.ApplicationId).ToList();
            var cifListofdisbursedLoan = cifList.Where(cif => disbursedLoanappList.Contains(cif.ApplicationId));

            var fundReceived = GenService.GetAll<FundConfirmationDetail>().Where(d => d.FundConfirmation.ApplicationId != null

                        && d.FundConfirmation.Application.DepositApplicationId != null
                        && d.FundConfirmation.Application.Product.DepositType != null
                        && (d.FundConfirmation.Application.Product.DepositType == DepositType.Fixed
                            || d.FundConfirmation.Application.Product.DepositType == DepositType.Recurring)
                        && d.CreateDate <= toDate
                        && d.CreateDate >= fromDate
                        && d.FundConfirmation.FundReceived != null
                        && d.FundConfirmation.FundReceived == true
                        && d.Status == EntityStatus.Active).ToList();

            var fundReceivedppList = fundReceived.Select(a => a.FundConfirmation.ApplicationId).ToList();
            var cifListoffundReceived = cifList.Where(cif => fundReceivedppList.Contains(cif.ApplicationId));
            #endregion Amount disbursed / received Today

            #region with Division

            List<ResidenceBreakdownDto> mainList = new List<ResidenceBreakdownDto>();
            List<ResidenceBreakdownDto> mainListProfession = new List<ResidenceBreakdownDto>();
            List<ResidenceBreakdownDto> mainListGender = new List<ResidenceBreakdownDto>();
            List<ResidenceBreakdownDto> mainListAge = new List<ResidenceBreakdownDto>();

            List<ResidenceBreakdownDto> mainIncomeRange = new List<ResidenceBreakdownDto>();
            List<ResidenceBreakdownDto> mainListMaritalStatus = new List<ResidenceBreakdownDto>();
            //
            var allCallsTemp =
                            allCallsToday.Where(
                                c => c.CustomerAddressId != null);
            var temp1 = allCallsTemp.ToList();
            var allLeadsTemp = allLeadsToday.Where(
                l => l.CustomerAddressId != null &&
                     l.CustomerAddress.ThanaId == thanaId);
            var temp2 = allLeadsTemp.ToList();

            var facilityTypeList = new List<ProposalFacilityType?>();
            var depositTypeList = new List<DepositType?>();
            if (products != null)
            {
                if (products.Contains(ProductSelection.Home_Loan))
                    facilityTypeList.Add(ProposalFacilityType.Home_Loan);
                if (products.Contains(ProductSelection.Auto_Loan))
                    facilityTypeList.Add(ProposalFacilityType.Auto_Loan);
                if (products.Contains(ProductSelection.Personal_Loan))
                    facilityTypeList.Add(ProposalFacilityType.Personal_Loan);
                if (products.Contains(ProductSelection.Fixed))
                    depositTypeList.Add(DepositType.Fixed);
                if (products.Contains(ProductSelection.Recurring))
                    depositTypeList.Add(DepositType.Recurring);
            }


            allCallsTemp = allCallsTemp.Where(c => c.ProductId != null && (facilityTypeList.Contains(c.Product.FacilityType) || depositTypeList.Contains(c.Product.DepositType)));
            allLeadsTemp = allLeadsTemp.Where(c => c.ProductId != null && (facilityTypeList.Contains(c.Product.FacilityType) || depositTypeList.Contains(c.Product.DepositType)));
            cifListofsubmiteapp = cifListofsubmiteapp.Where(c => facilityTypeList.Contains(c.Application.Product.FacilityType) || depositTypeList.Contains(c.Application.Product.DepositType));
            cifListofapprovedapp = cifListofapprovedapp.Where(c => facilityTypeList.Contains(c.Application.Product.FacilityType));
            cifListofdisbursedLoan = cifListofdisbursedLoan.Where(c => facilityTypeList.Contains(c.Application.Product.FacilityType));
            cifListoffundReceived = cifListoffundReceived.Where(c => depositTypeList.Contains(c.Application.Product.DepositType));

            #region With Branch
            if (branchIds != null && branchIds.Count > 0)
            {
                List<EmployeeDto> subordinateEmpList = new List<EmployeeDto>();
                foreach (var branchId in branchIds)
                {
                    if (branchId > 0)
                        subordinateEmpList.AddRange(_office.GetEmployeesByOfficeId((long)branchId));
                }

                var subordinateUserList = new List<long>();
                foreach (var sub in subordinateEmpList)
                {
                    var user = _user.GetUserByEmployeeId((long)sub.Id);
                    if (user != null && user.Id != null)
                        subordinateUserList.Add((long)user.Id);
                }
                allCallsTemp = allCallsTemp.Where(c => subordinateUserList.Contains((long)c.CreatedBy));
                allLeadsTemp = allLeadsTemp.Where(l => subordinateUserList.Contains((long)l.CreatedBy));
                cifListofsubmiteapp = cifListofsubmiteapp.Where(c => c.Application.BranchId != null && branchIds.Contains(c.Application.BranchId));
                cifListofapprovedapp = cifListofapprovedapp.Where(c => c.Application.BranchId != null && branchIds.Contains(c.Application.BranchId));
                cifListofdisbursedLoan = cifListofdisbursedLoan.Where(c => c.Application.BranchId != null && branchIds.Contains(c.Application.BranchId));
                cifListoffundReceived = cifListoffundReceived.Where(c => c.Application.BranchId != null && branchIds.Contains(c.Application.BranchId));
            }
            #endregion


            if (stage == Stages.Call)
            {
                mainList = allCallsTemp.ToList().Select(d => new ResidenceBreakdownDto
                {
                    DivisionId = d.CustomerAddress.DivisionId,
                    DivisionName = d.CustomerAddress.DivisionId != null ? d.CustomerAddress.Division.DivisionNameEng : "",
                    DistrictId = d.CustomerAddress.DistrictId,
                    DistrictName = d.CustomerAddress.DistrictId != null ? d.CustomerAddress.District.DistrictNameEng : "",
                    ThanaId = d.CustomerAddress.ThanaId,
                    ThanaName = d.CustomerAddress.ThanaId != null ? d.CustomerAddress.Thana.ThanaNameEng : "",
                    ProfessionId = d.ProfessionId,
                    ProfessionName = d.ProfessionId != null ? d.Profession.Name : "",
                    Gender = d.Gender,
                    GenderName = d.Gender != null && d.Gender > 0 ? d.Gender.ToString() : "",
                    IncomeRange = d.IncomeRange,
                    IncomeRangeName = d.IncomeRange != null && d.IncomeRange > 0 ? UiUtil.GetDisplayName(d.IncomeRange) : "",
                    AgeRange = d.AgeRange,
                    AgeRangeName = d.AgeRange != null && d.AgeRange > 0 ? UiUtil.GetDisplayName(d.AgeRange) : "",
                    MaritalStatus = d.MaritalStatus,
                    MaritalStatusName = d.MaritalStatus != null && d.MaritalStatus > 0 ? UiUtil.GetDisplayName(d.MaritalStatus) : "",
                    Count = 0,
                    Amount = d.Amount
                }).ToList();
            }
            else if (stage == Stages.Lead)
            {
                mainList = allLeadsTemp.ToList()
                        .Select(d => new ResidenceBreakdownDto
                        {
                            DivisionId = d.CustomerAddress.DivisionId,
                            DivisionName = d.CustomerAddress.DivisionId != null ? d.CustomerAddress.Division.DivisionNameEng : "",
                            DistrictId = d.CustomerAddress.DistrictId,
                            DistrictName = d.CustomerAddress.DistrictId != null ? d.CustomerAddress.District.DistrictNameEng : "",
                            ThanaId = d.CustomerAddress.ThanaId,
                            ThanaName = d.CustomerAddress.ThanaId != null ? d.CustomerAddress.Thana.ThanaNameEng : "",
                            ProfessionId = d.ProfessionId,
                            ProfessionName = d.Profession.Name,
                            Gender = d.Gender,
                            GenderName = d.Gender != null && d.Gender > 0 ? d.Gender.ToString() : "",
                            IncomeRange = d.IncomeRange,
                            IncomeRangeName = d.IncomeRange != null && d.IncomeRange > 0 ? UiUtil.GetDisplayName(d.IncomeRange) : "",
                            AgeRange = d.AgeRange,
                            AgeRangeName = d.AgeRange != null && d.AgeRange > 0 ? UiUtil.GetDisplayName(d.AgeRange) : "",
                            MaritalStatus = d.MaritalStatus,
                            MaritalStatusName = d.MaritalStatus != null && d.MaritalStatus > 0 ? UiUtil.GetDisplayName(d.MaritalStatus) : "",
                            Count = 0,
                            Amount = d.Amount
                        }).ToList();
            }
            else if (stage == Stages.Files_Submitted)
            {
                mainList = cifListofsubmiteapp.ToList()//.Where(d=>d.CIF_Personal.DateOfBirth != null)
                        .Select(d => new ResidenceBreakdownDto
                        {
                            DivisionId = d.CIF_Personal.ResidenceAddress.DivisionId,
                            DivisionName = d.CIF_Personal.ResidenceAddress.DivisionId != null ? d.CIF_Personal.ResidenceAddress.Division.DivisionNameEng : "",
                            DistrictId = d.CIF_Personal.ResidenceAddress.DistrictId,
                            DistrictName = d.CIF_Personal.ResidenceAddress.DistrictId != null ? d.CIF_Personal.ResidenceAddress.District.DistrictNameEng : "",
                            ThanaId = d.CIF_Personal.ResidenceAddress.ThanaId,
                            ThanaName = d.CIF_Personal.ResidenceAddress.ThanaId != null ? d.CIF_Personal.ResidenceAddress.Thana.ThanaNameEng : "",
                            ProfessionId = d.CIF_Personal.OccupationId != null ? d.CIF_Personal.Occupation.ProfessionId : 0,
                            ProfessionName = d.CIF_Personal.OccupationId != null && d.CIF_Personal.Occupation.ProfessionId != null ? d.CIF_Personal.Occupation.Profession.Name : "",
                            Gender = d.CIF_Personal.Gender,
                            GenderName = d.CIF_Personal.Gender.ToString(),
                            AgeRange = UiUtil.GetAgeRange(d.CIF_Personal.DateOfBirth),
                            AgeRangeName = UiUtil.GetDisplayName(UiUtil.GetAgeRange(d.CIF_Personal.DateOfBirth)),
                            IncomeRange = d.CIF_Personal.IncomeStatements.Where(i => i.Status == EntityStatus.Active).Any() ? UiUtil.GetIncomeRange(d.CIF_Personal.IncomeStatements.Where(i => i.Status == EntityStatus.Active).OrderByDescending(i => i.Id).FirstOrDefault().MonthlyIncomeTotalDeclared) : IncomeRange.NotSpecified,
                            IncomeRangeName = d.CIF_Personal.IncomeStatements.Where(i => i.Status == EntityStatus.Active).Any() ? UiUtil.GetDisplayName(UiUtil.GetIncomeRange(d.CIF_Personal.IncomeStatements.Where(i => i.Status == EntityStatus.Active).OrderByDescending(i => i.Id).FirstOrDefault().MonthlyIncomeTotalDeclared)) : UiUtil.GetDisplayName(IncomeRange.NotSpecified),
                            MaritalStatus = d.CIF_Personal.MaritalStatus,
                            MaritalStatusName = UiUtil.GetDisplayName(d.CIF_Personal.MaritalStatus),
                            Count = 0,
                            Amount = d.Application.DepositApplicationId != null ?
                                    d.Application.DepositApplication.TotalDepositAmount : d.Application.LoanApplicationId != null ?
                                    d.Application.LoanApplication.LoanAmountApplied : 0
                        }).ToList();
            }
            else if (stage == Stages.Files_Approved)
            {
                mainList = cifListofapprovedapp.ToList()
                        .Select(d => new ResidenceBreakdownDto
                        {
                            DivisionId = d.CIF_Personal.ResidenceAddress.DivisionId,
                            DivisionName = d.CIF_Personal.ResidenceAddress.DivisionId != null ? d.CIF_Personal.ResidenceAddress.Division.DivisionNameEng : "",
                            DistrictId = d.CIF_Personal.ResidenceAddress.DistrictId,
                            DistrictName = d.CIF_Personal.ResidenceAddress.DistrictId != null ? d.CIF_Personal.ResidenceAddress.District.DistrictNameEng : "",
                            ThanaId = d.CIF_Personal.ResidenceAddress.ThanaId,
                            ThanaName = d.CIF_Personal.ResidenceAddress.ThanaId != null ? d.CIF_Personal.ResidenceAddress.Thana.ThanaNameEng : "",
                            ProfessionId = d.CIF_Personal.Occupation.ProfessionId,
                            ProfessionName = d.CIF_Personal.Occupation.Profession.Name,
                            Gender = d.CIF_Personal.Gender,
                            GenderName = d.CIF_Personal.Gender.ToString(),
                            AgeRange = UiUtil.GetAgeRange(d.CIF_Personal.DateOfBirth),
                            AgeRangeName = UiUtil.GetDisplayName(UiUtil.GetAgeRange(d.CIF_Personal.DateOfBirth)),
                            IncomeRange = d.CIF_Personal.IncomeStatements.Where(i => i.Status == EntityStatus.Active).Any() ? UiUtil.GetIncomeRange(d.CIF_Personal.IncomeStatements.Where(i => i.Status == EntityStatus.Active).OrderByDescending(i => i.Id).FirstOrDefault().MonthlyIncomeTotalDeclared) : IncomeRange.NotSpecified,
                            IncomeRangeName = d.CIF_Personal.IncomeStatements.Where(i => i.Status == EntityStatus.Active).Any() ? UiUtil.GetDisplayName(UiUtil.GetIncomeRange(d.CIF_Personal.IncomeStatements.Where(i => i.Status == EntityStatus.Active).OrderByDescending(i => i.Id).FirstOrDefault().MonthlyIncomeTotalDeclared)) : UiUtil.GetDisplayName(IncomeRange.NotSpecified),
                            MaritalStatus = d.CIF_Personal.MaritalStatus,
                            MaritalStatusName = UiUtil.GetDisplayName(d.CIF_Personal.MaritalStatus),
                            Count = 0,
                            Amount = d.Application.DepositApplicationId != null ?
                                    d.Application.DepositApplication.TotalDepositAmount : d.Application.LoanApplicationId != null ?
                                    d.Application.LoanApplication.LoanAmountApplied : 0
                        }).ToList();
            }

            else if (stage == Stages.Files_Disbursed)
            {
                if (products.Contains(ProductSelection.Home_Loan) || products.Contains(ProductSelection.Auto_Loan) || products.Contains(ProductSelection.Personal_Loan))
                {
                    mainList = cifListofdisbursedLoan.ToList()
                            .Select(d => new ResidenceBreakdownDto
                            {
                                DivisionId = d.CIF_Personal.ResidenceAddress.DivisionId,
                                DivisionName = d.CIF_Personal.ResidenceAddress.DivisionId != null ? d.CIF_Personal.ResidenceAddress.Division.DivisionNameEng : "",
                                DistrictId = d.CIF_Personal.ResidenceAddress.DistrictId,
                                DistrictName = d.CIF_Personal.ResidenceAddress.DistrictId != null ? d.CIF_Personal.ResidenceAddress.District.DistrictNameEng : "",
                                ThanaId = d.CIF_Personal.ResidenceAddress.ThanaId,
                                ThanaName = d.CIF_Personal.ResidenceAddress.ThanaId != null ? d.CIF_Personal.ResidenceAddress.Thana.ThanaNameEng : "",
                                ProfessionId = d.CIF_Personal.Occupation.ProfessionId,
                                ProfessionName = d.CIF_Personal.Occupation.Profession.Name,
                                Gender = d.CIF_Personal.Gender,
                                GenderName = d.CIF_Personal.Gender.ToString(),
                                AgeRange = UiUtil.GetAgeRange(d.CIF_Personal.DateOfBirth),
                                AgeRangeName = UiUtil.GetDisplayName(UiUtil.GetAgeRange(d.CIF_Personal.DateOfBirth)),
                                IncomeRange = d.CIF_Personal.IncomeStatements.Where(i => i.Status == EntityStatus.Active).Any() ? UiUtil.GetIncomeRange(d.CIF_Personal.IncomeStatements.Where(i => i.Status == EntityStatus.Active).OrderByDescending(i => i.Id).FirstOrDefault().MonthlyIncomeTotalDeclared) : IncomeRange.NotSpecified,
                                IncomeRangeName = d.CIF_Personal.IncomeStatements.Where(i => i.Status == EntityStatus.Active).Any() ? UiUtil.GetDisplayName(UiUtil.GetIncomeRange(d.CIF_Personal.IncomeStatements.Where(i => i.Status == EntityStatus.Active).OrderByDescending(i => i.Id).FirstOrDefault().MonthlyIncomeTotalDeclared)) : UiUtil.GetDisplayName(IncomeRange.NotSpecified),
                                MaritalStatus = d.CIF_Personal.MaritalStatus,
                                MaritalStatusName = UiUtil.GetDisplayName(d.CIF_Personal.MaritalStatus),
                                Count = 0,
                                Amount = d.Application.LoanApplicationId != null ? d.Application.LoanApplication.LoanAmountApplied : 0
                            }).ToList();
                }
                else if (products.Contains(ProductSelection.Fixed) || products.Contains(ProductSelection.Recurring))
                {
                    mainList = cifListoffundReceived.ToList()
                        .Select(d => new ResidenceBreakdownDto
                        {
                            DivisionId = d.CIF_Personal.ResidenceAddress.DivisionId,
                            DivisionName = d.CIF_Personal.ResidenceAddress.DivisionId != null ? d.CIF_Personal.ResidenceAddress.Division.DivisionNameEng : "",
                            DistrictId = d.CIF_Personal.ResidenceAddress.DistrictId,
                            DistrictName = d.CIF_Personal.ResidenceAddress.DistrictId != null ? d.CIF_Personal.ResidenceAddress.District.DistrictNameEng : "",
                            ThanaId = d.CIF_Personal.ResidenceAddress.ThanaId,
                            ThanaName = d.CIF_Personal.ResidenceAddress.ThanaId != null ? d.CIF_Personal.ResidenceAddress.Thana.ThanaNameEng : "",
                            ProfessionId = d.CIF_Personal.Occupation.ProfessionId,
                            ProfessionName = d.CIF_Personal.Occupation.Profession.Name,
                            Gender = d.CIF_Personal.Gender,
                            GenderName = d.CIF_Personal.Gender.ToString(),
                            AgeRange = UiUtil.GetAgeRange(d.CIF_Personal.DateOfBirth),
                            AgeRangeName = UiUtil.GetDisplayName(UiUtil.GetAgeRange(d.CIF_Personal.DateOfBirth)),
                            IncomeRange = d.CIF_Personal.IncomeStatements.Where(i => i.Status == EntityStatus.Active).Any() ? UiUtil.GetIncomeRange(d.CIF_Personal.IncomeStatements.Where(i => i.Status == EntityStatus.Active).OrderByDescending(i => i.Id).FirstOrDefault().MonthlyIncomeTotalDeclared) : IncomeRange.NotSpecified,
                            IncomeRangeName = d.CIF_Personal.IncomeStatements.Where(i => i.Status == EntityStatus.Active).Any() ? UiUtil.GetDisplayName(UiUtil.GetIncomeRange(d.CIF_Personal.IncomeStatements.Where(i => i.Status == EntityStatus.Active).OrderByDescending(i => i.Id).FirstOrDefault().MonthlyIncomeTotalDeclared)) : UiUtil.GetDisplayName(IncomeRange.NotSpecified),
                            MaritalStatus = d.CIF_Personal.MaritalStatus,
                            MaritalStatusName = UiUtil.GetDisplayName(d.CIF_Personal.MaritalStatus),
                            Count = 0,
                            Amount = d.Application.DepositApplicationId != null ? d.Application.DepositApplication.TotalDepositAmount : 0
                        }).ToList();
                }
                else
                {
                    mainList = cifListofdisbursedLoan.ToList()
                            .Select(d => new ResidenceBreakdownDto
                            {
                                DivisionId = d.CIF_Personal.ResidenceAddress.DivisionId,
                                DivisionName = d.CIF_Personal.ResidenceAddress.DivisionId != null ? d.CIF_Personal.ResidenceAddress.Division.DivisionNameEng : "",
                                DistrictId = d.CIF_Personal.ResidenceAddress.DistrictId,
                                DistrictName = d.CIF_Personal.ResidenceAddress.DistrictId != null ? d.CIF_Personal.ResidenceAddress.District.DistrictNameEng : "",
                                ThanaId = d.CIF_Personal.ResidenceAddress.ThanaId,
                                ThanaName = d.CIF_Personal.ResidenceAddress.ThanaId != null ? d.CIF_Personal.ResidenceAddress.Thana.ThanaNameEng : "",
                                ProfessionId = d.CIF_Personal.Occupation.ProfessionId,
                                ProfessionName = d.CIF_Personal.Occupation.Profession.Name,
                                Gender = d.CIF_Personal.Gender,
                                //GenderName = d.CIF_Personal.Gender.ToString(),
                                AgeRange = UiUtil.GetAgeRange(d.CIF_Personal.DateOfBirth),
                                //AgeRangeName = UiUtil.GetDisplayName(UiUtil.GetAgeRange(d.CIF_Personal.DateOfBirth)),
                                IncomeRange = d.CIF_Personal.IncomeStatements.Where(i => i.Status == EntityStatus.Active).Any() ? UiUtil.GetIncomeRange(d.CIF_Personal.IncomeStatements.Where(i => i.Status == EntityStatus.Active).OrderByDescending(i => i.Id).FirstOrDefault().MonthlyIncomeTotalDeclared) : IncomeRange.NotSpecified,
                                IncomeRangeName = d.CIF_Personal.IncomeStatements.Where(i => i.Status == EntityStatus.Active).Any() ? UiUtil.GetDisplayName(UiUtil.GetIncomeRange(d.CIF_Personal.IncomeStatements.Where(i => i.Status == EntityStatus.Active).OrderByDescending(i => i.Id).FirstOrDefault().MonthlyIncomeTotalDeclared)) : UiUtil.GetDisplayName(IncomeRange.NotSpecified),
                                MaritalStatus = d.CIF_Personal.MaritalStatus,
                                MaritalStatusName = UiUtil.GetDisplayName(d.CIF_Personal.MaritalStatus),
                                Count = 0,
                                Amount = d.Application.LoanApplicationId != null ? d.Application.LoanApplication.LoanAmountApplied : 0
                            }).ToList();
                }
            }


            //
            var mainListResidence = new List<ResidenceBreakdownDto>();
            if (thanaId != null && thanaId > 0)
            {
                mainListResidence = mainList.Where(m => m.ThanaId == thanaId).GroupBy(m => m.ThanaId)
                        .Select(d => new ResidenceBreakdownDto
                        {
                            DivisionId = d.FirstOrDefault().DivisionId,
                            DivisionName = d.FirstOrDefault().DivisionName,
                            DistrictId = d.FirstOrDefault().DistrictId,
                            DistrictName = d.FirstOrDefault().DistrictName,
                            ThanaId = d.Key,
                            ThanaName = d.FirstOrDefault().ThanaName,
                            Count = d.Count(),
                            Amount = d.Sum(e => e.Amount)
                        }).ToList();
            }
            else if (districtId != null && districtId > 0)
            {
                mainListResidence = mainList.Where(m => m.DistrictId == districtId).GroupBy(m => m.DistrictId)
                        .Select(d => new ResidenceBreakdownDto
                        {
                            DivisionId = d.FirstOrDefault().DivisionId,
                            DivisionName = d.FirstOrDefault().DivisionName,
                            DistrictId = d.Key,
                            DistrictName = d.FirstOrDefault().DistrictName,
                            ThanaId = d.FirstOrDefault().ThanaId,
                            ThanaName = d.FirstOrDefault().ThanaName,
                            Count = d.Count(),
                            Amount = d.Sum(e => e.Amount)
                        }).ToList();
            }
            else if (divisionId != null && divisionId > 0)
            {
                mainListResidence = mainList.Where(m => m.DivisionId == divisionId).GroupBy(m => m.DivisionId)
                        .Select(d => new ResidenceBreakdownDto
                        {
                            DivisionId = d.Key,
                            DivisionName = d.FirstOrDefault().DivisionName,
                            DistrictId = d.FirstOrDefault().DistrictId,
                            DistrictName = d.FirstOrDefault().DistrictName,
                            ThanaId = d.FirstOrDefault().ThanaId,
                            ThanaName = d.FirstOrDefault().ThanaName,
                            Count = d.Count(),
                            Amount = d.Sum(e => e.Amount)
                        }).ToList();
            }
            else
            {
                mainListResidence = mainList.GroupBy(m => m.DivisionId)
                        .Select(d => new ResidenceBreakdownDto
                        {
                            DivisionId = d.Key,
                            DivisionName = d.FirstOrDefault().DivisionName,
                            DistrictId = d.FirstOrDefault().DistrictId,
                            DistrictName = d.FirstOrDefault().DistrictName,
                            ThanaId = d.FirstOrDefault().ThanaId,
                            ThanaName = d.FirstOrDefault().ThanaName,
                            Count = d.Count(),
                            Amount = d.Sum(e => e.Amount)
                        }).ToList();
            }

            data.ResidenceBreakdownProfession = mainList.GroupBy(m => m.ProfessionId)
                        .Select(d => new ResidenceBreakdownDto
                        {
                            ProfessionId = d.Key,
                            ProfessionName = d.FirstOrDefault().ProfessionName,
                            Count = d.Count(),
                            Amount = d.Sum(e => e.Amount)
                        }).ToList();
            #endregion
            mainList.ForEach(m => m.Gender = (m.Gender != null ? m.Gender : Infrastructure.Gender.Male));
            mainList.ForEach(m => m.AgeRange = (m.AgeRange != null ? m.AgeRange : Infrastructure.AgeRange.NotSpecified));
            mainListGender = mainList.GroupBy(m => new { m.Gender, m.AgeRange })
                        .Select(d => new ResidenceBreakdownDto
                        {
                            Gender = d.Key.Gender,
                            GenderName = d.FirstOrDefault().GenderName,
                            AgeRange = d.Key.AgeRange,
                            AgeRangeName = d.FirstOrDefault().AgeRangeName,
                            Count = d.Count(),
                            Amount = d.Sum(e => e.Amount)
                        })
                        //.OrderBy(m => m.Gender).ThenBy(m => m.AgeRange)
                        .ToList();
            var genderList = new List<EnumDto>();
            genderList.Add(new EnumDto { Id = 1, Name = "Male" });
            genderList.Add(new EnumDto { Id = 2, Name = "Female" });
            var _enum = new EnumFacade();
            var ageList = _enum.GetAgeRange();

            mainListGender = (from gender in genderList
                              from age in ageList
                              join gList in mainListGender on new { GenderId = (int)gender.Id, AgeId = (int)age.Id } equals new { GenderId = (int)gList.Gender.Value, AgeId = (int)gList.AgeRange.Value } into joined
                              from j in joined.DefaultIfEmpty()
                              select new ResidenceBreakdownDto
                              {
                                  Gender = (Finix.IPDC.Infrastructure.Gender)gender.Id,
                                  GenderName = gender.Name,
                                  AgeRange = (AgeRange)age.Id,
                                  AgeRangeName = age.Name,
                                  Count = j != null ? j.Count : 0,
                                  Amount = j != null ? j.Amount : 0
                              })
                              .OrderBy(m => m.Gender).ThenByDescending(m => m.AgeRange)
                              .ToList();

            var mainListIncome = mainList.GroupBy(m => new { m.IncomeRange, m.MaritalStatus })
                        .Select(d => new ResidenceBreakdownDto
                        {
                            IncomeRange = d.Key.IncomeRange,
                            IncomeRangeName = d.FirstOrDefault().IncomeRangeName,
                            MaritalStatus = d.Key.MaritalStatus,
                            MaritalStatusName = d.FirstOrDefault().MaritalStatusName,
                            Count = d.Count(),
                            Amount = d.Sum(e => e.Amount)
                        }).OrderBy(m => m.MaritalStatus).ThenBy(m => m.IncomeRange).ToList();

            var incomeList = _enum.GetIncomeRange();
            var maridList = new List<EnumDto>();
            maridList.Add(new EnumDto { Id = 2, Name = "Unmarried/Others" });
            maridList.Add(new EnumDto { Id = 1, Name = "Married" });

            mainList.ForEach(m => m.MaritalStatus = (m.MaritalStatus == Infrastructure.MaritalStatus.Married ? Infrastructure.MaritalStatus.Married : Infrastructure.MaritalStatus.UnMarried));
            mainListIncome = (from marid in maridList
                              from income in incomeList
                              join gList in mainListIncome.Where(m => m.MaritalStatus != null).ToList() on new { MaridId = (int)marid.Id, IncomeId = (int)income.Id } equals new { MaridId = (int)gList.MaritalStatus, IncomeId = (int)gList.IncomeRange.Value } into joined
                              from j in joined.DefaultIfEmpty()
                              select new ResidenceBreakdownDto
                              {
                                  MaritalStatus = (Infrastructure.MaritalStatus)marid.Id,
                                  MaritalStatusName = marid.Name,
                                  IncomeRange = (IncomeRange)income.Id,
                                  IncomeRangeName = income.Name,
                                  Count = j != null ? j.Count : 0,
                                  Amount = j != null ? j.Amount : 0
                              })
                              .OrderBy(m => m.MaritalStatus).ThenBy(m => m.IncomeRange)
                              .ToList();


            if (mainListResidence.Any())
                data.ResidenceBreakdownCall = mainListResidence;

            if (mainListProfession.Any())
                data.ResidenceBreakdownProfession = mainListProfession;

            if (mainListGender.Any())
                data.ResidenceBreakdownGender = mainListGender;

            if (mainListIncome != null && mainListIncome.Any())
                data.ResidenceBreakdownIncome = mainListIncome;

            return data;
        }
    }
}