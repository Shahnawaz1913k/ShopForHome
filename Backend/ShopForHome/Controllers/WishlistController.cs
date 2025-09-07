using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShopForHome.Api.Data;
using ShopForHome.Api.Models;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class WishlistController : ControllerBase
{
    private readonly ShopForHomeDbContext _context;
    public WishlistController(ShopForHomeDbContext context) { _context = context; }

    [HttpGet]
    public async Task<IActionResult> GetWishlist()
    {
        var userId = GetCurrentUserId();
        var wishlistItems = await _context.WishlistItems.Where(wi => wi.UserId == userId).Include(wi => wi.Product).Select(wi => wi.Product).ToListAsync();
        return Ok(wishlistItems);
    }

    [HttpPost("{productId}")]
    public async Task<IActionResult> AddToWishlist(int productId)
    {
        var userId = GetCurrentUserId();
        if (!await _context.Products.AnyAsync(p =>
        p.Id == productId))
            return NotFound("Product not found.");
        if (await _context.WishlistItems.AnyAsync(wi =>
        wi.UserId == userId && wi.ProductId == productId))
            return Ok("Item is already in your wishlist.");
        var wishlistItem = new WishlistItem {
            UserId = (int)userId,
            ProductId = productId };
        _context.WishlistItems.Add(wishlistItem);
        await _context.SaveChangesAsync();
        return Ok("Item added to wishlist.");
    }

    [HttpDelete("{productId}")]
    public async Task<IActionResult> RemoveFromWishlist(int productId)
    {
        var userId = GetCurrentUserId();
        var wishlistItem = await _context.WishlistItems.FirstOrDefaultAsync(wi =>
        wi.UserId == userId && wi.ProductId == productId);
        if (wishlistItem == null)
            return NotFound("Item not found in wishlist.");
        _context.WishlistItems.Remove(wishlistItem);
        await _context.SaveChangesAsync();
        return Ok("Item removed from wishlist.");
    }

    private int? GetCurrentUserId()
    {
        var userIdClaim = User.Claims.FirstOrDefault(c =>
    c.Type == ClaimTypes.NameIdentifier);
        if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId))
        {
            return userId;
        }
        return null;
    }
}