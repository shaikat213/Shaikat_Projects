using AutoMapper;
using Finix.Auth.Facade;
using Finix.IPDC.DTO;
using Finix.IPDC.Infrastructure;
using Finix.IPDC.Infrastructure.Models;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using Finix.IPDC.Util;
using Microsoft.Practices.ObjectBuilder2;

namespace Finix.IPDC.Facade
{
    public class OperationsFacade : BaseFacade
    {
        private SequencerFacade _sequencer = new SequencerFacade();
        private readonly UserFacade _user = new UserFacade();
        private readonly EmployeeFacade _employee = new EmployeeFacade();
        private readonly CRMFacade _crmFacade = new CRMFacade();
        ProductFacade _productFacade = new ProductFacade();


        public ResponseDto SaveApplication(ApplicationDto dto, long userId)
        {
            var entity = new Application();
            Address address;
            ResponseDto response = new ResponseDto();
            long employeeId = _user.GetEmployeeIdByUserId((long)userId);
            if (dto != null && dto.Id > 0)
            {
                if (dto.Id != null)
                    entity = GenService.GetById<Application>((long)dto.Id);

                try
                {
                    entity.HardCopyReceived = dto.HardCopyReceived;
                    entity.HardCopyReceiveDate = dto.HardCopyReceiveDate;
                    GenService.Save(entity);
                    response.Id = entity.Id;
                }
                catch (Exception ex)
                {
                    response.Id = entity.Id;
                    response.Message = "Application Edit Operation Failed";
                    return response;
                }

                response.Success = true;
                response.Message = "Application Hard Copy Received Successfully";

            }
            return response;
        }
        public ResponseDto SaveApprovedCreditMemoCurrentHoldings(ApplicationDto dto, long userId)
        {
            var entity = new Application();
            ResponseDto response = new ResponseDto();
            long employeeId = _user.GetEmployeeIdByUserId((long)userId);

            if (dto != null && dto.Id > 0)
            {
                if (dto.Id != null)
                {
                    entity = GenService.GetById<Application>((long)dto.Id);
                }
                try
                {
                    var proposal = _crmFacade.GetProposalObjByAppId((long)dto.Id);
                    if ((entity.CurrentHolding != null || entity.CurrentHolding > 0) && proposal.IsApproved == true)
                    {
                        entity.CurrentHolding = null;
                        entity.CurrentHoldingEmpId = null;
                    }
                    GenService.Save(entity);
                    response.Id = entity.Id;
                }
                catch (Exception ex)
                {
                    response.Id = entity.Id;
                    response.Message = "Operation Failed";
                    return response;
                }
                response.Success = true;
                response.Message = "Application Current Holding Assigned";
            }
            return response;
        }
        public ResponseDto SaveFundConfirm(FundConfirmationDto dto, long userId)
        {
            FundConfirmation entity = new FundConfirmation();
            ResponseDto responce = new ResponseDto();

            if (dto.Id > 0)
            {
                if (dto.Id != null)
                    entity = GenService.GetById<FundConfirmation>((long)dto.Id);
                var application = GenService.GetById<Application>((long)entity.ApplicationId);
                dto.CreateDate = entity.CreateDate;
                dto.CreatedBy = entity.CreatedBy;
                if (dto.Status == null)
                    dto.Status = entity.Status;

                //entity.Status = EntityStatus.Active;
                //entity.CreatedBy = userId;
                //entity.CreateDate = DateTime.Now;
                var log = new ApplicationLog();
                if (entity.FundReceived == true)
                {

                    log.Activity = Activity.Submit;
                    log.AppIdRef = entity.Application.Id;
                    log.ApplicationId = entity.Application.Id;
                    log.AppType = AppType.Application;
                    log.CreateDate = DateTime.Now;
                    log.CreatedBy = userId;
                    log.FromUserId = userId;
                    log.FromStage = entity.Application.ApplicationStage;
                    log.ToStage = ApplicationStage.FundReceived;
                    log.Status = EntityStatus.Active;
                    if (application != null)
                    {
                        application.ApplicationStage = ApplicationStage.FundReceived;
                    }
                    GenService.Save(log);
                    if (application != null)
                    {
                        GenService.Save(application);
                    }
                }


                using (var tran = new TransactionScope())
                {
                    try
                    {
                        Mapper.Map(dto, entity);
                        entity.EditDate = DateTime.Now;
                        GenService.Save(entity);
                        #region list updates

                        if (dto.Fundings != null)
                        {
                            foreach (var item in dto.Fundings)
                            {
                                if (item.Amount > 0)
                                {
                                    FundConfirmationDetail fundings;
                                    if (item.Id != null && item.Id > 0)
                                    {
                                        fundings = GenService.GetById<FundConfirmationDetail>((long)item.Id);
                                        if (item.Status == null)
                                            item.Status = fundings.Status;
                                        item.CreateDate = fundings.CreateDate;
                                        item.CreatedBy = fundings.CreatedBy;
                                        item.FundConfirmationId = fundings.FundConfirmationId;
                                        item.CreateDate = fundings.CreditDate;
                                        item.EditDate = DateTime.Now;
                                        item.EditedBy = userId;
                                        Mapper.Map(item, fundings);
                                        GenService.Save(fundings);
                                    }
                                    else
                                    {
                                        item.CreateDate = DateTime.Now;
                                        item.CreatedBy = userId;
                                        item.Status = EntityStatus.Active;
                                        item.FundConfirmationId = entity.Id;
                                        fundings = Mapper.Map<FundConfirmationDetail>(item);
                                        GenService.Save(fundings);
                                    }
                                }
                            }
                        }

                        #endregion

                        #region list deletes
                        if (dto.RemovedFundings != null)
                        {
                            foreach (var item in dto.RemovedFundings)
                            {
                                var fund = GenService.GetById<FundConfirmationDetail>(item);
                                if (fund != null)
                                {
                                    fund.Status = EntityStatus.Inactive;
                                    fund.EditDate = DateTime.Now;
                                    fund.EditedBy = userId;
                                }
                                GenService.Save(fund);
                            }
                        }
                        #endregion
                        tran.Complete();
                    }
                    catch (Exception ex)
                    {
                        tran.Dispose();
                        responce.Message = "Fund Confirmation Failed";
                        return responce;
                    }
                }
                responce.Id = entity.Id;
                responce.Success = true;
                responce.Message = "Fund Confirmed";
                return responce;
            }
            else
            {
                if (dto.ApplicationId != null)
                {
                    entity = Mapper.Map<FundConfirmation>(dto);
                    var application = GenService.GetById<Application>((long)entity.ApplicationId);
                    entity.Status = EntityStatus.Active;
                    entity.CreatedBy = userId;
                    entity.CreateDate = DateTime.Now;
                    var log = new ApplicationLog();
                    if (entity.FundReceived == true)
                    {

                        log.Activity = Activity.Submit;
                        log.AppIdRef = entity.ApplicationId;
                        log.ApplicationId = (long)entity.ApplicationId;
                        log.AppType = AppType.Application;
                        log.CreateDate = DateTime.Now;
                        log.CreatedBy = userId;
                        log.FromUserId = userId;
                        log.FromStage = application.ApplicationStage;
                        log.ToStage = ApplicationStage.FundReceived;
                        log.Status = EntityStatus.Active;
                        if (application != null)
                        {
                            application.ApplicationStage = ApplicationStage.FundReceived;
                        }
                        GenService.Save(log);
                        if (application != null)
                        {
                            GenService.Save(application);
                        }
                    }

                    using (var tran = new TransactionScope())
                    {
                        try
                        {
                            #region populate list

                            if (dto.Fundings != null)
                                dto.Fundings = dto.Fundings.Where(c => c.Amount > 0).ToList();
                            if (dto.Fundings != null && dto.Fundings.Count > 0)
                            {
                                entity.Fundings = Mapper.Map<List<FundConfirmationDetail>>(dto.Fundings);
                                foreach (var item in entity.Fundings)
                                {
                                    //item.FundConfirmationId = dto.Id;
                                    item.CreateDate = DateTime.Now;
                                    item.CreatedBy = userId;
                                    item.Status = EntityStatus.Active;
                                }
                            }
                            #endregion
                            tran.Complete();
                            //GenService.Save(entity);
                        }
                        catch (Exception ex)
                        {
                            tran.Dispose();
                            responce.Message = "Fund Confirmation Failed";
                            return responce;
                        }
                    }
                    GenService.Save(entity);
                    //GenService.SaveChanges();
                }
                responce.Id = entity.Id;
                responce.Success = true;
                responce.Message = "Fund Confirmed";
                return responce;
            }
        }
        public FundConfirmationDto LoadFundConfirmationByAppId(long AppId, long Id)
        {
            if (AppId > 0 && Id > 0)
            {
                //if (Id > 0)
                //{
                var app = GenService.GetById<FundConfirmation>(Id);
                //var application = GenService.GetById<Application>(AppId);
                if (app != null)
                {
                    var data = Mapper.Map<FundConfirmationDto>(app);

                    var application = GenService.GetById<Application>(AppId);
                    data.ApplicationNo = application.ApplicationNo;
                    data.AccountTitle = application.AccountTitle;
                    data.CustomerTypeName = application.CustomerType.ToString();
                    data.ProductName = application.Product.Name;
                    data.TotalDepositAmount = application.DepositApplication.TotalDepositAmount;
                    return data;
                }
                //}
            }
            else
            {
                if (AppId > 0)
                {
                    //var app = GenService.GetAll<FundConfirmation>().Where(r => r.Status == EntityStatus.Active);
                    var app = new FundConfirmation();

                    var data = Mapper.Map<FundConfirmationDto>(app);

                    var application = GenService.GetById<Application>(AppId);
                    data.ApplicationId = AppId;
                    data.ApplicationNo = application.ApplicationNo;
                    data.AccountTitle = application.AccountTitle;
                    data.CustomerTypeName = application.CustomerType.ToString();
                    data.ProductName = application.Product.Name;
                    data.TotalDepositAmount = application.DepositApplication.TotalDepositAmount;
                    return data;
                }
            }
            return null;
        }
        //public ApplicationDto GetCommonAppData(long AppId)
        //{
        //    if (AppId > 0)
        //    {
        //        var app = GenService.GetById<Application>(AppId);//<Application>().Where(r => r.Status == EntityStatus.Active);
        //        var data = Mapper.Map<ApplicationDto>(app);

        //        //var application = GenService.GetById<Application>(AppId);
        //        data.ApplicationNo = app.ApplicationNo;
        //        data.AccountTitle = app.AccountTitle;
        //        data.CustomerTypeName = app.CustomerType.ToString();
        //        data.ProductId = app.ProductId;
        //        data.ProductName = app.Product.Name;
        //        return data;
        //    }
        //    return null;
        //}
        public DocumentCheckListDto LoadDocumentCheckList(long? AppId, long? proposalId, long? id)
        {

            DocumentCheckListDto aCheckListDto = new DocumentCheckListDto();
            if (proposalId > 0 || id > 0)
            {
                if (id != null && id > 0)
                {
                    var data = GenService.GetById<DocumentCheckList>((long)id);
                    var result = Mapper.Map<DocumentCheckListDto>(data);
                    result.Documents.RemoveAll(f => f.Status != EntityStatus.Active);
                    result.Exceptions.RemoveAll(o => o.Status != EntityStatus.Active);
                    result.Securities.RemoveAll(o => o.Status != EntityStatus.Active);
                    result.Signatories.RemoveAll(o => o.Status != EntityStatus.Active);
                    if (result.Term <= 0 && AppId != null)
                    {
                        var application = GenService.GetById<Application>((long)AppId);
                        if (application != null)
                        {
                            result.Term = application.Term;
                        }

                    }
                    if (data.Product != null)
                    {
                        if (data.Product.ProductType != null)
                        {
                            result.ProductTypeId = data.Product.ProductType;
                        }
                    }

                    return result;
                }
                else if (proposalId > 0 && proposalId != null)
                {
                    var data = GenService.GetById<Proposal>((long)proposalId);
                    aCheckListDto.ProposalId = data != null ? data.Id : 0;
                    aCheckListDto.ApplicationId = data != null ? data.ApplicationId : 0;
                    aCheckListDto.ProductId = data != null ? data.Application != null ? (long?)data.Application.ProductId : null : 0;
                    aCheckListDto.FacilityType = data != null ? data.FacilityType : 0;
                    aCheckListDto.FacilityTypeName = data.FacilityType > 0 ? UiUtil.GetDisplayName(data.FacilityType) : "";//data != null ? data.FacilityType.ToString() : "";
                    aCheckListDto.ApplicationNo = data != null ? data.ApplicationNo : "";
                    aCheckListDto.ApplicationTitle = data != null ? data.Application != null ? data.Application.AccountTitle : null : "";
                    aCheckListDto.Term = data != null ? data.Application != null ? (int?)data.Application.LoanApplication.Term : null : 0;
                    aCheckListDto.ProductTypeId = data != null ? data.Application != null ? (ProductType?)data.Application.ProductType : null : 0;
                    var isIndividual = (data.Application.CustomerType == ApplicationCustomerType.Individual);
                    var temp = _productFacade.GetAllDocCheckList((long)aCheckListDto.ProductId, isIndividual, (isIndividual ? data.Application.CIFList.FirstOrDefault().CIF_OrganizationalId : null));
                    if (temp != null && temp.Count > 0)
                    {
                        var doclist = temp.Select(x => new DocumentCheckListDetailDto
                        {
                            DocumentSetupId = x.Id,
                            Name = x.DocName
                        }).ToList();
                        aCheckListDto.Documents = doclist;
                    }
                    var security = data.SecurityDetails;
                    if (security != null && security.Count > 0)
                    {
                        var secrlist = security.Select(x => new DocumentSecuritiesDto
                        {
                            SecurityDescription = x.Details
                        }).ToList();
                        aCheckListDto.Securities = secrlist;
                    }

                    return aCheckListDto;
                }
            }
            else
            {
                if (AppId > 0)
                {
                    //var app = GenService.GetAll<FundConfirmation>().Where(r => r.Status == EntityStatus.Active);
                    var app = new DocumentCheckList();

                    var data = Mapper.Map<DocumentCheckListDto>(app);

                    var application = GenService.GetById<Application>((long)AppId);
                    var isIndividual = (application.CustomerType == ApplicationCustomerType.Individual);
                    data.ApplicationId = (long)AppId;
                    data.ApplicationNo = application.ApplicationNo;
                    data.AccountTitle = application.AccountTitle;
                    data.CustomerTypeName = application.CustomerType.ToString();
                    data.ProductName = application.Product.Name;
                    data.Term = data != null ? (int?)data.Term : 0;
                    var temp = _productFacade.GetAllDocCheckList(application.ProductId, isIndividual, (isIndividual ? application.CIFList.FirstOrDefault().CIF_OrganizationalId : null));
                    if (temp != null && temp.Count > 0)
                    {
                        var doclist = temp.Select(x => new DocumentCheckListDetailDto
                        {
                            DocumentSetupId = x.Id,
                            Name = x.DocName
                        }).ToList();
                        data.Documents = doclist;
                    }
                    return data;
                }
            }

            return null;
        }
        public DocumentCheckListDto LoadDocumentCheckListForDeposit(long? AppId, long? id)
        {
            DocumentCheckListDto aCheckListDto = new DocumentCheckListDto();
            if (AppId > 0 && id > 0)
            {
                var data = GenService.GetById<DocumentCheckList>((long)id);
                var result = Mapper.Map<DocumentCheckListDto>(data);
                result.Documents.RemoveAll(f => f.Status != EntityStatus.Active);
                result.Exceptions.RemoveAll(o => o.Status != EntityStatus.Active);
                result.Securities.RemoveAll(o => o.Status != EntityStatus.Active);

                //result.ProductTypeId = data.Product.ProductType;

                result.ApplicationId = (long)AppId;
                result.ApplicationNo = data.Application.ApplicationNo;
                result.AccountTitle = data.Application.AccountTitle;
                result.CustomerTypeName = data.Application.CustomerType.ToString();
                result.ProductId = data.Application.Product.Id;
                result.ProductName = data.Application.Product.Name;
                result.Term = data.Application.DepositApplication.Term;

                //result.ProductName = data.Product.Name;
                return result;
            }
            else if (AppId > 0)
            {

                var app = new DocumentCheckList();

                //var data = Mapper.Map<DocumentCheckListDto>(app);

                var application = GenService.GetById<Application>((long)AppId);
                var isIndividual = (application.CustomerType == ApplicationCustomerType.Individual);
                var temp = _productFacade.GetAllDocCheckList((long)application.ProductId, isIndividual, (isIndividual ? application.CIFList.FirstOrDefault().CIF_OrganizationalId : null));
                if (temp != null && temp.Count > 0)
                {
                    var doclist = temp.Select(x => new DocumentCheckListDetailDto
                    {
                        DocumentSetupId = x.Id,
                        Name = x.DocName
                    }).ToList();
                    aCheckListDto.Documents = doclist;

                }
                //return data;


                aCheckListDto.ApplicationId = (long)AppId;
                aCheckListDto.ApplicationNo = application.ApplicationNo;
                aCheckListDto.AccountTitle = application.AccountTitle;
                aCheckListDto.CustomerTypeName = application.CustomerType.ToString();
                aCheckListDto.ProductName = application.Product.Name;
                aCheckListDto.Term = application.DepositApplication.Term;
                aCheckListDto.ProductId = application.ProductId;
                aCheckListDto.DCLDate = DateTime.Now;
                //aCheckListDto.FacilityTypeName = UiUtil.GetDisplayName(application.Product.FacilityType);
                return aCheckListDto;
            }

            return null;
        }
        public ResponseDto SaveDocumentCheckList(DocumentCheckListDto dto, long userId)
        {
            ResponseDto response = new ResponseDto();
            var log = new ApplicationLog();
            try
            {
                List<DCL_Signatory> signatoryList = new List<DCL_Signatory>();
                if (dto.ProposalId < 0)
                {
                    response.Message = "Proposal don't exist";
                    return response;
                }
                #region Edit
                if (dto.Id > 0)
                {
                    var prev = GenService.GetById<DocumentCheckList>((long)dto.Id);
                    if (dto.Documents != null)
                    {
                        foreach (var item in dto.Documents)
                        {
                            DocumentCheckListDetail docDetail;
                            if (item.Id != null && item.Id > 0)
                            {
                                docDetail = GenService.GetById<DocumentCheckListDetail>((long)item.Id);
                                if (item.Status == null)
                                    item.Status = docDetail.Status;
                                item.CreateDate = docDetail.CreateDate;
                                item.CreatedBy = docDetail.CreatedBy;
                                item.DCLId = docDetail.DCLId;
                                item.DocumentSetupId = docDetail.DocumentSetupId;
                                item.EditDate = DateTime.Now;
                                item.EditedBy = userId;

                                Mapper.Map(item, docDetail);
                                GenService.Save(docDetail);
                            }
                            else
                            {
                                docDetail = new DocumentCheckListDetail();
                                item.CreateDate = DateTime.Now;
                                item.CreatedBy = userId;
                                item.Status = EntityStatus.Active;
                                item.DCLId = prev.Id;
                                item.DocumentSetupId = item.DocumentSetupId == 0 ? null : item.DocumentSetupId;
                                docDetail = Mapper.Map<DocumentCheckListDetail>(item);
                                GenService.Save(docDetail);
                            }

                        }
                    }
                    if (dto.RemovedDocuments != null)
                    {
                        foreach (var item in dto.RemovedDocuments)
                        {
                            var detail = GenService.GetById<DocumentCheckListDetail>(item);
                            if (detail != null)
                            {
                                detail.Status = EntityStatus.Inactive;
                                detail.EditDate = DateTime.Now;
                                detail.EditedBy = userId;
                            }
                            GenService.Save(detail);
                        }
                    }
                    if (dto.Exceptions != null)
                    {
                        foreach (var item in dto.Exceptions)
                        {
                            DocumentCheckListException docDetail;
                            if (item.Id != null && item.Id > 0)
                            {
                                log.Activity = Activity.Submit;
                                log.AppIdRef = item.Id;
                                log.ApplicationId = prev.ApplicationId;
                                log.AppType = AppType.Application;
                                log.CreateDate = DateTime.Now;
                                log.CreatedBy = userId;
                                log.FromUserId = userId;
                                log.FromStage = ApplicationStage.DclException;
                                log.ToStage = ApplicationStage.DclObtained; // Previous 'Edit'
                                //log.ToUserId = parentUserId != null ? (long)parentUserId : 0;
                                log.Status = EntityStatus.Active;
                                docDetail = GenService.GetById<DocumentCheckListException>((long)item.Id);
                                if (item.Status == null)
                                    item.Status = docDetail.Status;
                                item.CreateDate = docDetail.CreateDate;
                                item.CreatedBy = docDetail.CreatedBy;
                                item.DCLId = docDetail.DCLId;
                                item.EditDate = DateTime.Now;
                                item.EditedBy = userId;
                                item.Status = EntityStatus.Active;
                                Mapper.Map(item, docDetail);
                                GenService.Save(docDetail);
                                GenService.Save(log);
                            }
                            else
                            {
                                docDetail = new DocumentCheckListException();
                                item.CreateDate = DateTime.Now;
                                item.CreatedBy = userId;
                                item.Status = EntityStatus.Active;
                                item.DCLId = prev.Id;
                                docDetail = Mapper.Map<DocumentCheckListException>(item);
                                GenService.Save(docDetail);
                            }

                        }
                    }

                    dto.DCLNo = prev.DCLNo;
                    if (dto.RemovedExceptions != null)
                    {
                        foreach (var item in dto.RemovedExceptions)
                        {
                            var detail = GenService.GetById<DocumentCheckListException>(item);
                            if (detail != null)
                            {
                                detail.Status = EntityStatus.Inactive;
                                detail.EditDate = DateTime.Now;
                                detail.EditedBy = userId;
                            }
                            GenService.Save(detail);
                        }
                    }
                    //if (dto.Exceptions == null)
                    //{
                    //    dto.IsApproved = true;
                    //}
                    //else
                    //{
                    //    dto.IsApproved = false;
                    //}
                    dto.IsApproved = false;//this modification was done with instructions from saleheen by shariful on 4th June, 2017
                    if (dto.Securities != null)
                    {
                        foreach (var item in dto.Securities)
                        {
                            DocumentSecurities docSecurity;
                            if (item.Id != null && item.Id > 0)
                            {
                                docSecurity = GenService.GetById<DocumentSecurities>((long)item.Id);
                                if (item.Status == null)
                                    item.Status = docSecurity.Status;
                                item.CreateDate = docSecurity.CreateDate;
                                item.CreatedBy = docSecurity.CreatedBy;
                                item.DCLId = docSecurity.DCLId;

                                item.EditDate = DateTime.Now;
                                item.EditedBy = userId;
                                item.Status = EntityStatus.Active;
                                Mapper.Map(item, docSecurity);
                                GenService.Save(docSecurity);
                            }
                            else
                            {
                                docSecurity = new DocumentSecurities();
                                item.CreateDate = DateTime.Now;
                                item.CreatedBy = userId;
                                item.Status = EntityStatus.Active;
                                item.DCLId = prev.Id;
                                docSecurity = Mapper.Map<DocumentSecurities>(item);
                                GenService.Save(docSecurity);
                            }

                        }
                    }
                    if (dto.RemovedSecurities != null)
                    {
                        foreach (var item in dto.RemovedSecurities)
                        {
                            var detail = GenService.GetById<DocumentSecurities>(item);
                            if (detail != null)
                            {
                                detail.Status = EntityStatus.Inactive;
                                detail.EditDate = DateTime.Now;
                                detail.EditedBy = userId;
                            }
                            GenService.Save(detail);
                        }
                    }
                    #region DCL_Signatory

                    if (dto.Signatories != null)
                    {
                        foreach (var signatorDto in dto.Signatories)
                        {
                            var signatory = new DCL_Signatory();
                            if (signatorDto.Id != null && signatorDto.Id > 0)
                            {
                                signatory = GenService.GetById<DCL_Signatory>((long)signatorDto.Id);
                                signatorDto.CreateDate = signatory.CreateDate;
                                signatorDto.CreatedBy = signatory.CreatedBy;
                                signatorDto.Status = signatory.Status;
                                signatorDto.EditDate = DateTime.Now;
                                signatorDto.DCLId = signatory.DCLId;
                                signatory = Mapper.Map(signatorDto, signatory);
                                signatoryList.Add(signatory);
                            }
                            else
                            {
                                //assesments = new Proposal_OverallAssessment();
                                signatory = Mapper.Map<DCL_Signatory>(signatorDto);
                                signatory.Status = EntityStatus.Active;
                                signatory.CreatedBy = userId;
                                signatory.CreateDate = DateTime.Now;
                                signatory.DCLId = prev.Id;
                                signatoryList.Add(signatory);
                                //GenService.Save(text);
                            }
                        }
                    }
                    #endregion
                    if (dto.RemovedSignatories != null)
                    {
                        foreach (var item in dto.RemovedSignatories)
                        {
                            var signatory = GenService.GetById<DCL_Signatory>(item);
                            if (signatory != null)
                            {
                                signatory.Status = EntityStatus.Inactive;
                                signatory.EditDate = DateTime.Now;
                                signatory.EditedBy = userId;
                            }
                            GenService.Save(signatory);
                        }
                    }
                    dto.CreateDate = prev.CreateDate;
                    dto.CreatedBy = prev.CreatedBy;
                    dto.Status = prev.Status;
                    dto.ProposalId = prev.ProposalId;
                    dto.EditDate = DateTime.Now;
                    prev = Mapper.Map(dto, prev);
                    GenService.Save(prev);
                    GenService.Save(signatoryList);
                    response.Id = prev.Id;
                    response.Success = true;
                    response.Message = "Document Check List Edited Successfully";
                }
                #endregion
                #region Add
                else
                {
                    if (dto.ApplicationId > 0)
                    {
                        var checklists =
                            GenService.GetAll<DocumentCheckList>().Where(p => p.ApplicationId == dto.ApplicationId).ToList();
                        checklists.ForEach(x => x.Status = EntityStatus.Inactive);
                        GenService.Save(checklists);
                    }
                    var data = Mapper.Map<DocumentCheckList>(dto);
                    data.EditDate = DateTime.Now;
                    data.Status = EntityStatus.Active;
                    data.CreateDate = DateTime.Now;
                    if (dto.Documents != null && dto.Documents.Count > 0)
                    {
                        data.Documents = Mapper.Map<List<DocumentCheckListDetail>>(dto.Documents);
                        foreach (var item in data.Documents)
                        {
                            item.DCLId = data.Id;
                            item.CreateDate = DateTime.Now;
                            item.CreatedBy = userId;
                            item.Status = EntityStatus.Active;
                        }
                    }
                    if (dto.Exceptions != null && dto.Exceptions.Count > 0)
                    {
                        data.Exceptions = Mapper.Map<List<DocumentCheckListException>>(dto.Exceptions);
                        foreach (var item in data.Exceptions)
                        {
                            item.DCLId = data.Id;
                            item.CreateDate = DateTime.Now;
                            item.CreatedBy = userId;
                            item.Status = EntityStatus.Active;
                        }
                        //data.IsApproved = false;
                    }
                    //if (dto.Exceptions == null)
                    //{
                    //    data.IsApproved = true;
                    //}
                    data.IsApproved = false; //this modification was done with instructions from saleheen by shariful on 4th June, 2017
                    if (dto.Securities != null && dto.Securities.Count > 0)
                    {
                        data.Securities = Mapper.Map<List<DocumentSecurities>>(dto.Securities);
                        foreach (var item in data.Securities)
                        {
                            item.DCLId = data.Id;
                            item.CreateDate = DateTime.Now;
                            item.CreatedBy = userId;
                            item.Status = EntityStatus.Active;
                        }
                    }

                    if (dto.Signatories != null && dto.Signatories.Count > 0)
                    {
                        data.Signatories = new List<DCL_Signatory>();
                        data.Signatories = Mapper.Map<List<DCL_Signatory>>(dto.Signatories);
                        data.Signatories = data.Signatories.Select(d =>
                        {
                            d.Status = EntityStatus.Active;
                            return d;
                        }).ToList();
                    }
                    data.DCLNo = _sequencer.GetUpdatedDCLNo();
                    //data.OfferLetterNo = _sequencer.GetUpdatedOfferLetterNo();
                    GenService.Save(data);
                    response.Id = data.Id;
                    response.Success = true;
                    response.Message = "Document Check List Saved Successfully";
                }
                #endregion

            }
            catch (Exception ex)
            {
                response.Message = "Error Message : " + ex;
            }
            return response;
        }
        public ResponseDto SaveApprovedDocumentCheckList(DocumentCheckListDto dto, long userId)
        {
            ResponseDto response = new ResponseDto();
            try
            {

                #region Edit
                if (dto.Id != null && dto.Id > 0)
                {

                    var prev = GenService.GetById<DocumentCheckList>((long)dto.Id);
                    var app = GenService.GetById<Application>(prev.ApplicationId);
                    var log = new ApplicationLog();
                    if (dto.IsApproved == true)
                    {

                        log.Activity = Activity.Submit;
                        log.AppIdRef = prev.Application.Id;
                        log.ApplicationId = prev.Application.Id;
                        log.AppType = AppType.Application;
                        log.CreateDate = DateTime.Now;
                        log.CreatedBy = userId;
                        log.FromUserId = userId;
                        log.FromStage = prev.Application.ApplicationStage;
                        if (app != null)
                        {
                            app.ApplicationStage = ApplicationStage.DCLApproved;
                            log.ToStage = ApplicationStage.DCLApproved;
                        }
                        log.Status = EntityStatus.Active;
                        dto.IsApproved = true;
                    }
                    else
                    {
                        dto.IsApproved = false;
                    }

                    dto.CreateDate = prev.CreateDate;
                    dto.CreatedBy = prev.CreatedBy;
                    dto.Status = prev.Status;
                    //dto.ProposalId = prev.ProposalId;
                    dto.EditDate = DateTime.Now;
                    prev = Mapper.Map(dto, prev);
                    GenService.Save(app);
                    GenService.Save(prev);
                    GenService.SaveChanges();
                    response.Id = prev.Id;

                    response.Success = true;
                    response.Message = "Document Approval Status Updated Successfully";
                }
                #endregion
            }
            catch (Exception ex)
            {
                response.Message = "Error Message : " + ex;
            }
            return response;
        }
        public long GetFundConfirmIdByAppId(long AppId)
        {
            long fundConfirmId = 0;
            var fundConfirm = GenService.GetAll<FundConfirmation>().FirstOrDefault(e => e.ApplicationId == AppId && e.Status == EntityStatus.Active);
            if (fundConfirm != null)
            {
                fundConfirmId = (long)fundConfirm.Id;
            }

            if (fundConfirmId > 0)
                return fundConfirmId;
            return 0;
        }

        public long GetTrackingIdByAppId(long AppId)
        {
            long fundConfirmId = 0;
            var fundConfirm = GenService.GetAll<DepositApplicationTracking>().FirstOrDefault(e => e.ApplicationId == AppId && e.Status == EntityStatus.Active);
            if (fundConfirm != null)
            {
                fundConfirmId = (long)fundConfirm.Id;
            }

            if (fundConfirmId > 0)
                return fundConfirmId;
            return 0;
        }


        //public IPagedList<FundConfirmationDto> GetOperationFundReceived(int pageSize, int pageCount, string searchString, long UserId)
        //{
        //    var allApp = GenService.GetAll<FundConfirmation>().Where(s => s.Application.CurrentHolding == UserId && s.FundReceived == true && s.Status == EntityStatus.Active).Select(s => new FundConfirmationDto
        //    {
        //        Id = s.Id,
        //        ApplicationId = s.ApplicationId,
        //        ApplicationNo = s.Application.ApplicationNo,
        //        AccountTitle = s.Application.AccountTitle,
        //        ProductName = s.Application.Product.Name,
        //        ProposalId = s.ProposalId
        //        //FundReceived = s.FundReceived != null && (s.FundReceived.ToString() == true ? "Fund Received" : "")
        //        //MaturityAmount = s.Application.DepositApplication.MaturityAmount

        //    });
        //    if (!string.IsNullOrEmpty(searchString))
        //        allApp = allApp.Where(a => a.ApplicationNo.ToLower().Contains(searchString.ToLower()));
        //    var temp = allApp.OrderBy(r => r.Id).ToPagedList(pageCount, pageSize);
        //    return temp;
        //}

        public IPagedList<ApplicationDto> GetOperationFundReceived(int pageSize, int pageCount, string searchString, long UserId)
        {
            var application = GenService.GetAll<Application>()
                    .Where(s => (s.ApplicationStage == ApplicationStage.FundReceived)
                    && s.CurrentHolding == UserId && (s.DepositApplicationId != null && s.DepositApplicationId > 0) && s.Status == EntityStatus.Active); // Previous-'SubmitedtoOperations'
            var fundrecived = GenService.GetAll<FundConfirmation>().Where(r => r.Application.CurrentHolding == UserId && r.FundReceived == true && r.Status == EntityStatus.Active);
            var allApp = (from app in application
                          join prop in fundrecived on app.Id equals prop.ApplicationId into extra
                          from extr in extra.DefaultIfEmpty()
                          join dcl in GenService.GetAll<DocumentCheckList>().Where(r => r.Status == EntityStatus.Active) on app.Id equals dcl.ApplicationId into dclExtra
                          from check in dclExtra.DefaultIfEmpty()
                          select new ApplicationDto
                          {
                              Id = app.Id,
                              ApplicationNo = app.ApplicationNo,
                              AccountTitle = app.AccountTitle,
                              CustomerType = app.CustomerType,
                              CustomerTypeName = app.CustomerType.ToString(),
                              ProductName = app.Product.Name,
                              MaturityAmount = app.DepositApplication.MaturityAmount ?? app.DepositApplication.TotalDepositAmount,
                              ApplicationType = app.ApplicationType,
                              ApplicationDate = app.ApplicationDate,
                              ApplicationDateText = app.ApplicationDate.ToString(),
                              LoanPrimarySecurityType =
                                  app.LoanApplication != null ? app.LoanApplication.LoanPrimarySecurityType : 0,
                              HardCopyReceived = app.HardCopyReceived,
                              HardCopyReceiveDate = app.HardCopyReceiveDate,
                              HardCopyReceiveDateText = app.HardCopyReceiveDate.ToString(),
                              ProposalId = extr != null ? extr.Id : 0,
                              DclId = check != null ? check.Id : 0
                          });
            if (!string.IsNullOrEmpty(searchString))
                allApp = allApp.Where(a =>
                a.ApplicationNo.ToLower().Contains(searchString.ToLower())
                || a.AccountTitle.ToLower().Contains(searchString.ToLower())
                || a.ProductName.ToLower().Contains(searchString.ToLower()));
            var temp = allApp.OrderByDescending(r => r.Id).ToPagedList(pageCount, pageSize);
            return temp;
        }

        public IPagedList<FundConfirmationDto> GetCaFundReceived(int pageSize, int pageCount, string searchString, long UserId)
        {
            var allApp = GenService.GetAll<FundConfirmation>().Where(s => s.Application.CurrentHolding == null && s.FundReceived == true && s.Status == EntityStatus.Active).Select(s => new FundConfirmationDto
            {
                Id = s.Id,
                ApplicationId = s.ApplicationId,
                ApplicationNo = s.Application.ApplicationNo,
                AccountTitle = s.Application.AccountTitle,
                ProductName = s.Application.Product.Name,
                ProposalId = s.ProposalId
            });
            if (!string.IsNullOrEmpty(searchString))
                allApp = allApp.Where(a => a.ApplicationNo.ToLower().Contains(searchString.ToLower()));
            var temp = allApp.OrderBy(r => r.Id).ToPagedList(pageCount, pageSize);
            return temp;
        }

        public IPagedList<DepositApplicationTrackingDto> GetApplicationsTrackList(int pageSize, int pageCount, string searchString, long UserId)
        {
            var allApp = GenService.GetAll<DepositApplicationTracking>().Where(s => s.Status == EntityStatus.Active).Select(s => new DepositApplicationTrackingDto
            {
                Id = s.Id,
                ApplicationId = s.ApplicationId,
                DepositApplicationId = s.Application.DepositApplicationId,
                ApplicationNo = s.Application.ApplicationNo,
                AccountTitle = s.Application.AccountTitle,
                MaturityAmount = s.Application.DepositApplication.MaturityAmount,
                InstrumentDeliveryStatusName = s.InstrumentDeliveryStatus.ToString(),
                ChangeDate = s.ChangeDate,
                WelcomeLetterStatusName = s.WelcomeLetterStatus.ToString()
            }).OrderByDescending(r => r.Id).ToList();
            if (!string.IsNullOrEmpty(searchString))
                allApp = allApp.Where(a => a.ApplicationNo.ToLower().Contains(searchString.ToLower())).ToList();
            allApp = (from app in allApp
                      group app by app.ApplicationNo into appGrouped
                      //from apg in appGrouped.DefaultIfEmpty()
                      select new DepositApplicationTrackingDto
                      {
                          Id = appGrouped.FirstOrDefault().Id,
                          ApplicationId = appGrouped.FirstOrDefault().ApplicationId,
                          DepositApplicationId = appGrouped.FirstOrDefault().DepositApplicationId,
                          ApplicationNo = appGrouped.Key,
                          AccountTitle = appGrouped.FirstOrDefault().AccountTitle,
                          MaturityAmount = appGrouped.FirstOrDefault().MaturityAmount,
                          InstrumentDeliveryStatusName = appGrouped.FirstOrDefault().InstrumentDeliveryStatusName,
                          ChangeDate = appGrouped.FirstOrDefault().ChangeDate,
                          WelcomeLetterStatusName = appGrouped.FirstOrDefault().WelcomeLetterStatusName
                      }).ToList();
            var temp = allApp.ToPagedList(pageCount, pageSize);
            return temp;
        }
        public IPagedList<DepositApplicationTrackingDto> GetApplicationsTrackListUserWise(int pageSize, int pageCount, string searchString, long UserId)
        {
            long employeeId = _user.GetEmployeeIdByUserId(UserId);
            var degignationOfEmployee = _employee.GetOfficeDesignationIdOfEmployee(employeeId);
            List<long> employeeIds = new List<long>();
            //List<long> userIds = new List<long>();
            //userIds.Add(UserId);
            employeeIds = _employee.GetEmployeeWiseBM(employeeId).Select(e => (long)e.EmployeeId).ToList();
            employeeIds.Add(employeeId);
            //foreach (var emp in employeeIds)
            //{
            //    var uId = _user.GetUserByEmployeeId(emp).Id;
            //    userIds.Add((long)uId);
            //}

            var allApp = GenService.GetAll<DepositApplicationTracking>().Where(s => s.Status == EntityStatus.Active && s.Application.RMId != null && employeeIds.Contains((long)s.Application.RMId)).Select(s => new DepositApplicationTrackingDto
            {
                Id = s.Id,
                ApplicationId = s.ApplicationId,
                DepositApplicationId = s.Application.DepositApplicationId,
                ApplicationNo = s.Application.ApplicationNo,
                AccountTitle = s.Application.AccountTitle,
                MaturityAmount = s.Application.DepositApplication.MaturityAmount,
                InstrumentDeliveryStatusName = s.InstrumentDeliveryStatus.ToString(),
                ChangeDate = s.ChangeDate,
                WelcomeLetterStatusName = s.WelcomeLetterStatus.ToString()
            });
            if (!string.IsNullOrEmpty(searchString))
                allApp = allApp.Where(a => a.ApplicationNo.ToLower().Contains(searchString.ToLower()));
            var temp = allApp.OrderBy(r => r.Id).ToPagedList(pageCount, pageSize);
            return temp;
        }
        public IPagedList<DocumentCheckListDto> GetApplicationsOpeningList(int pageSize, int pageCount, string searchString, long UserId)
        {

            var allApp = GenService.GetAll<DocumentCheckList>().Where(s => (s.IsApproved == true || s.Exceptions.Count == 0) && s.Status == EntityStatus.Active && s.Application.ProductType == ProductType.Deposit && s.Application.ApplicationStage == ApplicationStage.DCLApproved).Select(s => new DocumentCheckListDto
            {
                Id = s.Id,
                ApplicationId = s.ApplicationId,
                ApplicationNo = s.Application.ApplicationNo,
                AccountTitle = s.Application.AccountTitle,
                CustomerTypeName = s.Application.CustomerType.ToString(),
                ProductName = s.Product.Name,
                MaturityAmount = s.Application.DepositApplication.MaturityAmount != null ? s.Application.DepositApplication.MaturityAmount : s.Application.DepositApplication.TotalDepositAmount,
                ApplicationDateText = s.Application.ApplicationDate.ToString()
            });
            if (!string.IsNullOrEmpty(searchString))
                allApp = allApp.Where(a => a.ApplicationNo.ToLower().Contains(searchString.ToLower()));
            var temp = allApp.OrderByDescending(r => r.Id).ToPagedList(pageCount, pageSize);
            return temp;
        }

        //&& s.Application.CurrentHolding == UserId

        public IPagedList<DocumentCheckListDto> GetCaApplicationsOpeningList(int pageSize, int pageCount, string searchString, long UserId)
        {
            //(s.IsApproved == true || s.Exceptions.Count == 0) && s.Application.CurrentHolding == null && 
            var allApp = GenService.GetAll<DocumentCheckList>().Where(s => s.Status == EntityStatus.Active).Select(s => new DocumentCheckListDto
            {
                Id = s.Id,
                ApplicationId = s.ApplicationId,
                ApplicationNo = s.Application.ApplicationNo,
                AccountTitle = s.Application.AccountTitle,
                CustomerTypeName = s.Application.CustomerType.ToString(),
                ProductName = s.Product.Name,
                MaturityAmount = s.Application.DepositApplication.MaturityAmount,
                ApplicationDateText = s.Application.ApplicationDate.ToString()
            });
            if (!string.IsNullOrEmpty(searchString))
                allApp = allApp.Where(a => a.ApplicationNo.ToLower().Contains(searchString.ToLower()));
            var temp = allApp.OrderBy(r => r.Id).ToPagedList(pageCount, pageSize);
            return temp;
        }
        public IPagedList<ApplicationDto> GetOpendDepositAccountsList(int pageSize, int pageCount, string searchString, long UserId) // && s.CurrentHolding == UserId 
        {
            var allApp = GenService.GetAll<Application>().Where(s => (s.DepositApplicationId != null && s.DepositApplicationId > 0) &&
                                                                !string.IsNullOrEmpty(s.DepositApplication.CBSAccountNo) &&
                                                                s.DepositApplication.Status == EntityStatus.Active // &&
                                                                //s.ApplicationStage == ApplicationStage.AccountOpened
                                                                )
                                                                .Select(s => new ApplicationDto
                                                                {
                                                                    Id = s.Id,
                                                                    ApplicationNo = s.ApplicationNo,
                                                                    DepositApplicationId = s.DepositApplicationId,
                                                                    CBSAccountNo = s.DepositApplication.CBSAccountNo,
                                                                    AccountTitle = s.AccountTitle,
                                                                    ProductName = s.Product.Name,
                                                                    MaturityAmount = s.DepositApplication.MaturityAmount,
                                                                    MaturityDate = s.DepositApplication.MaturityDate
                                                                });
            if (!string.IsNullOrEmpty(searchString))
                allApp = allApp.Where(a => a.ApplicationNo.ToLower().Contains(searchString.ToLower()));
            var temp = allApp.OrderBy(r => r.Id).ToPagedList(pageCount, pageSize);
            return temp;
        }
        public List<ApplicationCIFsDto> GetAllCifByAppId(long AppId)
        {
            List<ApplicationCIFsDto> cifPersonal = new List<ApplicationCIFsDto>();

            var application = GenService.GetById<Application>(AppId);
            if (application != null)
            {

                if (application.CIFList.Where(c => c.CIF_PersonalId != null && c.Status == EntityStatus.Active).Count() > 0)
                {
                    var cifs = application.CIFList.Where(c => c.CIF_PersonalId != null && c.Status == EntityStatus.Active).Select(s => new ApplicationCIFsDto
                    {
                        Id = s.CIF_PersonalId,
                        ApplicationId = s.ApplicationId,
                        CIFNo = s.CIF_Personal.CIFNo,
                        CIFName = s.CIF_Personal.Name,
                        CBSCIFNo = s.CIF_Personal.CBSCIFNo,
                        ApplicantRole = s.ApplicantRole,
                        ApplicantRoleName = UiUtil.GetDisplayName(s.ApplicantRole)
                    });
                    cifPersonal.AddRange(cifs);
                }
                if (application.CIFList.Where(c => c.CIF_OrganizationalId != null && c.Status == EntityStatus.Active).Count() > 0)
                {
                    foreach (var org in application.CIFList.Where(c => c.CIF_OrganizationalId != null))
                    {
                        var owners = org.CIF_Organizational.Owners.Where(o => o.Status == EntityStatus.Active).Select(s => s.CIF_Personal).ToList();
                        cifPersonal.AddRange(owners.Select(n => new ApplicationCIFsDto
                        {
                            Id = n.Id,
                            //ApplicationId = n.ApplicationId,
                            CIFNo = n.CIFNo,
                            CIFName = n.Name,
                            CBSCIFNo = n.CBSCIFNo,
                            ApplicantRoleName = "Owner"
                        }));
                    }
                }
                if (application.DepositApplicationId != null)
                {
                    List<DepositNominee> nominees;
                    nominees = application.DepositApplication.Nominees.Where(n => n.Status == EntityStatus.Active).ToList();
                    cifPersonal.AddRange(nominees.Select(n => new ApplicationCIFsDto
                    {
                        Id = n.NomineeCif.Id,
                        //ApplicationId = n.ApplicationId,
                        CIFNo = n.NomineeCif.CIFNo,
                        CIFName = n.NomineeCif.Name,
                        CBSCIFNo = n.NomineeCif.CBSCIFNo,
                        ApplicantRoleName = "Nominee"
                    }));
                    cifPersonal.AddRange(nominees.Where(n => n.GuiardianCifId != null).Select(n => new ApplicationCIFsDto
                    {
                        Id = n.GuiardianCif.Id,
                        CIFNo = n.GuiardianCif.CIFNo,
                        CIFName = n.GuiardianCif.Name,
                        CBSCIFNo = n.GuiardianCif.CBSCIFNo,
                        ApplicantRoleName = "Nominee Guiardian"
                    }));
                    if (application.DepositApplication.GuiardianCifId != null)
                    {
                        var guiardian = new ApplicationCIFsDto();

                        cifPersonal.Add(new ApplicationCIFsDto
                        {
                            Id = application.DepositApplication.GuiardianCif.Id,
                            CIFNo = application.DepositApplication.GuiardianCif.CIFNo,
                            CIFName = application.DepositApplication.GuiardianCif.Name,
                            CBSCIFNo = application.DepositApplication.GuiardianCif.CBSCIFNo,
                            ApplicantRoleName = "Guiardian"
                        });
                    }
                }
                if (application.LoanApplicationId != null)
                {
                    var guarantors = application.LoanApplication.Guarantors.Where(n => n.Status == EntityStatus.Active).ToList();
                    cifPersonal.AddRange(guarantors.Select(n => new ApplicationCIFsDto
                    {
                        Id = n.GuarantorCif.Id,
                        //ApplicationId = n.ApplicationId,
                        CIFNo = n.GuarantorCif.CIFNo,
                        CIFName = n.GuarantorCif.Name,
                        CBSCIFNo = n.GuarantorCif.CBSCIFNo,
                        ApplicantRoleName = "Guarantor"
                    }));
                }

            }
            return cifPersonal;
        }

        public List<ApplicationCIFsDto> GetAllCifOrgByAppId(long AppId)
        {
            List<ApplicationCIFsDto> cifPersonal = new List<ApplicationCIFsDto>();

            var application = GenService.GetById<Application>(AppId);
            if (application != null)
            {

                if (application.CIFList.Where(c => c.CIF_OrganizationalId != null && c.Status == EntityStatus.Active).Count() > 0)
                {
                    var cifs = application.CIFList.Where(c => c.CIF_OrganizationalId != null && c.Status == EntityStatus.Active).Select(s => new ApplicationCIFsDto
                    {
                        Id = s.CIF_OrganizationalId,
                        ApplicationId = s.ApplicationId,
                        CIFNo = s.CIF_Organizational.CIFNo,
                        CIFName = s.CIF_Organizational.CompanyName,
                        CBSCIFNo = s.CIF_Organizational.CBSCIFNo, //.CBSCIFNo,
                        ApplicantRole = s.ApplicantRole,
                        ApplicantRoleName = s.ApplicantRole.ToString()
                    });
                    cifPersonal.AddRange(cifs);
                }
            }
            return cifPersonal;
        }

        public long GetCifIdByAppId(long AppId)
        {
            var appCif =
                GenService.GetAll<ApplicationCIFs>()
                    .Where(s => s.ApplicationId == AppId && s.Status == EntityStatus.Active)
                    .FirstOrDefault();
            if (appCif != null)
            {
                if (appCif.CIF_PersonalId != null)
                {
                    long cifId = (long)appCif.CIF_PersonalId;
                    if (cifId > 0)
                    {
                        return cifId;
                    }
                }
            }
            return 0;
        }

        public ApplicationCIFsDto GetAppCifInfo(long appId, long cifId)
        {
            //var cifData = GenService.GetAll<CIF_Personal>().FirstOrDefault(r => r.Id == cifId);
            var cifData = GenService.GetAll<ApplicationCIFs>().Where(a => a.ApplicationId == appId && a.CIF_PersonalId == cifId).FirstOrDefault();
            var result = Mapper.Map<ApplicationCIFsDto>(cifData);
            return result;
        }

        public List<ApplicationCIFsDto> GetAllAppCifInfo(long appId)
        {
            //var cifData = GenService.GetAll<CIF_Personal>().FirstOrDefault(r => r.Id == cifId);
            var cifData = GenService.GetAll<ApplicationCIFs>().Where(a => a.ApplicationId == appId).ToList();
            var result = Mapper.Map<List<ApplicationCIFsDto>>(cifData);
            return result;
        }

        public object GetCBSInfoForApplication(long AppId)
        {
            var application = GenService.GetById<Application>(AppId);
            if (application != null && application.DepositApplicationId != null)
            {
                var data = new
                {
                    AccountOpenDate = application.DepositApplication.AccountOpenDate,
                    InstrumentNo = application.DepositApplication.InstrumentNo,
                    InstrumentDate = application.DepositApplication.InstrumentDate,
                    CBSAccountNo = application.DepositApplication.CBSAccountNo,
                    MaturityDate = application.DepositApplication.MaturityDate,
                    InstrumentDispatchStatus = application.DepositApplication.InstrumentDispatchStatus,
                    MaturityAmount = application.DepositApplication.MaturityAmount,
                    ProductType = application.ProductType,
                    CBSBranchId = application.DepositApplication.CBSBranchId
                };

                return data;
            }
            else if (application != null && application.LoanApplicationId != null)
            {
                var data = new
                {
                    AccountOpenDate = application.LoanApplication.AccountOpenDate,
                    CBSAccountNo = application.LoanApplication.CBSAccountNo,
                    ProductType = application.ProductType,
                    CBSBranchId = application.LoanApplication.CBSBranchId
                };
                return data;
            }
            return null;
        }
        public ResponseDto SaveCBSInfo(CBSInfoDto dto, long userId)
        {
            var response = new ResponseDto();

            Application App = new Application();
            var log = new ApplicationLog();
            var cifList = new List<CIF_Personal>();

            if (dto.ApplicationId != null)
                App = GenService.GetById<Application>((long)dto.ApplicationId);
            if (App != null && App.DepositApplicationId != null)
            {
                App.DepositApplication.AccountOpenDate = dto.AccountOpenDate;
                App.DepositApplication.InstrumentNo = dto.InstrumentNo;
                App.DepositApplication.InstrumentDate = dto.InstrumentDate;
                App.DepositApplication.CBSAccountNo = dto.CBSAccountNo;
                App.DepositApplication.CBSBranchId = dto.CBSBranchId;
                App.DepositApplication.MaturityDate = dto.MaturityDate;
                App.DepositApplication.InstrumentDispatchStatus = dto.InstrumentDispatchStatus;
                App.DepositApplication.MaturityAmount = dto.MaturityAmount;

                log.Activity = Activity.Submit;
                log.AppIdRef = App.DepositApplicationId;
                log.ApplicationId = App.Id;
                log.AppType = AppType.DepositApplication;
                log.CreateDate = DateTime.Now;
                log.CreatedBy = userId;
                log.FromUserId = userId;
                log.FromStage = App.ApplicationStage;
                log.ToStage = ApplicationStage.AccountOpened;
                log.Status = EntityStatus.Active;

                App.ApplicationStage = ApplicationStage.AccountOpened;
                App.CurrentHolding = null;
                App.CurrentHoldingEmpId = null;

            }
            else if (App != null && App.LoanApplicationId != null)
            {
                App.LoanApplication.AccountOpenDate = dto.AccountOpenDate;
                App.LoanApplication.CBSAccountNo = dto.CBSAccountNo;
                App.LoanApplication.CBSBranchId = dto.CBSBranchId;

            }
            foreach (var depoCifs in dto.CIFs)
            {
                if (depoCifs.Id != null)
                {
                    var cif = GenService.GetById<CIF_Personal>((long)depoCifs.Id);
                    if (cif != null && !string.IsNullOrEmpty(depoCifs.CBSCIFNo))
                    {
                        cif.CBSCIFNo = depoCifs.CBSCIFNo;
                        cifList.Add(cif);
                    }
                }
            }
            using (var tran = new TransactionScope())
            {
                try
                {
                    if (App.DepositApplication != null)
                    {
                        GenService.Save(App.DepositApplication);
                        GenService.Save(App);
                        GenService.Save(log);
                    }
                    else if (App.LoanApplication != null)
                    {
                        GenService.Save(App.LoanApplication);
                    }
                    if (cifList.Count > 0)
                        GenService.Save(cifList);
                    tran.Complete();

                }
                catch (Exception ex)
                {
                    tran.Dispose();
                    response.Message = "CBS Info Update Failed For CIF";
                    return response;
                }
            }
            response.Success = true;
            response.Message = "CBS Info Updated For CIF Successfully";
            return response;
        }
        public IPagedList<DocumentCheckListDto> GetDocumentExceptionList(int pageSize, int pageCount, string searchString, long userId)
        {
            var allApp = GenService.GetAll<DocumentCheckList>().Where(s => s.Application.CurrentHolding == userId && (s.IsApproved == false || s.IsApproved != null) && s.Status == EntityStatus.Active && s.Exceptions.Count > 0).Select(s => new DocumentCheckListDto
            {
                Id = s.Id,
                ApplicationId = s.ApplicationId,
                ApplicationNo = s.Application.ApplicationNo,
                DCLNo = s.DCLNo,
                DCLDate = s.DCLDate,
                ProductName = s.Application.Product.Name,
                ExceptionCount = s.Exceptions.Count,
                ProductTypeId = s.Product != null ? s.Product.ProductType : 0
            });
            //var data = GenService.GetAll<DocumentCheckList>().Where( s => (s.IsApproved == false || s.IsApproved != null) && s.Status == EntityStatus.Active && s.Exceptions.Count > 0);
            //var allApp = Mapper.Map<List<DocumentCheckListDto>>(data);
            if (!string.IsNullOrEmpty(searchString))
                allApp = allApp.Where(a => a.DCLNo.ToLower().Contains(searchString.ToLower()));
            var temp = allApp.OrderBy(r => r.Id).ToPagedList(pageCount, pageSize);
            return temp;
        }

        public long GetDocCheckListIdByAppId(long AppId)
        {
            long docChkListId = 0;
            var docChkList = GenService.GetAll<DocumentCheckList>()
                .Where(e => e.ApplicationId == AppId && e.Status == EntityStatus.Active)
                .OrderByDescending(e => e.Id).FirstOrDefault();
            if (docChkList != null)
            {
                docChkListId = (long)docChkList.Id;
            }

            if (docChkListId > 0)
                return docChkListId;
            return 0;
        }

        public object GetExceptionListbyDclId(long dclId)
        {
            var exList = GenService.GetAll<DocumentCheckListException>().Where(x => x.DCLId == dclId).ToList();
            return exList;
        }
        public DocumentCheckListDto LoadDocumentCheckListById(long? id)
        {
            var data = GenService.GetById<DocumentCheckList>((long)id);
            var result = Mapper.Map<DocumentCheckListDto>(data);
            if (result.ProposalId != null)
            {
                var offer =
                    GenService.GetAll<OfferLetter>()
                        .Where(r => r.ProposalId == result.ProposalId && r.Status == EntityStatus.Active)
                        .FirstOrDefault();
                result.OfferLetterNo = offer != null ? offer.OfferLetterNo : "";
                result.MaturityAmount = GenService.GetById<Proposal>((long)result.ProposalId).RecomendedLoanAmountFromIPDC;
            }
            else
            {
                result.MaturityAmount = data.Application.DepositApplication.MaturityAmount;
                result.MaturityDate = data.Application.DepositApplication.MaturityDate;
            }

            result.ApplicationId = data.ApplicationId;
            result.ApplicationNo = data.Application.ApplicationNo;
            result.AccountTitle = data.Application.AccountTitle;
            result.CustomerTypeName = data.Application.CustomerType.ToString();

            if (data.Product != null)
            {
                result.ProductTypeId = data.Product.ProductType;
                result.FacilityTypeName = data.Product.ProductType > 0 ? UiUtil.GetDisplayName(data.Product.ProductType) : "";
                result.ProductName = data.Product.Name;
            }

            //result.MaturityAmount = data.Application != null ? data.Application.LoanApplicationId != null ? data.Application.LoanApplication.
            result.Documents.RemoveAll(f => f.Status != EntityStatus.Active);
            result.Exceptions.RemoveAll(o => o.Status != EntityStatus.Active);
            result.Securities.RemoveAll(o => o.Status != EntityStatus.Active);
            return result;
        }
        public ResponseDto SaveDepositAppTracking(DepositApplicationTrackingDto dto, long userId)
        {
            var entity = new DepositApplicationTracking();
            ResponseDto response = new ResponseDto();
            //if (dto != null && dto.Id > 0)
            //{
            //    if (dto.Id != null)
            //        entity = GenService.GetById<DepositApplicationTracking>((long)dto.Id);

            //    try
            //    {
            //        entity.ChangeDate = dto.ChangeDate;
            //        GenService.Save(entity);
            //        response.Id = entity.Id;
            //    }
            //    catch (Exception ex)
            //    {
            //        response.Id = entity.Id;
            //        response.Message = "Operation Failed";
            //        return response;
            //    }

            //    response.Success = true;
            //    response.Message = "Successfull";

            //}

            //else
            //{
            if (dto.ApplicationId != null)
            {
                var app = GenService.GetById<Application>((long)dto.ApplicationId);
                var log = new ApplicationLog();
                if (app != null)

                {
                    log.Activity = Activity.Submit;
                    log.AppIdRef = app.Id;
                    log.ApplicationId = app.Id;
                    log.AppType = AppType.Application;
                    log.CreateDate = DateTime.Now;
                    log.CreatedBy = userId;
                    log.FromUserId = userId;
                    log.FromStage = app.ApplicationStage;

                    if (dto.InstrumentDeliveryStatus == InstrumentDeliveryStatus.Delivered_to_RM)
                    {
                        log.ToStage = ApplicationStage.InsturmentSentToRM;
                        app.ApplicationStage = ApplicationStage.InsturmentSentToRM;
                    }
                    else if (dto.InstrumentDeliveryStatus == InstrumentDeliveryStatus.Delivered_to_client)
                    {
                        log.ToStage = ApplicationStage.InsturmentDeliveredtoClient;
                        app.ApplicationStage = ApplicationStage.InsturmentDeliveredtoClient;
                    }
                    else if (dto.InstrumentDeliveryStatus == InstrumentDeliveryStatus.Sent_to_Branch)
                    {
                        log.ToStage = ApplicationStage.InsturmentSentToBranch;
                        app.ApplicationStage = ApplicationStage.InsturmentSentToBranch;
                    }
                    else if (dto.InstrumentDeliveryStatus == InstrumentDeliveryStatus.Kept_in_file)
                    {
                        log.ToStage = ApplicationStage.InsturmentKeptinFile;
                        app.ApplicationStage = ApplicationStage.InsturmentKeptinFile;
                    }
                    else if (dto.InstrumentDeliveryStatus == InstrumentDeliveryStatus.Pending_issue)
                    {
                        log.ToStage = ApplicationStage.PendingIssue;
                        app.ApplicationStage = ApplicationStage.PendingIssue;
                    }


                    log.Status = EntityStatus.Active;
                }
                dto.Id = null;
                entity = new DepositApplicationTracking();
                entity = Mapper.Map<DepositApplicationTracking>(dto);
                entity.Status = EntityStatus.Active;
                entity.CreatedBy = userId;
                entity.CreateDate = DateTime.Now;

                using (var tran = new TransactionScope())
                {
                    try
                    {
                        GenService.Save(log);
                        GenService.Save(app);
                        GenService.Save(entity);
                        tran.Complete();
                        response.Success = true;
                        response.Message = "Tracking Saved Successfully";
                    }
                    catch (Exception ex)
                    {
                        tran.Dispose();
                        response.Message = ex.Message;
                    }
                }
                return response;
            }
            //}
            return response;
        }
        public DepositApplicationTrackingDto LoadDepositAppTrackingbyAppId(long AppId, long DepAppId, long Id)
        {
            if (Id > 0)
            {
                if (AppId > 0 && DepAppId > 0)
                {
                    var depApp = GenService.GetById<Application>(AppId);
                    var depAppTrack =
                        GenService.GetAll<DepositApplicationTracking>()
                            .Where(r => r.Id == Id && r.DepositApplicationId == DepAppId && r.ApplicationId == AppId && r.Status == EntityStatus.Active)
                            .OrderByDescending(r => r.Id)
                            .FirstOrDefault();
                    var result = Mapper.Map<DepositApplicationTrackingDto>(depAppTrack);
                    result.ApplicationNo = depApp.ApplicationNo;
                    result.AccountTitle = depApp.AccountTitle;
                    result.MaturityAmount = depApp.DepositApplication.MaturityAmount;
                    return result;
                }
            }
            if (AppId > 0 && DepAppId > 0)
            {

                var app = new DepositApplicationTracking();

                var result
                    = Mapper.Map<DepositApplicationTrackingDto>(app);

                var application = GenService.GetById<Application>(AppId);

                //var depApp = new DepositApplicationTracking();
                result.ApplicationId = application.Id;
                result.DepositApplicationId = DepAppId;
                result.ApplicationNo = application.ApplicationNo;
                result.AccountTitle = application.AccountTitle;
                result.MaturityAmount = application.DepositApplication.MaturityAmount;
                return result;
            }
            return null;
        }
        public IPagedList<ApplicationDto> GetApplicationPOList(int pageSize, int pageCount, string searchString, long UserId)
        {
            var application =
                (from app in GenService.GetAll<Application>().Where(s => s.Status == EntityStatus.Active && s.LoanApplicationId != null && s.ApplicationStage == ApplicationStage.DCLApproved && s.Product.FacilityType == ProposalFacilityType.Auto_Loan) // Previous 'SubmitedToOperation'
                 join loan in GenService.GetAll<LoanApplication>() on app.LoanApplicationId equals loan.Id
                 join dcl in GenService.GetAll<DocumentCheckList>().Where(dcl => dcl.IsApproved != null && (bool)dcl.IsApproved) on app.Id equals dcl.ApplicationId into clList
                 join po in GenService.GetAll<PurchaseOrder>().Where(r => r.ApplicationId > 0 && r.Status == EntityStatus.Active) on app.Id equals po.ApplicationId into prchOrd
                 from pOrd in prchOrd.DefaultIfEmpty()
                 from extr in clList.DefaultIfEmpty()
                 select new ApplicationDto
                 {
                     Id = app.Id,
                     ApplicationNo = app.ApplicationNo,
                     AppliedLoanAmount = extr.Proposal.RecomendedLoanAmountFromIPDC,
                     AccountTitle = app.AccountTitle,
                     ApplicationDate = app.ApplicationDate,
                     VehicleName = pOrd.VehicleBrand,
                     ModelYear = pOrd.ManufacturingYear,
                     DCLNo = extr.DCLNo,
                     LoanPrimarySecurityType = loan.LoanPrimarySecurityType,
                     PoId = pOrd != null ? pOrd.Id : 0,
                     RMName = app.RMEmp.Person.FirstName + " " + app.RMEmp.Person.LastName

                 }).Where(l => l.LoanPrimarySecurityType == LoanPrimarySecurityType.VehiclePrimarySecurity);
            if (!string.IsNullOrEmpty(searchString))
                application = application.Where(a => a.DCLNo.ToLower().Contains(searchString.ToLower())
                || a.ApplicationNo.ToLower().Contains(searchString.ToLower())
                || a.AccountTitle.ToLower().Contains(searchString.ToLower())
                || a.RMName.ToLower().Contains(searchString.ToLower()));
            var temp = application.OrderByDescending(r => r.Id).ToPagedList(pageCount, pageSize);
            return temp;
        }

        public PurchaseOrderDto LoadPurchaseOrder(long? proposalId, long? appId, long? id)
        {
            var purchase = new PurchaseOrderDto();
            if (id != null && id > 0)
            {
                var data = GenService.GetById<PurchaseOrder>((long)id);
                purchase = Mapper.Map<PurchaseOrderDto>(data);
                if (purchase != null)
                {
                    if (purchase.Documents != null)
                    {
                        purchase.Documents.RemoveAll(f => f.Status != EntityStatus.Active);
                    }
                }
                return purchase;
            }
            if (appId != null)
            {
                var application = GenService.GetById<Application>((long)appId);
                string applicantNames = "";
                if (application.CustomerType == ApplicationCustomerType.Individual)
                {
                    var cifList = application.CIFList.Where(c => c.CIF_PersonalId != null && c.Status == EntityStatus.Active).ToList();
                    for (int i = 1; i <= cifList.Count; i++)
                    {
                        applicantNames += cifList[i].CIF_Personal.Name;
                        if (i == (cifList.Count - 1))
                            applicantNames += " & ";
                        else if (i < (cifList.Count - 1))
                            applicantNames += ", ";
                    }
                }
                string[] documents = {
                    "Copy of Car Registration Certificate.",
                    "Copy of Fitness.",
                    "Copy of Tax Token.",
                    "Car sale Invoice.",
                    "Money Receipt of Down Payment.",
                    "Cash money receipt of Insurance.",
                    "Original Insurance Certificate.",
                    "Delivery Challan (“Received and Accepted” signed by " + applicantNames + ")."
                };
                if (application != null)
                {
                    purchase.ApplicationId = (long)appId;
                    purchase.ProposalId = proposalId;
                    purchase.ApplicationTitle = application.AccountTitle;
                    var applicationCiFs = application.CIFList.FirstOrDefault(c => c.ApplicantRole == ApplicantRole.Primary);
                    if (applicationCiFs != null)
                    {
                        var address = applicationCiFs.CIF_Personal.ResidenceAddress;
                        purchase.CustomerName = applicationCiFs.CIF_Personal.Title + " " + applicationCiFs.CIF_Personal.Name;
                        purchase.CustomerAddress = Mapper.Map<AddressDto>(address);
                    }
                    if (application.LoanApplication != null)
                    {
                        if (application.LoanApplication.LoanPrimarySecurityType == LoanPrimarySecurityType.VehiclePrimarySecurity)
                        {
                            var vehiclePrimarySecurity = GenService.GetAll<VehiclePrimarySecurity>().Where(r => r.Status == EntityStatus.Active).OrderByDescending(r => r.Id).FirstOrDefault(); //Where(r => r.Status == EntityStatus.Active).
                            if (vehiclePrimarySecurity != null)
                            {
                                if (vehiclePrimarySecurity.VendorType == VendorType.Showroom)
                                    purchase.SellersName = vehiclePrimarySecurity.Vendor.Name;
                                else
                                    purchase.SellersName = vehiclePrimarySecurity.SellersName;
                                purchase.SellersAddress = Mapper.Map<AddressDto>(vehiclePrimarySecurity.SellersAddress);
                                var proposal = GenService.GetAll<Proposal>().Where(p => p.ApplicationId == appId && p.Status == EntityStatus.Active).OrderByDescending(p => p.Id).FirstOrDefault();
                                if (proposal != null)
                                    purchase.RecomendedLoanAmountFromIPDC = (decimal)proposal.RecomendedLoanAmountFromIPDC;
                                if (vehiclePrimarySecurity.Price != null)
                                    purchase.QuotationPrice = (decimal)vehiclePrimarySecurity.Price;
                                purchase.VehicleBrand = vehiclePrimarySecurity.VehicleName;
                                purchase.ChassisNo = vehiclePrimarySecurity.ChassisNo;
                                purchase.EngineNo = vehiclePrimarySecurity.EngineNo;
                                purchase.ManufacturingYear = vehiclePrimarySecurity.MnufacturingYear;
                                purchase.Colour = vehiclePrimarySecurity.Colour;
                                purchase.CC = vehiclePrimarySecurity.CC;
                            }
                        }
                    }
                    purchase.Documents = documents.Select(x => new PODocumentDto
                    {
                        Name = x
                    }).ToList();
                }
                return purchase;
            }
            if (proposalId != null)
            {
                var proposal = GenService.GetById<Proposal>((long)proposalId);
                if (appId == null)
                {
                    appId = proposal != null ? proposal.ApplicationId : 0;
                }
            }

            return null;
        }

        public ResponseDto SavePurchaseOrder(PurchaseOrderDto dto, long userId)
        {
            ResponseDto response = new ResponseDto();
            var address = new Address();
            try
            {
                #region Edit
                if (dto.Id > 0)
                {
                    var prev = GenService.GetById<PurchaseOrder>((long)dto.Id);
                    dto.ApplicationId = prev.ApplicationId;
                    dto.ProposalId = prev.ProposalId;
                    if (dto.Documents != null)
                    {
                        foreach (var item in dto.Documents)
                        {
                            PODocument docDetail;
                            if (item.Id != null && item.Id > 0)
                            {
                                docDetail = GenService.GetById<PODocument>((long)item.Id);
                                item.Status = docDetail.Status;
                                item.CreateDate = docDetail.CreateDate;
                                item.CreatedBy = docDetail.CreatedBy;
                                item.EditDate = DateTime.Now;
                                item.EditedBy = userId;
                                item.POId = docDetail.POId;
                                Mapper.Map(item, docDetail);
                                GenService.Save(docDetail);
                            }
                            else
                            {
                                docDetail = new PODocument();
                                item.POId = prev.Id;
                                item.CreateDate = DateTime.Now;
                                item.CreatedBy = userId;
                                item.Status = EntityStatus.Active;
                                docDetail = Mapper.Map<PODocument>(item);
                                GenService.Save(docDetail);
                            }

                        }
                    }
                    if (dto.RemovedDocuments != null)
                    {
                        foreach (var item in dto.RemovedDocuments)
                        {
                            var detail = GenService.GetById<PODocument>(item);
                            if (detail != null)
                            {
                                detail.Status = EntityStatus.Inactive;
                                detail.EditDate = DateTime.Now;
                                detail.EditedBy = userId;
                            }
                            GenService.Save(detail);
                        }
                    }
                    if (dto.SellersAddress.IsChanged)
                    {
                        if (dto.SellersAddress.Id != null)
                        {
                            address = GenService.GetById<Address>((long)dto.SellersAddress.Id);
                            dto.SellersAddress.CreateDate = address.CreateDate;
                            dto.SellersAddress.CreatedBy = address.CreatedBy;
                            address = Mapper.Map(dto.SellersAddress, address);
                            GenService.Save(address);
                            dto.SellersAddressId = address.Id;
                        }
                        else if (dto.SellersAddress != null && dto.SellersAddress.CountryId != null && dto.SellersAddress.CountryId > 0)
                        {
                            var sellerAddress = Mapper.Map<Address>(dto.SellersAddress);
                            GenService.Save(sellerAddress);
                            dto.SellersAddressId = sellerAddress.Id;
                        }
                    }
                    if (dto.CustomerAddress.IsChanged)
                    {
                        if (dto.CustomerAddress.Id != null)
                        {
                            address = GenService.GetById<Address>((long)dto.CustomerAddress.Id);
                            dto.CustomerAddress.CreateDate = address.CreateDate;
                            dto.CustomerAddress.CreatedBy = address.CreatedBy;
                            address = Mapper.Map(dto.CustomerAddress, address);
                            GenService.Save(address);
                            dto.CustomerAddressId = address.Id;
                        }
                        else if (dto.CustomerAddress != null && dto.CustomerAddress.CountryId != null && dto.CustomerAddress.CountryId > 0)
                        {
                            var customerAddress = Mapper.Map<Address>(dto.CustomerAddress);
                            GenService.Save(customerAddress);
                            dto.CustomerAddressId = customerAddress.Id;
                        }
                    }
                    dto.CreateDate = prev.CreateDate;
                    dto.CreatedBy = prev.CreatedBy;
                    dto.Status = prev.Status;
                    dto.ProposalId = prev.ProposalId;
                    dto.EditDate = DateTime.Now;
                    prev = Mapper.Map(dto, prev);
                    GenService.Save(prev);
                    response.Id = prev.Id;
                    response.Success = true;
                    response.Message = "Purchase Order Edited Successfully";
                }
                #endregion
                #region Add
                else
                {
                    if (dto.ApplicationId > 0)
                    {
                        var app = GenService.GetAll<PurchaseOrder>().Where(r => r.Status == EntityStatus.Active && r.ApplicationId == dto.ApplicationId);
                        if (app != null)
                        {
                            app.ForEach(l => l.Status = EntityStatus.Inactive);
                            GenService.Save(app.ToList());
                        }
                    }
                    var data = Mapper.Map<PurchaseOrder>(dto);
                    data.EditDate = DateTime.Now;
                    data.Status = EntityStatus.Active;
                    data.CreateDate = DateTime.Now;
                    if (dto.Documents != null && dto.Documents.Count > 0)
                    {
                        data.Documents = Mapper.Map<List<PODocument>>(dto.Documents);
                        foreach (var item in data.Documents)
                        {
                            item.CreateDate = DateTime.Now;
                            item.CreatedBy = userId;
                            item.Status = EntityStatus.Active;
                        }
                    }
                    var sellerAddress = Mapper.Map<Address>(dto.SellersAddress);
                    if (dto.SellersAddress.CountryId != null)
                    {
                        GenService.Save(sellerAddress);
                        data.SellersAddressId = sellerAddress.Id;
                    }
                    var customerAddress = Mapper.Map<Address>(dto.CustomerAddress);
                    if (dto.CustomerAddress.CountryId != null)
                    {
                        GenService.Save(customerAddress);
                        data.CustomerAddressId = customerAddress.Id;
                    }
                    //data.DCLNo = _sequencer.GetUpdatedDCLNo();
                    //data.OfferLetterNo = _sequencer.GetUpdatedOfferLetterNo();
                    GenService.Save(data);
                    response.Id = data.Id;
                    response.Success = true;
                    response.Message = "Purchase Order Saved Successfully";
                }
                #endregion
            }
            catch (Exception ex)
            {
                response.Message = "Error Message : " + ex;
            }
            return response;
        }

        public ResponseDto SavePOApproval(PoApprovalDto dto, long userId)
        {
            ResponseDto response = new ResponseDto();
            var log = new ApplicationLog();

            if ((dto.PoId != null) && (dto.PoId > 0))
            {
                var po = GenService.GetById<PurchaseOrder>((long)dto.PoId);
                if (dto.PersonType == PersonType.Customer)
                {
                    po.CustomerApproved = true;
                    po.CustomerApprovalDate = dto.QuotationDate;
                }
                if (dto.PersonType == PersonType.Vendor)
                {
                    po.VendorApproved = true;
                    po.VendorApprovalDate = dto.QuotationDate;
                }
                if (dto.PersonType == PersonType.Po)
                {
                    po.POApproval = true;
                    po.POApprovalDate = dto.QuotationDate;
                }
                if ((po.CustomerApproved == true || po.VendorApproved == true || po.POApproval == true) && dto.ApplicationId > 0)
                {
                    if (dto.ApplicationId != null)
                    {
                        var application = GenService.GetById<Application>((long)dto.ApplicationId);
                        application.CurrentHolding = null;
                        application.CurrentHoldingEmpId = null;
                        log.Activity = Activity.Submit;
                        log.AppIdRef = dto.ApplicationId;
                        log.AppType = AppType.Application;
                        log.CreateDate = DateTime.Now;
                        log.CreatedBy = userId;
                        log.FromUserId = userId;
                        log.FromStage = application.ApplicationStage; // Previous 'SubmittedToOperations'
                        application.ApplicationStage = ApplicationStage.POIssued; //Previous 'SubmittedToMemo'
                        log.ToStage = ApplicationStage.POIssued; // Previous 'SubmittedToMemo'
                        log.ToUserId = userId; //parentUserId != null ? (long)parentUserId : 0;
                        log.Status = EntityStatus.Active;
                        log.ApplicationId = (long)dto.ApplicationId;
                        GenService.Save(application, false);
                        GenService.Save(log, false);
                    }
                }
                GenService.Save(po, false);
                GenService.SaveChanges();
                response.Success = true;
                response.Message = "Approval Succeed";
            }
            else
            {
                response.Message = "Please Save Po Entry First";
            }
            return response;
        }
        public IPagedList<ApplicationDto> ReadyForDisbursmentMemo(int pageSize, int pageCount, string searchString, long UserId)
        {
            var data = GenService.GetAll<Application>().Where(r => r.Status == EntityStatus.Active && ((r.Product.FacilityType == ProposalFacilityType.Auto_Loan && r.ApplicationStage == ApplicationStage.POIssued) || (r.Product.FacilityType != ProposalFacilityType.Auto_Loan && r.ApplicationStage == ApplicationStage.DCLApproved))).ToList(); // Previous 'SubmittedToMemo'
            var application = (from app in data
                               join dcl in GenService.GetAll<DocumentCheckList>().Where(r => r.Status == EntityStatus.Active).OrderByDescending(r => r.Id) on app.Id equals dcl.ApplicationId into check
                               from appDto in check.DefaultIfEmpty()
                               join proposal in GenService.GetAll<Proposal>().Where(r => r.Status == EntityStatus.Active).OrderByDescending(r => r.Id) on app.Id equals proposal.ApplicationId into prp
                               from extra in prp.DefaultIfEmpty()
                               join memo in GenService.GetAll<DisbursementMemo>().Where(r => r.Status == EntityStatus.Active).OrderByDescending(r => r.ApplicationId) on app.Id equals memo.ApplicationId into dm
                               from extr in dm.DefaultIfEmpty()
                               select new ApplicationDto
                               {
                                   Id = app.Id,
                                   ApplicationNo = app.ApplicationNo,
                                   AccountTitle = app.AccountTitle,
                                   FacilityTypeName = app.Product != null ? app.Product.FacilityType.ToString() : "",
                                   ProductName = app.Product != null ? app.Product.Name : "",
                                   AppliedLoanAmount = extra != null ? extra.RecomendedLoanAmountFromIPDC : 0,
                                   DclId = appDto != null ? appDto.Id : 0,
                                   DCLNo = appDto != null ? appDto.DCLNo : "",
                                   ProposalNo = extra != null ? extra.CreditMemoNo : "",
                                   ProposalId = extra != null ? (long)extra.Id : 0,
                                   DmId = extr != null ? extr.Id : 0
                               });
            if (!string.IsNullOrEmpty(searchString))
                application = application.Where(a => a.ApplicationNo.ToLower().Contains(searchString.ToLower())).ToList();
            var temp = application.OrderBy(r => r.Id).ToPagedList(pageCount, pageSize);
            return temp;
        }

        public ResponseDto SaveDisbursmentMemo(DisbursementMemoDto dto, long userId)
        {
            ResponseDto response = new ResponseDto();
            try
            {
                List<Disbursment_Signatory> signatoryList = new List<Disbursment_Signatory>();
                #region Edit
                if (dto.Id > 0)
                {
                    var entity = GenService.GetById<DisbursementMemo>((long)dto.Id);
                    if (entity.IsApproved != null && entity.IsApproved == true)
                    {
                        response.Message = "Approved Disbursement Memo cannot be edited.";
                        return response;
                    }
                    if (dto.Texts != null)
                    {
                        foreach (var item in dto.Texts)
                        {
                            DMText text;
                            if (item.Id != null && item.Id > 0)
                            {
                                text = GenService.GetById<DMText>((long)item.Id);
                                if (item.Status == null)
                                    item.Status = text.Status;
                                item.CreateDate = text.CreateDate;
                                item.CreatedBy = text.CreatedBy;
                                item.DMId = text.DMId;
                                item.EditDate = DateTime.Now;
                                item.EditedBy = userId;
                                item.Status = EntityStatus.Active;
                                Mapper.Map(item, text);
                                GenService.Save(text, false);
                            }
                            else
                            {
                                text = new DMText();
                                item.CreateDate = DateTime.Now;
                                item.CreatedBy = userId;
                                item.Status = EntityStatus.Active;
                                item.DMId = entity.Id;
                                text = Mapper.Map<DMText>(item);
                                GenService.Save(text, false);
                            }

                        }
                    }
                    if (dto.RemovedTexts != null)
                    {
                        foreach (var item in dto.RemovedTexts)
                        {
                            var text = GenService.GetById<DMText>(item);
                            if (text != null)
                            {
                                text.Status = EntityStatus.Inactive;
                                text.EditDate = DateTime.Now;
                                text.EditedBy = userId;
                            }
                            GenService.Save(text, false);
                        }
                    }
                    #region Disbursment_Signatory

                    if (dto.Signatories != null)
                    {
                        foreach (var signatorDto in dto.Signatories)
                        {
                            var signatory = new Disbursment_Signatory();
                            if (signatorDto.Id != null && signatorDto.Id > 0)
                            {
                                signatory = GenService.GetById<Disbursment_Signatory>((long)signatorDto.Id);
                                signatorDto.CreateDate = signatory.CreateDate;
                                signatorDto.CreatedBy = signatory.CreatedBy;
                                signatorDto.Status = signatory.Status;
                                signatorDto.EditDate = DateTime.Now;
                                signatorDto.DMId = signatory.DMId;
                                signatory = Mapper.Map(signatorDto, signatory);
                                signatoryList.Add(signatory);
                            }
                            else
                            {
                                //assesments = new Proposal_OverallAssessment();
                                signatory = Mapper.Map<Disbursment_Signatory>(signatorDto);
                                signatory.Status = EntityStatus.Active;
                                signatory.CreatedBy = userId;
                                signatory.CreateDate = DateTime.Now;
                                signatory.DMId = entity.Id;
                                signatoryList.Add(signatory);
                                //GenService.Save(text);
                            }
                        }
                    }
                    #endregion
                    if (dto.RemovedSignatories != null)
                    {
                        foreach (var item in dto.RemovedSignatories)
                        {
                            var signatory = GenService.GetById<Disbursment_Signatory>(item);
                            if (signatory != null)
                            {
                                signatory.Status = EntityStatus.Inactive;
                                signatory.EditDate = DateTime.Now;
                                signatory.EditedBy = userId;
                            }
                            signatoryList.Add(signatory);
                        }
                    }
                    dto.CreateDate = entity.CreateDate;
                    dto.CreatedBy = entity.CreatedBy;
                    dto.Status = entity.Status;
                    dto.ProposalId = entity.ProposalId;
                    dto.EditDate = DateTime.Now;
                    if (entity.ParentId > 0)
                    {
                        dto.ParentId = entity.ParentId;
                        dto.TotalDisbursedAmount = (entity.ParentMemo.TotalDisbursedAmount +
                                                    dto.CurrentDisbursementAmount);
                    }
                    else
                    {
                        dto.TotalDisbursedAmount = dto.CurrentDisbursementAmount;
                    }

                    if (entity.Proposal.RecomendedLoanAmountFromIPDC != null && entity.Proposal.RecomendedLoanAmountFromIPDC < dto.TotalDisbursedAmount)
                    {
                        response.Message = "Total disbursement amount exceeds total loan amount.";
                        return response;
                    }
                    entity = Mapper.Map(dto, entity);
                    GenService.Save(entity, false);
                    GenService.Save(signatoryList);
                    GenService.SaveChanges();
                    response.Id = entity.Id;
                    response.Success = true;
                    response.Message = "Disbursment Memo Edited Successfully";
                }
                #endregion
                #region Add
                else
                {
                    var entity = Mapper.Map<DisbursementMemo>(dto);
                    entity.EditDate = DateTime.Now;
                    entity.Status = EntityStatus.Active;
                    entity.CreateDate = DateTime.Now;
                    entity.CreatedBy = userId;
                    if (dto.Texts != null && dto.Texts.Count > 0)
                    {
                        entity.Texts = Mapper.Map<List<DMText>>(dto.Texts);
                        foreach (var item in entity.Texts)
                        {
                            item.CreateDate = DateTime.Now;
                            item.CreatedBy = userId;
                            item.Status = EntityStatus.Active;
                        }
                    }
                    entity.DMNo = _sequencer.GetUpdatedDMNo();

                    var proposal = GenService.GetById<Proposal>((long)entity.ProposalId);
                    if (entity.ParentId > 0)
                    {
                        var parentDisbursementMemo = GenService.GetById<DisbursementMemo>((long)dto.ParentId);
                        if (parentDisbursementMemo != null)
                        {
                            entity.TotalDisbursedAmount = entity.CurrentDisbursementAmount +
                                                          parentDisbursementMemo.TotalDisbursedAmount;
                        }
                    }
                    else
                    {
                        entity.TotalDisbursedAmount = entity.CurrentDisbursementAmount;
                    }
                    if (proposal != null && proposal.RecomendedLoanAmountFromIPDC != null && proposal.RecomendedLoanAmountFromIPDC < dto.TotalDisbursedAmount)
                    {
                        response.Message = "Total disbursement amount exceeds total loan amount.";
                        return response;
                    }
                    if (dto.Signatories != null && dto.Signatories.Count > 0)
                    {
                        entity.Signatories = new List<Disbursment_Signatory>();
                        entity.Signatories = Mapper.Map<List<Disbursment_Signatory>>(dto.Signatories);
                        entity.Signatories = entity.Signatories.Select(d =>
                        {
                            d.Status = EntityStatus.Active;
                            return d;
                        }).ToList();
                    }
                    GenService.Save(entity);
                    response.Id = entity.Id;
                    response.Success = true;
                    response.Message = "Disbursment Memo Saved Successfully";
                }
                #endregion

            }
            catch (Exception ex)
            {
                response.Message = "Error Message : " + ex;
            }
            return response;
        }

        public DisbursementMemoDto LoadDisbursmentMemo(long? proposalId, long? appId, long? id, long? parentId)
        {
            var result = new DisbursementMemoDto();
            if (id != null && id > 0)
            {
                var memoData = GenService.GetAll<DisbursementMemo>().FirstOrDefault(r => r.Id == id);
                if (memoData != null)
                {
                    result = Mapper.Map<DisbursementMemoDto>(memoData);
                    if (memoData.Application != null)
                    {
                        result.AccountTitle = memoData.Application.AccountTitle;
                        result.ApplicattionDate = memoData.Application != null ? memoData.Application.ApplicationDate : DateTime.MinValue;
                        var empId = _user.GetEmployeeIdByUserId((long)memoData.CreatedBy);
                        var emp = GenService.GetById<Employee>(empId);
                        if (emp != null)
                            result.CreatedByName = emp.Person.FirstName + " " + emp.Person.LastName;

                    }

                    if (memoData.Proposal != null)
                    {
                        result.FacilityType = memoData.Proposal.FacilityType != null ? memoData.Proposal.FacilityType : null;
                        result.FacilityTypeName = result.FacilityType > 0 ? UiUtil.GetDisplayName(result.FacilityType) : "";
                        result.BranchName = memoData.Proposal.BranchName;
                        if (result.FacilityType == ProposalFacilityType.Personal_Loan)
                        {
                            var cif = memoData.Application.CIFList.Where(c => c.Status == EntityStatus.Active && c.ApplicantRole == ApplicantRole.Primary && c.CIF_PersonalId != null).FirstOrDefault().CIF_Personal;
                            result.EmployeerName = cif.Occupation.OrganizationId != null ? cif.Occupation.Organization.Name : cif.Occupation.OrganizationName;
                        }

                        result.CRMApprovalDate = memoData.Proposal.ProposalDate;//offerLetter.CRMApprovalDate != null ? offerLetter.CRMApprovalDate : DateTime.MinValue;

                        result.LoanAmount = memoData.Proposal.RecomendedLoanAmountFromIPDC != null ? memoData.Proposal.RecomendedLoanAmountFromIPDC : 0;
                        result.DevelopersName = memoData.Proposal.DevelopersName;
                        result.ProjectName = memoData.Proposal.ProjectName;
                    }
                    if (result.ParentId == null)
                    {
                        var previousDisbursementMemo = GenService.GetAll<DisbursementMemo>().Where(d => d.ApplicationId == appId
                        && d.ProposalId == result.ProposalId
                        && d.IsApproved != null && d.IsApproved == true
                        && d.Status == EntityStatus.Active).OrderByDescending(d => d.Id).FirstOrDefault();
                        if (previousDisbursementMemo != null)
                        {
                            // result.TotalDisbursedAmount = previousDisbursementMemo.TotalDisbursedAmount;
                            result.ParentId = previousDisbursementMemo.Id;
                        }
                    }
                    result.Texts.RemoveAll(f => f.Status != EntityStatus.Active);
                    result.Signatories.RemoveAll(o => o.Status != EntityStatus.Active);
                }
            }
            else if (id == null && parentId > 0)
            {
                var dmInfo = GenService.GetById<DisbursementMemo>((long)parentId);
                var dmMap = Mapper.Map<DisbursementMemoDto>(dmInfo);
                if (dmInfo != null)
                {
                    result.ApplicationId = dmInfo.ApplicationId;
                    result.ProposalId = dmInfo.ProposalId;

                    //var totalAmount = dmInfo.TotalDisbursedAmount;
                    result.TotalLoanAmount = dmInfo.TotalLoanAmount;
                    result.TotalDisbursedAmount = dmInfo.TotalDisbursedAmount;
                    result.ApplicationNo = dmMap.ApplicationNo;
                    result.ProductName = dmMap.ProductName;
                    result.AccountTitle = dmMap.AccountTitle;
                    result.ParentId = parentId;
                }
            }
            else if (appId != null && appId > 0)
            {
                var proposal = GenService.GetAll<Proposal>().Where(r => r.ApplicationId == appId).OrderByDescending(r => r.Id).FirstOrDefault();
                result.ApplicationId = appId;
                result.ProposalId = proposalId != null ? proposalId : proposal.Id;
                //result.Id = id != null ? id : 0;
                var previousDisbursementMemo = GenService.GetAll<DisbursementMemo>()
                    .Where(d => d.ApplicationId == appId
                    && d.ProposalId == result.ProposalId
                    && d.IsApproved != null && d.IsApproved == true
                    && d.Status == EntityStatus.Active).OrderByDescending(d => d.Id).FirstOrDefault();
                if (proposal != null)
                {
                    var totalAmount = proposal.RecomendedLoanAmountFromIPDC;
                    result.TotalLoanAmount = totalAmount != null ? (decimal)totalAmount : 0;
                    if (previousDisbursementMemo != null)
                    {
                        result.TotalDisbursedAmount = previousDisbursementMemo.TotalDisbursedAmount;
                        result.ParentId = previousDisbursementMemo.Id;
                    }
                }
            }
            return result;
        }

        public ResponseDto SaveDclApproval(long id, long? appId, long userId)
        {
            ResponseDto response = new ResponseDto();
            var log = new ApplicationLog();
            long employeeId = _user.GetEmployeeIdByUserId((long)userId);
            long parentEmpId = employeeId != 0 ? _employee.GetEmployeeSupervisorEmpId(employeeId) : 0;
            //long? parentUserId = parentEmpId != 0 ? _user.GetUserByEmployeeId(parentEmpId).Id : 0;
            var dcl = GenService.GetById<DocumentCheckList>(id);
            if (dcl == null)
            {
                response.Message = "DCL record not found";
            }
            else
            {
                if (dcl.IsApproved == true)
                {
                    response.Message = "Already Approved";
                    return response;
                }
                dcl.IsApproved = true;
                if (appId > 0)
                {
                    var application = GenService.GetById<Application>((long)appId);
                    log.Activity = Activity.Submit;
                    log.AppIdRef = appId;
                    log.ApplicationId = (long)appId;
                    log.AppType = AppType.Application;
                    log.CreateDate = DateTime.Now;
                    log.CreatedBy = userId;
                    log.FromUserId = userId;
                    log.FromStage = application.ApplicationStage; // Previous 'SubmittedToOperations';
                    log.ToStage = ApplicationStage.DCLApproved; // Previous 'SubmittedToMemo'
                    log.ToUserId = userId;
                    log.Status = EntityStatus.Active;
                    application.CurrentHolding = null;
                    application.CurrentHoldingEmpId = null;
                    application.ApplicationStage = ApplicationStage.DCLApproved; // Previous 'SubmittedToMemo'
                    GenService.Save(application);
                    GenService.Save(log);

                }
                GenService.Save(dcl);
                GenService.SaveChanges();
                response.Id = dcl.Id;
                response.Success = true;
                response.Message = "Dcl Approval Successful";
                var app = GenService.GetById<Application>((long)appId);
                N.CreateNotificationForService(NotificationType.DCLApprovedForDA, (long)app.Id);

            }
            return response;
        }

        public IPagedList<ApplicationDto> SubmitToDisbursmentMemo(int pageSize, int pageCount, string searchString, long UserId)
        {
            var data = GenService.GetAll<Application>()
                .Where(r => r.Status == EntityStatus.Active &&
                            r.Product.ProductType != null &&
                            r.Product.ProductType == ProductType.Loan &&
                            ((r.Product.FacilityType != null &&
                              r.Product.FacilityType == ProposalFacilityType.Auto_Loan &&
                              r.ApplicationStage == ApplicationStage.POIssued) ||
                             (r.Product.FacilityType != ProposalFacilityType.Auto_Loan &&
                              (r.ApplicationStage == ApplicationStage.DCLApproved ||
                               r.ApplicationStage == ApplicationStage.ApprovedByOperations)))).ToList();
            var application = (from app in data
                               join dcl in GenService.GetAll<DocumentCheckList>().Where(r => r.Status == EntityStatus.Active).OrderByDescending(r => r.Id) on app.Id equals dcl.ApplicationId into check
                               from appDto in check.DefaultIfEmpty()
                               join proposal in GenService.GetAll<Proposal>().Where(r => r.Status == EntityStatus.Active).OrderByDescending(r => r.Id) on app.Id equals proposal.ApplicationId into prp
                               from extra in prp.DefaultIfEmpty()
                               join memo in GenService.GetAll<DisbursementMemo>().Where(r => r.Status == EntityStatus.Active).OrderByDescending(r => r.ApplicationId) on app.Id equals memo.ApplicationId into dm
                               from extr in dm.DefaultIfEmpty()
                               select new ApplicationDto
                               {
                                   Id = app.Id,
                                   ApplicationNo = app.ApplicationNo,
                                   AccountTitle = app.AccountTitle,
                                   FacilityTypeName = app.Product != null ? app.Product.FacilityType.ToString() : "",
                                   ProductName = app.Product != null ? app.Product.Name : "",
                                   AppliedLoanAmount = extra != null ? extra.RecomendedLoanAmountFromIPDC : 0,
                                   DCLNo = appDto != null ? "IPDC/DCL/" + (appDto.FacilityType == ProposalFacilityType.Home_Loan ? "HL/" :
                                                                             appDto.FacilityType == ProposalFacilityType.Auto_Loan ? "AL/" :
                                                                             appDto.FacilityType == ProposalFacilityType.Personal_Loan ? "PL/" : "") + appDto.Application.ApplicationNo : "",
                                   ProposalNo = extra != null ? "IPDC/CM/" + (extra.FacilityType == ProposalFacilityType.Home_Loan ? "HL/" :
                                                                             extra.FacilityType == ProposalFacilityType.Auto_Loan ? "AL/" :
                                                                             extra.FacilityType == ProposalFacilityType.Personal_Loan ? "PL/" : "") + extra.ApplicationNo : "",
                                   //ProposalNo = extra != null ? extra.CreditMemoNo : "",
                                   ProposalId = extra != null ? (long)extra.Id : 0,
                                   DmId = extr != null ? extr.Id : 0,
                                   RMName = app.RMEmp.Person.FirstName + " " + app.RMEmp.Person.LastName
                               });
            var proposalList =
                GenService.GetAll<DisbursementMemo>()
                    .Where(d => d.ProposalId != null && d.Status == EntityStatus.Active)
                    .Select(d => d.ProposalId)
                    .ToList();
            if (!string.IsNullOrEmpty(searchString))
            {
                searchString = searchString.ToLower();
                application = application.Where(a => a.ApplicationNo.ToLower().Contains(searchString) ||
                                                     a.AccountTitle.ToLower().Contains(searchString) ||
                                                     a.ProductName.ToLower().Contains(searchString) ||
                                                     a.RMName.ToLower().Contains(searchString)
                                                ).ToList();

            }
            if (proposalList != null)
                application = application.Where(a => !(proposalList.Contains(a.ProposalId))).ToList();
            var temp = application.OrderByDescending(r => r.Id).ToPagedList(pageCount, pageSize);
            return temp;
        }

        public ResponseDto SaveOprApproval(long? appId, long userId)
        {
            ResponseDto response = new ResponseDto();
            var log = new ApplicationLog();
            if (appId > 0)
            {
                var application = GenService.GetById<Application>((long)appId);
                log.Activity = Activity.Submit;
                log.AppIdRef = appId;
                log.ApplicationId = (long)appId;
                log.AppType = AppType.Application;
                log.CreateDate = DateTime.Now;
                log.CreatedBy = userId;
                log.FromUserId = userId;
                log.FromStage = application.ApplicationStage; // Previous 'SubmittedToMemo'
                application.ApplicationStage = ApplicationStage.ApprovedByOperations; //Previous SubmittedToDisbursment;
                log.ToStage = ApplicationStage.ApprovedByOperations; //Previous SubmittedToDisbursment;
                log.ToUserId = userId;
                log.Status = EntityStatus.Active;
                GenService.Save(application);
                GenService.Save(log);
                GenService.SaveChanges();
                response.Success = true;
                response.Message = "Operation Approval Successful";
                N.CreateNotificationForService(NotificationType.ApplicationReadyForDisbursement, (long)application.Id);
            }
            else
            {
                response.Message = "Operation Approval failed";
            }
            return response;
        }
        public IPagedList<DisbursementMemoDto> PreparedDisbursmentMemo(int pageSize, int pageCount, string searchString, long UserId)
        {
            var data = GenService.GetAll<DisbursementMemo>().Where(r => r.Status == EntityStatus.Active && r.IsApproved != true).ToList();
            var application = (from dm in data
                               select new DisbursementMemoDto
                               {
                                   Id = dm.Id,
                                   ApplicationNo = dm.Application != null ? dm.Application.ApplicationNo : "",
                                   AccountTitle = dm.Application != null ? dm.Application.AccountTitle : "",
                                   FacilityTypeName = dm.Proposal != null ? dm.Proposal.FacilityType != null ? dm.Proposal.FacilityType.ToString() : null : "",
                                   ProductName = dm.Application != null ? dm.Application.Product != null ? dm.Application.Product.Name : null : "",
                                   TotalLoanAmount = dm.TotalLoanAmount,
                                   CurrentDisbursementAmount = dm.CurrentDisbursementAmount,
                                   TotalDisbursedAmount = dm.TotalDisbursedAmount,
                                   ProposalId = dm.ProposalId != null ? dm.ProposalId : 0,
                                   ApplicationId = dm.ApplicationId != null ? dm.ApplicationId : 0
                               });

            if (!string.IsNullOrEmpty(searchString))
                application = application.Where(a => a.ApplicationNo.ToLower().Contains(searchString.ToLower())).ToList();
            var temp = application.OrderBy(r => r.Id).ToPagedList(pageCount, pageSize);
            return temp;
        }

        public ResponseDto SaveMemoApproval(DisbursementMemoDto dto, long userId)
        {
            ResponseDto response = new ResponseDto();
            var log = new ApplicationLog();
            long employeeId = _user.GetEmployeeIdByUserId((long)userId);
            //long parentEmpId = employeeId != 0 ? _employee.GetEmployeeSupervisorEmpId(employeeId) : 0;
            //long? parentUserId = parentEmpId != 0 ? _user.GetUserByEmployeeId(parentEmpId).Id : 0;
            if (dto.Id > 0)
            {
                var memo = GenService.GetById<DisbursementMemo>((long)dto.Id);
                var app = GenService.GetById<Application>((long)memo.ApplicationId);
                memo.IsApproved = true;
                memo.ApprovalDate = dto.ApprovalDate;
                memo.ApprovedByEmpId = employeeId;

                log.Activity = Activity.Submit;
                log.AppIdRef = dto.ApplicationId;
                log.ApplicationId = (long)dto.ApplicationId;
                log.AppType = AppType.Application;
                log.CreateDate = DateTime.Now;
                log.CreatedBy = userId;
                log.FromUserId = userId;
                log.FromStage = app.ApplicationStage; //Previous SubmittedToDisbursment;
                app.ApplicationStage = ApplicationStage.ReadyForDeisbursement;
                log.ToStage = ApplicationStage.ReadyForDeisbursement; //Previous SubmittedToDisbursment;
                log.ToUserId = userId;
                log.Status = EntityStatus.Active;
                try
                {
                    if (memo.ParentId == null)
                    {
                        GenService.Save(app);
                        GenService.Save(log);
                    }
                    GenService.Save(memo);
                    GenService.SaveChanges();
                    response.Success = true;
                    response.Message = "Disbursment Memo Approval Successful";
                    N.CreateNotificationForService(NotificationType.ApplicationReadyForDisbursement, (long)app.Id);
                }
                catch (Exception ex) { }
            }
            else
            {
                response.Message = "Disbursment Memo Approval failed";
            }
            return response;
        }
        public IPagedList<DisbursementMemoDto> PartiallyDisbursedList(int pageSize, int pageCount, string searchString, long UserId)
        {
            var memoList = new List<DisbursementMemoDto>();
            var proposalList =
                GenService.GetAll<DisbursementMemo>()
                    .Where(d => d.ProposalId != null && d.Status == EntityStatus.Active && d.IsApproved != false && d.IsDisbursed != false && d.Application.ApplicationStage >= 0)
                    .Select(d => d.ProposalId).Distinct()
                    .ToList();
            foreach (var proposalId in proposalList)
            {
                var data = GenService.GetAll<DisbursementMemo>().Where(r => r.Status == EntityStatus.Active && r.IsApproved == true && r.ProposalId == proposalId).OrderByDescending(r => r.Id).FirstOrDefault();
                if (data != null && data.TotalDisbursedAmount < data.TotalLoanAmount)
                {
                    memoList.Add(new DisbursementMemoDto
                    {
                        Id = data.Id,
                        ApplicationNo = data.Application != null ? data.Application.ApplicationNo : "",
                        AccountTitle = data.Application != null ? data.Application.AccountTitle : "",
                        FacilityTypeName = data.Proposal != null ? data.Proposal.FacilityType != null ? data.Proposal.FacilityType.ToString() : null : "",
                        ProductName = data.Application != null ? data.Application.Product != null ? data.Application.Product.Name : null : "",
                        TotalLoanAmount = data.TotalLoanAmount,
                        CurrentDisbursementAmount = data.CurrentDisbursementAmount,
                        TotalDisbursedAmount = data.TotalDisbursedAmount,
                        ProposalId = data.ProposalId != null ? data.ProposalId : 0,
                        ApplicationId = data.ApplicationId != null ? data.ApplicationId : 0
                    });
                }
            }
            if (!string.IsNullOrEmpty(searchString))
                memoList = memoList.Where(a => a.ApplicationNo.ToLower().Contains(searchString.ToLower())
                || a.AccountTitle.ToLower().Contains(searchString.ToLower())
                || a.ProductName.ToLower().Contains(searchString.ToLower())).ToList();
            memoList = (from memo in memoList
                        group memo by memo.ProposalId into joined
                        select new DisbursementMemoDto
                        {
                            ApplicationNo = joined.FirstOrDefault().ApplicationNo,
                            AccountTitle = joined.FirstOrDefault().AccountTitle,
                            FacilityTypeName = joined.FirstOrDefault().FacilityTypeName,
                            ProductName = joined.FirstOrDefault().ProductName,
                            TotalLoanAmount = joined.FirstOrDefault().TotalLoanAmount,
                            TotalDisbursedAmount = joined.Sum(q => q.CurrentDisbursementAmount),
                            ProposalId = joined.Key,
                            ApplicationId = joined.FirstOrDefault().ApplicationId
                        }).ToList();
            var temp = memoList.OrderByDescending(r => r.Id).ToPagedList(pageCount, pageSize);
            return temp;
        }
        public IPagedList<DisbursementMemoDto> DisbursedLoanApplication(int pageSize, int pageCount, string searchString, long UserId)
        {
            var memoList = new List<DisbursementMemo>();
            var proposalList =
                GenService.GetAll<DisbursementMemo>()
                    .Where(d => d.ProposalId != null && d.Status == EntityStatus.Active && d.IsApproved != false && d.IsDisbursed != false)
                    .Select(d => d.ProposalId).Distinct()
                    .ToList();
            foreach (var proposalId in proposalList)
            {
                var data = GenService.GetAll<DisbursementMemo>().Where(r => r.Status == EntityStatus.Active && r.IsApproved == true && r.ProposalId == proposalId).OrderByDescending(r => r.Id).FirstOrDefault();
                if (data != null && data.TotalDisbursedAmount == data.TotalLoanAmount)
                {
                    memoList.Add(Mapper.Map<DisbursementMemo>(data));
                }
            }
            if (!string.IsNullOrEmpty(searchString))
                memoList = memoList.Where(a => a.DMNo.ToLower().Contains(searchString.ToLower())).ToList();
            var memoDto = (from memo in memoList.OrderByDescending(m => m.Id)
                           group memo by memo.ProposalId into joined
                           join dcls in GenService.GetAll<DocumentCheckList>().Where(d => d.Status == EntityStatus.Active) on joined.Key equals dcls.ProposalId into usableDcl
                           from dcl in usableDcl.DefaultIfEmpty()
                           select new DisbursementMemoDto
                           {
                               ApplicationNo = joined.FirstOrDefault().Application.ApplicationNo,
                               AccountTitle = joined.FirstOrDefault().Application.AccountTitle,

                               ProductName = joined.FirstOrDefault().Application.Product.Name,
                               TotalLoanAmount = joined.FirstOrDefault().TotalLoanAmount,
                               TotalDisbursedAmount = joined.Sum(q => q.CurrentDisbursementAmount),
                               ProposalId = joined.Key,
                               ApplicationId = joined.FirstOrDefault().Application.Id,
                               CreditMemoNo = joined.FirstOrDefault().Proposal.CreditMemoNo,
                               DisbursedDate = (joined.FirstOrDefault().DisbursementDetails != null && joined.FirstOrDefault().DisbursementDetails.Count > 0) ? joined.FirstOrDefault().DisbursementDetails.FirstOrDefault().CreateDate : null,
                               CBSAccNo = joined.FirstOrDefault().Application.LoanApplication.CBSAccountNo,
                               DCLNo = dcl != null ? dcl.DCLNo : ""
                           }).ToList();
            var temp = memoDto.OrderBy(r => r.Id).ToPagedList(pageCount, pageSize);
            return temp;
        }

        public IPagedList<DisbursementMemoDto> PreparedDisbursedList(int pageSize, int pageCount, string searchString, long userId)
        {
            var memoList = new List<DisbursementMemoDto>();
            var proposalList =
                GenService.GetAll<DisbursementMemo>()
                    .Where(d => d.ProposalId != null && d.Status == EntityStatus.Active && d.IsApproved != true && d.IsDisbursed != true && d.Application.ApplicationStage >= 0)
                    .Select(d => d.ProposalId).Distinct()
                    .ToList();
            foreach (var proposalId in proposalList)
            {
                var data = GenService.GetAll<DisbursementMemo>().Where(r => r.Status == EntityStatus.Active && r.ProposalId == proposalId).OrderByDescending(r => r.Id).FirstOrDefault();
                if (data != null)
                {
                    memoList.Add(new DisbursementMemoDto
                    {
                        Id = data.Id,
                        ApplicationNo = data.Application != null ? data.Application.ApplicationNo : "",
                        AccountTitle = data.Application != null ? data.Application.AccountTitle : "",
                        FacilityTypeName = data.Proposal != null ? data.Proposal.FacilityType != null ? data.Proposal.FacilityType.ToString() : null : "",
                        ProductName = data.Application != null ? data.Application.Product != null ? data.Application.Product.Name : null : "",
                        TotalLoanAmount = data.TotalLoanAmount,
                        CurrentDisbursementAmount = data.CurrentDisbursementAmount,
                        TotalDisbursedAmount = data.TotalDisbursedAmount,
                        ProposalId = data.ProposalId != null ? data.ProposalId : 0,
                        ApplicationId = data.ApplicationId != null ? data.ApplicationId : 0
                    });
                }
            }
            if (!string.IsNullOrEmpty(searchString))
                memoList = memoList.Where(a => a.ApplicationNo.ToLower().Contains(searchString.ToLower())
                || a.AccountTitle.ToLower().Contains(searchString.ToLower())
                || a.ProductName.ToLower().Contains(searchString.ToLower())).ToList();
            var temp = memoList.OrderByDescending(r => r.Id).ToPagedList(pageCount, pageSize);
            return temp;
        }
        //public ResponseDto SaveDisbursmentMemoApproval(DisbursementMemoDto dto, long userId)
        //{
        //    ResponseDto response = new ResponseDto();
        //    var log = new ApplicationLog();
        //    long employeeId = _user.GetEmployeeIdByUserId((long)userId);
        //    long parentEmpId = employeeId != 0 ? _employee.GetEmployeeSupervisorEmpId(employeeId) : 0;

        //    //long? parentUserId = parentEmpId != 0 ? _user.GetUserByEmployeeId(parentEmpId).Id : 0;
        //    if (dto.Id > 0)
        //    {
        //        var memo = GenService.GetById<DisbursementMemo>((long)dto.Id);
        //        var app = GenService.GetById<Application>((long)memo.ApplicationId);
        //        memo.IsDisbursed = true;
        //        memo.DisbursedDate = dto.ApprovalDate;
        //        memo.DisbursedByEmpId = employeeId;
        //        GenService.Save(memo);
        //        log.Activity = Activity.Submit;
        //        log.AppIdRef = dto.ApplicationId;
        //        log.ApplicationId = (long)dto.ApplicationId;
        //        log.AppType = AppType.Application;
        //        log.CreateDate = DateTime.Now;
        //        log.CreatedBy = userId;
        //        log.FromUserId = userId;
        //        log.FromStage = app.ApplicationStage;
        //        if (memo.TotalDisbursedAmount == memo.TotalLoanAmount)
        //        {
        //            log.ToStage = ApplicationStage.DisbursementComplete;
        //            app.ApplicationStage = ApplicationStage.DisbursementComplete;
        //        }
        //        else
        //        {
        //            log.ToStage = ApplicationStage.PartialDisbursementComplete;
        //            app.ApplicationStage = ApplicationStage.PartialDisbursementComplete;
        //        }
        //        log.ToUserId = userId;
        //        log.Status = EntityStatus.Active;
        //        GenService.Save(app);
        //        GenService.Save(log);
        //        GenService.SaveChanges();
        //        response.Success = true;
        //        response.Message = "Disbursment Disbursed Approval Successful";
        //    }
        //    else
        //    {
        //        response.Message = "Disbursment Disbursed Approval failed";
        //    }
        //    return response;
        //}
        public IPagedList<DisbursementMemoDto> ApprovedDisbursedList(int pageSize, int pageCount, string searchString, long userId)
        {
            var memoList = new List<DisbursementMemoDto>();
            var proposalList =
                GenService.GetAll<DisbursementMemo>()
                    .Where(d => d.ProposalId != null && d.Status == EntityStatus.Active && d.IsApproved == true && d.IsDisbursed != true && d.Application.ApplicationStage >= 0)
                    .Select(d => d.ProposalId).Distinct()
                    .ToList();
            foreach (var proposalId in proposalList)
            {
                var data = GenService.GetAll<DisbursementMemo>().Where(r => r.Status == EntityStatus.Active && r.ProposalId == proposalId).OrderByDescending(r => r.Id).FirstOrDefault();
                if (data != null)
                {
                    memoList.Add(Mapper.Map<DisbursementMemoDto>(data));
                }
            }
            if (!string.IsNullOrEmpty(searchString))
                memoList = memoList.Where(a => a.DMNo.ToLower().Contains(searchString.ToLower())
                || a.ApplicationNo.ToLower().Contains(searchString.ToLower())
                || a.AccountTitle.ToLower().Contains(searchString.ToLower())
                || a.ProductName.ToLower().Contains(searchString.ToLower())).ToList();
            var temp = memoList.OrderByDescending(r => r.Id).Select(t => new DisbursementMemoDto
            {
                ApplicationId = t.ApplicationId,
                ApplicationNo = t.ApplicationNo,
                AccountTitle = t.AccountTitle,
                FacilityTypeName = t.FacilityTypeName,
                ProductName = t.ProductName,
                LoanAmount = t.LoanAmount,
                TotalLoanAmount = t.TotalLoanAmount,
                TotalDisbursedAmount = t.TotalDisbursedAmount,
                CurrentDisbursementAmount = t.CurrentDisbursementAmount,
                ProposalId = t.ProposalId,
                DMNo = t.DMNo,
                CreditMemoNo = t.CreditMemoNo,
                Id = t.Id
            }).ToPagedList(pageCount, pageSize);
            return temp;
        }
        public ResponseDto SaveDisbursmentMemoDetails(DisbursementMemoDto dto, long userId)
        {
            ResponseDto responce = new ResponseDto();
            var dm = new DisbursementMemo();
            long employeeId = _user.GetEmployeeIdByUserId((long)userId);
            if (dto.Id > 0)
            {
                dm = GenService.GetById<DisbursementMemo>((long)dto.Id);
            }
            foreach (var detail in dto.DisbursementDetails)
            {
                //if (detail.Id > 0)
                //{
                if (detail.Id != null && detail.Id > 0)
                {
                    #region old Entry
                    DMDetail oldEntries = GenService.GetById<DMDetail>((long)detail.Id);
                    detail.CreatedBy = oldEntries.CreatedBy;
                    detail.CreateDate = oldEntries.CreateDate;
                    if (detail.Status == null)
                        detail.Status = oldEntries.Status;
                    Mapper.Map(detail, oldEntries);
                    oldEntries.EditDate = DateTime.Now;
                    oldEntries.EditedBy = userId;

                    using (var tran = new TransactionScope())
                    {
                        try
                        {
                            GenService.Save(oldEntries);
                            GenService.SaveChanges();
                            responce.Success = true;
                            tran.Complete();
                        }
                        catch (Exception ex)
                        {
                            tran.Dispose();

                            responce.Message = "DM Detail Save Failed for ";
                            return responce;
                        }
                        responce.Message = "DM Details Edited Successfully...";
                    }
                    //return responce;
                    //GenService.Save(oldEntries);
                    #endregion
                }

                else
                {
                    #region New Entry
                    var entity = Mapper.Map<DMDetail>(detail);
                    entity.CreateDate = DateTime.Now;
                    entity.CreatedBy = userId;
                    entity.Status = EntityStatus.Active;

                    using (var tran = new TransactionScope())
                    {
                        try
                        {

                            GenService.Save(entity);
                            GenService.SaveChanges();
                            tran.Complete();

                        }
                        catch (Exception ex)
                        {
                            tran.Dispose();
                            responce.Message = "DM Detail Save Failed for ";
                            return responce;
                        }
                        responce.Message = "DM Details Saved Successfully...";
                    }
                    //return responce;
                    #endregion
                }
                //return responce;
            }
            #region list deletes

            var removedDtl = dto.RemovedDMDetails;
            if (removedDtl != null)
            {
                foreach (var item in removedDtl)
                {
                    var dtl = GenService.GetById<FundConfirmationDetail>(item);
                    if (dtl != null)
                    {
                        dtl.Status = EntityStatus.Inactive;
                        dtl.EditDate = DateTime.Now;
                        dtl.EditedBy = userId;
                    }
                    GenService.Save(dtl);
                }
            }
            #endregion
            if (dm != null)
            {
                dm.IsDisbursed = true;
                var application = GenService.GetById<Application>((long)dm.ApplicationId);
                var log = new ApplicationLog();
                log.Activity = Activity.Submit;
                log.AppIdRef = dm.Application.Id;
                log.ApplicationId = dm.Application.Id;
                log.AppType = AppType.Application;
                log.CreateDate = DateTime.Now;
                log.CreatedBy = userId;
                log.FromUserId = userId;
                log.FromStage = dm.Application.ApplicationStage;

                log.Status = EntityStatus.Active;
                if (dm.TotalDisbursedAmount == dm.TotalLoanAmount)
                {
                    if (application != null)
                    {
                        application.ApplicationStage = ApplicationStage.DisbursementComplete;
                        log.ToStage = ApplicationStage.DisbursementComplete;
                    }
                }
                else
                {
                    if (application != null)
                    {
                        application.ApplicationStage = ApplicationStage.PartialDisbursementComplete;
                        log.ToStage = ApplicationStage.PartialDisbursementComplete;
                    }
                }
                //dm.DisbursedDate =;
                dm.DisbursedByEmpId = employeeId;
                GenService.Save(dm);
                GenService.Save(application);
                GenService.Save(log);
                N.CreateNotificationForService(NotificationType.ApplicationFullyPartialyDisbursement, (long)application.Id);
                GenService.SaveChanges();
            }
            //responce.Message = "DM Detail Saved Successfully";
            return responce;
        }

        public DisbursementMemoDto LoadDMDetails(long dmId)
        {
            if (dmId > 0)
            {
                var data = GenService.GetById<DisbursementMemo>(dmId);
                var result = Mapper.Map<DisbursementMemoDto>(data);
                result.DisbursementDetails.RemoveAll(f => f.Status != EntityStatus.Active);
                return result;
            }
            return null;
        }
        public ApplicationDetailDto LoadApplicationByAppIdForCRM(long appId, long userId)
        {
            List<ApplicationCIFsDto> cif = new List<ApplicationCIFsDto>();
            List<CIF_Org_OwnersDto> owners = new List<CIF_Org_OwnersDto>();

            try
            {
                var app = GenService.GetById<Application>(appId);
                if (app != null)
                {
                    var proposal =
                        GenService.GetAll<Proposal>()
                            .Where(i => i.ApplicationId == app.Id)
                            .OrderByDescending(r => r.ApplicationId)
                            .FirstOrDefault();
                    var data = Mapper.Map<ApplicationDetailDto>(app);
                    data.LoanApplication = Mapper.Map<LoanApplicationDto>(app.LoanApplication);
                    if (data.LoanApplication != null)
                    {
                        if (data.LoanApplication.Guarantors != null)
                        {
                            data.LoanApplication.Guarantors = data.LoanApplication.Guarantors.Where(g => g.Status == EntityStatus.Active).ToList();
                            data.LoanApplication.Guarantors.ForEach(z => z.ApplicationId = appId);
                        }
                    }
                    foreach (var applicationCiFse in app.CIFList.Where(x => x.Status == EntityStatus.Active))
                    {
                        var refce = Mapper.Map<List<CIF_ReferenceDto>>(applicationCiFse.CIF_Personal.References.Where(r => r.Status == EntityStatus.Active)).ToList();
                        data.References.AddRange(refce);

                        if (applicationCiFse.CIF_Organizational != null && applicationCiFse.CIF_Organizational.Owners != null)
                        {
                            owners.AddRange(Mapper.Map<List<CIF_Org_OwnersDto>>(applicationCiFse.CIF_Organizational.Owners.Where(o => o.Status == EntityStatus.Active)));
                        }
                    }
                    owners.ForEach(z => z.ApplicationId = appId);
                    foreach (var applicationCiFsDto in data.CIFList)
                    {
                        //var test = GenService.GetAll<CIF_IncomeStatement>().Where(r => r.CIF_PersonalId == applicationCiFsDto.CIF_PersonalId && r.Status == EntityStatus.Active).OrderByDescending(r => r.Id).FirstOrDefault();

                        if (proposal != null && proposal.Incomes != null)
                        {
                            var cifIncome = proposal.Incomes.Where(r => r.IsConsidered && r.CIFNo == applicationCiFsDto.CIFNo).ToList();
                            if (cifIncome.Any())
                            {
                                applicationCiFsDto.MonthlyIncome = cifIncome.Sum(t => t.ConsideredAmount);
                            }

                        }
                        //applicationCiFsDto.ProfessionName = 
                        cif.Add(applicationCiFsDto);
                    }
                    if (data.LoanApplication != null)
                    {
                        var consumer =
                            GenService.GetAll<ConsumerGoodsPrimarySecurity>().Where(C => C.Status == EntityStatus.Active).OrderByDescending(C => C.Id).FirstOrDefault(r => r.LoanApplicationId == data.LoanApplication.Id && data.LoanApplication.LoanPrimarySecurityType == LoanPrimarySecurityType.ConsumerGoodsPrimarySecurity);
                        //var vehicle =
                        //    GenService.GetAll<VehiclePrimarySecurity>().Where(C => C.Status == EntityStatus.Active).OrderByDescending(C => C.Id).OrderByDescending(C => C.Id).FirstOrDefault(r => r.LoanApplicationId == data.LoanApplication.Id && data.LoanApplication.LoanPrimarySecurityType == LoanPrimarySecurityType.VehiclePrimarySecurity);
                        var vehicle =
                            GenService.GetAll<VehiclePrimarySecurity>().FirstOrDefault(r => r.LoanApplicationId == data.LoanApplication.Id && r.LoanApplication.LoanPrimarySecurityType == LoanPrimarySecurityType.VehiclePrimarySecurity && r.Status == EntityStatus.Active);
                        var fdr =
                            GenService.GetAll<FDRPrimarySecurity>().Where(C => C.Status == EntityStatus.Active).OrderByDescending(C => C.Id).OrderByDescending(C => C.Id).FirstOrDefault(r => r.LoanApplicationId == data.LoanApplication.Id && data.LoanApplication.LoanPrimarySecurityType == LoanPrimarySecurityType.FDRPrimarySecurity);
                        var lpSecurity =
                            GenService.GetAll<LPPrimarySecurity>().Where(C => C.Status == EntityStatus.Active).OrderByDescending(C => C.Id).OrderByDescending(C => C.Id).FirstOrDefault(r => r.LoanApplicationId == data.LoanApplication.Id && data.LoanApplication.LoanPrimarySecurityType == LoanPrimarySecurityType.LPPrimarySecurity);
                        if (consumer != null)
                        {
                            data.LoanApplication.ConsumerGoodsPrimarySecurity = Mapper.Map<ConsumerGoodsPrimarySecurityDto>(consumer);
                        }
                        else if (vehicle != null)
                        {
                            data.LoanApplication.VehiclePrimarySecurity = Mapper.Map<VehiclePrimarySecurityDto>(vehicle);
                            var verification = GenService.GetAll<VehiclePrimarySecurityValuation>()
                                .Where(t => t.Status == EntityStatus.Active && t.VerificationState == VerificationState.Verified && t.VehiclePrimarySecurityId == vehicle.Id)
                                .OrderByDescending(r => r.Id); //.Select(r => r.Id);
                            if (verification.Any())
                            {
                                data.LoanApplication.VehiclePrimarySecurity.VerificationId = verification.FirstOrDefault().VerificationState;
                            }
                            else
                            {
                                data.LoanApplication.VehiclePrimarySecurity.VerificationId = VerificationState.Pending;
                            }
                        }
                        else if (fdr != null)
                        {
                            data.LoanApplication.FDRPrimarySecurity = Mapper.Map<FDRPrimarySecurityDto>(fdr);
                            data.LoanApplication.FDRPrimarySecurity.FDRPSDetails.RemoveAll(f => f.Status != EntityStatus.Active);
                        }
                        else if (lpSecurity != null)
                        {
                            data.LoanApplication.LPPrimarySecurity = Mapper.Map<LPPrimarySecurityDto>(lpSecurity);
                            if (lpSecurity.Valuations.Count > 0)
                            {
                                data.LoanApplication.LPPrimarySecurity.Valuations = data.LoanApplication.LPPrimarySecurity.Valuations.Where(s => s.Status == EntityStatus.Active).ToList();
                                data.LoanApplication.LPPrimarySecurity.Valuations.ForEach(s => s.Remarks = data.LoanApplication.Purpose != null ? data.LoanApplication.Purpose : "");
                                if (data.LoanApplication.LPPrimarySecurity.Valuations != null)
                                {
                                    data.LoanApplication.LPPrimarySecurity.Valuations.ForEach(c => { c.ProjectId = lpSecurity.ProjectId != null ? (long)lpSecurity.ProjectId : 0; });
                                    if (lpSecurity.ProjectId != null)
                                    {
                                        var legaldoc = GenService.GetAll<LegalDocumentVerification>().Where(r => r.ProjectId == lpSecurity.ProjectId && r.Status == EntityStatus.Active).OrderByDescending(l => l.Id).FirstOrDefault();
                                        if (legaldoc != null)
                                        {
                                            data.LoanApplication.LPPrimarySecurity.Valuations.ForEach(c => { c.LegalDocId = legaldoc.Id != null ? (long)legaldoc.Id : 0; });
                                        }
                                        var technical = GenService.GetAll<ProjectTechnicalVerification>().Where(r => r.ProjectId == lpSecurity.ProjectId && r.Status == EntityStatus.Active).OrderByDescending(l => l.Id).FirstOrDefault();
                                        if (technical != null)
                                        {
                                            data.LoanApplication.LPPrimarySecurity.Valuations.ForEach(c => { c.ProjectTechnicalId = technical.Id != null ? (long)technical.Id : 0; });
                                        }
                                        var legal = GenService.GetAll<ProjectLegalVerification>().Where(r => r.ProjectId == lpSecurity.ProjectId && r.Status == EntityStatus.Active).OrderByDescending(l => l.Id).FirstOrDefault();
                                        if (legal != null)
                                        {
                                            data.LoanApplication.LPPrimarySecurity.Valuations.ForEach(c => { c.ProjectLegalId = legal.Id != null ? (long)legal.Id : 0; });
                                        }
                                    }

                                }
                            }
                        }
                        if (proposal != null)
                        {
                            data.LoanApplication.ServiceChargeAmount = proposal.ProcessingFeeAndDocChargesAmount;
                            data.BranchName = proposal.BranchName;
                            data.RMName = proposal.RMName;
                            data.LoanApplication.LoanAmountApplied = proposal.RecomendedLoanAmountFromIPDC;
                            data.LoanApplication.Rate = proposal.InterestRateOffered;
                            data.LoanApplication.Term = proposal.AppliedLoanTerm;
                            //data.LeadPriorityName = proposal
                        }
                        //return data;
                    }

                    data.CIFList = cif;
                    var res = GenService.GetAll<Proposal>().Where(i => i.ApplicationId == appId && i.Status == EntityStatus.Active).OrderByDescending(r => r.Id).FirstOrDefault();
                    if (res != null)
                        data.ProposalId = res.Id;
                    data.OwnerList = owners;
                    data.IsSelfSubmitted = app.CurrentHolding != null ? app.CurrentHolding == userId ? true : false : false;
                    if (data.ProposalId != null || data.ProposalId > 0)
                    {
                        var firstOrDefault = GenService.GetAll<OfferLetter>()
                            .Where(t => t.ProposalId == data.ProposalId).OrderByDescending(u => u.Id).FirstOrDefault();
                        if (firstOrDefault != null)
                            data.OfferLetterId = firstOrDefault.Id;
                    }
                    var disbursment = GenService.GetAll<DisbursementMemo>()
                           .Where(t => t.ApplicationId == appId).OrderByDescending(u => u.Id).FirstOrDefault();
                    if (disbursment != null)
                        data.DmId = disbursment.Id;

                    // For Operation Approval Loan Application DCL Report Print

                    var docCeckList = GenService.GetAll<DocumentCheckList>().FirstOrDefault(d => d.ApplicationId == appId && d.Status == EntityStatus.Active);
                    if (docCeckList != null)
                        data.DclId = docCeckList.Id;
                    // For Operation Approval Loan Application DCL Report Print

                    return data;
                }
                else
                    return null;
            }
            catch (Exception ex)
            {
                return null;
            }

        }
    }
}
