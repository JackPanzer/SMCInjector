using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SMCInjector.Tests.Model;

namespace SMCInjector.Tests.Services
{
    public class ProductServiceMock : IProductService
    {
        private IRepository<Product> _repository;

        public ProductServiceMock(IRepository<Product> repository)
        {
            this._repository = repository;

            var elements = new List<Product>
            {
                new Product { Id = 0, Name = "Bocata de jamón", Price = 2.50 },
                new Product { Id = 0, Name = "Bocata de pavo", Price = 1.50 },
                new Product { Id = 0, Name = "Bocata de chorizo", Price = 2.00 }
            };

            _repository.Elements = elements.AsQueryable();
        }

        public IEnumerable<Product> GetAllProducts()
        {
            var task = GetAllProductsAsync();
            task.Wait();

            return task.Result;
        }

        public Task<IEnumerable<Product>> GetAllProductsAsync()
        {
            return Task.Run(() => _repository.GetAllElements().AsEnumerable());
        }

        public Product GetProductById(int id)
        {
            var task = GetProductByIdAsync(id);
            task.Wait();

            return task.Result;
        }

        public async Task<Product> GetProductByIdAsync(int id)
        {
            return (await GetAllProductsAsync()).FirstOrDefault(prod => prod.Id.Equals(id));
        }
    }
}
