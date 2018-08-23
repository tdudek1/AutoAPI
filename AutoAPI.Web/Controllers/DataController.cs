﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AutoAPI.Web.Controllers
{
	[Route("/api/data/{*query}")]
	public class DataController : AutoAPI.AutoAPIController
	{
		public DataController(DataContext context) : base(context)
		{

		}
	}
}
