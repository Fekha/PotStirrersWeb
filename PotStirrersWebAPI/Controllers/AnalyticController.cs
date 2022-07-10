using DataModel;
using PotStirrersWebAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace PotStirrersWebAPI.Controllers
{
    public class AnalyticController : ApiController
    {
        private static Queue<int> UsersSearching = new Queue<int>();
        private static List<GameState> ActiveGames = new List<GameState>();
        TimeZoneInfo easternZone = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");

        private int CreateGame(int Player1, int Player2) {
            var timeNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, easternZone);
            using (PotStirreresDBEntities context = new PotStirreresDBEntities())
            {
                var newGame = new GameAnalytic()
                {
                    GameStartTime = timeNow,
                    Player1Id = Player1,
                    Player2Id = Player2,
                };
                context.GameAnalytics.Add(newGame);
                context.SaveChanges();
                return newGame.GameId;
            }
        }

        [HttpGet]
        [Route("api/Analytic/GameStart")]
        public IHttpActionResult GameStart(int Player1, int Player2, bool WineMenu)
        {
            return Json(CreateGame(Player1, Player2));
        }

        [HttpGet]
        [Route("api/Analytic/FindMyGame")]
        public IHttpActionResult FindMyGame(int UserId, int GameId)
        {
            GameState game = ActiveGames.FirstOrDefault(x => (x.Player1.UserId == UserId || x.Player2.UserId == UserId) && x.GameId == GameId);
            if (game != null)
            {
                return Json(new GameStateDTO(game));
            }
            return Json(game);
        }

        [HttpGet]
        [Route("api/Analytic/CheckGameAlive")]
        public IHttpActionResult CheckGameAlive(int UserId, int GameId)
        {
            var timeNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, easternZone);
            var timePast = timeNow.AddMinutes(-1);
            var game = ActiveGames.FirstOrDefault(x => (x.Player1.UserId == UserId || x.Player2.UserId == UserId) && x.GameId == GameId);
            if (game == null)
            {
                return Json(false);
            }
            if (game.Player1.UserId == UserId)
            {
                game.Player1Ping = timeNow;
            }
            if (game.Player2.UserId == UserId)
            {
                game.Player2Ping = timeNow;
            }
            if (game.Player1Ping < timePast || game.Player2Ping < timePast)
            {
                ActiveGames.Remove(game);
                return Json(false);
            }
            return Json(true);
        }
        
        [HttpGet]
        [Route("api/Analytic/UpdateGameRoll")]
        public IHttpActionResult UpdateGameRoll(int GameId, int roll1, int roll2)
        {
            var game = ActiveGames.FirstOrDefault(x => x.GameId == GameId);
            if (game != null)
                game.GameRolls.Enqueue(new GameRoll() { roll1 = roll1, roll2 = roll2 });
            return Json(true);
        }

        [HttpGet]
        [Route("api/Analytic/GetGameRoll")]
        public IHttpActionResult GetGameLastRoll(int GameId)
        {
            GameRoll roll = null;
            var game = ActiveGames.FirstOrDefault(x => x.GameId == GameId);
            if (game != null && game.GameRolls.Count > 0)
                roll = game.GameRolls.Dequeue();
            return Json(roll);
        }

        [HttpGet]
        [Route("api/Analytic/UpdateTurn")]
        public IHttpActionResult UpdateTurn(int GameId, int IngId, bool Higher)
        {
            var game = ActiveGames.FirstOrDefault(x => x.GameId == GameId);
            if(game != null)
                game.GameTurns.Enqueue(new GameTurn() { IngId = IngId, Higher = Higher });
            return Json(true);
        }

        [HttpGet]
        [Route("api/Analytic/GetTurn")]
        public IHttpActionResult GetTurn(int GameId)
        {
            GameTurn turn = null;
            var game = ActiveGames.FirstOrDefault(x => x.GameId == GameId);
            if (game != null && game.GameTurns.Count > 0)
                turn = game.GameTurns.Dequeue();
            return Json(turn);
        }   
        
        [HttpGet]
        [Route("api/Analytic/UpdateShouldTrash")]
        public IHttpActionResult UpdateShouldTrash(int GameId, bool Trash)
        {
            var game = ActiveGames.FirstOrDefault(x => x.GameId == GameId);
            if(game != null)
                game.ShouldTrash = Trash;
            return Json(true);
        }

        [HttpGet]
        [Route("api/Analytic/GetShouldTrash")]
        public IHttpActionResult GetShouldTrash(int GameId)
        {
            bool? answer = null;
            var game = ActiveGames.FirstOrDefault(x => x.GameId == GameId);
            if (game != null)
            {
                if (game.ShouldTrash != null)
                {
                    answer = game.ShouldTrash;
                    game.ShouldTrash = null;
                }
                return Json(answer);
            }
            return Json(game);
        } 
        
        [HttpGet]
        [Route("api/Analytic/EndTurn")]
        public IHttpActionResult EndTurn(int GameId)
        {
            GameState game = ActiveGames.FirstOrDefault(x => x.GameId == GameId);
            if (game != null)
            {
                game.IsPlayer1Turn = !game.IsPlayer1Turn;
                return Json(game.IsPlayer1Turn);
            }
            return Json(game);
        }
        
        [HttpGet]
        [Route("api/Analytic/EndGame")]
        public IHttpActionResult EndGame(int GameId)
        {
            var game = ActiveGames.FirstOrDefault(x => x.GameId == GameId);
            if (game != null)
                ActiveGames.Remove(game);
            return Json(true);
        }

        [HttpGet]
        [Route("api/Analytic/LookforGame")]
        public IHttpActionResult LookforGame(int UserId)
        {
            var timeNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, easternZone);
            var timeForward = timeNow.AddMinutes(1);

            using (PotStirreresDBEntities context = new PotStirreresDBEntities())
            {
                var game = ActiveGames.FirstOrDefault(x => x.Player1.UserId == UserId || x.Player2.UserId == UserId);
                if (game != null)
                {
                    if (game.StartedPlaying == false)
                    {
                        game.StartedPlaying = true;
                        return Json(game.GameId);
                    }
                    else
                    {
                        ActiveGames.Remove(game);
                    }
                }
                else
                {
                    if (UsersSearching.Count > 0 && !UsersSearching.Contains(UserId))
                    {
                        var queuedId = UsersSearching.Dequeue();
                        var playerX = context.Players.FirstOrDefault(x => x.UserId == queuedId);
                        var playerY = context.Players.FirstOrDefault(x => x.UserId == UserId);
                        PlayerDTO p1;
                        PlayerDTO p2;
                        if (playerX.Wins > playerY.Wins)
                        {
                            p1 = new PlayerDTO(playerX);
                            p2 = new PlayerDTO(playerY);
                        }
                        else
                        {
                            p1 = new PlayerDTO(playerY);
                            p2 = new PlayerDTO(playerX);
                        }
                        var newGameId = CreateGame(p1.UserId, p2.UserId);
                        Random rand = new Random();
                        var turn = rand.Next(0, 2) == 0;
                        ActiveGames.Add(new GameState()
                        {
                            GameId = newGameId,
                            Player1 = p1,
                            Player2 = p2,
                            CreatedDate = timeNow,
                            StartedPlaying = false,
                            ShouldTrash = null,
                            Player1Ping = timeNow,
                            Player2Ping = timeNow,
                            IsPlayer1Turn = turn,
                            GameTurns = new Queue<GameTurn>(),
                            GameRolls = new Queue<GameRoll>()
                        });
                        return Json(newGameId);
                    }
                    else if(!UsersSearching.Contains(UserId))
                    {
                        UsersSearching.Enqueue(UserId);
                    }
                }
                return Json(0);
            }
        } 
        
        [HttpGet]
        [Route("api/Analytic/StopLookingforGame")]
        public IHttpActionResult StopLookingforGame(int UserId)
        {
            if (UsersSearching.Contains(UserId))
                UsersSearching = new Queue<int>(UsersSearching.Where(x => x != UserId));
            return Json(true);
        }

        [HttpGet]
        [Route("api/Analytic/GameEnd")]
        public IHttpActionResult GameEnd(int GameId, int Player1Cooked, int Player2Cooked, int TotalTurns, bool HardMode = false, bool RageQuit = false)
        {
            var timeNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, easternZone);
            var game = ActiveGames.FirstOrDefault(x => x.GameId == GameId);
            if(game != null)
                ActiveGames.Remove(game);
            using (PotStirreresDBEntities context = new PotStirreresDBEntities())
            {
                var BonusXP = HardMode ? 100 : 0;
                var BonusStars = HardMode ? 50 : 0;
                var oldGame = context.GameAnalytics.FirstOrDefault(x => x.GameId == GameId);
                oldGame.GameEndTime = timeNow;
                oldGame.Player1CookedNum = Player1Cooked;
                oldGame.Player2CookedNum = Player2Cooked;
                oldGame.TotalTurns = TotalTurns;
                context.SaveChanges();
               
                var player1 = context.Players.FirstOrDefault(x => x.UserId == oldGame.Player.UserId);
                var player2 = context.Players.FirstOrDefault(x => x.UserId == oldGame.Player1.UserId);
                var Player1IsCPU = player1.Password == "CPU";
                var Player2IsCPU = player2.Password == "CPU";
                if (TotalTurns > 20)
                {
                    if (Player1Cooked > Player2Cooked)
                    {
                        player1.Xp += BonusXP + 300 + ((3 - Player2Cooked) * 50);
                        player2.Xp += BonusXP + 150 + (Player2Cooked * 50) * (RageQuit ? 0 : 1);
                    }
                    else if (Player1Cooked < Player2Cooked)
                    {
                        player2.Xp += BonusXP + 300 + ((3 - Player1Cooked) * 50);
                        player1.Xp += BonusXP + 150 + (Player1Cooked * 50) * (RageQuit ? 0 : 1);
                    }

                    player1.Stars += BonusStars + (Player1Cooked * 50);
                    player2.Stars += BonusStars + (Player2Cooked * 50);
                    player1.Cooked += Player1Cooked;
                    player2.Cooked += Player2Cooked;
                }

                context.SaveChanges();

                return Json(true);
            }
        }
    }
}
