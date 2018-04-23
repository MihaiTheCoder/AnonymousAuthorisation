using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace BlindChatCore.Model
{
    public class ConversationMessage
    {
        [Key]
        public int Id { get; set; }

        public string Message { get; set; }

        [ForeignKey("ParticipantId")]
        public int? ParticipantId { get; set; }

        [ForeignKey("GroupId")]
        public Guid? GroupId { get; set; }

        public virtual Participant Participant { get; set; }
        public virtual Group Group { get; set; }
    }
}
