namespace AnimalCounter.Context
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("MarketStandSpeciesDateCount2")]
    public partial class MarketStandSpeciesDateCount
    {
        public MarketStandSpeciesDateCount()
        {
        }

        public MarketStandSpeciesDateCount(string standNumber, DateTime? observationDate, int quantityAnimals)
        {
            StandNumber = standNumber;
            ObservationDate = observationDate;
            QuantityAnimals = quantityAnimals;
        }

        [Key]
        public int ID { get; set; }

        public int MarketId { get; set; }
        public string StandNumber { get; set; }
        public int SpeciesId { get; set; }

        public DateTime? ObservationDate { get; set; }

        public int QuantityAnimals { get; set; }

        public bool IsNotRealStand()
        {
            return this.StandNumber == "999" || !int.TryParse(this.StandNumber, out int parsed);
        }
    }
}
