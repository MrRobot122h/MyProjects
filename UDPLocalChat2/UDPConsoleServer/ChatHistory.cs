public class ChatHistory
{
    public string Member1 { get; set; }
    public string Member2 { get; set; }
    public string Chat { get; set; }
    public ChatHistory(string member1, string member2, string chat)
    {
        Member1 = member1;
        Member2 = member2;
        Chat = chat;
    }
    public override bool Equals(object obj)
    {
        if (obj is ChatHistory other)
        {
            return (Member1 == other.Member1 && Member2 == other.Member2) ||
                   (Member1 == other.Member2 && Member2 == other.Member1);
        }
        return false;
    }
}
