using System;
using System.Collections.Generic;
using System.Linq;

namespace AutoAPI.Expressions
{
    public class IncludeExpression : IExpression<List<string>>
    {
        APIEntity aPIEntity;
        string values;
        public IncludeExpression(APIEntity aPIEntity, string values)
        {
            this.aPIEntity = aPIEntity;
            this.values = values;
        }


        public List<string> Build()
        {
            var items = values.Split(",", StringSplitOptions.RemoveEmptyEntries).ToList();
            items = items.Intersect(aPIEntity.NavigationProperties.Select(x => x.Name), StringComparer.InvariantCultureIgnoreCase).ToList();

            return aPIEntity.NavigationProperties.Where(x => items.Contains(x.Name.ToLower(), StringComparer.InvariantCultureIgnoreCase)).Select(x => x.Name).ToList();
        }
    }
}
