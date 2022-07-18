using DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PotStirrersWebAPI.Models
{
    public class ProfileDTO
    {
        public ProfileDTO(PlayerProfile x)
        {
            Username = x.Username;
            DailyWins = x.DailyWins;
            WeeklyWins = x.WeeklyWins;
            AllWins = x.AllWins;
            AllPVPWins=x.AllPVPWins;
            Level = x.Level;
            CreatedDate = x.CreatedDate;
            Cooked = x.Cooked;
            Stars = x.Stars;
            Calories = x.Calories;
            LastLogin = x.LastLogin;
        }
        public string Username { get; set; }
        public Nullable<int> DailyWins { get; set; }
        public Nullable<int> WeeklyWins { get; set; }
        public Nullable<int> AllWins { get; set; }
        public Nullable<int> AllPVPWins { get; set; }
        public int Level { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public int Cooked { get; set; }
        public int Stars { get; set; }
        public int Calories { get; set; }
        public bool IsOnline { get; set; }
        public Nullable<System.DateTime> LastLogin { get; set; }
    }
}