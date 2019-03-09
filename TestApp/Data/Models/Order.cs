using System;
using System.ComponentModel.DataAnnotations;

namespace TestApp.Data.Models
{
    public class Order
    {
        [Key]
        public int Id { get; set; }
        public DateTime DateCreated { get; set; }
        public double TotalPrice { get; set; }
        public bool Completed { get; set; }
        // in future - ForeignKey to Users table
        // currently - SessionID of a user
        public int UserID { get; set; }
    }
}