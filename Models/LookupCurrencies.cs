using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PEF.Models
{
    public class LookupCurrencies
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(250)]
        [DisplayName("Title")]
        public string Name { get; set; } = String.Empty;

        [Required]
        [DisplayName("Arabic Title")]
        [StringLength(250)]
        public string ArName { get; set; } = String.Empty;

        public string? IconHtml { get; set; } = String.Empty;

        public Boolean Deleted { get; set; } = false;

        public virtual ICollection<Projects>? Projects { get; set; }
        public virtual ICollection<Jobs>? Jobs { get; set; }
    }
}
