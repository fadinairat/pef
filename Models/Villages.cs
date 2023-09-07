using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PEF.Models
{
    public class Villages
    {
        [Required]
        [Key]
        public int Id { get; set; }

        public string Name { get; set; } = String.Empty;

        public string ArName { get; set; } = String.Empty;

        [Required]
        [DisplayName("City")]
        [ForeignKey("City")]
        public int? CityId { get; set; }
        public virtual City? City { get; set; }

        public Boolean Deleted { get; set; } = false;

        public virtual ICollection<Members>? Members { get; set; }
        public virtual ICollection<Jobs>? Jobs { get; set; }
        public virtual ICollection<Employers>? Employers { get; set; }
    }
}
