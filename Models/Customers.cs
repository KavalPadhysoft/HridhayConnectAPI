using Microsoft.EntityFrameworkCore;
using HridhayConnect_API.Infra;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HridhayConnect_API.Models
{
      public class Customers : EntityBase
      {
            [Key]
            public long Id { get; set; }

            public string? Name { get; set; }
            public string? OwnerName { get; set; }
            public string? Phone { get; set; }
            public string? Email { get; set; }
            public string? GSTNumber { get; set; }
            public string? CustomerType { get; set; }
            public string? AddressLine { get; set; }
            public string? Area { get; set; }
            public string? City { get; set; }
            public string? State { get; set; }
            public string? Location { get; set; }
            public int? Pincode { get; set; }
            public decimal? CreditLimit { get; set; }
            public int? CreditDays { get; set; }
            public bool? IsActive { get; set; }
            public bool? IsDeleted { get; set; }

           
      }
    

   
}
