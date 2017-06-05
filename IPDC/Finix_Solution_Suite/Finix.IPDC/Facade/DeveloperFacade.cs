using AutoMapper;
using Finix.IPDC.DTO;
using Finix.IPDC.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using Finix.IPDC.Infrastructure;
using PagedList;

namespace Finix.IPDC.Facade
{
    public class DeveloperFacade : BaseFacade
    {
        //public List<DeveloperDto> GetAllDevelopers()
        //{
        //    var developers = GenService.GetAll<Developer>().ToList();
        //    return Mapper.Map<List<DeveloperDto>>(developers);
        //}

        public List<DeveloperDto> GetAllDevelopers()
        {
            List<DeveloperDto> result = new List<DeveloperDto>();
            try
            {
                var settings = GenService.GetAll<Developer>();
                result = (from setting in settings
                          select new DeveloperDto
                          {
                              Id = setting.Id,
                              GroupName = setting.GroupName,
                              ContactPerson = setting.ContactPerson,
                              ContactPersonPhone = setting.ContactPersonPhone

                          }).ToList();
                //return result;
            }
            catch (Exception ex)
            {
                //
            }
            return result;
        }

        public List<ProjectDto> GetProjectsByDevelopers(long id)
        {
            var projects = GenService.GetAll<Project>().Where(i => i.DeveloperId == id).ToList();
            return Mapper.Map<List<ProjectDto>>(projects);
        }
        public ResponseDto SaveDeveloper(DeveloperDto dto, long userId)
        {
            var response = new ResponseDto();
            Developer entity;

            //create or update developer
            if (dto.Id != null && dto.Id > 0)
            {
                entity = GenService.GetById<Developer>((long)dto.Id);
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

                        if (dto.Members != null)
                        {
                            foreach (var item in dto.Members)
                            {
                                if (item.Id != null && item.Id > 0)
                                {
                                    var obj = entity.Members.FirstOrDefault(o => o.Id == item.Id);
                                    GenService.GetById<DeveloperMember>((long)item.Id);

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
                                        item.DeveloperId = entity.Id;
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
                                    item.DeveloperId = entity.Id;
                                    var members = new DeveloperMember();
                                    Mapper.Map(item, members);
                                    GenService.Save(members);
                                }
                            }
                        }

                        if (dto.RemovedMembers != null)
                        {
                            foreach (var item in dto.RemovedMembers)
                            {
                                var devMembers = GenService.GetById<DeveloperMember>(item);//entity.FactoryAddress.Where(o => o.Id == item).FirstOrDefault();//
                                if (devMembers != null)
                                {
                                    devMembers.Status = EntityStatus.Inactive;
                                    devMembers.EditDate = DateTime.Now;
                                    devMembers.EditedBy = userId;
                                }
                                GenService.Save(devMembers);
                            }
                        }

                        if (dto.Directors != null)
                        {
                            foreach (var item in dto.Directors)
                            {
                                DeveloperDirector director;
                                if (item.Id != null && item.Id > 0)
                                {
                                    director = GenService.GetById<DeveloperDirector>((long)item.Id);
                                    if (item.DeveloperId != null)
                                    {
                                        director.DeveloperId = (long)item.DeveloperId;
                                        item.CreateDate = director.CreateDate;
                                        item.CreatedBy = director.CreatedBy;
                                        Mapper.Map(item, director);
                                        director.EditDate = DateTime.Now;
                                        director.Status = EntityStatus.Active;
                                        director.EditedBy = userId;
                                        GenService.Save(director);
                                    }
                                }
                                else
                                {
                                    director = Mapper.Map<DeveloperDirector>(item);
                                    director.Status = EntityStatus.Active;
                                    director.CreatedBy = userId;
                                    director.CreateDate = DateTime.Now;
                                    director.DeveloperId = entity.Id;
                                    GenService.Save(director);
                                }
                            }
                        }

                        if (dto.RemovedDirectors != null)
                        {
                            foreach (var item in dto.RemovedDirectors)
                            {
                                var director = GenService.GetById<DeveloperDirector>(item);//entity.Owners.Where(o => o.Id == item).FirstOrDefault();//
                                if (director != null)
                                {
                                    director.Status = EntityStatus.Inactive;
                                    director.EditDate = DateTime.Now;
                                    director.EditedBy = userId;
                                }
                                GenService.Save(director);
                            }
                        }

                        if (dto.OtherDocuments != null)
                        {
                            foreach (var item in dto.OtherDocuments)
                            {
                                DeveloperDocument otherDoc;
                                if (item.Id != null && item.Id > 0)
                                {
                                    otherDoc = GenService.GetById<DeveloperDocument>((long)item.Id);
                                    if (item.DeveloperId != null)
                                    {
                                        otherDoc.DeveloperId = (long)item.DeveloperId;
                                        otherDoc.EditDate = DateTime.Now;
                                        otherDoc.EditedBy = userId;
                                        GenService.Save(otherDoc);
                                    }
                                }
                                else
                                {
                                    otherDoc = Mapper.Map<DeveloperDocument>(item);
                                    otherDoc.Status = EntityStatus.Active;
                                    otherDoc.CreatedBy = userId;
                                    otherDoc.CreateDate = DateTime.Now;
                                    otherDoc.DeveloperId = entity.Id;
                                    GenService.Save(otherDoc);
                                }
                            }
                        }

                        if (dto.RemovedDirectors != null)
                        {
                            foreach (var item in dto.RemovedDirectors)
                            {
                                var otherDoc = GenService.GetById<DeveloperDocument>(item);//entity.Owners.Where(o => o.Id == item).FirstOrDefault();//
                                if (otherDoc != null)
                                {
                                    otherDoc.Status = EntityStatus.Inactive;
                                    otherDoc.EditDate = DateTime.Now;
                                    otherDoc.EditedBy = userId;
                                }
                                GenService.Save(otherDoc);
                            }
                        }
                        tran.Complete();
                    }
                    catch (Exception)
                    {

                        tran.Dispose();
                        response.Message = "Developer Edit Failed";
                        return response;
                    }
                }

                response.Success = true;
                response.Message = "Developer Edited Successfully";

            }
            else
            {
                entity = Mapper.Map<Developer>(dto);
                entity.CreateDate = DateTime.Now;
                entity.CreatedBy = userId;
                if (dto.EnlistmentStatus == 0)
                {
                    entity.EnlistmentStatus = DeveloperEnlistmentStatus.New;
                }
                using (var tran = new TransactionScope())
                {
                    try
                    {
                        
                        if (dto.Members != null)
                        {
                            entity.Members = new List<DeveloperMember>();
                            foreach (var item in dto.Members)
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
                                entity.Members.Add(Mapper.Map<DeveloperMember>(item));
                            }
                        }

                        if (dto.Directors != null)
                        {
                            entity.Directors = Mapper.Map<List<DeveloperDirector>>(dto.Directors);
                            foreach (var item in entity.Directors)
                            {
                                item.CreateDate = DateTime.Now;
                                item.CreatedBy = userId;
                                item.Status = EntityStatus.Active;
                            }
                        }

                        if (dto.OtherDocuments != null)
                        {
                            entity.OtherDocuments = Mapper.Map<List<DeveloperDocument>>(dto.OtherDocuments);
                            foreach (var item in entity.OtherDocuments)
                            {
                                item.CreateDate = DateTime.Now;
                                item.CreatedBy = userId;
                                item.Status = EntityStatus.Active;
                            }
                        }

                        GenService.Save(entity);
                        tran.Complete();
                    }
                    catch (Exception ex)
                    {
                        tran.Dispose();
                        response.Message = "Devloper Saved Failed";
                    }
                }
                response.Success = true;
                response.Message = "Developer Saved Successfully";
            }

            return response;
        }
        public DeveloperDto GetDeveloperById(long devId)
        {
            return Mapper.Map<DeveloperDto>(GenService.GetById<Developer>(devId));
        }

        public IPagedList<DeveloperDto> DeveloperList(int pageSize, int pageCount, string searchString, long UserId)
        {
            var allApp = GenService.GetAll<Developer>().Where(s => s.Status == EntityStatus.Active).Select(s => new DeveloperDto
            {
                Id = s.Id,
                GroupName = s.GroupName,
                ContactPerson = s.ContactPerson,
                EnlistmentStatusName =  s.EnlistmentStatus.ToString() ,
                Website = s.Website
            });
            if (!string.IsNullOrEmpty(searchString))
                allApp = allApp.Where(a => a.GroupName.ToLower().Contains(searchString.ToLower()) || a.ContactPerson.ToLower().Contains(searchString.ToLower()) || a.Website.ToLower().Contains(searchString.ToLower()));
            var temp = allApp.OrderBy(r => r.Id).ToPagedList(pageCount, pageSize);
            return temp;
        }
        public IPagedList<ProjectDto> ProjectList(int pageSize, int pageCount, string searchString, long UserId)
        {
            var allApp = GenService.GetAll<Project>().Where(s => s.Status == EntityStatus.Active).Select(s => new ProjectDto
            {
                Id = s.Id,
                DeveloperId = s.DeveloperId,
                DeveloperName = s.Developer.GroupName,
                ProjectName = s.ProjectName,
                Area = s.Area
            });
            if (!string.IsNullOrEmpty(searchString))
                allApp = allApp.Where(a => a.DeveloperName.ToLower().Contains(searchString.ToLower()) || a.DeveloperName.ToLower().Contains(searchString.ToLower()) || a.ProjectName.ToLower().Contains(searchString.ToLower()));
            var temp = allApp.OrderBy(r => r.Id).ToPagedList(pageCount, pageSize);
            return temp;
        }
        public ProjectDto LoadProjectById(long devId)
        {
            return Mapper.Map<ProjectDto>(GenService.GetById<Project>(devId));
        }
        public ResponseDto SaveProject(ProjectDto dto, long uerId)
        {
            var entity = new Project();
            var address = new Address();
            ResponseDto responce = new ResponseDto();
            if (dto.Id != null && dto.Id > 0)
            {
                entity = GenService.GetById<Project>((long)dto.Id);
                dto.HandoverDate = entity.HandoverDate;
                dto.AsOfDate = entity.AsOfDate;
                dto.CreateDate = entity.CreateDate;
                dto.CreatedBy = entity.CreatedBy;
                using (var tran = new TransactionScope())
                {
                    try
                    {
                        if (dto.ProjectAddress.IsChanged)
                        {
                            if (dto.ProjectAddress.Id != null)
                            {
                                address = GenService.GetById<Address>((long)dto.ProjectAddress.Id);
                                dto.ProjectAddress.CreateDate = address.CreateDate;
                                dto.ProjectAddress.CreatedBy = address.CreatedBy;
                                address = Mapper.Map(dto.ProjectAddress, address);
                                GenService.Save(address);
                                dto.ProjectAddressId = address.Id;
                            }
                            else
                            {
                                var customerAddress = Mapper.Map<Address>(dto.ProjectAddress);
                                GenService.Save(customerAddress);
                                dto.ProjectAddressId = customerAddress.Id;
                            }

                        }
                        else
                        {
                            dto.ProjectAddressId = entity.ProjectAddressId;
                        }

                        entity = Mapper.Map(dto, entity);
                        entity.EditDate = DateTime.Now;
                        entity.EditedBy = uerId;
                        GenService.Save(entity);
                        tran.Complete();
                        responce.Success = true;
                        responce.Message = "Project Information Edited Successfully";
                    }
                    catch (Exception ex)
                    {
                        tran.Dispose();
                        responce.Message = "Project Information Edit Operation Failed";
                    }
                }
                
            }
            else
            {
                entity = Mapper.Map<Project>(dto);
                entity.HandoverDate = dto.HandoverDate;
                entity.AsOfDate = dto.AsOfDate;
                //entity.CIFNo = _sequencer.GetUpdatedCIFPersonalNo();
                entity.Status = EntityStatus.Active;
                entity.CreatedBy = uerId;
                entity.CreateDate = DateTime.Now;
                var projectAddress = Mapper.Map<Address>(dto.ProjectAddress);

                using (var tran = new TransactionScope())
                {
                    try
                    {
                        if (projectAddress.CountryId != null)
                        {   
                            GenService.Save(projectAddress);
                            entity.ProjectAddressId = projectAddress.Id;
                        }

                        GenService.Save(entity);
                        GenService.Save(entity);
                        tran.Complete();
                        responce.Success = true;
                        responce.Message = "Project Information Saved Successfuly.";
                    }
                    catch (Exception ex)
                    {
                        tran.Dispose();
                        responce.Message = "Project Information Save Failed";
                    }
                }
            }
            return responce;
        }
        public ProjectDto GetProjectById(long projId)
        {
            return Mapper.Map<ProjectDto>(GenService.GetById<Project>(projId));
        }
    }
}
