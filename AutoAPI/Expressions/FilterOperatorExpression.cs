using System;
using System.Collections.Generic;
using System.Reflection;

namespace AutoAPI.Expressions
{
    public class FilterOperatorExpression
    {
        private PropertyInfo property;
        private string value;
        private int index;
        private string comparisonOperator;
        private readonly List<string> stringOperators = new List<string> { "eq", "neq", "like", "nlike" };
        private readonly List<string> valueTypeOperators = new List<string> { "eq", "neq", "lt", "gt", "gteq", "lteq" };
        private readonly List<string> guidOperators = new List<string> { "eq", "neq" };


        public FilterOperatorExpression(PropertyInfo property, string value, int index, string comparisonOperator)
        {
            this.property = property;
            this.value = value;
            this.index = index;
            this.comparisonOperator = comparisonOperator;
        }

        public FilterResult Build()
        {

            if (property.PropertyType == typeof(string) && !stringOperators.Contains(comparisonOperator))
            {
                throw new NotSupportedException($"String properties only support {string.Join(",", stringOperators)}");
            }

            if ((property.PropertyType == typeof(DateTime) || property.PropertyType.IsValueType) && !valueTypeOperators.Contains(comparisonOperator))
            {
                throw new NotSupportedException($"Value type and DateTime properties only support {string.Join(",", stringOperators)}");
            }

            if (property.PropertyType == typeof(Guid) && !valueTypeOperators.Contains(comparisonOperator))
            {
                throw new NotSupportedException($"Guid properties only support {string.Join(",", guidOperators)}");
            }

            return new FilterResult()
            {
                Filter = GetExpresssion(property.Name, comparisonOperator, index),
                Values = new[] { Convert.ChangeType(value, property.PropertyType) },
                NextIndex = this.index + 1
            };
        }

        private string GetExpresssion(string propertyName, string comparisonOperator, int index)
        {
            switch (comparisonOperator)
            {
                case "eq":
                    return $"{propertyName} == @{index}";
                case "neq":
                    return $"{propertyName} != @{index}"; ;
                case "like":
                    return $"@{index}.Contains({propertyName})";
                case "nlike":
                    return $"!@{index}.Contains({propertyName})";
                case "gt":
                    return $"{propertyName} > @{index}";
                case "lt":
                    return $"{propertyName} < @{index}";
                case "gteq":
                    return $"{propertyName} >= @{index}";
                case "lteq":
                    return $"{propertyName} <= @{index}";
            }

            throw new NotSupportedException($"Operator {comparisonOperator} is not supported");
        }
    }
}
