using DataModel;
using System.Collections.Generic;
using System.Linq;

public partial class PurchasableDTO
{
    //public PurchasableDTO(Purchasable x)
    //{
    //    PurchaseId = x.PurchaseId;
    //    PurchaseName = x.PurchaseName;
    //    PurchaseCost = x.PurchaseCost;
    //    LevelMinimum = x.LevelMinimum;
    //    RequiredPurchase = x.RequiredPurchases.Select(c=>c.RequiredPurchaseId).ToList();
    //}
    public int PurchaseId { get; set; }
    public string PurchaseName { get; set; }
    public int PurchaseCost { get; set; }
    public int LevelMinimum { get; set; }
    public List<int> RequiredPurchase { get; set; }
}