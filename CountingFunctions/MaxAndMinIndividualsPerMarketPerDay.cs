using AnimalCounter.Context;
using AnimalCounter.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AnimalCounter.CountingFunctions
{
    public class MaxAndMinIndividualsPerMarketPerDay
    {
        private AnimalContext _ctx;
        private Dictionary<int, string> _speciesLookup;
        private Dictionary<int, string> _marketLookup;

        public MaxAndMinIndividualsPerMarketPerDay()
        {
            _ctx = new AnimalContext();

            _speciesLookup = _ctx.Species
                        .ToDictionary(mc => mc.ID, mc => mc.SpeciesName);

            _marketLookup = _ctx.Markets
                        .ToDictionary(mc => mc.ID, mc => mc.MarketName);
        }


        public void Calculate()
        {
            Console.WriteLine("Market,Date,IndividualAnimalCount");
            foreach (var market in _ctx.Markets.ToList())
            {
                var result = new List<SpeciesCount>();
                var records = _ctx.MarketSpeciesDateCount.Where(m => m.MarketId == market.ID).ToList();
                var dates = records.Select(m => m.ObservationDate).Distinct();

                foreach (var day in dates.OrderBy(d => d.Value))
                {
                    var individualCount = records
                        .Where(s => s.ObservationDate.Value.Date == day.Value.Date)
                        .Sum(s => s.QuantityAnimals);
                    Console.WriteLine($"{market.MarketName},{day.Value.Date.ToShortDateString()},{individualCount}");
                }
            }
        }
    }
}
