using System.ComponentModel.DataAnnotations;

namespace TestApp.Data.Models
{
    public enum EquipmentType 
    {
        Heavy,
        Regular,
        Specialized
    }

    public class EquipmentItem
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public EquipmentType Type { get;set; }
    }
}