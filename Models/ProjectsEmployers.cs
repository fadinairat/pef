using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace PEF.Models
{
    public class ProjectsEmployers
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [ForeignKey("Employer")]
        public int EmployerId { get; set; }
        public virtual Employers? Employer { get; set; }


        [Required]
        [ForeignKey("Project")]
        public int ProjectId { get; set; }
        public virtual Projects? Project { get; set; }
    }
}
