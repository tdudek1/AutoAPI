using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace AutoAPI.Web.Controllers
{
	public class TestController : ControllerBase
	{
		[Route("/api/test/test")]
		public IActionResult Index()
		{
			return this.Ok();
		}
	}
}
