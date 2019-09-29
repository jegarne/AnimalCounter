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

    public class SpeciesInteraction
    {
        public SpeciesInteraction(int speciesId)
        {
            SpeciesId = speciesId;
        }

        public int SpeciesId { get; set; }
        public Dictionary<int, int> Meetings { get; } = new Dictionary<int, int>();

        public void AddMeetings(List<int> ids)
        {
            var otherIds = ids.Where(s => s != this.SpeciesId);

            foreach (var id in otherIds)
            {
                if (this.Meetings.ContainsKey(id))
                    this.Meetings[id] = this.Meetings[id] + 1;
                else
                    this.Meetings.Add(id, 1);
            }
        }

        public void WriteMeetings(string path, Dictionary<int, string> speciesLookup)
        {
            using (System.IO.StreamWriter file = new System.IO.StreamWriter(path, true))
            {
                foreach (var kv in this.Meetings)
                {
                    var sp1 = speciesLookup[this.SpeciesId];
                    var sp2 = speciesLookup[kv.Key];
                    file.WriteLine($"{sp1},{sp2},{kv.Value}");
                }
            }

        }
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

        public void SpeciesPerStandPerDate()
        {
            using (System.IO.StreamWriter file = new System.IO.StreamWriter(@"C:\Users\Jeremy\Desktop\patricia\WildSpeciesPerStand.txt", true))
            {
                file.WriteLine("Market,Stand,Date,SpeciesCount,SpeciesIds");
            }
            //Console.WriteLine("Market,Stand,Date,SpeciesCount,SpeciesIds");
            foreach (var market in ctx.Markets.ToList())
            {
                var result = new List<SpeciesCount>();
                var records = ctx.MarketStandSpeciesDateCount.Where(m => m.MarketId == market.ID).ToList();
                var stands = records.Select(m => m.StandNumber).Distinct();

                foreach (var standNumber in stands.OrderBy(d => d))
                {
                    var dates = records.Where(s => s.StandNumber == standNumber).Select(m => m.ObservationDate).Distinct();
                    foreach (var day in dates.OrderBy(d => d.Value))
                    {
                        var speciesCount = records
                            .Where(s => s.ObservationDate.Value.Date == day.Value.Date
                            && s.StandNumber == standNumber
                            && includedSpeciesIds.Contains(s.SpeciesId))
                            .Select(s => s.SpeciesId).ToList();
                        if (speciesCount.Count > 0)
                            result.Add(new SpeciesCount(day.Value, speciesCount));
                    }



                    using (System.IO.StreamWriter file = new System.IO.StreamWriter(@"C:\Users\Jeremy\Desktop\patricia\WildSpeciesPerStand.txt", true))
                    {
                        foreach (var dc in result)
                        {
                            file.WriteLine($"{market.WetmarketName},{standNumber},{dc.Date.Date.ToShortDateString()},{dc.Count},{String.Join(",", dc.SpeciesIds.Distinct().ToArray())}");
                        }
                    }
                    Console.WriteLine("done");
                    //Console.WriteLine($"{market.WetmarketName},{standNumber},{dc.Date.Date.ToShortDateString()},{dc.Count},{String.Join(",", dc.SpeciesIds.Distinct().ToArray())}");
                }
            }
        }

        public void OccurencesOfSpeciesInStandTogether()
        {
            var path = @"C:\Users\Jeremy\Desktop\patricia\OccurencesOfSpeciesInStandTogether.txt";
            using (System.IO.StreamWriter file = new System.IO.StreamWriter(path, true))
            {
                file.WriteLine("SpeciesId1,SpeciesId2,OccurencesInSameStand");
            }

            var result = new List<SpeciesInteraction>();

            foreach (var market in ctx.Markets.ToList())
            {
                var records = ctx.MarketStandSpeciesDateCount.Where(m => m.MarketId == market.ID).ToList();
                var stands = records.Select(m => m.StandNumber).Distinct();

                foreach (var standNumber in stands.OrderBy(d => d))
                {
                    var dates = records.Where(s => s.StandNumber == standNumber).Select(m => m.ObservationDate).Distinct();
                    foreach (var day in dates.OrderBy(d => d.Value))
                    {
                        var speciesIdList = records
                            .Where(s => s.ObservationDate.Value.Date == day.Value.Date
                            && s.StandNumber == standNumber
                            && includedSpeciesIds.Contains(s.SpeciesId))
                            .Select(s => s.SpeciesId).ToList();

                        foreach (var id in speciesIdList.Distinct())
                        {
                            var interaction = result.FirstOrDefault(s => s.SpeciesId == id);

                            if (interaction == null)
                            {
                                var newInteraction = new SpeciesInteraction(id);
                                result.Add(newInteraction);
                                interaction = result.FirstOrDefault(s => s.SpeciesId == id);
                            }

                            interaction.AddMeetings(speciesIdList);
                        }
                    }
                }
            }

            var speciesLookup = ctx.Species
                .Where(s => includedSpeciesIds.Contains(s.ID)).ToDictionary(mc => mc.ID, mc => mc.SpeciesName);
            foreach (var si in result)
            {
                si.WriteMeetings(path, speciesLookup);
            }

            Console.WriteLine("done");

        }
    }
}
