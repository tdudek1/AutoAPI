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
        private static readonly Object obj = new Object();

        public AutoAPISwaggerDocumentFilterTests()
		{
            lock (obj)
            {
                if (APIConfiguration.AutoAPIEntityCache.Count == 0)
                {
                    APIConfiguration.AutoAPIEntityCache.AddRange(APIConfiguration.Init<DataContext>("/api/data"));
                }
            }
		}

		[Fact]
		public void Apply_WhenEntity_AddDefinitions()
		{
            var swaggerDoc = new SwaggerDocument() { Definitions = new Dictionary<string, Schema>(), Paths = new Dictionary<string, PathItem>() };
			var filter = new AutoAPISwaggerDocumentFilter();

			filter.Apply(swaggerDoc, null);

			Assert.Equal(2, swaggerDoc.Definitions.Count);
			Assert.Equal("author", swaggerDoc.Definitions.First().Key);
		}

		[Fact]
		public void Apply_WhenEntityExists_DontAddDefinitions()
		{
            var swaggerDoc = new SwaggerDocument() { Definitions = new Dictionary<string, Schema>(), Paths = new Dictionary<string, PathItem>() };
			var filter = new AutoAPISwaggerDocumentFilter();

			filter.Apply(swaggerDoc, null);

			Assert.Equal(2, swaggerDoc.Definitions.Count);
			Assert.Equal("book", swaggerDoc.Definitions.Keys.ToList()[1]);
		}

		[Fact]
		public void Apply_WhenEntity_AddPaths()
		{
            var swaggerDoc = new SwaggerDocument() { Definitions = new Dictionary<string, Schema>(), Paths = new Dictionary<string, PathItem>() };
			var filter = new AutoAPISwaggerDocumentFilter();

			filter.Apply(swaggerDoc, null);

			Assert.Equal(4, swaggerDoc.Paths.Count);

		}
	}
}
