using System;
using System.Collections.Generic;

namespace PotStirrersAPI.Models
{
    public partial class DiceSkin
    {
        public DiceSkin()
        {
            UserDiceUnlocks = new HashSet<UserDiceUnlock>();
            Users = new HashSet<Player>();
        }

        public int DiceSkinId { get; set; }
        public string DiceSkinName { get; set; } = null!;
        public int Rarity { get; set; }

        public virtual ICollection<UserDiceUnlock> UserDiceUnlocks { get; set; }

        public virtual ICollection<Player> Users { get; set; }
    }
}
