using System;
using System.Collections.Generic;
using System.Text;

namespace SMCInjector.Core.Models
{
    public class SMTException : System.Exception
    {
        public SMTException(string msg) : base(msg)
        {

        }
    }
}
