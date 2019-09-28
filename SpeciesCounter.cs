using AnimalCounter.Context;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AnimalCounter
{

    public class SpeciesCount
    {
        public SpeciesCount(DateTime date, List<int> speciesIds)
        {
            Date = date;
            SpeciesIds = speciesIds;
        }

        public DateTime Date { get; set; }
        public int Count
        {
            get
            {
                return this.SpeciesIds.Distinct().Count();
            }
        }
        public List<int> SpeciesIds { get; set; }
    }

    public class SpeciesCounter
    {
        private AnimalContext ctx;
        private readonly List<int> includedSpeciesIds;

        public SpeciesCounter(List<int> animalCodes)
        {
            ctx = new AnimalContext();
            includedSpeciesIds = ctx.Species.Where(a => animalCodes.Contains(a.AnimalCode.Value)).Select(a => a.ID).ToList();
        }

        public void SpeciesMaxMin()
        {
            Console.WriteLine("Market,Max,Min");
            foreach (var market in ctx.Markets.ToList())
            {
                var result = new List<SpeciesCount>();
                var records = ctx.MarketSpeciesDateCount.Where(m => m.MarketId == market.ID).ToList();
                var dates = records.Select(m => m.ObservationDate);

                foreach (var day in dates.OrderBy(d => d.Value))
                {
                    var speciesCount = records
                        .Where(s => s.ObservationDate.Value.Date == day.Value.Date
                        && includedSpeciesIds.Contains(s.SpeciesId)
                        ).Select(s => s.SpeciesId).ToList();
                    result.Add(new SpeciesCount(day.Value, speciesCount));
                }

                var max = result.Max(v => v.Count);
                var min = result.Min(v => v.Count);

                Console.WriteLine(market.WetmarketName + "," + max + "," + min);
            }
        }


        public void SpeciesPerDate()
        {
            Console.WriteLine("Market,Date,SpeciesCount,SpeciesIds");
            foreach (var market in ctx.Markets.ToList())
            {
                var result = new List<SpeciesCount>();
                var records = ctx.MarketSpeciesDateCount.Where(m => m.MarketId == market.ID).ToList();
                var dates = records.Select(m => m.ObservationDate).Distinct();

                foreach (var day in dates.OrderBy(d => d.Value))
                {
                    var speciesCount = records
                        .Where(s => s.ObservationDate.Value.Date == day.Value.Date
                        && includedSpeciesIds.Contains(s.SpeciesId))
                        .Select(s => s.SpeciesId).ToList();
                    result.Add(new SpeciesCount(day.Value, speciesCount));
                }

                foreach (var dc in result)
                {
                    Console.WriteLine($"{market.WetmarketName},{dc.Date.Date.ToShortDateString()},{dc.Count},{String.Join(",", dc.SpeciesIds.Distinct().ToArray())}");
                }
            }
        }

        public void MaxAndMinIndividualsPerMarketPerDay()
        {
            Console.WriteLine("Market,Date,IndividualAnimalCount");
            foreach (var market in ctx.Markets.ToList())
            {
                var result = new List<SpeciesCount>();
                var records = ctx.MarketSpeciesDateCount.Where(m => m.MarketId == market.ID).ToList();
                var dates = records.Select(m => m.ObservationDate).Distinct();

                foreach (var day in dates.OrderBy(d => d.Value))
                {
                    var individualCount = records
                        .Where(s => s.ObservationDate.Value.Date == day.Value.Date
                        && includedSpeciesIds.Contains(s.SpeciesId))
                        .Sum(s => s.QuantityAnimals);
                    Console.WriteLine($"{market.WetmarketName},{day.Value.Date.ToShortDateString()},{individualCount}");
                }
            }

        }
    }
}
