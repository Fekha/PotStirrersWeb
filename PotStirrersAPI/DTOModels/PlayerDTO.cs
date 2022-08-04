
using PotStirrersAPI.Models;
using PotStirrersWebAPI.Controllers;
using System.Collections.Generic;
using System.Linq;

public class PlayerDTO
{
    public PlayerDTO(Player x, PlayerProfile? y = null, DateTime ? timeNow = null, DateTime ? timeBefore = null)
    {
        Username = x.Username;
        Password = x.Password;
        UserId = x.UserId;
        Wins = x.Wins;
        SelectedDice = x.DiceSkins.Select(y => y.DiceSkinId).ToList();
        SelectedIngs = x.IngredientSkins.Select(y => y.IngredientSkinId).ToList();
        SelectedTitles = x.Titles.Select(y => y.TitleName).ToList();
        HasNewMessage = x.MessageFroms.Any(y => !y.IsRead);
        HasNewChest = timeNow == null ? false : x.Chests.Count(y => !y.IsOpened) == 0 ? false : x.Chests.Where(y => !y.IsOpened).All(y => y.FinishUnlock == null) ? true : x.Chests.Any(y => y.FinishUnlock != null && y.FinishUnlock < timeNow && !y.IsOpened);
        WineMenu = x.WineMenu;
        UseD8s = x.UseD8s;
        DisableDoubles = x.DisableDoubles;
        PlayAsPurple = x.PlayAsPurple;
        Stars = x.Stars;
        Calories = x.Calories;
        Cooked = x.Cooked;
        Xp = x.Xp;
        Level = x.Level;
        LocalWins = x.LocalWins;
        OnlineWins = x.OnlineWins;
        MusicVolume = (float)x.GameVolume;
        TurnVolume = (float)x.TurnVolume;
        VoiceVolume = (float)x.VoiceVolume;
        EffectsVolume = (float)x.EffectsVolume;
        MasterVolume = (float)x.MasterVolume;
        SeasonScore = x.SeasonScore;

        DailyWins = y?.DailyWins ?? 0;
        WeeklyWins = y?.WeeklyWins ?? 0;
        AllWins = y?.AllWins ?? 0;
        AllPVPWins = y?.AllPvpwins ?? 0;
        CreatedDate = y?.CreatedDate;
        LastLogin = y?.LastLogin;
        IsOnline = timeBefore == null ? false : MultiplayerController.Pings.FirstOrDefault(x => x.UserId == UserId)?.PlayerLastPing > timeBefore;
    }
    public int UserId { get; set; }
    public int Wins { get; set; }
    public int LocalWins { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
    public List<int> SelectedDice { get; set; }
    public List<int> SelectedIngs { get; set; }
    public List<string> SelectedTitles { get; set; }
    public int Stars { get; set; }
    public int Cooked { get; set; }
    public int Xp { get; set; }
    public int SeasonScore { get; set; }
    public int Level { get; set; }
    public int Calories { get; set; }
    public int OnlineWins { get; set; }
    public bool WineMenu { get; set; }
    public bool HasNewMessage { get; set; }
    public bool HasNewChest { get; set; }
    public bool UseD8s { get; set; }
    public bool DisableDoubles { get; set; }
    public bool PlayAsPurple { get; set; }
    public float MusicVolume { get; set; }
    public float TurnVolume { get; set; }
    public float VoiceVolume { get; set; }
    public float EffectsVolume { get; set; }
    public float MasterVolume { get; set; }
    public Nullable<int> DailyWins { get; set; }
    public Nullable<int> WeeklyWins { get; set; }
    public Nullable<int> AllWins { get; set; }
    public Nullable<int> AllPVPWins { get; set; }
    public Nullable<System.DateTime> CreatedDate { get; set; }
    public bool IsOnline { get; set; }
    public Nullable<System.DateTime> LastLogin { get; set; }
}