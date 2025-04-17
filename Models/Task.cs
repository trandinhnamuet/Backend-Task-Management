using System.ComponentModel.DataAnnotations;

namespace TaskManagementBackend.Models
{
    public class Task
    {
        [Key]
        public int TaskID { get; set; }
        public required string TaskName { get; set; }
        public required string TaskDescription { get; set; }
        public bool IsFinished { get; set; }
        public DateTime CreationTime { get; set; } = DateTime.Now;
        public DateTime? FinishedTime { get; set; }
    }
}
