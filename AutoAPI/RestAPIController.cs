﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace AutoAPI
{

    public class RESTAPIController : IRestAPIController
    {
        private readonly DbContext dbContext;
        private readonly ActionContext actionContext;
        private readonly IObjectModelValidator objectModelValidator;
        private const string KeylessError = "Operation not allowed for keyless entities";

        public RESTAPIController(DbContext dbContext, ActionContext actionContext, IObjectModelValidator objectModelValidator)
        {
            this.dbContext = dbContext;
            this.actionContext = actionContext;
            this.objectModelValidator = objectModelValidator;
        }

        public ObjectResult Get(RouteInfo routeInfo)
        {
            if (routeInfo.Id != null)
            {
                if (routeInfo.Entity.Id == null)
                    throw new Exception(KeylessError);

                var result = ((dynamic)routeInfo.Entity.DbSet.GetValue(dbContext)).Find(TypeConverter.ChangeType(routeInfo.Id, routeInfo.Entity.Id.PropertyType));
                if (result != null)
                {
                    return GetOkObjectResult(result);
                }
                else
                {
                    return new NotFoundObjectResult(null);
                }
            }
            else if (routeInfo.HasModifiers)
            {
                IQueryable dbSet = ((IQueryable)routeInfo.Entity.DbSet.GetValue(dbContext));

                foreach (var include in routeInfo.IncludeExpression)
                {
                    dbSet = (dynamic)EntityFrameworkQueryableExtensions.Include((dynamic)dbSet, include);
                }

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

                if (routeInfo.IsCount)
                {
                    return GetOkObjectResult(dbSet.Count());
                }

                if (routeInfo.IsPageResult)
                {
                    return GetOkObjectResult(GetPagedResult(dbSet, (IQueryable)routeInfo.Entity.DbSet.GetValue(dbContext), routeInfo.Page, routeInfo.Take));
                }

                return GetOkObjectResult(dbSet.ToDynamicList());
            }
            else
            {
                if (routeInfo.IsCount)
                {
                    return GetOkObjectResult(((IQueryable)routeInfo.Entity.DbSet.GetValue(dbContext)).Count());
                }
                else if (routeInfo.IsPageResult)
                {
                    return GetOkObjectResult(GetPagedResult((IQueryable)routeInfo.Entity.DbSet.GetValue(dbContext), (IQueryable)routeInfo.Entity.DbSet.GetValue(dbContext), 1, 0));
                }
                else
                {
                    return GetOkObjectResult(routeInfo.Entity.DbSet.GetValue(dbContext));
                }
            }
        }

        public ObjectResult Post(RouteInfo routeInfo, object entity)
        {
            if (routeInfo.Entity.Id == null)
                throw new Exception(KeylessError);

            if (!IsValid(entity))
            {
                return new BadRequestObjectResult(this.actionContext.ModelState);
            }

            dbContext.Add(entity);
            dbContext.SaveChanges();

            return new CreatedResult(routeInfo.Entity.Route, entity);
        }

        public ObjectResult Put(RouteInfo routeInfo, object entity)
        {
            if (routeInfo.Entity.Id == null)
                throw new Exception(KeylessError);

            var objectId = routeInfo.Entity.Id.GetValue(entity);
            var routeId = TypeConverter.ChangeType(routeInfo.Id, routeInfo.Entity.Id.PropertyType);

            if (!objectId.Equals(routeId))
            {
                return new BadRequestObjectResult(null);
            }

            if (!IsValid(entity))
            {
                return new BadRequestObjectResult(this.actionContext.ModelState);
            }

            dbContext.Entry(entity).State = EntityState.Modified;
            dbContext.SaveChanges();

            return new OkObjectResult(entity);
        }

        public ObjectResult Delete(RouteInfo routeInfo)
        {
            if (routeInfo.Entity.Id == null)
                throw new Exception(KeylessError);

            object entity = ((dynamic)routeInfo.Entity.DbSet.GetValue(dbContext)).Find(TypeConverter.ChangeType(routeInfo.Id, routeInfo.Entity.Id.PropertyType));

            if (entity == null)
            {
                return new NotFoundObjectResult(null);
            }

            dbContext.Remove(entity);
            dbContext.SaveChanges();

            return new OkObjectResult("");
        }

        protected virtual bool IsValid(object entity)
        {
            objectModelValidator.Validate(this.actionContext, null, "", entity);
            return this.actionContext.ModelState.IsValid;
        }

        private PagedResult GetPagedResult(IQueryable dbSet, IQueryable totalDbSet, int page, int pageSize)
        {
            var total = totalDbSet.Count();

            return new PagedResult()
            {
                Items = dbSet.ToDynamicList(),
                Page = page,
                PageSize = pageSize == 0 ? total : pageSize,
                PageCount = pageSize == 0 ? 1 : (int)Math.Ceiling((decimal)total / (decimal)pageSize),
                Total = total
            };
        }

        private OkObjectResult GetOkObjectResult(object result)
        {
            if (APIConfiguration.AutoAPIOptions.JsonSerializerOptions != null)
            {
                var objectResult = new OkObjectResult(result);
                objectResult.Formatters.Add(new SystemTextJsonOutputFormatter(APIConfiguration.AutoAPIOptions.JsonSerializerOptions));

                return objectResult;
            }
            else
            {
                return new OkObjectResult(result);
            }
        }
    }
}

