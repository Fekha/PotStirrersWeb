using System;
using System.Collections.Generic;

namespace PotStirrersAPI.Models
{
    public partial class Player
    {
        public Player()
        {
            Chests = new HashSet<Chest>();
            Devices = new HashSet<Device>();
            GameAnalyticPlayer1s = new HashSet<GameAnalytic>();
            GameAnalyticPlayer2s = new HashSet<GameAnalytic>();
            GiveawayKeys = new HashSet<GiveawayKey>();
            LoggedIns = new HashSet<LoggedIn>();
            MessageFroms = new HashSet<Message>();
            MessageUsers = new HashSet<Message>();
            UserDiceUnlocks = new HashSet<UserDiceUnlock>();
            UserIngredientUnlocks = new HashSet<UserIngredientUnlock>();
            DiceSkins = new HashSet<DiceSkin>();
            Friends = new HashSet<Player>();
            IngredientSkins = new HashSet<IngredientSkin>();
            Titles = new HashSet<Title>();
            TitlesNavigation = new HashSet<Title>();
            Users = new HashSet<Player>();
        }

        public int UserId { get; set; }
        public string Username { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string Email { get; set; } = null!;
        public int Level { get; set; }
        public int Xp { get; set; }
        public int Stars { get; set; }
        public int Cooked { get; set; }
        public int LocalWins { get; set; }
        public int Wins { get; set; }
        public bool WineMenu { get; set; }
        public bool UseD8s { get; set; }
        public bool DisableDoubles { get; set; }
        public bool PlayAsPurple { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int OnlineWins { get; set; }
        public int Calories { get; set; }
        public int SeasonScore { get; set; }
        public bool DisableTurnDing { get; set; }
        public double GameVolume { get; set; }
        public double TurnVolume { get; set; }
        public double VoiceVolume { get; set; }
        public double EffectsVolume { get; set; }
        public double MasterVolume { get; set; }

        public virtual ICollection<Chest> Chests { get; set; }
        public virtual ICollection<Device> Devices { get; set; }
        public virtual ICollection<GameAnalytic> GameAnalyticPlayer1s { get; set; }
        public virtual ICollection<GameAnalytic> GameAnalyticPlayer2s { get; set; }
        public virtual ICollection<GiveawayKey> GiveawayKeys { get; set; }
        public virtual ICollection<LoggedIn> LoggedIns { get; set; }
        public virtual ICollection<Message> MessageFroms { get; set; }
        public virtual ICollection<Message> MessageUsers { get; set; }
        public virtual ICollection<UserDiceUnlock> UserDiceUnlocks { get; set; }
        public virtual ICollection<UserIngredientUnlock> UserIngredientUnlocks { get; set; }

        public virtual ICollection<DiceSkin> DiceSkins { get; set; }
        public virtual ICollection<Player> Friends { get; set; }
        public virtual ICollection<IngredientSkin> IngredientSkins { get; set; }
        public virtual ICollection<Title> Titles { get; set; }
        public virtual ICollection<Title> TitlesNavigation { get; set; }
        public virtual ICollection<Player> Users { get; set; }
    }
}
