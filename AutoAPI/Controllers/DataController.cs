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
        private readonly DbContext context;

        public DataController(DbContext context)
        {
            this.context = context;
        }


        [HttpGet]
        public IActionResult Get()
        {

            //var routeInfo = this.GetRoutInfo();

            //if (routeInfo.Entity == null)
            //    return NotFound();

            //if (routeInfo.Id == null)
            //    return Ok(routeInfo.Entity.DbSet.GetValue(context));
            //else
            //    return Ok(((IQueryable)routeInfo.Entity.DbSet.GetValue(context)).Where("Id == @0", routeInfo.Id).FirstOrDefault());
        }

        [HttpPost]
        public IActionResult Post()
        {
            //var routeInfo = this.GetRoutInfo();

            //if (routeInfo.Entity == null)
            //    return NotFound();

            //context.Add(routeInfo.Data);
            //context.SaveChanges();

            //return Created(routeInfo.Entity.Route, routeInfo.Data);
        }

        [HttpPut]
        public IActionResult Put()
        {
            //var routeInfo = this.GetRoutInfo();

            //if (routeInfo.Entity == null || routeInfo.Id == null)
            //    return NotFound();

            //var objectId = Convert.ChangeType(routeInfo.Entity.Id.GetValue(routeInfo.Data), routeInfo.Entity.Id.PropertyType);
            //var routeId = Convert.ChangeType(routeInfo.Id, routeInfo.Entity.Id.PropertyType);

            //if (!objectId.Equals(routeId))
            //    return BadRequest();

            //context.Entry(routeInfo.Data).State = EntityState.Modified;
            //context.SaveChanges();

            //return Ok(routeInfo.Data);
        }

        [HttpDelete]
        public IActionResult Delete()
        {
            //var routeInfo = this.GetRoutInfo();

            //if (routeInfo.Entity == null)
            //    return NotFound();

            //object entity = ((dynamic)routeInfo.Entity.DbSet.GetValue(context)).Find(Convert.ChangeType(routeInfo.Id, routeInfo.Entity.Id.PropertyType));

            //if (entity == null)
            //{
            //    return NotFound();
            //}
            //context.Remove(entity);
            //context.SaveChanges();
            //return Ok();
        }
    }
}
