﻿using DataModel;
using System;
using System.Collections.Generic;
using System.Linq;

public class PlayerDTO
{
    public PlayerDTO(Player x)
    {
        Username = x.Username;
        Password = x.Password;
        UserId = x.UserId;
        Wins = x.Wins;
        SelectedDice = x.DiceSkins.Select(y=> y.DiceSkinId).ToList();
        SelectedIngs = x.IngredientSkins.Select(y=> y.IngredientSkinId).ToList();
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
    }
    public int UserId { get; set; }
    public int Wins { get; set; }
    public int LocalWins { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
    public List<int> SelectedDice { get; set; }
    public List<int> SelectedIngs { get; set; }
    public int Stars { get; set; }
    public int Cooked { get; set; }
    public int Xp { get; set; }
    public int Level { get; set; }
    public int Calories { get; set; }
    public int OnlineWins { get; set; }
    public bool WineMenu { get; set; }
    public bool UseD8s { get; set; }
    public bool DisableDoubles { get; set; }
    public bool PlayAsPurple { get; set; }
}