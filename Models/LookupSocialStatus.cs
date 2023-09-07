using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PEF.Models
{
    public class LookupSocialStatus
    {
        [Key]
        public int Id { get; set; }

        public string Name { get; set; } = String.Empty;

        public string ArName { get; set; } = String.Empty;

        public Boolean Deleted { get; set; } = false;

        public virtual ICollection<Members>? Members { get; set; }
        public virtual ICollection<Jobs>? Jobs { get; set; }
    }
}
