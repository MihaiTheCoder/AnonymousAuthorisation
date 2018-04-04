namespace BlindChatCore.Certificate
{
    public interface ISimpleRandomGenerator
    {
        int Next(int min, int max);
    }
}