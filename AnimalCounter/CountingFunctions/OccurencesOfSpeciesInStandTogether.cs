using AnimalCounter.Context;
using AnimalCounter.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AnimalCounter.CountingFunctions
{
    public class OccurencesOfSpeciesInStandTogether
    {
        private AnimalContext _ctx;
        private Dictionary<int, string> _speciesLookup;
        private Dictionary<int, string> _marketLookup;

        public OccurencesOfSpeciesInStandTogether()
        {
            _ctx = new AnimalContext();

            _speciesLookup = _ctx.Species
                        .ToDictionary(mc => mc.ID, mc => mc.SpeciesName);

            _marketLookup = _ctx.Markets
                        .ToDictionary(mc => mc.ID, mc => mc.MarketName);
        }

        public List<SpeciesInteraction> Calculate(bool filterByIncludedSpecies)
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
                        List<int> speciesIdList;
                        if (filterByIncludedSpecies)
                        {
                            speciesIdList = records
                                .Where(s => s.ObservationDate.Value.Date == day.Value.Date
                                && s.StandNumber == standNumber)
                                .Select(s => s.SpeciesId).ToList();
                        }
                        else
                        {
                            speciesIdList = records
                            .Where(s => s.ObservationDate.Value.Date == day.Value.Date
                            && s.StandNumber == standNumber)
                            .Select(s => s.SpeciesId).ToList();
                        }

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
            var summaryPath = @"C:\Users\Jeremy\Desktop\patricia\OccurencesOfSpeciesInStandTogether.csv";
            File.WriteAllText(summaryPath, String.Empty);

            using (System.IO.StreamWriter file = new System.IO.StreamWriter(summaryPath, true))
            {
                file.WriteLine("SpeciesId1,SpeciesId2,OccurencesInSameStand");
            }

            var result = this.Calculate(false);

            foreach (var si in result)
            {
                si.WriteMeetings(summaryPath, _speciesLookup);
            }

            Console.WriteLine("done");

        }

        public void WriteOccurencesOfSpeciesInStandTogetherGrid()
        {
            var grid = DataModeler.BuildSpeciesGrid(new List<int>());
            var result = this.Calculate(true);

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
    }
}
