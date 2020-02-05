using System;

namespace DatingApp.API.DTOs
{
    public class MessageForCreationDTO
    {
        public int SenderId { get; set; }
        public int RecipientId { get; set; }
        public DateTime MessageSent { get; set; }
        public string Content { get; set; }
        public MessageForCreationDTO()
        {
            MessageSent = DateTime.Now;
        }
    }
}