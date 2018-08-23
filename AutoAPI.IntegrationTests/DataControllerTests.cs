using AutoAPI.Web.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using Xunit;

namespace AutoAPI.IntegrationTests
{
    public class DataControllerTests
    {
        private readonly Uri baseUrl = new Uri("http://localhost:5000/api/data/");

        [Fact]
        public async void DateController_WhenGetAll_ReturnList()
        {
            //arrange
            var request = new HttpRequestMessage(HttpMethod.Get, new Uri(baseUrl, "authors"));

            //act
            var result = await Helper.Json<List<Author>>(request);

            //Assert
            Assert.Equal(HttpStatusCode.OK, result.StatusCode);
            Assert.Equal(2, result.Object.Count());
            Assert.Equal(1, result.Object.First().Id);
            Assert.Equal("Ernest Hemingway", result.Object.First().Name);
        }

        [Fact]
        public async void DateController_WhenGetById_ReturnOne()
        {
            //arrange
            var request = new HttpRequestMessage(HttpMethod.Get, new Uri(baseUrl, "authors/1"));

            //act
            var result = await Helper.Json<Author>(request);

            //Assert
            Assert.Equal(HttpStatusCode.OK, result.StatusCode);
            Assert.Equal(1, result.Object.Id);
            Assert.Equal("Ernest Hemingway", result.Object.Name);
        }

        [Fact]
        public async void DateController_WhenGetAllOrderDesc_ReturnListDesc()
        {
            //arrange
            var request = new HttpRequestMessage(HttpMethod.Get, new Uri(baseUrl, "authors?sort[id]=desc"));

            //act
            var result = await Helper.Json<List<Author>>(request);

            //Assert
            Assert.Equal(HttpStatusCode.OK, result.StatusCode);
            Assert.Equal(2, result.Object.Count());
            Assert.Equal(2, result.Object.First().Id);
            Assert.Equal("Stephen King", result.Object.First().Name);
        }


        [Theory]
        [InlineData("filter[name]=Ernest Hemingway", 1, 1, "Ernest Hemingway")]
        [InlineData("filter[name][neq]=Ernest Hemingway", 1, 2, "Stephen King")]
        [InlineData("filter[id][gt]=1", 1, 2, "Stephen King")]
        [InlineData("filter[id][lt]=2", 1, 1, "Ernest Hemingway")]
        [InlineData("filter[id][gt]=0&filter[id][lt]=2", 1, 1, "Ernest Hemingway")]
        [InlineData("filter[id][gteq]=1", 2, 1, "Ernest Hemingway")]
        [InlineData("filter[id][lteq]=2", 2, 1, "Ernest Hemingway")]
        public async void DateController_WhenGetFilter_ReturnFiltered(string filter, int count, int id, string name)
        {
            //arrange
            var request = new HttpRequestMessage(HttpMethod.Get, new Uri(baseUrl, $"authors?{filter}"));

            //act
            var result = await Helper.Json<List<Author>>(request);

            //Assert
            Assert.Equal(HttpStatusCode.OK, result.StatusCode);
            Assert.Equal(count, result.Object.Count);
            Assert.Equal(id, result.Object.First().Id);
            Assert.Equal(name, result.Object.First().Name);
        }
    }
}
