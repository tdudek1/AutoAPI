using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AutoAPI
{
    public class AutoAPISwaggerDocumentFilter : IDocumentFilter
    {

        public void Apply(SwaggerDocument swaggerDoc, DocumentFilterContext context)
        {
            foreach (var entity in APIConfiguration.AutoAPIEntityCache)
            {
                var idschema = SchemaTypeMap[entity.Id.PropertyType]();
                var tag = entity.EntityType.Name;
                //get,post
                swaggerDoc.Paths.Add($"{entity.Route.ToLower()}", new PathItem
                {
                    Get = new Operation
                    {
                        OperationId = $"{entity.Route.ToLower()}/get".ToOperationID(),
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
                            },
                        Tags = new List<string>() { tag }
                    },
                    Post = new Operation
                    {
                        OperationId = $"{entity.Route.ToLower()}/post".ToOperationID(),
                        Consumes = new List<string>() { "application/json" },
                        Parameters = new List<IParameter>()
                            {
                                new BodyParameter()
                                {
                                    Name = entity.EntityType.Name.ToLower(),
                                    Schema = new Schema()
                                    {
                                        Type="object",
                                        Ref = $"#/definitions/{entity.EntityType.Name.ToLower()}"
                                    }
                                }
                            },
                        Produces = new List<string>() { "application/json" },
                        Responses = new Dictionary<string, Response>()
                            {
                                {"201", new Response()
                                        {
                                            Description = "Created",
                                            Schema = new Schema()
                                            {
                                                Type="object",
                                                Ref = $"#/definitions/{entity.EntityType.Name.ToLower()}"
                                            }
                                        }
                                }
                            },
                        Tags = new List<string>() { tag }
                    }
                });

                //get,put,delete by id
                swaggerDoc.Paths.Add($"{entity.Route.ToLower()}/{{id}}", new PathItem
                {
                    Get = new Operation
                    {
                        OperationId = $"{entity.Route.ToLower()}/getById".ToOperationID(),
                        Parameters = new List<IParameter>()
                            {
                                new NonBodyParameter()
                                {
                                    Name = "id",
                                    In = "path",
                                    Type = idschema.Type,
                                    Format = idschema.Format,
                                    Required = true
                                }
                            },
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
                            },
                        Tags = new List<string>() { tag }
                    },

                    Put = new Operation
                    {
                        OperationId = $"{entity.Route.ToLower()}/putById".ToOperationID(),
                        Consumes = new List<string>() { "application/json" },
                        Parameters = new List<IParameter>()
                            {
                                new NonBodyParameter()
                                {
                                    Name = "id",
                                    In = "path",
                                    Type = idschema.Type,
                                    Format = idschema.Format,
                                    Required = true
                                },
                                new BodyParameter()
                                {
                                    Name = entity.EntityType.Name.ToLower(),
                                    Schema = new Schema()
                                    {
                                        Type="object",
                                        Ref = $"#/definitions/{entity.EntityType.Name.ToLower()}"
                                    }
                                }
                            },
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
                                        }
                                }
                            },
                        Tags = new List<string>() { tag }
                    },

                    Delete = new Operation
                    {
                        OperationId = $"{entity.Route.ToLower()}/deleteById".ToOperationID(),
                        Consumes = new List<string>() { "application/json" },
                        Parameters = new List<IParameter>()
                            {
                                new NonBodyParameter()
                                {
                                    Name = "id",
                                    In = "path",
                                    Type = idschema.Type,
                                    Format = idschema.Format,
                                    Required = true
                                }
                            },
                        Produces = new List<string>() { "application/json" },
                        Responses = new Dictionary<string, Response>()
                            {
                                {"200", new Response()
                                        {
                                            Description = "Success"
                                        }
                                }
                            },
                        Tags = new List<string>() { tag }
                    }

                });

                swaggerDoc.Paths.Add($"{entity.Route.ToLower()}/count", new PathItem()
                {
                    Get = new Operation
                    {
                        OperationId = $"{entity.Route.ToLower()}/count".ToOperationID(),
                        Produces = new List<string>() { "application/json" },
                        Responses = new Dictionary<string, Response>()
                            {
                                {"200", new Response()
                                {
                                    Description = "Success",
                                    Schema =SchemaTypeMap[typeof(int)]()
                                } }
                            },
                        Tags = new List<string>() { tag }
                    }
                });

                if (entity.ExposePagedResult)
                {
                    swaggerDoc.Paths.Add($"{entity.Route.ToLower()}/pagedresult", new PathItem()
                    {
                        Get = new Operation
                        {
                            OperationId = $"{entity.Route.ToLower()}/pagedresult".ToOperationID(),
                            Produces = new List<string>() { "application/json" },
                            Responses = new Dictionary<string, Response>()
                            {
                                {"200", new Response()
                                {
                                    Description = "Success",
                                    Schema = new Schema()
                                    {
                                        Type = "object",
                                        Ref =  $"#/definitions/pagedresult"
                                    }
                                } }
                            },
                            Tags = new List<string>() { tag }
                        }
                    });
                }
            }

            foreach (var entity in APIConfiguration.AutoAPIEntityCache)
            {
                if (!swaggerDoc.Definitions.ContainsKey(entity.EntityType.Name.ToLower()))
                {
                    swaggerDoc.Definitions.Add(entity.EntityType.Name.ToLower(), new Schema()
                    {
                        Type = "object",
                        Properties = new Dictionary<string, Schema>()

                    });

                    foreach (var prop in entity.Properties)
                    {
                        swaggerDoc.Definitions.Last().Value.Properties.Add(prop.Name, SchemaTypeMap[Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType]());
                    }
                }
            }

            if (APIConfiguration.AutoAPIEntityCache.Any(x=>x.ExposePagedResult) && !swaggerDoc.Definitions.ContainsKey("pagedresult") )
            {
                swaggerDoc.Definitions.Add("pagedresult", new Schema()
                {
                    Type = "object",
                    Properties = new Dictionary<string, Schema>()
                });

                foreach (var prop in typeof(PagedResult).GetProperties())
                {
                    if (!prop.PropertyType.IsGenericType)
                    {
                        swaggerDoc.Definitions.Last().Value.Properties.Add(prop.Name, SchemaTypeMap[Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType]());
                    }
                    else
                    {
                        swaggerDoc.Definitions.Last().Value.Properties.Add(prop.Name, new Schema()
                        {
                            Type = "array",
                            Items = new Schema()
                            {
                                Type = "object"
                            }
                        });
                    }
                }

            }
        }

        private static readonly Dictionary<Type, Func<Schema>> SchemaTypeMap = new Dictionary<Type, Func<Schema>>
        {
            { typeof(string), () => new Schema { Type = "string", Format = "string" } },
            { typeof(DateTime), () => new Schema { Type = "string", Format = "date-time" } },
            { typeof(DateTimeOffset), () => new Schema { Type = "string", Format = "date-time" } },
            { typeof(Guid), () => new Schema { Type = "string", Format = "uuid" } },
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
            { typeof(bool), () => new Schema { Type = "boolean" } }
        };
    }
}
