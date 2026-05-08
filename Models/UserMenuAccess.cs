namespace HridhayConnect_API.Models
{
    public class UserMenuAccess
    {
        public long UserId { get; set; }
        public long RoleId { get; set; }
        public long MenuId { get; set; }
        public bool IsRead { get; set; }
        public bool IsCreate { get; set; }
        public bool IsUpdate { get; set; }
        public bool IsDelete { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
    }
}
