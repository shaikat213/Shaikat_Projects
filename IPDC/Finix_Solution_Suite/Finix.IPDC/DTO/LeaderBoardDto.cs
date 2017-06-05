using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finix.IPDC.DTO
{
    public class LeaderBoardDto
    {
        public List<PerformerDto> TopFirst { get; set; }
        public List<PerformerDto> TopSecond { get; set; }
        public List<PerformerDto> BottomFirst { get; set; }
        public List<PerformerDto> BottomSecond { get; set; }
    }
}
