using System.ComponentModel.DataAnnotations;

namespace TaskManagementBackend.Models
{
    public class User
    {
        [Key]
        public int UserID { get; set; }
        public required string UserName { get; set; }
        public required string FullName { get; set; }
        public required string Password { get; set; }
        public required string Email { get; set; }
        public required string PhoneNumber { get; set; }
        public DateTime DOB { get; set; }
    }
}
