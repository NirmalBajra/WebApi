using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WebApi.Data;
using WebApi.Repository.Interface;
using WebApi.ViewModels;
using WebApi.Models;
using Microsoft.AspNetCore.Http.HttpResults; // Assuming you have a Product entity

namespace WebApi.Repository
{
    public class ProductRepository : IProductRepository
    {
        private readonly FirstRunDbContext dbContext;

        public ProductRepository(FirstRunDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<IEnumerable<ProductVm>> GetProducts()
        {
            var product = await dbContext.Products
                .Select(p => new ProductVm
                {
                    Id = p.Id,
                    Name = p.Name,
                    Price = p.Price,
                    Description = p.Description
                })
                .ToListAsync();
            return product;
        }

        public async Task<ProductVm> GetProductById(int id)
        {
            var product = await dbContext.Products
                .Where(p => p.Id == id)
                .Select(p => new ProductVm
                {
                    Id = p.Id,
                    Name = p.Name,
                    Price = p.Price,
                    Description = p.Description
                })
                .FirstOrDefaultAsync();
            
            return product ?? new ProductVm();
        }

        public async Task<IEnumerable<ProductVm>> SearchProduct(string item)
        {
            if(string.IsNullOrWhiteSpace(item)){
                return new List<ProductVm>();
            }

            var product = await dbContext.Products
                .Where(p => p.Name.Contains(item))
                .Select(p => new ProductVm
                {
                    Id = p.Id,
                    Name = p.Name,
                    Price = p.Price,
                    Description = p.Description
                })
                .ToListAsync();
            return product;
        }
    }
}
