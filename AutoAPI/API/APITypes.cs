using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace AutoAPI.API
{
    public class APIEntity
    {
        public string Route { get; set; }

        public PropertyInfo DbSet { get; set; }

        public Type EntityType { get; set; }

        public List<PropertyInfo> Properties { get; set; }

        public PropertyInfo Id { get; set; }
    }
}
