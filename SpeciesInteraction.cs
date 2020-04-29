using System;
using System.Collections.Generic;
using System.Linq;

namespace AnimalCounter
{
    public class Meeting
    {
        public int SpeciesId { get; set; }
        public int Count { get; set; }
        public DateTime Date { get; set; }
    }

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

                if(previousMeeting != null)
                    previousMeeting.Count++;
                else
                    this.Meetings.Add(new Meeting() { SpeciesId = id, Date = date, Count = 1 });
            }
        }

        // Date,Group1,SpeciesId1,Group2,SpeciesId2,OccurencesInSameCage
        public void WriteMeetings(string path, Dictionary<int, string> speciesLookup, Dictionary<int, string> groupLookup)
        {
            using (System.IO.StreamWriter file = new System.IO.StreamWriter(path, true))
            {
                foreach (var m in this.Meetings)
                {
                    var g1 = groupLookup[this.SpeciesId];
                    var sp1 = speciesLookup[this.SpeciesId];
                    var g2 = groupLookup[m.SpeciesId];
                    var sp2 = speciesLookup[m.SpeciesId];
                    file.WriteLine($"{m.Date.ToString()},{g1},{sp1},{g2},{sp2},{m.Count}");
                }
            }

        }
    }

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

        public void WriteMeetings(string path, Dictionary<int, string> speciesLookup, Dictionary<int, string> groupLookup)
        {
            using (System.IO.StreamWriter file = new System.IO.StreamWriter(path, true))
            {
                foreach (var kv in this.Meetings)
                {
                    var g1 = groupLookup[this.SpeciesId];
                    var sp1 = speciesLookup[this.SpeciesId];
                    var g2 = groupLookup[kv.Key];
                    var sp2 = speciesLookup[kv.Key];
                    file.WriteLine($"{g1},{sp1},{g2},{sp2},{kv.Value}");
                }
            }

        }
    }

    public class IndividualMeeting
    {
        public int SpeciesId { get; set; }
        public int Individuals { get; set; }
        public DateTime Date { get; set; }
    }

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
        public void WriteMeetings(string path, Dictionary<int, string> speciesLookup, Dictionary<int, string> groupLookup)
        {
            using (System.IO.StreamWriter file = new System.IO.StreamWriter(path, true))
            {
                foreach (var m in this.Meetings)
                {
                    var g1 = groupLookup[this.SpeciesId];
                    var sp1 = speciesLookup[this.SpeciesId];
                    var g2 = groupLookup[m.SpeciesId];
                    var sp2 = speciesLookup[m.SpeciesId];
                    file.WriteLine($"{this.Id},{m.Date.ToString()},{g1},{sp1},{this.Quantity},{g2},{sp2},{m.Individuals},{this.Quantity * m.Individuals}");
                }
            }

        }
    }
}
