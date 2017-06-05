using Finix.IPDC.DTO;
using Finix.IPDC.Infrastructure;
using Finix.IPDC.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finix.IPDC.Facade
{
    public class OfficeFacade : BaseFacade
    {
        //private readonly GenService _service = new GenService();
        public List<OfficeLayerDto> GetOfficeLayers()
        {
            var officelayers = GenService.GetAll<OfficeLayer>();
            var data = officelayers.Select(x => new OfficeLayerDto
            {
                Id = x.Id,
                Name = x.Name,
                Level = x.Level

            }).ToList();
            return data;
        }
        public void SaveOfficeLayer(OfficeLayerDto officelayerdto, long UserId)
        {
            if (officelayerdto.Id > 0)
            {
                var officelayer = GenService.GetById<OfficeLayer>(officelayerdto.Id);
                officelayer.Name = officelayerdto.Name;
                officelayer.Level = officelayerdto.Level;
                officelayer.EditDate = DateTime.Now;
                officelayer.EditedBy = UserId;
                GenService.Save(officelayer);

            }
            else
            {
                GenService.Save(new OfficeLayer
                {
                    Name = officelayerdto.Name,
                    Level = officelayerdto.Level,
                    CreateDate = DateTime.Now,
                    CreatedBy = UserId                    
                });
            }
            GenService.SaveChanges();
        }


        public void DeleteOfficeLayer(long id)
        {
            GenService.Delete<OfficeLayer>(id);
            GenService.SaveChanges();
        }


        public List<OfficeDto> GetOffice(long? id = null)
        {
            var offices = GenService.GetAll<Office>();
            if (id.HasValue)
                offices = offices.Where(x => x.Id == id);

            var data = offices.Select(x => new OfficeDto
            {
                Id = x.Id,
                Name = x.Name,
                OfficeLayerId = x.OfficeLayerId,
                OfficeLayerName = x.OfficeLayer.Name,
                ParentId = x.ParentId,
                ParentName = x.ParentOffice.Name
            }).ToList();
            return ApplyJqFilter(data).ToList();
        }

        public void SaveOffice(OfficeDto officedto)
        {
            if (officedto.Id > 0)
            {
                var office = GenService.GetById<Office>(officedto.Id);
                office.Name = officedto.Name;
                office.ParentId = officedto.ParentId == 0 ? null : officedto.ParentId;
                office.OfficeLayerId = officedto.OfficeLayerId;
            }
            else
            {
                GenService.Save(new Office
                {
                    Name = officedto.Name,
                    OfficeLayerId = officedto.OfficeLayerId,
                    ParentId = officedto.ParentId == 0 ? null : officedto.ParentId
                });
            }
            GenService.SaveChanges();
        }

        public void DeleteOffice(int id)
        {
            GenService.Delete<Office>(id);
            GenService.SaveChanges();
        }

        public List<OfficeDto> GetCorrespondingParentOffices(long officelayerid)
        {
            List<OfficeDto> officelist;
            int officelevel = GenService.GetById<OfficeLayer>(officelayerid).Level;
            if (officelevel == 0)
            {
                officelist = new List<OfficeDto>
                {
                   new OfficeDto{Id=0,Name="No parent"}

                };
                return officelist;
            }
            var offices = GenService.GetAll<Office>().Where(x => x.OfficeLayer.Level == officelevel - 1);
            officelist = offices.Select(x => new OfficeDto
            {
                Id = x.Id,
                Name = x.Name,
                OfficeLayerId = x.OfficeLayerId,
                OfficeLayerName = x.OfficeLayer.Name,
                ParentId = x.ParentId,
                ParentName = x.ParentOffice.Name
            }).ToList();

            return officelist;
        }

        //public List<OfficeUnitDto> GetOfficeUnit(long? id = null)
        //{
        //    var officeunits = GenService.GetAll<OfficeUnit>();
        //    if (id.HasValue)
        //        officeunits = GenService.GetAll<OfficeUnit>().Where(x => x.Id == id);
        //    var data = officeunits.Select(x => new OfficeUnitDto
        //    {
        //        Id = x.Id,
        //        Name = x.Name,
        //        UnitType = (int)x.OfficeUnitType,
        //        UnitTypeName = x.OfficeUnitType.ToString(),
        //        ParentId = x.ParentId,
        //        ParentName = x.ParentUnit.Name
        //    }).ToList();
        //    return data;
        //}

        //public void SaveOfficeUnit(OfficeUnitDto officeunitdto)
        //{
        //    if (officeunitdto.Id > 0)
        //    {
        //        var officeunit = GenService.GetById<OfficeUnit>(officeunitdto.Id);
        //        officeunit.Name = officeunitdto.Name;
        //        officeunit.OfficeUnitType = (OfficeUnitType)officeunitdto.UnitType;
        //        officeunit.ParentId = officeunitdto.ParentId == 0 ? null : officeunitdto.ParentId;
        //    }
        //    else
        //    {
        //        GenService.Save(new OfficeUnit
        //        {
        //            Name = officeunitdto.Name,
        //            ParentId = officeunitdto.ParentId == 0 ? null : officeunitdto.ParentId,
        //            OfficeUnitType = (OfficeUnitType)officeunitdto.UnitType
        //        });
        //    }
        //    GenService.SaveChanges();
        //}

        //public void DeleteOfficeUnit(int id)
        //{
        //    GenService.Delete<OfficeUnit>(id);
        //    GenService.SaveChanges();
        //}

        //public List<OfficeUnitDto> GetOfficeUnitsByUnitType(int unittype)
        //{
        //    var officeunits = GenService.GetAll<OfficeUnit>().Where(x => (int)x.OfficeUnitType == unittype);
        //    var data = officeunits.Select(x => new OfficeUnitDto
        //    {
        //        Id = x.Id,
        //        Name = x.Name,
        //        ParentId = x.ParentId

        //    }).ToList();
        //    return data;
        //}
        
        //public List<OfficeUnitSettingDto> GetOfficeUnitSettingsByOfficeId(int officeid)
        //{
        //    var officeunitsettings = GenService.GetAll<OfficeUnitSetting>().Where(x => x.OfficeId == officeid).OrderBy(x => x.Sl);
        //    var data = officeunitsettings.Select(x => new OfficeUnitSettingDto
        //    {
        //        Id = x.Id,
        //        OfficeId = x.OfficeId,
        //        OfficeName = x.Office.Name,
        //        UnitType = (int)x.OfficeUnit.OfficeUnitType,
        //        OfficeUnitId = x.OfficeUnitId,
        //        OfficeUnitName = x.OfficeUnit.Name,
        //        ParentUnitId = x.ParentUnitId,
        //        ParentUnitName = x.ParentUnit.Name,
        //        Sl = x.Sl
        //    }).ToList();

        //    return data;
        //}
        //public void SaveOfficeUnitSetting(OfficeUnitSettingDto officeunitsettingdto)
        //{
        //    if (officeunitsettingdto.Id > 0)
        //    {
        //        var officeunitsetting = GenService.GetById<OfficeUnitSetting>(officeunitsettingdto.Id);
        //        officeunitsetting.OfficeId = officeunitsettingdto.OfficeId;
        //        officeunitsetting.OfficeUnitId = officeunitsettingdto.OfficeUnitId;
        //        officeunitsetting.ParentUnitId = officeunitsettingdto.ParentUnitId == 0 ? null : officeunitsettingdto.ParentUnitId;
        //        officeunitsetting.Sl = officeunitsettingdto.Sl;
        //    }
        //    else
        //    {
        //        GenService.Save(new OfficeUnitSetting
        //        {
        //            OfficeId = officeunitsettingdto.OfficeId,
        //            OfficeUnitId = officeunitsettingdto.OfficeUnitId,
        //            ParentUnitId = officeunitsettingdto.ParentUnitId == 0 ? null : officeunitsettingdto.ParentUnitId,
        //            Sl = officeunitsettingdto.Sl
        //        });
        //    }
        //    GenService.SaveChanges();
        //}

        //public void DeleteOfficeUnitSetting(int id)
        //{
        //    GenService.Delete<OfficeUnitSetting>(id);
        //    GenService.SaveChanges();
        //}
        //public List<OfficeUnitDto> GetCorrespondingParentUnit(int unittype)
        //{
        //    List<OfficeUnitDto> officeunitlist;
        //    var officeunits = GenService.GetAll<OfficeUnit>().Where(x => (int)(x.OfficeUnitType) == unittype - 1);
        //    officeunitlist = officeunits.Select(x => new OfficeUnitDto
        //    {
        //        Id = x.Id,
        //        Name = x.Name,
        //        ParentId = x.ParentId,
        //        ParentName = x.ParentUnit.Name,
        //        UnitType = (int)x.OfficeUnitType,
        //        UnitTypeName = x.OfficeUnitType.ToString()
        //    }).ToList();

        //    return officeunitlist;
        //}

        public List<OfficeDto> GetOfficesByLayer(long officeLayer)
        {
            var offices = GenService.GetAll<Office>().Where(x => x.OfficeLayer.Level == officeLayer);
            var officelist = offices.Select(x => new OfficeDto
            {
                Id = x.Id,
                Name = x.Name,
                OfficeLayerId = x.OfficeLayerId,
                OfficeLayerName = x.OfficeLayer.Name,
                ParentId = x.ParentId,
                ParentName = x.ParentOffice.Name
            }).ToList();

            return officelist;
        }

        //public List<OfficePositionDto> GetOfficePositions()
        //{
        //    var officepositions = GenService.GetAll<OfficePosition>();
        //    var data = officepositions.Select(x => new OfficePositionDto
        //    {
        //        Id = x.Id,
        //        Name = x.Name,
        //        OfficeLayerId = x.Office != null ? x.Office.OfficeLayerId : 0,
        //        OfficeId = x.OfficeId,
        //        OfficeName = x.Office.Name,
        //        //DefaultDesignationId = x.DefaultDesignationId,
        //        //DefaultDesignationName = x.DefaultDesignation.Name,
        //        PositionWeight = x.PositionWeight
        //    }).ToList();
        //    return data;
        //}

        //public void SaveOfficePosition(OfficePositionDto officepositiondto)
        //{
        //    if (officepositiondto.Id > 0)
        //    {
        //        OfficePosition officeposition = GenService.GetById<OfficePosition>(officepositiondto.Id);
        //        officeposition.Name = officepositiondto.Name;
        //        officeposition.OfficeId = officepositiondto.OfficeId;
        //        //officeposition.DefaultDesignationId = officepositiondto.DefaultDesignationId;
        //        officeposition.PositionWeight = officepositiondto.PositionWeight;
        //    }
        //    else
        //    {
        //        GenService.Save(new OfficePosition
        //        {
        //            Name = officepositiondto.Name,
        //            OfficeId = officepositiondto.OfficeId,
        //            //DefaultDesignationId = officepositiondto.DefaultDesignationId,
        //            PositionWeight = officepositiondto.PositionWeight
        //        });
        //    }
        //    GenService.SaveChanges();
        //}

        //public void DeleteOfficePosition(int id)
        //{
        //    GenService.Delete<OfficePosition>(id);
        //    GenService.SaveChanges();
        //}

        public List<OfficeDto> GetOfficesByLayerId(long officelayerid)
        {
            var offices = GenService.GetAll<Office>().Where(x => x.OfficeLayerId == officelayerid);
            var data = offices.Select(x => new OfficeDto
            {
                Id = x.Id,
                Name = x.Name,
                OfficeLayerId = x.OfficeLayerId,
                ParentId = x.ParentId
            }).ToList();

            return data;
        }

        //public List<OrganogramDto> GetOrganograms()
        //{
        //    var organograms = GenService.GetAll<OrganoGram>();
        //    var data = organograms.Select(x => new OrganogramDto
        //    {
        //        Id = x.Id,
        //        EmployeeId = x.EmployeeId,
        //        EmployeeName = x.Employee.Name,
        //        //DesignationId = x.DesignationId,
        //        //DesignationName = x.Designation.Name,
        //        OfficeLayerId = x.Office.OfficeLayerId,
        //        OfficeId = x.OfficeId,
        //        OfficeName = x.Office.Name,
        //        UnitType = (int)x.OfficeUnit.OfficeUnitType,
        //        OfficeUnitId = x.OfficeUnitId,
        //        OfficeUnitName = x.OfficeUnit.Name,
        //        PositionId = x.PositionId,
        //        PositionName = x.OfficePosition.Name,
        //        ParentId = x.ParentId,
        //        ParentName = x.ParentId == null ? null : x.Parent.Employee.Name
        //    }).ToList();
        //    return data;
        //}
        
        //public void SaveOrganogram(OrganogramDto organogramdto)
        //{
        //    if (organogramdto.Id > 0)
        //    {
        //        OrganoGram organogram = GenService.GetById<OrganoGram>(organogramdto.Id);
        //        //organogram.Name = organogramdto.Name;
        //        organogram.EmployeeId = organogramdto.EmployeeId;
        //        //organogram.DesignationId = organogramdto.DesignationId;
        //        organogram.OfficeId = organogramdto.OfficeId;
        //        organogram.OfficeUnitId = organogramdto.OfficeUnitId;
        //        organogram.PositionId = organogramdto.PositionId;
        //        organogram.ParentId = organogramdto.ParentId == 0 ? null : organogramdto.ParentId;
        //    }
        //    else
        //    {
        //        GenService.Save(new OrganoGram
        //        {
        //            //Name=organogramdto.Name,
        //            EmployeeId = organogramdto.EmployeeId,
        //            //DesignationId = organogramdto.DesignationId,
        //            OfficeId = organogramdto.OfficeId,
        //            OfficeUnitId = organogramdto.OfficeUnitId,
        //            PositionId = organogramdto.PositionId,
        //            ParentId = organogramdto.ParentId == 0 ? null : organogramdto.ParentId
        //        });
        //    }
        //    GenService.SaveChanges();
        //}

        //public void DeleteOrganogram(int id)
        //{
        //    GenService.Delete<OrganoGram>(id);
        //    GenService.SaveChanges();
        //}

        //public List<OfficeUnitDto> GetCorrespondingOfficeUnit(int unittype)
        //{
        //    var offficeunitlist = GenService.GetAll<OfficeUnit>().Where(x => x.OfficeUnitType == (OfficeUnitType)unittype);
        //    var data = offficeunitlist.Select(x => new OfficeUnitDto
        //    {
        //        Id = x.Id,
        //        Name = x.Name,
        //        UnitType = (int)x.OfficeUnitType,
        //        ParentId = x.ParentId
        //    }).ToList();
        //    return data;
        //}

        //public List<OrganogramDto> GetParentOrganogramsByPosition(int positionid, int officeid)
        //{
        //    long posweight = GenService.GetAll<OfficePosition>().FirstOrDefault(p => p.Id == positionid).PositionWeight;
        //    var organogramlist = GenService.GetAll<OrganoGram>().Where(x => x.OfficeId == officeid && x.OfficePosition.PositionWeight <= posweight);
        //    var data = organogramlist.Select(x => new OrganogramDto
        //    {
        //        Id = x.Id,
        //        EmployeeName = x.Employee.Name,
        //        ParentId = x.ParentId

        //    }).ToList();
        //    if (GenService.GetAll<OfficePosition>().FirstOrDefault(x => x.Id == positionid).PositionWeight == 1)
        //    {
        //        OrganogramDto dto = new OrganogramDto { Id = 0, EmployeeName = "No Parent" };
        //        data.Add(dto);
        //    }
        //    return data;
        //}

        //public Object GetEmployeesByOffice(int officeid)
        //{
        //    var employees = GenService.GetAll<Employee>().Where(x => x.OfficeId == officeid).ToList();

        //    var officepositions = GenService.GetAll<OfficePosition>().Where(x => x.OfficeId == officeid).ToList();
        //    return new { Employees = employees, OfficePositions = officepositions };

        //}


        //public List<OfficeUnitDto> GetOfficeLayerById(int officelayerid)
        //{
        //    List<OfficeUnitDto> officelist;

        //    if (officelayerid == 0)
        //    {
        //        officelist = new List<OfficeUnitDto>
        //        {
        //           new OfficeUnitDto{Id=0,Name="No parent"}

        //        };
        //        return officelist;
        //    }
        //    var offices = GenService.GetAll<OfficeUnit>().Where(x => x.OfficeUnitType == (OfficeUnitType)officelayerid);
        //    officelist = offices.Select(x => new OfficeUnitDto
        //    {
        //        Id = x.Id,
        //        Name = x.Name,
        //        UnitType = (int)x.OfficeUnitType,

        //        ParentId = x.ParentId
        //    }).ToList();

        //    return officelist;

        //}

        public List<OfficeDto> GetAllOffices(int officeLayerId)
        {

            List<OfficeDto> officelist;

            if (officeLayerId == 0)
            {
                officelist = new List<OfficeDto>
                {
                   new OfficeDto{Id=0,Name="No parent"}

                };
                return officelist;
            }
            var offices = GenService.GetAll<Office>().Where(x => x.OfficeLayerId == officeLayerId);
            officelist = offices.Select(x => new OfficeDto
            {
                Id = x.Id,
                Name = x.Name,
                OfficeLayerId = x.OfficeLayerId,
                ParentId = x.ParentId
            }).ToList();

            return officelist;
        }

        public List<OfficeLayerDto> GetAllOfficeLayersJsonResult()
        {
            var offficeunitlist = GenService.GetAll<OfficeLayer>();
            var data = offficeunitlist.Select(x => new OfficeLayerDto
            {
                Id = x.Id,
                Name = x.Name
            }).ToList();
            return data;
        }

        //public List<OfficeUnitDto> GetOfficetUnits(int unittype)
        //{
        //    List<OfficeUnitDto> officeunitlist;
        //    var officeunits = GenService.GetAll<OfficeUnit>().Where(x => (int)(x.OfficeUnitType) == unittype);
        //    officeunitlist = officeunits.Select(x => new OfficeUnitDto
        //    {
        //        Id = x.Id,
        //        Name = x.Name,
        //        ParentId = x.ParentId,
        //        ParentName = x.ParentUnit.Name,
        //        UnitType = (int)x.OfficeUnitType,
        //        UnitTypeName = x.OfficeUnitType.ToString()
        //    }).ToList();

        //    return officeunitlist;
        //}

        public List<OfficeDto> GetAllActiveOffices()
        {
            List<OfficeDto> officelist;
            var offices = GenService.GetAll<Office>();
            officelist = offices.Select(x => new OfficeDto
            {
                Id = x.Id,
                Name = x.Name,
                OfficeLayerId = x.OfficeLayerId,
                ParentId = x.ParentId
            }).ToList();

            return officelist;
        }

        //public List<OfficeUnitDto> GetOfficeUnitByOffice(int officeid)
        //{
        //    var offunitsettings = GenService.GetAll<OfficeUnitSetting>().Where(x => x.OfficeId == officeid);
        //    var unitIdList = offunitsettings.Select(x => x.OfficeUnitId);
        //    var offunits = GenService.GetAll<OfficeUnit>().Where(x => unitIdList.Contains(x.Id));
        //    var data = offunits.Select(x => new OfficeUnitDto
        //    {
        //        Id = x.Id,
        //        Name = x.Name,
        //        Checked = false

        //    }).ToList();
        //    return data;
        //}
    }
}
