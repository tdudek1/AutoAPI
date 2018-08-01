using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace AutoAPI.Tests
{
    public class RequestProcessorTest
    {
        private readonly List<APIEntity> entityList;

        public RequestProcessorTest()
        {
            entityList = APIConfiguration.Init<TestContext>();
        }

        [Fact]
        public void BuildFilter_WhenOneProperty_ThenFilter()
        {
            //arrange
            var queryString = new Dictionary<string, StringValues>();
            queryString.Add("filter[Name]", "Ernest Hemingway");

            //act
            var result = (new RequestProcessor()).BuildFilter(entityList.Where(x => x.Route == "authors").First(), new QueryCollection(queryString));

            Assert.Equal("Name = @0", result.Item1);
            Assert.Single(result.Item2);
            Assert.Equal("Ernest Hemingway", (string)result.Item2[0]);

        }
    }
}
