using AutoAPI.Web;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Moq;
using Moq.Protected;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace AutoAPI.Tests
{
	public class AutoAPIMiddlewareTests
	{
		private readonly List<APIEntity> entityList;

		public AutoAPIMiddlewareTests()
		{
			entityList = APIConfiguration.Init<DataContext>("/api/data");
		}


		[Fact]
		public void IsAuthorized_WhenAnonymous_ThenTrue()
		{
			//arrange
			var middleware = new AutoAPIMiddleware(null);
			var userMock = new Mock<ClaimsPrincipal>();
			var identityMock = new Mock<IIdentity>();
			identityMock.Setup(x => x.IsAuthenticated).Returns(false);

			userMock.Setup(x => x.Identity).Returns(identityMock.Object);
			var entity = entityList.Where(x => x.Route == "/api/data/authors").First();

			//act
			var result = middleware.IsAuthorized(null, userMock.Object, entity, "GET");

			Assert.True(result);
		}


		[Fact]
		public void IsAuthorized_WhenNeedsAuthorizedButNot_ThenFalse()
		{
			//arrange
			var middleware = new AutoAPIMiddleware(null);
			var userMock = new Mock<ClaimsPrincipal>();
			var identityMock = new Mock<IIdentity>();
			identityMock.Setup(x => x.IsAuthenticated).Returns(false);

			userMock.Setup(x => x.Identity).Returns(identityMock.Object);
			var entity = entityList.Where(x => x.Route == "/api/data/authors").First();
			entity.Authorize = true;

			//act
			var result = middleware.IsAuthorized(null, userMock.Object, entity, "GET");

			Assert.False(result);
		}


		[Fact]
		public void IsAuthorized_WhenEntityPolicyButDoesntMatch_ThenFalse()
		{
			//arrange
			var middlewareMock = new Mock<AutoAPIMiddleware>(null);
			var userMock = new Mock<ClaimsPrincipal>();
			var identityMock = new Mock<IIdentity>();
			identityMock.Setup(x => x.IsAuthenticated).Returns(true);
			userMock.Setup(x => x.Identity).Returns(identityMock.Object);
			var entity = entityList.Where(x => x.Route == "/api/data/authors").First();
			entity.Authorize = false;
			entity.EntityPolicy = "IsAdmin";
			middlewareMock.Protected().Setup<bool>("Authorize", ItExpr.IsAny<IAuthorizationService>(), ItExpr.IsAny<ClaimsPrincipal>(), ItExpr.IsAny<string>()).Returns(false);


			//act
			var result = middlewareMock.Object.IsAuthorized(null, userMock.Object, entity, "GET");

			Assert.False(result);
		}


		[Theory]
		[InlineData("GET")]
		[InlineData("POST")]
		[InlineData("PUT")]
		[InlineData("DELETE")]
		public void IsAuthorized_WhenMethodPolicyButDoesntMatch_ThenFalse(string method)
		{
			//arrange
			var middlewareMock = new Mock<AutoAPIMiddleware>(null);
			var userMock = new Mock<ClaimsPrincipal>();
			var identityMock = new Mock<IIdentity>();
			identityMock.Setup(x => x.IsAuthenticated).Returns(true);
			userMock.Setup(x => x.Identity).Returns(identityMock.Object);
			var entity = entityList.Where(x => x.Route == "/api/data/authors").First();
			entity.Authorize = false;
			entity.GETPolicy = "IsAdmin";
			entity.POSTPolicy = "IsAdmin";
			entity.PUTPolicy = "IsAdmin";
			entity.DELETEPolicy = "IsAdmin";
			middlewareMock.Protected().Setup<bool>("Authorize", ItExpr.IsAny<IAuthorizationService>(), ItExpr.IsAny<ClaimsPrincipal>(), ItExpr.IsAny<string>()).Returns(false);


			//act
			var result = middlewareMock.Object.IsAuthorized(null, userMock.Object, entity, method);

			//assert
			Assert.False(result);
		}

		[Fact]
		public async void InvokeAsync_RequestNotMatched_ThenInvokeNext()
		{
			//arrange
			var nextMock = new Mock<RequestDelegate>();
			var requestProcessorMock = new Mock<IRequestProcessor>();
			requestProcessorMock.Setup(x => x.GetRoutInfo(It.IsAny<HttpRequest>())).Returns(new RouteInfo());

			var contextMock = new Mock<HttpContext>();
			var middlewareMock = new Mock<AutoAPIMiddleware>(nextMock.Object);

			//act
			await middlewareMock.Object.InvokeAsync(contextMock.Object, requestProcessorMock.Object, null);

			//assert
			nextMock.Verify(x => x(It.IsAny<HttpContext>()), Times.Once);
		}

}
