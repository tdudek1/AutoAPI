using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AutoAPI
{
    [AttributeUsage(AttributeTargets.Property)]
    public class AutoAPIEntity : Attribute
    {
        public string Route { get; set; }

		public string GETPolicy { get; set; }

		public string POSTtPolicy { get; set; }

		public string PUTPolicy { get; set; }

		public string DELETEPolicy { get; set; }

	}
}
