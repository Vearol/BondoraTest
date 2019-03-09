namespace TestApp.Models
{
    public class OrderItemModel
    {
        public int Id { get; private set; }
        public int RentDurationInDays { get; private set; }
        public EquipmentItemModel EquipmentItemModel { get; private set; }

        public OrderItemModel(int id, int rentDurationInDays, EquipmentItemModel equipmentItemModel)
        {
            Id = id;
            RentDurationInDays = rentDurationInDays;
            EquipmentItemModel = equipmentItemModel;
        }
    }
}