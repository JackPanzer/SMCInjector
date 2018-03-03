using SMCInjector.Tests.Model;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SMCInjector.Tests.Services
{
    public interface IProductService
    {
        IEnumerable<Product> GetAllProducts();
        Task<IEnumerable<Product>> GetAllProductsAsync();

        Product GetProductById(int id);
        Task<Product> GetProductByIdAsync(int id);
    }
}
