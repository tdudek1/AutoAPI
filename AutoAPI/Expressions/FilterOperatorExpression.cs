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


        public FilterOperatorExpression(PropertyInfo property, string value, int index, string comparisonOperator)
        {
            this.property = property;
            this.value = value;
            this.index = index;
            this.comparisonOperator = comparisonOperator;
        }

        public FilterResult Build()
        {
            if(!property.PropertyType.IsOperatorSuported(comparisonOperator))
            {
                throw new NotSupportedException($"Operator {comparisonOperator} is not suported for {property.PropertyType.Name}");
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
                    return $"{propertyName}.Contains(@{index})";
                case "nlike":
                    return $"!{propertyName}.Contains(@{index})";
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
