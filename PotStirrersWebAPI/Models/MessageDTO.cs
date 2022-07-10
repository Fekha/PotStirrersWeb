using DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PotStirrersWebAPI.Models
{
    public class MessageDTO
    {
        public MessageDTO(Message x)
        {
            MessageId = x.MessageId;
            UserId = x.UserId;
            Subject = x.Subject;
            Body = x.Body;
            IsRead = x.IsRead;
            CreatedDate = x.CreatedDate;
            FromName = x.Player1.Username;
        }
        public int MessageId { get; set; }
        public int UserId { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public bool IsRead { get; set; }
        public bool HasFriendedYou { get; set; }
        public string FromName { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}