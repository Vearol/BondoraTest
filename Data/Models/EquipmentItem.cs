using System.ComponentModel.DataAnnotations;

namespace Data.Models
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

        public EquipmentItem(string name, EquipmentType type)
        {
            Name = name;
            Type = type;
        }
    }
}