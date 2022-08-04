using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


public class ChestDTO
{
    public int ChestId { get; set; }
    public int ChestTypeId { get; set; }
    public int ChestSize { get; set; }
    public DateTime? FinishUnlock { get;set; }
}