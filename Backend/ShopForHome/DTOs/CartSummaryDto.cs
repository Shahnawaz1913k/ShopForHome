namespace ShopForHome.Api.DTOs
{
    public class CartSummaryDto
    {
        public decimal Subtotal { get; set; }
        public decimal CouponDiscount { get; set; }
        public string? AppliedCouponCode { get; set; } 
        public decimal DeliveryCharge { get; set; }
        public decimal GrandTotal { get; set; }
    }
}