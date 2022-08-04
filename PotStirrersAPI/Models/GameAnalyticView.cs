using System;
using System.Collections.Generic;

namespace PotStirrersAPI.Models
{
    public partial class GameAnalyticView
    {
        public int Edmxid { get; set; }
        public string? Player1 { get; set; }
        public string? Player2 { get; set; }
        public bool? Player1Won { get; set; }
        public bool? VsCpu { get; set; }
        public int? TotalTurns { get; set; }
        public int? GameMinutes { get; set; }
        public DateTime? GameEndDate { get; set; }
    }
}
