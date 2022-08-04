using System;
using System.Collections.Generic;

namespace PotStirrersAPI.Models
{
    public partial class PlayerProfile
    {
        public int UserId { get; set; }
        public string Username { get; set; } = null!;
        public int? DailyWins { get; set; }
        public int? WeeklyWins { get; set; }
        public int? AllWins { get; set; }
        public int? AllPvpwins { get; set; }
        public int Level { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int Cooked { get; set; }
        public int Stars { get; set; }
        public int Calories { get; set; }
        public DateTime? LastLogin { get; set; }
        public int SeasonScore { get; set; }
    }
}
