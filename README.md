# Bug Colony Game

A **Unity colony simulation** featuring autonomous bug AI with emergent population dynamics, built with clean architecture patterns: **VContainer DI**, a custom **MonoContainer reactive system**, **State/Strategy patterns**, and **Object Pooling**.

> **Live WebGL build:** open `index.html` in the repository root.

---

## Table of Contents

1. [Project Structure](#project-structure)
2. [Dependency Injection (VContainer)](#dependency-injection-vcontainer)
3. [Reactive Programming — MonoContainer](#reactive-programming--monocontainer)
4. [UI System & UiController](#ui-system--uicontroller)
5. [Configuration Injection](#configuration-injection)
6. [Design Patterns](#design-patterns)
7. [Gameplay — Worker Bug](#gameplay--worker-bug)
8. [Gameplay — Hunter (Predator) Bug](#gameplay--hunter-predator-bug)
9. [Difficulty & Balance](#difficulty--balance)

---

## Project Structure

```
BugColony.Unity/Assets/Scripts/
├── Bugs/                        # Bug entities
│   ├── BugBase.cs               # Abstract base (health, energy, state machine, behavior)
│   ├── BugStateMachine.cs       # Finite-state machine driver
│   ├── WorkerBug.cs             # Worker: gathers resources, splits
│   ├── PredatorBug.cs           # Predator (hunter): hunts, 10 s lifespan
│   ├── Behaviors/               # Strategy pattern — AI brains
│   │   ├── BugBehaviorBase.cs   # Abstract behavior
│   │   ├── WorkerBehavior.cs    # Flee → Seek resources → Idle
│   │   └── PredatorBehavior.cs  # Melee attack → Chase prey → Idle
│   └── States/                  # State pattern — atomic actions
│       ├── IdleState.cs
│       ├── MoveState.cs         # Random wandering
│       ├── ChaseState.cs        # Pursue a target
│       ├── AttackState.cs       # Melee combat
│       ├── EatState.cs          # Consume a resource
│       ├── FleeState.cs         # Run from a predator
│       └── DeadState.cs         # Terminal state
├── Configs/                     # ScriptableObject configs
│   ├── BugsConfig.cs
│   ├── ColonyConfig.cs
│   ├── ResourceSpawnerConfig.cs
│   └── GameBootstrapConfig.cs
├── Core/                        # Interfaces, DI, entry point
│   ├── GameLifetimeScope.cs     # VContainer composition root
│   ├── GameBootstrap.cs         # IStartable / ITickable entry point
│   ├── IBug.cs, IBugBehavior.cs, IBugState.cs, IResource.cs, ISpawnable.cs
│   └── Resource.cs
├── Factory/
│   ├── BugFactory.cs            # Creates / recycles bugs via pool lookup
│   └── ResourceFactory.cs
├── MonoContainer/
│   └── MonoContainer.cs         # Lightweight reactive value containers
├── Pool/
│   ├── ObjectPool.cs            # Generic pool with deferred DI injection
│   ├── BugPool.cs
│   └── ResourcePool.cs
├── Systems/
│   ├── ColonyManager.cs         # Population tracking, splitting, mutations
│   ├── ResourceSpawner.cs       # Timer-based resource spawning
│   ├── LifetimeSystem.cs        # Dead-bug cleanup scheduler
│   └── MutationSystem.cs        # Mutation logic (placeholder)
├── Tools/
│   ├── CameraController.cs
│   ├── FieldMeasurer.cs
│   └── TimeScaler.cs
└── UI/
    ├── UiController.cs          # Pooled UI fields, dynamic container binding
    └── UIManager.cs             # Subscribes MonoContainers → UI
```

Namespaces mirror the folder layout: `BugColony.Core`, `BugColony.Bugs`, `BugColony.Factory`, `BugColony.Pool`, `BugColony.Systems`, `BugColony.UI`, `MonoContainer`.

---

## Dependency Injection (VContainer)

The project uses **[VContainer](https://github.com/hadashiA/VContainer)** — a lightweight, high-performance DI framework for Unity.

### Composition Root — `GameLifetimeScope`

`GameLifetimeScope` extends `LifetimeScope` and is the single place where the entire object graph is wired:

```
GameLifetimeScope.Configure(IContainerBuilder builder)
  ├─ Object Pools   (BugPool × 2, ResourcePool)
  ├─ Factories      (BugFactory, ResourceFactory)
  ├─ Configs        (BugsConfig, ColonyConfig, ResourceSpawnerConfig, GameBootstrapConfig)
  ├─ Systems        (ColonyManager, ResourceSpawner, MutationSystem, LifetimeSystem)
  ├─ UI             (UiController instance, UIManager)
  └─ Entry Point    (GameBootstrap as IStartable + ITickable)
```

### Deferred Injection Pattern

Object pools **pre-warm** instances before the DI container is built. This creates a chicken-and-egg problem: pooled `MonoBehaviour` instances need injected dependencies, but the container does not exist yet.

The solution is a **`DeferredInjector`** inner class:

```csharp
private class DeferredInjector
{
    private IObjectResolver _resolver;
    public void SetResolver(IObjectResolver resolver) => _resolver = resolver;
    public void Inject(object instance) => _resolver?.Inject(instance);
}
```

1. A `DeferredInjector` is created **before** the container builds.
2. Each pool receives it as an `onCreated` callback — every new instance is passed through `injector.Inject(instance)`.
3. During pre-warm the resolver is `null`, so injection is a no-op.
4. After the container builds, `RegisterBuildCallback` sets the resolver and calls `pool.InjectAll(container)` to retroactively inject all pre-warmed instances.
5. Any instances created at **runtime** (pool growth) are automatically injected because the resolver is now set.

```csharp
builder.RegisterBuildCallback(container =>
{
    injector.SetResolver(container);
    workerPool.InjectAll(container);   // retroactive injection
    predatorPool.InjectAll(container);
});
```

### Entry Point

`GameBootstrap` is registered via `builder.RegisterEntryPoint<GameBootstrap>()`.  
VContainer calls `IStartable.Start()` once after the container builds (spawns the initial colony) and `ITickable.Tick()` every frame (drives systems).

---

## Reactive Programming — MonoContainer

Instead of pulling in a heavy reactive library (UniRx / R3), the project implements a **minimal, custom reactive container** system in `MonoContainer/MonoContainer.cs`.

### How It Works

```csharp
public abstract class MonoContainer<T>
{
    private T _value;
    public T Value
    {
        get => _value;
        set { _value = value; OnValueChanged?.Invoke(_value); }
    }
    public event Action<T> OnValueChanged;
    public abstract T DefaultValue();
}
```

Concrete types: `IntContainer`, `FloatContainer`, `StringContainer`.

Every **write** to `.Value` automatically fires `OnValueChanged`, pushing the new value to all subscribers. This gives a **reactive, push-based data flow** without any polling or manual UI refresh calls.

### Data Flow

```
ColonyManager (domain logic)
  │
  │  TotalAlive.Value++          ← write triggers event
  │
  ▼
MonoContainer.OnValueChanged     ← event fires
  │
  ▼
UIManager (subscriber)           ← updates UiController text field
```

`ColonyManager` exposes public `IntContainer` fields:

| Container | Tracks |
|-----------|--------|
| `TotalAlive` | Number of living bugs |
| `TotalDead` | Number of dead bugs |
| `DeadWorkers` | Cumulative worker deaths |
| `DeadPredators` | Cumulative predator deaths |

`UIManager` subscribes in its constructor and pushes formatted strings to `UiController`:

```csharp
_colonyManager.TotalAlive.OnValueChanged += val =>
    _ui.GetFreeOrNewField("TotalAliveText").text = $"Total Alive: {val}";
```

---

## UI System & UiController

### Dynamic Container Binding

`UiController` is a **pooled UI field manager** that allows any system to dynamically request a UI text field by a string key (the "container name"). This design decouples the UI layout from the data model — you do not need to pre-assign UI elements in the Inspector for every metric.

**Key features:**

| Method | Purpose |
|--------|---------|
| `GetFreeOrNewField(string key)` | Returns (or creates) a `TextMeshProUGUI` field mapped to `key`. Reuses the same field on repeated calls with the same key. |
| `ReleaseField(string key)` | Returns the field to the pool and removes the mapping. |
| `ReleaseAll()` | Returns all active fields to the pool. |

Internally, `UiController` maintains:

- A **`Stack<TextMeshProUGUI>`** of free (pooled) fields.
- A **`Dictionary<string, TextMeshProUGUI>`** mapping container keys → active fields.
- A reverse **`Dictionary<TextMeshProUGUI, string>`** for clean-up.

When a new container key is requested and no free field exists, `UiController` instantiates a new field from a prefab. Released fields are deactivated, re-parented, and pushed back into the pool for reuse.

### How It Connects

`UiController` is registered as an **instance** in VContainer (`builder.RegisterInstance<UiController>(uiPrefab)`), and injected into `UIManager`. Any system that needs to display data simply calls:

```csharp
_ui.GetFreeOrNewField("MyMetric").text = "value";
```

This makes it trivial to add new on-screen metrics without touching prefabs or scene hierarchy — just pick a unique key string and write to the returned field.

---

## Configuration Injection

All tunable game parameters live in **ScriptableObject** assets. This allows designers to tweak values in the Unity Inspector without recompiling, and makes it easy to swap config profiles.

| Config Class | Key Fields | Consumed By |
|---|---|---|
| **`GameBootstrapConfig`** | `InitialWorkerCount` (5), `InitialPredatorCount` (2), `InitialSpawnRangeMin/Max` (−10…10) | `GameBootstrap` |
| **`BugsConfig`** | `WorkerLifetime` (30 s), `PredatorLifetime` (20 s), `DeadBugCleanupDelay` (5 s), `MutationChance` (10 %) | `LifetimeSystem` |
| **`ColonyConfig`** | `MutationColonyThreshold` (10), `MutationChance` (10 %), `SplitSpawnRadius` (1.5 u) | `ColonyManager` |
| **`ResourceSpawnerConfig`** | `SpawnInterval` (2 s), `SpawnAreaMin/Max` (−20…20) | `ResourceSpawner` |

### Registration

Configs are serialized references on `GameLifetimeScope` and registered as singleton instances:

```csharp
builder.RegisterInstance<BugsConfig>(bugsConfig);
builder.RegisterInstance<ColonyConfig>(colonyConfig);
builder.RegisterInstance<ResourceSpawnerConfig>(resourceSpawnerConfig);
builder.RegisterInstance<GameBootstrapConfig>(gameBootstrapConfig);
```

VContainer resolves them via constructor injection into the systems that need them. No service locator, no static access — pure constructor DI.

---

## Design Patterns

### State Pattern — `BugStateMachine`

Each bug has its own `BugStateMachine` that holds the current `IBugState`. States implement `Enter`, `Execute`, and `Exit`. Transitions are triggered by behaviors or by the states themselves.

```
IdleState ──(timer)──▶ MoveState ──(timer)──▶ IdleState
     │                                            │
     ├──(predator nearby)──▶ FleeState ──────────┘
     ├──(resource spotted)──▶ ChaseState ──▶ EatState ──┘
     └──(prey spotted)──────▶ ChaseState ──▶ AttackState ─┘
                                              │
                                        (target dies) ──▶ IdleState
```

### Strategy Pattern — `IBugBehavior`

Each bug type has a dedicated **behavior strategy** (`WorkerBehavior`, `PredatorBehavior`) set during `Awake()`. The behavior runs every frame via `_behavior.Execute(bug)` and decides which state to transition into based on physics queries (`SphereCastNonAlloc`, `OverlapSphereNonAlloc`).

### Factory + Object Pool

`BugFactory` maps `BugType` → `BugPool`. Calling `Create()` pulls from the pool; `Recycle()` returns. `ObjectPool<T>` is generic, supports pre-warming, and integrates with VContainer's deferred injection.

---

## Gameplay — Worker Bug

The **Worker Bug** is the colony's gatherer and primary population driver.

### Behavior Loop

1. **Idle** — waits for a random short duration.
2. **Move** — wanders in a random direction for ~3 seconds.
3. **Detect predator** (5 m radius) → **Flee** — runs in the opposite direction for up to 5 seconds.
4. **Spot resource** (8 m forward SphereCast) → **Chase** → **Eat** — gains +20 energy.

### Splitting (Reproduction)

After eating **2 resources**, a worker splits:

- The parent is despawned (recycled to pool).
- **2 offspring** spawn at the parent's position (±1.5 u random offset).
- If the colony has **more than 10 bugs**, each offspring has a **10 % chance to mutate into a Predator** instead of a Worker.

### Survival

- Energy decays at **1 /s** (100 max → ~100 s to starvation if unfed).
- Health is **100**; a single predator attack deals **100 damage** (instant kill).
- Workers live indefinitely as long as they keep eating and avoid predators.

---

## Gameplay — Hunter (Predator) Bug

The **Predator Bug** (hunter) is the colony's apex predator — fast, aggressive, and short-lived.

### Behavior Loop

1. **Melee scan** (1.2 m `OverlapSphere`) — if any live bug or resource is in range → **Attack** / **Eat**.
2. **Vision scan** (15 m forward SphereCast, 2 m radius) — spot any live bug or resource → **Chase**.
3. If nothing is found → **Idle** / **Move** (random wandering).

### Key Mechanics

- **Cannibalistic** — predators attack workers, resources, **and other predators**.
- **Timed lifespan** — each predator dies after exactly **10 seconds**, regardless of energy.
- **Split after 3 kills/eats** — the parent is despawned; 2 new predators spawn with fresh 10 s timers.
- **Attack damage: 100** — one-hit kill against workers (100 HP).

### Lifecycle

```
Spawn (10 s timer starts)
  ├─ Hunt/eat 3 targets → Split into 2 new predators (parent dies)
  └─ Timer expires       → Die (recycled to pool after 5 s cleanup)
```

---

## Difficulty & Balance

The game creates **emergent difficulty** through the interaction of population dynamics, resource availability, and mutation mechanics.

### Balance Levers

| Parameter | Value | Effect |
|---|---|---|
| Resource spawn rate | 1 every **2 s** | Controls food supply; slower = harder for workers |
| Worker split threshold | **2** resources eaten | Lower = faster worker growth |
| Predator split threshold | **3** kills / eats | Lower = faster predator growth |
| Predator lifespan | **10 s** | Shorter = less threat; longer = predators dominate |
| Mutation colony threshold | **10** bugs alive | Predators only emerge from mutations when colony is large |
| Mutation chance | **10 %** per offspring | Higher = more predators emerge |
| Worker detection range | **5 m** (predator) / **8 m** (resource) | Tuning determines flee success rate |
| Predator vision range | **15 m** | Longer range = harder for workers to escape |
| Energy decay | **1 /s** (100 max) | Faster decay forces more frequent foraging |
| Attack damage | **100** (instant kill) | One-hit mechanics keep encounters decisive |

### Emergent Dynamics

- **Early game (< 10 bugs):** Only workers exist. Colony grows by gathering resources and splitting. No predator mutations yet.
- **Mid game (> 10 bugs):** Mutations kick in. Worker splits occasionally produce predators, introducing natural population control.
- **Predator cascade:** If predators hunt successfully, they split and create more predators — leading to explosive but short-lived predator booms (10 s lifespan acts as a natural limiter).
- **Colony wipeout safety net:** If all bugs die, a single **rescue worker** spawns at the origin to restart the cycle.
- **Self-balancing feedback loop:** More workers → more food competition → more mutations → more predators → fewer workers → fewer mutations → predators die out → workers recover.

### Spawn Area

- **Initial colony**: 5 workers + 2 predators within (−10, 10) range.
- **Resource field**: random positions within (−20, 20) on X and Z axes.

---

## Tech Stack

- **Unity** (3D, C#)
- **[VContainer](https://github.com/hadashiA/VContainer)** — Dependency Injection
- **TextMeshPro** — UI text rendering
- **Custom MonoContainer** — Lightweight reactive value containers