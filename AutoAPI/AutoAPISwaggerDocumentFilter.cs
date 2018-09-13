using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace AutoAPI
{
	public class AutoAPISwaggerDocumentFilter : IDocumentFilter
	{
		private List<string> basePaths;

		public AutoAPISwaggerDocumentFilter(List<string> basePaths)
		{
			this.basePaths = basePaths;
		}

		public void Apply(SwaggerDocument swaggerDoc, DocumentFilterContext context)
		{
			foreach (var basePath in basePaths)
			{
				foreach (var entity in APIConfiguration.AutoAPIEntityCache)
				{
					var idschema = SchemaTypeMap[entity.Id.PropertyType]();

					//get,post
					swaggerDoc.Paths.Add($"{basePath}{entity.Route.ToLower()}", new PathItem
					{
						Get = new Operation
						{
							OperationId = $"get{basePath}{entity.Route.ToLower()}",
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
							Tags = new List<string>() { $"{entity.EntityType.Name} - {basePath}" }
						},
						Post = new Operation
						{
							OperationId = $"post{basePath}{entity.Route.ToLower()}",
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
							Tags = new List<string>() { $"{entity.EntityType.Name} - {basePath}" }
						}
					});

					//get,put,delete by id
					swaggerDoc.Paths.Add($"{basePath}{entity.Route.ToLower()}/{{id}}", new PathItem
					{
						Get = new Operation
						{
							OperationId = $"get{basePath}{entity.Route.ToLower()}byid",
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
							Tags = new List<string>() { $"{entity.EntityType.Name} - {basePath}" }
						},

						Put = new Operation
						{
							OperationId = $"put{basePath}{entity.Route.ToLower()}",
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
							Tags = new List<string>() { $"{entity.EntityType.Name} - {basePath}" }
						},

						Delete = new Operation
						{
							OperationId = $"delete{basePath}{entity.Route.ToLower()}",
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
							Tags = new List<string>() { $"{entity.EntityType.Name} - {basePath}" }
						}

					});

				}
			}

			foreach (var entity in APIConfiguration.AutoAPIEntityCache)
			{
				swaggerDoc.Definitions.Add(entity.EntityType.Name.ToLower(), new Schema()
				{
					Type = "object",
					Properties = new Dictionary<string, Schema>()

				});

				foreach (var prop in entity.Properties)
				{
					swaggerDoc.Definitions.Last().Value.Properties.Add(prop.Name, SchemaTypeMap[prop.PropertyType]());
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
