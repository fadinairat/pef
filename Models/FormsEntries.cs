using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PEF.Models
{
    public class FormsEntries
    {
        [Required]
        [Key]
        public int Id { get; set; }

        
        [DisplayName("Job Seeker")]
        [ForeignKey("Member")]
        public int? MemberId { get; set; }

        public virtual Members? Member { get; set; }

        [DisplayName("Employer/Association")]
        [ForeignKey("Employer")]
        public int? EmployerId { get; set; }
        public virtual Employers? Employer { get; set; }

        [Required]
        [ForeignKey("Form")]
        public int FormId { get; set; }
        public virtual Forms? Form { get; set; }

        
        [ForeignKey("Job Application")]
        public int? JobId { get; set; }
        public virtual Jobs? Job { get; set; }

        [DisplayName("Form Entry For")]
        public string? Type { get; set; } = "Job"; //Job/Page/Project


        [Required]
        [DisplayName("IP Address")]
        public string? IpAddress { get; set; } = String.Empty;


        [Required]
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:dd-MMM-yyyy HH:mm}")]
        public DateTime AddedTime { get; set; } = DateTime.Now;

        
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:dd-MMM-yyyy HH:mm}")]
        public DateTime? EditedTime { get; set; } = DateTime.Now;
        public Boolean Deleted { get; set; } = false;

        public virtual ICollection<FormsEntriesFields>? Entries { get; set; }
    }
}
