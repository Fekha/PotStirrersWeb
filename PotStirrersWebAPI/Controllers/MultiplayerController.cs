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
    public class MultiplayerController : ApiController
    {
        private static Queue<MatchmakingUser> UsersSearching = new Queue<MatchmakingUser>();
        private static List<GameState> ActiveGames = new List<GameState>();
        public static List<PlayerPing> Pings = new List<PlayerPing>();
        public static Random random = new Random();

        private static TimeZoneInfo easternZone = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");

        private int CreateGame(int Player1, int Player2, int Wager, bool friendGame = false, bool isCPUGame = false) {
            var timeNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, easternZone);
            using (PotStirreresDBEntities context = new PotStirreresDBEntities())
            {
                var newGame = new GameAnalytic()
                {
                    GameStartTime = timeNow,
                    Player1Id = Player1,
                    Player2Id = Player2,
                    Wager = Wager,
                    IsFriendGame = friendGame,
                    IsCPUGame = isCPUGame
                };
                context.GameAnalytics.Add(newGame);
                context.SaveChanges();
                return newGame.GameId;
            }
        }

        [HttpGet]
        [Route("api/Multiplayer/CPUGameStart")]
        public IHttpActionResult CPUGameStart(int Player1, int Player2, bool FakeOnlineGame = false)
        {
            var timeNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, easternZone);
            using (PotStirreresDBEntities context = new PotStirreresDBEntities())
            {
                var playerX = context.Players.FirstOrDefault(x => x.UserId == Player1);
                var wager = (Player2 == 42 ? 100 : 0);
                playerX.Calories -= wager;
                context.SaveChanges();
                var newGameId = CreateGame(Player1, Player2, wager, false, true);
                return Json(newGameId);
            }
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
            if (OtherUserId != 41 && OtherUserId != 42)
            {
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
        [Route("api/Multiplayer/LookforGame")]
        public IHttpActionResult LookforGame(int UserId, int wager)
        {
            var timeNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, easternZone);
            var timeForward = timeNow.AddMinutes(1);
            var timeBackward = timeNow.AddSeconds(-5);
            UpdatePing(UserId);
            //prune
            if (Pings.Any(x => x.PlayerLastPing < timeBackward))
            {
                var prunableQue = Pings.Where(x => x.PlayerLastPing < timeBackward).Select(x => x.UserId).ToList();
                UsersSearching = new Queue<MatchmakingUser>(UsersSearching.Where(x => !prunableQue.Contains(x.UserId)));
            }
            using (PotStirreresDBEntities context = new PotStirreresDBEntities())
            {
                var game = ActiveGames.FirstOrDefault(x => x.Player1.UserId == UserId || x.Player2.UserId == UserId);
                if (game != null)
                {
                    if (game.StartedPlaying == false && !game.IsFriendGame)
                    {
                        game.StartedPlaying = true;
                        return Json(game.GameId);
                    }
                }
                else
                {
                    if (UsersSearching.Count > 0 && !UsersSearching.Select(x=>x.UserId).Contains(UserId))
                    {
                        var queuedUser = UsersSearching.Dequeue();
                        var playerX = context.Players.FirstOrDefault(x => x.UserId == queuedUser.UserId);
                        var playerY = context.Players.FirstOrDefault(x => x.UserId == UserId);
                        var gameCost = Math.Min(wager,queuedUser.Wager);
                        playerX.Calories -= gameCost;
                        playerY.Calories -= gameCost;
                        context.SaveChanges();
                        PlayerDTO p1 = new PlayerDTO(playerX);
                        PlayerDTO p2 = new PlayerDTO(playerY);
                        var newGameId = CreateGame(p1.UserId, p2.UserId, gameCost);
                        Random rand = new Random();
                        var turn = rand.Next(0, 2) == 0;
                        ActiveGames.Add(new GameState()
                        {
                            GameId = newGameId,
                            Player1 = p1,
                            Player2 = p2,
                            CreatedDate = timeNow,
                            IsFriendGame = false,
                            StartedPlaying = false,
                            ShouldTrash = null,
                            IsPlayer1Turn = turn,
                            GameTurns = new Queue<GameTurn>(),
                            GameSelections = new Queue<GameSelections>(),
                            GameRolls = new Queue<GameRoll>()
                        });
                        return Json(newGameId);
                    }
                    else if(!UsersSearching.Select(x => x.UserId).Contains(UserId))
                    {
                        UsersSearching.Enqueue(new MatchmakingUser() { UserId = UserId, Wager = wager });
                    }
                }
                return Json(0);
            }
        }
        [HttpGet]
        [Route("api/Multiplayer/FriendGameStarted")]
        public IHttpActionResult FriendGameStarted(int UserId)
        {
            UpdatePing(UserId);
            var game = ActiveGames.FirstOrDefault(x => (x.Player1.UserId == UserId || x.Player2.UserId == UserId) && x.IsFriendGame);
            if (game != null)
            {
                if(game.StartedPlaying)
                    return Json(game.GameId);
                else
                    return Json(0);
            }
            return Json(game);
        }
        
        [HttpGet]
        [Route("api/Multiplayer/CheckForFriendGameInvite")]
        public IHttpActionResult CheckForFriendGameInvite(int UserId)
        {
            UpdatePing(UserId);
            var game = ActiveGames.FirstOrDefault(x => (x.Player1.UserId == UserId || x.Player2.UserId == UserId) && x.IsFriendGame && !x.StartedPlaying);
            if (game != null)
            {
                return Json(game.GameId);
            }
            return Json(0);
        }
        
        [HttpGet]
        [Route("api/Multiplayer/StartFriendGame")]
        public IHttpActionResult StartFriendGame(int UserId)
        {
            UpdatePing(UserId);
            var game = ActiveGames.FirstOrDefault(x => x.Player1.UserId == UserId || x.Player2.UserId == UserId);
            if (game != null)
            {
                if (game.StartedPlaying == false)
                {
                    game.StartedPlaying = true;
                    return Json(game.GameId);
                }
            }
            return Json(0);
        }
        [HttpGet]
        [Route("api/Multiplayer/FriendGameInvite")]
        public IHttpActionResult FriendGameInvite(int UserId, int OtherUserId)
        {
            var timeNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, easternZone);
            var timeBackward = timeNow.AddSeconds(-10);
            UpdatePing(UserId);
            var otherUser = Pings.FirstOrDefault(x => x.UserId == OtherUserId && x.PlayerLastPing > timeBackward);
            if (otherUser != null)
            {
                if (!ActiveGames.Any(x => x.Player1.UserId == OtherUserId || x.Player2.UserId == OtherUserId))
                {
                    using (PotStirreresDBEntities context = new PotStirreresDBEntities())
                    {
                        var playerX = context.Players.FirstOrDefault(x => x.UserId == OtherUserId);
                        var playerY = context.Players.FirstOrDefault(x => x.UserId == UserId);
                        PlayerDTO p1 = new PlayerDTO(playerX);
                        PlayerDTO p2 = new PlayerDTO(playerY);
                        var newGameId = CreateGame(p1.UserId, p2.UserId, 0, true);
                        Random rand = new Random();
                        var turn = rand.Next(0, 2) == 0;
                        ActiveGames.Add(new GameState()
                        {
                            GameId = newGameId,
                            Player1 = p1,
                            Player2 = p2,
                            CreatedDate = timeNow,
                            StartedPlaying = false,
                            IsFriendGame = true,
                            ShouldTrash = null,
                            IsPlayer1Turn = turn,
                            GameTurns = new Queue<GameTurn>(),
                            GameSelections = new Queue<GameSelections>(),
                            GameRolls = new Queue<GameRoll>()
                        });
                        return Json(newGameId);
                    }
                }
                else
                {
                    return Json(0);
                }
            }
            else
            {
                return Json(0);
            }
        }

        public static void UpdatePing(int UserId)
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
            if (UsersSearching.Select(x=>x.UserId).Contains(UserId))
                UsersSearching = new Queue<MatchmakingUser>(UsersSearching.Where(x => x.UserId != UserId));
            return Json(true);
        }
        
        [HttpGet]
        [Route("api/Multiplayer/DeclineFriendGame")]
        public IHttpActionResult DeclineFriendGame(int GameId)
        {
            var game = ActiveGames.FirstOrDefault(x => x.GameId == GameId);
            if (game != null)
                ActiveGames.Remove(game);
            return Json(true);
        }

        [HttpGet]
        [Route("api/Multiplayer/GameEnd")]
        public IHttpActionResult GameEnd(int GameId, int Player1Cooked, int Player2Cooked, int TotalTurns, int RageQuit = 0)
        {
            var timeNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, easternZone);
            var message = "";
            var game = ActiveGames.FirstOrDefault(x => x.GameId == GameId);
            if (game != null) {
                ActiveGames.Remove(game);
            }
            using (PotStirreresDBEntities context = new PotStirreresDBEntities())
            {
                var rewardsGiven = false;
                var oldGame = context.GameAnalytics.FirstOrDefault(x => x.GameId == GameId);
                if (oldGame.GameEndTime == null)
                {
                    if (RageQuit != 0)
                    {
                        oldGame.Quit = true;
                    }
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
                    if(!oldGame.IsCPUGame)
                        winner.OnlineWins++;
                    else
                        winner.Wins++;

                    if (!oldGame.IsFriendGame && (!oldGame.IsCPUGame || oldGame.Player2Id == 42))
                    {
                        if (TotalTurns >= 20)
                        {
                            if (winner.Chests.Where(x => !x.IsOpened).Count() < 4)
                            {
                                var chestEarned = getChest();
                                context.Chests.Add(new Chest()
                                {
                                    ChestSize = chestEarned,
                                    UserId = winner.UserId,
                                    ChestTypeId = random.Next(1, 3)
                                });
                            }
                            if (player1Won)
                            {
                                player1.Calories += oldGame.Wager * 2;
                                player1.SeasonScore += oldGame.Wager * 2;
                            }
                            else
                            {
                                player2.Calories += oldGame.Wager * 2;
                                player2.SeasonScore += oldGame.Wager * 2;
                            }

                            if (player1.UserId == 5 && !player1Won && !player1.Titles1.Any(x => x.TitleId == 11))
                            {
                                player2.Titles1.Add(context.Titles.FirstOrDefault(x => x.TitleId == 11));
                            }
                            else if (player2.UserId == 5 && player1Won && !player2.Titles1.Any(x => x.TitleId == 11))
                            {
                                player1.Titles1.Add(context.Titles.FirstOrDefault(x => x.TitleId == 11));
                            }
                        }
                        else
                        {
                            if (player1Won)
                            {
                                player1.Calories += oldGame.Wager;
                            }
                            else
                            {
                                player2.Calories += oldGame.Wager;
                            }
                        }
                    }
                    else if (oldGame.IsCPUGame)
                    {
                        if (player1Won)
                        {
                            player1.Calories += 50;
                        }
                    }

                    player1.Xp += Player1Cooked * 50;
                    player2.Xp += Player2Cooked * 50;

                    player1.Cooked += Player1Cooked;
                    player2.Cooked += Player2Cooked;

                    context.SaveChanges();
                }
                
                if (RageQuit == 0) {
                    message += $" {winner.Username} Won! \n \n";
                } else {
                    message += $" {loser.Username} Quit! \n \n";
                }

                message += $"The match lasted {TotalTurns} turns! \n \n";
                if (oldGame.IsFriendGame)
                {
                    message += $"This was a friend match so no extra rewards or wagers were earned.";
                }
                else if (oldGame.IsCPUGame)
                {
                    if (player1Won)
                    {
                        if (player2.UserId == 41) 
                        {
                            message += $"Great work, its no easy task beating Jenn! You earned 50 Calories.";
                        }
                        else 
                        {
                            message += $"Great work, can't believe you beat Ethan! You won 100 Calories!";
                        }
                    }
                    else
                    {
                        if (player2.UserId == 41)
                        {
                            message += "Keep practicing, there's more skill to the game than you might think!";
                        }
                        else
                        {
                            message += "Keep practicing, until then Ethan took 100 Calories from you.";
                        }
                    }
                }
                else
                {
                    if (TotalTurns < 20)
                    {
                        message += $"Entry fees of {oldGame.Wager} Calories were returned and no rewards were earned since the round didn't last 20 turns.";
                    }
                    else
                    {
                        message += $"{winner.Username} earned {oldGame.Wager * 2} Calories and a Skin Pack!";
                    }
                }
                message += $"\n \n Each Player gained 50 XP per cooked ingredient!";
                return Json(message);
            }
        }

        public static int getChest()
        {
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

        public class MatchmakingUser
        {
            public int UserId { get; set; }
            public int Wager { get; set; }
        }
    }
}
