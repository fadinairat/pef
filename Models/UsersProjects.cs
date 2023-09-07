using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace PEF.Models
{
    public class UsersProjects
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [ForeignKey("User")]
        public int UserId { get; set; }
        public virtual User? User { get; set; }


        [Required]
        [ForeignKey("Project")]
        public int ProjectId { get; set; }
        public virtual Projects? Project { get; set; }
    }
}

