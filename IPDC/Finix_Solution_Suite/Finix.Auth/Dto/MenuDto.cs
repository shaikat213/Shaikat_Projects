using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finix.Auth.DTO
{
    public class MenuDto
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public string Description { get; set; }
        public int Sl { get; set; }
        public string Url { get; set; }
        public long ModuleId { get; set; }
        public string ModuleName { get; set; }
        public long SubModuleId { get; set; }
        public string SubModuleName { get; set; }
    }
    
}
