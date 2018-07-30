using System;
using System.Linq;
using Xunit;
using AutoAPI.API;
using AutoAPI.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Moq;
using Microsoft.AspNetCore.Routing;
using System.Collections.Generic;

namespace AutoAPI.Tests
{
    public class DataControllerTests
    {
        private readonly List<APIEntity> enitityList;

        public DataControllerTests()
        {
            enitityList = APIConfiguration.Init<TestContext>();
        }

        [Fact]
        public void Get_WithNoID_ReturnAll()
        {
            //arrange
            var requestProcessorMock = new Mock<IRequestProcessor>();
            requestProcessorMock.Setup(x => x.GetRoutInfo(It.IsAny<RouteData>())).Returns(new RouteInfo() { Entity = enitityList.Where(x => x.Route == "authors").First() });

            //act
            var controller = new DataController(SetupHelper.BuildTestContext(), requestProcessorMock.Object);
            var result = (OkObjectResult)controller.Get();

            //assert
            Assert.Equal(200, result.StatusCode.Value);
            Assert.Equal(2, ((IQueryable<Author>)result.Value).Count());
            Assert.Equal(1, ((IQueryable<Author>)result.Value).First().Id);
            Assert.Equal("Ernest Hemingway", ((IQueryable<Author>)result.Value).First().Name);

        }

        [Fact]
        public void Get_WithIdID_ReturnOne()
        {
            //arrange
            var requestProcessorMock = new Mock<IRequestProcessor>();
            requestProcessorMock.Setup(x => x.GetRoutInfo(It.IsAny<RouteData>())).Returns(new RouteInfo() { Entity = enitityList.Where(x => x.Route == "authors").First(), Id = "1" });

            //act
            var controller = new DataController(SetupHelper.BuildTestContext(), requestProcessorMock.Object);
            var result = (OkObjectResult)controller.Get();

            //assert
            Assert.Equal(200, result.StatusCode.Value);
            Assert.Equal(1, ((Author)result.Value).Id);
            Assert.Equal("Ernest Hemingway", ((Author)result.Value).Name);

        }

        [Fact]
        public void Get_WithInvalidRoute_ReturnNotFound()
        {
            //arrange
            var requestProcessorMock = new Mock<IRequestProcessor>();
            requestProcessorMock.Setup(x => x.GetRoutInfo(It.IsAny<RouteData>())).Returns(new RouteInfo() { Entity = null });

            //act
            var controller = new DataController(SetupHelper.BuildTestContext(), requestProcessorMock.Object);
            var result = (NotFoundResult)controller.Get();

            //assert
            Assert.Equal(404, result.StatusCode);


        }
    }
}