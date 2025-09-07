using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

[Route("api/[controller]")]
[ApiController]
[Authorize(Roles = "Admin")]
public class SalesReportsController : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetSalesReport([FromQuery] DateTime startDate,
    [FromQuery] DateTime endDate)
    {
        
        var reportData = new {
            StartDate = startDate,
            EndDate = endDate,
            TotalSales = 150000, 
            TotalOrders = 42
        };
        return Ok(reportData);
    }
}