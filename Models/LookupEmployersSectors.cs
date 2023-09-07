using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace PEF.Models
{
    public class LookupEmployersSectors
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(250)]
        [DisplayName("Name")]
        public string Name { get; set; } = String.Empty;

        [Required]
        [DisplayName("Arabic Name")]
        [StringLength(250)]
        public string ArName { get; set; } = String.Empty;

        [DisplayName("Type")]
        public int Type { get; set; } = 1;   //1: company, 2: Institution

        public Boolean Deleted { get; set; } = false;

        public virtual ICollection<Employers>? Employers { get; set; }
    }
}
