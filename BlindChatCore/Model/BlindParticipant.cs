using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace BlindChatCore.Model
{
    public class BlindParticipant
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("GroupId")]
        public Guid? GroupId { get; set; }

        public string PublicKey { get; set; }

        public string Signature { get; set; }

        public string NickName { get; set; }

        public virtual Group Group { get; set; }
    }
}
