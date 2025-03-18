using System;

namespace WebApi.ViewModels;

public class ProductVm
{
    public int Id { get; set;}
    public string Name { get; set;}
    public string Description { get; set;}
    public decimal Price { get; set;}
    public bool IsActive {get; set;}
    public int CategoryId { get; set;}
}
