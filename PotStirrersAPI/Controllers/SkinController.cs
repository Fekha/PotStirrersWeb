using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PotStirrersAPI.Models;

namespace PotStirrersWebAPI.Controllers
{
    public class SkinController : Controller
    {
        TimeZoneInfo easternZone = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
        Random random = new Random();

        [HttpGet]
        [Route("api/skin/UseKey")]
        public ActionResult UseKey(int userId, string key)
        {
            using (PotStirreresDBContext context = new PotStirreresDBContext())
            {
                var reward = context.GiveawayKeys.FirstOrDefault(x => x.KeyCode == key && !x.IsClaimed);
                if (reward != null)
                {
                    var player = context.Players.First(x => x.UserId == userId);
                    reward.IsClaimed = true;
                    reward.ClaimedTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, easternZone);
                    reward.ClaimedById = userId;
                    player.Calories += reward.RewardAmount;
                    context.SaveChanges();
                    return Ok(reward.RewardAmount);
                }
                return Ok(0);
            }
        }
        [HttpGet]
        [Route("api/skin/CraftSkins")]
        public ActionResult CraftSkins(int UserId, int ToDeleteSkinId, int ToCraftSkinId, bool isDie)
        {
            using (PotStirreresDBContext context = new PotStirreresDBContext())
            {
                var player = context.Players.First(x => x.UserId == UserId);
                int cost = 9999999;
                if (!isDie)
                {
                    var toDeleteFrom = context.UserIngredientUnlocks.Include(x => x.IngredientSkin).First(x => x.UserId == UserId && x.IngredientSkinId == ToDeleteSkinId);
                    var toAddTo = context.UserIngredientUnlocks.Include(x => x.IngredientSkin).First(x => x.UserId == UserId && x.IngredientSkinId == ToCraftSkinId);
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
                    var toDeleteFrom = context.UserDiceUnlocks.Include(x => x.DiceSkin).First(x => x.UserId == UserId && x.DiceSkinId == ToDeleteSkinId);
                    var toAddTo = context.UserDiceUnlocks.Include(x => x.DiceSkin).First(x => x.UserId == UserId && x.DiceSkinId == ToCraftSkinId);
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
                return Ok(true);
            }
        }
        [HttpGet]
        [Route("api/skin/UpdateIngredientSkins")]
        public ActionResult UpdateIngredientSkins(int UserId, int skinId, bool add)
        {
            using (PotStirreresDBContext context = new PotStirreresDBContext())
            {
                var dbPlayer = context.Players.Include(x => x.IngredientSkins).First(x => x.UserId == UserId);
                if (add && !dbPlayer.IngredientSkins.Any(x => x.IngredientSkinId == skinId))
                {
                    dbPlayer.IngredientSkins.Add(context.IngredientSkins.First(x => x.IngredientSkinId == skinId));
                }
                if (!add && dbPlayer.IngredientSkins.Any(x => x.IngredientSkinId == skinId))
                {
                    dbPlayer.IngredientSkins.Remove(context.IngredientSkins.First(x => x.IngredientSkinId == skinId));
                }
                context.SaveChanges();
                return Ok(true);
            }
        }

        [HttpGet]
        [Route("api/skin/GetMyIngredientSkins")]
        public ActionResult GetMyIngredientSkins(int UserId)
        {
            using (PotStirreresDBContext context = new PotStirreresDBContext())
            {
                var skins = context.UserIngredientUnlocks.Where(x=>x.UserId == UserId).Select(x => new SkinDTO()
                {
                    SkinId = x.IngredientSkinId,
                    IsUnlocked = x.SkinOwned,
                    UnlockedQty = x.SkinQty,
                    SkinType = 1,
                    Rarity = x.IngredientSkin.Rarity
                }).ToList();
                return Ok(skins);
            }
        }

        [HttpGet]
        [Route("api/skin/UpdateDiceSkins")]
        public ActionResult UpdateDiceSkins(int UserId, int skinId, bool add)
        {
            using (PotStirreresDBContext context = new PotStirreresDBContext())
            {
                var dbPlayer = context.Players.Include(x=>x.DiceSkins).First(x => x.UserId == UserId);
                if (add && !dbPlayer.DiceSkins.Any(x => x.DiceSkinId == skinId))
                {
                    dbPlayer.DiceSkins.Add(context.DiceSkins.First(x => x.DiceSkinId == skinId));
                }
                if (!add && dbPlayer.DiceSkins.Any(x => x.DiceSkinId == skinId))
                {
                    dbPlayer.DiceSkins.Remove(context.DiceSkins.First(x => x.DiceSkinId == skinId));
                }
                context.SaveChanges();
                return Ok(true);
            }
        }
        [HttpGet]
        [Route("api/skin/UnlockIngSkin")]
        public ActionResult UnlockIngSkin(int UserId, int SkinId)
        {
            using (PotStirreresDBContext context = new PotStirreresDBContext())
            {
                var dbPlayer = context.Players.Include(x => x.UserIngredientUnlocks).ThenInclude(x=>x.IngredientSkin).First(x => x.UserId == UserId);
                var toUnlock = dbPlayer.UserIngredientUnlocks.First(x => x.IngredientSkinId == SkinId);
                var cost = (toUnlock.IngredientSkin.Rarity == 3 ? 1500 : toUnlock.IngredientSkin.Rarity == 2 ? 1000 : 500);
                if (dbPlayer.Calories >= cost)
                {
                    toUnlock.SkinQty -= 4;
                    toUnlock.SkinOwned = true;
                    dbPlayer.Calories -= cost;
                }
                context.SaveChanges();
                return Ok(true);
            }
        }
        [HttpGet]
        [Route("api/skin/UnlockDiceSkin")]
        public ActionResult UnlockDiceSkin(int UserId, int SkinId)
        {
            using (PotStirreresDBContext context = new PotStirreresDBContext())
            {
                var dbPlayer = context.Players.Include(x => x.UserDiceUnlocks).ThenInclude(x => x.DiceSkin).First(x => x.UserId == UserId);
                var toUnlock = dbPlayer.UserDiceUnlocks.First(x => x.DiceSkinId == SkinId);
                var cost = (toUnlock.DiceSkin.Rarity == 3 ? 1500 : toUnlock.DiceSkin.Rarity == 2 ? 1000 : 500);
                if (dbPlayer.Calories >= cost)
                {
                    toUnlock.DiceFaceUnlockedQty -= 10;
                    toUnlock.DieOwned = true;
                    dbPlayer.Calories -= cost;
                }
                context.SaveChanges();
                return Ok(true);
            }
        }
        [HttpGet]
        [Route("api/skin/GetMyDiceSkins")]
        public ActionResult GetMyDiceSkins(int UserId)
        {
            using (PotStirreresDBContext context = new PotStirreresDBContext())
            {
                var skins = context.UserDiceUnlocks.Where(x => x.UserId == UserId).Select(x => new SkinDTO()
                {
                    SkinId = x.DiceSkinId,
                    IsUnlocked = x.DieOwned,
                    UnlockedQty = x.DiceFaceUnlockedQty,
                    SkinType = 2,
                    Rarity = x.DiceSkin.Rarity
                }).ToList();
                return Ok(skins);
            }
        }

        [HttpGet]
        [Route("api/skin/UpdateTitle")]
        public ActionResult UpdateTitle(int UserId, int skinId, bool add)
        {
            using (PotStirreresDBContext context = new PotStirreresDBContext())
            {
                var dbPlayer = context.Players.Include(x => x.Titles).First(x => x.UserId == UserId);
                if (add && !dbPlayer.Titles.Any(x => x.TitleId == skinId))
                {
                    dbPlayer.Titles.Add(context.Titles.First(x => x.TitleId == skinId));
                }
                if (!add && dbPlayer.Titles.Any(x => x.TitleId == skinId))
                {
                    dbPlayer.Titles.Remove(context.Titles.First(x => x.TitleId == skinId));
                }
                context.SaveChanges();
                return Ok(true);
            }
        }

        [HttpGet]
        [Route("api/skin/GetMyTitles")]
        public ActionResult GetMyTitles(int UserId)
        {
            using (PotStirreresDBContext context = new PotStirreresDBContext())
            {
                var dbPlayer = context.Players.Include(x => x.TitlesNavigation).First(x => x.UserId == UserId);
                var skins = dbPlayer.TitlesNavigation.Select(x => new SkinDTO()
                {
                    SkinId = x.TitleId,
                    SkinName = x.TitleName,
                    SkinDesc = x.EarnDescription,
                    IsUnlocked = true
                }).ToList();
                return Ok(skins);
            }
        }

        [HttpGet]
        [Route("api/skin/GetAllTitles")]
        public ActionResult GetAllTitles()
        {
            using (PotStirreresDBContext context = new PotStirreresDBContext())
            {
                var skins = context.Titles.Select(x => new SkinDTO()
                {
                    SkinId = x.TitleId,
                    SkinName = x.TitleName,
                    SkinDesc = x.EarnDescription
                }).ToList();
                return Ok(skins);
            }
        }

        [HttpGet]
        [Route("api/skin/GetMyChests")]
        public ActionResult GetChests(int UserId)
        {
            using (PotStirreresDBContext context = new PotStirreresDBContext())
            {
                var chests = context.Chests.Where(x => x.UserId == UserId && !x.IsOpened).OrderByDescending(x => x.FinishUnlock).Select(x => new ChestDTO()
                {
                    ChestId = x.ChestId,
                    ChestSize = x.ChestSize,
                    ChestTypeId = x.ChestTypeId,
                    FinishUnlock = x.FinishUnlock
                }).Take(4).ToList();
                return Ok(chests);
            }
        }

        [HttpGet]
        [Route("api/skin/OpenMyChest")]
        public ActionResult OpenMyChest(int UserId, int ChestId)
        {
            using (PotStirreresDBContext context = new PotStirreresDBContext())
            {
                List<DiceSkin> unlockedDice = new List<DiceSkin>();
                List<IngredientSkin> unlockedIngs = new List<IngredientSkin>();
                List<SkinDTO> unlockedSkin = new List<SkinDTO>();
                var chest = context.Chests.First(x => x.UserId == UserId && x.ChestId == ChestId && !x.IsOpened);
                chest.IsOpened = true;

                if (chest.ChestTypeId == 1)
                {
                    for (int i = 0; i < chest.ChestSize; i++)
                    {
                        var num = getIngredientToUnlock();
                        unlockedIngs.Add(context.IngredientSkins.First(x => x.IngredientSkinId == num));
                    }
                }
                else if (chest.ChestTypeId == 2)
                {
                    for (int i = 0; i < chest.ChestSize * 2; i++)
                    {
                        var num = getDieToUnlock();
                        unlockedDice.Add(context.DiceSkins.First(x => x.DiceSkinId == num));
                    }
                }
                unlockedIngs.ForEach(x =>
                {
                    var die = context.UserIngredientUnlocks.FirstOrDefault(y => y.IngredientSkinId == x.IngredientSkinId && y.UserId == UserId);
                    if (die != null)
                    {
                        die.SkinQty++;
                    }
                    else
                    {
                        die = new UserIngredientUnlock()
                        {
                            IngredientSkinId = x.IngredientSkinId,
                            UserId = UserId,
                            SkinQty = 1,
                        };
                        context.UserIngredientUnlocks.Add(die);
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
                    var die = context.UserDiceUnlocks.FirstOrDefault(y => y.DiceSkinId == x.DiceSkinId && y.UserId == UserId);
                    if (die != null)
                    {
                        die.DiceFaceUnlockedQty++;
                    }
                    else
                    {
                        die = new UserDiceUnlock()
                        {
                            DiceSkinId = x.DiceSkinId,
                            UserId = UserId,
                            DiceFaceUnlockedQty = 1,
                        };
                        context.UserDiceUnlocks.Add(die);
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
                return Ok(unlockedSkin);
            }
        }
        [HttpGet]
        [Route("api/skin/StartChestUnlock")]
        public ActionResult StartChestUnlock(int ChestId)
        {
            var timeNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, easternZone);
            using (PotStirreresDBContext context = new PotStirreresDBContext())
            {
                var chest = context.Chests.First(x => x.ChestId == ChestId && !x.IsOpened);
                chest.FinishUnlock = timeNow.AddHours(chest.ChestSize * 2);
                context.SaveChanges();
                return Ok(chest.FinishUnlock);
            }
        }
        [HttpGet]
        [Route("api/skin/PurchaseChestUnlock")]
        public ActionResult PurchaseChestUnlock(int ChestId)
        {
            var timeNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, easternZone);
            using (PotStirreresDBContext context = new PotStirreresDBContext())
            {
                var chest = context.Chests.Include(x => x.User).First(x => x.ChestId == ChestId && !x.IsOpened);
                if (chest.FinishUnlock != null && !chest.IsOpened)
                {
                    if (chest.User.Level < 5)
                    {
                        chest.FinishUnlock = timeNow;
                    }
                    else
                    {
                        TimeSpan cost = (DateTime)chest.FinishUnlock - timeNow;
                        if (chest.User.Calories >= cost.Minutes)
                        {
                            chest.User.Calories -= (int)cost.Minutes;
                            chest.FinishUnlock = timeNow;
                        }
                    }
                    context.SaveChanges();
                }
                return Ok(chest.FinishUnlock);
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
        public ActionResult CheckForUnlocks(int userId)
        {
            var timeNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, easternZone);

            using (PotStirreresDBContext context = new PotStirreresDBContext())
            {
                string returnText = "";
                var player = context.Players.Include(x => x.TitlesNavigation).First(x => x.UserId == userId);
                context.Titles.Select(x => x.TitleId).ToList().ForEach(x =>
                {
                    if (!player.TitlesNavigation.Select(x => x.TitleId).Contains(x))
                    {
                        var unlocked = CheckOnTitle(x, player);
                        if (unlocked != 0)
                        {
                            var title = context.Titles.First(y => y.TitleId == unlocked);
                            returnText += $"You accomplished '{title.EarnDescription}' and unlocked the '{title.TitleName}' Title! \n \n";
                            player.TitlesNavigation.Add(title);
                        }
                    }
                });
                context.SaveChanges();
                return Content(returnText.Length > 4 ? returnText.Substring(0, returnText.Length - 4) : returnText);
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
