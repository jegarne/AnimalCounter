using AnimalCounter.CountingFunctions;
using System;

namespace AnimalCounter
{
    class Program
    {
        static void Main(string[] args)
        {
            var sc = new CountIndividualsInStandBySpeciesAndTimePeriod();
            sc.Calculate();
            Console.ReadKey();
        }
    }
}
