using System;
using System.Collections.Generic;
using System.Linq;

namespace AnimalCounter
{
    public class PeriodCounter
    {
        public DateTime StartDate { get; }
        public DateTime EndDate { get; }
        public int SpeciesId { get; }

        private List<CountablePeriod> _periods = new List<CountablePeriod>();
        private int _activePeriodNumber = 0;

        public PeriodCounter(DateTime startDate, DateTime endDate, int speciesId)
        {
            StartDate = startDate;
            EndDate = endDate;
            SpeciesId = speciesId;
            BuildCountablePeriods();
        }

        public CountablePeriod GetActivePeriod() {
            return _periods.FirstOrDefault(p => p.PeriodNumber == _activePeriodNumber);
        }

        public IEnumerable<CountablePeriod> GetAllPeriods()
        {
            return _periods;
        }

        public void NextPeriod()
        {
            _activePeriodNumber++;
        }

        public void ResetPeriod()
        {
            _activePeriodNumber = 0;
        }

        private void BuildCountablePeriods()
        {
            _periods.Add(new CountablePeriod(0, StartDate, StartDate.AddDays(10)));
            CountablePeriod lastPeriod = _periods.First();
            while (lastPeriod.EndDate < EndDate)
            {
                var startDate = lastPeriod.EndDate.AddDays(1);
                _periods.Add(new CountablePeriod(_periods.Count(), startDate, startDate.AddDays(10)));
                lastPeriod = _periods.First(p => p.PeriodNumber == _periods.Max(ps => ps.PeriodNumber));
            }
        }
    }

    public class CountablePeriod
    {
        private Dictionary<int, int> _observations = new Dictionary<int, int>();
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

        public void AddObservations(Dictionary<DateTime, int> observedDates)
        {
            var count = 0;
            foreach (var kv in observedDates.OrderBy(kv => kv.Key))
            {
                _observations.Add(count, kv.Value);
                count++;
            }
        }

        public int TotalIndividuals()
        {
            var result = 0;

            if (_observations.Count == 1)
                return _observations[0];

            for (int i = 0; i < _observations.Count-1; i++)
            {
                if (i == 0)
                {
                    result = _observations[0];
                }

                var diff = _observations[i + 1] - _observations[i];
                if (diff > 0)
                    result += diff;
            }

            return result;
        }        
    }
}
