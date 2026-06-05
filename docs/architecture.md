# Architecture

DrunkenBestManEFSM will use a layered architecture with a clear dependency direction. The goal is to keep EFSM game rules explicit, testable, and separate from console input/output and XML loading.

This is not full Clean Architecture, but it follows a similar dependency principle:

- Business and game rules remain in the center.
- Technical details stay outside.
- Console UI and XML loading do not pollute domain logic.

## Dependency Direction

The intended dependency direction is:

```text
Presentation -> Application -> Domain
Infrastructure -> Application abstractions / external technical details
```

Rules:

- Domain must not depend on Application.
- Domain must not depend on Infrastructure.
- Domain must not depend on Presentation.
- Application may depend on Domain.
- Presentation may depend on Application.
- Infrastructure may implement contracts defined by Application.
- Infrastructure must not contain game rules.

## Layers

### Domain

The Domain layer contains EFSM concepts and pure game rules.

It will contain entities and models such as:

- `CharacterStats`
- `GameState`
- `Transition`
- `TransitionResult`

It will contain enums such as:

- `Location`
- `ActionType`
- `TravelMode`
- `GameResult`

It will contain rules such as:

- Victory conditions
- Defeat conditions
- Driving eligibility
- Memory eligibility
- Shopping conditions

It will contain action effects such as:

- Applying electrolytes
- Applying alcohol
- Applying passive turn effects
- Applying vomit effects
- Applying travel costs

It will also contain travel map and route cost definitions.

The Domain layer must not use `Console`, read XML, load files, depend on infrastructure, or require UI or file system access. It must remain testable without the console or external files.

### Application

The Application layer coordinates use cases. It receives commands from Presentation, calls Domain rules and transition resolution, and returns results to Presentation.

It defines contracts needed by the application, such as `ITextProvider`.

The Application layer must not directly print to the console, directly read XML files, contain low-level UI logic, or own pure domain rules.

Example use cases:

- `StartNewGame`
- `GetCurrentGameStatus`
- `GetAvailableActions`
- `GetAvailableDestinations`
- `TravelToDestination`
- `BuyElectrolytes`
- `BuyFuel`
- `BuyAlcohol`
- `PickUpRings`
- `EnterChurch`
- `ProcessTurn`

### Infrastructure

The Infrastructure layer contains technical implementations.

It will load narrative texts from XML and implement `ITextProvider`. It may later contain persistence, configuration loading, random provider implementation, or logging adapters.

Infrastructure must not decide game rules, print UI directly, or contain EFSM transition logic.

### Presentation

The Presentation layer is the console interface.

It displays player stats, displays menus, reads user input, shows errors and narrative messages, and calls Application services.

Presentation must not directly modify `GameState`, directly access Domain transition maps unless explicitly exposed through Application, or read XML directly.

### Program.cs

`Program.cs` is the entry point. It should wire dependencies manually, create services, and start the console menu.

It should stay small.

## Nested Feature Modules

Complex nested features may use subfolders inside each layer while preserving the same dependency direction. Blackjack is the first planned nested feature module and is documented as a nested EFSM inside the main game EFSM.

Root files in each layer should continue representing the main game. Blackjack-specific files may later live under `Blackjack/` subfolders inside `Domain`, `Application`, and `Presentation`. Shared contracts such as `IRandomProvider` and shared result wrappers such as `UseCaseResult<T>` should remain outside Blackjack-specific folders.

See [Nested Blackjack EFSM](blackjack-efsm.md).

## XML Text Strategy

Narrative text will be stored in XML under:

```text
src/DrunkenBestManEFSM/Resources/Texts/
```

Application will depend on an abstraction:

```text
ITextProvider
```

Infrastructure will implement this abstraction using XML.

This prevents hardcoded narrative text from spreading through Domain or Application.

## Testing Strategy

Domain should be easy to test because it does not depend on console, XML, or files.

Tests should focus on:

- Win and loss conditions
- Transition conditions
- Action effects
- Travel costs
- Random event behavior
- Balance simulations

## Repository Folders

Intended structure:

```text
src/DrunkenBestManEFSM/
|-- Domain/
|-- Application/
|-- Infrastructure/
|-- Presentation/
|-- Resources/
|   `-- Texts/
`-- Program.cs

docs/
|-- architecture.md
|-- efsm-model.md
`-- diagrams/

tests/
```

Folders may currently contain `.gitkeep` files until implementation commits add real files.

## Why This Structure Matters

This structure makes the project easier to evolve from a console game to other interfaces. Game rules can be tested without running the console. XML text loading can be replaced without changing domain logic. The EFSM remains explicit and understandable. The repository history will show deliberate architecture instead of accidental structure.

## Diagram

- [Layered architecture](diagrams/layered-architecture.mmd)
- [State ownership](diagrams/state-ownership.mmd)
