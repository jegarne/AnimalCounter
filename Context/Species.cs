namespace AnimalCounter.Context
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Species
    {
        [Key]
        public int ID { get; set; }

        [StringLength(255)]
        public string SpeciesName { get; set; }

        public int? AnimalCode { get; set; }

        [StringLength(255)]
        public string CITES { get; set; }

        [StringLength(255)]
        public string TaxaGroup { get; set; }

        [StringLength(255)]
        public string TaxOrder { get; set; }

        [StringLength(255)]
        public string Family { get; set; }

        [StringLength(255)]
        public string CommonName { get; set; }

        public double? Rx { get; set; }
    }
}
