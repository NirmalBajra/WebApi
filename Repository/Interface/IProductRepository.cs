using System;
using WebApi.ViewModels;

namespace WebApi.Repository.Interface;

public interface IProductRepository
{
    Task<IEnumerable<ProductVm>> GetProducts();
    Task<ProductVm> GetProductById(int id);
    Task<IEnumerable<ProductVm>> SearchProduct(string item);
}
