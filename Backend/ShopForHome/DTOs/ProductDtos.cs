namespace ShopForHome.Api.DTOs 
{ 
    public class ProductCreateDto { public string Name { get; set; } public string Description { get; set; } public decimal Price { get; set; } public int CategoryId { get; set; } public double Rating { get; set; } public int StockQuantity { get; set; } }
    public class ProductCsvDto { public string Name { get; set; } public string Description { get; set; } public decimal Price { get; set; } public double Rating { get; set; } public int StockQuantity { get; set; } public int CategoryId { get; set; } }
}