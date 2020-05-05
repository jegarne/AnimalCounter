using AnimalCounter;
using AnimalCounter.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;

namespace AnimalCounterTests
{
    [TestClass]
    public class PeriodCounterTests
    {
        [TestMethod]
        public void BuildsCorrectCountablePeriods()
        {
            var sut = new PeriodCounter(DateTime.Now.AddDays(-30), DateTime.Now, 0);
            var result = sut.GetAllPeriods();

            Assert.AreEqual(4, result.Count());
        }

        [TestMethod]
        public void BuildsCorrectCountablePeriods2()
        {
            var sut = new PeriodCounter(DateTime.Now.AddDays(-32), DateTime.Now, 0);
            var result = sut.GetAllPeriods();

            Assert.AreEqual(4, result.Count());
        }

        [TestMethod]
        public void PeriodsAreConsecutive()
        {
            var start = DateTime.Now.AddDays(-29);
            var end = DateTime.Now;

            var sut = new PeriodCounter(start, end, 0);
            var result = sut.GetAllPeriods();

            var first = result.FirstOrDefault(p => p.PeriodNumber == 0);
            var second = result.FirstOrDefault(p => p.PeriodNumber == 1);
            var third = result.FirstOrDefault(p => p.PeriodNumber == 2);

            Assert.AreEqual(start.Date, first.StartDate.Date);
            Assert.AreEqual(first.EndDate.Date, second.StartDate.Date.AddDays(-1));
            Assert.AreEqual(second.EndDate.Date, third.StartDate.Date.AddDays(-1));
            Assert.IsTrue(third.StartDate.Date < end.Date);
            Assert.IsTrue(third.EndDate.Date == end.Date);
        }

    }
}

