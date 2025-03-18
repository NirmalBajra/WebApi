using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApi.Entity;
[Table("product_category")]
public class ProductCategory
{
    [Column("id")]
    [Key]
    public int Id { get; set;}
    public string Name { get; set;}
    public bool IsActive { get; set;}
    public DateTime? CreatedAt { get; set;}
}
