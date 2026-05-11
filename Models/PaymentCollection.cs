using Microsoft.EntityFrameworkCore;
using HridhayConnect_API.Infra;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HridhayConnect_API.Models
{
      public class PaymentCollection : EntityBase
      {
            [Key]
            public long Id { get; set; }
            public long CustomerId { get; set; }
            public long Collected_By { get; set; }
            public DateTime? PaymentDate { get; set; }
            public string? Payment_Receipt_No { get; set; }
            public string? CustomerName { get; set; }
            public string? Collected_By_Text { get; set; }
            public string? PaymentMode { get; set; }
            public string? PaymentMode_Text { get; set; }
            public string? ReferenceNo { get; set; }
            public string? Remarks { get; set; }           
            public decimal? Amount { get; set; }           
            public bool? IsActive { get; set; }
            public bool? IsDeleted { get; set; }
            public decimal? Total_Amount { get; set; }

    }
    

   
}
