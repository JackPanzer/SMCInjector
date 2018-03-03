using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SMCInjector.Tests.Services
{
    public class RepositoryMock<T> : IRepository<T>
        where T : class
    {
        public IQueryable<T> GetAllElements()
        {
            return Elements.AsQueryable();
        }
        
        public IQueryable<T> Elements { get; set; }
    }
}
