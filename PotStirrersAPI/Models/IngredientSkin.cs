using System;
using System.Collections.Generic;

namespace PotStirrersAPI.Models
{
    public partial class IngredientSkin
    {
        public IngredientSkin()
        {
            UserIngredientUnlocks = new HashSet<UserIngredientUnlock>();
            Users = new HashSet<Player>();
        }

        public int IngredientSkinId { get; set; }
        public string IngredientSkinName { get; set; } = null!;
        public int Rarity { get; set; }

        public virtual ICollection<UserIngredientUnlock> UserIngredientUnlocks { get; set; }

        public virtual ICollection<Player> Users { get; set; }
    }
}
