using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TestApp.Data.Models
{
    public class OrderItem
    {
        [Key]
        public int Id { get; set; }
        public DateTime DateAdded { get; set; }
        
        public int OrderId { get; set; }
        public int EquipmentId { get; set; }
        
        [ForeignKey("EquipmentId")]
        public virtual EquipmentItem Equipment { get; set; }
        [ForeignKey("OrderId")]
        public virtual Order Order { get; set; }
    }
}