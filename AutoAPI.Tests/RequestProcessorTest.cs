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
			APIConfiguration.AutoAPIEntityCache.AddRange(APIConfiguration.Init<DataContext>("/api/data"));
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
			var result = (new RequestProcessor(null, null)).GetData(httpRequestMock.Object, typeof(Author));

			//assert
			Assert.Equal(1, ((Author)result).Id);
			Assert.Equal("Ernest Hemingway", ((Author)result).Name);
		}

		[Fact]
		public void GetRoutInfo_WhenPath_ThenEntity()
		{
			//arrange
			var requestMock = new Mock<HttpRequest>();
			requestMock.Setup(x => x.Path).Returns(new PathString("/api/data/authors"));

			//act
			var requestProcessor = new RequestProcessor(null, null);
			var result = requestProcessor.GetRoutInfo(requestMock.Object);

			//assert
			Assert.Equal(APIConfiguration.AutoAPIEntityCache.Where(x => x.Route.Equals("/api/data/authors")).First(), result.Entity);

		}

		[Fact]
		public void GetRoutInfo_WhenPathMixedCase_ThenEntity()
		{
			//arrange
			var requestMock = new Mock<HttpRequest>();
			requestMock.Setup(x => x.Path).Returns(new PathString("/api/Data/auThors"));

			//act
			var requestProcessor = new RequestProcessor(null, null);
			var result = requestProcessor.GetRoutInfo(requestMock.Object);

			//assert
			Assert.Equal(APIConfiguration.AutoAPIEntityCache.Where(x => x.Route.Equals("/api/data/authors")).First(), result.Entity);

		}

		[Fact]
		public void GetRoutInfo_WhenPathEndsInSlash_ThenEntity()
		{
			//arrange
			var requestMock = new Mock<HttpRequest>();
			requestMock.Setup(x => x.Path).Returns(new PathString("/api/data/authors/"));

			//act
			var requestProcessor = new RequestProcessor(null, null);
			var result = requestProcessor.GetRoutInfo(requestMock.Object);

			//assert
			Assert.Equal(APIConfiguration.AutoAPIEntityCache.Where(x => x.Route.Equals("/api/data/authors")).First(), result.Entity);

		}


		[Fact]
		public void GetRoutInfo_WhenPathAndId_ThenEntityAndId()
		{
			//arrange
			var requestMock = new Mock<HttpRequest>();
			requestMock.Setup(x => x.Path).Returns(new PathString("/api/data/authors/1"));

			//act
			var requestProcessor = new RequestProcessor(null, null);
			var result = requestProcessor.GetRoutInfo(requestMock.Object);

			//assert
			Assert.Equal(APIConfiguration.AutoAPIEntityCache.Where(x => x.Route.Equals("/api/data/authors")).First(), result.Entity);
			Assert.Equal("1", result.Id);
		}

		[Fact]
		public void GetRoutInfo_WhenPathWhenFiltersPagingSorting_ThenEntity()
		{
			//arrange
			var requestMock = new Mock<HttpRequest>();
			requestMock.Setup(x => x.Path).Returns(new PathString("/api/data/authors"));
			requestMock.Setup(x => x.Query).Returns(new QueryCollection(new Dictionary<string, StringValues>() { { "page", "1" } }));

			//act
			var requestProcessor = new RequestProcessor(null, null);
			var result = requestProcessor.GetRoutInfo(requestMock.Object);

			//assert
			Assert.Equal(APIConfiguration.AutoAPIEntityCache.Where(x => x.Route.Equals("/api/data/authors")).First(), result.Entity);

		}

	}
}
