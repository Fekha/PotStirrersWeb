using DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


public class SkinDTO
{
    public int SkinId { get; set; }
    public string SkinName { get; set; }
    public bool IsUnlocked { get; set; }
    public int UnlockedQty { get; set; }
}