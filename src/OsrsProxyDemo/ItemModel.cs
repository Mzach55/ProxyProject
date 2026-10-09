namespace OsrsProxyDemo;

public sealed class ItemModel : IItemModel
{
    private readonly string _itemName;

    public ItemModel(string itemName)
    {
        ArgumentNullException.ThrowIfNull(itemName);
        _itemName = itemName;

        Console.WriteLine($"[Real model] Loading {_itemName}...");
        // Simulates loading a large model and textures; no game assets are needed.
        Thread.Sleep(500);
    }

    public void Display()
    {
        Console.WriteLine($"[Real model] Displaying {_itemName}.");
    }
}
