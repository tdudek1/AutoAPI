using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Swashbuckle.AspNetCore.Swagger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace AutoAPI
{
    public class AutoAPIController
    {
        private readonly DbContext context;
        private readonly IRequestProcessor requestProcessor;
        
        public AutoAPIController(DbContext context, IRequestProcessor requestProcessor)
        {
            this.context = context;
            this.requestProcessor = requestProcessor;
        }

        public ObjectResult Get(RouteInfo routeInfo)
        {
            if (routeInfo.Id != null)
            {
                var result = ((dynamic)routeInfo.Entity.DbSet.GetValue(context)).Find(Convert.ChangeType(routeInfo.Id, routeInfo.Entity.Id.PropertyType));
                if (result != null)
                {
                    return new  OkObjectResult(result);
                }
                else
                {
                    return new NotFoundObjectResult(null);
                }
            }
            else if (routeInfo.HasModifiers)
            {
                IQueryable dbSet = ((IQueryable)routeInfo.Entity.DbSet.GetValue(context));

                if (!string.IsNullOrWhiteSpace(routeInfo.FilterExpression))
                {
                    dbSet = dbSet.Where(routeInfo.FilterExpression, routeInfo.FilterValues);
                }

                if (routeInfo.Take != 0)
                {
                    dbSet = dbSet.Skip(routeInfo.Skip).Take(routeInfo.Take);
                }

                if (!string.IsNullOrWhiteSpace(routeInfo.SortExpression))
                {
                    dbSet = dbSet.OrderBy(routeInfo.SortExpression);
                }

                return new OkObjectResult(dbSet.ToDynamicList());
            }
            else
            {
                return new OkObjectResult(routeInfo.Entity.DbSet.GetValue(context));
            }
        }

        //[HttpPost]
        //public IActionResult Post()
        //{
        //    var routeInfo = requestProcessor.GetRoutInfo(RouteData, Request);

        //    if (routeInfo.Entity == null)
        //    {
        //        return NotFound();
        //    }

        //    if (!requestProcessor.Authorize(User, routeInfo.Entity.POSTtPolicy, authorizationService))
        //    {
        //        return Unauthorized();
        //    }

        //    var entity = requestProcessor.GetData(Request, routeInfo.Entity.EntityType);

        //    if (!requestProcessor.Validate(this, entity))
        //    {
        //        return BadRequest(ModelState);
        //    }

        //    context.Add(entity);
        //    context.SaveChanges();

        //    return Created(routeInfo.Entity.Route, entity);
        //}

        //[HttpPut]
        //public IActionResult Put()
        //{
        //    var routeInfo = requestProcessor.GetRoutInfo(RouteData, Request);

        //    if (routeInfo.Entity == null || routeInfo.Id == null)
        //    {
        //        return NotFound();
        //    }

        //    if (!requestProcessor.Authorize(User, routeInfo.Entity.PUTPolicy, authorizationService))
        //    {
        //        return Unauthorized();
        //    }

        //    var entity = requestProcessor.GetData(Request, routeInfo.Entity.EntityType);
        //    var objectId = Convert.ChangeType(routeInfo.Entity.Id.GetValue(entity), routeInfo.Entity.Id.PropertyType);
        //    var routeId = Convert.ChangeType(routeInfo.Id, routeInfo.Entity.Id.PropertyType);

        //    if (!requestProcessor.Validate(this, entity))
        //    {
        //        return BadRequest(ModelState);
        //    }

        //    if (!objectId.Equals(routeId))
        //    {
        //        return BadRequest();
        //    }

        //    context.Entry(entity).State = EntityState.Modified;
        //    context.SaveChanges();

        //    return Ok(entity);
        //}

        //[HttpDelete]
        //public IActionResult Delete()
        //{
        //    var routeInfo = requestProcessor.GetRoutInfo(RouteData, Request);

        //    if (routeInfo.Entity == null || routeInfo.Id == null)
        //    {
        //        return NotFound();
        //    }

        //    if (!requestProcessor.Authorize(User, routeInfo.Entity.DELETEPolicy, authorizationService))
        //    {
        //        return Unauthorized();
        //    }

        //    object entity = ((dynamic)routeInfo.Entity.DbSet.GetValue(context)).Find(Convert.ChangeType(routeInfo.Id, routeInfo.Entity.Id.PropertyType));

        //    if (entity == null)
        //    {
        //        return NotFound();
        //    }

        //    context.Remove(entity);
        //    context.SaveChanges();

        //    return Ok();
        //}
    }
}

