using HridhayConnect_API.Infra;
using System.ComponentModel.DataAnnotations.Schema;

namespace HridhayConnect_API.Models
{
    public class User : EntityBase
    {
        public long Id { get; set; }
        public string? UserName { get; set; }
        public string? Password { get; set; }
        public bool? IsActive { get; set; }
        public bool? IsDeleted { get; set; }
        public string? Email { get; set; }
        public string? MobileNumber { get; set; }

        [NotMapped]
        public long? RoleId { get; set; } // NEW 
        [NotMapped]
        public string? Rolename { get; set; }
    }
}
