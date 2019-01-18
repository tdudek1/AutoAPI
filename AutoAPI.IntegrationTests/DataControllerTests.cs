using AutoAPI.Web.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace AutoAPI.IntegrationTests
{
    [TestCaseOrderer("AutoAPI.IntegrationTests.PriorityOrderer", "AutoAPI.IntegrationTests")]
    public class DataControllerTests
    {
        private ITestOutputHelper output;

        public DataControllerTests(ITestOutputHelper output)
        {
            this.output = output;
        }
        private readonly Uri baseUrl = new Uri("http://localhost:5000/api/data/");
        private string token;

        [Fact, TestPriority(1)]
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

        [Fact, TestPriority(2)]
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

        [Fact, TestPriority(3)]
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


        [Theory, TestPriority(4)]
        [InlineData("filter[name][like]=Ernest", 1, 1, "Ernest Hemingway")]
        [InlineData("filter[name][nlike]=Ernest", 1, 2, "Stephen King")]
        [InlineData("filter[name]=Ernest Hemingway", 1, 1, "Ernest Hemingway")]
        [InlineData("filter[name][neq]=Ernest Hemingway", 1, 2, "Stephen King")]
        [InlineData("filter[id][gt]=1", 1, 2, "Stephen King")]
        [InlineData("filter[id][lt]=2", 1, 1, "Ernest Hemingway")]
        [InlineData("filter[id][gt]=0&filter[id][lt]=2", 1, 1, "Ernest Hemingway")]
        [InlineData("filter[id][gteq]=1", 2, 1, "Ernest Hemingway")]
        [InlineData("filter[id][lteq]=2", 2, 1, "Ernest Hemingway")]
        [InlineData("filter[id][in]=[1]", 1, 1, "Ernest Hemingway")]
        [InlineData("filter[id][nin]=[2]", 1, 1, "Ernest Hemingway")]
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


        [Fact, TestPriority(5)]
        public async void DateController_WhenPostToBooks_ReturnNewBook()
        {
            //arrange
            var request = new HttpRequestMessage(HttpMethod.Post, new Uri(baseUrl, $"books"));
            request.Headers.Add("Authorization", await Login());

            //act
            var result = await Helper.Json<Book>(request, new Book() { ISBN = Guid.NewGuid().ToString(), AuthorId = 1, Title = "The Sun Also Rises" });

            //assert
            Assert.Equal(HttpStatusCode.Created, result.StatusCode);
            Assert.Equal(1, result.Object.AuthorId);
            Assert.Equal("The Sun Also Rises", result.Object.Title);
        }


        [Fact, TestPriority(6)]
        public async void DateController_WhenPostToBooksAndInvalid_ReturnBadRequest()
        {
            //arrange
            var request = new HttpRequestMessage(HttpMethod.Post, new Uri(baseUrl, $"books"));
            request.Headers.Add("Authorization", await Login());

            //act
            var result = await Helper.Response(request, new Book() { ISBN = Guid.NewGuid().ToString(), AuthorId = 1, Title = "" });

            //assert
            Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);
        }


        [Fact, TestPriority(7)]
        public async void DateController_WhenPutToBooks_ReturnUpdateBookd()
        {

            //arrange
            var request = new HttpRequestMessage(HttpMethod.Put, new Uri(baseUrl, $"books/5678"));
            request.Headers.Add("Authorization", await Login());
            //act

            var result = await Helper.Json<Book>(request, new Book() { ISBN = "5678", AuthorId = 1, Title = "The Sun Also Rises" });

            //Assert
            Assert.Equal(HttpStatusCode.OK, result.StatusCode);
            Assert.Equal(1, result.Object.AuthorId);
            Assert.Equal("The Sun Also Rises", result.Object.Title);

        }


        [Fact, TestPriority(8)]
        public async void DateController_WhenDeleteToBooks_ReturnDeletedOK()
        {
            //arrange
            var request = new HttpRequestMessage(HttpMethod.Delete, new Uri(baseUrl, $"books/99999"));
            request.Headers.Add("Authorization", await Login());

            //act
            var result = await Helper.Response(request);

            //Assert
            Assert.Equal(HttpStatusCode.OK, result.StatusCode);

        }

        [Fact, TestPriority(9)]
        public async void DateController_WhenGetToBooksAndNoToken_ReturnUnauthorized()
        {
            //arrange
            var request = new HttpRequestMessage(HttpMethod.Get, new Uri(baseUrl, $"books"));

            //act
            var result = await Helper.Response(request);

            //Assert
            Assert.Equal(HttpStatusCode.Unauthorized, result.StatusCode);
        }

        [Fact, TestPriority(10)]
        public async void DateController_WhenGetCount_ReturnCount()
        {
            //arrange
            var request = new HttpRequestMessage(HttpMethod.Get, new Uri(baseUrl, "authors/count"));

            //act
            var result = await Helper.Json<int>(request);

            //Assert
            Assert.Equal(HttpStatusCode.OK, result.StatusCode);
            Assert.Equal(2, (int)result.Object);
        }



        [Fact, TestPriority(11)]
        public async void DateController_WhenGetPagedResult_ReturnPagedResult()
        {
            //arrange
            var request = new HttpRequestMessage(HttpMethod.Get, new Uri(baseUrl, "authors/pagedresult?page=1&pageSize=1"));

            //act
            var result = await Helper.Json<PagedResult>(request);
            var pagedResult = (PagedResult)result.Object;

            //Assert
            Assert.Equal(HttpStatusCode.OK, result.StatusCode);
            Assert.Equal(2, pagedResult.Total);
            Assert.Single(pagedResult.Items);
            Assert.Equal(1, pagedResult.PageSize);
            Assert.Equal(2, pagedResult.PageCount);
        }


        [Fact, TestPriority(12)]
        public async void DateController_WhenPagedResultWithNoPagfe_ReturnPagedResult()
        {
            //arrange
            var request = new HttpRequestMessage(HttpMethod.Get, new Uri(baseUrl, "authors/pagedresult"));

            //act
            var result = await Helper.Json<PagedResult>(request);
            var pagedResult = (PagedResult)result.Object;

            //Assert
            Assert.Equal(HttpStatusCode.OK, result.StatusCode);
            Assert.Equal(2, pagedResult.Total);
            Assert.Equal(2, pagedResult.Items.Count());
            Assert.Equal(2, pagedResult.PageSize);
            Assert.Equal(1, pagedResult.PageCount);
        }

        [Fact, TestPriority(13)]
        public async void DateController_WhenGetCountAndFilter_ReturnCount()
        {
            //arrange
            var request = new HttpRequestMessage(HttpMethod.Get, new Uri(baseUrl, "authors/count?filter[Id]=1"));

            //act
            var result = await Helper.Json<int>(request);

            //Assert
            Assert.Equal(HttpStatusCode.OK, result.StatusCode);
            Assert.Equal(1, (int)result.Object);
        }

        [Fact, TestPriority(14)]
        public async void DateController_WhenGetAllAndInclude_ReturnList()
        {
            //arrange
            var request = new HttpRequestMessage(HttpMethod.Get, new Uri(baseUrl, "authors?include=Books"));

            //act
            var result = await Helper.Json<List<Author>>(request);

            //Assert
            Assert.Equal(HttpStatusCode.OK, result.StatusCode);
            Assert.Equal(1, result.Object.First().Id);
            Assert.Equal("Ernest Hemingway", result.Object.First().Name);
            Assert.True(result.Object.First().Books.Count() > 0);
        }

        private async Task<string> Login()
        {
            if (string.IsNullOrEmpty(this.token))
            {
                var result = await Helper.Json<Token>(new HttpRequestMessage()
                {
                    Method = HttpMethod.Post,
                    RequestUri = new Uri("http://localhost:5000/login"),
                    Content = new FormUrlEncodedContent(new Dictionary<string, string>() { { "UserName", "test@test.com" }, { "Password", "Password1234!" } })
                });

                this.token = result.Object.token;
            }

            return "Bearer " + this.token;
        }

        private class Token
        {
            public string token { get; set; }
        }

    }
}
