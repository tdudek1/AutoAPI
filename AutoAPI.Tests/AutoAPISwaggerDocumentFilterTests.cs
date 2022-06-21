using AutoAPI.Web;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Swagger;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace AutoAPI.Tests
{
    public class AutoAPISwaggerDocumentFilterTests
    {

        public AutoAPISwaggerDocumentFilterTests()
        {
            lock (APIConfiguration.AutoAPIEntityCache)
            {
                if (APIConfiguration.AutoAPIEntityCache.Count == 0)
                {
                    APIConfiguration.AutoAPIEntityCache.AddRange(APIConfiguration.Init<DataContext>(new AutoAPIOptions() { Path ="/api/data"}));
                }
            }
        }

        [Fact]
        public void Apply_WhenEntity_AddDefinitions()
        {
            var swaggerDoc = new OpenApiDocument();
            swaggerDoc.Paths = new OpenApiPaths();
            swaggerDoc.Components = new OpenApiComponents() { Schemas = new Dictionary<string, OpenApiSchema>() };
            var filter = new AutoAPISwaggerDocumentFilter();


            filter.Apply(swaggerDoc, null);

            Assert.Equal(4, swaggerDoc.Components.Schemas.Count);
            Assert.Equal("author", swaggerDoc.Components.Schemas.First().Key);
        }

        [Fact]
        public void Apply_WhenEntityExists_DontAddDefinitions()
        {
            var swaggerDoc = new OpenApiDocument();
            swaggerDoc.Paths = new OpenApiPaths();
            swaggerDoc.Components = new OpenApiComponents() { Schemas = new Dictionary<string, OpenApiSchema>() };
            var filter = new AutoAPISwaggerDocumentFilter();

            filter.Apply(swaggerDoc, null);

            Assert.Equal(4, swaggerDoc.Components.Schemas.Count);
            Assert.Equal("book", swaggerDoc.Components.Schemas.Keys.ToList()[1]);
        }

        [Fact]
        public void Apply_WhenEntity_AddPaths()
        {
            var swaggerDoc = new OpenApiDocument();
            swaggerDoc.Paths = new OpenApiPaths();
            swaggerDoc.Components = new OpenApiComponents() { Schemas = new Dictionary<string, OpenApiSchema>() };
            var filter = new AutoAPISwaggerDocumentFilter();

            filter.Apply(swaggerDoc, null);

            Assert.Equal(3, APIConfiguration.AutoAPIEntityCache.Count);
            Assert.Equal(10, swaggerDoc.Paths.Count);

        }
    }
}
