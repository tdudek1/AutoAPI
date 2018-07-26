using AutoAPI.API;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;

namespace AutoAPI.Controllers
{


    [Route("/api/data/{*query}")]
    public class DataController : ControllerBase
    {
        private readonly DataContext context;

        public DataController(DataContext context)
        {
            this.context = context;
        }


        [HttpGet]
        public IActionResult Get()
        {

            var routeInfo = this.GetRoutInfo();

            if (routeInfo.Entity == null)
                return NotFound();

            if (routeInfo.Id == null)
                return Ok(routeInfo.Entity.DbSet.GetValue(context));
            else
                return Ok(((IQueryable)routeInfo.Entity.DbSet.GetValue(context)).Where("Id == @0", routeInfo.Id).FirstOrDefault());
        }

        [HttpPost]
        public IActionResult Post()
        {
            var routeInfo = this.GetRoutInfo();

            if (routeInfo.Entity == null)
                return NotFound();

            context.Add(routeInfo.Data);
            context.SaveChanges();

            return Created(routeInfo.Entity.Route, routeInfo.Data);
        }

        [HttpPut]
        public IActionResult Put()
        {
            var routeInfo = this.GetRoutInfo();

            if (routeInfo.Entity == null)
                return NotFound();

            context.Entry(routeInfo.Data).State = EntityState.Modified;
            context.SaveChanges();

            return Ok();
        }

        [HttpDelete]
        public IActionResult Delete()
        {
            var routeInfo = this.GetRoutInfo();

            if (routeInfo.Entity == null)
                return NotFound();

            object entity = ((dynamic)routeInfo.Entity.DbSet.GetValue(context)).Find(routeInfo.Id);
            if (entity != null)
            {
                context.Remove(entity);
            }
            context.SaveChanges();
            return Ok();
        }
    }
}
