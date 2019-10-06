using System;
using System.Collections.Generic;
using System.Linq;

namespace AnimalCounter
{
    public class SpeciesCount
    {
        public SpeciesCount(DateTime date, List<int> speciesIds)
        {
            Date = date;
            SpeciesIds = speciesIds;
        }

        public DateTime Date { get; set; }
        public int Count
        {
            get
            {
                return this.SpeciesIds.Distinct().Count();
            }
        }
        public List<int> SpeciesIds { get; set; }
    }
}
