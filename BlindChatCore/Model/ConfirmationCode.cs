using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BlindChatCore.Model
{
    public class ConfirmationCode
    {
        [Key]
        public int Id { get; set; }

        public int Code { get; set; }

        public bool IsDeleted { get; set; }

        [ForeignKey("GroupId")]
        public Guid? GroupId { get; set; }

        public virtual Group Group { get; set; }
    }
}
