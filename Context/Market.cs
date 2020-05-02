namespace AnimalCounter.Context
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("Market2")]
    public partial class Market
    {
        [Key]
        public int ID { get; set; }

        [Column("Market")]
        [StringLength(255)]
        public string MarketName { get; set; }
    }
}
