using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace AutoAPI
{
    public class APIEntity
    {
        public string Route { get; set; }

		public string GETPolicy { get; set; }

		public string POSTtPolicy { get; set; }

		public string PUTPolicy { get; set; }

		public string DELETEPolicy { get; set; }

		public PropertyInfo DbSet { get; set; }

        public Type EntityType { get; set; }

        public List<PropertyInfo> Properties { get; set; }

        public PropertyInfo Id { get; set; }
    }
}
