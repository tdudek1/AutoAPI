using AutoAPI.Web;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace AutoAPI.Tests
{
    public class ExpressionBuilderTests
    {

        private readonly List<APIEntity> entityList;

        public ExpressionBuilderTests()
        {
            entityList = APIConfiguration.Init<DataContext>();
        }

        #region Paging

        [Fact]
        public void BuildPaging_NoPaging_ThenZerot()
        {
            //arrange
            var queryString = new Dictionary<string, StringValues>();

            //act
            var result = (new Expressions.ExpressionBuilder(new QueryCollection(queryString), null)).BuildPagingResult();

            Assert.Equal(0, result.Take);
        }

        [Fact]
        public void BuildPaging_PageSize10_ThenTakeTen()
        {
            //arrange
            var queryString = new Dictionary<string, StringValues>();
            queryString.Add("pageSize", "10");

            //act
            var result = (new Expressions.ExpressionBuilder(new QueryCollection(queryString), null)).BuildPagingResult();

            //assert
            Assert.Equal(10, result.Take);
            Assert.Equal(0, result.Skip);
        }

        [Fact]
        public void BuildPaging_PageSize10AndPage3_ThenTakeTenSkip3()
        {
            //arrange
            var queryString = new Dictionary<string, StringValues>();
            queryString.Add("pageSize", "10");
            queryString.Add("page", "3");

            //act
            var result = (new Expressions.ExpressionBuilder(new QueryCollection(queryString), null)).BuildPagingResult();

            //assert
            Assert.Equal(10, result.Take);
            Assert.Equal(20, result.Skip);
        }

        #endregion

        #region Sort

        [Fact]
        public void BuildSort_WhenTwoProperties_ThenFilter()
        {
            //arrange
            var queryString = new Dictionary<string, StringValues>();
            queryString.Add("sort[Name]", "desc");
            queryString.Add("sort[DateOfBirth]", "asc");

            //act
            var result = (new Expressions.ExpressionBuilder(new QueryCollection(queryString), entityList.Where(x => x.Route == "authors").First())).BuildSortResult();

            //assert
            Assert.Equal("Name desc, DateOfBirth asc", result);

        }

        [Fact]
        public void BuildSort_NoSorts_ThenNull()
        {
            //arrange
            var queryString = new Dictionary<string, StringValues>();

            //act
            var result = (new Expressions.ExpressionBuilder(new QueryCollection(queryString), entityList.Where(x => x.Route == "authors").First())).BuildSortResult();

            //assert
            Assert.Null(result);
        }

        #endregion

        #region Filter

        [Fact]
        public void BuildFilter_WhenOneProperty_ThenFilter()
        {
            //arrange
            var queryString = new Dictionary<string, StringValues>();
            queryString.Add("filter[Name]", "Ernest Hemingway");

            //act
            var result = (new Expressions.ExpressionBuilder(new QueryCollection(queryString), entityList.Where(x => x.Route == "authors").First())).BuildFilterResult();

            //assert
            Assert.Equal("Name == @0", result.Filter);
            Assert.Single(result.Values);
            Assert.Equal("Ernest Hemingway", (string)result.Values[0]);

        }

        [Fact]
        public void BuildFilter_WhenTwoProperties_ThenFilter()
        {
            //arrange
            var queryString = new Dictionary<string, StringValues>();
            queryString.Add("filter[Name]", "Ernest Hemingway");
            queryString.Add("filter[DateOfBirth]", "7/21/1899");

            //act
            var result = (new Expressions.ExpressionBuilder(new QueryCollection(queryString), entityList.Where(x => x.Route == "authors").First())).BuildFilterResult();

            //assert
            Assert.Equal("Name == @0 && DateOfBirth == @1", result.Filter);
            Assert.Equal(2, result.Values.Count());
            Assert.Equal("Ernest Hemingway", (string)result.Values[0]);
            Assert.Equal(new DateTime(1899, 7, 21), (DateTime)result.Values[1]);

        }


        [Fact]
        public void BuildFilter_NoFilters_ThenNull()
        {
            //arrange
            var queryString = new Dictionary<string, StringValues>();

            //act
            var result = (new Expressions.ExpressionBuilder(new QueryCollection(queryString), entityList.Where(x => x.Route == "authors").First())).BuildFilterResult();

            Assert.Null(result.Filter);
        }

        [Fact]
        public void BuildFilter_TwoFilterAndOr_ThenExpression()
        {
            //arrange
            var queryString = new Dictionary<string, StringValues>();
            queryString.Add("filter[Name]", "Ernest Hemingway");
            queryString.Add("filter[DateOfBirth]", "7/21/1899");
            queryString.Add("operator", "or");

            //act
            var result = (new Expressions.ExpressionBuilder(new QueryCollection(queryString), entityList.Where(x => x.Route == "authors").First())).BuildFilterResult();

            //assert
            Assert.Equal("Name == @0 || DateOfBirth == @21", result.Filter);
            Assert.Equal(2, result.Values.Count());
        }

        #endregion
    }
}
