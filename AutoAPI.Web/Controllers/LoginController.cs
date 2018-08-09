using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoAPI.Web.Controllers
{
    [Route("/login")]
    public class LoginController : ControllerBase
    {
        DataContext context;
        UserManager<IdentityUser> signInManager;

        public LoginController(DbContext context, UserManager<IdentityUser> signInManager)
        {
            this.context = (DataContext)context;
            this.signInManager = signInManager;
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<ActionResult> Login([FromForm] String UserName, [FromForm] String Password)
        {
            var user = await signInManager.FindByNameAsync(UserName);

            if (user != null && await signInManager.CheckPasswordAsync(user, Password))
            {
                return Ok();
            }
            else
            {
                return Unauthorized();
            }
        }

    }
}
