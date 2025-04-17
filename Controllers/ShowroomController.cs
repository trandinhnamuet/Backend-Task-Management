using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskManagementBackend.Data;
using TaskManagementBackend.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

[Route("api/[controller]")]
[ApiController]
public class ShowroomController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public ShowroomController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Showroom>>> GetShowrooms()
    {
        return await _context.Showrooms.ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Showroom>> GetShowroom(int id)
    {
        var showroom = await _context.Showrooms.FindAsync(id);
        if (showroom == null) return NotFound();
        return showroom;
    }

    [HttpPost]
    public async Task<ActionResult<Showroom>> CreateShowroom(Showroom showroom)
    {
        _context.Showrooms.Add(showroom);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetShowroom), new { id = showroom.ShowroomID }, showroom);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateShowroom(int id, Showroom showroom)
    {
        if (id != showroom.ShowroomID) return BadRequest();
        _context.Entry(showroom).State = EntityState.Modified;
        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteShowroom(int id)
    {
        var showroom = await _context.Showrooms.FindAsync(id);
        if (showroom == null) return NotFound();
        _context.Showrooms.Remove(showroom);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}
