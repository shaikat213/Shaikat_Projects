using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using AutoMapper;
using Finix.IPDC.DTO;
using Finix.IPDC.Infrastructure;
using Finix.IPDC.Infrastructure.Models;
using PagedList;

namespace Finix.IPDC.Facade
{
    public class VendorFacade : BaseFacade
    {
        public ResponseDto SaveVendor(VendorDto dto, long userId)
        {
            var response = new ResponseDto();
            Vendor entity;

            //create or update vendor
            if (dto.Id != null && dto.Id > 0)
            {
                entity = GenService.GetById<Vendor>((long)dto.Id);
                dto.CreateDate = entity.CreateDate;
                dto.CreatedBy = entity.CreatedBy;

                using (var tran = new TransactionScope())
                {
                    try
                    {
                        Mapper.Map(dto, entity);
                        dto.EditDate = DateTime.Now;
                        dto.EditedBy = userId;
                        GenService.Save(entity);

                        if (dto.Showrooms != null)
                        {
                            foreach (var item in dto.Showrooms)
                            {
                                if (item.Id != null && item.Id > 0)
                                {
                                    var obj = entity.Showrooms.FirstOrDefault(o => o.Id == item.Id);
                                    GenService.GetById<VendorShowrooms>((long)item.Id);

                                    if (obj != null)
                                    {
                                        item.CreateDate = obj.CreateDate;
                                        item.CreatedBy = obj.CreatedBy;
                                        item.EditDate = DateTime.Now;
                                        item.EditedBy = userId;
                                        if (item.Address.IsChanged)
                                        {
                                            Address address;
                                            if (item.AddressId != null)
                                            {
                                                address = GenService.GetById<Address>((long)item.AddressId);
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
                                                address.CreatedBy = userId;
                                                address.Status = EntityStatus.Active;
                                            }
                                            item.AddressId = address.Id;
                                            GenService.Save(address);
                                        }
                                        item.VendorId = entity.Id;
                                        Mapper.Map(item, obj);
                                        GenService.Save(obj);
                                    }
                                }
                                else
                                {
                                    var additionalAddress = new Address();
                                    Mapper.Map(item.Address, additionalAddress);
                                    additionalAddress.CreatedBy = userId;
                                    additionalAddress.CreateDate = DateTime.Now;
                                    additionalAddress.Status = EntityStatus.Active;
                                    GenService.Save(additionalAddress);

                                    item.AddressId = additionalAddress.Id;
                                    item.CreatedBy = userId;
                                    item.CreateDate = DateTime.Now;
                                    item.VendorId = entity.Id;
                                    var showrooms = new VendorShowrooms();
                                    Mapper.Map(item, showrooms);
                                    GenService.Save(showrooms);
                                }
                            }
                        }

                        if (dto.RemovedShowrooms != null)
                        {
                            foreach (var item in dto.RemovedShowrooms)
                            {
                                var venshowrooms = GenService.GetById<VendorShowrooms>(item);//entity.FactoryAddress.Where(o => o.Id == item).FirstOrDefault();//
                                if (venshowrooms != null)
                                {
                                    venshowrooms.Status = EntityStatus.Inactive;
                                    venshowrooms.EditDate = DateTime.Now;
                                    venshowrooms.EditedBy = userId;
                                }
                                GenService.Save(venshowrooms);
                            }
                        }

                        
                        tran.Complete();
                    }
                    catch (Exception)
                    {

                        tran.Dispose();
                        response.Message = "Vendor Edit Failed";
                        return response;
                    }
                }

                response.Success = true;
                response.Message = "Vendor Edited Successfully";

            }
            else
            {
                entity = Mapper.Map<Vendor>(dto);
                entity.CreateDate = DateTime.Now;
                entity.CreatedBy = userId;
                entity.Status = EntityStatus.Active;
                
                using (var tran = new TransactionScope())
                {
                    try
                    {

                        if (dto.Showrooms != null)
                        {
                            entity.Showrooms = new List<VendorShowrooms>();
                            foreach (var item in dto.Showrooms)
                            {
                                var memAddress = Mapper.Map<Address>(item.Address);

                                memAddress.CreateDate = DateTime.Now;
                                memAddress.CreatedBy = userId;
                                memAddress.Status = EntityStatus.Active;

                                if (memAddress.CountryId != null)
                                {
                                    GenService.Save(memAddress);
                                    item.AddressId = memAddress.Id;
                                }
                                item.CreateDate = DateTime.Now;
                                item.CreatedBy = userId;
                                item.Status = EntityStatus.Active;
                                entity.Showrooms.Add(Mapper.Map<VendorShowrooms>(item));
                            }
                        }
                        GenService.Save(entity);
                        tran.Complete();
                        response.Message = "Vendor Saved Successfully";
                    }
                    catch (Exception ex)
                    {
                        tran.Dispose();
                        response.Message = "Vendor Saved Failed";
                    }
                }
            }

            return response;
        }

        public IPagedList<VendorDto> VendorList(int pageSize, int pageCount, string searchString, long UserId)
        {
            var allApp = GenService.GetAll<Vendor>().Where(s => s.Status == EntityStatus.Active).Select(s => new VendorDto
                                                               {
                                                                   Id = s.Id,
                                                                   Name = s.Name,
                                                                   VendorProductType = s.VendorProductType,
                                                                   VendorProductTypeName = s.VendorProductType.ToString(),
                                                                   ContactPerson = s.ContactPerson,
                                                                   Website = s.Website
                                                               });
            if (!string.IsNullOrEmpty(searchString))
                allApp = allApp.Where(a => a.Name.ToLower().Contains(searchString.ToLower()) || a.ContactPerson.ToLower().Contains(searchString.ToLower()) || a.Website.ToLower().Contains(searchString.ToLower()));
            var temp = allApp.OrderBy(r => r.Id).ToPagedList(pageCount, pageSize);
            return temp;
        }
        public VendorDto GetVendorById(long venId)
        {
            var vendorData = GenService.GetById<Vendor>(venId);//GenService.GetAll<Vendor>().FirstOrDefault(r => r.Id == venId);
            var result = Mapper.Map<VendorDto>(vendorData);
            result.Showrooms.RemoveAll(f => f.Status != EntityStatus.Active);
            //result.Owners.RemoveAll(o => o.Status != EntityStatus.Active);

            return result;
        }
        public List<VendorDto> GetAllVendors()
        {
            var vendors = GenService.GetAll<Vendor>().ToList();
            return Mapper.Map<List<VendorDto>>(vendors);
        }
        public List<VendorShowroomsDto> GetAllVendorShowrooms(long vendorId)
        {
            var vendorsShowroom = GenService.GetAll<VendorShowrooms>().Where(r=>r.VendorId == vendorId).ToList();
            return Mapper.Map<List<VendorShowroomsDto>>(vendorsShowroom);
        }

        public List<VendorShowroomsDto> GetOnlyShowRooms()
        {
            var vendorsShowroom = GenService.GetAll<VendorShowrooms>().ToList();
            return Mapper.Map<List<VendorShowroomsDto>>(vendorsShowroom);
        }
    }
}
