using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShopForHome.Api.Data;
using ShopForHome.Api.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using System.Linq;
using System.Security.Claims;
using System.Text.Json;

[Route("api/[controller]")]
[ApiController]
[Authorize(Roles = "Admin")] 
public class CouponsController : ControllerBase
{
    private readonly ShopForHomeDbContext _context;

    public CouponsController(ShopForHomeDbContext context)
    {
        _context = context;
    }

    // GET: api/Coupons (Admin Only)
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Coupon>>> GetCoupons()
    {
        return await _context.Coupons.ToListAsync();
    }

    // POST: api/Coupons (Admin Only)
    [HttpPost]
    public async Task<ActionResult<Coupon>> CreateCoupon([FromBody] CouponCreateDto couponDto)
    {
        var coupon = new Coupon
        {
            Code = couponDto.Code,
            DiscountAmount = couponDto.DiscountAmount,
            MinimumSpend = couponDto.MinimumSpend,
            ExpirationDate = couponDto.ExpirationDate
        };
        _context.Coupons.Add(coupon);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetCoupons), new { id = coupon.Id }, coupon);
    }

    // POST: api/Coupons/assign (Admin Only)
    [HttpPost("assign")]
    public async Task<IActionResult> AssignCoupon([FromBody] AssignCouponRequest request)
    {
        var coupon = await _context.Coupons.FirstOrDefaultAsync(c => c.Code == request.CouponCode);
        if (coupon == null) return NotFound("Coupon not found.");

        var assignments = new List<UserCoupon>();
        foreach (var userId in request.UserIds)
        {
            var exists = await _context.UserCoupons.AnyAsync(uc => uc.UserId == userId && uc.CouponId == coupon.Id);
            if (!exists)
            {
                assignments.Add(new UserCoupon { UserId = userId, CouponId = coupon.Id });
            }
        }
        await _context.UserCoupons.AddRangeAsync(assignments);
        await _context.SaveChangesAsync();
        return Ok(new { message = $"Coupon '{request.CouponCode}' assigned to {request.UserIds.Count} users." });
    }

    
    [HttpPost("validate")]
    [Authorize] 
    public async Task<IActionResult> ValidateCoupon([FromBody] JsonElement payload)
    {
        var userId = GetCurrentUserId();
        if (userId == null) return Unauthorized();

        if (!payload.TryGetProperty("couponCode", out var codeElement) || !payload.TryGetProperty("subtotal", out var subtotalElement))
        {
            return BadRequest(new { message = "Missing coupon code or subtotal." });
        }
        
        string couponCode = codeElement.GetString();
        decimal subtotal = subtotalElement.GetDecimal();
        
        var coupon = await _context.Coupons.FirstOrDefaultAsync(c => c.Code.ToUpper() == couponCode.ToUpper());

        if (coupon == null || coupon.ExpirationDate < DateTime.UtcNow || subtotal < coupon.MinimumSpend)
        {
            return NotFound(new { message = "Coupon is invalid or not applicable." });
        }

        var isAssignedToUser = await _context.UserCoupons.AnyAsync(uc => uc.CouponId == coupon.Id && uc.UserId == userId);

        if (!isAssignedToUser)
        {
            return NotFound(new { message = "Coupon is not valid for this user." });
        }

        return Ok(coupon);
    }

    private int? GetCurrentUserId()
    {
        var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
        if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId)) { return userId; }
        return null;
    }
}


public class CouponCreateDto
{
    public string Code { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal MinimumSpend { get; set; }
    public DateTime ExpirationDate { get; set; }
}

public class AssignCouponRequest
{
    public string CouponCode { get; set; }
    public List<int> UserIds { get; set; }
}