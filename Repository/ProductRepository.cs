using System;
using WebApi.Data;
using WebApi.Repository.Interface;

namespace WebApi.Repository;

public class ProductRepository : IProductRepository
{
    private readonly FirstRunDbContext dbContext;
    public ProductRepository(FirstRunDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

}
