using System.ComponentModel.DataAnnotations;

namespace TaskManagementBackend.Models
{
    public class Showroom
    {
        [Key]
        public int ShowroomID { get; set; }
        public required string ShowroomName { get; set; }
    }
}
