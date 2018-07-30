using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace AutoAPI.API
{
    public class RouteInfo
    {
        public APIEntity Entity{ get; set; }
        public string Id { get; set; }
        public Dictionary<string, object> Filters { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
    }
}
