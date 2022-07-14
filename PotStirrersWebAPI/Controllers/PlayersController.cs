﻿using DataModel;
using PotStirrersWebAPI.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace PotStirrersWebAPI.Controllers
{
    public class PlayersController : ApiController
    {
        TimeZoneInfo easternZone = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");

        [HttpGet]
        [Route("api/player/GetUserByName")]
        public PlayerDTO GetUserByName(string username)
        {
            using (PotStirreresDBEntities context = new PotStirreresDBEntities())
            {
                return context.Players.Where(x => x.Username == username).ToList().Select(x => new PlayerDTO(x)).FirstOrDefault();
            }
        }

        [HttpGet]
        [Route("api/player/LoginUser")]
        public IHttpActionResult LoginUser(string username, string password, bool rememberMe, Guid deviceId)
        {
            using (PotStirreresDBEntities context = new PotStirreresDBEntities())
            {
                var data = context.Players.Where(x => x.Username == username && x.Password == password).ToList().Select(x => new PlayerDTO(x)).FirstOrDefault();
                if (data != null)
                {
                    context.SaveChanges();
                    RememberDevice(rememberMe, deviceId, data.UserId, context);
                }
                return Json(data);
            }
        }

        private void RememberDevice(bool rememberMe, Guid deviceId, int userId, PotStirreresDBEntities context)
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
        public IHttpActionResult GetDevice(Guid deviceId)
        {
            using (PotStirreresDBEntities context = new PotStirreresDBEntities())
            {
                PlayerDTO user = null;
                if (context.Devices.Any(x => x.DeviceId == deviceId))
                {
                    user = new PlayerDTO(context.Devices.FirstOrDefault(x => x.DeviceId == deviceId).Player);
                }
                return Json(user);
            }
        } 
        
        [HttpGet]
        [Route("api/player/RegisterUser")]
        public IHttpActionResult RegisterUser(string username, string password, bool rememberMe, Guid deviceId)
        {
            var timeNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, easternZone);
            using (PotStirreresDBEntities context = new PotStirreresDBEntities())
            {
                Player user = null;
                if (GetUserByName(username) == null)
                {
                    user = new Player()
                    {
                        Username = username,
                        Password = password,
                        CreatedDate = timeNow,
                        Email = "",
                        Stars = 900,
                        Level = 1
                    };
                    var feca = context.Players.FirstOrDefault(x => x.UserId == 5);
                    context.Players.Add(user);
                    context.SaveChanges();
                    user.Messages.Add(new Message()
                    {
                        Subject = "Welcome to Potstirrers!",
                        Body = "You are amazing! Please let me know any and all feedback you might have!",
                        FromId = 5,
                        CreatedDate = timeNow
                    });
                    user.Players.Add(feca);
                    feca.Players.Add(user);
                    context.SaveChanges();
                    RememberDevice(rememberMe, deviceId, user.UserId, context);
                    return Json(new PlayerDTO(user));
                }
                return Json(user);
            }
        }       
       
        [HttpGet]
        [Route("api/player/UpdateSettings")]
        public IHttpActionResult UpdateSettings(int UserId, bool WineMenu, bool UseD8s, bool DisableDoubles, bool PlayAsPurple)
        {
            using (PotStirreresDBEntities context = new PotStirreresDBEntities())
            {
                var dbPlayer = context.Players.FirstOrDefault(x => x.UserId == UserId);
                if (dbPlayer != null) {
                    dbPlayer.UseD8s = UseD8s;
                    dbPlayer.DisableDoubles = DisableDoubles;
                    dbPlayer.WineMenu = WineMenu;
                    dbPlayer.PlayAsPurple = PlayAsPurple;
                }
                context.SaveChanges();
                return Json(dbPlayer);
            }
        }

        [HttpGet]
        [Route("api/player/GetAppVersion")]
        public IHttpActionResult GetAppVersion()
        {
            using (PotStirreresDBEntities context = new PotStirreresDBEntities())
            {
                var ver = context.AppVersions.FirstOrDefault();
                return Json(ver.AppVersion1);
            }
        }

        [HttpGet]
        [Route("api/player/UpdateLevel")]
        public IHttpActionResult UpdateLevel(int userId)
        {
            using (PotStirreresDBEntities context = new PotStirreresDBEntities())
            {
                string returnText = "";
                bool leveledUp = false;
                int starsGained = 0;
                var player = context.Players.FirstOrDefault(x => x.UserId == userId);
                if (player != null)
                {
                    var amountToLevel = (100 + (player.Level * 50));
                    while (player.Xp >= amountToLevel)
                    {
                        var starsToGain = (player.Level * 5) + 100;
                        leveledUp = true;
                        player.Xp -= amountToLevel;
                        player.Level += 1;
                        player.Stars += starsToGain;
                        starsGained += starsToGain;
                    }
                }
                context.SaveChanges();
                if (leveledUp)
                {
                    returnText = $"Congrats you reached level {player.Level}! \n \n You gained {starsGained} Calories!";
                }
                return Json(returnText);
            }
        }
        
        [HttpGet]
        [Route("api/player/CheckForReward")]
        public IHttpActionResult CheckForReward(int userId)
        {
            var timeNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, easternZone);

            using (PotStirreresDBEntities context = new PotStirreresDBEntities())
            {
                string returnText = "";
                var player = context.Players.FirstOrDefault(x => x.UserId == userId);
                if (player.LoggedIns.Count() == 0 || timeNow.Date > player.LoggedIns.OrderByDescending(x=>x.LoginDate).FirstOrDefault().LoginDate)
                {
                    var daysLoggedIn = 1;
                    while (player.LoggedIns.Any(x => DateTime.Compare(x.LoginDate.Date, timeNow.Date.AddDays(daysLoggedIn * -1).Date) == 0))
                    {
                        daysLoggedIn++;
                    }
                    int starsGained = 75 + (Math.Min(daysLoggedIn, 7) * 25);
                    player.Stars += starsGained;
                    returnText = $"Day {daysLoggedIn} login bonus: \n \n You gained {starsGained} Calories! {(daysLoggedIn == 7 ? "Great job, the bonus max's out at 7 days, but keep coming back so it wont reset!" : "")}";
                }

                player.LoggedIns.Add(new LoggedIn() {  UserId = player.UserId, LoginDate = timeNow });
                context.SaveChanges();
                return Json(returnText);
            }
        }

        [HttpGet]
        [Route("api/player/GetLeaderboard")]
        public IHttpActionResult GetLeaderboard()
        {
            using (PotStirreresDBEntities context = new PotStirreresDBEntities())
            {             
                return Json(context.PlayerProfiles.Where(x=>x.Username != "Jenn" && x.Username != "Chrissy" && x.Username != "Zach" && x.Username != "Joe").ToList());
            }
        }

        [HttpGet]
        [Route("api/player/GetProfile")]
        public IHttpActionResult GetProfile(string username)
        {
            using (PotStirreresDBEntities context = new PotStirreresDBEntities())
            {
                return Json(context.PlayerProfiles.FirstOrDefault(x => x.Username == username));
            }
        }
        
        [HttpGet]
        [Route("api/player/GetFriends")]
        public IHttpActionResult GetFriends(int userId)
        {
            using (PotStirreresDBEntities context = new PotStirreresDBEntities())
            {
                var friends = context.Players.FirstOrDefault(x => x.UserId == userId).Player1.Select(y => new FriendDTO() { 
                    Username = y.Username,
                    RealFriend = y.Player1.Any(z => z.UserId == userId),
                    Level = y.Level
                }).ToList();
                return Json(friends);
            }
        } 
        
        [HttpGet]
        [Route("api/player/EditFriend")]
        public IHttpActionResult EditFriend(int userId, string username, bool add)
        {
            var timeNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, easternZone);

            using (PotStirreresDBEntities context = new PotStirreresDBEntities())
            {
                var player = context.Players.FirstOrDefault(x => x.UserId == userId);
                var friend = context.Players.FirstOrDefault(x => x.Username == username);
                if (add)
                {
                    if (!player.Player1.Any(x => x.Username == username))
                    {
                        player.Player1.Add(friend);
                        context.Messages.Add(new Message()
                        {
                            Subject = player.Username + " added you as a friend!",
                            Body = friend.Player1.Any(x => x.UserId == userId) ? "You have both added each other so you can start sending each other messages!" : "Add them to your friends list to send messages to each other!",
                            FromId = player.UserId,
                            UserId = friend.UserId,
                            CreatedDate = timeNow
                        });
                    }
                }
                else
                {
                    if (player.Player1.Any(x => x.Username == username))
                        player.Player1.Remove(friend);
                }

                context.SaveChanges();
                return GetFriends(userId);
            }
        }
        

        [HttpGet]
        [Route("api/player/GetMessages")]
        public IHttpActionResult GetMessages(int userId)
        {
            using (PotStirreresDBEntities context = new PotStirreresDBEntities())
            {
                return Json(context.Messages.Where(x => x.UserId == userId && !x.IsDeleted).ToList().Select(x=>new MessageDTO(x)).ToList());
            }
        }

        [HttpGet]
        [Route("api/player/ReadMessage")]
        public IHttpActionResult ReadMessage(int MessageID)
        {
            using (PotStirreresDBEntities context = new PotStirreresDBEntities())
            {
                var message = context.Messages.FirstOrDefault(x => x.MessageId == MessageID);
                message.IsRead = true;
                context.SaveChanges();
                return GetMessages(message.UserId);
            }
        }
        
        [HttpGet]
        [Route("api/player/DeleteMessage")]
        public IHttpActionResult DeleteMessage(int MessageID)
        {
            using (PotStirreresDBEntities context = new PotStirreresDBEntities())
            {
                var message = context.Messages.FirstOrDefault(x => x.MessageId == MessageID);
                message.IsDeleted = true;
                context.SaveChanges();
                return GetMessages(message.UserId);
            }
        }
        [HttpGet]
        [Route("api/player/SendMessage")]
        public IHttpActionResult SendMessage(int userId, string toName, string subject, string body)
        {
            var timeNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, easternZone);

            using (PotStirreresDBEntities context = new PotStirreresDBEntities())
            {
                var friend = context.Players.FirstOrDefault(x => x.Username == toName);
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
            return Json(true);
        }
    }
}
