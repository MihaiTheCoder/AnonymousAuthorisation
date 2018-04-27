using Microsoft.EntityFrameworkCore;
using BlindChatCore;

namespace BlindChat.Infrastructure.Repository
{
    public class BlindChatDbContext: DbContext
    {
        public DbSet<BlindChatCore.Model.Group> Groups { get; set; }

        public DbSet<BlindChatCore.Model.Participant> Participants { get; set; }

        public DbSet<BlindChatCore.Model.ConfirmationCode> ConfirmationCodes { get; set; }

        public DbSet<BlindChatCore.Model.AuthenticationMessage> AuthenticationMessages { get; set; }

        public DbSet<BlindChatCore.Model.ConversationMessage> ConversationMessages { get; set; }

        public DbSet<BlindChatCore.Model.BlindParticipant> BlindParticipants { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(
                "Server=(localdb)\\mssqllocaldb;Database=BlindChat;Trusted_Connection=True;");
        }
    }
}
