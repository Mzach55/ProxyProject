namespace OsrsProxyDemo;

public sealed class Player
{
    public string Name { get; }
    public bool IsMember { get; set; }

    public Player(string name, bool isMember)
    {
        ArgumentNullException.ThrowIfNull(name);
        Name = name;
        IsMember = isMember;
    }
}
