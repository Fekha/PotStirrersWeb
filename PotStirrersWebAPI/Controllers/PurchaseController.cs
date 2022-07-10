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
        public IHttpActionResult UpdateDiceSkins(int UserId, List<int> DiceSkins)
        {
            using (PotStirreresDBEntities context = new PotStirreresDBEntities())
            {
                var dbPlayer = context.Players.FirstOrDefault(x => x.UserId == UserId);
                dbPlayer.DiceSkins.Clear();
                DiceSkins.ForEach(x => dbPlayer.DiceSkins.Add(new DiceSkin() { DiceSkinId = x }));
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
                    UnlockedQty = 1
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
                var dbPlayer = context.Players.FirstOrDefault(x => x.UserId == UserId);
                var skins = context.DiceSkins.Select(x => new SkinDTO()
                {
                    SkinId = x.DiceSkinId,
                    SkinName = x.DiceSkinName,
                    UnlockedQty = dbPlayer.User_Dice_Unlock.FirstOrDefault(y=>y.DiceSkinId == x.DiceSkinId).DiceFaceUnlockedQty
                }).ToList();
                return Json(skins);
            }
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
