

namespace ShopForHome.Api.Models
{
    public class Coupon
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal MinimumSpend { get; set; }
        public DateTime ExpirationDate { get; set; }
        public ICollection<UserCoupon> UserCoupons { get; set; }
    }
}