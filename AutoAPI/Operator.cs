using System;
using System.Collections.Generic;
using System.Text;

namespace AutoAPI
{
    public class Operator
    {
        public string Name { get; set; }
        public string Expression { get; set; }
        public bool SupportsString { get; set; }
        public bool SupportsValueType { get; set; }
        public bool SupportsGuid { get; set; }
    }
}
