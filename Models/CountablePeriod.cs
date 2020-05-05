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
        private bool _isNotRealStand = false;

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
            _isNotRealStand = observations.FirstOrDefault()?.IsNotRealStand() ?? false;

            foreach (var ob in observations)
            {
                if (countPerDate.ContainsKey(ob.ObservationDate.Value)) continue;

                var total = observations
                            .Where(x => x.ObservationDate.Value.Date == ob.ObservationDate.Value.Date)
                            .Sum(x => x.QuantityAnimals);
                countPerDate.Add(ob.ObservationDate.Value, total);
            }
        }

        public int TotalIndividuals()
        {
            if (_isNotRealStand)
                return countPerDate.Sum(x => x.Value);

            var result = 0;
            var individualsPerObservation = new Dictionary<int, int>();

            var count = 0;
            foreach (var ob in countPerDate.OrderBy(x => x.Key))
            {
                individualsPerObservation.Add(count, ob.Value);
                count++;
            }

            if (individualsPerObservation.Count == 1)
                return individualsPerObservation[0];

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

            return result;
        }
    }
}
