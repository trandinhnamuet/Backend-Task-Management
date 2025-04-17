using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskManagementBackend.Data;
using TaskManagementBackend.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

[Route("api/[controller]")]
[ApiController]
public class RoleController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public RoleController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Role>>> GetRoles()
    {
        return await _context.Roles.ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Role>> GetRole(int id)
    {
        var role = await _context.Roles.FindAsync(id);
        if (role == null) return NotFound();
        return role;
    }

    [HttpPost]
    public async Task<ActionResult<Role>> CreateRole(Role role)
    {
        _context.Roles.Add(role);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetRole), new { id = role.RoleID }, role);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateRole(int id, Role role)
    {
        if (id != role.RoleID) return BadRequest();
        _context.Entry(role).State = EntityState.Modified;
        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteRole(int id)
    {
        var role = await _context.Roles.FindAsync(id);
        if (role == null) return NotFound();
        _context.Roles.Remove(role);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}
