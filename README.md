# OSRS Proxy Demonstration

A small C# console app demonstrating **Virtual Proxy** and **Protection Proxy** through an Old School RuneScape equipment preview.

## Run

Requires the .NET 10 SDK. From this folder:

```sh
dotnet run --project src/OsrsProxyDemo
```

The demonstration runs automatically, with a brief simulated loading delay. It does not use external packages, a game client, network requests, or actual 3D assets.

## What to watch for

1. Two proxy chains are created without loading any models.
2. A free player's Dragon scimitar request is denied without loading anything.
3. After membership is granted, the first request loads and displays the model.
4. A second request displays the same model without loading it again.
5. After membership is revoked, another request is denied, even though the model is already loaded.
6. The unused Abyssal whip's model never loads.

You should see **one loading message**, **two display messages**, and **two access denials**.

## How the patterns work

```text
Program → ProtectionProxy → VirtualProxy → ItemModel
            checks access     creates once    displays
```

| Class | Pattern role | Responsibility |
| --- | --- | --- |
| `IItemModel` | Subject interface | Defines the shared `Display()` operation. |
| `ItemModel` | Real subject | Simulates expensive initialization and displays the preview. |
| `VirtualProxy` | Virtual proxy | Defers creation until first use, then delegates to the same model. |
| `ProtectionProxy` | Protection proxy | Checks membership on every call before delegating. |
| `Player` | Access context | Holds the player's name and current membership. |
| `Program` | Client | Composes the objects and calls them through `IItemModel`. |

### Virtual Proxy: when should the real object exist?

`VirtualProxy` stores a factory (`Func<IItemModel>`) and an initially null model reference. A factory is simply a function that creates an object when called:

```csharp
new VirtualProxy(() => new ItemModel("Dragon scimitar"))
```

The lambda is saved, not executed, during proxy construction. On the first `Display()` call, the proxy invokes it and saves the result. Later calls reuse that object. Objects never requested never incur the loading cost.

This is a proxy because callers use the same interface as the real subject, while the stand-in controls its creation and forwards operations. Lazy initialization is the mechanism inside it.

### Protection Proxy: may this caller use the object?

`ProtectionProxy` wraps another `IItemModel` and checks membership before forwarding `Display()`. It knows nothing about loading. The virtual proxy knows nothing about membership.

Putting protection on the outside means a denied request never reaches the virtual proxy. Rechecking every call also prevents reuse of a previously loaded model after membership is revoked.

Both proxies could wrap an `ItemModel` directly, but composing them gives both behaviors through the same interface. Start reading at `Program.cs`, then `ProtectionProxy.cs`, then `VirtualProxy.cs`.

```sh
dotnet build src/OsrsProxyDemo
```
