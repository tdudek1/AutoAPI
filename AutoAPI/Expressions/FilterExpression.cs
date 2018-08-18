using System;
using System.Reflection;

namespace AutoAPI.Expressions
{
    public class FilterExpression : IExpression<FilterResult>
    {
        private PropertyInfo property;
        private string value;
        private int index;

        public FilterExpression(PropertyInfo property, string value, int index)
        {
            this.property = property;
            this.value = value;
            this.index = index;
        }

        public FilterResult Build()
        {
            return new FilterResult()
            {
                Filter = $"{property.Name} == @{index}",
                Values = new[] { Convert.ChangeType(value, property.PropertyType) },
                NextIndex = this.index + 1
            };
        }
    }
}
