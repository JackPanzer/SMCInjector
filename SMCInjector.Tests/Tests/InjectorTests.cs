using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SMCInjector.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace SMCInjector.Tests.Tests
{
    [TestClass]
    public class InjectorTests
    {
        private IServiceCollection services;

        public void Startup(string configFile)
        {
            services = new ServiceCollection();
            services.AddSmcInjection(configFile);
        }
        
        [TestMethod]
        public void IntegrityCheck()
        {
            Startup(@"Config\IntegrityCheck.xml");
            Assert.IsTrue(true);
        }

        [TestMethod]
        public void TypeNotExists()
        {
            try
            {
                Startup(@"Config\TypeNotExists.xml");
                Assert.IsTrue(false);
            }
            catch (Exception ex)
            {
                Assert.IsTrue(ex is SMTException);
            }
        }
    }
}
