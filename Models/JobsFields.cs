using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Permissions;

namespace PEF.Models
{
    public class JobsFields
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [ForeignKey("Job")]
        public int JobId { get; set; }
        public virtual Jobs? Job { get; set; }

        [Required]
        [ForeignKey("Field")]
        public int FieldId { get; set; }
        public virtual LookupJobsFields? Field {get;set;}
    }
}
