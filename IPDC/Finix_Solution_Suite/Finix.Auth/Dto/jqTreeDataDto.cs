using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finix.Auth.DTO
{
    public class jsTreeDataDto
    {
        public jsTreeDataDto()
        {
            state = new jsTreeNodeState();
            children = new List<jsTreeDataDto>();
        }
        public string id { get; set; }
        public string text { get; set; }
        public string type { get; set; }
        public long typeId { get; set; }
        public jsTreeNodeState state { get; set; }
        public string icon { get; set; }
        public List<jsTreeDataDto> children { get; set; }
    }

    public class jsTreeNodeState
    {
        public jsTreeNodeState()
        {
            opened = false;
            disabled = false;
            selected = false;
        }
        public bool opened { get; set; }
        public bool disabled { get; set; }
        public bool selected { get; set; }
    }
}
