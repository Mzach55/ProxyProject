# OSRS Proxies Implementation Plan

> Execution complete. The user subsequently requested no automated tests; test creation and test-run steps below are superseded by direct build and console verification. See [execution record](progress.md). The original checklist is retained as the approved plan, not a completion checklist.

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox syntax for tracking.

**Goal:** Build a small, readable C# console demonstration of virtual and protection proxies with OSRS equipment.

**Architecture:** `Program` calls a protection proxy, which wraps a virtual proxy, which lazily creates the real model. All implement `IItemModel`; membership is checked on every request and successful initialization is reused.

**Tech Stack:** C#, .NET 10 console projects, dependency-free console tests.

**Spec:** [Approved design](../specs/2026-10-06-osrs-proxies-design.md)

## Global Constraints

- Use the installed .NET 10 SDK.
- No GUI, external packages, network calls, game assets, or OSRS account integration are needed.
- This single-threaded console demo does not implement concurrent initialization.
- The membership restriction is a rule of this demonstration's preview app, not a claim about viewing equipment in OSRS.
- Use an explicit null check for lazy creation and counting test doubles for verification.
- The workspace has no Git repository; do not require commits or a worktree to run the project.

## Review Focus

- Null constructor dependencies must fail immediately (Task 1).
- A model factory that throws must leave initialization retryable (Task 1).
- A factory returning null must fail clearly without caching an invalid subject (Task 1).
- Revoked membership must prevent delegation even after initialization (Task 2).
- Multiple proxies must keep independent lazy state (Task 2).

## Task 1: Virtual proxy and real model

**Files:** Create `src/OsrsProxyDemo/{OsrsProxyDemo.csproj,IItemModel.cs,ItemModel.cs,VirtualProxy.cs,Program.cs}`, `tests/OsrsProxyDemo.Tests/{OsrsProxyDemo.Tests.csproj,Program.cs}`, and `.gitignore`.

**Interfaces:** Namespace `OsrsProxyDemo`; `IItemModel.Display(): void`; `ItemModel(string itemName)`; `VirtualProxy(Func<IItemModel> modelFactory)`. Tests reference the application project. Both target `net10.0` with nullable references and implicit usings enabled. Initial application entry point is empty until Task 2.

- [ ] Create project definitions, interface, minimal compilable class stubs, and the console test runner. The runner prints each result, catches assertion failures, and returns nonzero on failure. Ignore `bin/` and `obj/`.
- [ ] Write counting-double tests with these assertions: `ConstructionIsLazy` checks `factoryCalls == 0`; `RepeatedDisplayReusesSubject` checks `factoryCalls == 1` and `displayCalls == 2`; `NullDependenciesRejected` checks `ArgumentNullException` for null factory and item name; `FailedCreationCanRetry` checks the original exception propagates, the second request succeeds, and `factoryCalls == 2`; `NullFactoryResultCanRetry` checks `InvalidOperationException` for a null result and success on the next valid result.
- [ ] Run `dotnet run --project tests/OsrsProxyDemo.Tests`; confirm the relevant behavior assertions fail before implementation.
- [ ] Implement `VirtualProxy.Display()` using an initially null subject field, creating and storing a valid subject only after the factory succeeds, then delegating. Implement `ItemModel` with a constructor loading message, a 500 ms simulated delay, and `Display()` printing the item name. Guard null dependencies.
- [ ] Run `dotnet run --project tests/OsrsProxyDemo.Tests`; expect every Task 1 test to pass with exit code zero.

## Task 2: Protection proxy, composition, and teaching materials

**Files:** Create `src/OsrsProxyDemo/{Player.cs,ProtectionProxy.cs}` and `README.md`. Update both `Program.cs` files.

**Interfaces:** `Player(string name, bool isMember)` exposes read-only `string Name` and mutable `bool IsMember`. `ProtectionProxy(IItemModel subject, Player player)` implements `IItemModel`. It consumes Task 1's interface and always requires membership for its wrapped preview.

- [ ] Add compilable stubs and tests: `DeniedRequestDoesNotDelegateOrLoad` checks both counts stay zero; `AllowedRequestDelegates` checks one load and one display; `MembershipRevocationBlocksCachedModel` checks display count stays one after revocation; `UnusedProxyRemainsUnloaded` displays the first of two proxies and checks the second factory count stays zero; `ProtectionDependenciesRejected` checks `ArgumentNullException` for null subject, player, or player name.
- [ ] Run `dotnet run --project tests/OsrsProxyDemo.Tests`; expect new behavior tests to fail while Task 1 tests still pass.
- [ ] Implement the player and protection proxy: guard constructor dependencies, inspect membership on every call, print denial and return when unauthorized, otherwise print access granted and delegate.
- [ ] Implement the labeled console script using `IItemModel` references: create two protected virtual proxies, deny the free player's Dragon scimitar request, grant membership and display it twice, revoke membership and deny again, leave the Abyssal whip unused. Explain that membership changes and model loading are simulated.
- [ ] Write README instructions using `dotnet run --project src/OsrsProxyDemo` and `dotnet run --project tests/OsrsProxyDemo.Tests`. Include the request-chain diagram, mapping of classes to pattern roles, expected sequence, and why denied requests cannot trigger loading. Explain that protection controls access while the virtual proxy controls expensive creation.
- [ ] Run `dotnet build src/OsrsProxyDemo` and `dotnet run --project tests/OsrsProxyDemo.Tests`; expect a successful build and all tests passing.
- [ ] Run `dotnet run --project src/OsrsProxyDemo`; confirm one Dragon scimitar load, two previews, denials before membership and after revocation, and no Abyssal whip load. Review source and README against the approved spec before reporting completion.
