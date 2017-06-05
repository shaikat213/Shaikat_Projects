using AutoMapper;
using Finix.IPDC.DTO;
using Finix.IPDC.Infrastructure.Models;
using Finix.IPDC.Service;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Finix.IPDC.Infrastructure;
using System.Transactions;
using System.Web;
using Finix.IPDC.Util;
using PagedList;

namespace Finix.IPDC.Facade
{
    public class CIFFacade : BaseFacade
    {
        //private readonly  GenService _service = new GenService();
        private readonly SequencerFacade _sequencer = new SequencerFacade();
        private readonly Auth.Facade.UserFacade _user = new Auth.Facade.UserFacade();
        private readonly EmployeeFacade _employee = new EmployeeFacade();
        public ResponseDto SaveCifPersonal(CIF_PersonalDto dto, long UserId)
        {
            var entity = new CIF_Personal();
            var address = new Address();
            ResponseDto responce = new ResponseDto();

            //try
            //{
            if (dto.Id != null && dto.Id > 0)
            {
                entity = GenService.GetById<CIF_Personal>((long)dto.Id);
                dto.CIFNo = entity.CIFNo;
                dto.CreateDate = entity.CreateDate;
                dto.CreatedBy = entity.CreatedBy;
                dto.CustomersHomeBranch = entity.CustomersHomeBranch;
                using (var tran = new TransactionScope())
                {
                    try
                    {
                        if (dto.PermanentAddress.IsChanged)
                        {
                            if (dto.PermanentAddress.Id != null)
                            {
                                address = GenService.GetById<Address>((long)dto.PermanentAddress.Id);
                                dto.PermanentAddress.CreateDate = address.CreateDate;
                                dto.PermanentAddress.CreatedBy = address.CreatedBy;
                                address = Mapper.Map(dto.PermanentAddress, address);
                                GenService.Save(address);
                                dto.PermanentAddressId = address.Id;
                            }
                            else
                            {
                                var permanentAddress = Mapper.Map<Address>(dto.PermanentAddress);
                                GenService.Save(permanentAddress);
                                dto.PermanentAddressId = permanentAddress.Id;
                            }

                        }
                        else
                        {
                            dto.PermanentAddressId = entity.PermanentAddressId;
                        }
                        if (dto.ResidenceAddress.IsChanged)
                        {
                            if (dto.ResidenceAddress.Id != null)
                            {
                                address = GenService.GetById<Address>((long)dto.ResidenceAddress.Id);
                                dto.ResidenceAddress.CreateDate = address.CreateDate;
                                dto.ResidenceAddress.CreatedBy = address.CreatedBy;
                                address = Mapper.Map(dto.ResidenceAddress, address);
                                GenService.Save(address);
                                dto.ResidenceAddressId = address.Id;
                            }
                            else
                            {
                                var residenceAddress = Mapper.Map<Address>(dto.ResidenceAddress);
                                GenService.Save(residenceAddress);
                                dto.ResidenceAddressId = residenceAddress.Id;
                            }

                        }
                        else
                        {
                            dto.ResidenceAddressId = entity.ResidenceAddressId;
                        }
                        if (dto.SpouseWorkAddress.IsChanged)
                        {
                            if (dto.SpouseWorkAddress.Id != null)
                            {
                                address = GenService.GetById<Address>((long)dto.SpouseWorkAddress.Id);
                                dto.SpouseWorkAddress.CreateDate = address.CreateDate;
                                dto.SpouseWorkAddress.CreatedBy = address.CreatedBy;
                                address = Mapper.Map(dto.SpouseWorkAddress, address);
                                GenService.Save(address);
                                dto.SpouseWorkAddressId = address.Id;
                            }
                            else
                            {
                                var swAddress = Mapper.Map<Address>(dto.SpouseWorkAddress);
                                GenService.Save(swAddress);
                                dto.SpouseWorkAddressId = swAddress.Id;
                            }

                        }
                        else
                        {
                            dto.SpouseWorkAddressId = entity.SpouseWorkAddressId;
                        }

                        entity = Mapper.Map(dto, entity);
                        entity.EditDate = DateTime.Now;
                        entity.EditedBy = UserId;
                        entity.Status = EntityStatus.Active;
                        GenService.Save(entity);
                        responce.Id = entity.Id;
                        tran.Complete();
                    }
                    catch (Exception ex)
                    {
                        tran.Dispose();
                        responce.Message = "Client Information Edit Operation Failed";
                    }
                }
                responce.Success = true;
                responce.Message = "Client Information Edited Successfully";
            }
            else
            {

                entity = Mapper.Map<CIF_Personal>(dto);
                entity.Status = EntityStatus.Active;
                entity.CreatedBy = UserId;
                entity.CreateDate = DateTime.Now;
                var residenceAddress = Mapper.Map<Address>(dto.ResidenceAddress);
                var permanentAddress = Mapper.Map<Address>(dto.PermanentAddress);
                var swAddress = Mapper.Map<Address>(dto.SpouseWorkAddress);
                var employeeId = _user.GetEmployeeIdByUserId(UserId);
                entity.CustomersHomeBranch = _employee.GetEmployeeOfficeByEmployeeID(employeeId);
                using (var tran = new TransactionScope())
                {
                    try
                    {
                        if (residenceAddress.CountryId != null)
                        {
                            GenService.Save(residenceAddress);
                            entity.ResidenceAddressId = residenceAddress.Id;
                        }
                        if (permanentAddress.CountryId != null)
                        {
                            GenService.Save(permanentAddress);
                            entity.PermanentAddressId = permanentAddress.Id;
                        }
                        if (swAddress.CountryId != null)
                        {
                            GenService.Save(swAddress);
                            entity.SpouseWorkAddressId = swAddress.Id;
                        }
                        GenService.Save(entity);
                        entity.CIFNo = _sequencer.GetUpdatedCIFPersonalNo();
                        GenService.Save(entity);
                        responce.Id = entity.Id;
                        tran.Complete();
                    }
                    catch (Exception ex)
                    {
                        tran.Dispose();
                        responce.Message = "Client Information Save Failed";
                    }
                }

                responce.Success = true;
                responce.Message = "Client Information Saved Successfully";
            }
            return responce;


        }
        public ResponseDto SaveCIFIncomeStatement(CIF_IncomeStatementDto dto, long UserId)
        {
            ResponseDto responce = new ResponseDto();
            try
            {
                if (dto.CIF_PersonalId > 0)
                {
                    if (dto.Id > 0)
                    {
                        CIF_IncomeStatement oldEntries = GenService.GetAll<CIF_IncomeStatement>().Where(c => c.CIF_PersonalId == dto.CIF_PersonalId && c.Status == EntityStatus.Active).OrderByDescending(r => r.Id).FirstOrDefault();
                        if (dto.MonthlyOtherIncomesDeclared != null)
                        {
                            foreach (var item in dto.MonthlyOtherIncomesDeclared)
                            {
                                CIF_AdditionalIncomeDeclared additionalIncome;
                                if (item.Id != null && item.Id > 0)
                                {
                                    additionalIncome = GenService.GetById<CIF_AdditionalIncomeDeclared>((long)item.Id);
                                    if (item.Status == null)
                                    {
                                        item.Status = additionalIncome.Status;
                                    } 
                                    item.CreateDate = additionalIncome.CreateDate;
                                    item.CreatedBy = additionalIncome.CreatedBy;
                                    item.CIF_IncomeStatementId = additionalIncome.CIF_IncomeStatementId;
                                    item.EditDate = DateTime.Now;
                                    item.EditedBy = UserId;
                                    Mapper.Map(item, additionalIncome);
                                    GenService.Save(additionalIncome);
                                }
                                else
                                {
                                    additionalIncome = new CIF_AdditionalIncomeDeclared();
                                    item.CreateDate = DateTime.Now;
                                    item.CreatedBy = UserId;
                                    item.Status = EntityStatus.Active;
                                    item.CIF_IncomeStatementId = oldEntries.Id;
                                    additionalIncome = Mapper.Map<CIF_AdditionalIncomeDeclared>(item);
                                    GenService.Save(additionalIncome);
                                }
                            }
                            #region list deletes
                            if (dto.RemovedAdditionalIncomeDeclared != null)
                            {
                                foreach (var item in dto.RemovedAdditionalIncomeDeclared)
                                {
                                    var cif = GenService.GetById<ApplicationCIFs>(item);
                                    if (cif != null)
                                    {
                                        cif.Status = EntityStatus.Inactive;
                                        cif.EditDate = DateTime.Now;
                                        cif.EditedBy = UserId;
                                    }
                                    GenService.Save(cif);
                                }
                            }

                            //if(dto.r)
                            #endregion
                        }
                        dto.CreatedBy = oldEntries.CreatedBy;
                        dto.CreatedBy = oldEntries.CreatedBy;
                        oldEntries = Mapper.Map(dto, oldEntries);
                        oldEntries.EditDate = DateTime.Now;
                        oldEntries.EditedBy = UserId;
                        oldEntries.Status = EntityStatus.Active;
                        GenService.Save(oldEntries);
                        responce.Success = true;
                        responce.Message = "CIF IncomeStatement Edited Successfully";
                    }
                    else
                    {
                        var oldEntries = GenService.GetAll<CIF_IncomeStatement>().Where(c => c.CIF_PersonalId == dto.CIF_PersonalId && c.Status == EntityStatus.Active).ToList();
                        foreach (var item in oldEntries)
                        {
                            item.EditDate = DateTime.Now;
                            item.EditedBy = UserId;
                            item.Status = EntityStatus.Inactive;
                        }
                        var entity = Mapper.Map<CIF_IncomeStatement>(dto);
                        entity.CreateDate = DateTime.Now;
                        entity.CreatedBy = UserId;
                        entity.Status = EntityStatus.Active;
                        if (dto.MonthlyOtherIncomesDeclared != null && dto.MonthlyOtherIncomesDeclared.Count > 0)
                        {
                            entity.MonthlyOtherIncomesDeclared = new List<CIF_AdditionalIncomeDeclared>();
                            entity.MonthlyOtherIncomesDeclared = Mapper.Map<List<CIF_AdditionalIncomeDeclared>>(dto.MonthlyOtherIncomesDeclared);
                            entity.MonthlyOtherIncomesDeclared = entity.MonthlyOtherIncomesDeclared.Select(d => { d.Status = EntityStatus.Active; return d; }).ToList();
                        }
                        if (oldEntries != null && oldEntries.Count > 0)
                            GenService.Save(oldEntries);
                        GenService.Save(entity);
                        responce.Success = true;
                        responce.Id = entity.Id;
                        responce.Message = "CIF IncomeStatement Saved Successfully";
                    }
                }
                else
                {
                    responce.Message = "Please Give Entry Of CIF Personal First";
                    return responce;
                }
            }
            catch (Exception)
            {
                responce.Message = "CIF_IncomeStatement Save Failed";
            }
            GenService.SaveChanges();
            return responce;
        }
        public ResponseDto SaveCIFReference(List<CIF_ReferenceDto> dto, long UserId)
        {
            ResponseDto responce = new ResponseDto();
            foreach (var cifReferenceDto in dto)
            {

                if (cifReferenceDto.CIF_PersonalId > 0)
                {

                    if (cifReferenceDto.Id != null && cifReferenceDto.Id > 0)
                    {
                        #region New Entry
                        CIF_Reference oldEntries = GenService.GetById<CIF_Reference>((long)cifReferenceDto.Id);
                        cifReferenceDto.CreatedBy = oldEntries.CreatedBy;
                        cifReferenceDto.CreateDate = oldEntries.CreateDate;
                        if (cifReferenceDto.Status == null)
                            cifReferenceDto.Status = oldEntries.Status;

                        #region address changes
                        if (cifReferenceDto.PermanentAddress.IsChanged)
                        {
                            if (cifReferenceDto.PermanentAddress.Id != null && cifReferenceDto.PermanentAddress.Id > 0)
                            {
                                var address = GenService.GetById<Address>((long)cifReferenceDto.PermanentAddress.Id);
                                cifReferenceDto.PermanentAddress.CreateDate = address.CreateDate;
                                cifReferenceDto.PermanentAddress.CreatedBy = address.CreatedBy;
                                address = Mapper.Map(cifReferenceDto.PermanentAddress, address);
                                GenService.Save(address);
                                cifReferenceDto.PermanentAddressId = address.Id;
                            }
                            else
                            {
                                var prAddress = Mapper.Map<Address>(cifReferenceDto.PermanentAddress);
                                GenService.Save(prAddress);
                                cifReferenceDto.PermanentAddressId = prAddress.Id;
                            }
                        }
                        else
                        {
                            cifReferenceDto.PermanentAddressId = oldEntries.PermanentAddressId;
                        }
                        if (cifReferenceDto.ResidenceAddress.IsChanged)
                        {
                            if (cifReferenceDto.ResidenceAddress.Id != null && cifReferenceDto.ResidenceAddress.Id > 0)
                            {
                                var address = GenService.GetById<Address>((long)cifReferenceDto.ResidenceAddress.Id);
                                cifReferenceDto.ResidenceAddress.CreateDate = address.CreateDate;
                                cifReferenceDto.ResidenceAddress.CreatedBy = address.CreatedBy;
                                address = Mapper.Map(cifReferenceDto.ResidenceAddress, address);
                                GenService.Save(address);
                                cifReferenceDto.ResidenceAddressId = address.Id;
                            }
                            else
                            {
                                var resAddress = Mapper.Map<Address>(cifReferenceDto.ResidenceAddress);
                                GenService.Save(resAddress);
                                cifReferenceDto.ResidenceAddressId = resAddress.Id;
                            }
                        }
                        else
                        {
                            cifReferenceDto.ResidenceAddressId = oldEntries.ResidenceAddressId;
                        }
                        if (cifReferenceDto.OfficeAddress.IsChanged)
                        {
                            if (cifReferenceDto.OfficeAddress.Id != null && cifReferenceDto.OfficeAddress.Id > 0)
                            {
                                var address = GenService.GetById<Address>((long)cifReferenceDto.OfficeAddress.Id);
                                cifReferenceDto.OfficeAddress.CreateDate = address.CreateDate;
                                cifReferenceDto.OfficeAddress.CreatedBy = address.CreatedBy;
                                address = Mapper.Map(cifReferenceDto.OfficeAddress, address);
                                GenService.Save(address);
                                cifReferenceDto.OfficeAddressId = address.Id;
                            }
                            else
                            {
                                var ofcAddress = Mapper.Map<Address>(cifReferenceDto.OfficeAddress);
                                GenService.Save(ofcAddress);
                                cifReferenceDto.OfficeAddressId = ofcAddress.Id;
                            }
                        }
                        else
                        {
                            cifReferenceDto.OfficeAddressId = oldEntries.OfficeAddressId;
                        }
                        #endregion

                        oldEntries = Mapper.Map<CIF_Reference>(cifReferenceDto);
                        oldEntries.EditDate = DateTime.Now;
                        oldEntries.EditedBy = UserId;
                        GenService.Save(oldEntries);
                        responce.Id = oldEntries.Id;
                        #endregion
                    }

                    else
                    {
                        #region New Entry
                        var entity = Mapper.Map<CIF_Reference>(cifReferenceDto);
                        entity.CreateDate = DateTime.Now;
                        entity.CreatedBy = UserId;
                        entity.Status = EntityStatus.Active;
                        var residenceAddress = Mapper.Map<Address>(cifReferenceDto.ResidenceAddress);
                        var permanentAddress = Mapper.Map<Address>(cifReferenceDto.PermanentAddress);
                        var officeAddress = Mapper.Map<Address>(cifReferenceDto.OfficeAddress);
                        using (var tran = new TransactionScope())
                        {
                            try
                            {
                                if (residenceAddress.CountryId != null)
                                {
                                    GenService.Save(residenceAddress);
                                    entity.ResidenceAddressId = residenceAddress.Id;
                                }
                                if (permanentAddress.CountryId != null)
                                {
                                    GenService.Save(permanentAddress);
                                    entity.PermanentAddressId = permanentAddress.Id;
                                }
                                if (officeAddress.CountryId != null)
                                {
                                    GenService.Save(officeAddress);
                                    entity.OfficeAddressId = officeAddress.Id;
                                }
                                GenService.Save(entity);
                                GenService.SaveChanges();
                                tran.Complete();
                                responce.Id = entity.Id;
                            }
                            catch (Exception ex)
                            {
                                tran.Dispose();
                                responce.Message = "Client Information Save Failed for " + cifReferenceDto.Name;
                                return responce;
                            }
                        }
                        #endregion
                    }
                }
                else
                {
                    responce.Message = "CIF Not Found for " + cifReferenceDto.Name;
                }
            }

            responce.Message = "Reference Saved Successfully";
            return responce;
        }
        public ResponseDto SaveCIFNetWorth(CIF_NetWorthDto dto, long UserId)
        {
            ResponseDto responce = new ResponseDto();

            if (dto.CIF_PersonalId > 0)
            {
                var entity = new CIF_NetWorth();
                //foreach (var item in oldEntries)
                //{
                //    item.EditDate = DateTime.Now;
                //    item.EditedBy = UserId;
                //    item.Status = EntityStatus.Inactive;
                //}
                if (dto.Id > 0)
                {
                    entity = GenService.GetById<CIF_NetWorth>((long)dto.Id);
                    dto.CreateDate = entity.CreateDate;
                    dto.CreatedBy = entity.CreatedBy;
                    dto.Status = EntityStatus.Active;
                    using (var tran = new TransactionScope())
                    {
                        try
                        {
                            if (dto.SavingsInBank != null)
                            {
                                foreach (var cifSavingsInBankDto in dto.SavingsInBank)
                                {
                                    if (cifSavingsInBankDto.Id > 0)
                                    {
                                        var sib = GenService.GetById<CIF_SavingsInBank>((long)cifSavingsInBankDto.Id);//entity.Owners.Where(o => o.Id == item.Id).FirstOrDefault();//
                                        if (cifSavingsInBankDto.Status == null)
                                            cifSavingsInBankDto.Status = sib.Status;
                                        sib = Mapper.Map(cifSavingsInBankDto, sib);
                                        sib.CIF_NetWorthId = entity.Id;
                                        sib.EditDate = DateTime.Now;
                                        sib.EditedBy = UserId;
                                        GenService.Save(sib);
                                    }
                                    else
                                    {
                                        cifSavingsInBankDto.Status = EntityStatus.Active;
                                        cifSavingsInBankDto.CIF_NetWorthId = entity.Id;
                                        cifSavingsInBankDto.CreatedBy = UserId;
                                        var savings = Mapper.Map<CIF_SavingsInBank>(cifSavingsInBankDto);
                                        GenService.Save(savings);
                                    }
                                }
                            }
                            if (dto.RemovedSavings != null)
                            {
                                foreach (var item in dto.RemovedSavings)
                                {
                                    var sv = GenService.GetById<CIF_SavingsInBank>(item);//entity.Owners.Where(o => o.Id == item).FirstOrDefault();//
                                    if (sv != null)
                                    {
                                        sv.Status = EntityStatus.Inactive;
                                        sv.EditDate = DateTime.Now;
                                        sv.EditedBy = UserId;
                                    }
                                    GenService.Save(sv);
                                }
                            }

                            if (dto.Investments != null)
                            {
                                foreach (var inv in dto.Investments)
                                {
                                    if (inv.Id > 0)
                                    {
                                        var investment = GenService.GetById<CIF_Investment>((long)inv.Id);//entity.Owners.Where(o => o.Id == item.Id).FirstOrDefault();//
                                        if (inv.Status == null)
                                            inv.Status = investment.Status;
                                        investment = Mapper.Map(inv, investment);
                                        investment.CIF_NetWorthId = entity.Id;
                                        investment.EditDate = DateTime.Now;
                                        investment.EditedBy = UserId;
                                        GenService.Save(investment);
                                    }
                                    else
                                    {
                                        inv.Status = EntityStatus.Active;
                                        inv.CIF_NetWorthId = entity.Id;
                                        inv.CreatedBy = UserId;
                                        var investments = Mapper.Map<CIF_Investment>(inv);
                                        GenService.Save(investments);
                                    }
                                }
                            }
                            if (dto.RemovedInvestments != null)
                            {
                                foreach (var item in dto.RemovedInvestments)
                                {
                                    var inv = GenService.GetById<CIF_Investment>(item);//entity.Owners.Where(o => o.Id == item).FirstOrDefault();//
                                    if (inv != null)
                                    {
                                        inv.Status = EntityStatus.Inactive;
                                        inv.EditDate = DateTime.Now;
                                        inv.EditedBy = UserId;
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
                                        var property = GenService.GetById<CIF_NW_Property>((long)prop.Id);//entity.Owners.Where(o => o.Id == item.Id).FirstOrDefault();//
                                        if (prop.Status == null)
                                            prop.Status = property.Status;
                                        property = Mapper.Map(prop, property);
                                        property.CIF_NetWorthId = entity.Id;
                                        property.EditDate = DateTime.Now;
                                        property.EditedBy = UserId;
                                        GenService.Save(property);
                                    }
                                    else
                                    {
                                        prop.Status = EntityStatus.Active;
                                        prop.CIF_NetWorthId = entity.Id;
                                        prop.CreatedBy = UserId;
                                        var property = Mapper.Map<CIF_NW_Property>(prop);
                                        GenService.Save(property);
                                    }
                                }
                            }
                            if (dto.RemovedProperties != null)
                            {
                                foreach (var item in dto.RemovedProperties)
                                {
                                    var prop = GenService.GetById<CIF_NW_Property>(item);//entity.Owners.Where(o => o.Id == item).FirstOrDefault();//
                                    if (prop != null)
                                    {
                                        prop.Status = EntityStatus.Inactive;
                                        prop.EditDate = DateTime.Now;
                                        prop.EditedBy = UserId;
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
                                        var share = GenService.GetById<CIF_BusinessShares>((long)bs.Id);//entity.Owners.Where(o => o.Id == item.Id).FirstOrDefault();//
                                        if (bs.Status == null)
                                            bs.Status = share.Status;
                                        share = Mapper.Map(bs, share);
                                        share.CIF_NetWorthId = entity.Id;
                                        share.EditDate = DateTime.Now;
                                        share.EditedBy = UserId;
                                        GenService.Save(share);
                                    }
                                    else
                                    {
                                        bs.Status = EntityStatus.Active;
                                        bs.CIF_NetWorthId = entity.Id;
                                        bs.CreatedBy = UserId;
                                        var share = Mapper.Map<CIF_BusinessShares>(bs);
                                        GenService.Save(share);
                                    }
                                }
                            }
                            if (dto.RemovedShareinBusines != null)
                            {
                                foreach (var item in dto.RemovedShareinBusines)
                                {
                                    var bshare = GenService.GetById<CIF_BusinessShares>(item);//entity.Owners.Where(o => o.Id == item).FirstOrDefault();//
                                    if (bshare != null)
                                    {
                                        bshare.Status = EntityStatus.Inactive;
                                        bshare.EditDate = DateTime.Now;
                                        bshare.EditedBy = UserId;
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
                                        var liability = GenService.GetById<CIF_Liability>((long)lb.Id);//entity.Owners.Where(o => o.Id == item.Id).FirstOrDefault();//
                                        if (lb.Status == null)
                                            lb.Status = liability.Status;
                                        liability = Mapper.Map(lb, liability);
                                        liability.CIF_NetWorthId = entity.Id;
                                        liability.EditDate = DateTime.Now;
                                        liability.EditedBy = UserId;
                                        GenService.Save(liability);
                                    }
                                    else
                                    {
                                        lb.Status = EntityStatus.Active;
                                        lb.CIF_NetWorthId = entity.Id;
                                        lb.CreatedBy = UserId;
                                        var liability = Mapper.Map<CIF_Liability>(lb);
                                        GenService.Save(liability);
                                    }
                                }
                            }
                            if (dto.RemovedLiabilities != null)
                            {
                                foreach (var item in dto.RemovedLiabilities)
                                {
                                    var lib = GenService.GetById<CIF_Liability>(item);//entity.Owners.Where(o => o.Id == item).FirstOrDefault();//
                                    if (lib != null)
                                    {
                                        lib.Status = EntityStatus.Inactive;
                                        lib.EditDate = DateTime.Now;
                                        lib.EditedBy = UserId;
                                    }
                                    GenService.Save(lib);
                                }
                            }
                            entity = Mapper.Map(dto, entity);
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
                    responce.Id = entity.Id;
                    responce.Message = "Net Worth Edited Successfully";

                }
                else
                {
                    using (var tran = new TransactionScope())
                    {
                        //entity = new CIF_NetWorth();
                        try
                        {
                            var oldEntries = GenService.GetAll<CIF_NetWorth>().Where(c => c.CIF_PersonalId == dto.CIF_PersonalId && c.Status == EntityStatus.Active).ToList();
                            oldEntries.Select(c => { c.EditedBy = UserId; c.Status = EntityStatus.Inactive; return c; });
                            entity = Mapper.Map<CIF_NetWorth>(dto);
                            entity.Status = EntityStatus.Active;
                            entity.CreatedBy = UserId;
                            entity.CreateDate = DateTime.Now;
                            GenService.Save(entity);
                            if (dto.SavingsInBank != null)
                            {
                                foreach (var cifSavingsInBankDto in dto.SavingsInBank)
                                {
                                    cifSavingsInBankDto.Status = EntityStatus.Active;
                                    cifSavingsInBankDto.CIF_NetWorthId = entity.Id;
                                    cifSavingsInBankDto.CreatedBy = UserId;
                                    var savings = Mapper.Map<CIF_SavingsInBank>(cifSavingsInBankDto);
                                    GenService.Save(savings);
                                }

                            }

                            if (dto.Investments != null)
                            {
                                foreach (var inv in dto.Investments)
                                {
                                    inv.Status = EntityStatus.Active;
                                    inv.CIF_NetWorthId = entity.Id;
                                    inv.CreatedBy = UserId;
                                    var investments = Mapper.Map<CIF_Investment>(inv);
                                    GenService.Save(investments);
                                }

                            }

                            if (dto.Properties != null)
                            {
                                foreach (var prop in dto.Properties)
                                {
                                    prop.Status = EntityStatus.Active;
                                    prop.CIF_NetWorthId = entity.Id;
                                    prop.CreatedBy = UserId;
                                    var property = Mapper.Map<CIF_NW_Property>(prop);
                                    GenService.Save(property);
                                }


                            }

                            if (dto.BusinessShares != null)
                            {
                                foreach (var bs in dto.BusinessShares)
                                {
                                    bs.Status = EntityStatus.Active;
                                    bs.CIF_NetWorthId = entity.Id;
                                    bs.CreatedBy = UserId;
                                    var share = Mapper.Map<CIF_BusinessShares>(bs);
                                    GenService.Save(share);
                                }


                            }
                            if (dto.Liabilities != null)
                            {
                                foreach (var lb in dto.Liabilities)
                                {
                                    lb.Status = EntityStatus.Active;
                                    lb.CIF_NetWorthId = entity.Id;
                                    lb.CreatedBy = UserId;
                                    var liability = Mapper.Map<CIF_Liability>(lb);
                                    GenService.Save(liability);
                                }


                            }
                            tran.Complete();
                            responce.Success = true;
                            responce.Id = entity.Id;
                            responce.Message = "Client Net Worth Saved Successfully";
                        }
                        catch (Exception ex)
                        {
                            tran.Dispose();
                            responce.Message = "Client Net Worth Save Failed";
                        }

                    }
                }
            }
            else
            {
                responce.Message = "CIF Not Found";
            }
            GenService.SaveChanges();
            return responce;
        }
        public ResponseDto SaveBankAccountsAndCreditCards(CIFCardAndBanksVM dto)
        {
            var response = new ResponseDto();
            var bankAccounts = new List<BankAccount>();
            var creditCards = new List<CreditCard>();
            using(var tran = new TransactionScope())
            {
                try
                {
                    if(dto.RemovedBankAccounts != null && dto.RemovedBankAccounts.Count > 0)
                    {
                        #region remove bank accounts
                        foreach(var item in dto.RemovedBankAccounts)
                        {
                            var oldEntry = GenService.GetById<BankAccount>(item);
                            if(oldEntry != null)
                            {
                                oldEntry.EditDate = DateTime.Now;
                                oldEntry.EditedBy = dto.UserId;
                                oldEntry.Status = EntityStatus.Inactive;
                                bankAccounts.Add(oldEntry);
                            }
                        }
                        response.Message = "Bank accounts removed. ";
                        #endregion
                    }
                    if (dto.RemovedCreditCards != null && dto.RemovedCreditCards.Count > 0)
                    {
                        #region remove credit cards
                        foreach(var item in dto.RemovedCreditCards)
                        {
                            var oldEntry = GenService.GetById<CreditCard>(item);
                            if(oldEntry != null)
                            {
                                oldEntry.EditDate = DateTime.Now;
                                oldEntry.EditedBy = dto.UserId;
                                oldEntry.Status = EntityStatus.Inactive;
                                creditCards.Add(oldEntry);
                            }
                        }
                        response.Message += "Credit cards removed. ";
                        #endregion
                    }

                    if(dto.BankAccountDtos != null && dto.BankAccountDtos.Count > 0)
                    {
                        foreach(var item in dto.BankAccountDtos)
                        {
                            BankAccount entity;
                            if (item.Id != null && item.Id > 0)
                            {
                                entity = GenService.GetById<BankAccount>((long)item.Id);
                                item.CreateDate = entity.CreateDate;
                                item.CreatedBy = entity.CreatedBy;
                                item.Status = entity.Status;
                                Mapper.Map(item, entity);
                            }
                            else
                            {
                                entity = new BankAccount();
                                entity = Mapper.Map<BankAccount>(item);
                                entity.CreateDate = DateTime.Now;
                                entity.CreatedBy = dto.UserId;
                                entity.Status = EntityStatus.Active;
                            }
                            bankAccounts.Add(entity);
                        }
                        response.Message += "New bank accounts added. ";
                    }

                    if(dto.CreditCardDtos != null && dto.CreditCardDtos.Count > 0)
                    {
                        foreach(var item in dto.CreditCardDtos)
                        {
                            CreditCard entity;
                            if (item.Id != null && item.Id > 0)
                            {
                                entity = GenService.GetById<CreditCard>((long)item.Id);
                                item.CreateDate = entity.CreateDate;
                                item.CreatedBy = entity.CreatedBy;
                                item.Status = entity.Status;
                                Mapper.Map(item, entity);
                            }
                            else
                            {
                                entity = new CreditCard();
                                entity = Mapper.Map<CreditCard>(item);
                                entity.CreateDate = DateTime.Now;
                                entity.CreatedBy = dto.UserId;
                                entity.Status = EntityStatus.Active;
                            }
                            creditCards.Add(entity);
                        }
                        response.Message += "New credit card informations added.";
                    }

                    GenService.Save(bankAccounts);
                    GenService.Save(creditCards);
                    //GenService.SaveChanges();
                    tran.Complete();
                    response.Success = true;
                }
                catch(Exception ex)
                {
                    response.Message = ex.Message;
                    tran.Dispose();
                }
            }

            return response;
        }
        public CIFCardAndBanksVM GetBankAccountsAndCreditCards(long cifId)
        {
            CIFCardAndBanksVM result = new CIFCardAndBanksVM();

            var cif = GenService.GetById<CIF_Personal>(cifId);
            var bankAccounts = cif.BankAccounts.Where(b => b.Status == EntityStatus.Active).ToList();
            var creditCards = cif.CreditCards.Where(b => b.Status == EntityStatus.Active).ToList();
            if (bankAccounts != null && bankAccounts.Count > 0)
                result.BankAccountDtos = Mapper.Map<List<BankAccountDto>>(bankAccounts);
            if (creditCards != null && creditCards.Count > 0)
                result.CreditCardDtos = Mapper.Map<List<CreditCardDto>>(creditCards);

            return result;
        }

        public CIF_PersonalDto GetCIF_Info(long cifId)
        {
            //var cifData = GenService.GetAll<CIF_Personal>().FirstOrDefault(r => r.Id == cifId);
            var cifData = GenService.GetById<CIF_Personal>(cifId);
            var result = Mapper.Map<CIF_PersonalDto>(cifData);
            result.PhotoName = cifData.Photo != null ? Path.GetFileName(cifData.Photo) : "" ;
            result.SignaturePhotoName = cifData.SignaturePhoto != null ? Path.GetFileName(cifData.SignaturePhoto) : "";
            result.ProfessionName = cifData.OccupationId != null && cifData.Occupation.OccupationType > 0 ?
                UiUtil.GetDisplayName(cifData.Occupation.OccupationType) : "";
            return result;
        }

        public CIF_OrganizationalDto GetCIFOrganizational(long cifOrgId)
        {
            var cifData = GenService.GetAll<CIF_Organizational>().FirstOrDefault(r => r.Id == cifOrgId);
            var result = Mapper.Map<CIF_OrganizationalDto>(cifData);
            //result.BusinessType = UiUtil.GetDisplayName();
            result.FactoryAddress.RemoveAll(f => f.Status != EntityStatus.Active);
            result.Owners.RemoveAll(o => o.Status != EntityStatus.Active);

            return result;
        }

        public ResponseDto SaveClientOccupation(OccupationDto dto, long userId)
        {
            var entity = new Occupation();
            var address = new Address();
            ResponseDto responce = new ResponseDto();
            if (dto.CIF_PersonalId <= 0)
            {
                responce.Message = "Please Give Entry Of CIF Personal First";
                return responce;

            }
            if (dto.Id != null)
            {
                entity = GenService.GetById<Occupation>((long)dto.Id);
                dto.CreateDate = entity.CreateDate;
                dto.CreatedBy = entity.CreatedBy;
                dto.Status = EntityStatus.Active;
                using (var tran = new TransactionScope())
                {
                    try
                    {
                        if (dto.OfficeAddress.IsChanged)
                        {
                            if (dto.OfficeAddress.Id != null)
                            {
                                address = GenService.GetById<Address>((long)dto.OfficeAddress.Id);
                                dto.OfficeAddress.CreateDate = address.CreateDate;
                                dto.OfficeAddress.CreatedBy = address.CreatedBy;
                                address = Mapper.Map(dto.OfficeAddress, address);
                                GenService.Save(address);
                                dto.OfficeAddressId = address.Id;
                            }
                            else
                            {
                                var permanentAddress = Mapper.Map<Address>(dto.OfficeAddress);
                                GenService.Save(permanentAddress);
                                dto.OfficeAddressId = permanentAddress.Id;
                            }
                        }
                        else
                        {
                            dto.OfficeAddressId = entity.OfficeAddressId;
                            //if (dto.OccupationType == OccupationType.Business || dto.OccupationType == OccupationType.Private || dto.OccupationType == OccupationType.Govt)
                            //{
                            //    dto.OfficeAddressId = entity.OfficeAddressId;
                            //}  
                        }
                        if (dto.OccupationType == OccupationType.LandOwner)
                        {
                            if (entity.LandOwnerProperties.Count == 0 && dto.LandOwnerProperties != null)
                            {
                                dto.LandOwnerProperties.Select(c => { c.OccupationId = entity.Id; return c; }).ToList();
                                var landProp = Mapper.Map<List<LandOwnerProperty>>(dto.LandOwnerProperties);
                                GenService.Save(landProp);
                            }
                            else
                            {
                                var landOwnersprop = new List<LandOwnerProperty>();
                                foreach (var item in dto.LandOwnerProperties)
                                {
                                    LandOwnerProperty ownerProp;
                                    if (item.Id != null && item.Id > 0)
                                    {
                                        ownerProp = GenService.GetById<LandOwnerProperty>((long)item.Id);//entity.Owners.Where(o => o.Id == item.Id).FirstOrDefault();//
                                        if (item.Status == null)
                                            item.Status = ownerProp.Status;
                                        ownerProp = Mapper.Map(item, ownerProp);
                                        //ownerProp.Status = EntityStatus.Active;
                                        ownerProp.OccupationId = entity.Id;
                                        //owner.CIF_Org_OwnersRole = (CIF_Org_OwnersRole)item.CIF_Org_OwnersRole;
                                        ownerProp.EditDate = DateTime.Now;
                                        ownerProp.EditedBy = userId;
                                        GenService.Save(ownerProp);
                                    }
                                    else
                                    {
                                        ownerProp = new LandOwnerProperty();
                                        ownerProp = Mapper.Map<LandOwnerProperty>(item);
                                        ownerProp.OccupationId = entity.Id;
                                        ownerProp.Status = EntityStatus.Active;
                                        ownerProp.CreatedBy = userId;
                                        ownerProp.CreateDate = DateTime.Now;
                                        GenService.Save(ownerProp);
                                    }
                                }

                                if (dto.RemovedLandOwnerProp != null)
                                {
                                    foreach (var item in dto.RemovedLandOwnerProp)
                                    {
                                        var owner = GenService.GetById<LandOwnerProperty>(item);//entity.Owners.Where(o => o.Id == item).FirstOrDefault();//
                                        if (owner != null)
                                        {
                                            owner.Status = EntityStatus.Inactive;
                                            owner.EditDate = DateTime.Now;
                                            owner.EditedBy = userId;
                                        }
                                        GenService.Save(owner);
                                    }
                                }

                            }

                        }
                        entity = Mapper.Map(dto, entity);
                        entity.EditDate = DateTime.Now;
                        GenService.Save(entity);
                        CIF_Personal cif = GenService.GetById<CIF_Personal>(dto.CIF_PersonalId);
                        cif.OccupationId = entity.Id;
                        GenService.Save(cif);
                        tran.Complete();
                    }
                    catch (Exception ex)
                    {
                        tran.Dispose();
                        responce.Message = "Occupation Edit Operation Failed";
                    }
                }
                responce.Success = true;
                responce.Message = "Occupation Edited Successfully";
            }
            else
            {

                entity = Mapper.Map<Occupation>(dto);
                //entity.CIFNo = _sequencer.GetUpdatedCIFPersonalNo();
                entity.Status = EntityStatus.Active;
                entity.CreatedBy = userId;
                entity.CreateDate = DateTime.Now;
                var officeAddress = Mapper.Map<Address>(dto.OfficeAddress);

                using (var tran = new TransactionScope())
                {
                    try
                    {
                        if (officeAddress.CountryId != null)
                        {
                            GenService.Save(officeAddress);
                            entity.OfficeAddressId = officeAddress.Id;
                        }
                        GenService.Save(entity);
                        if (dto.LandOwnerProperties != null)
                        {
                            dto.LandOwnerProperties.Select(c => { c.OccupationId = entity.Id; return c; }).ToList();
                            var landProp = Mapper.Map<List<LandOwnerProperty>>(dto.LandOwnerProperties);
                            GenService.Save(landProp);
                        }
                        CIF_Personal cif = GenService.GetById<CIF_Personal>(dto.CIF_PersonalId);
                        cif.OccupationId = entity.Id;
                        GenService.Save(cif);
                        tran.Complete();
                    }
                    catch (Exception ex)
                    {
                        tran.Dispose();
                        responce.Message = "Occupation Save Failed";
                    }
                }

                responce.Success = true;
                responce.Message = "Occupation Saved Successfully";
            }
            return responce;
        }
        public object GetAllCif()
        {
            List<CIF_PersonalDto> cifAll = GenService.GetAll<CIF_Personal>().Select(r => new CIF_PersonalDto
            {
                Id = r.Id,
                ConcatName = r.CIFNo + "-" + r.Name,
                Name = r.Name
            }).ToList();
            return cifAll;
        }

        public List<CIF_PersonalDto> GetCIF_InfoWithAge()
        {
            List<CIF_PersonalDto> cifAllWithAge = GenService.GetAll<CIF_Personal>().Select(r => new CIF_PersonalDto
            {
                Id = r.Id,
                Name = r.CIFNo + "-" + r.Name + "- (Age-" + (DateTime.Now - r.DateOfBirth) + ")"
            }).ToList();
            return cifAllWithAge;
        }
        public List<CIF_PersonalDto> GetAllCIFPersons()
        {
            var settings = GenService.GetAll<CIF_Personal>().ToList();
            return Mapper.Map<List<CIF_PersonalDto>>(settings);
        }

        public CIF_PersonalDto GetCifByguardiaId(long guardId)
        {
            var settings = GenService.GetById<CIF_Personal>(guardId);// <CIF_Personal>().ToList();
            return Mapper.Map<CIF_PersonalDto>(settings);
        }
        public object GetAllCifOrgList()
        {
            var result = GenService.GetAll<CIF_Organizational>().Where(o => o.Status == EntityStatus.Active).Select(o=> new { CIFNo = o.CIFNo, Id = o.Id, Name = o.CompanyId != null ? o.Company.Name : o.CompanyName}).ToList();
            //var temp = Mapper.Map<List<CIF_OrganizationalDto>>(result);
            return result;
        }

        public ResponseDto SaveCifOrganizational(CIF_OrganizationalDto dto, long UserId)
        {
            var entity = new CIF_Organizational();
            //var address = new Address();
            ResponseDto response = new ResponseDto();
            if (dto.Id != null && dto.Id > 0)
            {
                entity = GenService.GetById<CIF_Organizational>((long)dto.Id);
                dto.CIFNo = entity.CIFNo;
                dto.CreateDate = entity.CreateDate;
                dto.CreatedBy = entity.CreatedBy;
                if (dto.Status == null)
                    dto.Status = entity.Status;
                using (var tran = new TransactionScope())
                {
                    try
                    {
                        Mapper.Map(dto, entity);
                        #region address updates

                        if (dto.RegAddress.IsChanged)
                        {
                            if (dto.RegAddress.Id != null)
                            {
                                var regAddress = new Address();
                                regAddress = GenService.GetById<Address>((long)dto.RegAddress.Id);
                                dto.RegAddress.CreateDate = regAddress.CreateDate;
                                dto.RegAddress.CreatedBy = regAddress.CreatedBy;
                                dto.RegAddress.Status = regAddress.Status;
                                Mapper.Map(dto.RegAddress, regAddress);
                                GenService.Save(regAddress);
                                dto.RegAddressId = regAddress.Id;
                            }
                            else
                            {
                                var regAddress = Mapper.Map<Address>(dto.RegAddress);
                                if (regAddress.CountryId != null)
                                {
                                    GenService.Save(regAddress);
                                    entity.RegAddressId = regAddress.Id;
                                }
                                
                            }
                        }

                        if (dto.OfficeAddress.IsChanged)
                        {
                            if (dto.OfficeAddress.Id != null)
                            {
                                var offAddress = new Address();
                                offAddress = GenService.GetById<Address>((long)dto.OfficeAddress.Id);
                                dto.OfficeAddress.CreateDate = offAddress.CreateDate;
                                dto.OfficeAddress.CreatedBy = offAddress.CreatedBy;
                                dto.OfficeAddress.Status = offAddress.Status;
                                Mapper.Map(dto.OfficeAddress, offAddress);
                                GenService.Save(offAddress);
                                dto.OfficeAddressId = offAddress.Id;
                            }
                            else
                            {
                                var officeAddress = Mapper.Map<Address>(dto.OfficeAddress);
                                if (officeAddress.CountryId != null)
                                {
                                    GenService.Save(officeAddress);
                                    entity.OfficeAddressId = officeAddress.Id;
                                }
                            }
                        }

                        #endregion

                        
                        GenService.Save(entity);
                        //var org_owners = new List<CIF_Org_Owners>();
                        if (dto.Owners != null)
                        {
                            foreach (var item in dto.Owners)
                            {
                                CIF_Org_Owners owner;
                                if (item.Id != null && item.Id > 0)
                                {
                                    owner = GenService.GetById<CIF_Org_Owners>((long)item.Id);
                                    //entity.Owners.Where(o => o.Id == item.Id).FirstOrDefault();//

                                    owner.CIF_PersonalId = (long)item.CIF_PersonalId;
                                    owner.CIF_Org_OwnersRole = (CIF_Org_OwnersRole)item.CIF_Org_OwnersRole;
                                    owner.EditDate = DateTime.Now;
                                    owner.EditedBy = UserId;
                                    GenService.Save(owner);
                                }
                                else
                                {
                                    owner = new CIF_Org_Owners();
                                    owner = Mapper.Map<CIF_Org_Owners>(item);
                                    owner.Status = EntityStatus.Active;
                                    owner.CreatedBy = UserId;
                                    owner.CreateDate = DateTime.Now;
                                    owner.CIF_OrganizationalId = entity.Id;
                                    GenService.Save(owner);
                                }
                            }
                        }
                        //entity.FactoryAddress = new List<FactoryAddress>();
                        if (dto.FactoryAddress != null)
                        {
                            foreach (var item in dto.FactoryAddress)
                            {
                                if (item.Id != null && item.Id > 0)
                                {
                                    var obj = entity.FactoryAddress.Where(o => o.Id == item.Id).FirstOrDefault();
                                    //GenService.GetById<CIF_Org_Owners>((long)item.Id);
                                    item.Status = obj.Status;
                                    item.CreateDate = obj.CreateDate;
                                    item.CreatedBy = obj.CreatedBy;
                                    item.EditDate = DateTime.Now;
                                    item.EditedBy = UserId;
                                    if (item.Address.IsChanged)
                                    {
                                        Address address;
                                        if (item.AddressId != null)
                                        {
                                            address = GenService.GetById<Address>((long)item.Address.Id);
                                            //obj.Address;
                                            item.Address.CreateDate = address.CreateDate;
                                            item.Address.CreatedBy = address.CreatedBy;
                                            item.Address.Status = address.Status;
                                            Mapper.Map(item.Address, address);
                                        }
                                        else
                                        {
                                            address = new Address();
                                            Mapper.Map(item.Address, address);
                                            address.CreateDate = DateTime.Now;
                                            address.CreatedBy = UserId;
                                            address.Status = EntityStatus.Active;
                                        }
                                        item.AddressId = address.Id;
                                        GenService.Save(address);
                                    }
                                    item.CIF_OrganizationalId = entity.Id;
                                    Mapper.Map(item, obj);
                                    GenService.Save(obj);
                                }
                                else
                                {
                                    item.Status = EntityStatus.Active;
                                    var additionalAddress = new Address();
                                    Mapper.Map(item.Address, additionalAddress);
                                    additionalAddress.CreatedBy = UserId;
                                    additionalAddress.CreateDate = DateTime.Now;
                                    additionalAddress.Status = EntityStatus.Active;
                                    GenService.Save(additionalAddress);

                                    item.AddressId = additionalAddress.Id;
                                    item.CreatedBy = UserId;
                                    item.CreateDate = DateTime.Now;
                                    item.CIF_OrganizationalId = entity.Id;
                                    //entity.FactoryAddress.Add(Mapper.Map<FactoryAddress>(item));
                                    var facAddress = new FactoryAddress();
                                    Mapper.Map(item, facAddress);
                                    GenService.Save(facAddress);
                                }
                            }
                        }

                        if (dto.RemovedOwners != null)
                        {
                            foreach (var item in dto.RemovedOwners)
                            {
                                var owner = GenService.GetById<CIF_Org_Owners>(item);//entity.Owners.Where(o => o.Id == item).FirstOrDefault();//
                                if (owner != null)
                                {
                                    owner.Status = EntityStatus.Inactive;
                                    owner.EditDate = DateTime.Now;
                                    owner.EditedBy = UserId;
                                }
                                GenService.Save(owner);
                            }
                        }
                        if (dto.RemovedFactoryAddress != null)
                        {
                            foreach (var item in dto.RemovedFactoryAddress)
                            {
                                var facAddress = GenService.GetById<FactoryAddress>(item);//entity.FactoryAddress.Where(o => o.Id == item).FirstOrDefault();//
                                if (facAddress != null)
                                {
                                    facAddress.Status = EntityStatus.Inactive;
                                    facAddress.EditDate = DateTime.Now;
                                    facAddress.EditedBy = UserId;
                                }
                                GenService.Save(facAddress);
                                //entity.FactoryAddress.Add(facAddress);
                            }
                        }

                        tran.Complete();
                    }
                    catch (Exception ex)
                    {
                        tran.Dispose();
                        response.Message = "CIF Organizational Edit Operation Failed";
                        return response;
                    }
                }
                response.Success = true;
                response.Message = "CIF Organizational Edited Successfully";
            }
            else
            {

                entity = Mapper.Map<CIF_Organizational>(dto);
                //entity.CIFNo = _sequencer.GetUpdatedCIFPersonalNo();
                entity.Status = EntityStatus.Active;
                entity.CreatedBy = UserId;
                entity.CreateDate = DateTime.Now;
                var regAddress = Mapper.Map<Address>(dto.RegAddress);
                var officeAddress = Mapper.Map<Address>(dto.OfficeAddress);

                using (var tran = new TransactionScope())
                {
                    try
                    {
                        if (regAddress.CountryId != null)
                        {
                            GenService.Save(regAddress);
                            entity.RegAddressId = regAddress.Id;
                        }
                        if (officeAddress.CountryId != null)
                        {
                            GenService.Save(officeAddress);
                            entity.OfficeAddressId = officeAddress.Id;
                        }

                        if (dto.FactoryAddress != null)
                        {
                            entity.FactoryAddress = new List<FactoryAddress>();
                            for (int i = 0; i < dto.FactoryAddress.Count; i++)
                            {
                                var fAddress = new FactoryAddress();
                                var fAddressAddress = Mapper.Map<Address>(dto.FactoryAddress[i].Address);
                                fAddressAddress.Status = EntityStatus.Active;
                                fAddressAddress.CreateDate = DateTime.Now;
                                fAddressAddress.CreatedBy = UserId;

                                if (fAddressAddress.CountryId != null)
                                {
                                    GenService.Save(fAddressAddress);
                                    fAddress.AddressId = fAddressAddress.Id;
                                    fAddress.Status = EntityStatus.Active;
                                    fAddress.CreateDate = DateTime.Now;
                                    fAddress.CreatedBy = UserId;
                                }
                                entity.FactoryAddress.Add(fAddress);
                            }
                        }

                        if (dto.Owners != null)
                        {
                            entity.Owners = new List<CIF_Org_Owners>();
                            foreach (var item in dto.Owners)
                            {
                                item.CreatedBy = UserId;
                                item.CreateDate = DateTime.Now;
                                item.Status = EntityStatus.Active;
                                entity.Owners.Add(Mapper.Map<CIF_Org_Owners>(item));
                            }
                        }

                        GenService.Save(entity);
                        entity.CIFNo = _sequencer.GetUpdatedCIFOrganizationalNo();
                        GenService.Save(entity);
                        tran.Complete();
                    }
                    catch (Exception ex)
                    {
                        tran.Dispose();
                        response.Message = "CIF Organizational Save Failed";
                    }
                }

                response.Success = true;
                response.Message = "CIF Organizational Saved Successfully";
            }
            return response;
        }

        public OccupationDto GetCIF_Occupation(long? personId)
        {
            OccupationDto data = new OccupationDto();
            if (personId != null)
            {
                var cif = GenService.GetById<CIF_Personal>((long)personId);
                if (cif.OccupationId != null)
                {
                    try
                    {
                        var occupation = GenService.GetById<Occupation>((long)cif.OccupationId);
                        data = Mapper.Map<OccupationDto>(occupation);
                        data.LandOwnerProperties.RemoveAll(f => f.Status != EntityStatus.Active);
                        return data;
                    }
                    catch (Exception ex)
                    {

                    }

                }

            }
            return data;
        }

        public CIF_IncomeStatementDto LoadIncomeStatement(long cifPersonId)
        {
            var incomeStmt =
                GenService.GetAll<CIF_IncomeStatement>()
                    .Where(r => r.CIF_PersonalId == cifPersonId && r.Status == EntityStatus.Active)
                    .OrderByDescending(r => r.Id)
                    .FirstOrDefault();
            var result =  Mapper.Map<CIF_IncomeStatementDto>(incomeStmt);
            if (result != null)
            {
                result.MonthlyOtherIncomesDeclared.RemoveAll(f => f.Status != EntityStatus.Active);
            }
            return result;
        }

        public CIF_NetWorthDto LoadNetWorth(long cifPersonId)
        {
            var networth =
                  GenService.GetAll<CIF_NetWorth>()
                    .Where(r => r.CIF_PersonalId == cifPersonId && r.Status == EntityStatus.Active)
                    .OrderByDescending(r => r.Id)
                    .FirstOrDefault();
            CIF_NetWorthDto data = Mapper.Map<CIF_NetWorthDto>(networth);
            if(data != null)
            {
                data.SavingsInBank.RemoveAll(f => f.Status != EntityStatus.Active);
                data.Investments.RemoveAll(f => f.Status != EntityStatus.Active);
                data.Properties.RemoveAll(f => f.Status != EntityStatus.Active);
                data.Liabilities.RemoveAll(f => f.Status != EntityStatus.Active);
                data.BusinessShares.RemoveAll(f => f.Status != EntityStatus.Active);
            }

            return data;
        }

        public long GetNetWorthIdbyCifId(long cifId)
        {
            var netWorthId =
                GenService.GetAll<CIF_NetWorth>()
                    .Where(r => r.CIF_PersonalId == cifId && r.Status == EntityStatus.Active)
                    .FirstOrDefault();
            if (netWorthId!=null)
                return netWorthId.Id;
            return 0;
        }

        public IPagedList<CIF_PersonalDto> GetCifPagedList(int pageSize, int pageCount, string searchString)
        {
            var allCif = GenService.GetAll<CIF_Personal>().Where(s => s.Status == EntityStatus.Active).Select(s => new CIF_PersonalDto
            {
                Id = s.Id,
                Name = s.Name,
                CIFNo = s.CIFNo,
                GenderName = s.Gender.ToString(),
                MaritalStatusName = s.MaritalStatus.ToString(),
                PassportNo = s.PassportNo,
                NIDNo = s.NIDNo,
                FathersName = s.FathersName,
                MothersName = s.MothersName
            });
            if (!string.IsNullOrEmpty(searchString))
            {
                searchString = searchString.ToLower();
                allCif = allCif.Where(s => s.Name.ToLower().Contains(searchString) 
                                           || s.NIDNo.ToLower().Contains(searchString) 
                                           || s.PassportNo.ToLower().Contains(searchString) 
                                           || s.CIFNo.ToLower().Contains(searchString)
                                           || s.FathersName.ToLower().Contains(searchString));
            }
            var temp = allCif.OrderByDescending(r => r.Id).ToPagedList(pageCount, pageSize);
            return temp;
        }

        public IPagedList<CIF_OrganizationalDto> GetCifOrgPagedList(int pageSize, int pageCount, string searchString)
        {
            var allCifOrg = GenService.GetAll<CIF_Organizational>().Where(s => s.Status == EntityStatus.Active).Select(s => new CIF_OrganizationalDto
            {
                Id = s.Id,
                CompanyName = s.CompanyId > 0 ? s.Company.Name : s.CompanyName,
                CIFNo = s.CIFNo,
                ETIN = s.ETIN,
                TradeLicenceNo = s.TradeLicenceNo,
                ContactPersonCellPhone = s.ContactPersonCellPhone,
                ContactPersonName = s.ContactPersonName
            });
            if (!string.IsNullOrEmpty(searchString))
            {
                searchString = searchString.ToLower();
                allCifOrg = allCifOrg.Where(s => s.CompanyName.ToLower().Contains(searchString) 
                                                 || s.CIFNo.ToLower().Contains(searchString) 
                                                 || s.TradeLicenceNo.ToLower().Contains(searchString) 
                                                 || s.ContactPersonCellPhone.ToLower().Contains(searchString));
            }
            
            var temp = allCifOrg.OrderBy(r => r.Id).ToPagedList(pageCount, pageSize);
            return temp;
        }

        public List<CIF_ReferenceDto> GetCIFReference(long cifId)
        {
            var cifRef =
                GenService.GetAll<CIF_Reference>()
                    .Where(r => r.CIF_PersonalId == cifId && r.Status == EntityStatus.Active).ToList();
            var data = Mapper.Map<List<CIF_ReferenceDto>>(cifRef);
            //var networth =
            //      GenService.GetAll<CIF_NetWorth>()
            //        .Where(r => r.CIF_PersonalId == cifPersonId && r.Status == EntityStatus.Active)
            //        .OrderByDescending(r => r.Id)
            //        .FirstOrDefault();
            //CIF_NetWorthDto data = Mapper.Map<CIF_NetWorthDto>(networth);
            //data.SavingsInBank.RemoveAll(f => f.Status != EntityStatus.Active);
            //data.Investments.RemoveAll(f => f.Status != EntityStatus.Active);
            //data.Properties.RemoveAll(f => f.Status != EntityStatus.Active);
            //data.Liabilities.RemoveAll(f => f.Status != EntityStatus.Active);
            //data.BusinessShares.RemoveAll(f => f.Status != EntityStatus.Active);
            return data;
        }

        public bool RemoveRef(long cifId)
        {
            var aRef = GenService.GetById<CIF_Reference>(cifId);
            aRef.Status = EntityStatus.Inactive;
            GenService.Save(aRef);
            return true;
        }

        public List<CIF_PersonalDto> GetCifpForAutoFill(string prefix, List<long> exclusionList)
        {
            if (exclusionList == null)
                exclusionList = new List<long>();
            prefix = prefix.ToLower();
            var data = GenService.GetAll<CIF_Personal>()
                .Where(x => x.Status == EntityStatus.Active && (x.CIFNo.ToLower().Contains(prefix) || x.Name.ToLower().Contains(prefix)) && !exclusionList.Contains(x.Id))
                .OrderBy(x => x.CIFNo)
                .Select(x => new CIF_PersonalDto { Id = x.Id, Name = x.Name, CIFNo = x.CIFNo })
                .Take(20)
                .ToList();
            return data;
        }

        public List<SectorCodeDto> GetSectorCodesForAutoFill(string prefix, SectorCodeType SectorCodeType)
        {
            prefix = prefix.ToLower();
            var data = GenService.GetAll<SectorCode>()
                .Where(x => x.Status == EntityStatus.Active && (x.Code.ToLower().Contains(prefix)) && x.SectorCodeType == SectorCodeType)
                .OrderBy(x => x.Code)
                .Take(20)
                .ToList();
            if(data != null)
                return Mapper.Map<List<SectorCodeDto>>(data);
            return null;
        }
        public ResponseDto UploadPicture(CIF_PersonalDto dto, long userId)
        {
            ResponseDto response = new ResponseDto();
            string filePath = "~/Content/image/cif/";//"~/Uploaded/ProfilePicture/";
            string path = HttpContext.Current.Server.MapPath(filePath);
            var cif = GenService.GetById<CIF_Personal>((long)dto.Id);
            try
            {
                if (dto.Photo != null)
                {
                    if (dto.Photo != null)
                    {
                       
                        if (!Directory.Exists(path))
                        {
                            Directory.CreateDirectory(path);
                        }
                        string customfileName = "Profile_Picture_" + cif.CIFNo;
                        string fileExt = Path.GetExtension(dto.Photo.FileName);
                        path = Path.Combine(path, customfileName + fileExt);
                        //path = Path.Combine(path, dto.Photo.FileName);
                        for (int i = 1; File.Exists(path);)
                        {
                            var length = path.Length;
                            if (!string.IsNullOrEmpty(fileExt))
                                path = path.Substring(0, (length - 3 - fileExt.Length)) + (++i).ToString("000") + fileExt;
                            else
                                path = path.Substring(0, (length - 3)) + (++i).ToString("000");
                        }
                        //path = Path.Combine(path, dto.Photo.FileName);
                        dto.Photo.SaveAs(path);
                        cif.Photo = path;
                       
                    }
                }
                if (dto.SignaturePhoto != null)
                {
                    filePath = "~/Content/image/signature/";//"~/Uploaded/ProfilePicture/";
                    path = HttpContext.Current.Server.MapPath(filePath);
                    if (dto.SignaturePhoto != null)
                    {

                        if (!Directory.Exists(path))
                        {
                            Directory.CreateDirectory(path);
                        }
                        string customfileName = "Signature_" + cif.CIFNo;
                        string fileExt = Path.GetExtension(dto.SignaturePhoto.FileName);
                        path = Path.Combine(path, customfileName + fileExt);
                        for (int i = 1; File.Exists(path);)
                        {
                            var length = path.Length;
                            if (!string.IsNullOrEmpty(fileExt))
                                path = path.Substring(0, (length - 3 - fileExt.Length)) + (++i).ToString("000") + fileExt;
                            else
                                path = path.Substring(0, (length - 3)) + (++i).ToString("000");
                        }
                        dto.SignaturePhoto.SaveAs(path);
                        cif.SignaturePhoto = path;

                    }
                }
                GenService.Save(cif);
                response.Message = "Photo Uploaded";
            }
            catch (Exception ex)
            {

            }
            return response;
        }
    }
}
