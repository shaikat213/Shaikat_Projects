using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finix.Auth.DTO
{
    public class SubModuleDto
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public string Description { get; set; }
        public int Sl { get; set; }
        public int ColSpan { get; set; }
        public long ModuleId { get; set; }
        public string ModuleName { get; set; }
    }

}
