using System;
using System.Collections.Generic;
using System.Linq;

namespace AnimalCounter.Models
{

    public class IndividualInteractionWithDate
    {
        public IndividualInteractionWithDate(string id, int speciesId, int quantity, DateTime date)
        {
            Id = id;
            SpeciesId = speciesId;
            Quantity = quantity;
            Date = date;
        }

        public string Id { get; set; }
        public int SpeciesId { get; set; }
        public int Quantity { get; set; }
        public DateTime Date { get; set; }
        public List<IndividualMeeting> Meetings { get; } = new List<IndividualMeeting>();

        public void AddMeetings(Dictionary<int, int> ids, DateTime date)
        {
            var otherIds = ids.Where(s => s.Key != this.SpeciesId);

            foreach (var kv in otherIds)
            {
                var previousMeeting = this.Meetings.FirstOrDefault(x => x.SpeciesId == kv.Key && x.Date.Date == date.Date);

                if (previousMeeting != null)
                    previousMeeting.Individuals += kv.Value;
                else
                    this.Meetings.Add(new IndividualMeeting() { SpeciesId = kv.Key, Date = date, Individuals = kv.Value });
            }
        }

        // Date,Group1,SpeciesId1,Number,Group2,SpeciesId2,Number,OccurencesInSameCage
        public void WriteMeetings(string path, Dictionary<int, string> speciesLookup)
        {
            using (System.IO.StreamWriter file = new System.IO.StreamWriter(path, true))
            {
                foreach (var m in this.Meetings)
                {
                    var sp1 = speciesLookup[this.SpeciesId];
                    var sp2 = speciesLookup[m.SpeciesId];
                    file.WriteLine($"{this.Id},{m.Date.ToString()},{sp1},{this.Quantity},{sp2},{m.Individuals},{this.Quantity * m.Individuals}");
                }
            }

        }
    }
}
