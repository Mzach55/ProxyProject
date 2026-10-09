namespace OsrsProxyDemo;

public sealed class ProtectionProxy : IItemModel
{
    private readonly IItemModel _subject;
    private readonly Player _player;

    public ProtectionProxy(IItemModel subject, Player player)
    {
        ArgumentNullException.ThrowIfNull(subject);
        ArgumentNullException.ThrowIfNull(player);
        _subject = subject;
        _player = player;
    }

    public void Display()
    {
        // Recheck access on every call, even if the virtual proxy has loaded a model.
        if (!_player.IsMember)
        {
            Console.WriteLine($"[Protection] Access denied for {_player.Name}: this preview requires membership.");
            return;
        }

        Console.WriteLine($"[Protection] Access granted for {_player.Name}.");
        _subject.Display();
    }
}
