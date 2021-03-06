﻿using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BlindChatCore.Model
{
    public class AuthenticationMessage
    {
        [Key]
        public int Id { get; set; }

        public string Message { get; set; }
        
        public bool? IsSigned { get; set; }

        public string Signature { get; set; }

        [ForeignKey("ParticipantId")]
        public int? ParticipantId { get; set; }

        [ForeignKey("GroupId")]
        public Guid? GroupId { get; set; }

        public virtual Participant Participant { get; set; }
        public virtual Group Group { get; set; }
    }
}
