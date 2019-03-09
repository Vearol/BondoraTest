namespace TestApp.Models
{
    public class OrderItemModel
    {
        public int Id { get; private set; }
        public EquipmentItemModel EquipmentItemModel { get; private set; }

        public OrderItemModel(int id, EquipmentItemModel equipmentItemModel)
        {
            Id = id;
            EquipmentItemModel = equipmentItemModel;
        }
    }
}