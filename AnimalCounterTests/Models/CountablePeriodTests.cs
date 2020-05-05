using AnimalCounter.Context;
using AnimalCounter.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace AnimalCounterTests.Models
{
    [TestClass]
    public class CountablePeriodTests
    {
        [TestMethod]
        public void CountsTotals()
        {
            var observations = new List<MarketStandSpeciesDateCount>();
            observations.Add(new MarketStandSpeciesDateCount("1", DateTime.Now.AddDays(-9), 1));
            observations.Add(new MarketStandSpeciesDateCount("1", DateTime.Now.AddDays(-8), 3));
            observations.Add(new MarketStandSpeciesDateCount("1", DateTime.Now.AddDays(-7), 9));
            observations.Add(new MarketStandSpeciesDateCount("1", DateTime.Now.AddDays(-6), 4));
            observations.Add(new MarketStandSpeciesDateCount("1", DateTime.Now.AddDays(-5), 8));


            var sut = new CountablePeriod();
            sut.AddObservations(observations);

            Assert.AreEqual(13, sut.TotalIndividuals());
        }


        [TestMethod]
        public void CountsTotalsOneRecord()
        {
            var observations = new List<MarketStandSpeciesDateCount>();
            observations.Add(new MarketStandSpeciesDateCount("1", DateTime.Now.AddDays(-9), 1));

            var sut = new CountablePeriod();
            sut.AddObservations(observations);

            Assert.AreEqual(1, sut.TotalIndividuals());
        }


        [TestMethod]
        public void CountsTotalsWith999Stands()
        {
            var observations = new List<MarketStandSpeciesDateCount>();
            observations.Add(new MarketStandSpeciesDateCount("999", DateTime.Now.AddDays(-9), 1));
            observations.Add(new MarketStandSpeciesDateCount("999", DateTime.Now.AddDays(-6), 2));
            observations.Add(new MarketStandSpeciesDateCount("999", DateTime.Now.AddDays(-6), 1));
            observations.Add(new MarketStandSpeciesDateCount("999", DateTime.Now.AddDays(-5), 2));
            observations.Add(new MarketStandSpeciesDateCount("999", DateTime.Now.AddDays(-5), 1));
            observations.Add(new MarketStandSpeciesDateCount("999", DateTime.Now.AddDays(-5), 2));

            var sut = new CountablePeriod();
            sut.AddObservations(observations);

            Assert.AreEqual(9, sut.TotalIndividuals());
        }

        [TestMethod]
        public void CountsTotalsWithDuplicateDates()
        {
            var observations = new List<MarketStandSpeciesDateCount>();
            observations.Add(new MarketStandSpeciesDateCount("1", DateTime.Now.AddDays(-9), 9));
            observations.Add(new MarketStandSpeciesDateCount("1", DateTime.Now.AddDays(-6), 4));
            observations.Add(new MarketStandSpeciesDateCount("1", DateTime.Now.AddDays(-6), 4));
            observations.Add(new MarketStandSpeciesDateCount("1", DateTime.Now.AddDays(-5), 8));
            observations.Add(new MarketStandSpeciesDateCount("1", DateTime.Now.AddDays(-5), 4));
            observations.Add(new MarketStandSpeciesDateCount("1", DateTime.Now.AddDays(-5), 8));

            var sut = new CountablePeriod();
            sut.AddObservations(observations);

            Assert.AreEqual(21, sut.TotalIndividuals());
        }

        [TestMethod]
        public void CountsZeroWhenNoObservations()
        {
            var observations = new List<MarketStandSpeciesDateCount>();

            var sut = new CountablePeriod();
            sut.AddObservations(observations);

            Assert.AreEqual(0, sut.TotalIndividuals());
        }
    }
}
