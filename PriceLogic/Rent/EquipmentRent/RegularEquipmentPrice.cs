namespace PriceLogic.Rent.EquipmentRent
{
    public class RegularEquipmentPrice : IEquipmentPrice
    {
        public float CalculatePrice(int numberOfDays)
        {
            var defaultFee = (short)FeeType.OneTime + (short)FeeType.PremiumDaily * 2;

            if (numberOfDays >= 2)
                defaultFee += (short) FeeType.RegularDaily * (numberOfDays - 2);

            return defaultFee;
        }
    }
}
