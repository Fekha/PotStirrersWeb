using PotStirrersAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PotStirrersWebAPI.Models
{
    public class GameState
    {
        public int GameId { get; set; }
        public DateTime CreatedDate { get; set; }
        public bool StartedPlaying { get; set; }
        public bool IsFriendGame { get; set; }
        public bool? ShouldTrash { get; set; }
        public int Player1Id { get; set; }
        public int Player2Id { get; set; }
        public bool IsPlayer1Turn { get; set; }
        public Queue<GameTurn> GameTurns { get; set; }
        public Queue<GameSelections> GameSelections { get; set; }
        public Queue<GameRoll> GameRolls { get; set; }
    }
    public class GameStateDTO
    {
        public PlayerDTO Player1 { get; set; }
        public PlayerDTO Player2 { get; set; }
        public bool IsPlayer1Turn { get; set; }
    }
    public class PlayerPing
    {
        public PlayerPing(int UserId, DateTime now)
        {
            this.UserId = UserId;
            PlayerLastPing = now;
        }
        public int UserId { get; set; }
        public DateTime PlayerLastPing { get; set; }
    }
    public class GameTurn
    {
        public int UserId { get; set; }
        public int IngId { get; set; }
        public bool Higher { get; set; }
    }

    public class GameRoll
    {
        public int UserId { get; set; }
        public int roll1 { get; set; }
        public int roll2 { get; set; }
    }

    public class GameSelections
    {
        public int UserId { get; set; }
        public bool Selection { get; set; }
    }
}