namespace AnimalCounter.Context
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Markets
    {
        [Key]
        public int ID { get; set; }

        [StringLength(255)]
        public string WetmarketName { get; set; }
    }
}
