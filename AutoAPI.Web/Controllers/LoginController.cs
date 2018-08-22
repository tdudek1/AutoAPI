using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace AutoAPI.Web.Controllers
{
    [Route("/login")]
    public class LoginController : ControllerBase
    {
        UserManager<IdentityUser> userManager;

        public LoginController(UserManager<IdentityUser> userManager)
        {
            this.userManager = userManager;
        }

        [HttpPost]
        public async Task<ActionResult> Login([FromForm] String UserName, [FromForm] String Password)
        {
            var user = await userManager.FindByNameAsync(UserName);

            if (user != null && await userManager.CheckPasswordAsync(user, Password))
            {
                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("SuperDuperSecureKey"));
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                var token = new JwtSecurityToken(
                    issuer: "test.com",
                    audience: "test.com",
                    claims: userManager.GetClaimsAsync(user).Result,
                    expires: DateTime.Now.AddMinutes(30),
                    signingCredentials: creds);
                
                return Ok(new
                {
                    token = new JwtSecurityTokenHandler().WriteToken(token)
                });
            }
            else
            {
                return Unauthorized();
            }
        }

        [HttpGet]
        [Route("test")]
        [Authorize]
        public async Task<ActionResult> Test()
        {
            var user = await userManager.FindByNameAsync(User.Claims.Where(x => x.Type == ClaimTypes.Name).FirstOrDefault()?.Value);
            return Ok(user);
        }

    }
}
