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
    
    public partial class GiveawayKey
    {
        public bool IsClaimed { get; set; }
        public Nullable<int> ClaimedById { get; set; }
        public Nullable<System.DateTime> ClaimedTime { get; set; }
        public int RewardAmount { get; set; }
        public int GiveawayKeyId { get; set; }
        public string KeyCode { get; set; }
    
        public virtual Player Player { get; set; }
    }
}