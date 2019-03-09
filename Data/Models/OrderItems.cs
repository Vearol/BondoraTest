using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data.Models
{
    public class OrderItem
    {
        [Key]
        public int Id { get; private set; }
        public DateTime DateAdded { get; private set; }
        public int RentDurationInDays { get; private set; }

        public int OrderId { get; set; }
        public int EquipmentId { get; set; }
        
        [ForeignKey("EquipmentId")]
        public virtual EquipmentItem Equipment { get; private set; }
        [ForeignKey("OrderId")]
        public virtual Order Order { get; private set; }

        public OrderItem(DateTime dateAdded, int rentDurationInDays, int orderId, int equipmentId)
        {
            DateAdded = dateAdded;
            RentDurationInDays = rentDurationInDays;
            OrderId = orderId;
            EquipmentId = equipmentId;
        }
    }
}