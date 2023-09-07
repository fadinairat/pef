using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PEF.Models
{
    public class Permissions
    {
        public int Id { get; set; }

        public string Name { get; set; } = String.Empty;

        public string Controller { get; set; } = String.Empty;

        public string Action { get; set; } = String.Empty;

        public string Area { get; set; } = String.Empty;

        public Boolean Allowed { get; set; } = true;

        [ForeignKey("User")]
        public int UserId { get; set; }
        public virtual User? User { get; set; }

        public Boolean Reserved { get; set; } = false;

        public virtual ICollection<GroupPermissions>? PermissionsList { get; set; }

    }
}
