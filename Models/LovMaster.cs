using HridhayConnect_API.Infra;
using System.ComponentModel.DataAnnotations.Schema;

namespace HridhayConnect_API.Models
{
    public class LovMaster : EntityBase
    {
        public string? Lov_Column { get; set; }
        public string? Lov_Code { get; set; }
        public string? Lov_Desc { get; set; }
        public string? Display_Text { get; set; }
        public int DisplayOrder { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        [NotMapped] public long?
            MaxDisplay_Seq_No { get; set; }
        [NotMapped] public string? Action { get; set; }
    }
}
