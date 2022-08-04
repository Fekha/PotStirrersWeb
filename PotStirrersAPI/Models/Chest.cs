using System;
using System.Collections.Generic;

namespace PotStirrersAPI.Models
{
    public partial class Chest
    {
        public int ChestId { get; set; }
        public int UserId { get; set; }
        public int ChestSize { get; set; }
        public bool IsOpened { get; set; }
        public int ChestTypeId { get; set; }
        public DateTime? FinishUnlock { get; set; }

        public virtual ChestType ChestType { get; set; } = null!;
        public virtual Player User { get; set; } = null!;
    }
}
