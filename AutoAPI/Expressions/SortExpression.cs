using System;
using System.Collections.Generic;
using System.Text;

namespace AutoAPI.Expressions
{
    public class SortExpression : IExpression<string>
    {
        private string propertyName;
        private string sortOrder;

        public SortExpression(string propertyName, string sortOrder)
        {
            this.propertyName = propertyName;
            this.sortOrder = sortOrder;
        }

        public string Build()
        {
            switch (sortOrder.ToLower())
            {
                case "desc":
                case "1":
                case "descending":
                    return $"{propertyName} desc";
                default:
                    return $"{propertyName} asc";
            }
        }
    }
}
