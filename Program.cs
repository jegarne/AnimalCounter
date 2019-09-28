using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnimalCounter
{
    class Program
    {
        static void Main(string[] args)
        {
            var sc = new SpeciesCounter(new List<int> {1, 4, 7, 8});
            sc.MaxAndMinIndividualsPerMarketPerDay();
            Console.ReadKey();
        }
    }
}
