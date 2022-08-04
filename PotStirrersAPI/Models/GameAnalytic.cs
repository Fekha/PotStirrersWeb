using System;
using System.Collections.Generic;

namespace PotStirrersAPI.Models
{
    public partial class GameAnalytic
    {
        public int GameId { get; set; }
        public DateTime GameStartTime { get; set; }
        public DateTime? GameEndTime { get; set; }
        public int Player1Id { get; set; }
        public int Player2Id { get; set; }
        public int? Player1CookedNum { get; set; }
        public int? Player2CookedNum { get; set; }
        public bool WineMenu { get; set; }
        public int? TotalTurns { get; set; }
        public bool Quit { get; set; }
        public int Wager { get; set; }
        public bool IsFriendGame { get; set; }
        public bool IsCpuGame { get; set; }

        public virtual Player Player1 { get; set; } = null!;
        public virtual Player Player2 { get; set; } = null!;
    }
}
