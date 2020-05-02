using AnimalCounter.Context;
using AnimalCounter.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AnimalCounter.CountingFunctions
{
    public class OccurencesOfSpeciesIndividualsInCageTogetherByDate
    {
        private AnimalContext _ctx;
        private Dictionary<int, string> _speciesLookup;
        private Dictionary<int, string> _marketLookup;

        public OccurencesOfSpeciesIndividualsInCageTogetherByDate()
        {
            _ctx = new AnimalContext();

            _speciesLookup = _ctx.Species
                        .ToDictionary(mc => mc.ID, mc => mc.SpeciesName);

            _marketLookup = _ctx.Markets
                        .ToDictionary(mc => mc.ID, mc => mc.MarketName);
        }

        public List<IndividualInteractionWithDate> Calculate()
        {
            var result = new List<IndividualInteractionWithDate>();

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
                            var speciesIdList = records
                            .Where(s => s.ObservationDate.Value.Date == day.Value.Date
                            && s.StandNumber == standNumber
                            && s.CageNumber == cageNumber)
                            .Select(s => s.SpeciesId).Distinct().ToList();


                            if (speciesIdList.Count == 1) continue;


                            Console.WriteLine($"{market.ID}-{standNumber}-{cageNumber}");

                            Dictionary<int, int> speciesIndividuals = new Dictionary<int, int>();

                            foreach (var id in speciesIdList)
                            {
                                var count = (int)records
                                    .Where(s => s.ObservationDate.Value.Date == day.Value.Date
                                    && s.StandNumber == standNumber
                                    && s.CageNumber == cageNumber
                                    && s.SpeciesId == id)
                                    .Sum(s => s.QuantityAnimals);

                                speciesIndividuals[id] = count;
                            }


                            foreach (var id in speciesIdList)
                            {
                                var objectId = $"{market.ID}-{standNumber}-{cageNumber}-{id}";

                                var interaction = result.FirstOrDefault(s => s.Id == objectId);

                                if (interaction == null)
                                {
                                    var newInteraction = new IndividualInteractionWithDate(objectId, id, speciesIndividuals[id], day ?? new DateTime());
                                    result.Add(newInteraction);
                                    interaction = result.FirstOrDefault(s => s.Id == objectId);
                                }

                                interaction.AddMeetings(speciesIndividuals, day ?? new DateTime());
                            }
                        }
                    }
                }
            }

            return result;
        }

        public void WriteOccurencesOfInvidualSpeciesInCageTogether()
        {
            var summaryPath = @"C:\Users\Jeremy\Desktop\patricia\OccurencesOfIndividualsInCageTogetherBySpeciesAndDate.csv";
            File.WriteAllText(summaryPath, String.Empty);

            using (System.IO.StreamWriter file = new System.IO.StreamWriter(summaryPath, true))
            {
                file.WriteLine("Id,Date,Group1,SpeciesId1,Number,Group2,SpeciesId2,Number,OccurencesInSameCage");
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
