namespace BlindChatCore
{
    public interface IEmailSender
    {
        void SendConfirmationCodeToGroupCreator(string email, int confirmationCode);
        void SendInvitation(string email, int invitationCode);
    }
}