using Microsoft.EntityFrameworkCore;
using HridhayConnect_API.Infra;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HridhayConnect_API.Models
{
    public class Category: EntityBase
    {
        [Key]
        public long Id { get; set; }      
        public string? Name { get; set; }      
        public bool? IsActive { get; set; }
        public bool? IsDeleted { get; set; }
    }

   
}
