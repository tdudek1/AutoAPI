using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AutoAPI.Web.Controllers
{
    [Route("/api/authdata/{*query}")]
    public class AuthorizedDataController : AutoAPI.AutoAPIController
    {
        public AuthorizedDataController(DbContext context, IAuthorizationService authorizationService) : base(context, authorizationService)
        {

        }
    }
}
