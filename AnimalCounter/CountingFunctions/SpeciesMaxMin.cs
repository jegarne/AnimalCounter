using AnimalCounter.Context;
using AnimalCounter.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AnimalCounter.CountingFunctions
{
    public class SpeciesMaxMin
    {
        private AnimalContext _ctx;
        private Dictionary<int, string> _speciesLookup;
        private Dictionary<int, string> _marketLookup;

        public SpeciesMaxMin()
        {
            _ctx = new AnimalContext();

            _speciesLookup = _ctx.Species
                        .ToDictionary(mc => mc.ID, mc => mc.SpeciesName);

            _marketLookup = _ctx.Markets
                        .ToDictionary(mc => mc.ID, mc => mc.MarketName);
        }

        public void Calculate()
        {
            Console.WriteLine("Market,Max,Min");
            foreach (var market in _ctx.Markets.ToList())
            {
                var result = new List<SpeciesCount>();
                var records = _ctx.MarketSpeciesDateCount.Where(m => m.MarketId == market.ID).ToList();
                var dates = records.Select(m => m.ObservationDate);

                foreach (var day in dates.OrderBy(d => d.Value))
                {
                    var speciesCount = records
                        .Where(s => s.ObservationDate.Value.Date == day.Value.Date
                        ).Select(s => s.SpeciesId).ToList();
                    result.Add(new SpeciesCount(day.Value, speciesCount));
                }

                var max = result.Max(v => v.Count);
                var min = result.Min(v => v.Count);

                Console.WriteLine(market.MarketName + "," + max + "," + min);
            }
        }
    }
}
