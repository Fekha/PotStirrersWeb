using DataModel;
using System;
using System.Linq;

public class PlayerDTO
{
    public PlayerDTO(Player x)
    {
        Username = x.Username;
        Password = x.Password;
        UserId = x.UserId;
        Wins = x.Wins;
        SelectedMeat = x.SelectedMeat;
        SelectedVeggie = x.SelectedVeggie;
        SelectedFruit = x.SelectedFruit;
        SelectedFourth = x.SelectedFourthIngredient;
        SelectedDie = x.SelectedDie;
        SelectedDie2 = x.SelectedDie2;
        WineMenu = x.WineMenu;
        UseD8s = x.UseD8s;
        DisableDoubles = x.DisableDoubles;
        PlayAsPurple = x.PlayAsPurple;
        Stars = x.Stars;
        Cooked = x.Cooked;
        Xp = x.Xp;
        Level = x.Level;
        LocalWins = x.LocalWins;
    }
    public int UserId { get; set; }
    public int Wins { get; set; }
    public int LocalWins { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
    public int SelectedMeat { get; set; }
    public int SelectedVeggie { get; set; }
    public int SelectedFruit { get; set; }
    public int SelectedFourth { get; set; }
    public int SelectedDie { get; set; }
    public int SelectedDie2 { get; set; }
    public int Stars { get; set; }
    public int Cooked { get; set; }
    public int Xp { get; set; }
    public int Level { get; set; }
    public bool WineMenu { get; set; }
    public bool UseD8s { get; set; }
    public bool DisableDoubles { get; set; }
    public bool PlayAsPurple { get; set; }
}