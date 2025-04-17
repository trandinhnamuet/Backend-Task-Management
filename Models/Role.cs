using System.ComponentModel.DataAnnotations;

namespace TaskManagementBackend.Models
{
    public class Role
    {
        [Key]
        public int RoleID { get; set; }
        public required string RoleName { get; set; }
    }
}
