using Microsoft.EntityFrameworkCore;
using HridhayConnect_API.Infra;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HridhayConnect_API.Models
{
    public class Item : EntityBase
    {
        [Key]
        public long Id { get; set; }

        public long? CategoryId { get; set; }
        public string? ItemName { get; set; }
        public string? Category_Name { get; set; }
        public string? Unit { get; set; }
        public string? Unit_Text { get; set; }
        public string? PackagingType { get; set; }
        public string? PackagingType_Text { get; set; }
        public string? HSNCode { get; set; }
        public decimal? TaxPercent { get; set; }
        public string? Description { get; set; }
        public decimal? Rate { get; set; }
        public decimal? MRP { get; set; }
        public bool? IsAvailableForSale { get; set; }
        public bool? IsActive { get; set; }
        public bool? IsDeleted { get; set; }       
    }


}
