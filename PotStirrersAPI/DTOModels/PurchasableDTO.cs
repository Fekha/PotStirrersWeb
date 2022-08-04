
public partial class PurchasableDTO
{
    public int PurchaseId { get; set; }
    public string PurchaseName { get; set; }
    public int PurchaseCost { get; set; }
    public int LevelMinimum { get; set; }
    public List<int> RequiredPurchase { get; set; }
}