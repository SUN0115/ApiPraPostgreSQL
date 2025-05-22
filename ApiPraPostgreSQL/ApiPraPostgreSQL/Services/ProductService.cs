// Services/ProductService.cs
using System.Collections.Generic;
using System.Threading.Tasks;
using ApiPraPostgreSQL.Models;
using ApiPraPostgreSQL.Repositories;
using ApiPraPostgreSQL.Services;
using Microsoft.EntityFrameworkCore;

namespace ApiPraPostgreSQL.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;

        public ProductService(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public async Task<IEnumerable<Product>> GetAllProductsAsync()
        {
            return await _productRepository.GetAllAsync();
        }

        public async Task<Product> GetProductByIdAsync(int id)
        {
            return await _productRepository.GetByIdAsync(id);
        }

        public async Task<bool> CreateProductAsync(Product product)
        {
            await _productRepository.AddAsync(product);
            return true;
        }

        public async Task<bool> UpdateProductAsync(Product product)
        {
            var existingProduct = await _productRepository.GetByIdAsync(product.id);
            if (existingProduct == null)
            {
                return false;
            }
            existingProduct.name = product.name;
            existingProduct.price = product.price;

            await _productRepository.UpdateAsync(existingProduct);
            return true;
        }

        public async Task<bool> DeleteProductAsync(int id)
        {
            await _productRepository.DeleteAsync(id);
            return true;
        }
    }
}