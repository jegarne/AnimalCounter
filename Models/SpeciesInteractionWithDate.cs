using System;
using System.Collections.Generic;
using System.Linq;

namespace AnimalCounter.Models
{
    public class SpeciesInteractionWithDate
    {
        public SpeciesInteractionWithDate(int speciesId)
        {
            SpeciesId = speciesId;
        }

        public int SpeciesId { get; set; }
        public List<Meeting> Meetings { get; } = new List<Meeting>();

        public void AddMeetings(List<int> ids, DateTime date)
        {
            var otherIds = ids.Where(s => s != this.SpeciesId);

            foreach (var id in otherIds)
            {
                var previousMeeting = this.Meetings.FirstOrDefault(x => x.SpeciesId == id && x.Date.Date == date.Date);

                if (previousMeeting != null)
                    previousMeeting.Count++;
                else
                    this.Meetings.Add(new Meeting() { SpeciesId = id, Date = date, Count = 1 });
            }
        }

        // Date,SpeciesId1,SpeciesId2,OccurencesInSameCage
        public void WriteMeetings(string path, Dictionary<int, string> speciesLookup)
        {
            using (System.IO.StreamWriter file = new System.IO.StreamWriter(path, true))
            {
                foreach (var m in this.Meetings)
                {
                    var sp1 = speciesLookup[this.SpeciesId];
                    var sp2 = speciesLookup[m.SpeciesId];
                    file.WriteLine($"{m.Date.ToString()},{sp1},{sp2},{m.Count}");
                }
            }

        }
    }
}
