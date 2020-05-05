using AnimalCounter.Context;
using AnimalCounter.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AnimalCounter.CountingFunctions
{
    public class OccurencesOfSpeciesInCageTogetherByDate
    {
        private AnimalContext _ctx;
        private Dictionary<int, string> _speciesLookup;
        private Dictionary<int, string> _marketLookup;

        public OccurencesOfSpeciesInCageTogetherByDate()
        {
            _ctx = new AnimalContext();

            _speciesLookup = _ctx.Species
                        .ToDictionary(mc => mc.ID, mc => mc.SpeciesName);

            _marketLookup = _ctx.Markets
                        .ToDictionary(mc => mc.ID, mc => mc.MarketName);
        }

        public List<SpeciesInteractionWithDate> Calculate()
        {
            var result = new List<SpeciesInteractionWithDate>();

            foreach (var market in _ctx.Markets.ToList())
            {
                var records = _ctx.MarketStandCageSpeciesDateCount.Where(m => m.MarketId == market.ID).OrderBy(x => x.ObservationDate).ToList();
                var stands = records.Select(m => m.StandNumber).Distinct();

                foreach (var standNumber in stands.OrderBy(d => d))
                {
                    var dates = records.Where(s => s.StandNumber == standNumber).Select(m => m.ObservationDate).Distinct();
                    foreach (var day in dates.OrderBy(d => d.Value))
                    {
                        var cageNumbers = records
                            .Where(s => s.StandNumber == standNumber
                            && s.ObservationDate == day
                            && s.CageNumber != null)
                            .Select(m => m.CageNumber).Distinct();

                        foreach (var cageNumber in cageNumbers)
                        {
                            List<int> speciesIdList;

                            speciesIdList = records
                            .Where(s => s.ObservationDate.Value.Date == day.Value.Date
                            && s.StandNumber == standNumber
                            && s.CageNumber == cageNumber)
                            .Select(s => s.SpeciesId).Distinct().ToList();


                            if (speciesIdList.Count == 1) continue;
                            Console.WriteLine($"{market.ID}-{standNumber}-{cageNumber}");

                            foreach (var id in speciesIdList)
                            {
                                var interaction = result.FirstOrDefault(s => s.SpeciesId == id);

                                if (interaction == null)
                                {
                                    var newInteraction = new SpeciesInteractionWithDate(id);
                                    result.Add(newInteraction);
                                    interaction = result.FirstOrDefault(s => s.SpeciesId == id);
                                }

                                interaction.AddMeetings(speciesIdList, day ?? new DateTime());
                            }
                        }
                    }
                }
            }

            return result;
        }

        public void WriteOccurencesOfSpeciesInCageTogetherByDate()
        {
            var summaryPath = @"C:\Users\Jeremy\Desktop\patricia\OccurencesOfSpeciesInCageTogetherByDate.csv";
            File.WriteAllText(summaryPath, String.Empty);

            using (System.IO.StreamWriter file = new System.IO.StreamWriter(summaryPath, true))
            {
                file.WriteLine("Date,Group1,SpeciesId1,Group2,SpeciesId2,OccurencesInSameCage");
            }

            var result = this.Calculate();

            foreach (var si in result)
            {
                si.WriteMeetings(summaryPath, _speciesLookup);
            }

            Console.WriteLine("done");

        }
    }
}
