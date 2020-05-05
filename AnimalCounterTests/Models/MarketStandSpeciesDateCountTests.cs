using AnimalCounter.Context;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AnimalCounterTests.Models
{
    [TestClass]
    public class MarketStandSpeciesDateCountTests
    {
        [TestMethod]
        public void IsNotStandWhen999()
        {
            var sut = new MarketStandSpeciesDateCount();

            sut.StandNumber = "999";

            Assert.AreEqual(true, sut.IsNotRealStand());
        }

        [TestMethod]
        public void IsNotStandWhenNotNumeric()
        {
            var sut = new MarketStandSpeciesDateCount();

            sut.StandNumber = "ABC";

            Assert.AreEqual(true, sut.IsNotRealStand());
        }

        [TestMethod]
        public void IsStandWhenNumeric()
        {
            var sut = new MarketStandSpeciesDateCount();

            sut.StandNumber = "123";

            Assert.AreEqual(false, sut.IsNotRealStand());
        }
    }
}
