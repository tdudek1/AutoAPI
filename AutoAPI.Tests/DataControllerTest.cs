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
		[Fact]
		public void Delete_WithIdID_Deletes()
		{
			//arrange
			var requestProcessorMock = new Mock<IRequestProcessor>();
			requestProcessorMock.Setup(x => x.GetRoutInfo(It.IsAny<RouteData>())).Returns(new RouteInfo() { Entity = enitityList.Where(x => x.Route == "authors").First(), Id = "1" });
			var testContext = SetupHelper.BuildTestContext();

			//act
			var controller = new DataController(testContext, requestProcessorMock.Object);
			var result = (OkResult)controller.Delete();

			//assert
			Assert.Equal(200, result.StatusCode);
			Assert.Null(testContext.Authors.Find(1));

		}

		[Fact]
		public void Delete_WithInvalidID_ReturnNotFound()
		{
			//arrange
			var requestProcessorMock = new Mock<IRequestProcessor>();
			requestProcessorMock.Setup(x => x.GetRoutInfo(It.IsAny<RouteData>())).Returns(new RouteInfo() { Entity = enitityList.Where(x => x.Route == "authors").First(), Id = "5" });

			//act
			var controller = new DataController(SetupHelper.BuildTestContext(), requestProcessorMock.Object);
			var result = (NotFoundResult)controller.Delete();

			//assert
			Assert.Equal(404, result.StatusCode);


		}
		[Fact]
		public void Delete_WithInvalidRoute_ReturnNotFound()
		{
			//arrange
			var requestProcessorMock = new Mock<IRequestProcessor>();
			requestProcessorMock.Setup(x => x.GetRoutInfo(It.IsAny<RouteData>())).Returns(new RouteInfo() { Entity = null });

			//act
			var controller = new DataController(SetupHelper.BuildTestContext(), requestProcessorMock.Object);
			var result = (NotFoundResult)controller.Delete();

			//assert
			Assert.Equal(404, result.StatusCode);



		}

		[Fact]
		public void Post_WithEntity_ReturnsCreated()
		{
			//arrange
			var requestProcessorMock = new Mock<IRequestProcessor>();
			requestProcessorMock.Setup(x => x.GetRoutInfo(It.IsAny<RouteData>())).Returns(new RouteInfo() { Entity = enitityList.Where(x => x.Route == "authors").First() });
			requestProcessorMock.Setup(x => x.GetData(It.IsAny<HttpRequest>(), It.IsAny<Type>())).Returns(new Author { Id = 3, Name = "J.R.R.Tolkien" });
			var testContext = SetupHelper.BuildTestContext();

			//act
			var controller = new DataController(testContext, requestProcessorMock.Object);
			var result = (CreatedResult)controller.Post();

			//assert
			Assert.Equal(201, result.StatusCode.Value);
			Assert.Equal(3, ((Author)result.Value).Id);
			Assert.Equal("J.R.R.Tolkien", ((Author)result.Value).Name);
			Assert.Equal("J.R.R.Tolkien", testContext.Authors.Find(3).Name);
		}

		[Fact]
		public void Post_WithInvalidRoute_ReturnNotFound()
		{
			//arrange
			var requestProcessorMock = new Mock<IRequestProcessor>();
			requestProcessorMock.Setup(x => x.GetRoutInfo(It.IsAny<RouteData>())).Returns(new RouteInfo() { Entity = null });

			//act
			var controller = new DataController(SetupHelper.BuildTestContext(), requestProcessorMock.Object);
			var result = (NotFoundResult)controller.Post();

			//assert
			Assert.Equal(404, result.StatusCode);

		}

		[Fact]
		public void Put_WithEntity_ReturnsUpdated()
		{
			//arrange
			var requestProcessorMock = new Mock<IRequestProcessor>();
			requestProcessorMock.Setup(x => x.GetRoutInfo(It.IsAny<RouteData>())).Returns(new RouteInfo() { Entity = enitityList.Where(x => x.Route == "authors").First(), Id = "1" });
			requestProcessorMock.Setup(x => x.GetData(It.IsAny<HttpRequest>(), It.IsAny<Type>())).Returns(new Author { Id = 1, Name = "J.R.R.Tolkien" });
			var testContext = SetupHelper.BuildTestContext();

			//act
			var controller = new DataController(testContext, requestProcessorMock.Object);
			var result = (OkObjectResult)controller.Put();

			//assert
			Assert.Equal(200, result.StatusCode.Value);
			Assert.Equal(1, ((Author)result.Value).Id);
			Assert.Equal("J.R.R.Tolkien", ((Author)result.Value).Name);
			Assert.Equal("J.R.R.Tolkien", testContext.Authors.Find(1).Name);
		}

		[Fact]
		public void Put_WithInvalidRoute_ReturnNotFound()
		{
			//arrange
			var requestProcessorMock = new Mock<IRequestProcessor>();
			requestProcessorMock.Setup(x => x.GetRoutInfo(It.IsAny<RouteData>())).Returns(new RouteInfo() { Entity = null });
			var testContext = SetupHelper.BuildTestContext();

			//act
			var controller = new DataController(testContext, requestProcessorMock.Object);
			var result = (NotFoundResult)controller.Put();

			//assert
			Assert.Equal(404, result.StatusCode);

		}

	}
}