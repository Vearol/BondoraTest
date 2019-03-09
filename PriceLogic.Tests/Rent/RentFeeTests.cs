using Data.Models;
using NUnit.Framework;
using PriceLogic.Rent;

namespace PriceLogic.Tests.Rent
{
    public class RentFeeTests
    {
        [Test]
        public void CalculateHeavyFeeTest()
        {
            var heavyRentFee = new RentFee(EquipmentType.Heavy, 1);

            Assert.AreEqual(heavyRentFee.CalculateFee(), 160.0f);
        }

        [Test]
        public void CalculateRegularFeeTest()
        {
            var regularRentFee = new RentFee(EquipmentType.Regular, 1);
            
            Assert.AreEqual(regularRentFee.CalculateFee(), 220.0f);
        }

        [Test]
        public void CalculateSpecializedFeeTest()
        {
            var specializedRentFee = new RentFee(EquipmentType.Specialized, 1);
            
            Assert.AreEqual(specializedRentFee.CalculateFee(), 180.0f);
        }
    }
}
