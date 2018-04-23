using BlindindScheme;
using Org.BouncyCastle.Crypto.Parameters;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace BlindChatCore.Model
{
    public class Group
    {
        [Key]
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string OwnerEmail { get; set; }

        private string PublicKey { get; set; }

        [NotMapped]
        public RsaKeyParameters RsaPublicKey
        {
            get
            {
                return RsaKeyUtils.GetDeserializedKPublicKey(PublicKey);
            }
            set
            {
                PublicKey = RsaKeyUtils.GetSerializedPublicKey(value);
            }
        }
    }
}
