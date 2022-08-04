using System;
using System.Collections.Generic;

namespace PotStirrersAPI.Models
{
    public partial class UserIngredientUnlock
    {
        public int UserId { get; set; }
        public int IngredientSkinId { get; set; }
        public int SkinQty { get; set; }
        public bool SkinOwned { get; set; }

        public virtual IngredientSkin IngredientSkin { get; set; } = null!;
        public virtual Player User { get; set; } = null!;
    }
}
