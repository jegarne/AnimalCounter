namespace AnimalCounter.Context
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class ObservationDates
    {
        [Key]
        public int ID { get; set; }

        public DateTime? DateObserved { get; set; }
    }
}
