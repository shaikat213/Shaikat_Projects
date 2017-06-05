using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Finix.IPDC.DTO;

namespace Finix.IPDC.Infrastructure.Models
{
    public class ViewModelDemo
    {
        public PagedList.IPagedList<CIF_PersonalDto> Personal { get; set; }
    }
}
