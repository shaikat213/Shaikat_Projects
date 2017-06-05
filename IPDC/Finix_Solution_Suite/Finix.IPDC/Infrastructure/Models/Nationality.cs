﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finix.IPDC.Infrastructure.Models
{
    [Table("Nationality")]
    public class Nationality : Entity
    {
        public string Name { get; set; }
    }
}
