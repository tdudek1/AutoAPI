using System;
using System.Linq;
using Xunit;
using AutoAPI.API;
using AutoAPI.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

namespace AutoAPI.Tests
{
    public class DataControllerTests
    {
        [Fact]
        public void Get_WithNoID_ReturnAll()
        {
            Extensions.Init<TestContext>();

            var controller = new DataController(SetupHelper.BuildTestContext());

        }
    }
}