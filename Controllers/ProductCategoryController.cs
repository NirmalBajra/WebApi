using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApi.Data;
using WebApi.Entity;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductCategoryController : ControllerBase
    {
        private readonly FirstRunDbContext dbContext;
        public ProductCategoryController(FirstRunDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        [HttpGet]
        public async Task<IActionResult> GetProductCategory()
        {
            var productCategories = await dbContext.ProductCategories.ToListAsync();
            return Ok(productCategories);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetProductCategoryById(int id)
        {
            var productCategories = await dbContext.ProductCategories.FirstOrDefaultAsync(x=> x.Id == id);

            if(productCategories == null)
            {
                return NotFound("Product Category Not Found.");
            }
            return Ok(productCategories);
        }   

        [HttpPost]     
        public async Task<IActionResult> AddProductCategory([FromBody] ProductCategory productCategory){
            if(productCategory == null){
                return BadRequest("Invalid Product Category data");
            }
            await dbContext.ProductCategories.AddAsync(productCategory);
            await dbContext.SaveChangesAsync();
            return CreatedAtAction(nameof(GetProductCategoryById),new {id = productCategory.Id},productCategory);
        }

        //Delete Product_Category
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProductCategory(int id)
        {
            var productCategories = await dbContext.ProductCategories.FirstOrDefaultAsync(x=> x.Id == id);
            if(productCategories == null){
                return NotFound("Product Not Found");
            }

            dbContext.ProductCategories.Remove(productCategories);
            await dbContext.SaveChangesAsync();
            return Ok("Product Category Deleted Successfully");
        }
        
        //Update Product Category
        [HttpPut]
        public async Task<IActionResult> UpdateProductCategory(int id,[FromBody] ProductCategory productCategory){
            var existingCategory = await dbContext.ProductCategories.FirstOrDefaultAsync(x=> x.Id == id);
            if(existingCategory == null)
            {
                return NotFound("Product Category not Found.");
            }
            existingCategory.Name = productCategory.Name;
            
            dbContext.ProductCategories.Update(existingCategory);
            await dbContext.SaveChangesAsync();
            return Ok(existingCategory);
        }   
    }
}
