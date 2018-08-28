using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Swashbuckle.AspNetCore.Swagger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Reflection;

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
            if (RouteData != null && RouteData.Values["query"]?.ToString() == "swagger.json")
            {
                return GetSwaggerDocument();
            }

            var routeInfo = requestProcessor.GetRoutInfo(RouteData, Request);

            if (routeInfo.Entity == null)
            {
                return NotFound();
            }

            if (!requestProcessor.Authorize(User, routeInfo.Entity.GETPolicy, authorizationService))
            {
                return Unauthorized();
            }

            if (routeInfo.Id != null)
            {
                var result = ((dynamic)routeInfo.Entity.DbSet.GetValue(context)).Find(Convert.ChangeType(routeInfo.Id, routeInfo.Entity.Id.PropertyType));
                if (result != null)
                {
                    return Ok(result);
                }
                else
                {
                    return NotFound();
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
            var routeInfo = requestProcessor.GetRoutInfo(RouteData, Request);

            if (routeInfo.Entity == null)
            {
                return NotFound();
            }

            if (!requestProcessor.Authorize(User, routeInfo.Entity.POSTtPolicy, authorizationService))
            {
                return Unauthorized();
            }

            var entity = requestProcessor.GetData(Request, routeInfo.Entity.EntityType);

            if (!requestProcessor.Validate(this, entity))
            {
                return BadRequest(ModelState);
            }

            context.Add(entity);
            context.SaveChanges();

            return Created(routeInfo.Entity.Route, entity);
        }

        [HttpPut]
        public IActionResult Put()
        {
            var routeInfo = requestProcessor.GetRoutInfo(RouteData, Request);

            if (routeInfo.Entity == null || routeInfo.Id == null)
            {
                return NotFound();
            }

            if (!requestProcessor.Authorize(User, routeInfo.Entity.PUTPolicy, authorizationService))
            {
                return Unauthorized();
            }

            var entity = requestProcessor.GetData(Request, routeInfo.Entity.EntityType);
            var objectId = Convert.ChangeType(routeInfo.Entity.Id.GetValue(entity), routeInfo.Entity.Id.PropertyType);
            var routeId = Convert.ChangeType(routeInfo.Id, routeInfo.Entity.Id.PropertyType);

            if (!requestProcessor.Validate(this, entity))
            {
                return BadRequest(ModelState);
            }

            if (!objectId.Equals(routeId))
            {
                return BadRequest();
            }

            context.Entry(entity).State = EntityState.Modified;
            context.SaveChanges();

            return Ok(entity);
        }

        [HttpDelete]
        public IActionResult Delete()
        {
            var routeInfo = requestProcessor.GetRoutInfo(RouteData, Request);

            if (routeInfo.Entity == null || routeInfo.Id == null)
            {
                return NotFound();
            }

            if (!requestProcessor.Authorize(User, routeInfo.Entity.DELETEPolicy, authorizationService))
            {
                return Unauthorized();
            }

            object entity = ((dynamic)routeInfo.Entity.DbSet.GetValue(context)).Find(Convert.ChangeType(routeInfo.Id, routeInfo.Entity.Id.PropertyType));

            if (entity == null)
            {
                return NotFound();
            }

            context.Remove(entity);
            context.SaveChanges();

            return Ok();
        }


        public JsonResult GetSwaggerDocument()
        {
            var doc = new SwaggerDocument();

            doc.Info = new Info() { Version = "v1", Description = "AutoAPI" };
            doc.Paths = new Dictionary<string, PathItem>();
            doc.Definitions = new Dictionary<string, Schema>();
            
            foreach (var entity in APIConfiguration.AutoAPIEntityCache)
            {

                doc.Paths.Add($"{this.Request.Path.Value.Replace("swagger.json", "")}{entity.Route.ToLower()}", new PathItem
                {
                    Get = new Operation
                    {
                        OperationId = $"get{entity.Route.ToLower()}",
                        Produces = new List<string>() { "application/json" },
                        Responses = new Dictionary<string, Response>()
                        {
                            {"200", new Response()
                            {
                                Description = "Success",
                                Schema = new Schema()
                                {
                                    Type="array",
                                    Items = new Schema()
                                    {
                                        Ref = $"#/definitions/{entity.EntityType.Name.ToLower()}"
                                    }
                                }
                            } }
                        }

                    }
                });

                doc.Paths.Add($"{this.Request.Path.Value.Replace("swagger.json", "")}{entity.Route.ToLower()}/{{id}}", new PathItem
                {
                    Get = new Operation
                    {
                        OperationId = $"get{entity.Route.ToLower()}",
                        Produces = new List<string>() { "application/json" },
                        Responses = new Dictionary<string, Response>()
                        {
                            {"200", new Response()
                            {
                                Description = "Success",
                                Schema = new Schema()
                                {
                                    Type="object",
                                    Ref = $"#/definitions/{entity.EntityType.Name.ToLower()}"
                                }
                            } }
                        }

                    }
                });

                doc.Definitions.Add(entity.EntityType.Name, new Schema()
                {
                    Type = "object",
                    Properties = new Dictionary<string, Schema>()

                });

                foreach (var prop in entity.Properties)
                {
                    doc.Definitions.Last().Value.Properties.Add(prop.Name, SchemaTypeMap[prop.PropertyType]());
                }
            }

            return new JsonResult(doc, new JsonSerializerSettings()
            {
                NullValueHandling = NullValueHandling.Ignore,
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            });
        }

        private static readonly Dictionary<Type, Func<Schema>> SchemaTypeMap = new Dictionary<Type, Func<Schema>>
        {
            { typeof(short), () => new Schema { Type = "integer", Format = "int32" } },
            { typeof(ushort), () => new Schema { Type = "integer", Format = "int32" } },
            { typeof(int), () => new Schema { Type = "integer", Format = "int32" } },
            { typeof(uint), () => new Schema { Type = "integer", Format = "int32" } },
            { typeof(long), () => new Schema { Type = "integer", Format = "int64" } },
            { typeof(ulong), () => new Schema { Type = "integer", Format = "int64" } },
            { typeof(float), () => new Schema { Type = "number", Format = "float" } },
            { typeof(double), () => new Schema { Type = "number", Format = "double" } },
            { typeof(decimal), () => new Schema { Type = "number", Format = "double" } },
            { typeof(byte), () => new Schema { Type = "integer", Format = "int32" } },
            { typeof(sbyte), () => new Schema { Type = "integer", Format = "int32" } },
            { typeof(byte[]), () => new Schema { Type = "string", Format = "byte" } },
            { typeof(sbyte[]), () => new Schema { Type = "string", Format = "byte" } },
            { typeof(bool), () => new Schema { Type = "boolean" } },
            { typeof(DateTime), () => new Schema { Type = "string", Format = "date-time" } },
            { typeof(DateTimeOffset), () => new Schema { Type = "string", Format = "date-time" } },
            { typeof(Guid), () => new Schema { Type = "string", Format = "uuid" } },
            { typeof(string), () => new Schema { Type = "string", Format = "string" } }
        };
    }
}

