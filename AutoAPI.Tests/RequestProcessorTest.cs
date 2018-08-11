using AutoAPI.Web;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.Extensions.Primitives;
using Moq;
using System;
using System.Collections.Generic;
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
            entityList = APIConfiguration.Init<DataContext>();
        }

        [Fact]
        public void BuildFilter_WhenOneProperty_ThenFilter()
        {
            //arrange
            var queryString = new Dictionary<string, StringValues>();
            queryString.Add("filter[Name]", "Ernest Hemingway");

            //act
            var result = (new RequestProcessor()).GetFilter(entityList.Where(x => x.Route == "authors").First(), new QueryCollection(queryString));

            //assert
            Assert.Equal("Name == @0", result.Expression);
            Assert.Single(result.Values);
            Assert.Equal("Ernest Hemingway", (string)result.Values[0]);

        }

        [Fact]
        public void BuildFilter_WhenTwoProperties_ThenFilter()
        {
            //arrange
            var queryString = new Dictionary<string, StringValues>();
            queryString.Add("filter[Name]", "Ernest Hemingway");
            queryString.Add("filter[DateOfBirth]", "7/21/1899");

            //act
            var result = (new RequestProcessor()).GetFilter(entityList.Where(x => x.Route == "authors").First(), new QueryCollection(queryString));

            //assert
            Assert.Equal("Name == @0 && DateOfBirth == @1", result.Expression);
            Assert.Equal(2, result.Values.Count());
            Assert.Equal("Ernest Hemingway", (string)result.Values[0]);
            Assert.Equal(new DateTime(1899, 7, 21), (DateTime)result.Values[1]);

        }


        [Fact]
        public void BuildFilter_NoFilters_ThenNull()
        {
            //arrange
            var queryString = new Dictionary<string, StringValues>();

            //act
            var result = (new RequestProcessor()).GetFilter(entityList.Where(x => x.Route == "authors").First(), new QueryCollection(queryString));

            Assert.Null(result.Expression);
        }

        [Fact]
        public void BuildSort_WhenTwoProperties_ThenFilter()
        {
            //arrange
            var queryString = new Dictionary<string, StringValues>();
            queryString.Add("sort[Name]", "desc");
            queryString.Add("sort[DateOfBirth]", "asc");

            //act
            var result = (new RequestProcessor()).GetSort(entityList.Where(x => x.Route == "authors").First(), new QueryCollection(queryString));

            //assert
            Assert.Equal("Name desc, DateOfBirth asc", result);

        }

        [Fact]
        public void BuildSort_NoSorts_ThenNull()
        {
            //arrange
            var queryString = new Dictionary<string, StringValues>();

            //act
            var result = (new RequestProcessor()).GetSort(entityList.Where(x => x.Route == "authors").First(), new QueryCollection(queryString));

            //assert
            Assert.Null(result);
        }

        [Fact]
        public void BuildPaging_NoPaging_ThenZerot()
        {
            //arrange
            var queryString = new Dictionary<string, StringValues>();

            //act
            var result = (new RequestProcessor()).GetPaging(new QueryCollection(queryString));

            Assert.Equal(0, result.Take);
        }

        [Fact]
        public void BuildPaging_PageSize10_ThenTakeTen()
        {
            //arrange
            var queryString = new Dictionary<string, StringValues>();
            queryString.Add("pageSize", "10");

            //act
            var result = (new RequestProcessor()).GetPaging(new QueryCollection(queryString));

            //assert
            Assert.Equal(10, result.Take);
            Assert.Equal(0, result.Skip);
        }

        [Fact]
        public void BuildPaging_PageSize10AndPage3_ThenTakeTenSkip3()
        {
            //arrange
            var queryString = new Dictionary<string, StringValues>();
            queryString.Add("pageSize", "10");
            queryString.Add("page", "3");

            //act
            var result = (new RequestProcessor()).GetPaging(new QueryCollection(queryString));

            //assert
            Assert.Equal(10, result.Take);
            Assert.Equal(20, result.Skip);
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
    }
}
