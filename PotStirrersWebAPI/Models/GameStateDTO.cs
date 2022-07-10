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
        public bool? ShouldTrash { get; set; }
        public PlayerDTO Player1 { get; set; }
        public PlayerDTO Player2 { get; set; }
        public DateTime Player1Ping { get; set; }
        public DateTime Player2Ping { get; set; }
        public bool IsPlayer1Turn { get; set; }
        public int? roll1 { get; set; }
        public int? roll2 { get; set; }
        public Queue<GameTurn> GameTurns { get; set; }
        public Queue<GameRoll> GameRolls { get; set; }
    } 
    public class GameStateDTO
    {
        public GameStateDTO(GameState g)
        {
            Player1 = g.Player1;
            Player2 = g.Player2;
            IsPlayer1Turn = g.IsPlayer1Turn;
            Player1Ping = g.Player1Ping;
            Player2Ping = g.Player2Ping;
        }
        public PlayerDTO Player1 { get; set; }
        public PlayerDTO Player2 { get; set; }
        public bool IsPlayer1Turn { get; set; }
        public DateTime Player1Ping { get; set; }
        public DateTime Player2Ping { get; set; }
    }

    public class GameTurn
    {
        public int IngId { get; set; }
        public bool Higher { get; set; }
    }
    
    public class GameRoll
    {
        public int roll1 { get; set; }
        public int roll2 { get; set; }
    }
}