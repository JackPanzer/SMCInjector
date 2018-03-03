using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace SMCInjector.Core.Models
{
    public class ServiceModel
    {
        public String Alias { get; set; }
        public Type Class { get; set; }
        public Assembly Assembly { get; set; }
    }
    
}
