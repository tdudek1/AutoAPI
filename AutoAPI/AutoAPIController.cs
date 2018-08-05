using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;

namespace AutoAPI
{
    public class AutoAPIController : ControllerBase
    {
        private readonly DbContext context;
        private readonly IRequestProcessor requestProcessor;
        public AutoAPIController(DbContext context)
        {
            this.context = context;
            this.requestProcessor = new RequestProcessor();
        }

        public AutoAPIController(DbContext context, IRequestProcessor requestProcessor)
        {
            this.context = context;
            this.requestProcessor = requestProcessor;
        }


        [HttpGet]
        public IActionResult Get()
        {
            var routeInfo = requestProcessor.GetRoutInfo(this.RouteData);

            if (routeInfo.Entity == null)
                return NotFound();

            if (routeInfo.Id != null)
            {
                return Ok(((IQueryable)routeInfo.Entity.DbSet.GetValue(context)).Where("Id == @0", routeInfo.Id).FirstOrDefault());
            }
            else if (this.Request?.Query.Count > 0)
            {
                IQueryable dbSet = ((IQueryable)routeInfo.Entity.DbSet.GetValue(context));

                var fitler = requestProcessor.GetFilter(routeInfo.Entity, this.Request.Query);
                var paging = requestProcessor.GetPaging(this.Request.Query);

                if (fitler.Expression != null)
                    dbSet = dbSet.Where(fitler.Item1, fitler.Item2);

                if (paging.Take != 0)
                    dbSet = dbSet.Skip(paging.Skip).Take(paging.Take);


                return Ok(dbSet);
            }
            else
            {
                return Ok(routeInfo.Entity.DbSet.GetValue(context));
            }
        }

        [HttpPost]
        public IActionResult Post()
        {
            var routeInfo = requestProcessor.GetRoutInfo(this.RouteData);

            if (routeInfo.Entity == null)
                return NotFound();

            var entity = requestProcessor.GetData(this.Request, routeInfo.Entity.EntityType);

            context.Add(entity);
            context.SaveChanges();

            return Created(routeInfo.Entity.Route, entity);
        }

        [HttpPut]
        public IActionResult Put()
        {
            var routeInfo = requestProcessor.GetRoutInfo(this.RouteData);

            if (routeInfo.Entity == null || routeInfo.Id == null)
                return NotFound();

            var entity = requestProcessor.GetData(this.Request, routeInfo.Entity.EntityType);
            var objectId = Convert.ChangeType(routeInfo.Entity.Id.GetValue(entity), routeInfo.Entity.Id.PropertyType);
            var routeId = Convert.ChangeType(routeInfo.Id, routeInfo.Entity.Id.PropertyType);

            if (!objectId.Equals(routeId))
                return BadRequest();

            context.Entry(entity).State = EntityState.Modified;
            context.SaveChanges();

            return Ok(entity);
        }

        [HttpDelete]
        public IActionResult Delete()
        {
            var routeInfo = requestProcessor.GetRoutInfo(this.RouteData);

            if (routeInfo.Entity == null)
                return NotFound();

            object entity = ((dynamic)routeInfo.Entity.DbSet.GetValue(context)).Find(Convert.ChangeType(routeInfo.Id, routeInfo.Entity.Id.PropertyType));

            if (entity == null)
            {
                return NotFound();
            }
            context.Remove(entity);
            context.SaveChanges();
            return Ok();
        }
    }
}
