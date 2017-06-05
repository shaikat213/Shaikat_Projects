using Finix.Auth.Dto;
using Finix.Auth.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finix.Auth.Facade
{
    public class AssigmentFacade : BaseFacade
    {
        private readonly AssigmentFacadeUtil basicDataFacadeUtil = new AssigmentFacadeUtil();
        public Dictionary<string, List<string>> GetMany2ManyDdlData()
        {
            return basicDataFacadeUtil.GetMany2ManyDdlData();
        }
        public List<ManyToManyTableDto> GetMany2ManyTableDataByReflection(string leftTableName, string rightTableName)
        {
            return basicDataFacadeUtil.GetMany2ManyTableDataByReflection(leftTableName, rightTableName);
        }

        public List<long> GetMany2ManyRightTableAssignedIds(string leftTableName, string rightTableName, long leftTableRecordId)
        {
            return basicDataFacadeUtil.GetMany2ManyRightTableAssignedIds(leftTableName, rightTableName,
                leftTableRecordId);
        }
        public List<long> SetMany2ManyRightTableAssignedIds(string leftTableName, string rightTableName, long leftTableRecordId, List<long> rightTableRecordIds)
        {
            return basicDataFacadeUtil.SetMany2ManyRightTableAssignedIds(leftTableName, rightTableName,
                leftTableRecordId, rightTableRecordIds);
        }
        public object GetJsonData<T>(int count = 0) where T : Entity
        {

            var data = count <= 0
                ? GenService.GetAll<T>().ToList()
                : GenService.GetAll<T>().Take(count).ToList();
            return data;
        }
        //public object DummyOperation()
        //{
        //    var data = GetJsonData<Country>();
        //    return data;
        //}
    }
}
