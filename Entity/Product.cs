using System;

namespace WebApi.Entity;

public class Product
{
    public int Id{ get; set;}
    public string Name {get; set;}
    public string Description { get; set;}
    public decimal Price { get; set;}
    public bool IsActive { get; set;}
    public DateTime CreateAt { get; set;} = DateTime.UtcNow;

    public int CategoryId { get; set;}
    public ProductCategory Category { get; set;}
}
