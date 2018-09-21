using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace AutoAPI
{
    public class APIEntity
    {
        //
        // Summary:
        //     Route to entity REST endpoint
        public string Route { get; set; }
        
        //
        // Summary:
        //     Policy name for policy based authorization for GET method
        public string GETPolicy { get; set; }

        //
        // Summary:
        //     Policy name for policy based authorization for POST method
        public string POSTPolicy { get; set; }

        //
        // Summary:
        //     Policy name for policy based authorization for PUT method
        public string PUTPolicy { get; set; }

        //
        // Summary:
        //     Policy name for policy based authorization for DELETE method
        public string DELETEPolicy { get; set; }
        
        //
        // Summary:
        //     Policy name for policy based authorization for all methods
        public string EntityPolicy { get; set; }

        //
        // Summary:
        //     Set to true to require user to be logged in
        public bool Authorize { get; set; }

        public PropertyInfo DbSet { get; set; }

        public Type EntityType { get; set; }

        public List<PropertyInfo> Properties { get; set; }

        public PropertyInfo Id { get; set; }

        public Type DbContextType { get; set; }
    }
}
