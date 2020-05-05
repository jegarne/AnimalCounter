using AnimalCounter.Context;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AnimalCounter.CountingFunctions
{
    public class OccurencesOfSpeciesInCageTogether
    {
        private AnimalContext _ctx;
        private Dictionary<int, string> _speciesLookup;
        private Dictionary<int, string> _marketLookup;

        public OccurencesOfSpeciesInCageTogether()
        {
            _ctx = new AnimalContext();

            _speciesLookup = _ctx.Species
                        .ToDictionary(mc => mc.ID, mc => mc.SpeciesName);

            _marketLookup = _ctx.Markets
                        .ToDictionary(mc => mc.ID, mc => mc.MarketName);
        }

        public List<SpeciesInteraction> Calculate()
        {
            var result = new List<SpeciesInteraction>();

            foreach (var market in _ctx.Markets.ToList())
            {
                var records = _ctx.MarketStandCageSpeciesDateCount.Where(m => m.MarketId == market.ID).ToList();
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
                            .Select(m => m.CageNumber);

                        foreach (var cageNumber in cageNumbers)
                        {
                            List<int> speciesIdList;
                            if (cageNumber == "999")
                            {
                                // continue;
                                speciesIdList = records
                                 .Where(s => s.ObservationDate.Value.Date == day.Value.Date
                                 && s.StandNumber == standNumber)
                                 .Select(s => s.SpeciesId).Distinct().ToList();
                            }
                            else
                            {
                                speciesIdList = records
                                .Where(s => s.ObservationDate.Value.Date == day.Value.Date
                                && s.StandNumber == standNumber
                                && s.CageNumber == cageNumber)
                                .Select(s => s.SpeciesId).Distinct().ToList();
                            }

                            if (speciesIdList.Count < 2) continue;
                            Console.WriteLine($"{market.ID}-{standNumber}-{cageNumber}");

                            foreach (var id in speciesIdList)
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
            }

            return result;
        }

        public void WriteOccurencesOfSpeciesInCageTogether()
        {
            var summaryPath = @"C:\Users\Jeremy\Desktop\patricia\OccurencesOfSpeciesInCageTogetherNo999.csv";
            File.WriteAllText(summaryPath, String.Empty);

            using (System.IO.StreamWriter file = new System.IO.StreamWriter(summaryPath, true))
            {
                file.WriteLine("Group1,SpeciesId1,Group2,SpeciesId2,OccurencesInSameCage");
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
