using System;
using System.Collections.Generic;

namespace PotStirrersAPI.Models
{
    public partial class LoggedIn
    {
        public int UserId { get; set; }
        public DateTime LoginDate { get; set; }

        public virtual Player User { get; set; } = null!;
    }
}
