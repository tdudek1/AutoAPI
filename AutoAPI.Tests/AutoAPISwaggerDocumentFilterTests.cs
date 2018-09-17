using AutoAPI.Web;
using Swashbuckle.AspNetCore.Swagger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace AutoAPI.Tests
{
    public class AutoAPISwaggerDocumentFilterTests
    {
        [Fact]
        public void Apply_WhenEntity_AddDefinitions()
        {
            APIConfiguration.AutoAPIEntityCache =  APIConfiguration.Init<DataContext>();
            var swaggerDoc = new SwaggerDocument() { Definitions = new Dictionary<string, Schema>() };
            var filter = new AutoAPISwaggerDocumentFilter(new List<string>());

            filter.Apply(swaggerDoc, null);

            Assert.Equal(2, swaggerDoc.Definitions.Count);
            Assert.Equal("author", swaggerDoc.Definitions.First().Key);
        }

        [Fact]
        public void Apply_WhenEntityExists_DontAddDefinitions()
        {
            APIConfiguration.AutoAPIEntityCache = APIConfiguration.Init<DataContext>();
            var swaggerDoc = new SwaggerDocument() { Definitions = new Dictionary<string, Schema>() { { "author", new Schema() } } };
            var filter = new AutoAPISwaggerDocumentFilter(new List<string>());

            filter.Apply(swaggerDoc, null);

            Assert.Equal(2, swaggerDoc.Definitions.Count);
            Assert.Equal("book", swaggerDoc.Definitions.Keys.ToList()[1]);
        }
    }
}
