using Data.Models;

namespace TestApp.Models
{
    public class EquipmentItemModel
    {
        public int Id { get; private set; }
        public string Name { get; private set; }
        public EquipmentType Type { get; private set; }

        public EquipmentItemModel(int id, string name, EquipmentType type)
        {
            Id = id;
            Name = name;
            Type = type;
        }

        public EquipmentItemModel(EquipmentItem equipmentItem)
            : this(equipmentItem.Id, equipmentItem.Name, equipmentItem.Type)
        {
        }
    }
}