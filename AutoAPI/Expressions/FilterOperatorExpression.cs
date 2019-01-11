using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace AutoAPI.Expressions
{
    public class FilterOperatorExpression
    {
        private PropertyInfo property;
        private readonly string value;
        private int index;
        private readonly string comparisonOperator;


        public FilterOperatorExpression(PropertyInfo property, string value, int index, string comparisonOperator)
        {
            this.property = property;
            this.value = value;
            this.index = index;
            this.comparisonOperator = comparisonOperator;
        }

        public FilterResult Build()
        {
            if (!property.PropertyType.IsOperatorSuported(comparisonOperator))
            {
                throw new NotSupportedException($"Operator {comparisonOperator} is not suported for {property.PropertyType.Name}");
            }

            List<object> valueArray = null;

            if (comparisonOperator == "in")
            {
                var jarray = (JArray)JsonConvert.DeserializeObject(value);

                var temp = new List<object>();
                foreach (var v in jarray)
                {
                    temp.Add(Convert.ChangeType(v, property.PropertyType));
                }
                valueArray = temp;
            }

            return new FilterResult()
            {
                Filter = APIConfiguration.Operators.Where(x => x.Name.Equals(comparisonOperator, StringComparison.InvariantCultureIgnoreCase)).First().Expression.Replace("{propertyName}", property.Name).Replace("{index}", index.ToString()),
                Values = new[] { new List<int> { 1 } ?? Convert.ChangeType(value, property.PropertyType) },
                NextIndex = this.index + 1
            };
        }

    }
}
