using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finix.Auth.Facade
{
    internal static class ManyToManyDataAssignment
    {
        public static Dictionary<string, List<string>> ManyToMayAssociations { get; private set; }
        public static Dictionary<string, List<string>> TableDescriptionFieldList { get; private set; }

        static ManyToManyDataAssignment()
        {
            ManyToMayAssociations = new Dictionary<string, List<string>>
            {
                {"Role", new List<string>{"Menu"}},
                {"User", new List<string>{"Role"}},
                {"Application", new List<string>{"Module"}},
                //{"Institute", new List<string>{"Degree"}},
                //{"TrainingVendor", new List<string>{"Training"}},
                {"Menu", new List<string>{"Task"}}
            };
            TableDescriptionFieldList = new Dictionary<string, List<string>>
            {
                {"Role", new List<string>{"Name","Description"}},
                {"Task", new List<string>{"Name","Description"}},
                {"User", new List<string>{"UserName"}},
                {"Menu", new List<string>{"Name","Url"}},
                //{"Institute", new List<string>{"Name"}},
                //{"Degree", new List<string>{"Name"}},
                //{"TrainingVendor", new List<string>{"VendorName"}},
                //{"Training", new List<string>{"TrainingName"}},
                {"Application", new List<string>{"Name"}},
                {"Module", new List<string>{"Name"}}
            };
        }
    }
}
