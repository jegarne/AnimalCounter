using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnimalCounter.Models
{
    public class Observation
    {
        public int Index { get; set; }
        public int Individuals { get; set; }
        public string Stand { get; set; }
        public int Date { get; set; }

        public bool IsNotStand()
        {
            return this.Stand == "999" || !int.TryParse(this.Stand, out int parsed);
        }

    }
}
