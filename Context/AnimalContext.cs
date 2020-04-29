namespace AnimalCounter.Context
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class AnimalContext : DbContext
    {
        public AnimalContext()
            : base("name=AnimalContext")
        {
        }

        public virtual DbSet<Markets> Markets { get; set; }
        public virtual DbSet<MarketSpeciesDateCount> MarketSpeciesDateCount { get; set; }
        public virtual DbSet<MarketStandSpeciesDateCount> MarketStandSpeciesDateCount { get; set; }
        public virtual DbSet<MarketStandCageSpeciesDateCount> MarketStandCageSpeciesDateCount { get; set; }
        public virtual DbSet<ObservationDates> ObservationDates { get; set; }
        public virtual DbSet<Species> Species { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
        }
    }
}
