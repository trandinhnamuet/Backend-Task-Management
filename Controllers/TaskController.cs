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

        return tasks;
    }

    [HttpGet("by-role/{userId}")]
public async Task<ActionResult<IEnumerable<UserTasksDto>>> GetTasksByRole(int userId)
{
    // 1. Lấy RoleName của user dựa trên userId
    var roleName = await _context.Database
        .SqlQueryRaw<string>(
            @"SELECT Roles.RoleName AS Value 
              FROM Users 
              JOIN UserRole ON Users.UserID = UserRole.UserID 
              JOIN Roles ON UserRole.RoleID = Roles.RoleID 
              WHERE Users.UserID = @userId",
            new SqlParameter("@userId", userId))
        .FirstOrDefaultAsync();

    if (string.IsNullOrEmpty(roleName))
        return NotFound("Không tìm thấy người dùng hoặc vai trò");

    // 2. Lấy danh sách user và task dựa trên RoleName
    if (roleName == "CEO")
    {
        // CEO: Lấy tất cả user khác và task của họ
        var userTasks = await _context.Users
            .FromSqlRaw(
                @"SELECT DISTINCT Users.* 
                  FROM Users 
                  JOIN UserTask ON Users.UserID = UserTask.UserID 
                  WHERE Users.UserID != @userId",
                new SqlParameter("@userId", userId))
            .ToListAsync();

        var result = new List<UserTasksDto>();
        foreach (var user in userTasks)
        {
            var tasks = await _context.Tasks
                .FromSqlRaw(
                    @"SELECT Tasks.* 
                      FROM Tasks 
                      JOIN UserTask ON Tasks.TaskID = UserTask.TaskID 
                      WHERE UserTask.UserID = @userId",
                    new SqlParameter("@userId", user.UserID))
                .ToListAsync();

            result.Add(new UserTasksDto
            {
                UserID = user.UserID,
                UserName = user.UserName,
                FullName = user.FullName,
                Tasks = tasks
            });
        }

        return result;
    }
    else if (roleName == "Boss")
    {
        // Boss: Lấy user có vai trò Accountant và SaleAgent, cùng task của họ
        var userTasks = await _context.Users
            .FromSqlRaw(
                @"SELECT DISTINCT Users.* 
                  FROM Users 
                  JOIN UserTask ON Users.UserID = UserTask.UserID 
                  JOIN UserRole ON Users.UserID = UserRole.UserID 
                  JOIN Roles ON UserRole.RoleID = Roles.RoleID 
                  WHERE Roles.RoleName IN ('Accountant', 'SaleAgent')",
                new SqlParameter("@userId", userId))
            .ToListAsync();

        var result = new List<UserTasksDto>();
        foreach (var user in userTasks)
        {
            var tasks = await _context.Tasks
                .FromSqlRaw(
                    @"SELECT Tasks.* 
                      FROM Tasks 
                      JOIN UserTask ON Tasks.TaskID = UserTask.TaskID 
                      WHERE UserTask.UserID = @userId",
                    new SqlParameter("@userId", user.UserID))
                .ToListAsync();

            result.Add(new UserTasksDto
            {
                UserID = user.UserID,
                UserName = user.UserName,
                FullName = user.FullName,
                Tasks = tasks
            });
        }

        return result;
    }
    else
    {
        return new List<UserTasksDto>();
    }
}

    [HttpPost]
    public async Task<ActionResult<TaskManagementBackend.Models.Task>> CreateTask(TaskManagementBackend.Models.Task task)
    {
        _context.Tasks.Add(task);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetTask), new { id = task.TaskID }, task);
    }

    public class TaskWithUser
    {
        public TaskManagementBackend.Models.Task Task { get; set; } = null!;
        public int UserId { get; set; }
    }


    [HttpPost("create-with-user")]
    public async Task<IActionResult> CreateTaskWithUser([FromBody] TaskWithUser request)
    {
        if (request.Task == null || request.UserId <= 0)
            return BadRequest("Thông tin không hợp lệ");

        // 1. Tạo task mới
        _context.Tasks.Add(request.Task);
        await _context.SaveChangesAsync();

        // 2. Lấy TaskID vừa tạo
        int newTaskId = request.Task.TaskID;

        // 3. Thêm vào bảng UserTask bằng SQL
        var sql = @"
        INSERT INTO UserTask (UserID, TaskID)
        VALUES (@userId, @taskId)";

        await _context.Database.ExecuteSqlRawAsync(sql,
            new SqlParameter("@userId", request.UserId),
            new SqlParameter("@taskId", newTaskId));

        // 4. Trả về kết quả
        return CreatedAtAction(nameof(GetTask), new { id = newTaskId }, request.Task);
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

        // Xóa các bản ghi trong UserTask liên quan đến TaskID bằng SQL thuần
        await _context.Database.ExecuteSqlRawAsync("DELETE FROM UserTask WHERE TaskID = {0}", id);

        // Xóa bản ghi trong Tasks
        _context.Tasks.Remove(task);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}
