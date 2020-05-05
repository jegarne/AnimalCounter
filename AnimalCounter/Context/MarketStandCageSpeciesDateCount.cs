namespace AnimalCounter.Context
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("MarketStandCageSpeciesDateCount")]
    public partial class MarketStandCageSpeciesDateCount
    {
        [Key]
        public int ID { get; set; }

        public int MarketId { get; set; }
        public string StandNumber { get; set; }
        public string CageNumber { get; set; }
        public int SpeciesId { get; set; }
        public DateTime? ObservationDate { get; set; }
        public int QuantityAnimals { get; set; }
    }
}
