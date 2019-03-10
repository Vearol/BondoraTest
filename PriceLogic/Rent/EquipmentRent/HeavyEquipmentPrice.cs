namespace PriceLogic.Rent.EquipmentRent
{
    public class HeavyEquipmentPrice : IEquipmentPrice
    {
        public float CalculatePrice(int numberOfDays)
        {
            return (short)FeeType.OneTime + (short)FeeType.PremiumDaily * numberOfDays;
        }

        public int CalculateLoyaltyPoints(int numberOfDays)
        {
            return numberOfDays * 2;
        }
    }
}
