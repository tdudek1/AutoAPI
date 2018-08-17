using System;
using System.Collections.Generic;
using System.Text;

namespace AutoAPI.Expressions
{
    public interface IExpression<T>
    {
        T Build();
    }
}
