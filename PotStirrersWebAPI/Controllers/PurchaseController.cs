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
    public class PurchaseController : ApiController
    {

        TimeZoneInfo easternZone = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
        Random random = new Random();
        [HttpGet]
        [Route("api/purchase/GetPlayerPurchasables")]
        public IHttpActionResult GetPlayerPurchases(int UserId)
        {
            using (PotStirreresDBEntities context = new PotStirreresDBEntities())
            {
                var player = context.Players.FirstOrDefault(x => x.UserId == UserId);
                if (player != null)
                {
                    var purchasables = player.User_Purchase.Select(x => x.PurchaseId).ToList();
                    return Json(purchasables);
                }
                return null;
            }
        }

        [HttpGet]
        [Route("api/purchase/UseKey")]
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
                    player.Stars += reward.RewardAmount;
                    context.SaveChanges();
                    return Json(reward.RewardAmount);
                }
                return Json(0);
            }
        }

        [HttpGet]
        [Route("api/purchase/UpdateIngredientSkins")]
        public IHttpActionResult UpdateIngredientSkins(int UserId, int SelectedMeat, int SelectedVeggie, int SelectedFruit, int SelectedFourth)
        {
            using (PotStirreresDBEntities context = new PotStirreresDBEntities())
            {
                var dbPlayer = context.Players.FirstOrDefault(x => x.UserId == UserId);
                if (dbPlayer != null)
                {
                    dbPlayer.SelectedMeat = SelectedMeat;
                    dbPlayer.SelectedVeggie = SelectedVeggie;
                    dbPlayer.SelectedFruit = SelectedFruit;
                    dbPlayer.SelectedFourthIngredient = SelectedFourth;
                }
                context.SaveChanges();
                return Json(dbPlayer);
            }
        }
        
        [HttpGet]
        [Route("api/purchase/UpdateDiceSkins")]
        public IHttpActionResult UpdateDiceSkins(int UserId, int dieId, bool add)
        {
            using (PotStirreresDBEntities context = new PotStirreresDBEntities())
            {
                var dbPlayer = context.Players.FirstOrDefault(x => x.UserId == UserId);
                if (add && !dbPlayer.DiceSkins.Any(x => x.DiceSkinId == dieId)) {
                    dbPlayer.DiceSkins.Add(context.DiceSkins.FirstOrDefault(x => x.DiceSkinId == dieId));
                }
                if (!add && dbPlayer.DiceSkins.Any(x => x.DiceSkinId == dieId))
                {
                    dbPlayer.DiceSkins.Remove(context.DiceSkins.FirstOrDefault(x => x.DiceSkinId == dieId));
                }
                context.SaveChanges();
                return Json(dbPlayer);
            }
        }  

        [HttpGet]
        [Route("api/purchase/GetAllIngredientSkins")]
        public IHttpActionResult GetAllIngredientSkins()
        {
            using (PotStirreresDBEntities context = new PotStirreresDBEntities())
            {
                var skins = context.IngredientSkins.Select(x => new SkinDTO()
                {
                    SkinId = x.IngredientSkinId,
                    SkinName = x.IngredientSkinName,
                    UnlockedQty = 0
                }).ToList();
                return Json(skins);
            }
        }   
        
        [HttpGet]
        [Route("api/purchase/GetMyIngredientSkins")]
        public IHttpActionResult GetMyIngredientSkins(int UserId)
        {
            using (PotStirreresDBEntities context = new PotStirreresDBEntities())
            {
                var dbPlayer = context.Players.FirstOrDefault(x => x.UserId == UserId);
                var skins = dbPlayer.IngredientSkins.Select(x => new SkinDTO()
                {
                    SkinId = x.IngredientSkinId,
                    IsUnlocked = dbPlayer.IngredientSkins.Any(y => y.IngredientSkinId == x.IngredientSkinId)
                }).ToList();
                return Json(skins);
            }
        }
        
        [HttpGet]
        [Route("api/purchase/GetMyDiceSkins")]
        public IHttpActionResult GetMyDiceSkins(int UserId)
        {
            using (PotStirreresDBEntities context = new PotStirreresDBEntities())
            {
                var dbPlayer = context.Players.FirstOrDefault(x => x.UserId == UserId);
                var skins = dbPlayer.User_Dice_Unlock.Select(x => new SkinDTO()
                {
                    SkinId = x.DiceSkinId,
                    IsUnlocked = x.DiceFaceUnlockedQty >= 9,
                    UnlockedQty = x.DiceFaceUnlockedQty
                }).ToList();
                return Json(skins);
            }
        } 
        
        [HttpGet]
        [Route("api/purchase/GetAllDiceSkins")]
        public IHttpActionResult GetAllDiceSkins(int UserId)
        {
            using (PotStirreresDBEntities context = new PotStirreresDBEntities())
            {
                var skins = context.DiceSkins.Select(x => new SkinDTO()
                {
                    SkinId = x.DiceSkinId,
                    SkinName = x.DiceSkinName,
                    UnlockedQty = 0
                }).ToList();
                return Json(skins);
            }
        } 
        
        [HttpGet]
        [Route("api/purchase/GetMyChests")]
        public IHttpActionResult GetChests(int UserId)
        {
            using (PotStirreresDBEntities context = new PotStirreresDBEntities())
            {
                var chests = context.Chests.Where(x=>x.UserId == UserId && !x.IsOpened).Select(x=> new ChestDTO(){ ChestId = x.ChestId, ChestSize = x.ChestSize ?? 1}).Take(4).ToList();
                return Json(chests);
            }
        }
        
        [HttpGet]
        [Route("api/purchase/OpenMyChest")]
        public IHttpActionResult OpenMyChest(int UserId, int ChestId)
        {
            using (PotStirreresDBEntities context = new PotStirreresDBEntities())
            {
                List<DiceSkin> unlocked = new List<DiceSkin>();
                List<SkinDTO> unlockedSkin = new List<SkinDTO>();
                var chest = context.Chests.FirstOrDefault(x => x.UserId == UserId && x.ChestId == ChestId && !x.IsOpened);
                chest.IsOpened = true;
                for (int i = 0; i < chest.ChestSize*2; i++)
                {
                    var num = getDieToUnlock();
                    unlocked.Add(context.DiceSkins.FirstOrDefault(x => x.DiceSkinId == num));
                }
                unlocked.ForEach(x =>
                {
                    var die = context.User_Dice_Unlock.FirstOrDefault(y => y.DiceSkinId == x.DiceSkinId && y.UserId == UserId);
                    if (die != null)
                    {
                        if(die.DiceFaceUnlockedQty<9)
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
                        Rarity = die.DiceSkin.Rarity
                    });
                });
                context.SaveChanges();
                return Json(unlockedSkin);
            }
        }

        private int getDieToUnlock()
        {
            int returnNum = random.Next(0, 10);
            int rarity = random.Next(0, 10);
            rarity = random.Next(0, 10);
            if (rarity == 9)
            {
                returnNum = random.Next(10, 13);
            }
            else if (rarity > 5)
            {
                returnNum = random.Next(7, 10);
            }
            else
            {
                returnNum = random.Next(1, 7);
            }
            return returnNum;
        }

        //[HttpGet]
        //[Route("api/purchase/Purchase")]
        //public IHttpActionResult Purchase(int UserId, int PurchaseId)
        //{
        //    using (PotStirreresDBEntities context = new PotStirreresDBEntities())
        //    {
        //        var purchase = context.Purchasables.FirstOrDefault(x => x.PurchaseId == PurchaseId);
        //        var player = context.Players.FirstOrDefault(x => x.UserId == UserId);
        //        if (player.Stars >= purchase.PurchaseCost)
        //        {
        //            context.User_Purchase.Add(new User_Purchase(){
        //                PurchaseId = purchase.PurchaseId,
        //                UserId = player.UserId
        //            });
        //            player.Stars -= purchase.PurchaseCost;
        //            context.SaveChanges();
        //            return Json(true);
        //        }
        //        else
        //        {
        //            return Json(false);
        //        }
        //    }
        //}
    }

}
