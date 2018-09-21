using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
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
    public class AutoAPIController : IAutoAPIController
    {
        private readonly DbContext dbContext;
        private readonly ActionContext actionContext;
        private readonly IObjectModelValidator objectModelValidator;
        
        public AutoAPIController(DbContext dbContext,ActionContext actionContext,IObjectModelValidator objectModelValidator)
        {
            this.dbContext = dbContext;
            this.actionContext = actionContext;
            this.objectModelValidator = objectModelValidator;
        }

        public ObjectResult Get(RouteInfo routeInfo)
        {
            if (routeInfo.Id != null)
            {
                var result = ((dynamic)routeInfo.Entity.DbSet.GetValue(dbContext)).Find(Convert.ChangeType(routeInfo.Id, routeInfo.Entity.Id.PropertyType));
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
                IQueryable dbSet = ((IQueryable)routeInfo.Entity.DbSet.GetValue(dbContext));

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
                return new OkObjectResult(routeInfo.Entity.DbSet.GetValue(dbContext));
            }
        }

		public ObjectResult Post(RouteInfo routeInfo, object entity)
		{
            objectModelValidator.Validate(this.actionContext, null, "", entity);
            if (!this.actionContext.ModelState.IsValid)
            {
                return new BadRequestObjectResult(this.actionContext.ModelState);
            }

			dbContext.Add(entity);
			dbContext.SaveChanges();

			return new  CreatedResult(routeInfo.Entity.Route, entity);
		}

		public ObjectResult Put(RouteInfo routeInfo,object entity)
		{
			
			var objectId = Convert.ChangeType(routeInfo.Entity.Id.GetValue(entity), routeInfo.Entity.Id.PropertyType);
			var routeId = Convert.ChangeType(routeInfo.Id, routeInfo.Entity.Id.PropertyType);

			if (!objectId.Equals(routeId))
			{
				return new BadRequestObjectResult(null);
			}

            objectModelValidator.Validate(this.actionContext, null, "", entity);
            if (!this.actionContext.ModelState.IsValid)
            {
                return new BadRequestObjectResult(this.actionContext.ModelState);
            }

            dbContext.Entry(entity).State = EntityState.Modified;
			dbContext.SaveChanges();

			return new OkObjectResult(entity);
		}

		public ObjectResult Delete(RouteInfo routeInfo)
		{
			object entity = ((dynamic)routeInfo.Entity.DbSet.GetValue(dbContext)).Find(Convert.ChangeType(routeInfo.Id, routeInfo.Entity.Id.PropertyType));

			if (entity == null)
			{
				return new NotFoundObjectResult(null);
			}

			dbContext.Remove(entity);
			dbContext.SaveChanges();

			return new OkObjectResult(null);
		}
	}
}

