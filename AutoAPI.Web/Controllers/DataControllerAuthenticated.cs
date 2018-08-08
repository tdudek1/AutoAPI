using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AutoAPI.Web.Controllers
{
	[Route("/api/test")]
    public class DataControllerAuthenticated :ControllerBase
	{
        DataContext context;
        UserManager<IdentityUser> userManager;

        public DataControllerAuthenticated(DbContext context, UserManager<IdentityUser> userManager ) 
		{
            this.context = (DataContext)context;
            this.userManager = userManager;
		}

        public async Task<ActionResult> Get()
        {
            var user = await userManager.FindByEmailAsync("test@test.com");
            return this.Ok();
        }
	}
}
