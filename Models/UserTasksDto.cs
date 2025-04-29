namespace TaskManagementBackend.Models
{
    public class UserTasksDto
    {
        public int UserID { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public List<Task> Tasks { get; set; } = new List<Task>();
    }
}