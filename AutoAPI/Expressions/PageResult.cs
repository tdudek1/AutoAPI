using System;
using System.Collections.Generic;
using System.Text;

namespace AutoAPI.Expressions
{
    public class PagingResult
    {
        public int Skip { get; set; }
        public int Take { get; set; }
        public int Page { get; set; }
    }
}
