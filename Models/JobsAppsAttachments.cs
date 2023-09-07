using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PEF.Models
{
    public class JobsAppsAttachments
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [DisplayName("Job Application Number")]
        [ForeignKey("Job")]
        public int JobAppId { get; set; }

        public virtual JobsApplications? JobApp { get; set; }

        [Required]
        [DisplayName("Member")]
        [ForeignKey("Member")]
        public int MemberId { get; set; }
        public virtual Members? Member { get; set; }

        [Required]
        [DisplayName("Attachment Title")]
        [StringLength(250)]
        public string FileName { get; set; } = String.Empty;

        [Required]
        [DisplayName("Attachment")]
        [StringLength(250)]
        public string FilePath { get; set; } = String.Empty;

        [DisplayName("Add Time")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:dd-MMM-yyyy HH:mm}")]
        public DateTime AddDate { get; set; } = DateTime.Now;

        public Boolean Deleted { get; set; } = false;
    }
}
