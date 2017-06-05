using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Finix.Auth.Infrastructure;
using Finix.Auth.Service;

namespace Finix.Auth.Infrastructure
{
    public class JqGridModel
    {
        public bool _search { get; set; }
        public int rows { get; set; }
        public int pages { get; set; }
        public string sidx { get; set; }
        public string sord { get; set; }
        public JqFilter filters { get; set; }
    }

    public class JqFilter
    {
        public string groupOp { get; set; }
        public List<JqFilterRule> rules { get; set; } 

    }

    public class JqFilterRule
    {
        public string field { get; set; }
        public string op { get; set; }
        public string data { get; set; }
    }
}