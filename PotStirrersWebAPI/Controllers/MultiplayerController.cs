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
    //hello
    public class MultiplayerController : ApiController
    {
        private static Queue<int> UsersSearching = new Queue<int>();
        private static List<GameState> ActiveGames = new List<GameState>();
        private static List<PlayerPing> Pings = new List<PlayerPing>();

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
        [Route("api/Multiplayer/GameStart")]
        public IHttpActionResult GameStart(int Player1, int Player2, bool FakeOnlineGame = false)
        {
            if (FakeOnlineGame)
            {
                using (PotStirreresDBEntities context = new PotStirreresDBEntities())
                {
                    var playerX = context.Players.FirstOrDefault(x => x.UserId == Player1);
                    playerX.Stars -= 150;
                    context.SaveChanges();
                }
            }
            return Json(CreateGame(Player1, Player2));
        }

        [HttpGet]
        [Route("api/Multiplayer/FindMyGame")]
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
        [Route("api/Multiplayer/CheckGameAlive")]
        public IHttpActionResult CheckGameAlive(int UserId, int GameId, int OtherUserId)
        {
            var timeNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, easternZone);
            var timePast = timeNow.AddMinutes(-1);
            UpdatePing(UserId);
            var game = ActiveGames.FirstOrDefault(x => (x.Player1.UserId == UserId || x.Player2.UserId == UserId) && x.GameId == GameId);
            if (game == null)
            {
                return Json(OtherUserId);
            }
            if (Pings.FirstOrDefault(x => x.UserId == game.Player1.UserId).PlayerLastPing < timePast)
            {
                ActiveGames.Remove(game);
                return Json(game.Player1.UserId);
            }
            else if (Pings.FirstOrDefault(x => x.UserId == game.Player2.UserId).PlayerLastPing < timePast)
            {
                ActiveGames.Remove(game);
                return Json(game.Player2.UserId);
            }
            return Json(0);
        }

        //private void UpdatePing(int UserId, int GameId)
        //{
        //    var timeNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, easternZone);
        //    var game = ActiveGames.FirstOrDefault(x => (x.Player1.UserId == UserId || x.Player2.UserId == UserId) && x.GameId == GameId);
        //    if (game != null)
        //    {
        //        if (game.Player1.UserId == UserId)
        //        {
        //            game.Player1Ping = timeNow;
        //        }
        //        if (game.Player2.UserId == UserId)
        //        {
        //            game.Player2Ping = timeNow;
        //        }
        //    }
        //}

        [HttpGet]
        [Route("api/Multiplayer/UpdateGameRoll")]
        public IHttpActionResult UpdateGameRoll(int UserId, int GameId, int roll1, int roll2)
        {
            UpdatePing(UserId);
            var game = ActiveGames.FirstOrDefault(x => x.GameId == GameId);
            if (game != null)
                game.GameRolls.Enqueue(new GameRoll() {UserId = UserId, roll1 = roll1, roll2 = roll2 });
            return Json(true);
        }

        [HttpGet]
        [Route("api/Multiplayer/GetGameRoll")]
        public IHttpActionResult GetGameLastRoll(int UserId, int GameId)
        {
            UpdatePing(UserId);
            GameRoll roll = null;
            var game = ActiveGames.FirstOrDefault(x => x.GameId == GameId);
            if (game != null && game.GameRolls.Count > 0 && game.GameRolls.Peek().UserId != UserId)
                roll = game.GameRolls.Dequeue();
            return Json(roll);
        }

        [HttpGet]
        [Route("api/Multiplayer/UpdateTurn")]
        public IHttpActionResult UpdateTurn(int UserId, int GameId, int IngId, bool Higher)
        {
            UpdatePing(UserId);
            var game = ActiveGames.FirstOrDefault(x => x.GameId == GameId);
            if(game != null)
                game.GameTurns.Enqueue(new GameTurn() {UserId = UserId, IngId = IngId, Higher = Higher });
            return Json(true);
        }

        [HttpGet]
        [Route("api/Multiplayer/GetTurn")]
        public IHttpActionResult GetTurn(int UserId, int GameId)
        {
            UpdatePing(UserId);
            GameTurn turn = null;
            var game = ActiveGames.FirstOrDefault(x => x.GameId == GameId);
            if (game != null && game.GameTurns.Count > 0 && game.GameTurns.Peek().UserId != UserId)
                turn = game.GameTurns.Dequeue();
            return Json(turn);
        }   
        
        [HttpGet]
        [Route("api/Multiplayer/UpdateSelected")]
        public IHttpActionResult UpdateSelected(int UserId, int GameId, bool Higher)
        {
            UpdatePing(UserId);
            var game = ActiveGames.FirstOrDefault(x => x.GameId == GameId);
            if(game != null)
                game.GameSelections.Enqueue(new GameSelections() {UserId = UserId, Selection = Higher });
            return Json(true);
        }

        [HttpGet]
        [Route("api/Multiplayer/GetSelected")]
        public IHttpActionResult GetSelected(int UserId, int GameId)
        {
            UpdatePing(UserId);
            bool? sel = null;
            var game = ActiveGames.FirstOrDefault(x => x.GameId == GameId);
            if (game != null && game.GameSelections.Count > 0 && game.GameSelections.Peek().UserId != UserId)
                sel = game.GameSelections.Dequeue().Selection;
            return Json(sel);
        }   
        
        [HttpGet]
        [Route("api/Multiplayer/UpdateShouldTrash")]
        public IHttpActionResult UpdateShouldTrash(int GameId, bool Trash)
        {
            var game = ActiveGames.FirstOrDefault(x => x.GameId == GameId);
            if(game != null)
                game.ShouldTrash = Trash;
            return Json(true);
        }

        [HttpGet]
        [Route("api/Multiplayer/GetShouldTrash")]
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
        [Route("api/Multiplayer/EndTurn")]
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
        [Route("api/Multiplayer/EndGame")]
        public IHttpActionResult EndGame(int GameId)
        {
            var game = ActiveGames.FirstOrDefault(x => x.GameId == GameId);
            if (game != null)
                ActiveGames.Remove(game);
            return Json(true);
        }

        [HttpGet]
        [Route("api/Multiplayer/LookforGame")]
        public IHttpActionResult LookforGame(int UserId)
        {
            var timeNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, easternZone);
            var timeForward = timeNow.AddMinutes(1);
            var timeBackward = timeNow.AddSeconds(-5);
            UpdatePing(UserId);
            //prune
            if (Pings.Any(x => x.PlayerLastPing < timeBackward))
            {
                var prunableQue = Pings.Where(x => x.PlayerLastPing < timeBackward).Select(x => x.UserId).ToList();
                UsersSearching = new Queue<int>(UsersSearching.Where(x => !prunableQue.Contains(x)));
            }
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
                        playerX.Stars -= 150;
                        playerY.Stars -= 150;
                        context.SaveChanges();
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
                            IsPlayer1Turn = turn,
                            GameTurns = new Queue<GameTurn>(),
                            GameSelections = new Queue<GameSelections>(),
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

        private void UpdatePing(int UserId)
        {
            var timeNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, easternZone);
            var ping = Pings.FirstOrDefault(x => x.UserId == UserId);
            if (ping != null)
            {
                ping.PlayerLastPing = timeNow;
            }
            else
            {
                Pings.Add(new PlayerPing(UserId, timeNow));
            }
        }

        [HttpGet]
        [Route("api/Multiplayer/StopLookingforGame")]
        public IHttpActionResult StopLookingforGame(int UserId)
        {
            if (UsersSearching.Contains(UserId))
                UsersSearching = new Queue<int>(UsersSearching.Where(x => x != UserId));
            return Json(true);
        }

        [HttpGet]
        [Route("api/Multiplayer/CPUGameWon")]
        public IHttpActionResult CPUGameWon(int UserId)
        {
            using (PotStirreresDBEntities context = new PotStirreresDBEntities())
            {
                var player1 = context.Players.FirstOrDefault(x => x.UserId == UserId);
                player1.Stars += 50;
                return Json(true);
            }
        }

        [HttpGet]
        [Route("api/Multiplayer/GameEnd")]
        public IHttpActionResult GameEnd(int GameId, int Player1Cooked, int Player2Cooked, int TotalTurns, int RageQuit = 0)
        {
            var timeNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, easternZone);
            var game = ActiveGames.FirstOrDefault(x => x.GameId == GameId);
            if(game != null)
                ActiveGames.Remove(game);
            using (PotStirreresDBEntities context = new PotStirreresDBEntities())
            {
                var rewardsGiven = false;
                var oldGame = context.GameAnalytics.FirstOrDefault(x => x.GameId == GameId);
                if (oldGame.GameEndTime == null)
                {
                    oldGame.GameEndTime = timeNow;
                    oldGame.Player1CookedNum = Player1Cooked;
                    oldGame.Player2CookedNum = Player2Cooked;
                    oldGame.TotalTurns = TotalTurns;
                    context.SaveChanges();
                }
                else
                {
                    rewardsGiven = true;
                }
                bool player1Won = false;
               
                var player1 = context.Players.FirstOrDefault(x => x.UserId == oldGame.Player.UserId);
                var player2 = context.Players.FirstOrDefault(x => x.UserId == oldGame.Player1.UserId);

                if (RageQuit != 0)
                {
                    if (player1.UserId == RageQuit)
                    {
                        player1Won = false;
                    }
                    else if (player2.UserId == RageQuit)
                    {
                        player1Won = true;
                    }
                }
                else
                {
                    if (Player1Cooked == 4)
                    {
                        player1Won = true;
                    }
                    else
                    {
                        player1Won = false;
                    }
                }
                var winner = (player1Won ? player1 : player2);
                var loser = (player1Won ? player2 : player1);
                if (!rewardsGiven)
                {
                    winner.OnlineWins++;
                    if (TotalTurns >= 20)
                    {
                        if (winner.Chests.Where(x => !x.IsOpened).Count() < 4)
                        {
                            var chestEarned = getChest();
                            context.Chests.Add(new Chest()
                            {
                                ChestSize = chestEarned,
                                UserId = winner.UserId
                            });
                        }
                        if (player1Won)
                        {
                            player1.Stars += 300;
                        }
                        else
                        {
                            player2.Stars += 300;
                        }
                    }
                    else
                    {
                        if (player1Won)
                        {
                            player1.Stars += 150;
                        }
                        else
                        {
                            player2.Stars += 150;
                        }
                    }
                    player1.Xp += Player1Cooked * 50;
                    player2.Xp += Player2Cooked * 50;

                    player1.Cooked += Player1Cooked;
                    player2.Cooked += Player2Cooked;
                    context.SaveChanges();
                }
                var gameLastedText = $"{(TotalTurns < 20?"!" : " and a Dice Pack!")} \n \n The match lasted {TotalTurns} turns! {(TotalTurns < 20 ? "Since the round didn't last 20 turns no rewards were earned" : "")}";
                var message = "";
                if (RageQuit == 0) {
                    message += $" {winner.Username} Won! \n \n They";
                } else {
                    message += $" {loser.Username} Quit! \n \n {winner.Username}";
                }
                message += $" earned 300 Calories{gameLastedText} \n \n Each Player earned 50 XP per cooked ingredient";
                return Json(message);
            }
        }

        private int getChest()
        {
            Random random = new Random();
            var rarity = random.Next(0, 10);
            if (rarity == 9)
            {
                return 3;
            }
            else if (rarity > 5)
            {
                return 2;
            }
            else
            {
                return 1;
            }
        }
    }
}
