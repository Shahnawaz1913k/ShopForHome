using Microsoft.EntityFrameworkCore;
using ShopForHome.Api.Models;

namespace ShopForHome.Api.Data
{
    public class ShopForHomeDbContext : DbContext
    {
        public ShopForHomeDbContext(DbContextOptions<ShopForHomeDbContext> options) : base(options) { }

        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<WishlistItem> WishlistItems { get; set; }
        public DbSet<Coupon> Coupons { get; set; }
        public DbSet<UserCoupon> UserCoupons { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            // This configures the composite primary key for the UserCoupon join table
            modelBuilder.Entity<UserCoupon>()
                .HasKey(uc => new { uc.UserId, uc.CouponId });
        }
    }
}