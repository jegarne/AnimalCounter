using AnimalCounter.Context;
using AnimalCounter.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AnimalCounter.CountingFunctions
{
    public class CountIndividualsBySpeciesAndDate
    {
        private AnimalContext _ctx;
        private Dictionary<int, string> _speciesLookup;
        private Dictionary<int, string> _marketLookup;

        public CountIndividualsBySpeciesAndDate()
        {
            _ctx = new AnimalContext();

            _speciesLookup = _ctx.Species
                        .ToDictionary(mc => mc.ID, mc => mc.SpeciesName);

            _marketLookup = _ctx.Markets
                        .ToDictionary(mc => mc.ID, mc => mc.MarketName);
        }

        public void Calculate()
        {
            var summaryPath = @"C:\Users\Jeremy\Desktop\patricia\Individuals\IndividualCountsBySpeciesPerDate.csv";
            File.WriteAllText(summaryPath, String.Empty);

            using (System.IO.StreamWriter file = new System.IO.StreamWriter(summaryPath, true))
            {
                file.WriteLine("Species,Individuals,Date");
            }

            var speciesSummaryPath = @"C:\Users\Jeremy\Desktop\patricia\Individuals\IndividualCountsBySpeciesPerDate.csv";
            File.WriteAllText(speciesSummaryPath, String.Empty);
            using (StreamWriter file = File.AppendText(speciesSummaryPath))
            {
                file.WriteLine("Species,MarketId,StandId,StartDate,EndDate,Individuals");
            }

            var result = new Dictionary<int, int>();

            var speciesIds = _ctx.MarketStandSpeciesDateCount
                                .Select(s => s.SpeciesId).Distinct().ToList();

            var startDate = _ctx.MarketStandSpeciesDateCount.Min(x => x.ObservationDate);
            var endDate = _ctx.MarketStandSpeciesDateCount.Max(x => x.ObservationDate);

            var count = 1;
            foreach (var speciesId in speciesIds)
            {
                var speciesName = _speciesLookup[speciesId];

                result.Add(speciesId, 0);
                var records = _ctx.MarketStandSpeciesDateCount.Where(m => m.SpeciesId == speciesId).ToList();


                var marketIds = records.Select(r => r.MarketId).Distinct().ToList();

                foreach (var marketId in marketIds)
                {
                    var marketName = _marketLookup[marketId];
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

                        using (StreamWriter file = File.AppendText(speciesSummaryPath))
                        {
                            var counts = counter.GetAllPeriods();
                            foreach (var c in counts)
                            {
                                var total = c.TotalIndividuals();
                                if (total > 0)
                                    file.WriteLine(
                                        $"{speciesName},{marketName},{standId}," +
                                        $"{c.StartDate.Date.ToShortDateString()}, " +
                                        $"{c.EndDate.Date.ToShortDateString()}, {total}");
                            }

                        }
                    }
                }


                //using (System.IO.StreamWriter file = new System.IO.StreamWriter(summaryPath, true))
                //{
                //    file.WriteLine($"{speciesName},{result[speciesId]}");
                //}

                Console.WriteLine(count + " of " + speciesIds.Count);
                count++;
            }

            Console.WriteLine("done");
        }
    }
}
