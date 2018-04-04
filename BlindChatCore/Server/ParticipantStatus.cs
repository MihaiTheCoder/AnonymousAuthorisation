namespace BlindChatCore
{
    public class ParticipantStatus
    {
        public ParticipantStatus(string email, bool wasInvitedNow)
        {
            Email = email;
            WasInvitedNow = wasInvitedNow;
        }
        public string Email { get; set; }

        public bool WasInvitedNow { get; set; }
    }
}