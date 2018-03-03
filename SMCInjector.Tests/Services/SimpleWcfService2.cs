using System;
using System.Collections.Generic;
using System.Text;

namespace SMCInjector.Tests.Services
{
    public class SimpleWcfService2
    {
        public string Endpoint { get; set; }
        public int NumRequests { get; set; }
        public double Ttl { get; set; }

        public SimpleWcfService2(string binding, int numRequests, double ttl)
        {
            Endpoint = binding;
            NumRequests = numRequests;
            Ttl = ttl;
        }
    }
}
