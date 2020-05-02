using AnimalCounter.Context;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AnimalCounter.Models
{
    public class CountablePeriod
    {
        private Dictionary<int, MarketStandSpeciesDateCount> _observations = new Dictionary<int, MarketStandSpeciesDateCount>();
        private int _periodNumber;

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
            var count = 0;
            foreach (var ob in observations)
            {
                _observations.Add(count, ob);
                count++;
            }
        }

        public int TotalIndividuals()
        {
            var result = 0;

            if (_observations.Count == 1)
                return _observations[0].QuantityAnimals;

            for (int i = 0; i < _observations.Count-1; i++)
            {
                if (i == 0)
                {
                    result = _observations[0].QuantityAnimals;
                }

                if (_observations[i].IsNotRealStand())
                {
                    result += _observations[i].QuantityAnimals;
                }
                else
                {

                    var diff = _observations[i + 1].QuantityAnimals - _observations[i].QuantityAnimals;
                    if (diff > 0)
                        result += diff;
                }
            }

            return result;
        }        
    }
}
