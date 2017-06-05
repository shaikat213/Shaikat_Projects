using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Finix.Auth.DTO;
using Finix.Auth.Infrastructure;
using Finix.Auth.Service;
using AutoMapper;

namespace Finix.Auth.Facade
{
    public class CompanyProfileFacade : BaseFacade
    {
        private readonly GenService _service = new GenService();

        public ResponseDto SaveCompanyProfile(CompanyProfileDto companyDto, long? userId)
        {
            var response = new ResponseDto();
            CompanyProfile entity;

            if (companyDto.Id > 0)
            {
                entity = _service.GetById<CompanyProfile>(companyDto.Id);
                entity.EditDate = DateTime.Now;
                if (userId != null)
                    entity.EditedBy = userId;

                entity.Name = companyDto.Name;
                entity.Address = companyDto.Address;
                entity.PhoneNo = companyDto.PhoneNo;
                entity.Email = companyDto.Email;
                entity.Fax = companyDto.Fax;
                entity.ContactPerson = companyDto.ContactPerson;
                entity.CompanyType = companyDto.CompanyType;
                //entity.CompanyTypeName = companyDto.CompanyTypeName;
                entity.ParentId = companyDto.ParentId;
                //entity.ParentName 
            }
            else
            {
                entity = Mapper.Map<CompanyProfile>(companyDto);
                entity.CreateDate = DateTime.Now;
                if (userId != null)
                    entity.CreatedBy = userId;

            }
            try
            {
                _service.Save(entity);
                response.Success = true;
            }
            catch (Exception)
            {

            }
            return response;
        }
        public string GetUpdateBillNo()
        {
            try
            {
                var cp = _service.GetById<CompanyProfile>(1);
                var nBill = Convert.ToDecimal(cp.BillNo.Trim()) + 1;
                cp.BillNo = "" + nBill;
                _service.Save(cp);
                return "BN-" + nBill;
            }
            catch (Exception)
            {

                throw;
            }


        }

        public string GetUpdateInvoiceNo()
        {
            try
            {
                var cp = _service.GetById<CompanyProfile>(1);
                var nIv = Convert.ToDecimal(cp.InvoiceNo.Trim()) + 1;
                cp.InvoiceNo = "" + nIv;
                _service.Save(cp);
                return "CIV-" + nIv;
            }
            catch (Exception)
            {
                throw;
            }
        }
        //public string GetUpdateCr()
        //{
        //    try
        //    {
        //        var cp = _service.GetById<CompanyProfile>(1);
        //        var nCr = Convert.ToDecimal(cp.InvoiceNo.Trim()) + 1;
        //        cp.InvoiceNo = "" + nCr;
        //        _service.Save(cp);
        //        return "CV-" + nCr;
        //    }
        //    catch (Exception)
        //    {

        //        throw;
        //    }
        //}
        //public string GetUpdateJv()
        //{
        //    try
        //    {
        //        var cp = _service.GetById<CompanyProfile>(1);
        //        var nJv = Convert.ToDecimal(cp.InvoiceNo.Trim()) + 1;
        //        cp.InvoiceNo = "" + nJv;
        //        _service.Save(cp);
        //        return "JV-" + nJv + "-" + DateTime.Now.Year.ToString();
        //    }
        //    catch (Exception)
        //    {

        //        throw;
        //    }
        //}
        public string GetUpdateVoucherNo()
        {
            try
            {
                var cp = _service.GetById<CompanyProfile>(1);
                var nVn = Convert.ToDecimal(cp.VoucherNo.Trim()) + 1;
                cp.VoucherNo = "" + nVn;
                _service.Save(cp);
                return "" + DateTime.Now.Year + "-" + nVn;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public string GetUpdateCDvNo()
        {
            try
            {
                var cp = _service.GetById<CompanyProfile>(1);
                var nVn = Convert.ToDecimal(cp.CDvNo.Trim()) + 1;
                cp.CDvNo = "" + nVn;
                _service.Save(cp);
                return "" + DateTime.Now.Year + "-" + nVn;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public string GetUpdateBDvNo()
        {
            try
            {
                var cp = _service.GetById<CompanyProfile>(1);
                var nVn = Convert.ToDecimal(cp.BDvNo.Trim()) + 1;
                cp.BDvNo = "" + nVn;
                _service.Save(cp);
                return "" + DateTime.Now.Year + "-" + nVn;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public string GetUpdateCCvNo()
        {
            try
            {
                var cp = _service.GetById<CompanyProfile>(1);
                var nVn = Convert.ToDecimal(cp.CCvNo.Trim()) + 1;
                cp.CCvNo = "" + nVn;
                _service.Save(cp);
                return "" + DateTime.Now.Year + "-" + nVn;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public string GetUpdateBCvNo()
        {
            try
            {
                var cp = _service.GetById<CompanyProfile>(1);
                var nVn = Convert.ToDecimal(cp.BCvNo.Trim()) + 1;
                cp.BCvNo = "" + nVn;
                _service.Save(cp);
                return "" + DateTime.Now.Year + "-" + nVn;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public string GetUpdateJvNo()
        {
            try
            {
                var cp = _service.GetById<CompanyProfile>(1);
                var nVn = Convert.ToDecimal(cp.JvNo.Trim()) + 1;
                cp.JvNo = "" + nVn;
                _service.Save(cp);
                return "" + DateTime.Now.Year + "-" + nVn;
            }
            catch (Exception)
            {
                throw;
            }
        }

        //public string GetUpdateInvoiceNo()
        //{
        //    try
        //    {
        //        var cp = _service.GetById<CompanyProfile>(1);
        //        var nInv = Convert.ToDecimal(cp.InvoiceNo.Trim()) + 1;
        //        cp.InvoiceNo = "" + nInv;
        //        _service.Save(cp);
        //        return "INV-" + nInv;
        //    }
        //    catch (Exception)
        //    {
        //        throw;
        //    }
        //}

        public string GetUpdatedChalanNo()
        {
            try
            {
                var cp = _service.GetById<CompanyProfile>(1);
                var nCln = Convert.ToDecimal(cp.ChalanNo.Trim()) + 1;
                cp.ChalanNo = "" + nCln;
                _service.Save(cp);
                return "CLN-" + nCln;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public string GetUpdatedPreProdNo()
        {
            try
            {
                var cp = _service.GetById<CompanyProfile>(1);
                var prdno = Convert.ToDecimal(cp.PreProdNo.Trim()) + 1;
                cp.PreProdNo = "" + prdno;
                _service.Save(cp);
                return "PPDN-" + prdno;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public string GetUpdatedProdNo()
        {
            try
            {
                var cp = _service.GetById<CompanyProfile>(1);
                var prdno = Convert.ToDecimal(cp.ProdNo.Trim()) + 1;
                cp.ProdNo = "" + prdno;
                _service.Save(cp);
                return "PDN-" + prdno;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public ResponseDto UpdateClosingDate()
        {
            ResponseDto response = new ResponseDto();
            var model = _service.GetAll<CompanyProfile>().FirstOrDefault();
            var entity = new CompanyProfile();
            try
            {
                entity = _service.GetById<CompanyProfile>(model.Id);
                entity.SystemDate = model.SystemDate.AddDays(1);
                _service.Save(entity);
                response.Success = true;
                response.Message = "Date Update Successfully";
                //return true;
            }
            catch (Exception)
            {
                response.Message = "Date Updated Failed";
                //return false;
            }
            return response;
        }

        public DateTime DateToday(long? CompanyId)
        {
            DateTime SystemDate;
            if (CompanyId != null && CompanyId > 0)
            {
                var company = _service.GetById<CompanyProfile>((long)CompanyId);
                if (company != null)
                    SystemDate = company.SystemDate;
                else
                    SystemDate = DateTime.MinValue;
            }
            else
            {
                SystemDate = _service.GetAll<CompanyProfile>().Select(r => r.SystemDate).FirstOrDefault();
            }
            return SystemDate;
        }

        public DateTime GetCurrentFiscalYear()
        {
            return _service.GetById<CompanyProfile>(1).FiscalYear;
        }

        public List<CompanyProfileDto> GetAllCompanyProfiles()
        {
            return Mapper.Map<List<CompanyProfileDto>>(_service.GetAll<CompanyProfile>().ToList());
        }

        public List<CompanyProfileDto> GetAllActiveCompanyProfiles()
        {
            var temp = Mapper.Map<List<CompanyProfileDto>>(_service.GetAll<CompanyProfile>().Where(c => c.Status == EntityStatus.Active).ToList());
            return Mapper.Map<List<CompanyProfileDto>>(_service.GetAll<CompanyProfile>().Where(c => c.Status == EntityStatus.Active).ToList());
        }

        public CompanyProfileDto GetCompanyProfileById(long CompanyProfileId)
        {
            return Mapper.Map<CompanyProfileDto>(_service.GetById<CompanyProfile>(CompanyProfileId));
        }

        public List<CompanyProfileDto> GetAccessbleSubCompaniesAndProjects(long UserId, long CompanyId)
        {
            var list = new List<CompanyProfileDto>();
            var directPermissions = _service.GetAll<UserCompanyApplication>().Where(u => u.UserId == UserId && u.Status == EntityStatus.Active).ToList();
            if (directPermissions.Where(d => d.CompanyId == CompanyId).Any())
                list.Add(Mapper.Map<CompanyProfileDto>(directPermissions.Where(d => d.CompanyId == CompanyId).FirstOrDefault().CompanyProfile));
            var filteredPermissions = FilterSubPremissions(directPermissions.Select(d => d.CompanyId).ToList(), CompanyId);
            if (filteredPermissions != null)
                list.AddRange(filteredPermissions);
            return list;
        }

        public List<CompanyProfileDto> GetAllSubCompaniesAndProjects(long CompanyId)
        {
            var list = new List<CompanyProfileDto>();
            var subCompanies = _service.GetAll<CompanyProfile>().Where(c => c.ParentId == CompanyId).ToList();
            if (subCompanies != null && subCompanies.Count > 0)
            {
                list.AddRange(Mapper.Map<List<CompanyProfileDto>>(subCompanies));
                foreach (var company in subCompanies)
                {

                    list.AddRange(GetAllSubCompaniesAndProjects(company.Id));
                }
                return list;
            }
            else
                return new List<CompanyProfileDto>();
        }

        #region helpers
        private List<CompanyProfileDto> FilterSubPremissions(List<long> companyPermissions, long CurrentCompany)
        {
            var result = new List<CompanyProfileDto>();
            var subCompanies = _service.GetAll<CompanyProfile>().Where(c => c.ParentId == CurrentCompany).ToList();
            if (subCompanies == null || subCompanies.Count < 1)
                return null;
            var permissions = subCompanies.Where(s => companyPermissions.Contains(s.Id)).ToList();
            result.AddRange(Mapper.Map<List<CompanyProfileDto>>(permissions));


            foreach (var company in subCompanies)
            {
                var permission = FilterSubPremissions(companyPermissions, company.Id);
                if (permission != null && permission.Count > 0)
                    result.AddRange(permission);
            }
            return result;
        }
        #endregion


    }
}
