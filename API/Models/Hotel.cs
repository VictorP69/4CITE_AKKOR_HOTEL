using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection.Metadata;

namespace API.Models
{
    [Table("Hotel")]
    public class Hotel
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [Column("Name")]
        public string Name { get; set; }

        [Required]
        [Column("Location")]
        public string Location { get; set; }

        [Required]
        [Column("Description")]
        public string Description { get; set; }

        [Required]
        [Column("NightPrice")]
        public double NightPrice { get; set; }

        [Required]
        public List<HotelBlob> PictureList { get; set; } = new List<HotelBlob>();
    }
}
