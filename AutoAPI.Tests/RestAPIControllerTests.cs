using AutoAPI.Web;
using AutoAPI.Web.Entity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using Moq.Protected;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace AutoAPI.Tests
{
    public class RestAPIControllerTests
    {
        private readonly List<APIEntity> entityList;

        public RestAPIControllerTests()
        {
            entityList = APIConfiguration.Init<DataContext>("/api");
        }

        [Fact]
        public void Get_WithNoID_ReturnAll()
        {
            //arrange
            var controller = new AutoAPI.RESTAPIController(SetupHelper.BuildTestContext(), null, null);
            var routinfo = new RouteInfo() { Entity = entityList.Where(x => x.Route == "/api/authors").First() };

            //act
            var result = (OkObjectResult)controller.Get(routinfo);

            //assert
            Assert.Equal(200, result.StatusCode.Value);
            Assert.Equal(2, ((IQueryable<Author>)result.Value).Count());
            Assert.Equal(1, ((IQueryable<Author>)result.Value).First().Id);
            Assert.Equal("Ernest Hemingway", ((IQueryable<Author>)result.Value).First().Name);

        }

        [Fact]
        public void Get_WithId_ReturnOne()
        {
            //arrange
            var controller = new AutoAPI.RESTAPIController(SetupHelper.BuildTestContext(), null, null);
            var routeInfo = new RouteInfo() { Entity = entityList.Where(x => x.Route == "/api/authors").First(), Id = "1" };

            //act
            var result = (OkObjectResult)controller.Get(routeInfo);

            //assert
            Assert.Equal(200, result.StatusCode.Value);
            Assert.Equal(1, ((Author)result.Value).Id);
            Assert.Equal("Ernest Hemingway", ((Author)result.Value).Name);

        }

        [Fact]
        public void Get_WithFilterPagingAndSort_ReturnOne()
        {
            //arrange
            var controller = new AutoAPI.RESTAPIController(SetupHelper.BuildTestContext(), null, null);
            var routeInfo = new RouteInfo()
            {
                Entity = entityList.Where(x => x.Route == "/api/authors").First(),
                FilterExpression = "Id == @0",
                FilterValues = new object[] { 1 },
                SortExpression = "Name desc",
                Skip = 0,
                Take = 1
            };

            //act
            var result = (OkObjectResult)controller.Get(routeInfo);

            //assert
            Assert.Equal(200, result.StatusCode.Value);
            Assert.Equal(1, ((Author)((List<object>)result.Value).First()).Id);
        }


        [Fact]
        public void Get_WithCount_ReturnCount()
        {
            //arrange
            var controller = new AutoAPI.RESTAPIController(SetupHelper.BuildTestContext(), null, null);
            var routeInfo = new RouteInfo()
            {
                Entity = entityList.Where(x => x.Route == "/api/authors").First(),
                IsCount = true
            };

            //act
            var result = (OkObjectResult)controller.Get(routeInfo);

            //assert
            Assert.Equal(200, result.StatusCode.Value);
            Assert.Equal(2, (int)result.Value);
        }

        [Fact]
        public void Get_WithFilterAndCount_ReturnCount()
        {
            //arrange
            var controller = new AutoAPI.RESTAPIController(SetupHelper.BuildTestContext(), null, null);
            var routeInfo = new RouteInfo()
            {
                Entity = entityList.Where(x => x.Route == "/api/authors").First(),
                FilterExpression = "Id == @0",
                FilterValues = new object[] { 1 },
                IsCount = true
            };

            //act
            var result = (OkObjectResult)controller.Get(routeInfo);

            //assert
            Assert.Equal(200, result.StatusCode.Value);
            Assert.Equal(1, (int)result.Value);
        }


        [Fact]
        public void Get_WithFilterAndPagedResult_ReturnPaged()
        {
            //arrange
            var controller = new AutoAPI.RESTAPIController(SetupHelper.BuildTestContext(), null, null);
            var routeInfo = new RouteInfo()
            {
                Entity = entityList.Where(x => x.Route == "/api/authors").First(),
                FilterExpression = "Id == @0",
                FilterValues = new object[] { 1 },
                IsPageResult = true,
                Take = 1,
                Skip = 0,
                Page = 1
            };

            //act
            var result = (OkObjectResult)controller.Get(routeInfo);
            var pagedResult = (PagedResult)result.Value;
            //assert

            Assert.Equal(200, result.StatusCode.Value);
            Assert.Equal(1, ((Author)pagedResult.Items.First()).Id);
            Assert.Equal(2, pagedResult.Total);
        }



        [Fact]
        public void Get_WithInclude_ReturnNavigationProperty()
        {
            var db = SetupHelper.BuildTestContext();
            db.Authors.Include("abc");

            //arrange
            var controller = new AutoAPI.RESTAPIController(SetupHelper.BuildTestContext(), null, null);
            var routeInfo = new RouteInfo()
            {
                Entity = entityList.Where(x => x.Route == "/api/authors").First(),
                FilterExpression = "Id == @0",
                FilterValues = new object[] { 1 },
                IsPageResult = true,
                IncludeExpression = new List<string>() { "Books" },
                Take = 1,
                Skip = 0,
                Page = 1
            };

            //act
            var result = (OkObjectResult)controller.Get(routeInfo);
            var pagedResult = (PagedResult)result.Value;
            //assert

            Assert.Equal(200, result.StatusCode.Value);
            Assert.Equal(1, ((Author)pagedResult.Items.First()).Id);
            Assert.Equal(2, ((Author)pagedResult.Items.First()).Books.Count);
            Assert.Equal(2, pagedResult.Total);
        }


        [Fact]
        public void Get_WithPagedResult_ReturnPaged()
        {
            //arrange
            var controller = new AutoAPI.RESTAPIController(SetupHelper.BuildTestContext(), null, null);
            var routeInfo = new RouteInfo()
            {
                Entity = entityList.Where(x => x.Route == "/api/authors").First(),
                IsPageResult = true,
            };

            //act
            var result = (OkObjectResult)controller.Get(routeInfo);

            //assert
            Assert.Equal(200, result.StatusCode.Value);
            Assert.Equal(1, ((Author)((PagedResult)result.Value).Items.First()).Id);
            Assert.Equal(2, ((PagedResult)result.Value).Items.Count);
        }


        [Fact]
        public void Get_WithInvalidId_ReturnNotFound()
        {
            //arrange
            var controller = new AutoAPI.RESTAPIController(SetupHelper.BuildTestContext(), null, null);
            var routeInfo = new RouteInfo() { Entity = entityList.Where(x => x.Route == "/api/authors").First(), Id = "10" };

            //act
            var result = (NotFoundObjectResult)controller.Get(routeInfo);

            //assert
            Assert.Equal(404, result.StatusCode);

        }

        [Fact]
        public void Delete_WithIdID_Deletes()
        {
            //arrange
            var testContext = SetupHelper.BuildTestContext();
            var controller = new AutoAPI.RESTAPIController(testContext, null, null);
            var routeInfo = new RouteInfo() { Entity = entityList.Where(x => x.Route == "/api/authors").First(), Id = "1" };

            //act
            var result = (OkObjectResult)controller.Delete(routeInfo);

            //assert
            Assert.Equal(200, result.StatusCode);
            Assert.Null(testContext.Authors.Find(1));

        }

        [Fact]
        public void Delete_WithInvalidID_ReturnNotFound()
        {
            //arrange
            var controller = new AutoAPI.RESTAPIController(SetupHelper.BuildTestContext(), null, null);
            var routeInfo = new RouteInfo() { Entity = entityList.Where(x => x.Route == "/api/authors").First(), Id = "100" };

            //act
            var result = (NotFoundObjectResult)controller.Delete(routeInfo);

            //assert
            Assert.Equal(404, result.StatusCode);


        }

        [Fact]
        public void Post_WithEntity_ReturnsCreated()
        {
            //arrange
            var testContext = SetupHelper.BuildTestContext();
            var controllerMock = new Mock<RESTAPIController>(testContext, null, null);
            controllerMock.Protected().Setup<bool>("IsValid", ItExpr.Is<object>(x => true)).Returns(true);

            var routeInfo = new RouteInfo() { Entity = entityList.Where(x => x.Route == "/api/authors").First() };

            //act
            var result = (CreatedResult)controllerMock.Object.Post(routeInfo, new Author() { Id = 3, Name = "J.R.R.Tolkien" });

            //assert
            Assert.Equal(201, result.StatusCode.Value);
            Assert.Equal(3, ((Author)result.Value).Id);
            Assert.Equal("J.R.R.Tolkien", ((Author)result.Value).Name);
            Assert.Equal("J.R.R.Tolkien", testContext.Authors.Find(3).Name);
        }


        [Fact]
        public void Put_WithEntity_ReturnsUpdated()
        {
            //arrange
            var testContext = SetupHelper.BuildTestContext();
            var controllerMock = new Mock<RESTAPIController>(testContext, null, null);
            controllerMock.Protected().Setup<bool>("IsValid", ItExpr.Is<object>(x => true)).Returns(true);

            var routeInfo = new RouteInfo() { Entity = entityList.Where(x => x.Route == "/api/authors").First(), Id = "1" };

            //act
            var result = (OkObjectResult)controllerMock.Object.Put(routeInfo, new Author() { Id = 1, Name = "J.R.R.Tolkien" });

            //assert
            Assert.Equal(200, result.StatusCode.Value);
            Assert.Equal(1, ((Author)result.Value).Id);
            Assert.Equal("J.R.R.Tolkien", ((Author)result.Value).Name);
            Assert.Equal("J.R.R.Tolkien", testContext.Authors.Find(1).Name);
        }

    }
}