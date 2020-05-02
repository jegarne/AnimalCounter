using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnimalCounter.Models
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

        public CountablePeriod GetActivePeriod()
        {
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
                _periods.Add(new CountablePeriod(_periods.Count(), startDate, startDate.AddDays(9)));
                lastPeriod = _periods.First(p => p.PeriodNumber == _periods.Max(ps => ps.PeriodNumber));
            }
        }
    }
}
