using Finix.IPDC.Facade.FacadeUtil;
using Finix.IPDC.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Finix.IPDC.Facade
{
    public abstract class BaseFacade
    {
        private readonly GenService genService;
        public JqGridModel JqGridModel { get; set; }
        protected BaseFacade()
        {
            genService = new GenService();
        }

        protected GenService GenService
        {
            get { return genService; }
        }

        #region Apply jqgFilterOnDTO
        protected IEnumerable<T> ApplyJqFilter<T>(IEnumerable<T> data, Dictionary<string, string> filterColumnModelNames = null) where T : class
        {
            if (JqGridModel == null)
                return data;

            if (filterColumnModelNames != null && filterColumnModelNames.Any() && JqGridModel.filters != null && JqGridModel.filters.rules.Any())
            {
                var rules = JqGridModel.filters.rules;
                foreach (var f in filterColumnModelNames)
                {
                    for (var i = 0; i < rules.Count; i++)
                    {
                        if (rules[i].field == f.Key)
                        {
                            rules[i].field = f.Value;
                            break;
                        }
                    }
                }
            }
            var t = typeof(T);
            var pInfoList = t.GetProperties().ToList();

            data = ExecuteJqFilterRules(data, pInfoList, t);
            data = ExecuteJqSort(data, pInfoList, t);
            data = ExecuteJqPaging(data);
            return data;
        }
        private IEnumerable<T> ExecuteJqFilterRules<T>(IEnumerable<T> data, List<PropertyInfo> pInfoList, Type entityType) where T : class
        {

            if (!JqGridModel._search || JqGridModel.filters == null || JqGridModel.filters.rules.Count <= 0)
                return data;
            var qData = data.AsQueryable();
            var expressionList = new List<Expression>();
            //ref: http://stackoverflow.com/questions/7246715/use-reflection-to-get-lambda-expression-from-property-name
            foreach (var r in JqGridModel.filters.rules)
            {
                var p = pInfoList.FirstOrDefault(x => x.Name == r.field) ?? pInfoList.Single(x => x.Name == "Id");
                var parameter = Expression.Parameter(entityType, "x");
                var property = Expression.Property(parameter, p.Name);
                var target = Expression.Constant(r.data);
                switch (r.op)
                {
                    case "cn":
                        var containsMethod = Expression.Call(property, "Contains", null, target);
                        expressionList.Add(Expression.Lambda<Func<T, bool>>(containsMethod, parameter));
                        break;

                    case "eq":
                        var equalsMethod = Expression.Call(property, "Equals", null, target);
                        //http://stackoverflow.com/questions/15722880/build-expression-equals-on-string    
                        //MethodInfo equalsMethod = typeof(string).GetMethod("Equals", new[] { typeof(string) });
                        expressionList.Add(Expression.Lambda<Func<T, bool>>(equalsMethod, parameter));
                        break;

                    default:
                        break;
                }
            }
            if (JqGridModel.filters.groupOp.ToLower() == "and")
            {
                qData = expressionList.Aggregate(qData, (current, e) =>
                    current.Where((Expression<Func<T, bool>>)e));
            }
            else
            {

            }
            return qData.ToList();
        }
        private IEnumerable<T> ExecuteJqSort<T>(IEnumerable<T> data, List<PropertyInfo> pInfoList, Type entityType) where T : class
        {

            //ref:http://stackoverflow.com/questions/307512/how-do-i-apply-orderby-on-an-iqueryable-using-a-string-column-name-within-a-gene

            if (string.IsNullOrWhiteSpace(JqGridModel.sidx) || string.IsNullOrWhiteSpace(JqGridModel.sord))
            {
                //data = data.OrderBy(x => x.Id);
                return data;
            }

            var qData = data.AsQueryable();

            //var property = pInfoList.Single(x => x.Name == JqGridModel.sidx);
            var property = pInfoList.FirstOrDefault(x => x.Name == JqGridModel.sidx) ??
                           pInfoList.Single(x => x.Name == "Id");  // for temporary solution; it should traverse nav prop.

            var parameter = Expression.Parameter(entityType, "p");
            var propertyAccess = Expression.MakeMemberAccess(parameter, property);
            var orderByExp = Expression.Lambda(propertyAccess, parameter);
            var sDir = JqGridModel.sord.ToLower() == "asc" ? "OrderBy" : "OrderByDescending";

            var resultExp = Expression.Call(typeof(Queryable), sDir,
                new Type[] { entityType, property.PropertyType }, qData.Expression,
                Expression.Quote(orderByExp));
            return qData.Provider.CreateQuery<T>(resultExp).ToList();
        }
        private IEnumerable<T> ExecuteJqPaging<T>(IEnumerable<T> data) where T : class
        {
            var row2Skip = (JqGridModel.pages) * JqGridModel.rows;
            row2Skip = row2Skip <= 0 ? 0 : row2Skip;
            var row2Take = JqGridModel.rows;
            return data.Skip(row2Skip).Take(row2Take);
        }

        #endregion
        #region Apply jqgFilterOnDB
        /*
        protected IQueryable<T> ApplyJqFilter<T>(IQueryable<T> data, Dictionary<string, string> filterColumnModelNames = null) where T : Entity
        {
            if (JqGridModel == null)
                return data;

            if (filterColumnModelNames != null && filterColumnModelNames.Any() && JqGridModel.filters != null && JqGridModel.filters.rules.Any())
            {
                var rules = JqGridModel.filters.rules;
                foreach (var f in filterColumnModelNames)
                {
                    for (var i = 0; i < rules.Count; i++)
                    {
                        if (rules[i].field == f.Key)
                        {
                            rules[i].field = f.Value;
                            break;
                        }
                    }
                }
            }
            var t = typeof(T);
            var pInfoList = t.GetProperties().ToList();

            data = ExecuteJqFilterRules(data, pInfoList, t);
            data = ExecuteJqSort(data, pInfoList, t);
            data = ExecuteJqPaging(data, pInfoList, t);
            return data;
        }
        private IQueryable<T> ExecuteJqSort<T>(IQueryable<T> data, List<PropertyInfo> pInfoList, Type entityType) where T : Entity
        {

            //ref:http://stackoverflow.com/questions/307512/how-do-i-apply-orderby-on-an-iqueryable-using-a-string-column-name-within-a-gene

            if (string.IsNullOrWhiteSpace(JqGridModel.sidx) || string.IsNullOrWhiteSpace(JqGridModel.sord))
            {
                data = data.OrderBy(x => x.Id);
                return data;
            }
            //var property = pInfoList.Single(x => x.Name == JqGridModel.sidx);
            var property = pInfoList.FirstOrDefault(x => x.Name == JqGridModel.sidx) ??
                           pInfoList.Single(x => x.Name == "Id");  // for temporary solution; it should traverse nav prop.

            var parameter = Expression.Parameter(entityType, "p");
            var propertyAccess = Expression.MakeMemberAccess(parameter, property);
            var orderByExp = Expression.Lambda(propertyAccess, parameter);
            var sDir = JqGridModel.sord.ToLower() == "asc" ? "OrderBy" : "OrderByDescending";
            var resultExp = Expression.Call(typeof(Queryable), sDir,
                new Type[] { entityType, property.PropertyType }, data.Expression,
                Expression.Quote(orderByExp));
            return data.Provider.CreateQuery<T>(resultExp);
        }
        private IQueryable<T> ExecuteJqPaging<T>(IQueryable<T> data, List<PropertyInfo> pInfoList, Type entityType) where T : Entity
        {
            var row2Skip = (JqGridModel.pages) * JqGridModel.rows;
            row2Skip = row2Skip <= 0 ? 0 : row2Skip;
            var row2Take = JqGridModel.rows;
            return data.Skip(row2Skip).Take(row2Take);
        }
        private IQueryable<T> ExecuteJqFilterRules<T>(IQueryable<T> data, List<PropertyInfo> pInfoList, Type entityType) where T : Entity
        {

            if (!JqGridModel._search || JqGridModel.filters == null || JqGridModel.filters.rules.Count <= 0)
                return data;

            var expressionList = new List<Expression>();
            //ref: http://stackoverflow.com/questions/7246715/use-reflection-to-get-lambda-expression-from-property-name
            foreach (var r in JqGridModel.filters.rules)
            {
                var p = pInfoList.FirstOrDefault(x => x.Name == r.field) ?? pInfoList.Single(x => x.Name == "Id");
                var parameter = Expression.Parameter(entityType, "x");
                var property = Expression.Property(parameter, p.Name);
                var target = Expression.Constant(r.data);
                switch (r.op)
                {
                    case "cn":
                        var containsMethod = Expression.Call(property, "Contains", null, target);
                        expressionList.Add(Expression.Lambda<Func<T, bool>>(containsMethod, parameter));
                        break;

                    case "eq":
                        var equalsMethod = Expression.Call(property, "Equals", null, target);
                        //http://stackoverflow.com/questions/15722880/build-expression-equals-on-string    
                        //MethodInfo equalsMethod = typeof(string).GetMethod("Equals", new[] { typeof(string) });
                        expressionList.Add(Expression.Lambda<Func<T, bool>>(equalsMethod, parameter));
                        break;

                    default:
                        break;
                }
            }
            if (JqGridModel.filters.groupOp.ToLower() == "and")
            {
                data = expressionList.Aggregate(data, (current, e) =>
                    current.Where((Expression<Func<T, bool>>)e));
            }
            else
            {

            }
            return data;
        }
        */
        #endregion
    }
}
