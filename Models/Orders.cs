using Microsoft.EntityFrameworkCore;
using HridhayConnect_API.Infra;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HridhayConnect_API.Models
{
      public class Orders : EntityBase
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
            public string? Delivery_Challan_No { get; set; }
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
    public class OrderItem : EntityBase
    {
        [Key]
        public long ItemId { get; set; }

        public long? Order_ID { get; set; }
        public long? Reference_ID { get; set; }
        public long? Item_ID { get; set; }
        public string? ItemName { get; set; }
        public int? Quantity { get; set; }
        public decimal? Rate { get; set; }
        public decimal? Total_Amount { get; set; }        
        [NotMapped] public string? Order_No { get; set; }
        public bool? IsActive { get; set; }
        public bool? IsDeleted { get; set; }
    }
    public class OrderCombo
    {
        public Orders Order { get; set; } = new();
        public List<OrderItem> OrderItems { get; set; } = new();
    }

    public class OrderFullDTO
    {
        public Orders Order { get; set; } = new();
        public List<OrderItem> OrderItems { get; set; } = new();
    }
}
