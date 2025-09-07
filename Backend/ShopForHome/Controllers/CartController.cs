using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShopForHome.Api.Data;
using ShopForHome.Api.DTOs;
using ShopForHome.Api.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

[Route("api/[controller]")]
[ApiController]
[Authorize] // All actions require a user to be logged in
public class CartController : ControllerBase
{
    private readonly ShopForHomeDbContext _context;
    private readonly IConfiguration _configuration;

    public CartController(ShopForHomeDbContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }

    // GET: api/Cart
    // Gets all items in the current user's cart.
    [HttpGet]
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public async Task<ActionResult<IEnumerable<CartViewDto>>> GetCartItems()
    {
        var cart = await GetUserCart();
        if (cart == null)
        {
            return Ok(new List<CartViewDto>());
        }

        var cartItemsDto = cart.Items.Select(item => new CartViewDto
        {
            ProductId = item.ProductId,
            ProductName = item.Product.Name,
            Price = item.Product.Price,
            Quantity = item.Quantity
        }).ToList();

        return Ok(cartItemsDto);
    }

    // GET: api/Cart/summary
    // This endpoint calculates the subtotal, delivery charge, and grand total.
   [HttpGet("summary")]
public async Task<ActionResult<CartSummaryDto>> GetCartSummary([FromQuery] string? couponCode)
{
    var cart = await GetUserCart();
    var summary = new CartSummaryDto();

    if (cart == null || !cart.Items.Any())
    {
        return Ok(summary);
    }

    summary.Subtotal = cart.Items.Sum(item => item.Product.Price * item.Quantity);
    
    //[cite_start]// As a User, I should receive different discount coupons for promotions or sales events. [cite: 28]
    if (!string.IsNullOrEmpty(couponCode))
    {
        var userId = GetCurrentUserId();
        var coupon = await _context.Coupons.FirstOrDefaultAsync(c => c.Code.ToUpper() == couponCode.ToUpper());
        
        if (coupon != null && coupon.ExpirationDate >= DateTime.UtcNow && summary.Subtotal >= coupon.MinimumSpend)
        {
            var isAssignedToUser = await _context.UserCoupons.AnyAsync(uc => uc.CouponId == coupon.Id && uc.UserId == userId);
            if (isAssignedToUser)
            {
                summary.CouponDiscount = coupon.DiscountAmount;
                summary.AppliedCouponCode = coupon.Code;
            }
        }
    }
    
    var totalAfterDiscount = summary.Subtotal - summary.CouponDiscount;
    
    if (totalAfterDiscount > 0 && totalAfterDiscount < 999)
    {
        summary.DeliveryCharge = 49;
    }

    summary.GrandTotal = totalAfterDiscount + summary.DeliveryCharge;
    if (summary.GrandTotal < 0) { summary.GrandTotal = 0; }

    return Ok(summary);
}

    
    [HttpPost("add")]
    public async Task<ActionResult> AddToCart(CartItemDto itemDto)
    {
        var cart = await GetOrCreateUserCart();
        var cartItem = await _context.CartItems.FirstOrDefaultAsync(ci => ci.CartId == cart.Id && ci.ProductId == itemDto.ProductId);

        if (cartItem != null) { cartItem.Quantity += itemDto.Quantity; }
        else { cartItem = new CartItem { CartId = cart.Id, ProductId = itemDto.ProductId, Quantity = itemDto.Quantity }; _context.CartItems.Add(cartItem); }
        
        await _context.SaveChangesAsync();
        return Ok(new { message = "Item added to cart successfully." });
    }
    
    
    [HttpPost("update-quantity")]
    public async Task<ActionResult> UpdateQuantity(CartItemDto itemDto)
    {
        var cart = await GetUserCart();
        if (cart == null) return NotFound("Cart not found.");
        var cartItem = cart.Items.FirstOrDefault(ci => ci.ProductId == itemDto.ProductId);
        if (cartItem == null) return NotFound("Item not in cart.");

        if (itemDto.Quantity <= 0) { _context.CartItems.Remove(cartItem); }
        else { cartItem.Quantity = itemDto.Quantity; }
        
        await _context.SaveChangesAsync();
        return Ok(new { message = "Cart updated successfully." });
    }

    // DELETE: api/Cart/{productId}
    [HttpDelete("{productId}")]
    public async Task<ActionResult> RemoveFromCart(int productId)
    {
        var cart = await GetUserCart();
        if (cart == null) return NotFound("Cart not found.");
        var cartItem = cart.Items.FirstOrDefault(ci => ci.ProductId == productId);
        if (cartItem == null) return NotFound("Item not in cart.");
        _context.CartItems.Remove(cartItem);
        await _context.SaveChangesAsync();
        return Ok(new { message = "Item removed from cart successfully." });
    }

    //  HELPER METHODS 
    private async Task<Cart?> GetUserCart()
    {
        var userId = GetCurrentUserId();
        if (userId == null) return null;
        return await _context.Carts.Include(c => c.Items).ThenInclude(ci => ci.Product).FirstOrDefaultAsync(c => c.UserId == userId);
    }

    private async Task<Cart> GetOrCreateUserCart()
    {
        var userId = GetCurrentUserId();
        if (userId == null) throw new System.Exception("User not found.");
        var cart = await _context.Carts.FirstOrDefaultAsync(c => c.UserId == userId);
        if (cart == null)
        {
            cart = new Cart { UserId = (int)userId };
            _context.Carts.Add(cart);
            await _context.SaveChangesAsync();
        }
        return cart;
    }

    private int? GetCurrentUserId()
    {
        var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
        if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId))
        {
            return userId;
        }
        return null;
    }
}