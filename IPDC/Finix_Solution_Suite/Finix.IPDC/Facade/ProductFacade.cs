using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Finix.IPDC.DTO;
using Finix.IPDC.Infrastructure;
using Finix.IPDC.Infrastructure.Models;
using Finix.IPDC.Service;
using Finix.Auth.Facade;
using PagedList;

namespace Finix.IPDC.Facade
{
    public class ProductFacade : BaseFacade
    {
        private readonly GenService _service = new GenService();
        private readonly UserFacade _user = new UserFacade();
        private readonly EmployeeFacade _employee = new EmployeeFacade();
        
        public List<ProductDto> GetAllProducts()
        {
            var prod= _service.GetAll<Product>().Where(p => p.Status == EntityStatus.Active);
            return Mapper.Map<List<ProductDto>>(prod.ToList());
        }

        public List<ProductDto> GetProductByType(ProductType typeId)
        {
            var products = GenService.GetAll<Product>().Where(r => r.ProductType == typeId && r.Status == EntityStatus.Active).ToList();
            return Mapper.Map<List<ProductDto>>(products);
        }

        public List<AppDocChecklistDto> GetAllDocCheckList(long prodId, bool? IsIndividual, long? CifOrgId)
        {
            var docCheckList = GenService.GetAll<DocumentSetup>().Where(r => r.ProductId == prodId);
            if (IsIndividual != null)
            {
                if (IsIndividual == true)
                    docCheckList = docCheckList.Where(d => d.CustomerType == ApplicationCustomerType.Individual);
                else if (IsIndividual == false)
                {
                    docCheckList = docCheckList.Where(d => d.CustomerType == ApplicationCustomerType.Organizational);
                    if (CifOrgId != null)
                    {
                        var cifOrg = GenService.GetById<CIF_Organizational>((long)CifOrgId);
                        if (cifOrg != null && cifOrg.LegalStatus != null)
                            docCheckList = docCheckList.Where(d => d.CompanyLegalStatus == cifOrg.LegalStatus);
                    }
                }

            }
            var data = (from chk in docCheckList
                        select new AppDocChecklistDto
                        {
                            ApplicationId = 0,
                            ProductDocId = chk.Id,
                            ProductId = chk.ProductId,
                            ProductName = chk.Product.Name,
                            DocName = chk.Document.Name,
                            DocumentStatus = 0,
                            IsChecked = false
                        }).ToList();
            return data;
        }

        public ProductDto LoadProductById(long id)
        {
            var productDto = new ProductDto();
            if (id > 0)
            {
                var product = GenService.GetById<Product>((long)id);
                productDto = Mapper.Map<ProductDto>(product);
                productDto.ProductRates.RemoveAll(f => f.Status != EntityStatus.Active);
                productDto.ProductSpecialRate.RemoveAll(f => f.Status != EntityStatus.Active);
                productDto.DPSMaturitySchedule.RemoveAll(f => f.Status != EntityStatus.Active);
                productDto.DocumentSetups.RemoveAll(f => f.Status != EntityStatus.Active);
                productDto.ProductSecurity.RemoveAll(f => f.Status != EntityStatus.Active);

            }
            return productDto;
        }
        public ResponseDto SaveProduct(ProductDto dto, long userId)
        {
            ResponseDto response = new ResponseDto();
            var log = new ApplicationLog();
            try
            {
                #region Edit
                if (dto.Id > 0)
                {
                    var prev = GenService.GetById<Product>((long)dto.Id);
                    if (dto.ProductRates != null)
                    {
                        foreach (var item in dto.ProductRates)
                        {
                            ProductRates docDetail;
                            if (item.Id != null && item.Id > 0)
                            {
                                docDetail = GenService.GetById<ProductRates>((long)item.Id);
                                if (item.Status == null)
                                    item.Status = docDetail.Status;
                                item.CreateDate = docDetail.CreateDate;
                                item.CreatedBy = docDetail.CreatedBy;
                                item.ProductId = docDetail.ProductId;
                                item.EditDate = DateTime.Now;
                                item.EditedBy = userId;

                                Mapper.Map(item, docDetail);
                                GenService.Save(docDetail);
                            }
                            else
                            {
                                docDetail = new ProductRates();
                                item.CreateDate = DateTime.Now;
                                item.CreatedBy = userId;
                                item.Status = EntityStatus.Active;
                                item.ProductId = prev.Id;
                                docDetail = Mapper.Map<ProductRates>(item);
                                GenService.Save(docDetail);
                            }

                        }
                    }
                    if (dto.RemovedSecurities != null)
                    {
                        foreach (var item in dto.RemovedSecurities)
                        {
                            var detail = GenService.GetById<ProductSecurity>(item);
                            if (detail != null)
                            {
                                detail.Status = EntityStatus.Inactive;
                                detail.EditDate = DateTime.Now;
                                detail.EditedBy = userId;
                            }
                            GenService.Save(detail);
                        }
                    }
                    if (dto.RemovedProductRates != null)
                    {
                        foreach (var item in dto.RemovedProductRates)
                        {
                            var detail = GenService.GetById<ProductRates>(item);
                            if (detail != null)
                            {
                                detail.Status = EntityStatus.Inactive;
                                detail.EditDate = DateTime.Now;
                                detail.EditedBy = userId;
                            }
                            GenService.Save(detail);
                        }
                    }

                    if (dto.RemovedSpclProductRates != null)
                    {
                        foreach (var item in dto.RemovedSpclProductRates)
                        {
                            var detail = GenService.GetById<ProductSpecialRate>(item);
                            if (detail != null)
                            {
                                detail.Status = EntityStatus.Inactive;
                                detail.EditDate = DateTime.Now;
                                detail.EditedBy = userId;
                            }
                            GenService.Save(detail);
                        }
                    }

                    if (dto.RemovedDPSMaturitySchedule != null)
                    {
                        foreach (var item in dto.RemovedDPSMaturitySchedule)
                        {
                            var detail = GenService.GetById<DPSMaturitySchedule>(item);
                            if (detail != null)
                            {
                                detail.Status = EntityStatus.Inactive;
                                detail.EditDate = DateTime.Now;
                                detail.EditedBy = userId;
                            }
                            GenService.Save(detail);
                        }
                    }

                    if (dto.RemovedDocumentSetup != null)
                    {
                        foreach (var item in dto.RemovedDocumentSetup)
                        {
                            var detail = GenService.GetById<DocumentSetup>(item);
                            if (detail != null)
                            {
                                detail.Status = EntityStatus.Inactive;
                                detail.EditDate = DateTime.Now;
                                detail.EditedBy = userId;
                            }
                            GenService.Save(detail);
                        }
                    }


                    #region ProductSpecialRate
                    if (dto.ProductSpecialRate != null)
                    {
                        foreach (var signatorDto in dto.ProductSpecialRate)
                        {
                            var signatory = new ProductSpecialRate();
                            if (signatorDto.Id != null && signatorDto.Id > 0)
                            {
                                signatory = GenService.GetById<ProductSpecialRate>((long)signatorDto.Id);
                                signatorDto.CreateDate = signatory.CreateDate;
                                signatorDto.CreatedBy = signatory.CreatedBy;
                                signatorDto.Status = signatory.Status;
                                signatorDto.EditDate = DateTime.Now;
                                signatorDto.ProductId = signatory.ProductId;
                                signatorDto.AuthorizedBy = signatory.AuthorizedBy;
                                signatory = Mapper.Map(signatorDto, signatory);
                                GenService.Save(signatory);
                            }
                            else
                            {
                                //assesments = new Proposal_OverallAssessment();
                                signatory = Mapper.Map<ProductSpecialRate>(signatorDto);
                                signatory.Status = EntityStatus.Active;
                                signatory.CreatedBy = userId;
                                signatory.CreateDate = DateTime.Now;
                                signatory.ProductId = prev.Id;
                                GenService.Save(signatory);
                            }
                        }
                    }
                    #endregion
                    #region DPSMaturitySchedule
                    if (dto.DPSMaturitySchedule != null)
                    {
                        foreach (var signatorDto in dto.DPSMaturitySchedule)
                        {
                            var signatory = new DPSMaturitySchedule();
                            if (signatorDto.Id != null && signatorDto.Id > 0)
                            {
                                signatory = GenService.GetById<DPSMaturitySchedule>((long)signatorDto.Id);
                                signatorDto.CreateDate = signatory.CreateDate;
                                signatorDto.CreatedBy = signatory.CreatedBy;
                                signatorDto.Status = signatory.Status;
                                signatorDto.EditDate = DateTime.Now;
                                signatorDto.ProductId = signatory.ProductId;
                                signatory = Mapper.Map(signatorDto, signatory);
                                GenService.Save(signatory);
                            }
                            else
                            {
                                //assesments = new Proposal_OverallAssessment();
                                signatory = Mapper.Map<DPSMaturitySchedule>(signatorDto);
                                signatory.Status = EntityStatus.Active;
                                signatory.CreatedBy = userId;
                                signatory.CreateDate = DateTime.Now;
                                signatory.ProductId = prev.Id;
                                GenService.Save(signatory);
                            }
                        }
                    }
                    #endregion
                    #region DocumentSetups
                    if (dto.DocumentSetups != null)
                    {
                        foreach (var signatorDto in dto.DocumentSetups)
                        {
                            var signatory = new DocumentSetup();
                            if (signatorDto.Id != null && signatorDto.Id > 0)
                            {
                                signatory = GenService.GetById<DocumentSetup>((long)signatorDto.Id);
                                signatorDto.CreateDate = signatory.CreateDate;
                                signatorDto.CreatedBy = signatory.CreatedBy;
                                signatorDto.Status = signatory.Status;
                                signatorDto.EditDate = DateTime.Now;
                                signatorDto.ProductId = signatory.ProductId;
                                signatory = Mapper.Map(signatorDto, signatory);
                                GenService.Save(signatory);
                            }
                            else
                            {
                                //assesments = new Proposal_OverallAssessment();
                                signatory = Mapper.Map<DocumentSetup>(signatorDto);
                                signatory.Status = EntityStatus.Active;
                                signatory.CreatedBy = userId;
                                signatory.CreateDate = DateTime.Now;
                                signatory.ProductId = prev.Id;
                                GenService.Save(signatory);
                            }
                        }
                    }
                    #endregion

                    #region ProductSecurity
                    if (dto.ProductSecurity != null)
                    {
                        foreach (var signatorDto in dto.ProductSecurity)
                        {
                            var signatory = new ProductSecurity();
                            if (signatorDto.Id != null && signatorDto.Id > 0)
                            {
                                signatory = GenService.GetById<ProductSecurity>((long)signatorDto.Id);
                                signatorDto.CreateDate = signatory.CreateDate;
                                signatorDto.CreatedBy = signatory.CreatedBy;
                                signatorDto.Status = signatory.Status;
                                signatorDto.EditDate = DateTime.Now;
                                signatorDto.ProductId = signatory.ProductId;
                                signatory = Mapper.Map(signatorDto, signatory);
                                GenService.Save(signatory);
                            }
                            else
                            {
                                signatory = Mapper.Map<ProductSecurity>(signatorDto);
                                signatory.Status = EntityStatus.Active;
                                signatory.CreatedBy = userId;
                                signatory.CreateDate = DateTime.Now;
                                signatory.ProductId = prev.Id;
                                GenService.Save(signatory);
                            }
                        }
                    }
                    #endregion
                    dto.CreateDate = prev.CreateDate;
                    dto.CreatedBy = prev.CreatedBy;
                    dto.Status = prev.Status;
                    //dto.ProductId = prev.ProposalId;
                    dto.EditDate = DateTime.Now;
                    prev = Mapper.Map(dto, prev);
                    GenService.Save(prev);
                    //GenService.Save(signatoryList);
                    response.Id = prev.Id;
                    response.Success = true;
                    response.Message = "Product Edited Successfully";
                }
                #endregion
                #region Add
                else
                {
                    var data = Mapper.Map<Product>(dto);
                    data.EditDate = DateTime.Now;
                    data.Status = EntityStatus.Active;
                    data.CreateDate = DateTime.Now;
                    if (dto.ProductRates != null && dto.ProductRates.Count > 0)
                    {
                        data.ProductRates = Mapper.Map<List<ProductRates>>(dto.ProductRates);
                        foreach (var item in data.ProductRates)
                        {
                            item.ProductId = data.Id;
                            item.CreateDate = DateTime.Now;
                            item.CreatedBy = userId;
                            item.Status = EntityStatus.Active;
                        }
                    }
                  
                    if (dto.ProductSpecialRate != null && dto.ProductSpecialRate.Count > 0)
                    {
                        data.ProductSpecialRate = Mapper.Map<List<ProductSpecialRate>>(dto.ProductSpecialRate);
                        long employeeId = _user.GetEmployeeIdByUserId(userId);
                        long offDegSettingId = _employee.GetOfficeDesignationIdOfEmployee(employeeId);
                        foreach (var item in data.ProductSpecialRate)
                        {
                            item.ProductId = data.Id;
                            item.CreateDate = DateTime.Now;
                            item.CreatedBy = userId;
                            item.Status = EntityStatus.Active;
                            item.AuthorizedBy = offDegSettingId;
                        }
                        //data.ProductSpecialRate.ForEach(r=>r.AuthorizedBy = offDegSettingId);
                    }

                    if (dto.DPSMaturitySchedule != null && dto.DPSMaturitySchedule.Count > 0)
                    {
                        data.DPSMaturitySchedule = Mapper.Map<List<DPSMaturitySchedule>>(dto.DPSMaturitySchedule);
                        foreach (var item in data.DPSMaturitySchedule)
                        {
                            item.ProductId = data.Id;
                            item.CreateDate = DateTime.Now;
                            item.CreatedBy = userId;
                            item.Status = EntityStatus.Active;
                        }
                    }
                    if (dto.DocumentSetups != null && dto.DocumentSetups.Count > 0)
                    {
                        data.DocumentSetups = Mapper.Map<List<DocumentSetup>>(dto.DocumentSetups);
                        foreach (var item in data.DocumentSetups)
                        {
                            item.ProductId = data.Id;
                            item.CreateDate = DateTime.Now;
                            item.CreatedBy = userId;
                            item.Status = EntityStatus.Active;
                        }
                    }
                    if (dto.ProductSecurity != null && dto.ProductSecurity.Count > 0)
                    {
                        data.ProductSecurity = Mapper.Map<List<ProductSecurity>>(dto.ProductSecurity);
                        foreach (var item in data.ProductSpecialRate)
                        {
                            item.ProductId = data.Id;
                            item.CreateDate = DateTime.Now;
                            item.CreatedBy = userId;
                            item.Status = EntityStatus.Active;
                        }
                    }
                    //data.OfferLetterNo = _sequencer.GetUpdatedOfferLetterNo();
                    GenService.Save(data);
                    response.Id = data.Id;
                    response.Success = true;
                    response.Message = "Product Saved Successfully";
                }
                #endregion

            }
            catch (Exception ex)
            {
                response.Message = "Error Message : " + ex;
            }
            return response;
        }
        public List<DocumentDto> GetAllDocuments()
        {
            var prod = _service.GetAll<Document>().Where(p => p.Status == EntityStatus.Active);
            return Mapper.Map<List<DocumentDto>>(prod.ToList());
        }
        public IPagedList<ProductDto> GetProducts(int pageSize, int pageCount, string searchString, long UserId)
        {
            var allProduct = GenService.GetAll<Product>().Where(s =>s.Status == EntityStatus.Active).Select(s => new ProductDto() // Previous-'SubmitedToCRM'
            {
                Id = s.Id,
                Name = s.Name,
                FacilityTypeName = s.FacilityType.ToString(),
                ProductTypeName = s.ProductType.ToString(),
                DepositTypeName = s.DepositType.ToString(),
                ApplicationFee = s.ApplicationFee 
            }).ToList();
            //allApp.ForEach(r=>r.ApplicantName = r.CIFList.Where(l=> l.ApplicantRole == ApplicantRole.Primary).FirstOrDefault().ApplicantName);
           
            if (!string.IsNullOrEmpty(searchString))
                allProduct = allProduct.Where(a => a.Name.ToLower().Contains(searchString.ToLower())).ToList();
            var temp = allProduct.OrderByDescending(r => r.Id).ToPagedList(pageCount, pageSize);
            return temp;
        }
    }
}
