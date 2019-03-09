using System.Linq;
using Data.Models;

namespace Data
{
    public static class SeedData
    {
        public static void Initialize(StoreContext context)
        {
            if (context.EquipmentItems.Any())
            {
                return;
            }

            var equipmentItems = new[]
            {
                new EquipmentItem("Caterpillar bulldozer", EquipmentType.Heavy),
                new EquipmentItem("KamAZ truck", EquipmentType.Regular),
                new EquipmentItem("Komatsu crane", EquipmentType.Heavy),
                new EquipmentItem("Volvo steamroller", EquipmentType.Regular),
                new EquipmentItem("Bosch jackhammer", EquipmentType.Specialized)
            };

            foreach (var item in equipmentItems)
            {
                context.EquipmentItems.Add(item);
            }

            context.SaveChanges();
        }
    }
}
