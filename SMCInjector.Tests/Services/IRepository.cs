using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SMCInjector.Tests.Services
{
    public interface IRepository<T> 
        where T : class
    {
        IQueryable<T> Elements { get; set; }
        IQueryable<T> GetAllElements();
    }
}
