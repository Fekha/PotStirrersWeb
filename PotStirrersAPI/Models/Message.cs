using System;
using System.Collections.Generic;

namespace PotStirrersAPI.Models
{
    public partial class Message
    {
        public int MessageId { get; set; }
        public int UserId { get; set; }
        public string Subject { get; set; } = null!;
        public string Body { get; set; } = null!;
        public bool IsRead { get; set; }
        public DateTime CreatedDate { get; set; }
        public int FromId { get; set; }
        public bool IsDeleted { get; set; }

        public virtual Player From { get; set; } = null!;
        public virtual Player User { get; set; } = null!;
    }
}
