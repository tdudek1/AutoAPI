﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AutoAPI
{
    public interface IRequestProcessor
    {
        RouteInfo GetRoutInfo(RouteData routeData, IQueryCollection queryString = null);

        object GetData(HttpRequest request, Type type);

        ModelStateDictionary Validate(ControllerBase controller, object entity);
    }
}
