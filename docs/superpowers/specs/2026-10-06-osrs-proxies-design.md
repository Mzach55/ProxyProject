# OSRS equipment preview: virtual and protection proxies

> Implementation note: the user subsequently requested no automated tests. The test project and automated verification requirements below were superseded by build and console verification; the application design is unchanged.

## Purpose and scope

Build a small C# console application that clearly demonstrates both the Virtual Proxy and Protection Proxy patterns. Use Old School RuneScape equipment names for familiarity. Prioritize readable code and observable behavior over features.

Use the installed .NET 10 SDK. No GUI, external packages, network calls, game assets, or OSRS account integration are needed. The membership restriction is a rule of this demonstration's preview app, not a claim about viewing equipment in OSRS.

## Architecture

Every model and proxy implements `IItemModel`, which exposes `void Display()`.

The normal request path is:

```text
Program -> ProtectionProxy -> VirtualProxy -> ItemModel
           checks access      creates once    displays
```

- `ItemModel` is the real subject. Its constructor prints a loading message and simulates expensive model loading with a short delay. `Display()` prints the equipment preview. The console output represents a model; no actual graphics are rendered.
- `VirtualProxy` is the lightweight substitute. It stores a factory for creating an `IItemModel` and an initially absent model reference. Its first `Display()` call creates the real subject and delegates to it. Later calls delegate to the same subject. Construction of the proxy never invokes the factory. The factory makes deferred creation straightforward to verify without delays.
- `ProtectionProxy` wraps an `IItemModel` and checks the current demo player's membership on every `Display()` call. A denied request prints an explanation and returns without calling the wrapped subject. An allowed request delegates. It must wrap the virtual proxy so denied requests do not cause expensive initialization.
- `Player` contains a name and membership status. The scripted demonstration can change membership status to show authorization being checked again even after a model has loaded.
- `Program` composes these objects and runs a deterministic, labeled demonstration. Clients invoke `Display()` through `IItemModel`.

Each class has one responsibility. The protection proxy knows nothing about loading; the virtual proxy knows nothing about membership. An explicit null check in the virtual proxy keeps the lazy creation mechanism visible to a learner. This single-threaded console demo does not implement concurrent initialization.

## Demonstration

Create protected virtual proxies for the Dragon scimitar and Abyssal whip. Both require membership under the demo's preview rules. Start with a free player.

1. Construct both proxy chains. No loading messages appear.
2. Request the Dragon scimitar as a free player. Access is denied and no model loads.
3. Give the demo player membership and request the Dragon scimitar. Access is granted, the real model loads once, and the preview displays.
4. Request the Dragon scimitar again. Access is checked again and the same model displays without another load.
5. Remove membership and request it again. Access is denied even though the real model already exists.
6. Leave the Abyssal whip unused. Its real model is never constructed.

Explain the simulated membership changes in the output. This is an educational access-control example, not a production authentication system.

## Error handling

Access denial is an expected outcome and does not throw. Reject null constructor dependencies. If model creation fails, propagate the exception without storing a partially created model; a later request may attempt creation again. No retry UI or external failure simulation is needed.

## Files and documentation

- A .NET console project under `src/OsrsProxyDemo`, with one small file per interface/class.
- A dependency-free console verification project under `tests/OsrsProxyDemo.Tests`, referencing the demo project and using counting test doubles.
- A root `README.md` with run commands, the request-chain diagram, pattern-role mapping, expected demonstration behavior, and a brief explanation of why lazy initialization and access control are separate responsibilities.
- A root `.gitignore` excluding .NET build output.

## Verification and acceptance criteria

Build and run the application. Confirm its labeled output illustrates every demonstration step. Run automated checks that fail with a nonzero exit code when an assertion fails:

- Constructing a virtual proxy invokes its factory zero times.
- Displaying it twice invokes its factory once and the real subject twice.
- A denied protected request never reaches the wrapped subject or invokes the real-model factory.
- An allowed protected request reaches the wrapped subject.
- Removing membership after a successful request prevents subsequent delegation.
- An unused proxy never creates its real subject.

Use test doubles so verification does not depend on elapsed time or exact console wording. Successful completion requires a passing build, passing checks, and observed demonstration output.
