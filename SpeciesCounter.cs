using AnimalCounter.Context;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AnimalCounter
{
    public class SpeciesCounter
    {
        private AnimalContext _ctx;
        private readonly List<int> _includedSpeciesIds;
        private Dictionary<int, string> _speciesLookup;

        public SpeciesCounter(List<int> animalCodes)
        {
            _ctx = new AnimalContext();
            _includedSpeciesIds = _ctx.Species.Where(a => animalCodes.Contains(a.AnimalCode.Value))
                .Select(a => a.ID).ToList();
            _speciesLookup = _ctx.Species
                        .Where(s => _includedSpeciesIds.Contains(s.ID))
                        .ToDictionary(mc => mc.ID, mc => mc.SpeciesName);
        }

        public void SpeciesMaxMin()
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
                        && _includedSpeciesIds.Contains(s.SpeciesId)
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
            foreach (var market in _ctx.Markets.ToList())
            {
                var result = new List<SpeciesCount>();
                var records = _ctx.MarketSpeciesDateCount.Where(m => m.MarketId == market.ID).ToList();
                var dates = records.Select(m => m.ObservationDate).Distinct();

                foreach (var day in dates.OrderBy(d => d.Value))
                {
                    var speciesCount = records
                        .Where(s => s.ObservationDate.Value.Date == day.Value.Date
                        && _includedSpeciesIds.Contains(s.SpeciesId))
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
            foreach (var market in _ctx.Markets.ToList())
            {
                var result = new List<SpeciesCount>();
                var records = _ctx.MarketSpeciesDateCount.Where(m => m.MarketId == market.ID).ToList();
                var dates = records.Select(m => m.ObservationDate).Distinct();

                foreach (var day in dates.OrderBy(d => d.Value))
                {
                    var individualCount = records
                        .Where(s => s.ObservationDate.Value.Date == day.Value.Date
                        && _includedSpeciesIds.Contains(s.SpeciesId))
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
                            && s.StandNumber == standNumber
                            && _includedSpeciesIds.Contains(s.SpeciesId))
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

        public List<SpeciesInteraction> OccurencesOfSpeciesInStandTogether()
        {
            var result = new List<SpeciesInteraction>();

            foreach (var market in _ctx.Markets.ToList())
            {
                var records = _ctx.MarketStandSpeciesDateCount.Where(m => m.MarketId == market.ID).ToList();
                var stands = records.Select(m => m.StandNumber).Distinct();

                foreach (var standNumber in stands.OrderBy(d => d))
                {
                    var dates = records.Where(s => s.StandNumber == standNumber).Select(m => m.ObservationDate).Distinct();
                    foreach (var day in dates.OrderBy(d => d.Value))
                    {
                        var speciesIdList = records
                            .Where(s => s.ObservationDate.Value.Date == day.Value.Date
                            && s.StandNumber == standNumber
                            && _includedSpeciesIds.Contains(s.SpeciesId))
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

            return result;
        }

        public void WriteOccurencesOfSpeciesInStandTogether()
        {
            var path = @"C:\Users\Jeremy\Desktop\patricia\OccurencesOfSpeciesInStandTogether.txt";
            using (System.IO.StreamWriter file = new System.IO.StreamWriter(path, true))
            {
                file.WriteLine("SpeciesId1,SpeciesId2,OccurencesInSameStand");
            }

            var result = OccurencesOfSpeciesInStandTogether();

            foreach (var si in result)
            {
                si.WriteMeetings(path, _speciesLookup);
            }

            Console.WriteLine("done");

        }

        public void WriteOccurencesOfSpeciesInStandTogetherGrid()
        {
            var grid = DataModeler.BuildSpeciesGrid(_includedSpeciesIds);
            var result = OccurencesOfSpeciesInStandTogether();

            foreach (var si in result)
            {
                si.UpdateGrid(grid);
            }

            WriteSpeciesGrid(grid);

            Console.WriteLine("done");
        }

        public void WriteSpeciesGrid(Dictionary<int, Dictionary<int, int>> grid)
        {
            var path = @"C:\Users\Jeremy\Desktop\patricia\OccurencesOfSpeciesInStandTogetherGrid.csv";
            File.WriteAllText(path, String.Empty);

            using (System.IO.StreamWriter file = new System.IO.StreamWriter(path, true))
            {
                // header row
                var xKeys = grid.First().Value.Keys;
                var xSpecies = new List<string>();
                foreach (var key in xKeys)
                {
                    xSpecies.Add(_speciesLookup[key]);
                }
                file.WriteLine("," + string.Format("{0}", string.Join(",", xSpecies)));

                // grid body
                foreach (var kv in grid)
                {
                    file.WriteLine(_speciesLookup[kv.Key] + "," + string.Format("{0}", string.Join(",", kv.Value.Values)));
                }
            }

        }

        public void CountIndividuals()
        {
            var summaryPath = @"C:\Users\Jeremy\Desktop\patricia\Individuals\SummaryIndividualCounts.csv";
            File.WriteAllText(summaryPath, String.Empty);

            using (System.IO.StreamWriter file = new System.IO.StreamWriter(summaryPath, true))
            {
                file.WriteLine("Species,Individuals");
            }

            var result = new Dictionary<int, int>();

            var speciesIds = _ctx.MarketStandSpeciesDateCount
                                .Where(s => _includedSpeciesIds.Contains(s.SpeciesId))
                                .Select(s => s.SpeciesId).Distinct().ToList();

            var count = 1;
            foreach (var speciesId in speciesIds)
            {
                var speciesName = _speciesLookup[speciesId];
                //var speciesPathName = speciesName.Replace(" ", "_").Replace(".", "").ToLower();
                //var speciesPath = @"C:\Users\Jeremy\Desktop\patricia\Individuals\" + speciesPathName + ".csv";
                //File.WriteAllText(summaryPath, String.Empty);
                //using (StreamWriter file = File.AppendText(speciesPath))
                //{
                //    file.WriteLine("MarketId,StandId,StartDate,EndDate,Individuals");
                //}

                result.Add(speciesId, 0);
                var records = _ctx.MarketStandSpeciesDateCount.Where(m => m.SpeciesId == speciesId).ToList();
                var startDate = records.Min(x => x.ObservationDate);
                var endDate = records.Max(x => x.ObservationDate);

                var marketIds = records.Select(r => r.MarketId).Distinct().ToList();

                foreach (var marketId in marketIds)
                {
                    var standIds = records.Where(x => x.MarketId == marketId && x.StandNumber != null)
                        .Select(r => r.StandNumber).Distinct().ToList();

                    foreach (var standId in standIds)
                    {
                        var counter = new PeriodCounter(startDate.Value, endDate.Value, speciesId);
                        var current = counter.GetActivePeriod();
                        while (current != null)
                        {
                            var observedDates = records
                                .Where(r => r.ObservationDate >= current.StartDate
                                && r.ObservationDate <= current.EndDate
                                && r.StandNumber == standId
                                && r.MarketId == marketId).ToList();

                            var datesDict = new Dictionary<DateTime, int>();

                            foreach (var date in observedDates)
                            {
                                if (datesDict.ContainsKey(date.ObservationDate.Value)) continue;

                                var total = observedDates.Where(x => x.ObservationDate.Value.Date == date.ObservationDate.Value.Date)
                                            .Sum(x => x.QuantityAnimals);
                                datesDict.Add(date.ObservationDate.Value, (int)total);
                            }

                            current.AddObservations(datesDict);
                            result[speciesId] = result[speciesId] + current.TotalIndividuals();
                            counter.NextPeriod();
                            current = counter.GetActivePeriod();
                        }

                        //using (StreamWriter file = File.AppendText(speciesPath))
                        //{
                        //    var counts = counter.GetAllPeriods();
                        //    foreach (var c in counts)
                        //    {
                        //        var total = c.TotalIndividuals();
                        //        if (total > 0)
                        //            file.WriteLine($"{marketId},{standId}," +
                        //                $"{c.StartDate.Date.ToShortDateString()}, " +
                        //                $"{c.EndDate.Date.ToShortDateString()}, {total}");
                        //    }

                        //}
                    }
                }


                using (System.IO.StreamWriter file = new System.IO.StreamWriter(summaryPath, true))
                {
                    file.WriteLine($"{speciesName},{result[speciesId]}");
                }

                Console.WriteLine(count + " of " + speciesIds.Count);
                count++;
            }

            Console.WriteLine("done");
        }
    }
}
