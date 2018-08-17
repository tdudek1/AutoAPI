using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AutoAPI.Expressions
{
    public class PagingExpression : IExpression<PageResult>
    {
        private IQueryCollection queryString;
        private string prefix;
        public PagingExpression(IQueryCollection queryString, string prefix)
        {
            this.queryString = queryString;
            this.prefix = prefix;
        }

        public PageResult Build()
        {
            var result = new PageResult();
            var pageSize = 0U;
            var page = 1U;

            foreach (var key in queryString.Keys.Where(x => x.ToLower().StartsWith(prefix)))
            {
                if (key.ToLower() == $"{prefix}size")
                {
                    uint.TryParse(queryString[key].ToString(), out pageSize);
                }

                if (key.ToLower() == prefix)
                {
                    uint.TryParse(queryString[key].ToString(), out page);
                }
            }

            result.Take = (int)pageSize;
            result.Skip = (int)((page - 1U) * pageSize);

            return result;
        }
    }
}
