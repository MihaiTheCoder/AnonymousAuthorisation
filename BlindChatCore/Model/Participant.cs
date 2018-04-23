using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BlindChatCore.Model
{
    public class Participant
    {
        [Key]
        public int Id { get; set; }

        public string Email { get; set; }

        public bool EmailIsAlreadyInvited { get; set; }

        public bool EmailIsConfirmed { get; set; }

        public int InvitationCode { get; set; } 
        
        public string PublicKey { get; set; }

        public string Signature { get; set; }

        [ForeignKey("GroupId")]
        public Guid? GroupId { get; set; }

        public virtual Group Group { get; set; }

    }
}
