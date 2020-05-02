using AnimalCounter.Context;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AnimalCounter.Models
{
    public class CountablePeriod
    {
        private Dictionary<DateTime, int> countPerDate = new Dictionary<DateTime, int>();
        private int _periodNumber;
        private int _nonStandCount = 0;

        public CountablePeriod() { }
        public CountablePeriod(int periodNumber, DateTime startDate, DateTime endDate)
        {
            _periodNumber = periodNumber;
            StartDate = startDate;
            EndDate = endDate;
        }

        public int PeriodNumber => _periodNumber;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public void AddObservations(List<MarketStandSpeciesDateCount> observations)
        {
            foreach (var ob in observations)
            {
                if (ob.IsNotRealStand())
                {
                    _nonStandCount += ob.QuantityAnimals;
                    continue;
                }

                if (countPerDate.ContainsKey(ob.ObservationDate.Value)) continue;

                var total = observations
                            .Where(x => x.ObservationDate.Value.Date == ob.ObservationDate.Value.Date  
                            && !x.IsNotRealStand())
                            .Sum(x => x.QuantityAnimals);
                countPerDate.Add(ob.ObservationDate.Value, total);
            }
        }

        public int TotalIndividuals()
        {
            var result = 0;
            var individualsPerObservation = new Dictionary<int, int>();

            var count = 0;
            foreach (var ob in countPerDate.OrderBy(x => x.Key))
            {
                individualsPerObservation.Add(count, ob.Value);
                count++;
            }

            for (int i = 0; i < individualsPerObservation.Count - 1; i++)
            {
                if (i == 0)
                {
                    result = individualsPerObservation[0];
                }

                var diff = individualsPerObservation[i + 1] - individualsPerObservation[i];
                if (diff > 0)
                    result += diff;
            }

            return result + _nonStandCount;
        }
    }
}
