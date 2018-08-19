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
            var ex = Assert.Throws<NotSupportedException>(() => (new FilterOperatorExpression(propertyInfo, "Ernest Hemingway", 0, "gt")).Build());
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
        public void Build_WhenStringAndNotLike_ThenExpression()
        {
            //arrange
            var propertyInfo = entityList.Where(x => x.Route == "authors").First().Properties.Where(x => x.Name == "Name").First();

            //act
            var result = (new FilterOperatorExpression(propertyInfo, "Ernest Hemingway", 0, "nlike")).Build();

            //assert
            Assert.Equal("!@0.Contains(Name)", result.Filter);
            Assert.Equal("Ernest Hemingway", (string)result.Values[0]);
        }

    }
}
