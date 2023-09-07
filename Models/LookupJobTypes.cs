using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PEF.Models
{
    public class LookupJobTypes
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(250)]
        [DisplayName("العنوان")]
        public string Name { get; set; } = String.Empty;

        [Required]
        [DisplayName("العنوان العربي")]
        [StringLength(250)]
        public string ArName { get; set; } = String.Empty;

        public Boolean Deleted { get; set; } = false;

        public virtual ICollection<Jobs>? Jobs { get; set; }
    }
}
