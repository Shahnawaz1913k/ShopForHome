namespace ShopForHome.Api.DTOs 
{ 
    public class CartItemDto {
        public int ProductId { get; set; }
        public int Quantity { get; set; } } 
    public class CartViewDto {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; } } 
}