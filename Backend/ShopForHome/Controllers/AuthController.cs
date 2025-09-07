using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using ShopForHome.Api.Data;
using ShopForHome.Api.DTOs;
using ShopForHome.Api.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly ShopForHomeDbContext _context;
    private readonly IConfiguration _configuration;
    public AuthController(ShopForHomeDbContext context, IConfiguration configuration) { _context = context; _configuration = configuration; }

   
    [HttpPost("register")]
    public async Task<ActionResult<User>> Register(UserCreateDto request)
    {
        if (await _context.Users.AnyAsync(u => u.Username == request.Username)) { return BadRequest("Username already exists."); }
        string passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);
        var user = new User { Username = request.Username, PasswordHash = passwordHash, Role = request.Role };
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        return Ok(user);
    }

    [HttpPost("login")]
    public async Task<ActionResult<string>> Login(UserLoginDto request) 
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == request.Username);
        if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
        {
            return BadRequest("Invalid credentials.");
        }

        string token = CreateToken(user);
        return Ok(token);
    }

    private string CreateToken(User user)
    {
        var claims = new List<Claim> {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name", user.Username),
            new Claim("http://schemas.microsoft.com/ws/2008/06/identity/claims/role", user.Role)
        };
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.GetSection("Jwt:Key").Value!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var token = new JwtSecurityToken(issuer: _configuration.GetSection("Jwt:Issuer").Value, audience: _configuration.GetSection("Jwt:Audience").Value, claims: claims, expires: DateTime.Now.AddDays(1), signingCredentials: creds);
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}