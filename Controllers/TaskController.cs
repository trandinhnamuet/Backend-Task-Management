using Microsoft.AspNetCore.Mvc;
using TaskManagementBackend.Data;
using TaskManagementBackend.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;

[Route("api/[controller]")]
[ApiController]
public class TaskController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public TaskController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<TaskManagementBackend.Models.Task>>> GetTasks()
    {
        return await _context.Tasks.ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<TaskManagementBackend.Models.Task>> GetTask(int id)
    {
        var task = await _context.Tasks.FindAsync(id);
        if (task == null)
            return NotFound();
        return task;
    }

    [HttpGet("filter")]
    public async Task<ActionResult<IEnumerable<TaskManagementBackend.Models.Task>>> FilterTasks(int? userId = null, int? showroomId = null)
    {
        // Gán giá trị mặc định là 0 nếu userId hoặc showroomId là null
        int userIdValue = userId ?? 0;
        int showroomIdValue = showroomId ?? 0;

        // Truy vấn SQL thuần
        var tasks = await _context.Tasks
            .FromSqlRaw(@"
                SELECT DISTINCT Tasks.*
                FROM Tasks
                LEFT JOIN UserTask ON Tasks.TaskID = UserTask.TaskID
                LEFT JOIN UserShowroom ON UserTask.UserID = UserShowroom.UserID
                WHERE (@userId = 0 OR UserTask.UserID = @userId)
                  AND (@showroomId = 0 OR UserShowroom.ShowroomID = @showroomId)",
                new SqlParameter("@userId", userIdValue),
                new SqlParameter("@showroomId", showroomIdValue))
            .ToListAsync();

        if (!tasks.Any())
            return NotFound("Không tìm thấy task nào khớp với tiêu chí.");

        return tasks;
    }

    [HttpPost]
    public async Task<ActionResult<TaskManagementBackend.Models.Task>> CreateTask(TaskManagementBackend.Models.Task task)
    {
        _context.Tasks.Add(task);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetTask), new { id = task.TaskID }, task);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateTask(int id, TaskManagementBackend.Models.Task task)
    {
        if (id != task.TaskID)
            return BadRequest();

        _context.Entry(task).State = EntityState.Modified;
        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTask(int id)
    {
        var task = await _context.Tasks.FindAsync(id);
        if (task == null)
            return NotFound();

        _context.Tasks.Remove(task);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}
