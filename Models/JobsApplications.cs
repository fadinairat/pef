using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PEF.Models
{
    public class JobsApplications
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Display(Name = "Member")]
        public int MemberId { get; set; }

        public virtual Members Member { get; set; }

        [Required]
        [Display(Name = "Job Title")]
        [ForeignKey("Job")]
        public int JobId { get; set; }
        public virtual Jobs? Job { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Apply Date")]
        [DisplayFormat(DataFormatString = "{0:dd-MMM-yyyy}")]
        public DateTime? ApplyDate { get; set; } = DateTime.Now;

        [DataType(DataType.Date)]
        [Display(Name = "Employment Start Date")]
        [DisplayFormat(DataFormatString = "{0:dd-MMM-yyyy}")]
        public DateTime? StartDate { get; set; } = null;

        [DataType(DataType.Date)]
        [Display(Name = "Employment End Date")]
        [DisplayFormat(DataFormatString = "{0:dd-MMM-yyyy}")]
        public DateTime? EndDate { get; set; } = null;


        [Display(Name = "Forwarded")]
        public Boolean Forwarded { get; set; } = false; 

        [Required]
        [Display(Name = "Status")]
        public int Status { get; set; } = 0;//0:Pending, 1:Approved, 2:Under Study, 3:Waiting List, 4:Review, 5:Employed, 6:Rejected

        [DisplayName("Reject Reason")]
        public string? RejectReason { get; set; } = string.Empty;

        public Boolean Deleted { get; set; } = false;

        public virtual ICollection<JobsAppsAttachments>? JobsAppsAttachments { get; set; }
        public virtual ICollection<JobsStatusTracking>? JobsStatusTracking { get; set; }
    }
}
