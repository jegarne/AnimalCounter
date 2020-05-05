using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnimalCounter.Models
{
    public class Meeting
    {
        public int SpeciesId { get; set; }
        public int Count { get; set; }
        public DateTime Date { get; set; }
    }
}
