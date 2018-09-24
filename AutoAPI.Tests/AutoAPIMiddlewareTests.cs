using AutoAPI.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
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
            var user = new ClaimsPrincipal();
            var entity = entityList.Where(x => x.Route == "/api/data/authors").First();

            //act
        }
    }
}
