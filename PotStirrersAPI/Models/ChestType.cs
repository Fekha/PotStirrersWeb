using System;
using System.Collections.Generic;

namespace PotStirrersAPI.Models
{
    public partial class ChestType
    {
        public ChestType()
        {
            Chests = new HashSet<Chest>();
        }

        public int ChestTypeId { get; set; }
        public string ChestTypeName { get; set; } = null!;

        public virtual ICollection<Chest> Chests { get; set; }
    }
}
