using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PotStirrersAPI.Models;
using PotStirrersWebAPI.Models;

namespace PotStirrersWebAPI.Controllers
{
    public class MultiplayerController : Controller
    {
        private static Queue<MatchmakingUser> UsersSearching = new Queue<MatchmakingUser>();
        private static List<GameState> ActiveGames = new List<GameState>();
        public static List<PlayerPing> Pings = new List<PlayerPing>();
        public static Random random = new Random();

        private static TimeZoneInfo easternZone = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");

        private int CreateGame(int Player1, int Player2, int Wager, bool friendGame = false, bool IsCpuGame = false)
        {
            var timeNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, easternZone);
            using (PotStirreresDBContext context = new PotStirreresDBContext())
            {
                var newGame = new GameAnalytic()
                {
                    GameStartTime = timeNow,
                    Player1Id = Player1,
                    Player2Id = Player2,
                    Wager = Wager,
                    IsFriendGame = friendGame,
                    IsCpuGame = IsCpuGame
                };
                context.GameAnalytics.Add(newGame);
                context.SaveChanges();
                return newGame.GameId;
            }
        }

        [HttpGet]
        [Route("api/Multiplayer/CPUGameStart")]
        public ActionResult CPUGameStart(int Player1, int Player2, bool FakeOnlineGame = false)
        {
            var timeNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, easternZone);
            using (PotStirreresDBContext context = new PotStirreresDBContext())
            {
                var playerX = context.Players.First(x => x.UserId == Player1);
                var wager = (Player2 == 42 ? 100 : 0);
                playerX.Calories -= wager;
                context.SaveChanges();
                var newGameId = CreateGame(Player1, Player2, wager, false, true);
                return Ok(newGameId);
            }
        }

        [HttpGet]
        [Route("api/Multiplayer/FindMyGame")]
        public ActionResult FindMyGame(int UserId, int GameId)
        {
            GameState? game = ActiveGames.FirstOrDefault(x => (x.Player1Id == UserId || x.Player2Id == UserId) && x.GameId == GameId);
            if (game != null)
            {
                using (PotStirreresDBContext context = new PotStirreresDBContext())
                {
                    return Ok(new GameStateDTO()
                    {
                        Player1 = new PlayerDTO(context.Players.Include(x => x.TitlesNavigation).Include(x => x.DiceSkins).Include(x => x.IngredientSkins).First(x => x.UserId == game.Player1Id)),
                        Player2 = new PlayerDTO(context.Players.Include(x=>x.TitlesNavigation).Include(x => x.DiceSkins).Include(x => x.IngredientSkins).First(x => x.UserId == game.Player2Id)),
                        IsPlayer1Turn = game.IsPlayer1Turn
                    });
                }
            }
            return Ok(game);
        }

        [HttpGet]
        [Route("api/Multiplayer/CheckGameAlive")]
        public ActionResult CheckGameAlive(int UserId, int GameId, int OtherUserId)
        {
            var timeNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, easternZone);
            var timePast = timeNow.AddMinutes(-1);
            UpdatePing(UserId);
            if (OtherUserId != 41 && OtherUserId != 42)
            {
                var game = ActiveGames.FirstOrDefault(x => (x.Player1Id == UserId || x.Player2Id == UserId) && x.GameId == GameId);
                if (game == null)
                {
                    return Ok(OtherUserId);
                }
                if (Pings.FirstOrDefault(x => x.UserId == game.Player1Id)?.PlayerLastPing < timePast)
                {
                    ActiveGames.Remove(game);
                    return Ok(game.Player1Id);
                }
                else if (Pings.FirstOrDefault(x => x.UserId == game.Player2Id)?.PlayerLastPing < timePast)
                {
                    ActiveGames.Remove(game);
                    return Ok(game.Player2Id);
                }
            }
            return Ok(0);
        }

        [HttpGet]
        [Route("api/Multiplayer/UpdateGameRoll")]
        public ActionResult UpdateGameRoll(int UserId, int GameId, int roll1, int roll2)
        {
            UpdatePing(UserId);
            var game = ActiveGames.FirstOrDefault(x => x.GameId == GameId);
            if (game != null)
                game.GameRolls.Enqueue(new GameRoll() { UserId = UserId, roll1 = roll1, roll2 = roll2 });
            return Ok(true);
        }

        [HttpGet]
        [Route("api/Multiplayer/GetGameRoll")]
        public ActionResult GetGameLastRoll(int UserId, int GameId)
        {
            UpdatePing(UserId);
            GameRoll? roll = null;
            var game = ActiveGames.FirstOrDefault(x => x.GameId == GameId);
            if (game != null && game.GameRolls.Count > 0 && game.GameRolls.Peek().UserId != UserId)
                roll = game.GameRolls.Dequeue();
            return Ok(roll);
        }

        [HttpGet]
        [Route("api/Multiplayer/UpdateTurn")]
        public ActionResult UpdateTurn(int UserId, int GameId, int IngId, bool Higher)
        {
            UpdatePing(UserId);
            var game = ActiveGames.FirstOrDefault(x => x.GameId == GameId);
            if (game != null)
                game.GameTurns.Enqueue(new GameTurn() { UserId = UserId, IngId = IngId, Higher = Higher });
            return Ok(true);
        }

        [HttpGet]
        [Route("api/Multiplayer/GetTurn")]
        public ActionResult GetTurn(int UserId, int GameId)
        {
            UpdatePing(UserId);
            GameTurn? turn = null;
            var game = ActiveGames.FirstOrDefault(x => x.GameId == GameId);
            if (game != null && game.GameTurns.Count > 0 && game.GameTurns.Peek().UserId != UserId)
                turn = game.GameTurns.Dequeue();
            return Ok(turn);
        }

        [HttpGet]
        [Route("api/Multiplayer/UpdateSelected")]
        public ActionResult UpdateSelected(int UserId, int GameId, bool Higher)
        {
            UpdatePing(UserId);
            var game = ActiveGames.FirstOrDefault(x => x.GameId == GameId);
            if (game != null)
                game.GameSelections.Enqueue(new GameSelections() { UserId = UserId, Selection = Higher });
            return Ok(true);
        }

        [HttpGet]
        [Route("api/Multiplayer/GetSelected")]
        public ActionResult GetSelected(int UserId, int GameId)
        {
            UpdatePing(UserId);
            bool? sel = null;
            var game = ActiveGames.FirstOrDefault(x => x.GameId == GameId);
            if (game != null && game.GameSelections.Count > 0 && game.GameSelections.Peek().UserId != UserId)
                sel = game.GameSelections.Dequeue().Selection;
            return Ok(sel);
        }

        [HttpGet]
        [Route("api/Multiplayer/UpdateShouldTrash")]
        public ActionResult UpdateShouldTrash(int GameId, bool Trash)
        {
            var game = ActiveGames.FirstOrDefault(x => x.GameId == GameId);
            if (game != null)
                game.ShouldTrash = Trash;
            return Ok(true);
        }

        [HttpGet]
        [Route("api/Multiplayer/GetShouldTrash")]
        public ActionResult GetShouldTrash(int GameId)
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
                return Ok(answer);
            }
            return Ok(game);
        }

        [HttpGet]
        [Route("api/Multiplayer/EndTurn")]
        public ActionResult EndTurn(int GameId)
        {
            GameState? game = ActiveGames.FirstOrDefault(x => x.GameId == GameId);
            if (game != null)
            {
                game.IsPlayer1Turn = !game.IsPlayer1Turn;
                return Ok(game.IsPlayer1Turn);
            }
            return Ok(game);
        }

        [HttpGet]
        [Route("api/Multiplayer/LookforGame")]
        public ActionResult LookforGame(int UserId, int wager)
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
            using (PotStirreresDBContext context = new PotStirreresDBContext())
            {
                var game = ActiveGames.FirstOrDefault(x => x.Player1Id == UserId || x.Player2Id == UserId);
                if (game != null)
                {
                    if (game.StartedPlaying == false && !game.IsFriendGame)
                    {
                        game.StartedPlaying = true;
                        return Ok(game.GameId);
                    }
                }
                else
                {
                    if (UsersSearching.Count > 0 && !UsersSearching.Select(x => x.UserId).Contains(UserId))
                    {
                        var queuedUser = UsersSearching.Dequeue();
                        var playerX = context.Players.First(x => x.UserId == queuedUser.UserId);
                        var playerY = context.Players.First(x => x.UserId == UserId);
                        var gameCost = Math.Min(wager, queuedUser.Wager);
                        playerX.Calories -= gameCost;
                        playerY.Calories -= gameCost;
                        context.SaveChanges();
                        var newGameId = CreateGame(playerX.UserId, playerY.UserId, gameCost);
                        Random rand = new Random();
                        var turn = rand.Next(0, 2) == 0;
                        ActiveGames.Add(new GameState()
                        {
                            GameId = newGameId,
                            Player1Id = playerX.UserId,
                            Player2Id = playerY.UserId,
                            CreatedDate = timeNow,
                            IsFriendGame = false,
                            StartedPlaying = false,
                            ShouldTrash = null,
                            IsPlayer1Turn = turn,
                            GameTurns = new Queue<GameTurn>(),
                            GameSelections = new Queue<GameSelections>(),
                            GameRolls = new Queue<GameRoll>()
                        });
                        return Ok(newGameId);
                    }
                    else if (!UsersSearching.Select(x => x.UserId).Contains(UserId))
                    {
                        UsersSearching.Enqueue(new MatchmakingUser() { UserId = UserId, Wager = wager });
                    }
                }
                return Ok(0);
            }
        }
        [HttpGet]
        [Route("api/Multiplayer/FriendGameStarted")]
        public ActionResult FriendGameStarted(int UserId)
        {
            UpdatePing(UserId);
            var game = ActiveGames.FirstOrDefault(x => (x.Player1Id == UserId || x.Player2Id == UserId) && x.IsFriendGame);
            if (game != null)
            {
                if (game.StartedPlaying)
                    return Ok(game.GameId);
                else
                    return Ok(0);
            }
            return Ok(game);
        }

        [HttpGet]
        [Route("api/Multiplayer/CheckForFriendGameInvite")]
        public ActionResult CheckForFriendGameInvite(int UserId)
        {
            UpdatePing(UserId);
            var game = ActiveGames.FirstOrDefault(x => (x.Player1Id == UserId || x.Player2Id == UserId) && x.IsFriendGame && !x.StartedPlaying);
            if (game != null)
            {
                return Ok(game.GameId);
            }
            return Ok(0);
        }

        [HttpGet]
        [Route("api/Multiplayer/StartFriendGame")]
        public ActionResult StartFriendGame(int UserId)
        {
            UpdatePing(UserId);
            var game = ActiveGames.FirstOrDefault(x => x.Player1Id == UserId || x.Player2Id == UserId);
            if (game != null)
            {
                if (game.StartedPlaying == false)
                {
                    game.StartedPlaying = true;
                    return Ok(game.GameId);
                }
            }
            return Ok(0);
        }
        [HttpGet]
        [Route("api/Multiplayer/FriendGameInvite")]
        public ActionResult FriendGameInvite(int UserId, int OtherUserId)
        {
            var timeNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, easternZone);
            var timeBackward = timeNow.AddSeconds(-10);
            UpdatePing(UserId);
            var otherUser = Pings.FirstOrDefault(x => x.UserId == OtherUserId && x.PlayerLastPing > timeBackward);
            if (otherUser != null)
            {
                if (!ActiveGames.Any(x => x.Player1Id == OtherUserId || x.Player2Id == OtherUserId))
                {
                    using (PotStirreresDBContext context = new PotStirreresDBContext())
                    {
                        var playerX = context.Players.First(x => x.UserId == OtherUserId);
                        var playerY = context.Players.First(x => x.UserId == UserId);
                        var newGameId = CreateGame(playerX.UserId, playerY.UserId, 0, true);
                        Random rand = new Random();
                        var turn = rand.Next(0, 2) == 0;
                        ActiveGames.Add(new GameState()
                        {
                            GameId = newGameId,
                            Player1Id = playerX.UserId,
                            Player2Id = playerY.UserId,
                            CreatedDate = timeNow,
                            StartedPlaying = false,
                            IsFriendGame = true,
                            ShouldTrash = null,
                            IsPlayer1Turn = turn,
                            GameTurns = new Queue<GameTurn>(),
                            GameSelections = new Queue<GameSelections>(),
                            GameRolls = new Queue<GameRoll>()
                        });
                        return Ok(newGameId);
                    }
                }
                else
                {
                    return Ok(0);
                }
            }
            else
            {
                return Ok(0);
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
        public ActionResult StopLookingforGame(int UserId)
        {
            if (UsersSearching.Select(x => x.UserId).Contains(UserId))
                UsersSearching = new Queue<MatchmakingUser>(UsersSearching.Where(x => x.UserId != UserId));
            return Ok(true);
        }

        [HttpGet]
        [Route("api/Multiplayer/DeclineFriendGame")]
        public ActionResult DeclineFriendGame(int GameId)
        {
            var game = ActiveGames.FirstOrDefault(x => x.GameId == GameId);
            if (game != null)
                ActiveGames.Remove(game);
            return Ok(true);
        }

        [HttpGet]
        [Route("api/Multiplayer/GameEnd")]
        public ActionResult GameEnd(int GameId, int Player1Cooked, int Player2Cooked, int TotalTurns, int RageQuit = 0)
        {
            var timeNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, easternZone);
            var message = "";
            var game = ActiveGames.FirstOrDefault(x => x.GameId == GameId);
            if (game != null)
            {
                ActiveGames.Remove(game);
            }
            using (PotStirreresDBContext context = new PotStirreresDBContext())
            {
                var rewardsGiven = false;
                var oldGame = context.GameAnalytics.Include(x=>x.Player1).Include(x=>x.Player2).First(x => x.GameId == GameId);
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

                var player1 = context.Players.Include(x => x.TitlesNavigation).First(x => x.UserId == oldGame.Player1.UserId);
                var player2 = context.Players.Include(x => x.TitlesNavigation).First(x => x.UserId == oldGame.Player2.UserId);

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
                    if (!oldGame.IsCpuGame)
                        winner.OnlineWins++;
                    else
                        winner.Wins++;

                    if (!oldGame.IsFriendGame && (!oldGame.IsCpuGame || oldGame.Player2Id == 42))
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

                            if (player1.UserId == 5 && !player1Won && !player1.TitlesNavigation.Any(x => x.TitleId == 11))
                            {
                                player2.TitlesNavigation.Add(context.Titles.First(x => x.TitleId == 11));
                            }
                            else if (player2.UserId == 5 && player1Won && !player2.TitlesNavigation.Any(x => x.TitleId == 11))
                            {
                                player1.TitlesNavigation.Add(context.Titles.First(x => x.TitleId == 11));
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
                    else if (oldGame.IsCpuGame)
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

                if (RageQuit == 0)
                {
                    message += $" {winner.Username} Won! \n \n";
                }
                else
                {
                    message += $" {loser.Username} Quit! \n \n";
                }

                message += $"The match lasted {TotalTurns} turns! \n \n";
                if (oldGame.IsFriendGame)
                {
                    message += $"This was a friend match so no extra rewards or wagers were earned.";
                }
                else if (oldGame.IsCpuGame)
                {
                    if (player1Won)
                    {
                        if (player2.UserId == 41)
                        {
                            message += $"Jenn gets better each time you win against her, so play again and see if you got what it takes! You earned 50 Calories.";
                        }
                        else if (player2.UserId == 42)
                        {
                            message += $"Great work, can't believe you beat Ethan! You won 100 Calories!";
                        }
                        else
                        {
                            message += $"Great work beating the tutorial! Now go try out the game online!";
                        }
                    }
                    else
                    {
                        if (player2.UserId == 41)
                        {
                            message += "Keep practicing, there's more skill to the game than you might think!";
                        }
                        else if (player2.UserId == 42)
                        {
                            message += "Keep practicing, until then Ethan took 100 Calories from you.";
                        }
                        else
                        {
                            message += $"No worries, many of the greats have lost vs Mike, try again from the main menu!";
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
                return Content(message);
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
