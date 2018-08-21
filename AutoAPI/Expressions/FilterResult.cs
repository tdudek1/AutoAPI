using System;
using System.Collections.Generic;
using System.Text;

namespace AutoAPI.Expressions
{
    public class FilterResult
    {
        public string Filter { get; set; }

        public object [] Values { get; set; }

        public int NextIndex { get; set; }
    }
}
