using System;
using System.Collections.Generic;

namespace PotStirrersAPI.Models
{
    public partial class Device
    {
        public Guid DeviceId { get; set; }
        public int UserId { get; set; }

        public virtual Player User { get; set; } = null!;
    }
}
