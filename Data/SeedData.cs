using System.Linq;
using Data.Models;

namespace Data
{
    public static class SeedData
    {
        public static void Initialize(StoreContext context)
        {
            context.Database.EnsureCreated();

            if (context.EquipmentItems.Any())
            {
                return;
            }

            var equipmentItems = new[]
            {
                new EquipmentItem {Name = "Caterpillar bulldozer", Type = EquipmentType.Heavy},
                new EquipmentItem {Name = "KamAZ truck", Type = EquipmentType.Regular},
                new EquipmentItem {Name = "Komatsu crane", Type = EquipmentType.Heavy},
                new EquipmentItem {Name = "Volvo steamroller", Type = EquipmentType.Regular},
                new EquipmentItem {Name = "Bosch jackhammer", Type = EquipmentType.Specialized}
            };

            foreach (var item in equipmentItems)
            {
                context.EquipmentItems.Add(item);
            }

            context.SaveChanges();
        }
    }
}
