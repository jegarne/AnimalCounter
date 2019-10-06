using System.Collections.Generic;
using System.Linq;

namespace AnimalCounter
{
    public class SpeciesInteraction
    {
        public SpeciesInteraction(int speciesId)
        {
            SpeciesId = speciesId;
        }

        public int SpeciesId { get; set; }
        public Dictionary<int, int> Meetings { get; } = new Dictionary<int, int>();

        public void AddMeetings(List<int> ids)
        {
            var otherIds = ids.Where(s => s != this.SpeciesId);

            foreach (var id in otherIds)
            {
                if (this.Meetings.ContainsKey(id))
                    this.Meetings[id] = this.Meetings[id] + 1;
                else
                    this.Meetings.Add(id, 1);
            }
        }

        public void UpdateGrid(Dictionary<int, Dictionary<int, int>> grid)
        {
            foreach (var m in this.Meetings)
            {
                grid[this.SpeciesId][m.Key] = grid[this.SpeciesId][m.Key] + m.Value;
            }
        }

        public void WriteMeetings(string path, Dictionary<int, string> speciesLookup)
        {
            using (System.IO.StreamWriter file = new System.IO.StreamWriter(path, true))
            {
                foreach (var kv in this.Meetings)
                {
                    var sp1 = speciesLookup[this.SpeciesId];
                    var sp2 = speciesLookup[kv.Key];
                    file.WriteLine($"{sp1},{sp2},{kv.Value}");
                }
            }

        }
    }
}
