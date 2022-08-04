using System;
using System.Collections.Generic;

namespace PotStirrersAPI.Models
{
    public partial class UserDiceUnlock
    {
        public int UserId { get; set; }
        public int DiceSkinId { get; set; }
        public int DiceFaceUnlockedQty { get; set; }
        public bool DieOwned { get; set; }

        public virtual DiceSkin DiceSkin { get; set; } = null!;
        public virtual Player User { get; set; } = null!;
    }
}
