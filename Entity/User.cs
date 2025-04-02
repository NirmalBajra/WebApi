using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace WebApi.Entity;

public class User 
{
    [Key]
    [Column("id")]
    public int Id {get;set;}

    [Required]
    [StringLength(50)]
    public string UserName { get; set; }

    [Required]
    [EmailAddress]
    public string Email { get; set; }

    [StringLength(200)]
    public string? Address { get; set;}

    [Phone]
    [StringLength(15)]
    public string Phone { get; set;}

    [Required]
    [MinLength(6, ErrorMessage ="Password must be 6 character long.")]
    public string Password { get; set; }

    public string Salt { get; set;}

    
    public DateTime CreateAt { get; set;} = DateTime.UtcNow;
}
