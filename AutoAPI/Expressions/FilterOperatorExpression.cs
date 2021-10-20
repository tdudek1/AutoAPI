using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

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

            object list = null;

            if ((new string[] { "in", "nin" }).Contains(comparisonOperator))
            {
                var listType = typeof(List<>).MakeGenericType(new Type[] { property.PropertyType });

                list = JsonSerializer.Deserialize(value, listType);
            }

            var type = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;

            return new FilterResult()
            {
                Filter = APIConfiguration.Operators.Where(x => x.Name.Equals(comparisonOperator, StringComparison.InvariantCultureIgnoreCase)).First().Expression.Replace("{propertyName}", property.Name).Replace("{index}", index.ToString()),
                Values = new[] { list ?? System.ComponentModel.TypeDescriptor.GetConverter(type).ConvertFromString(value) },
                NextIndex = this.index + 1
            };
        }

        private void AddItems<T>(List<T> list, IList<string> jarray)
        {
            foreach (var i in jarray)
            {
                list.Add((T)Convert.ChangeType(i, typeof(T)));
            }
        }

    }
}
