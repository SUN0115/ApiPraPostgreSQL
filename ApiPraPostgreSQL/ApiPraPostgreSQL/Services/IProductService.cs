// Services/IProductService.cs
using System.Collections.Generic;
using System.Threading.Tasks;
using ApiPraPostgreSQL.Models;

namespace ApiPraPostgreSQL.Services
{
    public interface IProductService
    {
        Task<IEnumerable<Product>> GetAllProductsAsync();
        Task<Product> GetProductByIdAsync(int id);
        Task<bool> CreateProductAsync(Product product);
        Task<bool> UpdateProductAsync(int id, Product product);
        Task<bool> DeleteProductAsync(int id);
    }
}