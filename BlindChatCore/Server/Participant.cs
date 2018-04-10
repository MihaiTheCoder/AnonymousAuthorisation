using System;

namespace BlindChatCore
{
    public class Participant
    {
        public int ID { get; set; }

        public string Email { get; set; }

        public Guid GroupId { get; set; }
    }
}