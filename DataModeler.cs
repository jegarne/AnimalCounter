using System.Collections.Generic;
using System.Linq;

namespace AnimalCounter
{
    public class DataModeler
    {
        public static Dictionary<int, Dictionary<int, int>> BuildSpeciesGrid(List<int> speciesIds)
        {
            var result = new Dictionary<int, Dictionary<int, int>>();
            var orderedIds = speciesIds.OrderBy(x => x);

            foreach (var yId in orderedIds)
            {
                var xDictionary = new Dictionary<int, int>();
                foreach (var id in orderedIds)
                {
                    xDictionary.Add(id, 0);
                }
                result.Add(yId, xDictionary);
            }

            return result;
        }
    }
}
