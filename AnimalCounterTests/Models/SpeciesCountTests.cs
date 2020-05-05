using System;
using System.Collections.Generic;
using AnimalCounter;
using AnimalCounter.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AnimalCounterTests
{
    [TestClass]
    public class SpeciesCountTests
    {
        [TestMethod]
        public void CountsDistinctSpecies()
        {
            var sut = new SpeciesCount(DateTime.Now, new List<int>() { 1, 1, 2 });

            Assert.AreEqual(2, sut.Count);
        }
    }

    [TestClass]
    public class SpeciesInteractionTests
    {
        [TestMethod]
        public void AddsMeetings()
        {
            var sut = new SpeciesInteraction(1);

            sut.AddMeetings(new List<int>() { 1, 2, 3 });
            sut.AddMeetings(new List<int>() { 2 });

            Assert.AreEqual(2, sut.Meetings[2]);
            Assert.AreEqual(1, sut.Meetings[3]);
        }
    }

    [TestClass]
    public class SpeciesCounterTests
    {
        [TestMethod]
        public void BuildSpeciesGrid()
        {
            var result = DataModeler.BuildSpeciesGrid(new List<int>() { 1, 2, 3 });

            result[1][1] = 5;
            result[2][2] = 5;
            result[3][3] = 5;

            Assert.AreEqual(5, result[1][1]);
            Assert.AreEqual(0, result[1][2]);
            Assert.AreEqual(0, result[1][3]);

            Assert.AreEqual(0, result[2][1]);
            Assert.AreEqual(5, result[2][2]);
            Assert.AreEqual(0, result[2][3]);

            Assert.AreEqual(0, result[3][1]);
            Assert.AreEqual(0, result[3][2]);
            Assert.AreEqual(5, result[3][3]);
        }
    }
}
