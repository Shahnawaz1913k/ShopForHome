using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShopForHome.Api.Data;
using ShopForHome.Api.DTOs;
using ShopForHome.Api.Models;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

[Route("api/[controller]")]
[ApiController]
public class ProductsController : ControllerBase
{
    private readonly ShopForHomeDbContext _context;
    public ProductsController(ShopForHomeDbContext context) { _context = context; }

    
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Product>>> GetProducts([FromQuery] string? category, [FromQuery] decimal? maxPrice, [FromQuery] double? minRating, [FromQuery] bool? inStock)
    {
        var query = _context.Products.Include(p => p.Category).AsQueryable();
        if (!string.IsNullOrEmpty(category))
        {
            query = query.Where(p =>
        p.Category.Name.ToLower() == category.ToLower());
        }
        if (maxPrice.HasValue)
        {
            query = query.Where(p =>
            p.Price <= maxPrice.Value);
        }
        if (minRating.HasValue)
        {
            query = query.Where(p =>
            p.Rating >= minRating.Value);
        }
        if (inStock.HasValue && inStock.Value)
        {
            query = query.Where(p =>
            p.StockQuantity > 0);
        }
        return await query.ToListAsync();
    }

    
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<Product>> PostProduct(ProductCreateDto productDto)
    {
        var product = new Product { Name = productDto.Name,
            Description = productDto.Description,
            Price = productDto.Price,
            CategoryId = productDto.CategoryId,
            Rating = productDto.Rating,
            StockQuantity = productDto.StockQuantity };
        _context.Products.Add(product);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetProducts), new { id = product.Id }, product);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> PutProduct(int id, Product product)
    {
        
        if (id != product.Id)
        {
            return BadRequest("ID mismatch between route and body.");
        }

        
        _context.Entry(product).State = EntityState.Modified;

        try
        {
            
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            
            if (!_context.Products.Any(e => e.Id == id))
            {
                return NotFound();
            }
            else
            {
                
                throw;
            }
        }

        
        return NoContent();
    }
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteProduct(int id)
    {
        var product = await _context.Products.FindAsync(id);
        if (product == null)
        {
            return NotFound();
        }

        _context.Products.Remove(product);
        await _context.SaveChangesAsync();

        return NoContent(); // A 204 No Content response is standard for a successful delete
    }

    
    [HttpPost("bulk-upload")]
    [Authorize(Roles = "Admin")]
    
    public async Task<IActionResult> BulkUpload([FromForm] BulkUploadRequestDto request)
    {
        if (request.File == null || request.File.Length == 0)
        {
            return BadRequest("File not selected or empty.");
        }
        
        var newProducts = new List<Product>();
        var config = new CsvConfiguration(CultureInfo.InvariantCulture) 
        { 
            PrepareHeaderForMatch = args => args.Header.ToLower(),
        };
        
        using (var reader = new StreamReader(request.File.OpenReadStream()))
        using (var csv = new CsvReader(reader, config))
        {
            var records = csv.GetRecords<ProductCsvDto>().ToList();

            foreach (var record in records)
            {
                newProducts.Add(new Product 
                { 
                    Name = record.Name, 
                    Description = record.Description, 
                    Price = record.Price, 
                    Rating = record.Rating, 
                    StockQuantity = record.StockQuantity,
                    CategoryId = request.CategoryId // Assign the categoryId from the request DTO
                });
            }
        }
        
        await _context.Products.AddRangeAsync(newProducts);
        await _context.SaveChangesAsync();

        return Ok(new { message = $"{newProducts.Count} products uploaded successfully." });
    }
}

public class BulkUploadRequestDto
{
    public IFormFile File { get; set; }
    public int CategoryId { get; set; }
}