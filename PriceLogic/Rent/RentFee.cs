using System.Collections.Generic;
using Data.Models;
using PriceLogic.Rent.EquipmentRent;

namespace PriceLogic.Rent
{
    public class RentFee : IFee
    {
        private EquipmentType _equipmentType;
        private int _numberOfDays;

        private readonly Dictionary<EquipmentType, IEquipmentPrice> _equipmentTypePrice = new Dictionary<EquipmentType, IEquipmentPrice>()
        {
            { EquipmentType.Heavy, new HeavyEquipmentPrice() },
            { EquipmentType.Regular, new RegularEquipmentPrice() },
            { EquipmentType.Specialized, new SpecializedEquipmentPrice() }
        };

        public RentFee(EquipmentType equipmentType, int numberOfDays)
        {
            _equipmentType = equipmentType;
            _numberOfDays = numberOfDays;
        }

        public float CalculateFee()
        {
            return _equipmentTypePrice[_equipmentType].CalculatePrice(_numberOfDays);
        }
    }
}
