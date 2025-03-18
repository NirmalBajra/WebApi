using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApi.Data;
using WebApi.Entity;
using WebApi.ViewModels;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly FirstRunDbContext dbContext;
        public ProductController(FirstRunDbContext dbContext)
        {
            this.dbContext = dbContext;
        }
        //get the products
        [HttpGet]
        public async Task<IActionResult> GetAllProduct(){
            var products = await dbContext.Products.Include(x=>x.Category).ToListAsync();
            return Ok(products);
        }
        //get single products
        [HttpGet("{id}")]
        public async Task<IActionResult> GetProductById(int Id){
            var products = await dbContext.Products.Include(x=>x.Category).Where(x=> x.Id == Id).ToListAsync();
            return Ok(products);
        }
        //get the products by name
        [HttpGet("search/{name}")]
        public async Task<IActionResult> GetProductByName (string name)
        {
            var products = await dbContext.Products
                .Include(x=>x.Category)
                .Where(x => x.Name.ToLower().Contains(name.ToLower()))
                .ToListAsync();

            if (!products.Any()){
                return NotFound("No products found");
            }
            
            return Ok(products);
        }

        [HttpPost]
        public async Task<IActionResult> AddProduct([FromBody] ProductVm vm)
        {
            if(vm == null){
                return BadRequest("Invalid product Data");
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var existingCategory = await dbContext.ProductCategories.FindAsync(vm.CategoryId);
            if(existingCategory == null)
            {
                return NotFound("Product Category not Found");
            }
            var product = new Product{
                Id = vm.Id,
                Name = vm.Name,
                Description = vm.Description,
                Price = vm.Price,
                IsActive = vm.IsActive,
                CategoryId = vm.CategoryId
            };
            await dbContext.Products.AddAsync(product);
            await dbContext.SaveChangesAsync();
            return CreatedAtAction(nameof(GetProductById),new { id = product.Id},product);
        }

        //Put method to update the products
        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateProduct(int Id,[FromBody] Product updateProduct)
        {
            var product = await dbContext.Products.FirstOrDefaultAsync(x=> x.Id == Id);
            if(product == null){
                return NotFound("Product not Found");
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            product.Name = updateProduct.Name;
            product.Price = updateProduct.Price;
            product.Description = updateProduct.Description;
            product.CategoryId = updateProduct.CategoryId;

            dbContext.Products.Update(product);
            await dbContext.SaveChangesAsync();
            return Ok(product);
        }

        //Delete a product
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var product = await dbContext.Products.FirstOrDefaultAsync(x=> x.Id == id);
            {
                if(product == null){
                    return NotFound("Product not Found");
                }
                dbContext.Products.Remove(product);
                await dbContext.SaveChangesAsync();
                return Ok("Product Deleted");
            }
        }

        //Using patch method
        [HttpPatch("{id}")]
        public async Task<IActionResult> PatchProduct(int id,JsonPatchDocument<Product> patchDoc)
        {
            //Check if the patch doc is null
            if(patchDoc == null)
            {
                return BadRequest();
            }
            //Retrieve the existing item
            var existingItem = await dbContext.Products.FirstOrDefaultAsync(x => x.Id == id);
            if(existingItem == null)
            {
                return NotFound();
            }
            //Apply the patch
            patchDoc.ApplyTo(existingItem);
            
            if(!ModelState.IsValid){
                return BadRequest(ModelState);
            }
            await dbContext.SaveChangesAsync();
            return Ok(existingItem);
        }
    }
}
