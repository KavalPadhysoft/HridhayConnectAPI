using HridhayConnect_API.Infra;
using System.ComponentModel.DataAnnotations;

namespace HridhayConnect_API.Models
{
    public class Dashboard : EntityBase
    {
        [Key]
        public long Id { get; set; }
        public long CustomerId { get; set; }
        public long SalesPerson_Id { get; set; }
        public DateTime? Order_Date { get; set; }
        public DateTime? Delivery_Date { get; set; }
        public int? No_Of_Items { get; set; }
        public int? Delivered_Items { get; set; }
        public string? Order_No { get; set; }
        public string? CustomerName { get; set; }
        public string? SalesPersonName { get; set; }
        public string? GST_NO { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public string? Address { get; set; }
        public decimal? Total_Amount { get; set; }
        public bool? IsActive { get; set; }
        public bool? IsDeleted { get; set; }
        public string? Notes { get; set; }
        public string? Remarks { get; set; }
        public string? Order_Status { get; set; }
        public string? Order_Status_Text { get; set; }

    }
    public class PendingItem : EntityBase
    {
        [Key]
        public long Id { get; set; }

        public long? CategoryId { get; set; }
        public string? ItemName { get; set; }
        public string? CategoryName { get; set; }
        public string? Unit { get; set; }
        public string? Unit_Text { get; set; }
        public string? PackagingType { get; set; }
        public string? PackagingType_Text { get; set; }
        public int? TotalQuantity { get; set; }
    }
   
}
