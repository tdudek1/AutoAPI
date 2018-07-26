using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AutoAPI.API
{
    [AttributeUsage(AttributeTargets.Property)]
    public class AutoAPIEntity : Attribute
    {
        public string Route { get; set; }
    }
}
