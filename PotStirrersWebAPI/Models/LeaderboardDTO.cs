using DataModel;
using System;
using System.Linq;

public class LeaderboardDTO
{
    public int Wins { get; set; }
    public int WinsToday { get; set; }
    public int WinsThisWeek { get; set; }
    public int LocalWins { get; set; }
    public string Username { get; set; }
}