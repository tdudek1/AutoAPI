﻿using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace AutoAPI.Expressions
{
    public class ExpressionBuilder
    {
        private const string PAGINGPREFIX = "page";
        private const string FILTERPREFIX = "filter";
        private const string SORTPREFIX = "sort";

        private IQueryCollection queryString;
        private APIEntity apiEntity;

        public ExpressionBuilder(IQueryCollection queryString, APIEntity apiEntity)
        {
            this.queryString = queryString;
            this.apiEntity = apiEntity;
        }


        public PageResult BuildPagingResult()
        {
            return new PagingExpression(this.queryString, PAGINGPREFIX).Build();
        }

        public String BuildSortResult()
        {
            var expressionList = new List<SortExpression>();

            foreach (var key in queryString.Keys.Where(x => x.ToLower().StartsWith(SORTPREFIX)))
            {
                var parts = GetQueryStringParts(key);

                if (parts.property != null)
                {
                    expressionList.Add(new SortExpression(parts.property.Name, queryString[key]));
                }
            }

            if (expressionList.Count == 0)
            {
                return null;
            }
            else
            {
                return string.Join(", ", expressionList.Select(x => x.Build()));
            }
        }

        public FilterResult BuildFilterResult()
        {
            var expressionList = new List<FilterResult>();
            foreach (var key in queryString.Keys.Where(x => x.ToLower().StartsWith(FILTERPREFIX)))
            {
                var parts = GetQueryStringParts(key);

                if (parts.property != null)
                {
                    if (parts.queryStringParts.Count == 0)
                    {
                        expressionList.Add(new FilterExpression(parts.property, queryString[key], expressionList.LastOrDefault()?.NextIndex ?? 0).Build());
                    }
                    else
                    {
                        expressionList.Add(new FilterExpression(parts.property, queryString[key], expressionList.LastOrDefault()?.NextIndex ?? 0).Build());
                    }
                }
            }

            if (expressionList.Count == 0)
            {
                return new FilterResult();
            }
            else
            {
                return new FilterResult() { Filter = string.Join(" && ", expressionList.Select(x => x.Filter)), Values = expressionList.SelectMany(x => x.Values).ToArray() };
            }
        }

        private (PropertyInfo property, List<string> queryStringParts) GetQueryStringParts(string key)
        {
            var parts = key.Split(new char[] { '[', ']' }, StringSplitOptions.RemoveEmptyEntries).Skip(1).ToList();

            return (apiEntity.Properties.Where(x => x.Name.ToLower() == parts.FirstOrDefault()?.ToLower()).FirstOrDefault(), parts.Skip(1).ToList());
        }

    }
}
