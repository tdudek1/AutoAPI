﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace AutoAPI
{
    public class RouteInfo
    {
        public APIEntity Entity{ get; set; }
        public string Id { get; set; }
        public String FilterExpression { get; set; }
        public object[] FilterValues { get; set; }
        public int Skip { get; set; }
        public int Take { get; set; }
        public string SortExpression { get; set; }
        public bool HasModifiers { get; set; }

    }
}
