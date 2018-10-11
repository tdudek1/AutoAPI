using Microsoft.AspNetCore.Http;
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

        public string POSTPolicy { get; set; }

        public string PUTPolicy { get; set; }

        public string DELETEPolicy { get; set; }
        
        public string EntityPolicy { get; set; }

        public bool Authorize { get; set; }

        public PropertyInfo DbSet { get; set; }

        public Type EntityType { get; set; }

        public List<PropertyInfo> Properties { get; set; }

        public List<PropertyInfo> NavigationProperties { get; set; }

        public PropertyInfo Id { get; set; }

        public Type DbContextType { get; set; }

        public bool ExposePagedResult { get; set; }
    }
}
