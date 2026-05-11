using Microsoft.EntityFrameworkCore;
using HridhayConnect_API.Infra;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HridhayConnect_API.Models
{
    public class CustomerLedger : EntityBase
    {
        [Key]
        public long Id { get; set; }
        public DateTime? Date { get; set; }
        public string? ReferenceNo { get; set; }      
        public string? Description { get; set; }      
        public decimal? Debit { get; set; }      
        public decimal? Credit { get; set; }      
        public decimal? Balance { get; set; }      
        public bool? IsActive { get; set; }
        public bool? IsDeleted { get; set; }
    }

   
}
