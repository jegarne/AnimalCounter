using AnimalCounter.Context;
using AnimalCounter.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AnimalCounter.CountingFunctions
{
    public class SpeciesPerStandPerDate
    {
        private AnimalContext _ctx;
        private Dictionary<int, string> _speciesLookup;
        private Dictionary<int, string> _marketLookup;

        public SpeciesPerStandPerDate()
        {
            _ctx = new AnimalContext();

            _speciesLookup = _ctx.Species
                        .ToDictionary(mc => mc.ID, mc => mc.SpeciesName);

            _marketLookup = _ctx.Markets
                        .ToDictionary(mc => mc.ID, mc => mc.MarketName);
        }

        public void Calculate()
        {
            using (System.IO.StreamWriter file = new System.IO.StreamWriter(@"C:\Users\Jeremy\Desktop\patricia\WildSpeciesPerStand.txt", true))
            {
                file.WriteLine("Market,Stand,Date,SpeciesCount,SpeciesIds");
            }
            //Console.WriteLine("Market,Stand,Date,SpeciesCount,SpeciesIds");
            foreach (var market in _ctx.Markets.ToList())
            {
                var result = new List<SpeciesCount>();
                var records = _ctx.MarketStandSpeciesDateCount.Where(m => m.MarketId == market.ID).ToList();
                var stands = records.Select(m => m.StandNumber).Distinct();

                foreach (var standNumber in stands.OrderBy(d => d))
                {
                    var dates = records.Where(s => s.StandNumber == standNumber).Select(m => m.ObservationDate).Distinct();
                    foreach (var day in dates.OrderBy(d => d.Value))
                    {
                        var speciesCount = records
                            .Where(s => s.ObservationDate.Value.Date == day.Value.Date
                            && s.StandNumber == standNumber)
                            .Select(s => s.SpeciesId).ToList();
                        if (speciesCount.Count > 0)
                            result.Add(new SpeciesCount(day.Value, speciesCount));
                    }



                    using (System.IO.StreamWriter file = new System.IO.StreamWriter(@"C:\Users\Jeremy\Desktop\patricia\WildSpeciesPerStand.txt", true))
                    {
                        foreach (var dc in result)
                        {
                            file.WriteLine($"{market.MarketName},{standNumber},{dc.Date.Date.ToShortDateString()},{dc.Count},{String.Join(",", dc.SpeciesIds.Distinct().ToArray())}");
                        }
                    }
                    Console.WriteLine("done");
                    //Console.WriteLine($"{market.WetmarketName},{standNumber},{dc.Date.Date.ToShortDateString()},{dc.Count},{String.Join(",", dc.SpeciesIds.Distinct().ToArray())}");
                }
            }
        }
    }
}
