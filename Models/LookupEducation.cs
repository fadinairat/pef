using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PEF.Models
{
    public class LookupEducation
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

        [Required]
        [DisplayName("Eduation Level Type")]
        public int Type { get; set; } = 1; // 0: For all Types, 1:Worker, 2: Profession, 3: Graduate

        public Boolean Deleted { get; set; } = false;

        public virtual ICollection<MembersEducation>? MembersEducation { get; set; }
        public virtual ICollection<MembersFamily>? MembersFamily { get; set; }
    }
}
