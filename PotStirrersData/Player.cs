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
    
    public partial class Player
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Player()
        {
            this.GameAnalytics = new HashSet<GameAnalytic>();
            this.GameAnalytics1 = new HashSet<GameAnalytic>();
            this.Devices = new HashSet<Device>();
            this.LoggedIns = new HashSet<LoggedIn>();
            this.User_Purchase = new HashSet<User_Purchase>();
            this.Messages = new HashSet<Message>();
            this.Player1 = new HashSet<Player>();
            this.Players = new HashSet<Player>();
            this.Messages1 = new HashSet<Message>();
            this.GiveawayKeys = new HashSet<GiveawayKey>();
            this.User_Dice_Unlock = new HashSet<User_Dice_Unlock>();
            this.DiceSkins = new HashSet<DiceSkin>();
            this.IngredientSkins = new HashSet<IngredientSkin>();
            this.Chests = new HashSet<Chest>();
        }
    
        public int UserId { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public int Level { get; set; }
        public int Xp { get; set; }
        public int Stars { get; set; }
        public int Cooked { get; set; }
        public int LocalWins { get; set; }
        public int Wins { get; set; }
        public int SelectedMeat { get; set; }
        public int SelectedVeggie { get; set; }
        public int SelectedFruit { get; set; }
        public int SelectedDie { get; set; }
        public int SelectedDie2 { get; set; }
        public bool WineMenu { get; set; }
        public bool UseD8s { get; set; }
        public bool DisableDoubles { get; set; }
        public bool PlayAsPurple { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public int SelectedFourthIngredient { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<GameAnalytic> GameAnalytics { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<GameAnalytic> GameAnalytics1 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Device> Devices { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<LoggedIn> LoggedIns { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<User_Purchase> User_Purchase { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Message> Messages { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Player> Player1 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Player> Players { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Message> Messages1 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<GiveawayKey> GiveawayKeys { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<User_Dice_Unlock> User_Dice_Unlock { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DiceSkin> DiceSkins { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<IngredientSkin> IngredientSkins { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Chest> Chests { get; set; }
    }
}
