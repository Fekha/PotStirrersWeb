using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PotStirrersAPI.Models;
using PotStirrersWebAPI.Models;

namespace PotStirrersWebAPI.Controllers
{
    public class PlayerController : Controller
    {
        TimeZoneInfo easternZone = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");

        public PlayerDTO? GetUserByName(string username)
        {
            using (PotStirreresDBContext context = new PotStirreresDBContext())
            {
                PlayerDTO? playerDto = null;
                var timeNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, easternZone);
                Player? player = context.Players.FirstOrDefault(x => x.Username == username);
                if (player != null)
                {
                    playerDto = new PlayerDTO(player, null, timeNow);
                }
                return playerDto;
            }
        } 
        
        [HttpGet]
        [Route("api/player/GetUserById")]
        public PlayerDTO? GetUserById(int UserId)
        {
            using (PotStirreresDBContext context = new PotStirreresDBContext())
            {
                PlayerDTO? playerDto = null;
                var timeNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, easternZone);
                Player? player = context.Players.Include(x=>x.DiceSkins)
                    .FirstOrDefault(x => x.UserId == UserId);
                if (player != null)
                {
                    playerDto = new PlayerDTO(player, null, timeNow);
                }
                return playerDto;
            }
        }

        [HttpGet]
        [Route("api/player/LoginUser")]
        public ActionResult LoginUser(string username, string password, bool rememberMe, Guid deviceId, bool isGooglePlay = false)
        {
            using (PotStirreresDBContext context = new PotStirreresDBContext())
            {
               
                var user = context.Players.Where(x => x.Username == username && (x.Password == password || isGooglePlay))
                    .Include(x => x.IngredientSkins)
                    .Include(x => x.DiceSkins)
                    .Include(x => x.Titles)
                    .Include(x => x.UserDiceUnlocks)
                    .Include(x => x.UserIngredientUnlocks)
                    .Include(x => x.TitlesNavigation).FirstOrDefault();
                if (user != null)
                {
                    var friends = context.Players.Where(x => x.UserId == user.UserId).Include(x => x.Friends).First().Friends.Select(x => x.UserId).ToList();
                    //var friends = context.Players.Include(x => x.Friends).Where(x => x.Friends.Select(x => x.UserId).Contains(user.UserId)).Select(x => x.UserId).ToList();
                    RememberDevice(rememberMe, deviceId, user.UserId, context);
                    var player = new PlayerDTO(user);
                    player.Friends = friends;
                    return Ok(player);
                }
                return Ok(user);
            }
        }

        private void RememberDevice(bool rememberMe, Guid deviceId, int userId, PotStirreresDBContext context)
        {
            var player = context.Players.FirstOrDefault(x => x.UserId == userId);
            if (rememberMe)
            {
                var devices = context.Devices.Where(x => x.DeviceId == deviceId && userId != x.UserId);
                if (devices.Count() > 0)
                {
                    context.Devices.RemoveRange(devices);
                }

                if (!context.Devices.Any(x => x.DeviceId == deviceId && userId == x.UserId))
                {
                    context.Devices.Add(new Device() { DeviceId = deviceId, UserId = userId });
                }

            }
            else
            {
                var device = context.Devices.FirstOrDefault(x => x.DeviceId == deviceId && userId == x.UserId);
                if (device != null)
                {
                    context.Devices.Remove(device);
                }
            }
            context.SaveChanges();
        }

        [HttpGet]
        [Route("api/player/GetDevice")]
        public ActionResult GetDevice(Guid deviceId)
        {
            using (PotStirreresDBContext context = new PotStirreresDBContext())
            {
                PlayerDTO? user = null;
                if (context.Devices.Any(x => x.DeviceId == deviceId))
                {
                    user = new PlayerDTO(context.Devices.Include(x=>x.User).First(x => x.DeviceId == deviceId).User);
                }
                return Ok(user);
            }
        }

        [HttpGet]
        [Route("api/player/RegisterUser")]
        public ActionResult RegisterUser(string username, string password, bool rememberMe, Guid deviceId)
        {
            var timeNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, easternZone);
            Random random = new Random();
            using (PotStirreresDBContext context = new PotStirreresDBContext())
            {
                Player? user = null;
                if (GetUserByName(username) == null)
                {
                    user = new Player()
                    {
                        Username = username,
                        Password = password,
                        CreatedDate = timeNow,
                        Email = "",
                        Calories = 900,
                        Level = 1
                    };
                    context.Players.Add(user);
                    context.SaveChanges();
                    user.MessageUsers.Add(new Message()
                    {
                        Subject = "Welcome to Potstirrers!",
                        Body = "You are amazing! \n Message me back to let me know how you feel about the game for a free gift!",
                        FromId = 5,
                        CreatedDate = timeNow
                    });
                    user.Chests.Add(new Chest()
                    {
                        ChestSize = 1,
                        ChestTypeId = random.Next(1, 3)
                    });
                    user.Chests.Add(new Chest()
                    {
                        ChestSize = 2,
                        ChestTypeId = random.Next(1, 3)
                    });
                    user.Chests.Add(new Chest()
                    {
                        ChestSize = 3,
                        ChestTypeId = random.Next(1, 3)
                    });
                    var feca = context.Players.First(x => x.UserId == 5);
                    user.Titles.Add(context.Titles.First(x => x.TitleId == 1));
                    user.Friends.Add(feca);
                    feca.Friends.Add(user);
                    context.SaveChanges();
                    RememberDevice(rememberMe, deviceId, user.UserId, context);
                    return Ok(new PlayerDTO(user));
                }
                return Ok(user);
            }
        }

        [HttpGet]
        [Route("api/player/UpdateSettings")]
        public ActionResult UpdateSettings(int UserId, float MasterVolume, float MusicVolume, float TurnVolume, float VoiceVolume, float EffectVolume, bool? WineMenu = null, bool? PlayAsPurple = null)
        {
            using (PotStirreresDBContext context = new PotStirreresDBContext())
            {
                var dbPlayer = context.Players.FirstOrDefault(x => x.UserId == UserId);
                if (dbPlayer != null)
                {
                    dbPlayer.MasterVolume = MasterVolume;
                    dbPlayer.GameVolume = MusicVolume;
                    dbPlayer.TurnVolume = TurnVolume;
                    dbPlayer.VoiceVolume = VoiceVolume;
                    dbPlayer.EffectsVolume = EffectVolume;
                    if (WineMenu != null)
                        dbPlayer.WineMenu = (bool)WineMenu;
                    if (PlayAsPurple != null)
                        dbPlayer.PlayAsPurple = (bool)PlayAsPurple;
                }
                context.SaveChanges();
                return Ok(true);
            }
        }

        [HttpGet]
        [Route("api/player/GetAppVersion")]
        public ActionResult GetAppVersion(int UserId = 0)
        {
            using (PotStirreresDBContext context = new PotStirreresDBContext())
            {
                if (UserId != 0 && UserId != 40)
                {
                    MultiplayerController.UpdatePing(UserId);
                }
                var ver = context.AppVersions.First();
                return Ok(ver.AppVersion1);
            }
        }

        [HttpGet]
        [Route("api/player/UpdateLevel")]
        public ActionResult UpdateLevel(int userId)
        {
            using (PotStirreresDBContext context = new PotStirreresDBContext())
            {
                string returnText = "";
                bool leveledUp = false;
                int caloriesGained = 0;
                var player = context.Players.First(x => x.UserId == userId);
                var amountToLevel = (100 + (player.Level * 50));
                while (player.Xp >= amountToLevel)
                {
                    var caloriesToGain = (player.Level * 5) + 100;
                    leveledUp = true;
                    player.Xp -= amountToLevel;
                    player.Level += 1;
                    player.Calories += caloriesToGain;
                    caloriesGained += caloriesToGain;
                }
                context.SaveChanges();
                if (leveledUp)
                {
                    returnText = $"Congrats you reached level {player.Level}! \n \n You gained {caloriesGained} Calories!";
                }
                return Content(returnText);
            }
        }

        [HttpGet]
        [Route("api/player/CheckForReward")]
        public ActionResult CheckForReward(int userId)
        {
            var timeNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, easternZone);
            Random random = new Random();
            using (PotStirreresDBContext context = new PotStirreresDBContext())
            {
                string returnText = "";
                var player = context.Players.Include(x => x.LoggedIns).First(x => x.UserId == userId);
                if (player.LoggedIns.Count() == 0 || timeNow.Date > player.LoggedIns.OrderByDescending(x => x.LoginDate).First().LoginDate)
                {
                    var daysLoggedIn = 1;
                    while (player.LoggedIns.Any(x => DateTime.Compare(x.LoginDate.Date, timeNow.Date.AddDays(daysLoggedIn * -1).Date) == 0))
                    {
                        daysLoggedIn++;
                    }
                    int caloriesGained = 100 + (Math.Min(daysLoggedIn, 7) * 15);
                    player.Calories += caloriesGained;
                    var chestSize = MultiplayerController.getChest();
                    var chestType = random.Next(1, 3);
                    context.Chests.Add(new Chest()
                    {
                        ChestSize = chestSize,
                        UserId = player.UserId,
                        ChestTypeId = chestType
                    });
                    returnText = $"Day {daysLoggedIn} login bonus: \n \n You gained {caloriesGained} Calories and a {(chestSize == 3 ? "Large" : chestSize == 2 ? "Medium" : "Small" )} {(chestType == 2 ? "Dice" : "Ingredient")} Pack! {(daysLoggedIn == 7 ? "Great job, the bonus max's out at 7 days, but keep coming back so it wont reset!" : "")}";
                }

                player.LoggedIns.Add(new LoggedIn() { UserId = player.UserId, LoginDate = timeNow });
                context.SaveChanges();
                return Content(returnText);
            }
        }

        [HttpGet]
        [Route("api/player/GetLeaderboard")]
        public ActionResult GetLeaderboard()
        {
            using (PotStirreresDBContext context = new PotStirreresDBContext())
            {
                return Ok(context.PlayerProfiles.Where(x => x.Username != "Jenn" && x.Username != "Chrissy" && x.Username != "Zach" && x.Username != "Joe" && x.Username != "Guest" && x.Username != "Ethan").ToList());
            }
        }

        [HttpGet]
        [Route("api/player/GetProfile")]
        public ActionResult GetProfile(int UserId)
        {
            var timeNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, easternZone);
            using (PotStirreresDBContext context = new PotStirreresDBContext())
            {
                var timeBefore = timeNow.AddSeconds(-10);

                var profile = new PlayerDTO(context.Players.Include(x=>x.Friends).First(x => x.UserId == UserId), context.PlayerProfiles.First(x => x.UserId == UserId), timeNow, timeBefore);
                return Ok(profile);
            }
        }

        [HttpGet]
        [Route("api/player/GetFriends")]
        public ActionResult GetFriends(int userId, bool onlyOnline = false)
        {
            using (PotStirreresDBContext context = new PotStirreresDBContext())
            {
                var timeNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, easternZone);
                var timeBackward = timeNow.AddSeconds(-10);
                var friends = context.Players.Include(x => x.Friends).First(x => x.UserId == userId).Friends.Where(x => !onlyOnline 
                || (MultiplayerController.Pings.Any(y => x.UserId == y.UserId) 
                && MultiplayerController.Pings.First(y => x.UserId == y.UserId).PlayerLastPing > timeBackward)).Select(y => new FriendDTO()
                {
                    UserId = y.UserId,
                    Username = y.Username,
                    RealFriend = y.Friends.Any(z => z.UserId == userId),
                    Level = y.Level,
                }).ToList();
                return Ok(friends);
            }
        }

        [HttpGet]
        [Route("api/player/EditFriend")]
        public ActionResult EditFriend(int userId, string username, bool add)
        {
            var timeNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, easternZone);

            using (PotStirreresDBContext context = new PotStirreresDBContext())
            {
                var player = context.Players.Include(x => x.Friends).First(x => x.UserId == userId);
                var friend = context.Players.Include(x => x.Friends).First(x => x.Username == username);
                if (add)
                {
                    if (!player.Friends.Any(x => x.Username == username))
                    {
                        player.Friends.Add(friend);
                        context.Messages.Add(new Message()
                        {
                            Subject = player.Username + " added you as a friend!",
                            Body = friend.Friends.Any(x => x.UserId == userId) ? "You have both added each other so you can start sending each other messages!" : "Add them to your friends list to send messages to each other!",
                            FromId = player.UserId,
                            UserId = friend.UserId,
                            CreatedDate = timeNow
                        });
                        context.SaveChanges();
                        return Ok(new FriendDTO()
                        {
                            UserId = friend.UserId,
                            Username = friend.Username,
                            RealFriend = friend.Friends.Any(z => z.UserId == userId),
                            Level = friend.Level
                        });
                    }
                }
                else
                {
                    if (player.Friends.Any(x => x.Username == username))
                    {
                        player.Friends.Remove(friend);
                        context.SaveChanges();
                        return Ok(new FriendDTO()
                        {
                            UserId = friend.UserId,
                            Username = friend.Username,
                            RealFriend = friend.Friends.Any(z => z.UserId == userId),
                            Level = friend.Level
                        });
                    }
                }
                return Ok(null);
            }
        }


        [HttpGet]
        [Route("api/player/GetMessages")]
        public ActionResult GetMessages(int userId)
        {
            using (PotStirreresDBContext context = new PotStirreresDBContext())
            {
                return Ok(context.Messages.Include(x=>x.From).Where(x => x.UserId == userId && !x.IsDeleted).Select(x => new MessageDTO(x)).ToList());
            }
        }

        [HttpGet]
        [Route("api/player/ReadMessage")]
        public ActionResult ReadMessage(int MessageID)
        {
            using (PotStirreresDBContext context = new PotStirreresDBContext())
            {
                var message = context.Messages.First(x => x.MessageId == MessageID);
                message.IsRead = true;
                context.SaveChanges();
                return GetMessages(message.UserId);
            }
        }

        [HttpGet]
        [Route("api/player/DeleteMessage")]
        public ActionResult DeleteMessage(int MessageID)
        {
            using (PotStirreresDBContext context = new PotStirreresDBContext())
            {
                var message = context.Messages.First(x => x.MessageId == MessageID);
                message.IsDeleted = true;
                context.SaveChanges();
                return GetMessages(message.UserId);
            }
        }
        [HttpGet]
        [Route("api/player/SendMessage")]
        public ActionResult SendMessage(int userId, string toName, string subject, string body)
        {
            var timeNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, easternZone);

            using (PotStirreresDBContext context = new PotStirreresDBContext())
            {
                var friend = context.Players.First(x => x.Username == toName);
                context.Messages.Add(new Message()
                {
                    Subject = subject,
                    Body = body,
                    FromId = userId,
                    UserId = friend.UserId,
                    CreatedDate = timeNow
                });
                context.SaveChanges();
            }
            return Ok(true);
        }
    }
}
