using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PEF.Models
{
	public class City
	{
		[Key]
		public int Id { get; set; }

		[Required]
		[StringLength(50)]
		public string Name { get; set; }

		[Required]
		[StringLength(50)]
		public string ArName { get; set; }

		[Required]
		public string District { get; set; } = "WestBank";

		public int Priority { get; set; } = 999999;

		public virtual ICollection<Members>? Members { get; set; }
		public virtual ICollection<City>? Jobs { get; set; }
        public virtual ICollection<Projects>? Projects { get; set; }
        public virtual ICollection<Employers>? Employers { get; set; }
        public virtual ICollection<Villages>? Villages { get; set; }
        public virtual ICollection<JobsCities>? JobsCities { get; set; }
    }
}
