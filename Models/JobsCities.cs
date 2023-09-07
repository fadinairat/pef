using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Permissions;

namespace PEF.Models
{
    public class JobsCities
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [ForeignKey("Job")]
        public int JobId { get; set; }
        public virtual Jobs? Job { get; set; }

        [Required]
        [ForeignKey("City")]
        public int CityId { get; set; }
        public virtual City? City { get; set; }
    }
}
