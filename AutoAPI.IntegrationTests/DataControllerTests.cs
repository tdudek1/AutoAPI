using AutoAPI.Web.Entity;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace AutoAPI.IntegrationTests
{
    [TestCaseOrderer("AutoAPI.IntegrationTests.PriorityOrderer", "AutoAPI.IntegrationTests")]
    public class DataControllerTests
    {
        private ITestOutputHelper output;
        private HttpClient client;
        private HttpClient authClient;
        private readonly Uri baseUrl = new Uri("http://localhost:5000/api/data/");

        public DataControllerTests(ITestOutputHelper output)
        {
            this.output = output;
            this.client = new HttpClient() { BaseAddress = baseUrl };
            this.authClient = new HttpClient() { BaseAddress = baseUrl };
            this.authClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Login());
        }


        [Fact, TestPriority(1)]
        public async void DateController_WhenGetAll_ReturnList()
        {

            //act
            var result = await client.GetFromJsonAsync<IEnumerable<Author>>("authors");

            //Assert

            Assert.Equal(2, result.Count());
            Assert.Equal(1, result.First().Id);
            Assert.Equal("Ernest Hemingway", result.First().Name);
        }

        [Fact, TestPriority(2)]
        public async void DateController_WhenGetById_ReturnOne()
        {
            //act
            var result = await client.GetFromJsonAsync<Author>("authors/1");

            //Assert
            Assert.Equal(1, result.Id);
            Assert.Equal("Ernest Hemingway", result.Name);
        }

        [Fact, TestPriority(3)]
        public async void DateController_WhenGetAllOrderDesc_ReturnListDesc()
        {
            //act
            var result = await client.GetFromJsonAsync<IEnumerable<Author>>("authors?sort[id]=desc");

            //Assert
            Assert.Equal(2, result.Count());
            Assert.Equal(2, result.First().Id);
            Assert.Equal("Stephen King", result.First().Name);
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
        [InlineData("filter[dateofbirth][in]=[\"1899-07-21\"]", 1, 1, "Ernest Hemingway")]
        [InlineData("filter[dateofbirth][nin]=[\"1947-09-21\"]", 1, 1, "Ernest Hemingway")]
        [InlineData("filter[uniqueid][in]=[\"00000000-0000-0000-0000-000000000000\"]", 1, 1, "Ernest Hemingway")]
        [InlineData("filter[uniqueid][nin]=[\"00000000-0000-0000-0000-000000000000\"]", 1, 2, "Stephen King")]
        [InlineData("filter[name][in]=[\"Ernest Hemingway\",\"blah\"]", 1, 1, "Ernest Hemingway")]
        [InlineData("filter[name][nin]=[\"Ernest Hemingway\"]", 1, 2, "Stephen King")]
        [InlineData("filter[uniqueid][eq]=00000000-0000-0000-0000-000000000000", 1, 1, "Ernest Hemingway")]
        [InlineData("filter[uniqueid][neq]=00000000-0000-0000-0000-000000000000", 1, 2, "Stephen King")]
        public async void DateController_WhenGetFilter_ReturnFiltered(string filter, int count, int id, string name)
        {
            //act
            var result = await client.GetFromJsonAsync<IEnumerable<Author>>($"authors?{filter}");

            //Assert
            Assert.Equal(count, result.Count());
            Assert.Equal(id, result.First().Id);
            Assert.Equal(name, result.First().Name);
        }


        [Fact, TestPriority(5)]
        public async void DateController_WhenPostToBooks_ReturnNewBook()
        {
            //arrange
            var request = new HttpRequestMessage(HttpMethod.Post, new Uri(baseUrl, $"books"));
            request.Headers.Add("Authorization", Login());


            //act
            var result = await authClient.PostAsJsonAsync<Book>("books", new Book() { ISBN = Guid.NewGuid().ToString(), AuthorId = 1, Title = "The Sun Also Rises" },new JsonSerializerOptions());

            //assert
            Assert.Equal(HttpStatusCode.Created, result.StatusCode);
            var book = await result.Content.ReadFromJsonAsync<Book>();
            Assert.Equal(1, book.AuthorId);
            Assert.Equal("The Sun Also Rises", book.Title);
        }


        [Fact, TestPriority(6)]
        public async void DateController_WhenPostToBooksAndInvalid_ReturnBadRequest()
        {

            //act
            var result = await authClient.PostAsJsonAsync<Book>("books", new Book() { ISBN = Guid.NewGuid().ToString(), AuthorId = 1, Title = "" },new JsonSerializerOptions());

            //assert
            Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);

        }


        [Fact, TestPriority(7)]
        public async void DateController_WhenPutToBooks_ReturnUpdateBookd()
        {
            //act
            var result = await authClient.PutAsJsonAsync<Book>("books/5678", new Book() { ISBN = "5678", AuthorId = 1, Title = "The Sun Also Rises" },new JsonSerializerOptions());

            //Assert
            var book = await result.Content.ReadFromJsonAsync<Book>();
            Assert.Equal(HttpStatusCode.OK, result.StatusCode);
            Assert.Equal(1, book.AuthorId);
            Assert.Equal("The Sun Also Rises", book.Title);
        }


        [Fact, TestPriority(8)]
        public async void DateController_WhenDeleteToBooks_ReturnDeletedOK()
        {
            //act
            var result = await authClient.DeleteAsync("books/99999");

            //Assert
            Assert.Equal(HttpStatusCode.OK, result.StatusCode);

        }

        [Fact, TestPriority(9)]
        public async void DateController_WhenGetToBooksAndNoToken_ReturnUnauthorized()
        {
            //arrange
            var request = new HttpRequestMessage(HttpMethod.Get, new Uri(baseUrl, $"books"));

            //act
            var result = await client.GetAsync("books");

            //Assert
            Assert.Equal(HttpStatusCode.Unauthorized, result.StatusCode);
        }

        [Fact, TestPriority(10)]
        public async void DateController_WhenGetCount_ReturnCount()
        {
            //arrange
            var request = new HttpRequestMessage(HttpMethod.Get, new Uri(baseUrl, "authors/count"));

            //act
            var result = await client.GetFromJsonAsync<int>("authors/count");

            //Assert
            Assert.Equal(2, result);
        }



        [Fact, TestPriority(11)]
        public async void DateController_WhenGetPagedResult_ReturnPagedResult()
        {
            //arrange
            var request = new HttpRequestMessage(HttpMethod.Get, new Uri(baseUrl, "authors/pagedresult?page=1&pageSize=1"));

            //act
            var result = await client.GetFromJsonAsync<PagedResult>("authors/pagedresult?page=1&pageSize=1");

            //Assert
            Assert.Equal(2, result.Total);
            Assert.Single(result.Items);
            Assert.Equal(1, result.PageSize);
            Assert.Equal(2, result.PageCount);
        }


        [Fact, TestPriority(12)]
        public async void DateController_WhenPagedResultWithNoPage_ReturnPagedResult()
        {
            //act
            var result = await client.GetFromJsonAsync<PagedResult>("authors/pagedresult");

            //Assert
            Assert.Equal(2, result.Total);
            Assert.Equal(2, result.Items.Count());
            Assert.Equal(2, result.PageSize);
            Assert.Equal(1, result.PageCount);
        }

        [Fact, TestPriority(13)]
        public async void DateController_WhenGetCountAndFilter_ReturnCount()
        {
            //act
            var result = await client.GetFromJsonAsync<int>("authors/count?filter[Id]=1");

            //Assert
            Assert.Equal(1, result);
        }

        [Fact, TestPriority(14)]
        public async void DateController_WhenGetAllAndInclude_ReturnList()
        {

            //act
            var result = await client.GetFromJsonAsync<IEnumerable<Author>>("authors?include=Books");

            //Assert
            Assert.Equal(1, result.First().Id);
            Assert.Equal("Ernest Hemingway", result.First().Name);
            Assert.True(result.First().Books.Count() > 0);
        }

        private string Login()
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("SuperDuperSecureKey"));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var claims = new List<Claim> {
                new Claim(ClaimTypes.Role, "Admin"),
                new Claim(ClaimTypes.NameIdentifier, Guid.Empty.ToString()),
                new Claim(ClaimTypes.Name, "admin")
            };

            var token = new JwtSecurityToken(
                issuer: "test.com",
                audience: "test.com",
                claims: claims,
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: creds);

            var t = new JwtSecurityTokenHandler().WriteToken(token);
            return t;
        }

        private class Token
        {
            public string token { get; set; }
        }

    }
}
