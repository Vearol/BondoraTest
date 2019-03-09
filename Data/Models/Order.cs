using System;
using System.ComponentModel.DataAnnotations;

namespace Data.Models
{
    public class Order
    {
        [Key]
        public int Id { get; private set; }
        public DateTime DateCreated { get; private set; }
        public int UserId { get; private set; }

        public Order(DateTime dateCreated, int userId)
        {
            DateCreated = dateCreated;
            UserId = userId;
        }
    }
}