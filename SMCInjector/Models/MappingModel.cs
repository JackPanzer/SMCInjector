using System;
using System.Collections.Generic;
using System.Text;

namespace SMCInjector.Core.Models
{
    public class MappingModel
    {
        public ServiceModel From { get; set; }
        public ServiceModel To { get; set; }
        public String Name { get; set; }
        public String InjectionType { get; set; }
    }
}
