using Microsoft.CodeAnalysis;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PEF.Models
{
    public class JobsStatusTracking
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [ForeignKey("Job")]
        public int JobAppId { get; set; }
        public virtual JobsApplications? JobApp { get; set; }

        public int Status { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:dd-MMM-yyyy}")]
        public DateTime? ApplyDate { get; set; } = DateTime.Now;

        [Required]
        [ForeignKey("Employer")]
        public int EmployerId { get; set; }
        public virtual Employers? Employer { get; set; }

        [DisplayName("Employment Start Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MMM-yyyy}")]
        public DateTime? StartDate { get; set; } = null;

        [DisplayName("Employment End Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MMM-yyyy}")]
        public DateTime? EndDate { get; set; } = null;

        public Boolean Deleted { get; set; } = false;
    }
}
