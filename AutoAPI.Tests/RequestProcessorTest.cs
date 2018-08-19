using AutoAPI.Web;
using AutoAPI.Web.Entity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Primitives;
using Moq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace AutoAPI.Tests
{
    public class RequestProcessorTest
    {
        private readonly List<APIEntity> entityList;

        public RequestProcessorTest()
        {
            APIConfiguration.AutoAPIEntityCache.AddRange(APIConfiguration.Init<DataContext>());
        }

        [Fact]
        public void Authorize_WhenNoPolicyORAuthService_ThenReturnTrue()
        {
            //arrange
            var user = new ClaimsPrincipal();

            //act
            var result = (new RequestProcessor()).Authorize(user, null, null);

            //assert
            Assert.True(result);
        }

        [Fact]
        public void Authorize_WhenNoAuthService_ThenReturnTrue()
        {
            //arrange
            var user = new ClaimsPrincipal();

            //act
            var result = (new RequestProcessor()).Authorize(user, "Admin", null);

            //assert
            Assert.True(result);
        }

        [Fact]
        public void Authorize_WhenNoPolicy_ThenReturnTrue()
        {
            //arrange
            var user = new ClaimsPrincipal();
            var authServiceMock = new Mock<IAuthorizationService>();
            //act
            var result = (new RequestProcessor()).Authorize(user, null, authServiceMock.Object);

            //assert
            Assert.True(result);
        }

        [Fact]
        public void GetRequestInfo_WhenNoQuery_ThenEntityNull()
        {
            //arrange
            var routeData = new RouteData();

            //act
            var result = (new RequestProcessor()).GetRoutInfo(routeData, null);

            //assert
            Assert.Null(result.Entity);

        }

        [Fact]
        public void GetRequestInfo_WhenQueryAndEntityNotFound_ThenEntityNull()
        {
            //arrange
            var routeData = new RouteData();
            routeData.Values.Add("query", "notauthors");

            //act
            var result = (new RequestProcessor()).GetRoutInfo(routeData, null);

            //assert
            Assert.Null(result.Entity);

        }

        [Fact]
        public void GetRequestInfo_WhenQueryAndEntityFound_ThenEntityNull()
        {
            //arrange
            var routeData = new RouteData();
            routeData.Values.Add("query", "notauthors");

            //act
            var result = (new RequestProcessor()).GetRoutInfo(routeData, null);

            //assert
            Assert.Null(result.Entity);

        }

        [Fact]
        public void GetRequestInfo_WhenQueryAndEntity_ThenEntity()
        {
            //arrange
            var routeData = new RouteData();
            routeData.Values.Add("query", "authors");
            var httpRequestMock = new Mock<HttpRequest>();
            httpRequestMock.Setup(x => x.Query).Returns(new QueryCollection(new Dictionary<string, StringValues>()));


            //act
            var result = (new RequestProcessor()).GetRoutInfo(routeData, httpRequestMock.Object);

            //assert
            Assert.Equal("authors", result.Entity.Route.ToLower());

        }

        [Fact]
        public void GetRequestInfo_WhenQueryAndEntityAndQueryString_ThenEntity()
        {
            //arrange
            var routeData = new RouteData();
            routeData.Values.Add("query", "authors");
            var httpRequestMock = new Mock<HttpRequest>();
            var queryString = new Dictionary<string, StringValues>();
            queryString.Add("sort[Name]", "desc");
            queryString.Add("filter[Name]", "Ernest Hemingway");
            queryString.Add("pageSize", "10");
            queryString.Add("page", "2");


            httpRequestMock.Setup(x => x.Query).Returns(new QueryCollection(queryString));


            //act
            var result = (new RequestProcessor()).GetRoutInfo(routeData, httpRequestMock.Object);

            //assert
            Assert.Equal("authors", result.Entity.Route.ToLower());
            Assert.Equal("Name desc", result.SortExpression);
            Assert.Equal("Name == @0", result.FilterExpression);
            Assert.Equal(10, result.Skip);
            Assert.Equal(10, result.Take);
        }

        [Fact]
        public void GetData_WhenData_ThenEntity()
        {
            //arrange
            var input = @"{""id"": 1,""name"": ""Ernest Hemingway""}";
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(input);
            writer.Flush();
            stream.Position = 0;
            var httpRequestMock = new Mock<HttpRequest>();
            

            httpRequestMock.Setup(x => x.Body).Returns(stream);

            //act
            var result = (new RequestProcessor()).GetData(httpRequestMock.Object,typeof(Author));

            //assert
            Assert.Equal(1, ((Author)result).Id);
            Assert.Equal("Ernest Hemingway", ((Author)result).Name);
        }

    }
}
