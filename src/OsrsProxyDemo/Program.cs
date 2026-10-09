using OsrsProxyDemo;

Console.WriteLine("OSRS equipment preview — Virtual Proxy + Protection Proxy");

var player = new Player("Adventurer", isMember: false);

Console.WriteLine("1. Create two lightweight proxy chains: no models load yet.");
// The lambda describes how to create the model; it does not create it now.
// Protection goes first so denied requests never reach the lazy loader.
IItemModel dragonScimitar = new ProtectionProxy(
    new VirtualProxy(() => new ItemModel("Dragon scimitar")), player);

IItemModel abyssalWhip = new ProtectionProxy(
    new VirtualProxy(() => new ItemModel("Abyssal whip")), player);

Console.WriteLine("\n2. Free player requests the Dragon scimitar: denied, no loading.");
dragonScimitar.Display();

Console.WriteLine("\n3. Grant membership and request it: the model loads and displays.");
player.IsMember = true;
dragonScimitar.Display();

Console.WriteLine("\n4. Request it again: reuse the loaded model.");
dragonScimitar.Display();

Console.WriteLine("\n5. Revoke membership: even the loaded model is protected.");
player.IsMember = false;
dragonScimitar.Display();

// Intentionally never call abyssalWhip.Display(). Its real model never exists.
Console.WriteLine("\nProtection Proxy controls access. Virtual Proxy defers expensive creation.");
