//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace DataModel
{
    using System;
    using System.Collections.Generic;
    
    public partial class User_Ingredient_Unlock
    {
        public int UserId { get; set; }
        public int IngredientSkinId { get; set; }
        public int SkinQty { get; set; }
        public bool SkinOwned { get; set; }
    
        public virtual IngredientSkin IngredientSkin { get; set; }
        public virtual Player Player { get; set; }
    }
}
