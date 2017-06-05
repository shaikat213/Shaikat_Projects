using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Finix.IPDC.DTO;
using Finix.IPDC.Infrastructure;
using Finix.IPDC.Infrastructure.Models;
using Microsoft.Practices.ObjectBuilder2;
using System.Globalization;
using System.IO;
using System.Net;
using System.Transactions;
using System.Web;
using Finix.IPDC.Util;

namespace Finix.IPDC.Facade
{
    public class VerificationFacade : BaseFacade
    {
        private readonly Auth.Facade.UserFacade _user = new Auth.Facade.UserFacade();
        private readonly CIFFacade _cifFacade = new CIFFacade();
        private readonly EmployeeFacade _employee = new EmployeeFacade();
        public LoanApplication GetPrimarySecurityById(long id)
        {
            throw new NotImplementedException();
        }
        public ResponseDto SaveLPVerification(LPPrimarySecurityValuationDto valuation, long userId)
        {
            ResponseDto response = new ResponseDto();
            long empId = _user.GetEmployeeIdByUserId(userId);
            try
            {
                if (valuation.Id > 0)
                {
                    var prev = GenService.GetAll<LPPrimarySecurityValuation>().Where(r => r.LPPrimarySecurityId == valuation.LPPrimarySecurityId && r.Status == EntityStatus.Active).ToList();
                    prev.ForEach(c => { c.Status = EntityStatus.Inactive; });
                    GenService.Save(prev);
                }
                var data = Mapper.Map<LPPrimarySecurityValuation>(valuation);
                data.Id = 0;
                data.VerificationDate = DateTime.Now;
                data.EmpId = empId;
                data.Status = EntityStatus.Active;
                data.CreateDate = DateTime.Now;
                data.Status = EntityStatus.Active;
                GenService.Save(data);
                response.Success = true;
                response.Message = "LPP Primary Security Valuation Saved Successfully";
            }
            catch (Exception ex)
            {
                response.Message = "Error Message : " + ex;
            }
            return response;
        }
        public ResponseDto SaveCIBPersonal(CIB_PersonalDto dto, long userId)
        {
            ResponseDto response = new ResponseDto();
            string filePath = "~/UploadedFiles/CIBReports/";
            CIF_Personal cif = new CIF_Personal();
            if (dto.CIF_PersonalId > 0)
            {
                cif = GenService.GetById<CIF_Personal>((long)dto.CIF_PersonalId);
                if (!string.IsNullOrEmpty(cif.CIFNo))
                {
                    filePath += cif.CIFNo + "/";
                }
            }
            string path = HttpContext.Current.Server.MapPath(filePath);
            string file = "";
            var empId = _user.GetEmployeeIdByUserId(userId);
            try
            {
                if (dto.CIBReportFile != null)
                {
                    if (!Directory.Exists(path))
                        Directory.CreateDirectory(path);

                    string customfileName = "CIB_Report_" + cif.CIFNo + DateTime.Now.ToString("_yyyy_MM_dd_001", CultureInfo.InvariantCulture);
                    string fileExt = Path.GetExtension(dto.CIBReportFile.FileName);
                    path = Path.Combine(path, customfileName + fileExt);
                    for (int i = 1; File.Exists(path);)
                    {
                        var length = path.Length;
                        if (!string.IsNullOrEmpty(fileExt))
                            path = path.Substring(0, (length - 3 - fileExt.Length)) + (++i).ToString("000") + fileExt;
                        else
                            path = path.Substring(0, (length - 3)) + (++i).ToString("000");
                    }
                    dto.CIBReportFile.SaveAs(path);
                    file = path;
                }

                var entity = new CIB_Personal();
                if (dto.Id != null && dto.Id > 0)
                {
                    entity = GenService.GetById<CIB_Personal>((long)dto.Id);
                    if (string.IsNullOrEmpty(file) && !string.IsNullOrEmpty(entity.CIBReport))
                    {
                        file = entity.CIBReport;
                    }
                    dto.EmpId = entity.EmpId;
                    dto.CreateDate = entity.CreateDate;
                    dto.CreatedBy = entity.CreatedBy;
                    entity = Mapper.Map(dto, entity);
                    if (!string.IsNullOrEmpty(file))
                    {
                        entity.CIBReport = file;
                    }

                    entity.EditDate = DateTime.Now;
                    entity.EditedBy = userId;
                    entity.Status = EntityStatus.Active;
                    GenService.Save(entity);
                    response.Success = true;
                    response.Message = "CIB Personal Information Edited Successfully";
                }
                else
                {
                    entity = Mapper.Map<CIB_Personal>(dto);
                    entity.CIBReport = file;
                    entity.Status = EntityStatus.Active;
                    entity.EmpId = empId;
                    entity.CreatedBy = userId;
                    entity.CreateDate = DateTime.Now;
                    GenService.Save(entity);
                    response.Id = entity.Id;
                    response.Success = true;
                    response.Message = "CIB Personal Information Saved Successfully";
                }

            }
            catch (Exception ex)
            {
                response.Message = "Error Message : " + ex;
            }

            return response;
        }
        public CIB_PersonalDto LoadCIBPersonal(long? AppId, long? CIFPId, long? CibId)
        {
            var cib = new CIB_PersonalDto();
            if (CibId > 0 && AppId > 0 && CIFPId > 0)
            {
                var cibData = GenService.GetAll<CIB_Personal>().FirstOrDefault(r => r.Id == CibId && r.ApplicationId == AppId && r.CIF_PersonalId == CIFPId && r.Status == EntityStatus.Active);
                var application = GenService.GetById<Application>((long)AppId);
                cib = Mapper.Map<CIB_PersonalDto>(cibData);
                var cif = GenService.GetById<CIF_Personal>((long)CIFPId);
                if (cibData != null)
                {
                    cib.ApplicationId = cibData.ApplicationId;
                    cib.ApplicationNo = cibData.Application.ApplicationNo;
                    cib.AccountTitle = cibData.Application.AccountTitle;
                    cib.CIF_PersonalId = cibData.CIF_PersonalId;
                    cib.CIFNo = cibData.CIF.CIFNo;
                    cib.CIFName = cibData.CIF.Title + " " + cibData.CIF.Name;
                    cib.CIFFathersName = cibData.CIF.FathersTitle + " " + cibData.CIF.FathersName;
                    cib.CIFMothersName = cibData.CIF.MothersTitle + " " + cibData.CIF.MothersName;
                    cib.AppliedAmount = application.LoanApplicationId != null ? application.LoanApplication.LoanAmountApplied : 0;
                    cib.DateOfBirth = cibData.CIF.DateOfBirth;
                    cib.BirthDistrictName = cibData.CIF.BirthDistrictId != null ? cibData.CIF.BirthDistrict.DistrictNameEng : cibData.CIF.BirthDistrictForeign;
                    cib.NIDNo = cibData.CIF.NIDNo;
                    cib.PassportNo = cibData.CIF.PassportNo;
                    cib.DLNo = cibData.CIF.DLNo;
                    cib.BirthRegNo = cif.BirthRegNo;
                    cib.SmartNIDNo = cif.SmartNIDNo;
                    cib.PassportIssueCountryId = cif.PassportIssueCountryId;
                    cib.PassportIssueCountryName = cif.PassportIssueCountry != null ? cif.PassportIssueCountry.Name : "";
                    cib.PassportIssueDate = cif.PassportIssueDate;
                    if (cibData.CIF.PermanentAddressId != null)
                    {
                        cib.PermanentAddress = !string.IsNullOrEmpty(cibData.CIF.PermanentAddress.AddressLine1) ? cibData.CIF.PermanentAddress.AddressLine1 : "" +
                              (!string.IsNullOrEmpty(cibData.CIF.PermanentAddress.AddressLine2) ? (", " + cibData.CIF.PermanentAddress.AddressLine2) : "")
                              + (!string.IsNullOrEmpty(cibData.CIF.PermanentAddress.AddressLine3) ? (", " + cibData.CIF.PermanentAddress.AddressLine3) : "")
                              + (cibData.CIF.PermanentAddress.ThanaId != null ? (", " + cibData.CIF.PermanentAddress.Thana.ThanaNameEng) : "")
                              + (cibData.CIF.PermanentAddress.DistrictId != null ? (", " + cibData.CIF.PermanentAddress.District.DistrictNameEng) : "")
                              + (cibData.CIF.PermanentAddress.DivisionId != null ? (", " + cibData.CIF.PermanentAddress.Division.DivisionNameEng) : "")
                              + (cibData.CIF.PermanentAddress.CountryId != null ? (", " + cibData.CIF.PermanentAddress.Country.Name) : "");
                    }
                    return cib;
                }
            }
            else if (AppId > 0 && CIFPId > 0)
            {
                var cif = GenService.GetById<CIF_Personal>((long)CIFPId);
                var application = GenService.GetById<Application>((long)AppId);
                cib.ApplicationId = application.Id;
                cib.ApplicationNo = application.ApplicationNo;
                cib.AccountTitle = application.AccountTitle;
                cib.CIF_PersonalId = cif.Id;
                cib.CIFNo = cif.CIFNo;
                cib.CIFName = cif.Title + " " + cif.Name;
                cib.CIFFathersName = cif.FathersTitle + " " + cif.FathersName;
                cib.CIFMothersName = cif.MothersTitle + " " + cif.MothersName;
                cib.AppliedAmount = application.LoanApplicationId != null ? application.LoanApplication.LoanAmountApplied : 0;
                cib.DateOfBirth = cif.DateOfBirth;
                cib.BirthDistrictName = cif.BirthDistrictId != null ? cif.BirthDistrict.DistrictNameEng : cif.BirthDistrictForeign;
                cib.NIDNo = cif.NIDNo;
                cib.PassportNo = cif.PassportNo;
                cib.DLNo = cif.DLNo;
                cib.BirthRegNo = cif.BirthRegNo;
                cib.SmartNIDNo = cif.SmartNIDNo;
                cib.PassportIssueCountryId = cif.PassportIssueCountryId;
                cib.PassportIssueCountryName = cif.PassportIssueCountry != null ? cif.PassportIssueCountry.Name : "";
                cib.PassportIssueDate = cif.PassportIssueDate;
                if (cif.PermanentAddressId != null)
                {
                    cib.PermanentAddress = (!string.IsNullOrEmpty(cif.PermanentAddress.AddressLine1) ? cif.PermanentAddress.AddressLine1 : "") +
                          (!string.IsNullOrEmpty(cif.PermanentAddress.AddressLine2) ? (", " + cif.PermanentAddress.AddressLine2) : "")
                          + (!string.IsNullOrEmpty(cif.PermanentAddress.AddressLine3) ? (", " + cif.PermanentAddress.AddressLine3) : "")
                          + (cif.PermanentAddress.ThanaId != null ? (", " + cif.PermanentAddress.Thana.ThanaNameEng) : "")
                          + (cif.PermanentAddress.DistrictId != null ? (", " + cif.PermanentAddress.District.DistrictNameEng) : "")
                          + (cif.PermanentAddress.DivisionId != null ? (", " + cif.PermanentAddress.Division.DivisionNameEng) : "")
                          + (cif.PermanentAddress.CountryId != null ? (", " + cif.PermanentAddress.Country.Name) : "");
                }
            }
            return cib;
        }
        public CIBVerificationHistoryDto LoadCibHistory(long? AppId, long CIFPId, int? CibType)
        {
            CIBVerificationHistoryDto cibVerification = new CIBVerificationHistoryDto();
            var cibPersonalList = GenService.GetAll<CIB_Personal>().Where(r => r.ApplicationId == AppId && r.CIF_PersonalId == CIFPId && r.Status == EntityStatus.Active);
            var cibOrgList = GenService.GetAll<CIB_Organizational>().Where(r => r.ApplicationId == AppId && r.CIF_OrgId == CIFPId && r.Status == EntityStatus.Active);
            cibVerification.CIBPersonal = new List<CIB_PersonalDto>();
            cibVerification.CIBOrganizational = new List<CIB_OrganizationalDto>();
            if (CibType == 1)
            {
                var cif = GenService.GetById<CIF_Personal>(CIFPId);
                cibVerification.CifNo = cif.CIFNo;
                cibVerification.CIFName = cif.Title + " " + cif.Name;

                var data = (from cib in cibPersonalList
                            select new CIB_PersonalDto
                            {
                                Id = cib.Id,
                                ApplicationId = cib.ApplicationId,
                                ApplicationNo = cib.Application.ApplicationNo,
                                CIF_PersonalId = cib.CIF_PersonalId,
                                VerificationPersonRole = cib.VerificationPersonRole,
                                VerificationDate = cib.VerificationDate

                            }).AsEnumerable().Select(a => new CIB_PersonalDto
                            {
                                Id = a.Id,
                                ApplicationId = a.ApplicationId,
                                ApplicationNo = a.ApplicationNo,
                                CIF_PersonalId = a.CIF_PersonalId,
                                VerificationPersonRole = a.VerificationPersonRole,
                                VerificationPersonRoleName = a.VerificationPersonRole.ToString(),
                                VerificationDate = a.VerificationDate,
                                VerificationDateTxt = a.VerificationDate?.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture) ?? ""
                            }).ToList();

                cibVerification.CIBPersonal = data;
            }
            else if (CibType == 2)
            {
                var cif = GenService.GetById<CIF_Organizational>(CIFPId);
                cibVerification.CifNo = cif.CIFNo;
                cibVerification.CIFName = cif.CompanyName;
                var data = (from chk in cibOrgList
                            select new CIB_OrganizationalDto
                            {
                                ApplicationId = chk.ApplicationId,
                                ApplicationNo = chk.Application.ApplicationNo,
                                CIF_OrgId = chk.CIF_OrgId,
                                VerificationPersonRole = chk.VerificationPersonRole,
                                VerificationDate = chk.VerificationDate

                            }).AsEnumerable().Select(a => new CIB_OrganizationalDto
                            {
                                ApplicationId = a.ApplicationId,
                                ApplicationNo = a.ApplicationNo,
                                CIF_OrgId = a.CIF_OrgId,
                                VerificationPersonRole = a.VerificationPersonRole,
                                VerificationPersonRoleName = a.VerificationPersonRole.ToString(),
                                VerificationDate = a.VerificationDate,
                                VerificationDateTxt = a.VerificationDate?.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture) ?? ""
                            }).ToList();

                cibVerification.CIBOrganizational = data;
            }
            return cibVerification;
        }
        public ResponseDto SaveCIBOrganizational(CIB_OrganizationalDto dto, long userId)
        {
            ResponseDto response = new ResponseDto();
            var empId = _user.GetEmployeeIdByUserId(userId);
            try
            {
                var entity = new CIB_Organizational();
                if (dto.Id != null && dto.Id > 0)
                {
                    entity = GenService.GetById<CIB_Organizational>((long)dto.Id);
                    dto.EmpId = entity.EmpId;
                    dto.CreateDate = entity.CreateDate;
                    dto.CreatedBy = entity.CreatedBy;
                    entity = Mapper.Map(dto, entity);
                    entity.EditDate = DateTime.Now;
                    entity.EditedBy = userId;
                    entity.Status = EntityStatus.Active;
                    GenService.Save(entity);
                    response.Success = true;
                    response.Message = "CIB Organizational Information Edited Successfully";
                }
                else
                {
                    entity = Mapper.Map<CIB_Organizational>(dto);
                    entity.EmpId = empId;
                    entity.Status = EntityStatus.Active;
                    entity.CreatedBy = userId;
                    entity.CreateDate = DateTime.Now;
                    GenService.Save(entity);
                    response.Id = entity.Id;
                    response.Success = true;
                    response.Message = "CIB Organizational Information Saved Successfully";
                }

            }
            catch (Exception ex)
            {
                response.Message = "Error Message : " + ex;
            }

            return response;
        }
        public CIB_OrganizationalDto LoadCIBOrganizational(long? AppId, long? CifOrgId, long? CibOrgId)
        {
            var result = new CIB_OrganizationalDto();

            if (CibOrgId > 0 && AppId > 0 && CifOrgId > 0)
            {
                var cifData = GenService.GetAll<CIB_Organizational>().FirstOrDefault(r => r.Id == CibOrgId && r.ApplicationId == AppId && r.CIF.Id == CifOrgId && r.Status == EntityStatus.Active);
                result = Mapper.Map<CIB_OrganizationalDto>(cifData);
                if (cifData != null)
                {
                    result.CIF_OrgId = cifData.CIF_OrgId;
                    result.CifOrgNo = cifData.CIF.CIFNo;
                    result.CifOrgName = cifData.CIF.CompanyName;
                    result.ApplicationId = cifData.ApplicationId;
                    result.ApplicationNo = cifData.Application.ApplicationNo;
                    result.AccountTitle = cifData.Application.AccountTitle;
                    return result;
                }

            }
            else if (AppId != null && AppId > 0 && CifOrgId != null && CifOrgId > 0)
            {

                var app = GenService.GetById<Application>((long)AppId);
                var cifOrg = GenService.GetById<CIF_Organizational>((long)CifOrgId);

                result.CIF_OrgId = (long)CifOrgId;
                result.ApplicationId = (long)AppId;
                result.CifOrgName = cifOrg.CompanyName;//.Title + " " + cif.Name;
                result.CifOrgNo = cifOrg.CIFNo;
                result.ApplicationId = app.Id;
                result.ApplicationNo = app.ApplicationNo;
                result.AccountTitle = app.AccountTitle;

            }
            return result;
        }
        public ResponseDto SaveVisitReport(VisitReportDto dto, long userId)
        {
            dto.VisitedById = _user.GetEmployeeIdByUserId(userId);
            ResponseDto response = new ResponseDto();
            string filePath = "~/UploadedFiles/VisitReports/";
            var empId = _user.GetEmployeeIdByUserId(userId);

            try
            {
                Application app = new Application();
                if (dto.ApplicationId > 0)
                {
                    app = GenService.GetById<Application>((long)dto.ApplicationId);
                    if (app.BranchId > 0 && app.BranchOffice != null)
                    {
                        filePath += app.BranchOffice.Name + "/";
                    }
                    if (app.ApplicationDate > DateTime.MinValue)
                    {
                        filePath += app.ApplicationDate.Year + "/" + CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(app.ApplicationDate.Month) + "/";
                    }
                    if (!string.IsNullOrEmpty(app.ApplicationNo))
                    {

                        filePath += app.ApplicationNo + "/";
                    }
                }
                CIF_Personal cif = new CIF_Personal();
                if (dto.CIFId > 0)
                {
                    cif = GenService.GetById<CIF_Personal>((long)dto.CIFId);
                    if (!string.IsNullOrEmpty(cif.CIFNo))
                    {
                        filePath += cif.CIFNo + "/";
                    }
                }
                string path = HttpContext.Current.Server.MapPath(filePath);
                string file = "";
                if (dto.VisitReportFile != null)
                {
                    if (!Directory.Exists(path))
                        Directory.CreateDirectory(path);

                    string customfileName = "Visit_Report_" + cif.CIFNo + DateTime.Now.ToString("_yyyy_MM_dd_001", CultureInfo.InvariantCulture);
                    string fileExt = Path.GetExtension(dto.VisitReportFile.FileName);
                    path = Path.Combine(path, customfileName + fileExt);
                    for (int i = 1; File.Exists(path);)
                    {
                        var length = path.Length;
                        if (!string.IsNullOrEmpty(fileExt))
                            path = path.Substring(0, (length - 3 - fileExt.Length)) + (++i).ToString("000") + fileExt;
                        else
                            path = path.Substring(0, (length - 3)) + (++i).ToString("000");
                    }
                    dto.VisitReportFile.SaveAs(path);
                    file = path;
                }
                if (dto.Id > 0)
                {
                    var prev = GenService.GetById<VisitReport>(dto.Id);
                    if (dto.OfficeAddress.IsChanged)
                    {
                        if (dto.OfficeAddress.Id != null)
                        {
                            var address = GenService.GetById<Address>((long)dto.OfficeAddress.Id);
                            dto.OfficeAddress.CreateDate = address.CreateDate;
                            dto.OfficeAddress.CreatedBy = address.CreatedBy;
                            address = Mapper.Map(dto.OfficeAddress, address);
                            GenService.Save(address);
                            dto.OfficeAddressId = address.Id;
                        }
                        else
                        {
                            var officeAddress = Mapper.Map<Address>(dto.OfficeAddress);
                            GenService.Save(officeAddress);
                            dto.OfficeAddressId = officeAddress.Id;
                        }

                    }
                    else
                    {
                        dto.OfficeAddressId = prev.OfficeAddressId;
                    }
                    if (string.IsNullOrEmpty(file) && !string.IsNullOrEmpty(prev.VisitReportPath))
                    {
                        file = prev.VisitReportPath;
                    }
                    dto.CreateDate = prev.CreateDate;
                    dto.EmpId = prev.EmpId;
                    dto.CreatedBy = prev.CreatedBy;
                    dto.Status = prev.Status;
                    dto.EditDate = DateTime.Now;
                    prev = Mapper.Map(dto, prev);
                    if (!string.IsNullOrEmpty(file))
                    {
                        prev.VisitReportPath = file;
                    }
                    GenService.Save(prev);
                    response.Success = true;
                    response.Message = "Visit Report Edited Successfully";
                }
                else
                {
                    var data = Mapper.Map<VisitReport>(dto);
                    var officeAddress = Mapper.Map<Address>(dto.OfficeAddress);
                    if (officeAddress.CountryId != null)
                    {
                        GenService.Save(officeAddress);
                        data.OfficeAddressId = officeAddress.Id;
                    }
                    if (!string.IsNullOrEmpty(file))
                    {
                        data.VisitReportPath = file;
                    }
                    data.Status = EntityStatus.Active;
                    data.EmpId = empId;
                    data.CreateDate = DateTime.Now;
                    GenService.Save(data);
                    response.Success = true;
                    response.Message = "Visit Report Saved Successfully";
                }
            }
            catch (Exception ex)
            {
                response.Message = "Error Message : " + ex;
            }

            return response;
        }
        public VisitReportDto LoadVisitReport(long? AppId, long? CIFPId, long? Id)
        {
            var temp = new VisitReportDto();
            if (Id > 0)
            {
                var data = GenService.GetAll<VisitReport>().FirstOrDefault(d => d.Id == Id && d.Status == EntityStatus.Active);
                if (data != null)
                {
                    temp = Mapper.Map<VisitReportDto>(data);
                    temp.CIFNo = data.CIF.CIFNo;
                    return temp;
                }
            }
            else if (AppId > 0 && CIFPId > 0)
            {
                var cif = GenService.GetById<CIF_Personal>((long)CIFPId);
                temp.CIFId = (long)CIFPId;
                temp.ApplicationId = (long)AppId;
                temp.CIFName = cif.Title + " " + cif.Name;
                temp.CIFNo = cif.CIFNo;
            }
            return temp;
        }
        public List<VisitReportDto> LoadVisitReportHistory(long? applicationId, long cifId)
        {

            var data = GenService.GetAll<VisitReport>().Where(r => r.CIFId == cifId && r.Status == EntityStatus.Active);
            if (applicationId != null)
                data = data.Where(r => r.ApplicationId == applicationId);
            return Mapper.Map<List<VisitReportDto>>(data.ToList());

        }
        public ResponseDto SaveProjectLegalVerification(ProjectLegalVerificationDto dto, long? UserId)
        {
            var entity = new ProjectLegalVerification();
            ResponseDto response = new ResponseDto();
            string filePath = "~/UploadedFiles/ProjectLegal/VerificationFile/";
            string filePathVt = "~/UploadedFiles/ProjectLegal/Vetting/";
            long employeeId = 0;
            if (UserId != null && UserId > 0)
            {
                employeeId = _user.GetEmployeeIdByUserId((long)UserId);
            }
            Project proj = new Project();
            if (dto.ProjectId > 0)
            {
                //proj = GenService.GetById<Project>((long)dto.ProjectId);
                if (dto.ProjectId > 0)//!string.IsNullOrEmpty(proj.ProjectName))
                {
                    filePath += dto.ProjectId + "/";
                    filePathVt += dto.ProjectId + "/";
                }
            }
            //long employeeId = 0;
            //if (UserId != null && UserId > 0)
            //{
            //    employeeId = _user.GetEmployeeIdByUserId((long)UserId);
            //}
            string path = HttpContext.Current.Server.MapPath(filePath);
            string file = "";
            string pathVt = HttpContext.Current.Server.MapPath(filePathVt);
            string fileVt = "";
            if (dto.VettingReportFile != null)
            {
                if (!Directory.Exists(pathVt))
                    Directory.CreateDirectory(pathVt);

                string customfileName = "Project_Legal_Vetting_" + proj.ProjectName + DateTime.Now.ToString("_yyyy_MM_dd_001", CultureInfo.InvariantCulture);
                string fileExt = Path.GetExtension(dto.VettingReportFile.FileName);
                pathVt = Path.Combine(pathVt, customfileName + fileExt);
                for (int i = 1; File.Exists(path);)
                {
                    var length = pathVt.Length;
                    if (!string.IsNullOrEmpty(fileExt))
                        pathVt = pathVt.Substring(0, (length - 3 - fileExt.Length)) + (++i).ToString("000") + fileExt;
                    else
                        pathVt = pathVt.Substring(0, (length - 3)) + (++i).ToString("000");
                }
                dto.VettingReportFile.SaveAs(pathVt);
                fileVt = pathVt;
            }
            if (dto.VerificationReportFile != null)
            {
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);

                string customfileName = "Project_Legal_Verification_" + proj.ProjectName + DateTime.Now.ToString("_yyyy_MM_dd_001", CultureInfo.InvariantCulture);
                string fileExt = Path.GetExtension(dto.VerificationReportFile.FileName);
                path = Path.Combine(path, customfileName + fileExt);
                for (int i = 1; File.Exists(path);)
                {
                    var length = path.Length;
                    if (!string.IsNullOrEmpty(fileExt))
                        path = path.Substring(0, (length - 3 - fileExt.Length)) + (++i).ToString("000") + fileExt;
                    else
                        path = path.Substring(0, (length - 3)) + (++i).ToString("000");
                }
                dto.VerificationReportFile.SaveAs(path);
                file = path;
            }
            if (dto.Id != null && dto.Id > 0)
            {

                entity = GenService.GetById<ProjectLegalVerification>((long)dto.Id);
                if (string.IsNullOrEmpty(file) && !string.IsNullOrEmpty(entity.VerificationReportPath))
                {
                    file = entity.VerificationReportPath;
                }
                if (string.IsNullOrEmpty(fileVt) && !string.IsNullOrEmpty(entity.VettingReportPath))
                {
                    fileVt = entity.VettingReportPath;
                }
                dto.EmployeeId = entity.EmployeeId;
                dto.ProjectId = entity.ProjectId;
                dto.CreateDate = entity.CreateDate;
                dto.CreatedBy = entity.CreatedBy;
                if (dto.Status == null)
                    dto.Status = entity.Status;
                using (var tran = new TransactionScope())
                {
                    try
                    {
                        if (dto.VerificationReportFile ==null && dto.VettingReportFile == null)
                        {
                            Mapper.Map(dto, entity);
                        }
                        if (!string.IsNullOrEmpty(file))
                        {
                            entity.VerificationReportPath = file;
                        }
                        if (!string.IsNullOrEmpty(fileVt))
                        {
                            entity.VettingReportPath = fileVt;
                        }
                        GenService.Save(entity);
                        if (dto.Owners != null)
                        {
                            foreach (var item in dto.Owners)
                            {
                                ProjectPropertyOwner owner;
                                if (item.Id != null && item.Id > 0)
                                {
                                    owner = GenService.GetById<ProjectPropertyOwner>((long)item.Id);
                                    owner.EditDate = DateTime.Now;
                                    owner.EditedBy = UserId;
                                    GenService.Save(owner);
                                }
                                else
                                {
                                    owner = new ProjectPropertyOwner();
                                    owner = Mapper.Map<ProjectPropertyOwner>(item);
                                    owner.Status = EntityStatus.Active;
                                    owner.CreatedBy = UserId;
                                    owner.CreateDate = DateTime.Now;
                                    owner.ProjectLegalId = entity.Id;
                                    GenService.Save(owner);
                                }
                            }
                        }
                        if (dto.RemovedOwners != null)
                        {
                            foreach (var item in dto.RemovedOwners)
                            {
                                var owner = GenService.GetById<ProjectPropertyOwner>(item);
                                if (owner != null)
                                {
                                    owner.Status = EntityStatus.Inactive;
                                    owner.EditDate = DateTime.Now;
                                    owner.EditedBy = UserId;
                                }
                                GenService.Save(owner);
                            }
                        }
                        tran.Complete();
                        response.Id = entity.Id;
                    }
                    catch (Exception ex)
                    {
                        tran.Dispose();
                        response.Message = "Project Legal Verification Edit Operation Failed";
                        return response;
                    }
                }
                response.Success = true;
                response.Message = "Project Legal Verification Edited Successfully";
            }
            else
            {
                if (dto.ProjectId > 0)
                {
                    var project = GenService.GetAll<ProjectLegalVerification>().Where(r => r.Status == EntityStatus.Active && r.ProjectId == dto.ProjectId);
                    if (project != null)
                    {
                        project.ForEach(l => l.Status = EntityStatus.Inactive);
                        GenService.Save(project.ToList());
                    }
                }
                entity = Mapper.Map<ProjectLegalVerification>(dto);
                if (!string.IsNullOrEmpty(file))
                {
                    entity.VerificationReport = file;
                }
                if (!string.IsNullOrEmpty(fileVt))
                {
                    entity.VettingReport = fileVt;
                }
                entity.Status = EntityStatus.Active;
                entity.CreatedBy = UserId;
                entity.CreateDate = DateTime.Now;
                entity.EmployeeId = employeeId;
                using (var tran = new TransactionScope())
                {
                    try
                    {
                        if (dto.Owners != null)
                        {
                            entity.Owners = new List<ProjectPropertyOwner>();
                            foreach (var item in dto.Owners)
                            {
                                item.CreatedBy = UserId;
                                item.CreateDate = DateTime.Now;
                                item.Status = EntityStatus.Active;
                                entity.Owners.Add(Mapper.Map<ProjectPropertyOwner>(item));
                            }
                        }
                        GenService.Save(entity);

                        tran.Complete();
                        response.Id = entity.Id;
                    }
                    catch (Exception ex)
                    {
                        tran.Dispose();
                        response.Message = "Project Legal Verification Save Failed";
                    }
                }

                response.Success = true;
                response.Message = "Project Legal Verification Saved Successfully";
            }
            return response;
        }
        public ProjectLegalVerificationDto LoadProjectLegalVerification(long? ProjectId, long? ProjectLegalId)
        {
            var result = new ProjectLegalVerificationDto();
            if (ProjectLegalId > 0)
            {
                var cifData = GenService.GetAll<ProjectLegalVerification>().FirstOrDefault(r => r.Id == ProjectLegalId && r.Status == EntityStatus.Active);
                if (cifData != null)
                {
                    result = Mapper.Map<ProjectLegalVerificationDto>(cifData);
                    result.Owners.RemoveAll(o => o.Status != EntityStatus.Active);
                    return result;
                }
            }
            else
            {
                var data = GenService.GetAll<ProjectLegalVerification>().Where(r => r.Status == EntityStatus.Active);
                if (ProjectId > 0)
                {
                    data = data.Where(r => r.ProjectId == ProjectId);
                }
                result = Mapper.Map<ProjectLegalVerificationDto>(data.OrderByDescending(r => r.Id).FirstOrDefault());
            }
            return result;
        }
        public ResponseDto SaveVehiclePrimarySecurityValuation(VehiclePrimarySecurityValuationDto valuation, long userId)
        {
            ResponseDto response = new ResponseDto();
            long empId = _user.GetEmployeeIdByUserId(userId);
            try
            {
                if (valuation.Id > 0)
                {
                    var prev = GenService.GetById<VehiclePrimarySecurityValuation>((long)valuation.Id);
                    valuation.CreateDate = prev.CreateDate;
                    valuation.CreatedBy = prev.CreatedBy;
                    valuation.Status = prev.Status;
                    valuation.EmpId = prev.EmpId;
                    valuation.VerifiedByEmpDegMapId = prev.VerifiedByEmpDegMapId;
                    valuation.EditDate = DateTime.Now;
                    prev = Mapper.Map(valuation, prev);
                    GenService.Save(prev);
                    response.Success = true;
                    response.Message = "Vehicle Primary Security Valuation Edited Successfully";
                }
                else
                {
                    var prev = GenService.GetAll<VehiclePrimarySecurityValuation>().Where(r => r.VehiclePrimarySecurityId == valuation.VehiclePrimarySecurityId && r.Status == EntityStatus.Active).ToList();
                    prev.ForEach(c => { c.Status = EntityStatus.Inactive; });
                    GenService.Save(prev);
                    var data = Mapper.Map<VehiclePrimarySecurityValuation>(valuation);
                    if (valuation.VehiclePrimarySecurityId > 0)
                        data.VehiclePrimarySecurityId = (long)valuation.VehiclePrimarySecurityId;
                    //data.VerificationDate = DateTime.Now;
                    //data.VerifiedByEmpDegMapId = data.Use
                    data.EmpId = empId;
                    data.Status = EntityStatus.Active;
                    data.CreatedBy = userId;
                    data.CreateDate = DateTime.Now;
                    GenService.Save(data);
                    response.Success = true;
                    response.Message = "Vehicle Primary Security Valuation Saved Successfully";
                }
            }
            catch (Exception ex)
            {
                response.Message = "Error Message : " + ex;
            }

            return response;
        }
        public VehiclePrimarySecurityValuationDto LoadVehiclePrimarySecurity(long id)
        {

            var result = new VehiclePrimarySecurityValuationDto();
            if (id > 0)
            {
                var data =
                  GenService.GetAll<VehiclePrimarySecurityValuation>()
                      .Where(r => r.VehiclePrimarySecurityId == id && r.Status == EntityStatus.Active)
                      .OrderByDescending(r => r.Id).FirstOrDefault();
                if (data != null)
                {
                    result = Mapper.Map<VehiclePrimarySecurityValuationDto>(data);
                }
                return result;
            }


            return result;
        }
        public LPPrimarySecurityValuationDto LoadLPPrimarySecurityValuation(long id)
        {
            var data =
                GenService.GetAll<LPPrimarySecurityValuation>()
                    .Where(r => r.LPPrimarySecurityId == id && r.Status == EntityStatus.Active)
                    .OrderByDescending(r => r.Id).FirstOrDefault();
            return Mapper.Map<LPPrimarySecurityValuationDto>(data);
        }
        public ApplicationCIFsDto LoadAppCifsData(long? AppId, long? CIFPId)
        {
            var result = new ApplicationCIFsDto();
            if ((AppId != null && AppId > 0) && (CIFPId != null && CIFPId > 0))
            {
                var cifData = GenService.GetAll<ApplicationCIFs>().FirstOrDefault(s => s.ApplicationId == AppId && s.CIF_PersonalId == CIFPId);
                result = Mapper.Map<ApplicationCIFsDto>(cifData);
                if (cifData != null)
                {
                    result.ApplicationId = cifData.ApplicationId;
                    result.ApplicationNo = cifData.Application.ApplicationNo;
                    result.AccountTitle = cifData.Application.AccountTitle;
                    result.CIF_PersonalId = cifData.CIF_PersonalId;
                    result.CIFNo = cifData.CIF_Personal.CIFNo;
                    result.CIFName = cifData.CIF_Personal.Title + " " + cifData.CIF_Personal.Name;
                }
                return result;
            }
            return null;
        }
        public ResponseDto SaveNetWorthVerification(NetWorthVerificationDto dto, long userId)
        {
            ResponseDto responce = new ResponseDto();
            var empId = _user.GetEmployeeIdByUserId(userId);
            if (dto.CIF_PersonalId > 0)
            {
                var entity = new NetWorthVerification();
                if (dto.Id != null && dto.Id > 0)
                {
                    entity = GenService.GetById<NetWorthVerification>((long)dto.Id);
                    dto.EmpId = entity.EmpId;
                    dto.CreateDate = entity.CreateDate;
                    dto.CreatedBy = entity.CreatedBy;
                    dto.Status = EntityStatus.Active;
                    Mapper.Map(dto, entity);
                    using (var tran = new TransactionScope())
                    {
                        try
                        {
                            if (dto.SavingsInBank != null)
                            {
                                foreach (var nwvSavingsInBankDto in dto.SavingsInBank)
                                {
                                    if (nwvSavingsInBankDto.Id > 0)
                                    {
                                        var sib = GenService.GetById<NWV_SavingsInBank>((long)nwvSavingsInBankDto.Id);//entity.Owners.Where(o => o.Id == item.Id).FirstOrDefault();//
                                        nwvSavingsInBankDto.CreateDate = sib.CreateDate;
                                        nwvSavingsInBankDto.CreatedBy = sib.CreatedBy;
                                        nwvSavingsInBankDto.Status = sib.Status;
                                        Mapper.Map(nwvSavingsInBankDto, sib);
                                        sib.NWV_NetWorthId = entity.Id;
                                        sib.EditDate = DateTime.Now;
                                        sib.EditedBy = userId;
                                        entity.SavingsInBank.Add(sib);
                                    }
                                    else
                                    {
                                        nwvSavingsInBankDto.Status = EntityStatus.Active;
                                        nwvSavingsInBankDto.NWV_NetWorthId = entity.Id;
                                        nwvSavingsInBankDto.CreateDate = DateTime.Now;
                                        nwvSavingsInBankDto.CreatedBy = userId;
                                        var savings = Mapper.Map<NWV_SavingsInBank>(nwvSavingsInBankDto);
                                        entity.SavingsInBank.Add(savings);
                                    }
                                }
                            }
                            if (dto.RemovedSavings != null)
                            {
                                foreach (var item in dto.RemovedSavings)
                                {
                                    var sv = GenService.GetById<NWV_SavingsInBank>(item);//entity.Owners.Where(o => o.Id == item).FirstOrDefault();//
                                    if (sv != null)
                                    {
                                        sv.Status = EntityStatus.Inactive;
                                        sv.EditDate = DateTime.Now;
                                        sv.EditedBy = userId;
                                    }
                                    entity.SavingsInBank.Add(sv);
                                }
                            }

                            if (dto.Investments != null)
                            {
                                foreach (var inv in dto.Investments)
                                {
                                    if (inv.Id > 0)
                                    {
                                        var invest = GenService.GetById<NWV_Investment>((long)inv.Id);//entity.Owners.Where(o => o.Id == item.Id).FirstOrDefault();//
                                        inv.CreateDate = invest.CreateDate;
                                        inv.CreatedBy = invest.CreatedBy;
                                        inv.Status = invest.Status;
                                        Mapper.Map(inv, invest);
                                        invest.NWV_NetWorthId = entity.Id;
                                        invest.EditDate = DateTime.Now;
                                        invest.EditedBy = userId;
                                        entity.Investments.Add(invest);
                                    }
                                    else
                                    {
                                        inv.Status = EntityStatus.Active;
                                        inv.NWV_NetWorthId = entity.Id;
                                        inv.CreateDate = DateTime.Now;
                                        inv.CreatedBy = userId;
                                        var investmnt = Mapper.Map<NWV_Investment>(inv);
                                        entity.Investments.Add(investmnt);
                                    }
                                }
                            }
                            if (dto.RemovedInvestments != null)
                            {
                                foreach (var item in dto.RemovedInvestments)
                                {
                                    var inv = GenService.GetById<NWV_Investment>(item);//entity.Owners.Where(o => o.Id == item).FirstOrDefault();//
                                    if (inv != null)
                                    {
                                        inv.Status = EntityStatus.Inactive;
                                        inv.EditDate = DateTime.Now;
                                        inv.EditedBy = userId;
                                    }
                                    GenService.Save(inv);
                                }
                            }
                            if (dto.Properties != null)
                            {
                                foreach (var prop in dto.Properties)
                                {
                                    if (prop.Id > 0)
                                    {
                                        var properties = GenService.GetById<NWV_Property>((long)prop.Id);//entity.Owners.Where(o => o.Id == item.Id).FirstOrDefault();//
                                        prop.CreateDate = properties.CreateDate;
                                        prop.CreatedBy = properties.CreatedBy;
                                        prop.Status = properties.Status;
                                        Mapper.Map(prop, properties);
                                        properties.NWV_NetWorthId = entity.Id;
                                        properties.EditDate = DateTime.Now;
                                        properties.EditedBy = userId;
                                        entity.Properties.Add(properties);
                                    }
                                    else
                                    {
                                        prop.Status = EntityStatus.Active;
                                        prop.NWV_NetWorthId = entity.Id;
                                        prop.CreateDate = DateTime.Now;
                                        prop.CreatedBy = userId;
                                        var property = Mapper.Map<NWV_Property>(prop);
                                        entity.Properties.Add(property);
                                    }
                                }
                            }
                            if (dto.RemovedProperties != null)
                            {
                                foreach (var item in dto.RemovedProperties)
                                {
                                    var prop = GenService.GetById<NWV_Property>(item);//entity.Owners.Where(o => o.Id == item).FirstOrDefault();//
                                    if (prop != null)
                                    {
                                        prop.Status = EntityStatus.Inactive;
                                        prop.EditDate = DateTime.Now;
                                        prop.EditedBy = userId;
                                    }
                                    GenService.Save(prop);
                                }
                            }

                            if (dto.BusinessShares != null)
                            {
                                foreach (var bs in dto.BusinessShares)
                                {
                                    if (bs.Id > 0)
                                    {
                                        var sharebusiness = GenService.GetById<NWV_BusinessShares>((long)bs.Id);//entity.Owners.Where(o => o.Id == item.Id).FirstOrDefault();//
                                        bs.CreateDate = sharebusiness.CreateDate;
                                        bs.CreatedBy = sharebusiness.CreatedBy;
                                        bs.Status = sharebusiness.Status;
                                        Mapper.Map(bs, sharebusiness);
                                        sharebusiness.NWV_NetWorthId = entity.Id;
                                        sharebusiness.EditDate = DateTime.Now;
                                        sharebusiness.EditedBy = userId;
                                        entity.BusinessShares.Add(sharebusiness);
                                    }
                                    else
                                    {
                                        bs.Status = EntityStatus.Active;
                                        bs.NWV_NetWorthId = entity.Id;
                                        bs.CreateDate = DateTime.Now;
                                        bs.CreatedBy = userId;
                                        var property = Mapper.Map<NWV_BusinessShares>(bs);
                                        entity.BusinessShares.Add(property);
                                    }
                                }
                            }
                            if (dto.RemovedShareinBusines != null)
                            {
                                foreach (var item in dto.RemovedShareinBusines)
                                {
                                    var bshare = GenService.GetById<NWV_BusinessShares>(item);//entity.Owners.Where(o => o.Id == item).FirstOrDefault();//
                                    if (bshare != null)
                                    {
                                        bshare.Status = EntityStatus.Inactive;
                                        bshare.EditDate = DateTime.Now;
                                        bshare.EditedBy = userId;
                                    }
                                    GenService.Save(bshare);
                                }
                            }
                            if (dto.Liabilities != null)
                            {
                                foreach (var lb in dto.Liabilities)
                                {
                                    if (lb.Id > 0)
                                    {
                                        var liabilities = GenService.GetById<NWV_Liability>((long)lb.Id);//entity.Owners.Where(o => o.Id == item.Id).FirstOrDefault();//
                                        lb.CreateDate = liabilities.CreateDate;
                                        lb.CreatedBy = liabilities.CreatedBy;
                                        lb.Status = liabilities.Status;
                                        Mapper.Map(lb, liabilities);
                                        liabilities.NWV_NetWorthId = entity.Id;
                                        liabilities.EditDate = DateTime.Now;
                                        liabilities.EditedBy = userId;
                                        entity.Liabilities.Add(liabilities);
                                    }
                                    else
                                    {
                                        lb.Status = EntityStatus.Active;
                                        lb.NWV_NetWorthId = entity.Id;
                                        lb.CreateDate = DateTime.Now;
                                        lb.CreatedBy = userId;
                                        var liability = Mapper.Map<NWV_Liability>(lb);
                                        entity.Liabilities.Add(liability);
                                    }
                                }
                            }
                            if (dto.RemovedLiabilities != null)
                            {
                                foreach (var item in dto.RemovedLiabilities)
                                {
                                    var lib = GenService.GetById<NWV_Liability>(item);//entity.Owners.Where(o => o.Id == item).FirstOrDefault();//
                                    if (lib != null)
                                    {
                                        lib.Status = EntityStatus.Inactive;
                                        lib.EditDate = DateTime.Now;
                                        lib.EditedBy = userId;
                                    }
                                    GenService.Save(lib);
                                }
                            }

                            entity.EditDate = DateTime.Now;
                            GenService.Save(entity);
                            tran.Complete();
                        }
                        catch (Exception ex)
                        {
                            tran.Dispose();
                            responce.Message = "Net Worth Edit Operation Failed";
                        }
                    }
                    responce.Success = true;
                    responce.Message = "Net Worth Edited Successfully";

                }
                else
                {
                    using (var tran = new TransactionScope())
                    {
                        try
                        {
                            var oldEntries = GenService.GetAll<NetWorthVerification>().Where(c => c.NetWorthId == dto.NetWorthId && c.Status == EntityStatus.Active).ToList();
                            oldEntries.ForEach(c =>
                            {
                                c.Status = EntityStatus.Inactive; c.EditDate = DateTime.Now;
                                c.EditedBy = userId;
                            });

                            entity = Mapper.Map<NetWorthVerification>(dto);
                            entity.EmpId = empId;
                            entity.Status = EntityStatus.Active;
                            entity.CreatedBy = userId;
                            entity.CreateDate = DateTime.Now;
                            GenService.Save(entity);
                            GenService.Save(oldEntries);
                            if (dto.SavingsInBank != null)
                            {
                                foreach (var cifSavingsInBankDto in dto.SavingsInBank)
                                {
                                    cifSavingsInBankDto.Status = EntityStatus.Active;
                                    cifSavingsInBankDto.NWV_NetWorthId = entity.Id;
                                    cifSavingsInBankDto.CreatedBy = userId;
                                    var savings = Mapper.Map<NWV_SavingsInBank>(cifSavingsInBankDto);
                                    GenService.Save(savings);
                                }
                            }

                            if (dto.Investments != null)
                            {
                                foreach (var inv in dto.Investments)
                                {
                                    inv.Status = EntityStatus.Active;
                                    inv.NWV_NetWorthId = entity.Id;
                                    inv.CreatedBy = userId;
                                    var investments = Mapper.Map<NWV_Investment>(inv);
                                    GenService.Save(investments);
                                }
                            }

                            if (dto.Properties != null)
                            {
                                foreach (var prop in dto.Properties)
                                {
                                    prop.Status = EntityStatus.Active;
                                    prop.NWV_NetWorthId = entity.Id;
                                    prop.CreatedBy = userId;
                                    var property = Mapper.Map<NWV_Property>(prop);
                                    GenService.Save(property);
                                }
                            }

                            if (dto.BusinessShares != null)
                            {
                                foreach (var bs in dto.BusinessShares)
                                {
                                    bs.Status = EntityStatus.Active;
                                    bs.NWV_NetWorthId = entity.Id;
                                    bs.CreatedBy = userId;
                                    var share = Mapper.Map<NWV_BusinessShares>(bs);
                                    GenService.Save(share);
                                }
                            }

                            if (dto.Liabilities != null)
                            {
                                foreach (var lb in dto.Liabilities)
                                {
                                    lb.Status = EntityStatus.Active;
                                    lb.NWV_NetWorthId = entity.Id;
                                    lb.CreatedBy = userId;
                                    var liability = Mapper.Map<NWV_Liability>(lb);
                                    GenService.Save(liability);
                                }
                            }

                            tran.Complete();
                            responce.Success = true;
                            responce.Message = "Net Worth Verified Successfully";
                        }
                        catch (Exception ex)
                        {
                            tran.Dispose();
                            responce.Message = "Net Worth Verification Failed";
                        }
                    }
                }
            }
            else
            {
                responce.Message = "Net Worth Not Found";
            }
            GenService.SaveChanges();
            return responce;
        }
        public NetWorthVerificationDto LoadNetWorthVerification(long? AppId, long? NetWorthId, long? CifId, long? NWVId)
        {
            var data = GenService.GetAll<NetWorthVerification>().Where(r => r.Status == EntityStatus.Active);
            var result = new NetWorthVerificationDto();
            if (NWVId > 0 && AppId > 0 && CifId > 0)
            {
                var nwvData = GenService.GetAll<NetWorthVerification>().FirstOrDefault(r => r.Id == NWVId && r.ApplicationId == AppId && r.CIF_PersonalId == CifId && r.Status == EntityStatus.Active);
                //var cifData = GenService.GetById<NetWorthVerification>((long)NWVId);
                result = Mapper.Map<NetWorthVerificationDto>(nwvData);
                if (nwvData != null)
                {
                    result.CIF_PersonalId = nwvData.CIF_PersonalId;
                    result.CIFNo = nwvData.CIF.CIFNo;
                    result.CIFName = nwvData.CIF.Title + " " + nwvData.CIF.Name;
                    result.ApplicationId = nwvData.ApplicationId;
                    result.ApplicationNo = nwvData.Application.ApplicationNo;
                    result.AccountTitle = nwvData.Application.AccountTitle;
                    return result;
                }

            }

            if (CifId > 0)
            {
                var nw = new NetWorthVerificationDto();
                var cifInfo = GenService.GetById<CIF_Personal>((long)CifId);
                var netWorth = cifInfo.NetWorths.Where(n => n.Status == EntityStatus.Active).FirstOrDefault();
                if (netWorth != null)
                {
                    var temp = Mapper.Map<CIF_NetWorthDto>(netWorth);
                    result = Mapper.Map<NetWorthVerificationDto>(temp);
                    result.Id = null;
                    if (cifInfo != null)
                    {
                        result.CIFNo = cifInfo.CIFNo;
                        result.CIFName = cifInfo.Name;
                        if (AppId > 0)
                        {
                            var app = GenService.GetById<Application>((long)AppId);
                            result.ApplicationId = app.Id;
                            result.ApplicationNo = app.ApplicationNo;
                            result.AccountTitle = app.AccountTitle;
                        }
                    }
                }
            }


            return result;
        }
        public NetWorthVerificationHistoryDto LoadNWVerificationlHistory(long? AppId, long? NetWorthId, long? CIFPId)
        {
            NetWorthVerificationHistoryDto cibVerification = new NetWorthVerificationHistoryDto();
            var docCheckList = GenService.GetAll<NetWorthVerification>().Where(r => r.CIF_PersonalId == CIFPId); // r.ApplicationId == AppId &&
            cibVerification.NetWorthVerification = new List<NetWorthVerificationDto>();
            var cif = GenService.GetById<CIF_Personal>((long)CIFPId);
            cibVerification.CifNo = cif.CIFNo;
            cibVerification.CIFName = cif.Title + " " + cif.Name;
            var data = (from chk in docCheckList
                        select new NetWorthVerificationDto
                        {
                            ApplicationId = chk.ApplicationId,
                            ApplicationNo = chk.Application.ApplicationNo,
                            CIF_PersonalId = chk.CIF_PersonalId,
                            VerificationPersonRole = chk.VerificationPersonRole,
                            VerificationDate = chk.VerificationDate

                        }).AsEnumerable().Select(a => new NetWorthVerificationDto
                        {
                            ApplicationId = a.ApplicationId,
                            ApplicationNo = a.ApplicationNo,
                            CIF_PersonalId = a.CIF_PersonalId,
                            VerificationPersonRole = a.VerificationPersonRole,
                            VerificationPersonRoleName = a.VerificationPersonRole.ToString(),
                            VerificationDate = a.VerificationDate,
                            VerificationDateTxt = a.VerificationDate != null ? ((DateTime)a.VerificationDate).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture) : ""
                        }).ToList();

            cibVerification.NetWorthVerification = data;
            return cibVerification;
        }
        public ResponseDto SaveConsumerPrimarySecurityValuation(ConsumerGoodsPrimarySecurityValuationDto valuation, long userId)
        {
            ResponseDto response = new ResponseDto();
            long empId = _user.GetEmployeeIdByUserId(userId);
            try
            {
                if (valuation.Id > 0)
                {
                    var prev = GenService.GetById<ConsumerGoodsPrimarySecurityValuation>(valuation.Id);
                    valuation.EmpId = prev.EmpId;
                    valuation.CreateDate = prev.CreateDate;
                    valuation.CreatedBy = prev.CreatedBy;
                    valuation.Status = prev.Status;
                    valuation.VerifiedByEmpDegMapId = prev.VerifiedByEmpDegMapId;
                    valuation.EditDate = DateTime.Now;
                    prev = Mapper.Map(valuation, prev);
                    GenService.Save(prev);
                    response.Success = true;
                    response.Message = "Consumer Primary Security Valuation Edited Successfully";
                }
                else
                {
                    var prev = GenService.GetAll<ConsumerGoodsPrimarySecurityValuation>().Where(r => r.ConsumerGoodsPrimarySecurityId == valuation.ConsumerGoodsPrimarySecurityId && r.Status == EntityStatus.Active).ToList();
                    prev.ForEach(c => { c.Status = EntityStatus.Inactive; });
                    GenService.Save(prev);
                    var data = Mapper.Map<ConsumerGoodsPrimarySecurityValuation>(valuation);
                    data.VerificationDate = DateTime.Now;
                    data.EmpId = empId;
                    data.Status = EntityStatus.Active;
                    data.CreateDate = DateTime.Now;
                    GenService.Save(data);
                    response.Success = true;
                    response.Message = "Consumer Primary Security Valuation Saved Successfully";
                }
            }
            catch (Exception ex)
            {
                response.Message = "Error Message : " + ex;
            }
            return response;
        }
        public ConsumerGoodsPrimarySecurityValuationDto LoadConsumerPrimarySecurity(long id)
        {
            var data =
              GenService.GetAll<ConsumerGoodsPrimarySecurityValuation>()
                  .Where(r => r.ConsumerGoodsPrimarySecurityId == id && r.Status == EntityStatus.Active)
                  .OrderByDescending(r => r.Id).FirstOrDefault();
            return Mapper.Map<ConsumerGoodsPrimarySecurityValuationDto>(data);
        }
        public ResponseDto SaveCpvVerification(ContactPointVerificationDto dto, long userId)
        {
            ResponseDto response = new ResponseDto();
            var empId = _user.GetEmployeeIdByUserId(userId);
            try
            {
                List<BankAccountCpvDto> aCpvDtos = new List<BankAccountCpvDto>();
                List<ReferenceCpvDto> refList = new List<ReferenceCpvDto>();
                if (dto.Id > 0)
                {
                    List<BankAccountCpv> accounts = new List<BankAccountCpv>();
                    List<ReferenceCpv> refferences = new List<ReferenceCpv>();
                    if (dto.References != null)
                        foreach (var referenceCpvDto in dto.References)
                        {
                            if (referenceCpvDto.Id > 0 && referenceCpvDto.Id != null)
                            {
                                var aRef = GenService.GetById<ReferenceCpv>((long)referenceCpvDto.Id);
                                referenceCpvDto.CPVId = aRef.CPVId;
                                referenceCpvDto.CifReferenceId = aRef.CifReferenceId;
                                referenceCpvDto.CreatedBy = aRef.CreatedBy;
                                referenceCpvDto.CreateDate = aRef.CreateDate;
                                aRef = Mapper.Map(referenceCpvDto, aRef);
                                refferences.Add(aRef);
                            }
                        }
                    if (dto.BankAccounts != null)
                        foreach (var banks in dto.BankAccounts)
                        {
                            if (banks.Id > 0 && banks.Id != null)
                            {
                                var aRef = GenService.GetById<BankAccountCpv>((long)banks.Id);
                                banks.CPVId = aRef.CPVId;
                                banks.CreatedBy = aRef.CreatedBy;
                                banks.CreateDate = aRef.CreateDate;
                                aRef = Mapper.Map(banks, aRef);
                                accounts.Add(aRef);
                            }
                        }
                    var prev = GenService.GetById<ContactPointVerification>((long)dto.Id);
                    dto.EmpId = prev.EmpId;
                    dto.CreateDate = prev.CreateDate;
                    dto.CreatedBy = prev.CreatedBy;
                    dto.Status = prev.Status;
                    dto.VerifiedByEmpDegMapId = prev.VerifiedByEmpDegMapId;
                    dto.EditDate = DateTime.Now;
                    prev = Mapper.Map(dto, prev);
                    GenService.Save(prev);
                    GenService.Save(accounts);
                    GenService.Save(refferences);
                    response.Success = true;
                    response.Message = "Contact Point Verification Edited Successfully";
                }
                else
                {
                    //var rerfnc = dto.References;
                    //var bankAccounts = dto.BankAccounts;
                    var data = Mapper.Map<ContactPointVerification>(dto);
                    data.VerificationDate = DateTime.Now;
                    data.Status = EntityStatus.Active;
                    data.EmpId = empId;
                    data.CreatedBy = userId;
                    data.CreateDate = DateTime.Now;
                    GenService.Save(data);
                    try
                    {
                        if (dto.References != null)
                        {
                            dto.References.ForEach(c => { c.Status = EntityStatus.Active; });
                            foreach (var referenceCpvDto in dto.References)
                            {
                                referenceCpvDto.CPVId = data.Id;
                                refList.Add(referenceCpvDto);
                            }
                            var refCpv = Mapper.Map<List<ReferenceCpv>>(refList);
                            GenService.Save(refCpv);
                        }
                        if (dto.BankAccounts != null)
                        {
                            dto.BankAccounts.ForEach(c => { c.Status = EntityStatus.Active; });
                            foreach (var acc in dto.BankAccounts)
                            {
                                acc.CPVId = data.Id;
                                aCpvDtos.Add(acc);
                            }
                            var bankCpv = Mapper.Map<List<BankAccountCpv>>(aCpvDtos);
                            GenService.Save(bankCpv);
                        }
                    }
                    catch (Exception ex)
                    {

                    }


                    response.Success = true;
                    response.Message = "Contact Point Verification Saved Successfully";
                }
            }
            catch (Exception ex)
            {
                response.Message = "Error Message : " + ex;
            }
            return response;
        }
        public ContactPointVerificationDto LoadCpvById(long? appId, long? cifId, long? CpvId)
        {
            var result = new ContactPointVerificationDto();
            if (CpvId > 0)
            {

                var data = GenService.GetById<ContactPointVerification>((long)CpvId);
                result = Mapper.Map<ContactPointVerificationDto>(data);
                if (data.CifPersonal != null)
                {
                    result.PhotoName = data.CifPersonal.Photo != null ? Path.GetFileName(data.CifPersonal.Photo) : "";
                    result.SignaturePhotoName = data.CifPersonal.SignaturePhoto != null ? Path.GetFileName(data.CifPersonal.SignaturePhoto) : "";
                }
                if (data != null)
                {
                    result.CifId = data.CifId;
                    result.CIFNo = data.CifPersonal.CIFNo;
                    result.CIFName = data.CifPersonal.Title + " " + data.CifPersonal.Name;
                    result.ApplicationId = data.ApplicationId;
                    result.ApplicationNo = data.Application.ApplicationNo;
                    result.AccountTitle = data.Application.AccountTitle;
                    return result;
                }

            }
            else if (appId > 0 && cifId > 0)
            {
                result.BankAccounts = new List<BankAccountCpvDto>();
                result.References = new List<ReferenceCpvDto>();
                var application = GenService.GetById<Application>((long)appId);
                var cifPersonal = GenService.GetById<CIF_Personal>((long)cifId);
                if (cifPersonal != null)
                {
                    result.CifId = cifPersonal.Id;
                    result.CIFNo = cifPersonal.CIFNo;
                    result.CIFName = cifPersonal.Title + " " + cifPersonal.Name;
                    result.PhotoName = cifPersonal.Photo != null ? Path.GetFileName(cifPersonal.Photo) : "";
                    result.SignaturePhotoName = cifPersonal.SignaturePhoto != null ? Path.GetFileName(cifPersonal.SignaturePhoto) : "";
                    if (cifPersonal.OccupationId != null)
                        result.ProfessionAssessedId = cifPersonal.Occupation.ProfessionId;
                    var activeBankAccounts = cifPersonal.BankAccounts.Where(b => b.Status == EntityStatus.Active);
                    if (activeBankAccounts != null)
                    {
                        var accounts = activeBankAccounts.Select(x => new BankAccountCpvDto
                        {
                            BankName = x.BankName,
                            AccountNo = x.AccountNo,
                            AccountVerification = VerificationStatus.Not_Verified
                        }).ToList();
                        if (accounts.Count > 0)
                        {
                            result.BankAccounts.AddRange(accounts);
                        }
                    }
                    var activeReference = cifPersonal.References.Where(r => r.Status == EntityStatus.Active);
                    if (activeReference != null)
                    {
                        var reference = activeReference.Select(x => new ReferenceCpvDto
                        {
                            Name = x.Name,
                            CifReferenceId = x.Id,
                            Mobile = VerificationStatus.Not_Verified,
                            Phone = VerificationStatus.Not_Verified,
                            ResidenceStatus = VerificationStatus.Not_Verified,
                            ProfessionalInformation = VerificationStatus.Not_Verified,
                            RelationshipWithApplicant = VerificationStatus.Not_Verified
                        }).ToList();
                        if (reference.Count > 0)
                        {
                            result.References.AddRange(reference);
                        }
                    }
                }
                if (application != null)
                {
                    result.ApplicationId = application.Id;
                    result.ApplicationNo = application.ApplicationNo;
                    result.AccountTitle = application.AccountTitle;
                }
            }
            return result;
        }
        public List<ContactPointVerificationDto> LoadCpvHIstoryById(long? AppId, long CIFPId)
        {
            var data = GenService.GetAll<ContactPointVerification>().Where(r => r.Status == EntityStatus.Active && r.CifId == CIFPId);
            if (AppId > 0)
                data = data.Where(r => r.ApplicationId == AppId);
            var result = Mapper.Map<List<ContactPointVerificationDto>>(data.ToList());
            return result;
        }
        public ResponseDto SaveIncomeVerification(IncomeVerificationDto dto, long userId)
        {
            ResponseDto response = new ResponseDto();
            var empId = _user.GetEmployeeIdByUserId(userId);
            try
            {
                if (dto.Id > 0)
                {
                    var prev = GenService.GetById<IncomeVerification>((long)dto.Id);
                    if (dto.MonthlyOtherIncomesAssessed != null)
                    {
                        IncomeVerificationAdditionalIncomeAssessed additionalIncome;
                        foreach (var item in dto.MonthlyOtherIncomesAssessed)
                        {
                            if (item.Id != null && item.Id > 0)
                            {
                                additionalIncome = GenService.GetById<IncomeVerificationAdditionalIncomeAssessed>((long)item.Id);
                                if (item.Status == null)
                                {
                                    item.Status = additionalIncome.Status;
                                }
                                item.CreateDate = additionalIncome.CreateDate;
                                item.CreatedBy = additionalIncome.CreatedBy;
                                item.AdditionalIncomeDeclaredId = additionalIncome.AdditionalIncomeDeclaredId;
                                item.EditDate = DateTime.Now;
                                item.EditedBy = userId;
                                Mapper.Map(item, additionalIncome);
                                GenService.Save(additionalIncome);
                            }
                            else
                            {
                                additionalIncome = new IncomeVerificationAdditionalIncomeAssessed();
                                item.CreateDate = DateTime.Now;
                                item.CreatedBy = userId;
                                item.Status = EntityStatus.Active;
                                additionalIncome = Mapper.Map<IncomeVerificationAdditionalIncomeAssessed>(item);
                                GenService.Save(additionalIncome);
                            }
                        }
                    }
                    dto.CreateDate = prev.CreateDate;
                    dto.CreatedBy = prev.CreatedBy;
                    dto.EmpId = prev.EmpId;
                    dto.Status = prev.Status;
                    dto.EditDate = DateTime.Now;
                    Mapper.Map(dto, prev);
                    GenService.Save(prev);
                    response.Success = true;
                    response.Message = "Income Verification Edited Successfully";
                }
                else
                {
                    var prev =
                        GenService.GetAll<IncomeVerification>()
                            .Where(
                                r =>
                                    r.CifId == dto.CifId && r.ApplicationId == dto.ApplicationId &&
                                    r.Status == EntityStatus.Active).ToList();
                    prev.ForEach(c => { c.Status = EntityStatus.Inactive; });
                    GenService.Save(prev);
                    var data = Mapper.Map<IncomeVerification>(dto);
                    if (dto.MonthlyOtherIncomesAssessed != null && dto.MonthlyOtherIncomesAssessed.Count > 0)
                    {
                        data.MonthlyOtherIncomesAssessed = new List<IncomeVerificationAdditionalIncomeAssessed>();
                        data.MonthlyOtherIncomesAssessed = Mapper.Map<List<IncomeVerificationAdditionalIncomeAssessed>>(dto.MonthlyOtherIncomesAssessed);
                        data.MonthlyOtherIncomesAssessed = data.MonthlyOtherIncomesAssessed.Select(d => { d.Status = EntityStatus.Active; return d; }).ToList();
                    }
                    data.Status = EntityStatus.Active;
                    data.EmpId = empId;
                    data.CreatedBy = userId;
                    data.CreateDate = DateTime.Now;
                    GenService.Save(data);
                    response.Success = true;
                    response.Message = "Income Verification Saved Successfully";
                }
            }
            catch (Exception ex)
            {
                response.Message = "Error Message : " + ex;
            }
            return response;
        }
        public IncomeVerificationDto LoadIncomeStatementVerification(long? appId, long? CifId, long? IsvId)
        {
            var result = new IncomeVerificationDto();
            if (IsvId != null && IsvId > 0)
            {
                var existingIsv = GenService.GetAll<IncomeVerification>().FirstOrDefault(x => x.Id == IsvId && x.Status == EntityStatus.Active);
                if (existingIsv != null)
                {
                    result = Mapper.Map<IncomeVerificationDto>(existingIsv);
                    result.CifId = existingIsv.Id;
                    result.CifNo = existingIsv.CfPersonal.CIFNo;
                    result.CifName = existingIsv.CfPersonal.Title + " " + existingIsv.CfPersonal.Name;
                    result.ApplicationId = appId;
                    result.ApplicationNo = existingIsv.Application.ApplicationNo;
                    return result;
                }
            }
            else if (appId > 0 && CifId > 0)
            {
                var cif = GenService.GetById<CIF_Personal>((long)CifId);
                var app = GenService.GetById<Application>((long)appId);

                result.CifId = cif.Id;
                result.CifNo = cif.CIFNo;
                result.CifName = cif.Name;
                result.ApplicationId = appId;
                result.ApplicationNo = app.ApplicationNo;


                var cifIncomeStmt = GenService.GetAll<CIF_IncomeStatement>().Where(r => r.CIF_PersonalId == CifId && r.Status == EntityStatus.Active).OrderByDescending(r => r.Id).FirstOrDefault();

                result.IncomeStatement = Mapper.Map<CIF_IncomeStatementDto>(cifIncomeStmt);
                result.MonthlyOtherIncomesAssessed = new List<IncomeVerificationAdditionalIncomeAssessedDto>();
                if (cifIncomeStmt != null && cifIncomeStmt.MonthlyOtherIncomesDeclared != null)
                {
                    result.MonthlyOtherIncomesAssessed =
                        cifIncomeStmt.MonthlyOtherIncomesDeclared.Where(m => m.Status == EntityStatus.Active).Select(
                            x => new IncomeVerificationAdditionalIncomeAssessedDto
                            {
                                IncomeVerificationId = 0,
                                AdditionalIncomeDeclaredId = x.Id,
                                IsConsidered = true,
                                SourceOfIncome = x.SourceOfIncome,
                                IncomeAmount = x.IncomeAmount
                            }).ToList();
                    result.MonthlySalaryAssessedConsidered = true;
                    result.MonthlyInterestIncomeAssessedConsidered = true;
                    result.MonthlyRentalIncomeAssessedConsidered = true;
                    result.MonthlyBusinessIncomeAssessedConsidered = true;
                    result.MonthlyIncomeTotalAssessedConsidered = true;
                }
            }
            return result;
        }
        public List<IncomeVerificationDto> LoadIncomeHistory(long? applicationId, long cifId)
        {

            var data = GenService.GetAll<IncomeVerification>().Where(r => r.CifId == cifId && r.Status == EntityStatus.Active);
            if (applicationId != null)
                data = data.Where(r => r.ApplicationId == applicationId);
            return Mapper.Map<List<IncomeVerificationDto>>(data.ToList());

        }
        public ResponseDto SaveNidVerification(NIDVerificationDto dto, long userId)
        {
            ResponseDto response = new ResponseDto();
            var empId = _user.GetEmployeeIdByUserId(userId);
            try
            {
                if (dto.Id > 0)
                {
                    var prev = GenService.GetById<NIDVerification>(dto.Id);
                    dto.CreateDate = prev.CreateDate;
                    dto.EmpId = prev.EmpId;
                    dto.CreatedBy = prev.CreatedBy;
                    dto.Status = prev.Status;
                    dto.VerifiedByEmpDegMapId = prev.VerifiedByEmpDegMapId;
                    dto.EditDate = DateTime.Now;
                    prev = Mapper.Map(dto, prev);
                    GenService.Save(prev);
                    response.Success = true;
                    response.Message = "NId Verification Edited Successfully";
                }
                else
                {
                    var prev =
                        GenService.GetAll<NIDVerification>()
                            .Where(
                                r =>
                                    r.CifId == dto.CifId && r.ApplicationId == dto.ApplicationId &&
                                    r.Status == EntityStatus.Active).ToList();
                    prev.ForEach(c => { c.Status = EntityStatus.Inactive; });
                    GenService.Save(prev);
                    var data = Mapper.Map<NIDVerification>(dto);
                    data.EmpId = empId;
                    data.Status = EntityStatus.Active;
                    data.CreatedBy = userId;
                    data.CreateDate = DateTime.Now;
                    GenService.Save(data);
                    response.Success = true;
                    response.Message = "NId Verification Saved Successfully";
                }
            }
            catch (Exception ex)
            {
                response.Message = "Error Message : " + ex;
            }
            return response;
        }
        public NIDVerificationDto LoadNidVerification(long? applicationId, long? cifId, long? isnId)
        {
            var result = new NIDVerificationDto();
            if (isnId > 0)
            {
                var data = GenService.GetAll<NIDVerification>().FirstOrDefault(x => x.Id == isnId && x.Status == EntityStatus.Active);
                if (data != null)
                {
                    result = Mapper.Map<NIDVerificationDto>(data);
                    result.ApplicationId = applicationId;
                    result.ApplicationNo = data.Application.ApplicationNo;
                    result.CifId = data.CIF.Id;
                    result.CIFNo = data.CIF.CIFNo;
                    result.Name = data.CIF.Title + " " + data.CIF.Name;
                    result.NIDNo = data.NIDNo;
                    result.DateOfBirthText = data.DateOfBirth.ToString();
                    result.Name = data.CIF.Name;
                    return result;
                }
            }
            else if (cifId != null && cifId > 0 && applicationId != null && applicationId > 0)
            {
                var cif = GenService.GetById<CIF_Personal>((long)cifId);
                var application = GenService.GetById<Application>((long)applicationId);
                result.ApplicationId = applicationId;
                result.ApplicationNo = application.ApplicationNo;
                result.CifId = cif.Id;
                result.CIFNo = cif.CIFNo;
                result.Name = cif.Title + " " + cif.Name;
                result.NIDNo = cif.NIDNo;
                result.DateOfBirthText = cif.DateOfBirth.ToString();
                result.Name = cif.Title + " " + cif.Name;
                if (cif.DateOfBirth != null)
                    result.DateOfBirth = (DateTime)cif.DateOfBirth;
                result.NIDNo = cif.NIDNo;
            }
            return result;
        }
        public List<NIDVerificationDto> LoadNIDHIstoryById(long? appId, long cifId)
        {
            var data = GenService.GetAll<NIDVerification>().Where(r => r.CifId == cifId && r.Status == EntityStatus.Active);
            if (appId != null)
                data = data.Where(r => r.ApplicationId == appId);
            return Mapper.Map<List<NIDVerificationDto>>(data.ToList());

        }
        public List<ApplicantCIFVerifications> GetCIFPVerificationHistory(long? ApplicationId, long CifPersonalId)
        {
            var resultList = new List<ApplicantCIFVerifications>();

            Application app;
            app = GenService.GetById<Application>((long)ApplicationId);

            #region cpv
            if (app != null && app.Product.FacilityType != ProposalFacilityType.RLS)
            {
                var cpvList = GenService.GetAll<ContactPointVerification>().Where(c => c.CifId == CifPersonalId && c.Status == EntityStatus.Active && c.ApplicationId == app.Id).OrderByDescending(c => c.VerificationDate);
                var cpvCount = cpvList.Count();
                var cpv = new ApplicantCIFVerifications();
                cpv.VerificationType = "CPV";
                if (cpvCount > 0)
                {
                    var lastCpv = cpvList.FirstOrDefault();
                    cpv.Count = cpvCount;
                    if (lastCpv != null)
                    {
                        long employeeId = 0;
                        if (lastCpv.VerifiedByUserId != null)
                        {
                            employeeId = _user.GetEmployeeIdByUserId((long)lastCpv.VerifiedByUserId);
                        }
                        if (employeeId > 0)
                        {
                            var employee = _employee.GetEmployeeByEmployeeId(employeeId);
                            if (employee != null)
                            {
                                cpv.VerifierName = employee.Name;
                            }
                        }
                        cpv.LatestVerificationId = lastCpv.Id;
                        cpv.LatestVerificationDate = lastCpv.VerificationDate;
                        cpv.LatestVerificationDateText = lastCpv.VerificationDate != null ? ((DateTime)lastCpv.VerificationDate).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture) : "";
                        cpv.VerificationStatus = lastCpv.VerificationStatus;
                        cpv.VerificationStatusName = lastCpv.VerificationStatus.ToString();
                        cpv.LatestApplicationNo = lastCpv.Application.ApplicationNo;

                    }

                    if (ApplicationId != null && ApplicationId > 0)
                    {
                        var cpvForThisApp = cpvList.Where(c => c.ApplicationId == ApplicationId).FirstOrDefault();
                        if (cpvForThisApp != null)
                        {
                            cpv.VerificationStatusForThisApplication = cpvForThisApp.VerificationStatus;
                            cpv.VerificationStatusForThisApplicationName = cpvForThisApp.VerificationStatus.ToString();
                            cpv.LatestForThisApplicationId = cpvForThisApp.Id;
                            cpv.ApplicationNo = cpvForThisApp.Application.ApplicationNo;
                            cpv.VerificationDate = cpvForThisApp.VerificationDate;
                            cpv.VerificationDateText = cpvForThisApp.VerificationDate != null ? ((DateTime)cpvForThisApp.VerificationDate).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture) : "";

                        }
                    }
                }
                cpv.EditUrl = "/IPDC/Verification/CPV";
                resultList.Add(cpv);
            }
            #endregion

            #region cib
            var cibList = GenService.GetAll<CIB_Personal>().Where(c => c.CIF_PersonalId == CifPersonalId && c.Status == EntityStatus.Active && c.ApplicationId == app.Id).OrderByDescending(c => c.VerificationDate);
            var cibCount = cibList.Count();

            var cib = new ApplicantCIFVerifications();
            cib.VerificationType = "CIB";
            if (cibCount > 0)
            {
                var lastcib = cibList.FirstOrDefault();
                cib.Count = cibCount;
                if (lastcib != null)
                {
                    long employeeId = 0;
                    if (lastcib.VerifiedByUserId != null)
                    {
                        employeeId = _user.GetEmployeeIdByUserId((long)lastcib.VerifiedByUserId);
                    }
                    if (employeeId > 0)
                    {
                        var employee = _employee.GetEmployeeByEmployeeId(employeeId);
                        if (employee != null)
                        {
                            cib.VerifierName = employee.Name;
                        }
                    }
                    cib.LatestVerificationId = lastcib.Id;
                    cib.LatestVerificationDate = lastcib.VerificationDate;
                    cib.LatestVerificationDateText = lastcib.VerificationDate != null ? ((DateTime)lastcib.VerificationDate).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture) : "";
                    cib.LatestApplicationNo = lastcib.Application.ApplicationNo;
                }

                if (ApplicationId != null && ApplicationId > 0)
                {
                    var cibForThisApp = cibList.Where(c => c.ApplicationId == ApplicationId).FirstOrDefault();
                    if (cibForThisApp != null)
                    {
                        cib.LatestForThisApplicationId = cibForThisApp.Id;
                        cib.ApplicationNo = cibForThisApp.Application.ApplicationNo;
                        cib.VerificationDate = cibForThisApp.VerificationDate;
                        cib.VerificationDateText = cibForThisApp.VerificationDate != null ? ((DateTime)cibForThisApp.VerificationDate).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture) : "";

                    }
                }
            }
            cib.EditUrl = "/IPDC/Verification/CIB";
            resultList.Add(cib);
            #endregion

            #region cibOrganizational
            //var cibOrgList = GenService.GetAll<CIB_Organizational>().Where(c => c.CIF.Id == CifPersonalId).OrderByDescending(c => c.VerificationDate);
            //var cibOrgCount = cibOrgList.Count();
            //var cibOrg = new ApplicantCIFVerifications();
            //cibOrg.VerificationType = "Organizational";
            //if (cibCount > 0)
            //{
            //    var lastcibOrg = cibOrgList.FirstOrDefault();
            //    cibOrg.Count = cibOrgCount;
            //    if (lastcibOrg != null)
            //    {
            //        cibOrg.LatestVerificationId = lastcibOrg.Id;
            //        cibOrg.LatestVerificationDate = lastcibOrg.VerificationDate;
            //        cibOrg.LatestVerificationDateText = lastcibOrg.VerificationDate != null ? ((DateTime)lastcibOrg.VerificationDate).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture) : "";
            //        cibOrg.LatestApplicationNo = lastcibOrg.Application.ApplicationNo;
            //    }

            //    if (ApplicationId != null && ApplicationId > 0)
            //    {
            //        var cibOrgForThisApp = cibOrgList.Where(c => c.ApplicationId == ApplicationId).FirstOrDefault();
            //        if (cibOrgForThisApp != null)
            //        {
            //            cibOrg.LatestForThisApplicationId = cibOrgForThisApp.Id;
            //            cibOrg.ApplicationNo = cibOrgForThisApp.Application.ApplicationNo;
            //            cibOrg.VerificationDate = cibOrgForThisApp.VerificationDate;
            //            cibOrg.VerificationDateText = cibOrgForThisApp.VerificationDate != null ? ((DateTime)cibOrgForThisApp.VerificationDate).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture) : "";

            //        }
            //    }
            //}
            //cibOrg.EditUrl = "/IPDC/Verification/CIBOrganizational";
            //resultList.Add(cibOrg);
            #endregion

            #region nwsv
            if (app != null && app.Product.FacilityType != ProposalFacilityType.RLS)
            {

                var nwsvList = GenService.GetAll<NetWorthVerification>().Where(c => c.CIF_PersonalId == CifPersonalId && c.Status == EntityStatus.Active && c.ApplicationId == app.Id).OrderByDescending(c => c.VerificationDate);
                var nwsvCount = nwsvList.Count();
                var nwsv = new ApplicantCIFVerifications();
                nwsv.VerificationType = "NWSV";
                if (nwsvCount > 0)
                {
                    var lastnwsv = nwsvList.FirstOrDefault();
                    nwsv.Count = nwsvCount;
                    if (lastnwsv != null)
                    {
                        long employeeId = 0;
                        if (lastnwsv.VerifiedByUserId != null)
                        {
                            employeeId = _user.GetEmployeeIdByUserId((long)lastnwsv.VerifiedByUserId);
                        }
                        if (employeeId > 0)
                        {
                            var employee = _employee.GetEmployeeByEmployeeId(employeeId);
                            if (employee != null)
                            {
                                nwsv.VerifierName = employee.Name;
                            }
                        }
                        nwsv.LatestVerificationId = lastnwsv.Id;
                        nwsv.LatestVerificationDate = lastnwsv.VerificationDate;
                        nwsv.LatestVerificationDateText = lastnwsv.VerificationDate != null ? ((DateTime)lastnwsv.VerificationDate).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture) : "";
                        nwsv.VerificationStatus = lastnwsv.VerificationState;
                        nwsv.VerificationStatusName = lastnwsv.VerificationState.ToString();
                        nwsv.LatestApplicationNo = lastnwsv.Application.ApplicationNo;
                    }

                    if (ApplicationId != null && ApplicationId > 0)
                    {
                        var nwsvForThisApp = nwsvList.Where(c => c.ApplicationId == ApplicationId).FirstOrDefault();
                        if (nwsvForThisApp != null)
                        {
                            nwsv.VerificationStatusForThisApplication = nwsvForThisApp.VerificationState;
                            nwsv.VerificationStatusForThisApplicationName = nwsvForThisApp.VerificationState.ToString();
                            nwsv.LatestForThisApplicationId = nwsvForThisApp.Id;
                            nwsv.ApplicationNo = nwsvForThisApp.Application.ApplicationNo;
                            nwsv.VerificationDate = nwsvForThisApp.VerificationDate;
                            nwsv.VerificationDateText = nwsvForThisApp.VerificationDate != null ? ((DateTime)nwsvForThisApp.VerificationDate).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture) : "";

                        }
                    }
                }
                nwsv.EditUrl = "/IPDC/Verification/NetWorthVerification";
                resultList.Add(nwsv);
            }
            #endregion

            #region nid
            var nidList = GenService.GetAll<NIDVerification>().Where(c => c.CifId == CifPersonalId && c.Status == EntityStatus.Active && c.ApplicationId == app.Id).OrderByDescending(c => c.VerificationDate);
            var nidCount = nidList.Count();
            var nid = new ApplicantCIFVerifications();
            nid.VerificationType = "NID";
            if (nidCount > 0)
            {
                var lastnidv = nidList.FirstOrDefault();
                nid.Count = nidCount;
                if (lastnidv != null)
                {
                    long employeeId = 0;
                    if (lastnidv.VerifiedByUserId != null)
                    {
                        employeeId = _user.GetEmployeeIdByUserId((long)lastnidv.VerifiedByUserId);
                    }
                    if (employeeId > 0)
                    {
                        var employee = _employee.GetEmployeeByEmployeeId(employeeId);
                        if (employee != null)
                        {
                            nid.VerifierName = employee.Name;
                        }
                    }

                    nid.LatestVerificationId = lastnidv.Id;
                    nid.LatestVerificationDate = lastnidv.VerificationDate;
                    nid.LatestVerificationDateText = lastnidv.VerificationDate != null ? ((DateTime)lastnidv.VerificationDate).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture) : "";
                    nid.VerificationStatus = lastnidv.VerificationStatus;
                    nid.VerificationStatusName = lastnidv.VerificationStatus.ToString();
                    nid.LatestApplicationNo = lastnidv.Application.ApplicationNo;
                }

                if (ApplicationId != null && ApplicationId > 0)
                {
                    var nidForThisApp = nidList.Where(c => c.ApplicationId == ApplicationId).FirstOrDefault();
                    if (nidForThisApp != null)
                    {
                        nid.VerificationStatusForThisApplication = nidForThisApp.VerificationStatus;
                        nid.VerificationStatusForThisApplicationName = nidForThisApp.VerificationStatus.ToString();
                        nid.LatestForThisApplicationId = nidForThisApp.Id;
                        nid.ApplicationNo = nidForThisApp.Application.ApplicationNo;
                        nid.VerificationDate = nidForThisApp.VerificationDate;
                        nid.VerificationDateText = nidForThisApp.VerificationDate != null ? ((DateTime)nidForThisApp.VerificationDate).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture) : "";

                    }
                }
            }
            nid.EditUrl = "/IPDC/Verification/NIDVerification";
            resultList.Add(nid);
            #endregion

            #region income
            if (app != null && app.Product.FacilityType != ProposalFacilityType.RLS)
            {
                var incomeList = GenService.GetAll<IncomeVerification>().Where(c => c.CifId == CifPersonalId && c.Status == EntityStatus.Active && c.ApplicationId == app.Id).OrderByDescending(c => c.IncomeAssessmentDate);
                var incomeCount = incomeList.Count();
                var income = new ApplicantCIFVerifications();
                income.VerificationType = "Income Verification";
                if (incomeCount > 0)
                {
                    var lastincomev = incomeList.FirstOrDefault();
                    income.Count = nidCount;
                    if (lastincomev != null)
                    {
                        if (lastincomev.EmpId > 0)
                        {
                            var employee = _employee.GetEmployeeByEmployeeId((long)lastincomev.EmpId);
                            if (employee != null)
                            {
                                income.VerifierName = employee.Name;
                            }
                        }
                        income.LatestVerificationId = lastincomev.Id;
                        income.LatestVerificationDate = lastincomev.IncomeAssessmentDate;
                        income.LatestVerificationDateText = lastincomev.IncomeAssessmentDate != null ? ((DateTime)lastincomev.IncomeAssessmentDate).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture) : "";
                        income.VerificationStatus = lastincomev.VerificationState;
                        income.VerificationStatusName = lastincomev.VerificationState.ToString();
                        income.LatestApplicationNo = lastincomev.Application.ApplicationNo;
                    }

                    if (ApplicationId != null && ApplicationId > 0)
                    {
                        var incomeForThisApp = incomeList.Where(c => c.ApplicationId == ApplicationId).FirstOrDefault();
                        if (incomeForThisApp != null)
                        {
                            income.VerificationStatusForThisApplication = incomeForThisApp.VerificationState;
                            income.VerificationStatusForThisApplicationName = incomeForThisApp.VerificationState.ToString();
                            income.LatestForThisApplicationId = incomeForThisApp.Id;
                            income.ApplicationNo = incomeForThisApp.Application.ApplicationNo;
                            income.LatestVerificationDate = incomeForThisApp.IncomeAssessmentDate;
                            income.VerificationDateText = incomeForThisApp.IncomeAssessmentDate != null ? ((DateTime)incomeForThisApp.IncomeAssessmentDate).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture) : "";
                        }
                    }
                }
                income.EditUrl = "/IPDC/Verification/IncomeVerification";
                resultList.Add(income);
            }
            #endregion

            #region visitreport
            if (app != null && app.Product.FacilityType != ProposalFacilityType.RLS)
            {
                var visitList = GenService.GetAll<VisitReport>().Where(c => c.CIFId == CifPersonalId && c.Status == EntityStatus.Active && c.ApplicationId == app.Id);
                var visitCount = visitList.Count();
                var visit = new ApplicantCIFVerifications();
                visit.VerificationType = "Visit Report";
                if (visitCount > 0)
                {
                    var lastvisit = visitList.FirstOrDefault();
                    visit.Count = visitCount;
                    if (lastvisit != null)
                    {
                        if (lastvisit.VisitedById > 0)
                        {
                            var employee = _employee.GetEmployeeByEmployeeId((long)lastvisit.VisitedById);
                            if (employee != null)
                            {
                                visit.VerifierName = employee.Name;
                            }
                        }
                        visit.LatestVerificationId = lastvisit.Id;
                        visit.LatestApplicationNo = lastvisit.Application.ApplicationNo;
                        visit.VerificationStatusName = lastvisit.VerificationStatus != null ? UiUtil.GetDisplayName(lastvisit.VerificationStatus) : "";
                        visit.LatestVerificationDate = lastvisit.VisitTime;//.VerificationDate;
                        visit.LatestVerificationDateText = lastvisit.VisitTime != null ? ((DateTime)lastvisit.VisitTime).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture) : "";
                    }

                    if (ApplicationId != null && ApplicationId > 0)
                    {
                        var visitForThisApp = visitList.Where(c => c.ApplicationId == ApplicationId).FirstOrDefault();
                        if (visitForThisApp != null)
                        {
                            visit.LatestForThisApplicationId = visitForThisApp.Id;
                            visit.ApplicationNo = visitForThisApp.Application.ApplicationNo;
                            visit.VerificationStatusForThisApplicationName = visitForThisApp.VerificationStatus != null ? UiUtil.GetDisplayName(visitForThisApp.VerificationStatus) : "";
                            visit.VerificationDate = visitForThisApp.VisitTime;
                            visit.VerificationDateText = visitForThisApp.VisitTime != null ? ((DateTime)visitForThisApp.VisitTime).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture) : "";
                        }
                    }
                }
                visit.EditUrl = "/IPDC/Verification/VisitReport";
                resultList.Add(visit);
            }
            #endregion
            return resultList;
        }

        public ResponseDto SaveProjectTechnicalVerification(ProjectTechnicalVerificationDto dto, long? userId)
        {
            ResponseDto response = new ResponseDto();
            string filePath = "~/UploadedFiles/ProjectTechical/";
            long employeeId = 0;
            if (userId != null && userId > 0)
            {
                employeeId = _user.GetEmployeeIdByUserId((long)userId);
            }
            Project proj = new Project();
            if (dto.ProjectId > 0)
            {
                //proj = GenService.GetById<Project>((long)dto.ProjectId);
                if (!string.IsNullOrEmpty(proj.ProjectName))
                {
                    filePath += dto.ProjectId + "/";
                }
            }
            try
            {
                string path = HttpContext.Current.Server.MapPath(filePath);
                string file = "";
                if (dto.ProjectTechnicalVerificationFile != null)
                {
                    if (!Directory.Exists(path))
                        Directory.CreateDirectory(path);

                    string customfileName = "Project_Technical_" + proj.ProjectName + DateTime.Now.ToString("_yyyy_MM_dd_001", CultureInfo.InvariantCulture);
                    string fileExt = Path.GetExtension(dto.ProjectTechnicalVerificationFile.FileName);
                    path = Path.Combine(path, customfileName + fileExt);
                    for (int i = 1; File.Exists(path);)
                    {
                        var length = path.Length;
                        if (!string.IsNullOrEmpty(fileExt))
                            path = path.Substring(0, (length - 3 - fileExt.Length)) + (++i).ToString("000") + fileExt;
                        else
                            path = path.Substring(0, (length - 3)) + (++i).ToString("000");
                    }
                    dto.ProjectTechnicalVerificationFile.SaveAs(path);
                    file = path;
                }
                if (dto.Id > 0)
                {
                    var prev = GenService.GetById<ProjectTechnicalVerification>((long)dto.Id);
                    if (string.IsNullOrEmpty(file) && !string.IsNullOrEmpty(prev.ProjectTechnicalVerificationPath))
                    {
                        file = prev.ProjectTechnicalVerificationPath;
                    }
                    dto.CreateDate = prev.CreateDate;
                    dto.CreatedBy = prev.CreatedBy;
                    dto.Status = prev.Status;
                    dto.EmployeeId = prev.EmployeeId;
                    dto.EditDate = DateTime.Now;
                    prev = Mapper.Map(dto, prev);
                    if (!string.IsNullOrEmpty(file))
                    {
                        prev.ProjectTechnicalVerificationPath = file;
                    }
                    GenService.Save(prev);
                    response.Success = true;
                    response.Message = "Project Technical Verification Edited Successfully";
                }
                else
                {
                    if (dto.ProjectId > 0)
                    {
                        var project = GenService.GetAll<ProjectTechnicalVerification>().Where(r => r.Status == EntityStatus.Active && r.ProjectId == dto.ProjectId);
                        if (project != null)
                        {
                            project.ForEach(l => l.Status = EntityStatus.Inactive);
                            GenService.Save(project.ToList());
                        }
                    }
                    var data = Mapper.Map<ProjectTechnicalVerification>(dto);
                    if (!string.IsNullOrEmpty(file))
                    {
                        data.ProjectTechnicalVerificationPath = file;
                    }
                    data.Status = EntityStatus.Active;
                    data.CreateDate = DateTime.Now;
                    if (employeeId > 0)
                    {
                        data.EmployeeId = employeeId;
                    }
                    GenService.Save(data);
                    response.Success = true;
                    response.Message = "Project Technical Saved Successfully";
                }
            }
            catch (Exception ex)
            {
                response.Message = "Error Message : " + ex;
            }

            return response;
        }
        public ProjectTechnicalVerificationDto LoadProjectTechnicalData(long? id)
        {
            if (id != null)
            {
                var data = GenService.GetById<ProjectTechnicalVerification>((long)id);
                var result = Mapper.Map<ProjectTechnicalVerificationDto>(data);
                return result;
            }
            return null;
        }
        public ResponseDto SaveLegalDocumentVerification(LegalDocumentVerificationDto dto, long userId)
        {
            ResponseDto response = new ResponseDto();
            try
            {
                #region Edit
                if (dto.Id > 0)
                {
                    var prev = GenService.GetById<LegalDocumentVerification>((long)dto.Id);
                    if (dto.LegalDocuments != null)
                    {
                        foreach (var item in dto.LegalDocuments)
                        {
                            LegalDocumentStatus docDetail;
                            if (item.Id != null && item.Id > 0)
                            {
                                docDetail = GenService.GetById<LegalDocumentStatus>((long)item.Id);
                                item.Status = docDetail.Status;
                                item.CreateDate = docDetail.CreateDate;
                                item.CreatedBy = docDetail.CreatedBy;
                                item.EditDate = DateTime.Now;
                                item.EditedBy = userId;
                                //item.POId = docDetail.POId;
                                Mapper.Map(item, docDetail);
                                GenService.Save(docDetail);
                            }
                            else
                            {
                                docDetail = new LegalDocumentStatus();
                                //item.POId = prev.Id;
                                item.CreateDate = DateTime.Now;
                                item.CreatedBy = userId;
                                item.Status = EntityStatus.Active;
                                docDetail = Mapper.Map<LegalDocumentStatus>(item);
                                GenService.Save(docDetail);
                            }

                        }
                    }
                    if (dto.RemovedDocuments != null)
                    {
                        foreach (var item in dto.RemovedDocuments)
                        {
                            var detail = GenService.GetById<LegalDocumentStatus>(item);
                            if (detail != null)
                            {
                                detail.Status = EntityStatus.Inactive;
                                detail.EditDate = DateTime.Now;
                                detail.EditedBy = userId;
                            }
                            GenService.Save(detail);
                        }
                    }
                    dto.CreateDate = prev.CreateDate;
                    dto.CreatedBy = prev.CreatedBy;
                    dto.Status = prev.Status;
                    dto.ProjectId = prev.ProjectId;
                    dto.LandType = prev.LandType;
                    dto.EditDate = DateTime.Now;
                    prev = Mapper.Map(dto, prev);
                    GenService.Save(prev);
                    response.Id = prev.Id;
                    response.Success = true;
                    response.Message = "Legal Document Verification Edited Successfully";
                }
                #endregion
                #region Add
                else
                {
                    if (dto.ApplicationId > 0)
                    {
                        var app = GenService.GetAll<LegalDocumentVerification>().Where(r => r.Status == EntityStatus.Active && r.ProjectId == dto.ProjectId);
                        if (app != null)
                        {
                            app.ForEach(l => l.Status = EntityStatus.Inactive);
                            GenService.Save(app.ToList());
                        }
                    }
                    var data = Mapper.Map<LegalDocumentVerification>(dto);
                    data.EditDate = DateTime.Now;
                    data.Status = EntityStatus.Active;
                    data.CreateDate = DateTime.Now;
                    if (dto.LegalDocuments != null && dto.LegalDocuments.Count > 0)
                    {
                        data.LegalDocuments = Mapper.Map<List<LegalDocumentStatus>>(dto.LegalDocuments);
                        foreach (var item in data.LegalDocuments)
                        {
                            item.CreateDate = DateTime.Now;
                            item.CreatedBy = userId;
                            item.Status = EntityStatus.Active;
                        }
                    }

                    GenService.Save(data);
                    response.Id = data.Id;
                    response.Success = true;
                    response.Message = "Legal Document Verification Saved Successfully";
                }
                #endregion
            }
            catch (Exception ex)
            {
                response.Message = "Error Message : " + ex;
            }
            return response;
        }
        public LegalDocumentVerificationDto LoadLegalDocumentCheckList(long? appId, long? id)
        {
            var result = new LegalDocumentVerificationDto();
            if (appId > 0 && id > 0)
            {
                var application = GenService.GetById<Application>((long)appId);
                var legalDoc = GenService.GetById<LegalDocumentVerification>((long)id);
                result = Mapper.Map<LegalDocumentVerificationDto>(legalDoc);
                result.LegalDocuments.RemoveAll(f => f.Status != EntityStatus.Active);
                if (application != null)
                {
                    result.ApplicationNo = application.ApplicationNo;
                    result.ApplicationTitle = application.AccountTitle;
                    return result;
                }
            }
            else if (appId > 0)
            {
                var application = GenService.GetById<Application>((long)appId);
                if (application != null)
                {
                    if (application.LoanApplicationId > 0)
                    {
                        result.ApplicationId = application.Id;
                        result.ApplicationNo = application.ApplicationNo;
                        result.ApplicationTitle = application.AccountTitle;
                        var loan = GenService.GetAll<LPPrimarySecurity>()
                            .Where(
                                l =>
                                    l.LoanApplicationId == application.LoanApplicationId &&
                                    l.Status == EntityStatus.Active)
                            .OrderByDescending(l => l.Id).FirstOrDefault();
                        if (loan != null && loan.ProjectId != null)
                        {
                            var legal = GenService.GetAll<ProjectLegalVerification>().Where(r => r.Status == EntityStatus.Active && r.ProjectId == loan.ProjectId).OrderByDescending(r=>r.Id).FirstOrDefault();
                            if (legal != null && legal.LandType != null)
                            {
                                result.LandType = legal.LandType;
                                result.LandTypeName = legal.LandType.ToString();
                                result.ProjectId = legal.ProjectId;

                                if (result.LandType != null)
                                {
                                    var legalProp =
                                        GenService.GetAll<LegalDocPropType>().Where(i => i.LandType == result.LandType);
                                    result.LegalDocuments = legalProp.Select(x => new LegalDocumentStatusDto
                                    {
                                        LegalDocumentId = x.LegalDocumentId,
                                        LegalDocumentName = x.LegalDocument != null ? x.LegalDocument.DocumentName : ""
                                    }).ToList();
                                }
                            }
                        }
                    }
                }
            }

            return result;
        }

        public NIDVerificationDto GetVerifiedNIDCifId(long cifId)
        {
            var nidData = GenService.GetAll<NIDVerification>().OrderByDescending(r => r.Id).FirstOrDefault(r => r.CifId == cifId);
            if (nidData != null)
            {
                return Mapper.Map<NIDVerificationDto>(nidData);
            }
            return null;
        }
        public IncomeVerificationDto GetVerifiedIncomeByCifId(long cifId)
        {
            var cifData = GenService.GetAll<IncomeVerification>().FirstOrDefault(r => r.Id == cifId);
            var result = Mapper.Map<IncomeVerificationDto>(cifData);
            return result;
        }
    }


}
