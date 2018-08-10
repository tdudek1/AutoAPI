using Microsoft.AspNetCore.Authorization;
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
        private readonly IAuthorizationService authorizationService;

        public AutoAPIController(DbContext context)
        {
            this.context = context;
            this.requestProcessor = new RequestProcessor();
        }

        public AutoAPIController(DbContext context, IAuthorizationService authorizationService)
        {
            this.context = context;
            this.requestProcessor = new RequestProcessor();
            this.authorizationService = authorizationService;
        }

        public AutoAPIController(DbContext context, IRequestProcessor requestProcessor, IAuthorizationService authorizationService)
        {
            this.context = context;
            this.requestProcessor = requestProcessor;
            this.authorizationService = authorizationService;
        }


        [HttpGet]
        public IActionResult Get()
        {
            var routeInfo = requestProcessor.GetRoutInfo(RouteData, Request);

            if (routeInfo.Entity == null)
                return NotFound();

            if (!requestProcessor.Authorize(User, routeInfo.Entity.GETPolicy, authorizationService))
                return Unauthorized();


            if (routeInfo.Id != null)
            {
                return Ok(((IQueryable)routeInfo.Entity.DbSet.GetValue(context)).Where("Id == @0", routeInfo.Id).FirstOrDefault());
            }
            else if (routeInfo.HasModifiers)
            {
                IQueryable dbSet = ((IQueryable)routeInfo.Entity.DbSet.GetValue(context));

                if (routeInfo.FilterExpression != null)
                    dbSet = dbSet.Where(routeInfo.FilterExpression, routeInfo.FilterValues);

                if (routeInfo.Take != 0)
                    dbSet = dbSet.Skip(routeInfo.Skip).Take(routeInfo.Take);

                if (routeInfo.SortExpression != null)
                {
                    dbSet = dbSet.OrderBy(routeInfo.SortExpression);
                }

                return Ok(dbSet.ToDynamicList());
            }
            else
            {
                return Ok(routeInfo.Entity.DbSet.GetValue(context));
            }
        }

        [HttpPost]
        public IActionResult Post()
        {
            var routeInfo = requestProcessor.GetRoutInfo(RouteData);

            if (routeInfo.Entity == null)
                return NotFound();

            if (!requestProcessor.Authorize(User, routeInfo.Entity.POSTtPolicy, authorizationService))
                return Unauthorized();

            var entity = requestProcessor.GetData(Request, routeInfo.Entity.EntityType);

            if (!requestProcessor.Validate(this, entity))
                return BadRequest(ModelState);

            context.Add(entity);
            context.SaveChanges();

            return Created(routeInfo.Entity.Route, entity);
        }

        [HttpPut]
        public IActionResult Put()
        {
            var routeInfo = requestProcessor.GetRoutInfo(RouteData);

            if (routeInfo.Entity == null || routeInfo.Id == null)
                return NotFound();

            if (!requestProcessor.Authorize(User, routeInfo.Entity.PUTPolicy, authorizationService))
                return Unauthorized();

            var entity = requestProcessor.GetData(Request, routeInfo.Entity.EntityType);
            var objectId = Convert.ChangeType(routeInfo.Entity.Id.GetValue(entity), routeInfo.Entity.Id.PropertyType);
            var routeId = Convert.ChangeType(routeInfo.Id, routeInfo.Entity.Id.PropertyType);

            if (!requestProcessor.Validate(this, entity))
                return BadRequest(ModelState);

            if (!objectId.Equals(routeId))
                return BadRequest();

            context.Entry(entity).State = EntityState.Modified;
            context.SaveChanges();

            return Ok(entity);
        }

        [HttpDelete]
        public IActionResult Delete()
        {
            var routeInfo = requestProcessor.GetRoutInfo(RouteData);

            if (routeInfo.Entity == null || routeInfo.Id == null)
                return NotFound();

            if(!requestProcessor.Authorize(User, routeInfo.Entity.DELETEPolicy, authorizationService))
                return Unauthorized();

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
