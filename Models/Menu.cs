using Microsoft.EntityFrameworkCore;
using HridhayConnect_API.Infra;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HridhayConnect_API.Models
{
    public class Menu: EntityBase
    {
        [Key]
        public long Id { get; set; }
        public long ParentId { get; set; }
        public string? Area { get; set; }
        public string? Controller { get; set; }
        public string? Url { get; set; }
        public string? Name { get; set; }
        public string? Parent_Menu_Name { get; set; }
        public string? Icon { get; set; }
        public int? DisplayOrder { get; set; }
        public bool? IsSuperAdmin { get; set; }
        public bool? IsAdmin { get; set; }
        public bool? IsActive { get; set; }
        public bool? IsDeleted { get; set; }
    }

    [Keyless]
    public class MenuAccessDto
    {
        public long Id { get; set; }
        public long? ParentId { get; set; }

        // these string columns can be NULL in DB, so make them nullable
        public string? ParentMenuName { get; set; }
        public string? Area { get; set; }
        public string? Controller { get; set; }
        public string? Url { get; set; }
        public string? Name { get; set; }
        public string? Icon { get; set; }

        // DisplayOrder might be nullable in DB
        public int? DisplayOrder { get; set; }
        // Stored proc selects nullable bit columns in your Menu table,
        // so use nullable bools to be safe
        public bool? IsSuperAdmin { get; set; }
        public bool? IsAdmin { get; set; }
        public bool? IsActive { get; set; }
        public bool? IsDeleted { get; set; }

        // new for second tables
        [NotMapped]

        public long RoleId { get; set; }
        [NotMapped]
        public long MenuId { get; set; }
        [NotMapped]
        public bool IsRead { get; set; }
        [NotMapped]
        public bool IsCreate { get; set; }
        [NotMapped]
        public bool IsUpdate { get; set; }
        [NotMapped]
        public bool IsDelete { get; set; }
        [NotMapped]
        public long? CreatedBy { get; set; }

    }
}
