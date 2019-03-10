namespace PriceLogic.Rent.EquipmentRent
{
    public interface IEquipmentPrice
    {
        float CalculatePrice(int numberOfDays);

        int CalculateLoyaltyPoints(int numberOfDays);
    }
}
