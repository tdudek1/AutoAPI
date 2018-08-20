using AutoAPI.Expressions;
using AutoAPI.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Xunit;

namespace AutoAPI.Tests
{
    public class FilterOperatorExpressionTest
    {
        private readonly List<APIEntity> entityList;

        public FilterOperatorExpressionTest()
        {
            entityList = APIConfiguration.Init<DataContext>();
        }

        [Fact]
        public void Build_WhenStringAndNotSupported_ThenException()
        {
            //arrange
            var propertyInfo = entityList.Where(x => x.Route == "authors").First().Properties.Where(x => x.Name == "Name").First();

            //act
            var ex = Assert.Throws<NotSupportedException>(() => (new FilterOperatorExpression(propertyInfo, "Ernest Hemingway", 0, "abc")).Build());
        }

        [Fact]
        public void Build_WhenDateTimeAndNotSupported_ThenException()
        {
            //arrange
            var propertyInfo = entityList.Where(x => x.Route == "authors").First().Properties.Where(x => x.Name == "DateOfBirth").First();

            //act
            var ex = Assert.Throws<NotSupportedException>(() => (new FilterOperatorExpression(propertyInfo, "Ernest Hemingway", 0, "like")).Build());
        }

        [Fact]
        public void Build_WhenStringAndEquals_ThenExpression()
        {
            //arrange
            var propertyInfo = entityList.Where(x => x.Route == "authors").First().Properties.Where(x => x.Name == "Name").First();

            //act
            var result = (new FilterOperatorExpression(propertyInfo, "Ernest Hemingway", 0, "eq")).Build();

            //assert
            Assert.Equal("Name == @0", result.Filter);
            Assert.Equal("Ernest Hemingway", (string)result.Values[0]);
        }

        [Fact]
        public void Build_WhenStringAndNotEquals_ThenExpression()
        {
            //arrange
            var propertyInfo = entityList.Where(x => x.Route == "authors").First().Properties.Where(x => x.Name == "Name").First();

            //act
            var result = (new FilterOperatorExpression(propertyInfo, "Ernest Hemingway", 0, "neq")).Build();

            //assert
            Assert.Equal("Name != @0", result.Filter);
            Assert.Equal("Ernest Hemingway", (string)result.Values[0]);
        }


        [Fact]
        public void Build_WhenStringAndLike_ThenExpression()
        {
            //arrange
            var propertyInfo = entityList.Where(x => x.Route == "authors").First().Properties.Where(x => x.Name == "Name").First();

            //act
            var result = (new FilterOperatorExpression(propertyInfo, "Ernest Hemingway", 0, "like")).Build();

            //assert
            Assert.Equal("@0.Contains(Name)", result.Filter);
            Assert.Equal("Ernest Hemingway", (string)result.Values[0]);
        }

        [Fact]
        public void Build_WhenIntAndEq_ThenExpression()
        {
            //arrange
            var propertyInfo = entityList.Where(x => x.Route == "authors").First().Properties.Where(x => x.Name == "Id").First();

            //act
            var result = (new FilterOperatorExpression(propertyInfo, "1", 0, "eq")).Build();

            //assert
            Assert.Equal("Id == @0", result.Filter);
            Assert.Equal(1, (int)result.Values[0]);
        }

        [Fact]
        public void Build_WhenIntAndGt_ThenExpression()
        {
            //arrange
            var propertyInfo = entityList.Where(x => x.Route == "authors").First().Properties.Where(x => x.Name == "Id").First();

            //act
            var result = (new FilterOperatorExpression(propertyInfo, "1", 0, "gt")).Build();

            //assert
            Assert.Equal("Id > @0", result.Filter);
            Assert.Equal(1, (int)result.Values[0]);
        }

        [Fact]
        public void Build_WhenIntAndLt_ThenExpression()
        {
            //arrange
            var propertyInfo = entityList.Where(x => x.Route == "authors").First().Properties.Where(x => x.Name == "Id").First();

            //act
            var result = (new FilterOperatorExpression(propertyInfo, "1", 0, "lt")).Build();

            //assert
            Assert.Equal("Id < @0", result.Filter);
            Assert.Equal(1, (int)result.Values[0]);
        }

        [Fact]
        public void Build_WhenIntAndGtEq_ThenExpression()
        {
            //arrange
            var propertyInfo = entityList.Where(x => x.Route == "authors").First().Properties.Where(x => x.Name == "Id").First();

            //act
            var result = (new FilterOperatorExpression(propertyInfo, "1", 0, "gteq")).Build();

            //assert
            Assert.Equal("Id >= @0", result.Filter);
            Assert.Equal(1, (int)result.Values[0]);
        }

        [Fact]
        public void Build_WhenIntAndLtEq_ThenExpression()
        {
            //arrange
            var propertyInfo = entityList.Where(x => x.Route == "authors").First().Properties.Where(x => x.Name == "Id").First();

            //act
            var result = (new FilterOperatorExpression(propertyInfo, "1", 0, "lteq")).Build();

            //assert
            Assert.Equal("Id <= @0", result.Filter);
            Assert.Equal(1, (int)result.Values[0]);
        }


        [Fact]
        public void Build_WhenDateAndLtEq_ThenExpression()
        {
            //arrange
            var propertyInfo = entityList.Where(x => x.Route == "authors").First().Properties.Where(x => x.Name == "DateOfBirth").First();

            //act
            var result = (new FilterOperatorExpression(propertyInfo, "9/21/1947", 0, "lteq")).Build();

            //assert
            Assert.Equal("DateOfBirth <= @0", result.Filter);
            Assert.Equal(new DateTime(1947,9,21), (DateTime)result.Values[0]);
        }
    }
}
