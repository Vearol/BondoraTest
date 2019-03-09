namespace PriceLogic.Rent.EquipmentRent
{
    public class SpecializedEquipmentPrice : IEquipmentPrice
    {
        public float CalculatePrice(int numberOfDays)
        {
            var defaultFee = (short)FeeType.PremiumDaily * 3;

            if (numberOfDays >= 3)
                defaultFee += (short)FeeType.RegularDaily * (numberOfDays - 3);

            return defaultFee;
        }
    }
}
