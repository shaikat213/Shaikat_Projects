using Finix.IPDC.Infrastructure;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Finix.IPDC.Util
{
    public static class UiUtil
    {

        public static Dictionary<string, string> GetOptionsForJqGridDdl<T>(List<T> items, string valueProperty, string displayProperty) where T : class
        {
            var type = items.First().GetType();
            var valuePropertyInfo = type.GetProperty(valueProperty);
            var displayPropertyInfo = type.GetProperty(displayProperty);

            var retVal = new Dictionary<string, string>();
            foreach (var item in items)
            {
                var v = valuePropertyInfo.GetValue(item, null).ToString();
                var d = displayPropertyInfo.GetValue(item, null).ToString();
                retVal.Add(v, d);
            }
            return retVal;
        }

        //public static List<KeyValuePair<int, string>> EnumToKeyVal<T>(bool orderbyValue = true)
        //{
        //    var t = typeof(T);
        //    return orderbyValue
        //        ? Enum.GetNames(t)
        //            .Select(s => new KeyValuePair<int, string>((int)Enum.Parse(t, s), s))
        //            .ToList().OrderBy(x => x.Value).ToList()
        //        : Enum.GetNames(t)
        //            .Select(s => new KeyValuePair<int, string>((int)Enum.Parse(t, s), s))
        //            .ToList().OrderBy(x => x.Key).ToList();
        //}

        public static List<KeyValuePair<int, string>> EnumToKeyVal<T>(bool orderbyValue = true)
            //where T : struct
        {
            var t = typeof(T);
            //try
            //{
            //    var typeList = Enum.GetValues(t)
            //   .Cast<T>()
            //   .Select(s => new
            //   {
            //       key = (int)Enum.Parse(t, s.ToString()),
            //       value = GetDisplayName(s)
            //       //    (int)Enum.Parse(t, s.ToString()), GetDisplayName(s)) //GetDisplayName(Enum.GetValues(t).GetType().GetMember(s.ToString()).First())
            //   }).ToList();
            //    foreach(var item in typeList)
            //    {
            //        var temp = Enum.GetValues(t).Cast<T>().GetType().GetMember(item.Value).ToList();//.ToList();
            //    }
            //    //return typeList;
            //}
            //catch (Exception ex)
            //{
            //    //return null;
            //}

            return orderbyValue
                ? Enum.GetNames(t)
                    .Select(s => new KeyValuePair<int, string>((int)Enum.Parse(t, s), s))
                    .ToList().OrderBy(x => x.Value).ToList()
                : Enum.GetNames(t)
                    .Select(s => new KeyValuePair<int, string>((int)Enum.Parse(t, s), s))
                    .ToList().OrderBy(x => x.Key).ToList();
        }
        //var typeList = Enum.GetValues(typeof(HomeOwnership))
        //   .Cast<HomeOwnership>()
        //   .Select(t => new CurrentResidenceYearsDto
        //   {
        //       Id = ((int)t),
        //       Name = t.ToString(),
        //       DisplayName = UiUtil.GetDisplayName(t)
        //   });
        public static string GetDisplayName(Enum enumValue)
        {
            if (enumValue != null)
            {
                var temp = enumValue.GetType().GetMember(enumValue.ToString())
                          .First();
                if (temp.GetCustomAttribute<DisplayAttribute>() != null)
                    return temp.GetCustomAttribute<DisplayAttribute>().Name;
                else
                    return temp.Name;
            }
            return "";
        }

        public static AgeRange GetAgeRange(DateTime? DateOfBirth)
        {
            if (DateOfBirth == null)
                return AgeRange.NotSpecified;

            int Age = (int)((DateOfBirth.Value - DateTime.Now).TotalDays)/365;


            if (Age < 18)
            {
                return AgeRange.Below18;
            }
            else if(Age < 31)
            {
                return AgeRange.From18To30;
            }
            else if (Age < 46)
            {
                return AgeRange.From31To45;
            }
            else if (Age < 56)
            {
                return AgeRange.From46To55;
            }
            else if (Age < 66)
            {
                return AgeRange.From56To65;
            }
            else
                return AgeRange.Over65;
        }
        public static IncomeRange GetIncomeRange(decimal? Income)
        {
            if (Income == null || Income < 20000)
                return IncomeRange.NotSpecified;
            else if (Income >= 20000 && Income <= 50000)
                return IncomeRange.From20000To50000;
            else if (Income >= 50001 && Income <= 75000)
                return IncomeRange.From50001To75000;
            else if (Income >= 75001 && Income <= 100000)
                return IncomeRange.From75001To100000;
            else if (Income >= 100001 && Income <= 150000)
                return IncomeRange.From100001To150000;
            else if (Income >= 150001 && Income <= 200000)
                return IncomeRange.From150001To200000;
            else if (Income > 200000)
                return IncomeRange.Above200000;

            return IncomeRange.NotSpecified;

        }

        //modified

        //public static string GetDisplayName(MemberInfo enumValue)
        //{
        //    //var temp = enumValue.GetType().GetMember(enumValue.ToString())
        //    //               .First();
        //    if (enumValue.GetCustomAttribute<DisplayAttribute>() != null)
        //        return enumValue.GetCustomAttribute<DisplayAttribute>().Name;
        //    else
        //        return enumValue.Name;
        //}
    }
}
