using DataModel;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace PotStirrersWebAPI.Controllers
{
    public class SkinController : ApiController
    {

        TimeZoneInfo easternZone = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
        Random random = new Random();

        [HttpGet]
        [Route("api/skin/UseKey")]
        public IHttpActionResult UseKey(int userId, string key)
        {
            using (PotStirreresDBEntities context = new PotStirreresDBEntities())
            {
                var reward = context.GiveawayKeys.FirstOrDefault(x => x.KeyCode == key && !x.IsClaimed);
                if (reward != null)
                {
                    var player = context.Players.FirstOrDefault(x => x.UserId == userId);
                    reward.IsClaimed = true;
                    reward.ClaimedTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, easternZone);
                    reward.ClaimedById = userId;
                    player.Calories += reward.RewardAmount;
                    context.SaveChanges();
                    return Json(reward.RewardAmount);
                }
                return Json(0);
            }
        }
        [HttpGet]
        [Route("api/skin/CraftSkins")]
        public IHttpActionResult CraftSkins(int UserId, int ToDeleteSkinId, int ToCraftSkinId, bool isDie)
        {
            using (PotStirreresDBEntities context = new PotStirreresDBEntities())
            {
                var player = context.Players.FirstOrDefault(x=>x.UserId == UserId);
                int cost = 9999999;
                if (!isDie)
                {
                    var toDeleteFrom = context.User_Ingredient_Unlock.FirstOrDefault(x => x.UserId == UserId && x.IngredientSkinId == ToDeleteSkinId);
                    var toAddTo = context.User_Ingredient_Unlock.FirstOrDefault(x => x.UserId == UserId && x.IngredientSkinId == ToCraftSkinId);
                    var difference = toDeleteFrom.IngredientSkin.Rarity - toAddTo.IngredientSkin.Rarity;
                    var amountToCraft = (int)(difference > 0 ? Math.Pow(isDie ? 3 : 2, difference) : 1);
                    var amountToDestroy = (int)(difference > 0 ? 1 : Math.Pow(isDie ? 5 : 3, Math.Abs(difference)));
                    if (toDeleteFrom.SkinQty >= amountToDestroy)
                    {
                        toAddTo.SkinQty += amountToCraft;
                        toDeleteFrom.SkinQty -= amountToDestroy;
                        cost = (toAddTo.IngredientSkin.Rarity == 3 ? 450 : toAddTo.IngredientSkin.Rarity == 2 ? 150 : 50) * (isDie ? 1 : 2);
                    }
                }
                else
                {
                    var toDeleteFrom = context.User_Dice_Unlock.FirstOrDefault(x => x.UserId == UserId && x.DiceSkinId == ToDeleteSkinId);
                    var toAddTo = context.User_Dice_Unlock.FirstOrDefault(x => x.UserId == UserId && x.DiceSkinId == ToCraftSkinId);
                    var difference = toDeleteFrom.DiceSkin.Rarity - toAddTo.DiceSkin.Rarity;
                    var amountToCraft = (int)(difference > 0 ? Math.Pow(isDie ? 3 : 2, difference) : 1);
                    var amountToDestroy = (int)(difference > 0 ? 1 : Math.Pow(isDie ? 5 : 3, Math.Abs(difference)));
                    if (toDeleteFrom.DiceFaceUnlockedQty >= amountToDestroy)
                    {
                        toAddTo.DiceFaceUnlockedQty += amountToCraft;
                        toDeleteFrom.DiceFaceUnlockedQty -= amountToDestroy;
                        cost = (toAddTo.DiceSkin.Rarity == 3 ? 450 : toAddTo.DiceSkin.Rarity == 2 ? 150 : 50) * (isDie ? 2 : 1);
                    }
                }
                if (player.Calories >= cost)
                {
                    player.Calories -= cost;
                    context.SaveChanges();
                }
                return Json(true);
            }
        }
        [HttpGet]
        [Route("api/skin/UpdateIngredientSkins")]
        public IHttpActionResult UpdateIngredientSkins(int UserId, int skinId, bool add)
        {
            using (PotStirreresDBEntities context = new PotStirreresDBEntities())
            {
                var dbPlayer = context.Players.FirstOrDefault(x => x.UserId == UserId);
                if (add && !dbPlayer.IngredientSkins.Any(x => x.IngredientSkinId == skinId)) {
                    dbPlayer.IngredientSkins.Add(context.IngredientSkins.FirstOrDefault(x => x.IngredientSkinId == skinId));
                }
                if (!add && dbPlayer.IngredientSkins.Any(x => x.IngredientSkinId == skinId))
                {
                    dbPlayer.IngredientSkins.Remove(context.IngredientSkins.FirstOrDefault(x => x.IngredientSkinId == skinId));
                }
                context.SaveChanges();
                return Json(true);
            }
        }   
        
        [HttpGet]
        [Route("api/skin/GetMyIngredientSkins")]
        public IHttpActionResult GetMyIngredientSkins(int UserId)
        {
            using (PotStirreresDBEntities context = new PotStirreresDBEntities())
            {
                var dbPlayer = context.Players.FirstOrDefault(x => x.UserId == UserId);
                var skins = dbPlayer.User_Ingredient_Unlock.Select(x => new SkinDTO()
                {
                    SkinId = x.IngredientSkinId,
                    IsUnlocked = x.SkinOwned,
                    UnlockedQty = x.SkinQty,
                    SkinType = 1 ,
                    Rarity = x.IngredientSkin.Rarity
                }).ToList();
                return Json(skins);
            }
        }

        [HttpGet]
        [Route("api/skin/UpdateDiceSkins")]
        public IHttpActionResult UpdateDiceSkins(int UserId, int skinId, bool add)
        {
            using (PotStirreresDBEntities context = new PotStirreresDBEntities())
            {
                var dbPlayer = context.Players.FirstOrDefault(x => x.UserId == UserId);
                if (add && !dbPlayer.DiceSkins.Any(x => x.DiceSkinId == skinId))
                {
                    dbPlayer.DiceSkins.Add(context.DiceSkins.FirstOrDefault(x => x.DiceSkinId == skinId));
                }
                if (!add && dbPlayer.DiceSkins.Any(x => x.DiceSkinId == skinId))
                {
                    dbPlayer.DiceSkins.Remove(context.DiceSkins.FirstOrDefault(x => x.DiceSkinId == skinId));
                }
                context.SaveChanges();
                return Json(true);
            }
        }
        [HttpGet]
        [Route("api/skin/UnlockIngSkin")]
        public IHttpActionResult UnlockIngSkin(int UserId, int SkinId)
        {
            using (PotStirreresDBEntities context = new PotStirreresDBEntities())
            {
                var dbPlayer = context.Players.FirstOrDefault(x => x.UserId == UserId);
                var toUnlock = dbPlayer.User_Ingredient_Unlock.FirstOrDefault(x => x.IngredientSkinId == SkinId);
                var cost = (toUnlock.IngredientSkin.Rarity == 3 ? 1500 : toUnlock.IngredientSkin.Rarity == 2 ? 1000 : 500);
                if (dbPlayer.Calories >= cost) {
                    toUnlock.SkinQty -= 4;
                    toUnlock.SkinOwned = true;
                    dbPlayer.Calories -= cost;
                }
                context.SaveChanges();
                return Json(true);
            }
        }
        [HttpGet]
        [Route("api/skin/UnlockDiceSkin")]
        public IHttpActionResult UnlockDiceSkin(int UserId, int SkinId)
        {
            using (PotStirreresDBEntities context = new PotStirreresDBEntities())
            {
                var dbPlayer = context.Players.FirstOrDefault(x => x.UserId == UserId);
                var toUnlock = dbPlayer.User_Dice_Unlock.FirstOrDefault(x => x.DiceSkinId == SkinId);
                var cost = (toUnlock.DiceSkin.Rarity == 3 ? 1500 : toUnlock.DiceSkin.Rarity == 2 ? 1000 : 500);
                if (dbPlayer.Calories >= cost)
                {
                    toUnlock.DiceFaceUnlockedQty -= 10;
                    toUnlock.DieOwned = true;
                    dbPlayer.Calories -= cost;
                }
                context.SaveChanges();
                return Json(true);
            }
        }
        [HttpGet]
        [Route("api/skin/GetMyDiceSkins")]
        public IHttpActionResult GetMyDiceSkins(int UserId)
        {
            using (PotStirreresDBEntities context = new PotStirreresDBEntities())
            {
                var dbPlayer = context.Players.FirstOrDefault(x => x.UserId == UserId);
                var skins = dbPlayer.User_Dice_Unlock.Select(x => new SkinDTO()
                {
                    SkinId = x.DiceSkinId,
                    IsUnlocked = x.DieOwned,
                    UnlockedQty = x.DiceFaceUnlockedQty,
                    SkinType = 2,
                    Rarity = x.DiceSkin.Rarity
                }).ToList();
                return Json(skins);
            }
        }

        [HttpGet]
        [Route("api/skin/UpdateTitle")]
        public IHttpActionResult UpdateTitle(int UserId, int skinId, bool add)
        {
            using (PotStirreresDBEntities context = new PotStirreresDBEntities())
            {
                var dbPlayer = context.Players.FirstOrDefault(x => x.UserId == UserId);
                if (add && !dbPlayer.Titles.Any(x => x.TitleId == skinId))
                {
                    dbPlayer.Titles.Add(context.Titles.FirstOrDefault(x => x.TitleId == skinId));
                }
                if (!add && dbPlayer.Titles.Any(x => x.TitleId == skinId))
                {
                    dbPlayer.Titles.Remove(context.Titles.FirstOrDefault(x => x.TitleId == skinId));
                }
                context.SaveChanges();
                return Json(true);
            }
        }

        [HttpGet]
        [Route("api/skin/GetMyTitles")]
        public IHttpActionResult GetMyTitles(int UserId)
        {
            using (PotStirreresDBEntities context = new PotStirreresDBEntities())
            {
                var dbPlayer = context.Players.FirstOrDefault(x => x.UserId == UserId);
                var skins = dbPlayer.Titles1.Select(x => new SkinDTO()
                {
                    SkinId = x.TitleId,
                    SkinName = x.TitleName,
                    SkinDesc = x.EarnDescription,
                    IsUnlocked = true
                }).ToList();
                return Json(skins);
            }
        }

        [HttpGet]
        [Route("api/skin/GetAllTitles")]
        public IHttpActionResult GetAllTitles()
        {
            using (PotStirreresDBEntities context = new PotStirreresDBEntities())
            {
                var skins = context.Titles.Select(x => new SkinDTO()
                {
                    SkinId = x.TitleId,
                    SkinName = x.TitleName,
                    SkinDesc = x.EarnDescription
                }).ToList();
                return Json(skins);
            }
        }

        [HttpGet]
        [Route("api/skin/GetMyChests")]
        public IHttpActionResult GetChests(int UserId)
        {
            using (PotStirreresDBEntities context = new PotStirreresDBEntities())
            {
                var chests = context.Chests.Where(x=>x.UserId == UserId && !x.IsOpened).OrderByDescending(x=>x.FinishUnlock).Select(x=> new ChestDTO()
                { 
                    ChestId = x.ChestId, 
                    ChestSize = x.ChestSize, 
                    ChestTypeId = x.ChestTypeId,
                    FinishUnlock = x.FinishUnlock
                }).Take(4).ToList();
                return Json(chests);
            }
        }
        
        [HttpGet]
        [Route("api/skin/OpenMyChest")]
        public IHttpActionResult OpenMyChest(int UserId, int ChestId)
        {
            using (PotStirreresDBEntities context = new PotStirreresDBEntities())
            {
                List<DiceSkin> unlockedDice = new List<DiceSkin>();
                List<IngredientSkin> unlockedIngs = new List<IngredientSkin>();
                List<SkinDTO> unlockedSkin = new List<SkinDTO>();
                var chest = context.Chests.FirstOrDefault(x => x.UserId == UserId && x.ChestId == ChestId && !x.IsOpened);
                chest.IsOpened = true;

                if (chest.ChestTypeId == 1)
                {
                    for (int i = 0; i < chest.ChestSize; i++)
                    {
                        var num = getIngredientToUnlock();
                        unlockedIngs.Add(context.IngredientSkins.FirstOrDefault(x => x.IngredientSkinId == num));
                    }
                }
                else if (chest.ChestTypeId == 2)
                {
                    for (int i = 0; i < chest.ChestSize * 2; i++)
                    {
                        var num = getDieToUnlock();
                        unlockedDice.Add(context.DiceSkins.FirstOrDefault(x => x.DiceSkinId == num));
                    }
                }
                unlockedIngs.ForEach(x =>
                {
                    var die = context.User_Ingredient_Unlock.FirstOrDefault(y => y.IngredientSkinId == x.IngredientSkinId && y.UserId == UserId);
                    if (die != null)
                    {
                        die.SkinQty++;
                    }
                    else
                    {
                        die = new User_Ingredient_Unlock()
                        {
                            IngredientSkinId = x.IngredientSkinId,
                            UserId = UserId,
                            SkinQty = 1,
                        };
                        context.User_Ingredient_Unlock.Add(die);
                    }
                    context.SaveChanges();
                    unlockedSkin.Add(new SkinDTO()
                    {
                        SkinId = die.IngredientSkinId,
                        SkinType = 1,
                        UnlockedQty = die.SkinQty,
                        Rarity = die.IngredientSkin.Rarity
                    });
                });
                unlockedDice.ForEach(x =>
                {
                    var die = context.User_Dice_Unlock.FirstOrDefault(y => y.DiceSkinId == x.DiceSkinId && y.UserId == UserId);
                    if (die != null)
                    {
                        die.DiceFaceUnlockedQty++;
                    }
                    else
                    {
                        die = new User_Dice_Unlock()
                        {
                            DiceSkinId = x.DiceSkinId,
                            UserId = UserId,
                            DiceFaceUnlockedQty = 1,
                        };
                        context.User_Dice_Unlock.Add(die);
                    }
                    context.SaveChanges();
                    unlockedSkin.Add(new SkinDTO()
                    {
                        SkinId = die.DiceSkinId,
                        UnlockedQty = die.DiceFaceUnlockedQty,
                        SkinType = 2,
                        Rarity = die.DiceSkin.Rarity
                    });
                });
                
                context.SaveChanges();
                return Json(unlockedSkin);
            }
        }
        [HttpGet]
        [Route("api/skin/StartChestUnlock")]
        public IHttpActionResult StartChestUnlock(int ChestId)
        {
            var timeNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, easternZone);
            using (PotStirreresDBEntities context = new PotStirreresDBEntities())
            {
                var chest = context.Chests.FirstOrDefault(x => x.ChestId == ChestId && !x.IsOpened);
                chest.FinishUnlock = timeNow.AddHours(chest.ChestSize*2);
                context.SaveChanges();
                return Json(chest.FinishUnlock);
            }
        }
        [HttpGet]
        [Route("api/skin/PurchaseChestUnlock")]
        public IHttpActionResult PurchaseChestUnlock(int ChestId)
        {
            var timeNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, easternZone);
            using (PotStirreresDBEntities context = new PotStirreresDBEntities())
            {
                var chest = context.Chests.FirstOrDefault(x => x.ChestId == ChestId && !x.IsOpened);
                if (chest.FinishUnlock != null && !chest.IsOpened)
                {
                    if (chest.Player.Level < 5)
                    {
                        chest.FinishUnlock = timeNow;
                    }
                    else
                    {
                        TimeSpan cost = (DateTime)chest.FinishUnlock - timeNow;
                        if (chest.Player.Calories >= cost.Minutes)
                        {
                            chest.Player.Calories -= (int)cost.Minutes;
                            chest.FinishUnlock = timeNow;
                        }
                    }
                    context.SaveChanges();
                }
                return Json(chest.FinishUnlock);
            }
        }
        private int getDieToUnlock()
        {
            int returnNum = random.Next(0, 10);
            int rarity = random.Next(0, 10);
            rarity = random.Next(0, 26);
            if (rarity == 25)
            {
                returnNum = random.Next(16, 19);
            }
            else if (rarity > 16)
            {
                returnNum = random.Next(10, 16);
            }
            else
            {
                returnNum = random.Next(1, 10);
            }
            return returnNum;
        }
        private int getIngredientToUnlock()
        {
            int returnNum = random.Next(0, 10);
            int rarity = random.Next(0, 10);
            rarity = random.Next(0, 26);
            if (rarity == 25)
            {
                returnNum = random.Next(22, 25);
            }
            else if (rarity > 16)
            {
                returnNum = random.Next(15, 22);
            }
            else
            {
                returnNum = random.Next(5, 15);
            }
            return returnNum;
        }

        [HttpGet]
        [Route("api/skin/CheckForUnlocks")]
        public IHttpActionResult CheckForUnlocks(int userId)
        {
            var timeNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, easternZone);

            using (PotStirreresDBEntities context = new PotStirreresDBEntities())
            {
                string returnText = "";
                var player = context.Players.FirstOrDefault(x => x.UserId == userId);
                var ownedTitles = player.Titles1.Select(x => x.TitleId).ToList();
                var allTitles = context.Titles.Select(x=>x.TitleId).ToList();
                allTitles.ForEach(x =>
                {
                    if (!ownedTitles.Contains(x))
                    {
                        var unlocked = CheckOnTitle(x, player);
                        if (unlocked != 0)
                        {
                            var title = context.Titles.FirstOrDefault(y => y.TitleId == unlocked);
                            returnText += $"You accomplished '{title.EarnDescription}' and unlocked the '{title.TitleName}' Title! \n \n";
                            player.Titles1.Add(title);
                        }
                    }
                });
                context.SaveChanges();
                return Json(returnText.Length > 4 ? returnText.Substring(0,returnText.Length-4):returnText);
            }
        }

        private int CheckOnTitle(int x, Player player)
        {
            switch (x)
            {
                case 3:
                    {
                        if (player.Wins > 0)
                            return x;
                        break;
                    }
                case 4:
                    {
                        if (player.OnlineWins > 0)
                            return x;
                        break;
                    }
                case 5:
                    {
                        if (player.Cooked >= 10)
                            return x;
                        break;
                    }
                case 6:
                    {
                        if (player.Cooked >= 50)
                            return x;
                        break;
                    }
                case 7:
                    {
                        if (player.Cooked >= 100)
                            return x;
                        break;
                    }
                case 8:
                    {
                        if (player.Cooked >= 250)
                            return x;
                        break;
                    }
                case 9:
                    {
                        if (player.Cooked >= 500)
                            return x;
                        break;
                    }
                case 10:
                    {
                        if (player.Cooked >= 1000)
                            return x;
                        break;
                    }
                case 11:
                    {
                        //I'll call this specifically
                        break;
                    }
                case 12:
                    {
                        if (player.Level >= 5)
                            return x;
                        break;
                    }
                case 13:
                    {
                        if (player.Level >= 10)
                            return x;
                        break;
                    }
                case 14:
                    {
                        if (player.Level >= 20)
                            return x;
                        break;
                    }
                case 15:
                    {
                        if (player.Level >= 30)
                            return x;
                        break;
                    }
                case 16:
                    {
                        if (player.Level >= 50)
                            return x;
                        break;
                    }
                case 17:
                    {
                        if (player.Level >= 60)
                            return x;
                        break;
                    }
                case 18:
                    {
                        if (player.Level >= 66)
                            return x;
                        break;
                    }
                case 19:
                    {
                        if (player.Level >= 75)
                            return x;
                        break;
                    }
                case 20:
                    {
                        if (player.Level >= 100)
                            return x;
                        break;
                    } 
                case 21:
                    {
                        if (player.UserId == 14 || player.UserId == 33)
                            return x;
                        break;
                    }
                case 22:
                    {
                        return x;
                    }
                default:
                    break;


            }
            return 0;
        }
    }
}
