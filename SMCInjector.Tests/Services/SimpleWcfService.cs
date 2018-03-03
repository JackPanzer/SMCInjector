using System;
using System.Collections.Generic;
using System.Text;

namespace SMCInjector.Tests.Services
{
    public class SimpleWcfService
    {
        public string Endpoint { get; set; }

        public SimpleWcfService(string binding)
        {
            Endpoint = binding;
        }
    }
}
