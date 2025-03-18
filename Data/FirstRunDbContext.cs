using System;
using Microsoft.EntityFrameworkCore;

namespace WebApi.Data;

public class FirstRunDbContext : DbContext
{
    public FirstRunDbContext(DbContextOptions<FirstRunDbContext> options) : base(options)
    {

    }
    public DbSet<Entity.Product> Products {get; set;}
    public DbSet<Entity.ProductCategory> ProductCategories { get; set;}
}
