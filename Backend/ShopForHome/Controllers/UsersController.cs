using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShopForHome.Api.Data;
using ShopForHome.Api.DTOs;
using ShopForHome.Api.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

[Route("api/[controller]")]
[ApiController]
[Authorize(Roles = "Admin")]
public class UsersController : ControllerBase
{
    private readonly ShopForHomeDbContext _context;

    public UsersController(ShopForHomeDbContext context)
    {
        _context = context;
    }

    // GET: api/Users (Read all)
    [HttpGet]
    public async Task<ActionResult<IEnumerable<UserViewDto>>> GetUsers()
    {
        return await _context.Users
            .Select(u => new UserViewDto { Id = u.Id, Username = u.Username, Role = u.Role })
            .ToListAsync();
    }

    // POST: api/Users (Create)
    [HttpPost]
    public async Task<ActionResult<UserViewDto>> CreateUser(UserCreateDto userDto)
    {
        if (await _context.Users.AnyAsync(u => u.Username == userDto.Username))
        {
            return BadRequest("Username already exists.");
        }
        var user = new User
        {
            Username = userDto.Username,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(userDto.Password),
            Role = userDto.Role
        };
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        var userView = new UserViewDto { Id = user.Id, Username = user.Username, Role = user.Role };
        return CreatedAtAction(nameof(GetUsers), new { id = user.Id }, userView);
    }

    // PUT: api/Users/{id} (Update)
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateUser(int id, UserUpdateDto userDto)
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null) return NotFound();

        user.Username = userDto.Username;
        user.Role = userDto.Role;
        if (!string.IsNullOrEmpty(userDto.Password))
        {
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(userDto.Password);
        }
        await _context.SaveChangesAsync();
        return NoContent();
    }

    // DELETE: api/Users/{id} (Delete)
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteUser(int id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null) return NotFound();
        _context.Users.Remove(user);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}