using System;
using System.Collections.Generic;
using System.Text;

namespace SMCInjector.Core.Models
{
    public class ConstructorModel
    {
        public String MapName { get; set; }
        public ICollection<ParamModel> Params { get; set; }
    }
}
