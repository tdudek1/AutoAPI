using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AutoAPI
{
    public class AutoAPISwaggerDocumentFilter : IDocumentFilter
    {

        public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
        {
            try
            {
                foreach (var entity in APIConfiguration.AutoAPIEntityCache)
                {
                                   
                    //get
                    swaggerDoc.Paths.Add($"{entity.Route.ToLower()}", new OpenApiPathItem
                    {
                        Operations =
                        {
                            [OperationType.Get] = new OpenApiOperation
                            {
                                OperationId = GetOperationID($"{entity.Route.ToLower()}/get"),
                                Tags = new List<OpenApiTag>() { new OpenApiTag() { Name = entity.EntityType.Name} },
                                Description = $"Get all/filter items for {entity.EntityType.Name}",
                                Responses =
                                {
                                    ["200"] = new OpenApiResponse()
                                    {
                                        Description = "Success",
                                        Content =
                                        {
                                            ["application/json"] = new OpenApiMediaType()
                                            {
                                                Schema = new OpenApiSchema()
                                                {
                                                    Type = $"array",
                                                    Items  = new OpenApiSchema()
                                                    {
                                                        Reference = new OpenApiReference()
                                                        {
                                                            Type = ReferenceType.Schema,
                                                            Id = $"{entity.EntityType.Name.ToLower()}"
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    });

                    if (entity.Id == null)
                        continue;

                    //post
                    swaggerDoc.Paths[$"{entity.Route.ToLower()}"].Operations.Add(
                        OperationType.Post,
                        new OpenApiOperation
                        {
                            OperationId = GetOperationID($"{entity.Route.ToLower()}/post"),
                            Tags = new List<OpenApiTag>() { new OpenApiTag() { Name = entity.EntityType.Name } },
                            Description = $"Create {entity.EntityType.Name}",
                            Responses =
                            {
                                ["201"] = new OpenApiResponse()
                                {
                                    Description = "Success",
                                    Content =
                                    {
                                        ["application/json"] = new OpenApiMediaType()
                                        {
                                            Schema = new OpenApiSchema()
                                            {
                                                Reference = new OpenApiReference()
                                                {
                                                    Type = ReferenceType.Schema,
                                                    Id = $"{entity.EntityType.Name.ToLower()}"
                                                }
                                            }
                                        }
                                    }
                                }
                            },
                            RequestBody = new OpenApiRequestBody()
                            {
                                Content =
                                {
                                    ["application/json"] = new OpenApiMediaType()
                                    {
                                        Schema = new OpenApiSchema()
                                        {
                                            Reference = new OpenApiReference()
                                            {
                                                Type = ReferenceType.Schema,
                                                Id = $"{entity.EntityType.Name.ToLower()}"
                                            }
                                        }
                                    }
                                }
                            }
                        });



                    var idschema = SchemaTypeMap[entity.Id.PropertyType]();




                    //get,put,delete by id
                    swaggerDoc.Paths.Add($"{entity.Route.ToLower()}/{{id}}", new OpenApiPathItem
                    {
                        Operations =
                        {
                            [OperationType.Get] = new OpenApiOperation
                            {
                                OperationId = GetOperationID($"{entity.Route.ToLower()}/getById"),
                                Tags = new List<OpenApiTag>() { new OpenApiTag() { Name = entity.EntityType.Name} },
                                Description = $"Get {entity.EntityType.Name} by id",
                                Responses =
                                {
                                    ["200"] = new OpenApiResponse()
                                    {
                                        Description = "Success",
                                        Content =
                                        {
                                            ["application/json"] = new OpenApiMediaType()
                                            {
                                                Schema = new OpenApiSchema()
                                                {
                                                    Reference = new OpenApiReference()
                                                    {
                                                        Type = ReferenceType.Schema,
                                                        Id = $"{entity.EntityType.Name.ToLower()}"
                                                    }
                                                }
                                            }
                                        }
                                    }
                                },
                                Parameters = new List<OpenApiParameter>()
                                {
                                    new OpenApiParameter()
                                    {
                                        Name = "id",
                                        Schema =  SchemaTypeMap[entity.Id.PropertyType](),
                                        Required = true,
                                        In = ParameterLocation.Path
                                    }
                                }
                            },
                            [OperationType.Delete] = new OpenApiOperation
                            {
                                OperationId = GetOperationID($"{entity.Route.ToLower()}/deleteById"),
                                Tags = new List<OpenApiTag>() { new OpenApiTag() { Name = entity.EntityType.Name} },
                                Description = $"Delete {entity.EntityType.Name} by id",
                                Responses =
                                {
                                    ["200"] = new OpenApiResponse()
                                    {
                                        Description = "Success"
                                    }
                                },
                                Parameters = new List<OpenApiParameter>()
                                {
                                    new OpenApiParameter()
                                    {
                                        Name = "id",
                                        Schema =  SchemaTypeMap[entity.Id.PropertyType](),
                                        Required = true,
                                        In = ParameterLocation.Path
                                    }
                                }
                            },
                            [OperationType.Put] = new OpenApiOperation
                            {
                                OperationId = GetOperationID($"{entity.Route.ToLower()}/updateById"),
                                Tags = new List<OpenApiTag>() { new OpenApiTag() { Name = entity.EntityType.Name} },
                                Description = $"Update {entity.EntityType.Name} by id",
                                Responses =
                                {
                                    ["200"] = new OpenApiResponse()
                                    {
                                        Description = "Success",
                                        Content =
                                        {
                                            ["application/json"] = new OpenApiMediaType()
                                            {
                                                Schema = new OpenApiSchema()
                                                {
                                                    Reference = new OpenApiReference()
                                                    {
                                                        Type = ReferenceType.Schema,
                                                        Id = $"{entity.EntityType.Name.ToLower()}"
                                                    }
                                                }
                                            }
                                        }
                                    }
                                },
                                Parameters = new List<OpenApiParameter>()
                                {
                                    new OpenApiParameter()
                                    {
                                        Name = "id",
                                        Schema =  SchemaTypeMap[entity.Id.PropertyType](),
                                        Required = true,
                                        In = ParameterLocation.Path
                                    }
                                },
                                RequestBody = new OpenApiRequestBody()
                                {
                                    Content =
                                    {
                                        ["application/json"] = new OpenApiMediaType()
                                        {
                                            Schema = new OpenApiSchema()
                                            {
                                                Reference = new OpenApiReference()
                                                {
                                                    Type = ReferenceType.Schema,
                                                    Id = $"{entity.EntityType.Name.ToLower()}"
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    });

                    //count
                    swaggerDoc.Paths.Add($"{entity.Route.ToLower()}/count", new OpenApiPathItem
                    {
                        Operations =
                        {
                            [OperationType.Get] = new OpenApiOperation
                            {
                                OperationId = GetOperationID($"{entity.Route.ToLower()}/count"),
                                Tags = new List<OpenApiTag>() { new OpenApiTag() { Name = entity.EntityType.Name} },
                                Description = $"Get count of items for {entity.EntityType.Name}",
                                Responses =
                                {
                                    ["200"] = new OpenApiResponse()
                                    {
                                        Description = "Success",
                                        Content =
                                        {
                                            ["application/json"] = new OpenApiMediaType()
                                            {
                                                Schema = SchemaTypeMap[typeof(int)]()
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    });

                    //paged result
                    if (entity.ExposePagedResult)
                    {
                        swaggerDoc.Paths.Add($"{entity.Route.ToLower()}/pagedresult", new OpenApiPathItem
                        {
                            Operations =
                        {
                            [OperationType.Get] = new OpenApiOperation
                            {
                                OperationId = GetOperationID($"{entity.Route.ToLower()}/pagedresult"),
                                Tags = new List<OpenApiTag>() { new OpenApiTag() { Name = entity.EntityType.Name} },
                                Description = $"Get paged result of items for {entity.EntityType.Name}",
                                Responses =
                                {
                                    ["200"] = new OpenApiResponse()
                                    {
                                        Description = "Success",
                                        Content =
                                        {
                                            ["application/json"] = new OpenApiMediaType()
                                            {
                                                Schema = new OpenApiSchema()
                                                {
                                                    Reference = new OpenApiReference()
                                                    {
                                                        Type = ReferenceType.Schema,
                                                        Id = "pagedresult"
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        });
                    }

                }


                foreach (var entity in APIConfiguration.AutoAPIEntityCache)
                {
                    if (!swaggerDoc.Components.Schemas.ContainsKey(entity.EntityType.Name.ToLower()))
                    {
                        swaggerDoc.Components.Schemas.Add(entity.EntityType.Name.ToLower(), new OpenApiSchema()
                        {
                            Type = "object",
                            Properties = new Dictionary<string, OpenApiSchema>()

                        });

                        foreach (var prop in entity.Properties)
                        {
                            swaggerDoc.Components.Schemas.Last().Value.Properties.Add(prop.Name, SchemaTypeMap[Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType]());
                        }
                    }
                }

                if (APIConfiguration.AutoAPIEntityCache.Any(x => x.ExposePagedResult) && !swaggerDoc.Components.Schemas.ContainsKey("pagedresult"))
                {
                    swaggerDoc.Components.Schemas.Add("pagedresult", new OpenApiSchema()
                    {
                        Type = "object",
                        Properties = new Dictionary<string, OpenApiSchema>()
                    });

                    foreach (var prop in typeof(PagedResult).GetProperties())
                    {
                        if (!prop.PropertyType.IsGenericType)
                        {
                            swaggerDoc.Components.Schemas.Last().Value.Properties.Add(prop.Name, SchemaTypeMap[Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType]());
                        }
                        else
                        {
                            swaggerDoc.Components.Schemas.Last().Value.Properties.Add(prop.Name, new OpenApiSchema()
                            {
                                Type = "array",
                                Items = new OpenApiSchema()
                                {
                                    Type = "object"
                                }
                            });
                        }
                    }

                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static string GetOperationID(string path)
        {
            return string.Join("", path.Split("/", StringSplitOptions.RemoveEmptyEntries).Select(x => x.First().ToString().ToUpper() + x.Substring(1)));
        }

        private static readonly Dictionary<Type, Func<OpenApiSchema>> SchemaTypeMap = new Dictionary<Type, Func<OpenApiSchema>>
        {
            [typeof(string)] = () => new OpenApiSchema { Type = "string", Format = "string" },
            [typeof(DateTime)] = () => new OpenApiSchema { Type = "string", Format = "date-time" },
            [typeof(DateTimeOffset)] = () => new OpenApiSchema { Type = "string", Format = "date-time" },
            [typeof(Guid)] = () => new OpenApiSchema { Type = "string", Format = "uuid" },
            [typeof(short)] = () => new OpenApiSchema { Type = "integer", Format = "int32" },
            [typeof(ushort)] = () => new OpenApiSchema { Type = "integer", Format = "int32" },
            [typeof(int)] = () => new OpenApiSchema { Type = "integer", Format = "int32" },
            [typeof(uint)] = () => new OpenApiSchema { Type = "integer", Format = "int32" },
            [typeof(long)] = () => new OpenApiSchema { Type = "integer", Format = "int64" },
            [typeof(ulong)] = () => new OpenApiSchema { Type = "integer", Format = "int64" },
            [typeof(float)] = () => new OpenApiSchema { Type = "number", Format = "float" },
            [typeof(double)] = () => new OpenApiSchema { Type = "number", Format = "double" },
            [typeof(decimal)] = () => new OpenApiSchema { Type = "number", Format = "double" },
            [typeof(byte)] = () => new OpenApiSchema { Type = "integer", Format = "int32" },
            [typeof(sbyte)] = () => new OpenApiSchema { Type = "integer", Format = "int32" },
            [typeof(byte[])] = () => new OpenApiSchema { Type = "string", Format = "byte" },
            [typeof(sbyte[])] = () => new OpenApiSchema { Type = "string", Format = "byte" },
            [typeof(bool)] = () => new OpenApiSchema { Type = "boolean" }
        };
    }
}
