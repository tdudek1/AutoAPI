using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace AutoAPI
{
    [AttributeUsage(AttributeTargets.Property)]
    public class AutoAPIEntity : Attribute
    {
        ///<summary>
        ///     Relative Route to entity's REST endpoint
        ///</summary>
        public string Route { get; set; }
        ///<summary>
        ///      Policy name for policy based authorization for GET method
        ///</summary>
        public string GETPolicy { get; set; }
        ///<summary>
        ///      Policy name for policy based authorization for POST method
        ///</summary>
        public string POSTPolicy { get; set; }
        ///<summary>
        ///      Policy name for policy based authorization for PUT method
        ///</summary>
        public string PUTPolicy { get; set; }
        ///<summary>
        ///      Policy name for policy based authorization for DELETE method
        ///</summary>
        public string DELETEPolicy { get; set; }
        ///<summary>
        ///      Policy name for policy based authorization for all methods
        ///</summary>
        public string EntityPolicy { get; set; }
        ///<summary>
        ///      Set to true to require user to be logged in
        ///</summary>
        public bool Authorize { get; set; }
    }
}
