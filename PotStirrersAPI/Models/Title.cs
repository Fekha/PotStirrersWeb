using System;
using System.Collections.Generic;

namespace PotStirrersAPI.Models
{
    public partial class Title
    {
        public Title()
        {
            Users = new HashSet<Player>();
            UsersNavigation = new HashSet<Player>();
        }

        public int TitleId { get; set; }
        public string TitleName { get; set; } = null!;
        public string EarnDescription { get; set; } = null!;

        public virtual ICollection<Player> Users { get; set; }
        public virtual ICollection<Player> UsersNavigation { get; set; }
    }
}
