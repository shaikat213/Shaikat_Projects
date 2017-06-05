using Finix.Auth.Dto;
using Finix.Auth.Infrastructure;
using Finix.Auth.Service;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Finix.Auth.Facade
{
    internal class AssigmentFacadeUtil
    {
        private readonly string namespaceStr = "Finix.Auth";
        private readonly GenService _service;

        public AssigmentFacadeUtil(GenService service)
        {
            _service = service;
        }

        public AssigmentFacadeUtil()
        {
            _service = new GenService();
        }

        #region static many-2-many assignment

        /// <summary>
        /// Returns Static Many2Many Table List. Should be synchronized with DB structure.
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, List<string>> GetMany2ManyDdlData()
        {
            var retVal = ManyToManyDataAssignment.ManyToMayAssociations;
            return retVal;
        }
        public List<ManyToManyTableDto> GetMany2ManyTableDataByReflection(string leftTableName, string rightTableName)
        {
            var leftTableDesciptionFields =
                ManyToManyDataAssignment.TableDescriptionFieldList.First(x => x.Key == leftTableName).Value;
            var rightTableDesciptionFields =
                ManyToManyDataAssignment.TableDescriptionFieldList.First(x => x.Key == rightTableName).Value;

            var assembly = Assembly.Load(namespaceStr);
            var leftTableType = assembly.GetType(namespaceStr + ".Infrastructure." + leftTableName);
            var rightTableType = assembly.GetType(namespaceStr + ".Infrastructure." + rightTableName);

            var method = typeof(GenService).GetMethod("GetAll");
            var genericMethodForLeft = method.MakeGenericMethod(leftTableType);
            var genericMethodForRight = method.MakeGenericMethod(rightTableType);
            var leftTableData = ((IEnumerable<object>)genericMethodForLeft.Invoke(_service, null)).ToList();
            var rightTableData = ((IEnumerable<object>)genericMethodForRight.Invoke(_service, null)).ToList();


            var retVal = leftTableData.Select(ld => new ManyToManyTableDto
            {
                IsLeftTable = true,
                Id = ((long)ld.GetType().GetProperty("Id").GetValue(ld, null)),
                Description = GetDescriptionField(ld, leftTableDesciptionFields)
            }).ToList();
            retVal.AddRange(rightTableData.Select(ld => new ManyToManyTableDto
            {
                IsLeftTable = false,
                Id = ((long)ld.GetType().GetProperty("Id").GetValue(ld, null)),
                Description = GetDescriptionField(ld, rightTableDesciptionFields)
            }));

            return retVal;
        }

        public List<long> GetMany2ManyRightTableAssignedIds(string leftTableName, string rightTableName, long leftTableRecordId)
        {
            var assembly = Assembly.Load(namespaceStr);
            var leftTableType = assembly.GetType(namespaceStr + ".Infrastructure." + leftTableName);
            var rightTableType = assembly.GetType(namespaceStr + ".Infrastructure." + rightTableName);
            var arguments = new object[] { leftTableRecordId };
            var method = typeof(GenService).GetMethod("GetById");
            var genericMethodForLeft = method.MakeGenericMethod(leftTableType);
            var leftRecord = genericMethodForLeft.Invoke(_service, arguments);
            var rightTableRecords = GetRightTableRecords(leftRecord, rightTableType);
            return GetIdsByReflection(rightTableRecords);
        }
        public List<long> SetMany2ManyRightTableAssignedIds(string leftTableName, string rightTableName, long leftTableRecordId, List<long> rightTableRecordIds)
        {
            var assembly = Assembly.Load(namespaceStr);
            var leftTableType = assembly.GetType(namespaceStr + ".Infrastructure." + leftTableName);
            var rightTableType = assembly.GetType(namespaceStr + ".Infrastructure." + rightTableName);
            var arguments = new object[] { leftTableRecordId };
            var method = typeof(GenService).GetMethod("GetById");
            var genericMethodForLeft = method.MakeGenericMethod(leftTableType);
            var leftRecord = genericMethodForLeft.Invoke(_service, arguments);
            var rightTableRecords = SetRightTableRecords(leftRecord, rightTableType, rightTableRecordIds);
            DoPostProcessingStuff(leftTableName, rightTableName, leftTableRecordId, rightTableRecordIds);
            return GetIdsByReflection(rightTableRecords);
        }

        private void DoPostProcessingStuff(string leftTableName, string rightTableName, long leftTableRecordId, List<long> rightTableRecordIds)
        {
            if (leftTableName.ToLower() == "user" && rightTableName.ToLower() == "role")
            {
                var user = _service.GetById<User>(leftTableRecordId);
                var roles = _service.GetAll<Role>().Where(x => rightTableRecordIds.Contains(x.Id)).ToList();
                user.Menus = new List<Menu>();
                foreach (var menu in roles.SelectMany(role => role.Menus))
                {
                    user.Menus.Add(menu);
                }
                _service.SaveChanges();
            }
        }
        private string GetDescriptionField(object obj, IEnumerable<string> propNames)
        {
            var props = obj.GetType().GetProperties();
            var propVals = propNames.Select(p => (props.First(x => x.Name == p).GetValue(obj, null) ?? string.Empty).ToString()).ToList();
            return string.Join(" ", propVals.ToArray());
        }
        private object GetRightTableRecords(object leftTableRecord, Type rightTableType)
        {
            var propName = rightTableType.Name + "s";
            var propList = leftTableRecord.GetType().GetProperties();

            return (from p in propList where p.Name == propName && p.PropertyType.Name == "ICollection`1" select p.GetValue(leftTableRecord, null)).FirstOrDefault();
        }
        private object SetRightTableRecords(object leftTableRecord, Type rightTableType, List<long> rightTableRecordIds)
        {
            var propName = rightTableType.Name + "s";
            var method = typeof(GenService).GetMethod("GetAll");
            var genericMethodForRight = method.MakeGenericMethod(rightTableType);
            var rightTableData = ((IEnumerable<object>)genericMethodForRight.Invoke(_service, null)).ToList();

            for (var k = rightTableData.Count - 1; k >= 0; k--)
            {
                if (!rightTableRecordIds.Contains(((Entity)rightTableData[k]).Id))
                    rightTableData.RemoveAt(k);
            }

            var propList = leftTableRecord.GetType().GetProperties();
            var prop = propList.First(x => x.Name == propName && x.PropertyType.Name == "ICollection`1");
            var propVal = prop.GetValue(leftTableRecord, null);
            prop.PropertyType.GetMethod("Clear").Invoke(propVal, null);
            foreach (var item in rightTableData)
            {
                prop.PropertyType.GetMethod("Add").Invoke(propVal, new[] { (Entity)item });
            }
            var saveChanges = typeof(GenService).GetMethod("SaveChanges");
            saveChanges.Invoke(_service, null);
            return rightTableData;
        }
        private List<long> GetIdsByReflection(object recordList)
        {
            return (from object o in (IEnumerable)recordList select ((Entity)o).Id).ToList();
        }

        #endregion

        #region static independent data crud

        /// <summary>
        /// Returns Static ModuleNames and theier corresponding Table List for CRUD Operation
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, List<string>> GetFlatTablesForCrudOperation()
        {
            var retVal = new Dictionary<string, List<string>>
            {
                {"Product", new List<string>{"ProductCategory","ProductSize","ProductColors"}},
                {"BasicData", new List<string>{"Country","Bank","Location"}},
                {"Role", new List<string>{"Task","User"}},
                {"User", new List<string>{"Menu"}}
            };

            return retVal;
        }

        #endregion
    }
}
