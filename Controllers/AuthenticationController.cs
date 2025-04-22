using Microsoft.AspNetCore.Mvc;
using TaskManagementBackend.Data;
using TaskManagementBackend.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;

[Route("api/[controller]")]
[ApiController]

public class AuthenticationController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public AuthenticationController(ApplicationDbContext context)
    {
        _context = context;
    }

    //Hàm check thông tin đăng nhập
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromForm] string accountName, [FromForm] string password)
    {
        // Tìm người dùng theo tên đăng nhập
        var user = await _context.Users.FirstOrDefaultAsync(u => u.UserName == accountName);
        Console.WriteLine(accountName);
        Console.WriteLine(password);

        if (user == null) return NotFound(new { success = false, message = "Tài khoản không tồn tại." });
        if (user.Password != password) return BadRequest(new { success = false, message = "Mật khẩu không đúng." });

        // Trả về thông tin người dùng (trừ mật khẩu) nếu đăng nhập thành công
        return Ok(new
        {
            success = true,
            message = "Đăng nhập thành công.",
            data = new
            {
                user.UserID,
                user.UserName,
                user.FullName,
                user.Email,
                user.PhoneNumber,
                user.DOB
            }
        });
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] User user)
    {
        // Kiểm tra trùng tên đăng nhập
        var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.UserName == user.UserName);
        if (existingUser != null)return BadRequest(new { success = false, message = "Tên đăng nhập đã tồn tại." });

        // Thêm người dùng mới
        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        // Trả về thông tin không bao gồm mật khẩu
        return Ok(new
        {
            success = true,
            message = "Đăng ký thành công.",
            data = new
            {
                user.UserID,
                user.UserName,
                user.FullName,
                user.Email,
                user.PhoneNumber,
                user.DOB
            }
        });
    }


}