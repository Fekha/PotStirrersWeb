using System;
using System.Collections.Generic;

namespace PotStirrersAPI.Models
{
    public partial class GiveawayKey
    {
        public bool IsClaimed { get; set; }
        public int? ClaimedById { get; set; }
        public DateTime? ClaimedTime { get; set; }
        public int RewardAmount { get; set; }
        public int GiveawayKeyId { get; set; }
        public string? KeyCode { get; set; }

        public virtual Player? ClaimedBy { get; set; }
    }
}
