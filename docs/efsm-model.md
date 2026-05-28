# EFSM Model

## Game Concept

DrunkenBestManEFSM is a C# console game based on an Extended Finite State Machine. A drunken best man wakes up in a Strip Club with a severe hangover. He must deliver the wedding rings to the correct church before time runs out.

His phone is dead, he does not remember which church is correct, and he must manage his physical condition, money, fuel, and car location while trying to survive the trip.

The correct church is selected randomly during gameplay from five possible churches:

- LostLoveParish
- ForbiddenRoseChapel
- LastGoodbyeSanctuary
- SecretsOfTheSoulChurch
- FinalDestinyCathedral

The player starts at `StripClub`. The car also starts at `StripClub`. The player has limited fuel, enough to drive to the `GasStation`. The `StripClub` can also provide a costly recovery option that restores a random amount of health after the player spends some time enjoying a service offered by the venue. The player must eventually go to the `GasStation` to buy electrolytes and fuel, go to the `JewelryStore` to pick up the rings, identify the correct church, and reach it before losing by time, health, or dehydration.

## EFSM Definition

This game is not a simple finite state machine because transitions do not depend only on the current location and selected action. They also depend on extended game data such as health, hangover, drunkenness, fuel, money, remaining time, car location, rings, and memory.

The transition notation is:

```text
delta(CurrentLocation, Action, GameState) [Condition] / Effect -> NewLocation, UpdatedGameState
```

Each part means:

- `CurrentLocation`: where the player currently is.
- `Action`: what the player attempts to do.
- `GameState`: extended state containing stats, resources, flags, and random values.
- `Condition`: rule that decides whether the action is allowed.
- `Effect`: stat, resource, or state changes caused by the action.
- `NewLocation`: the resulting location after the transition.
- `UpdatedGameState`: the modified extended state after the transition.

## Extended State

The extended state contains the values that make the model an EFSM:

- `CurrentLocation`
- `CarLocation`
- `Health`
- `Hangover`
- `Drunkenness`
- `Fuel`
- `RemainingTime`
- `Money`
- `HasRings`
- `CorrectChurchKnown`
- `CorrectChurch`
- `MemoryThreshold`

Initial design values:

| Variable | Initial Value |
| --- | --- |
| `CurrentLocation` | `StripClub` |
| `CarLocation` | `StripClub` |
| `Health` | `30 / 100` |
| `Hangover` | `80 / 100` |
| `Drunkenness` | `45 / 100` |
| `Fuel` | `15 / 100` |
| `RemainingTime` | `60 minutes` |
| `Money` | `120` |
| `HasRings` | `false` |
| `CorrectChurchKnown` | `false` |
| `CorrectChurch` | Random church |
| `MemoryThreshold` | Random value |

## Locations

Game locations:

- StripClub
- GasStation
- JewelryStore
- Bar
- LostLoveParish
- ForbiddenRoseChapel
- LastGoodbyeSanctuary
- SecretsOfTheSoulChurch
- FinalDestinyCathedral
- Victory
- Defeat

Church locations:

- LostLoveParish
- ForbiddenRoseChapel
- LastGoodbyeSanctuary
- SecretsOfTheSoulChurch
- FinalDestinyCathedral

The correct church is randomly selected from the church list.

## Hangover and Drunkenness

`Hangover` and `Drunkenness` are separate variables.

`Hangover` represents dehydration, exhaustion, and physical deterioration. It increases passively as turns pass. It can be reduced with electrolytes. If it reaches `100`, the player loses by dehydration.

`Drunkenness` represents active alcohol intoxication. It decreases passively as turns pass and increases when buying alcohol. It affects whether the player can drive and increases the probability of vomiting. It also prevents alcohol abuse from becoming a safe strategy for gaining time.

Drunkenness does not directly cause defeat, but it increases risk by blocking driving, increasing vomit probability, making alcohol abuse dangerous, and indirectly causing loss through time, health, or poor mobility.

## Strategic Pressures

The player must manage:

- Time
- Health
- Hangover
- Drunkenness
- Fuel
- Money
- Car location
- Rings
- Memory of the correct church

The game is mainly about balancing time, sobriety, and health.

## Action Categories

Travel actions:

- `WalkTo(destination)`
- `DriveTo(destination)`

Location actions:

- `BuyElectrolytes`
- `BuyFuel`
- `BuyAlcohol`
- `RestAtStripClub`
- `PickUpRings`
- `EnterChurch`
- `CheckStats`

Random events:

- `Vomit`
- `FindMoney`
- `NoEvent`

Passive effects per turn:

- Hangover increases.
- Drunkenness decreases.

## Conceptual Mechanics

Electrolytes cost money, reduce hangover, slightly reduce drunkenness, provide only limited health recovery, and consume a small amount of time. Their main purpose is real recovery from dehydration and exhaustion, not full healing.

The Strip Club recovery action costs money, consumes time, and restores a random amount of health within a configured range. This gives the starting location a strategic purpose and separates health recovery from electrolyte recovery. The random health gain should be selected outside the Domain layer, then passed into the domain effect so the rule/effect logic remains testable.

Alcohol costs money, reduces hangover slightly, increases drunkenness, reduces health, and increases remaining time up to a defined cap. This represents the character feeling artificially functional, not literal time travel. This mechanic is risky because drunkenness increases vomit probability and may block driving.

Vomit is a random event that becomes more likely when drunkenness is high. It reduces health, increases hangover because it worsens dehydration, reduces drunkenness, consumes time, and can cause defeat if health reaches `0`.

Walking does not consume fuel, consumes more time, leaves the car behind, has a higher chance to find money, and may worsen hangover slightly because of physical exhaustion.

Driving consumes fuel, consumes less time, requires the car to be at the current location, requires enough fuel, requires drunkenness below the driving limit, moves the car to the destination, and has a lower chance to find money.

## Car Location

`CarLocation` is part of the extended state. If the player walks, `CurrentLocation` changes but `CarLocation` does not. If the player drives, both `CurrentLocation` and `CarLocation` move to the destination.

This is one of the clearest examples of why the game is an EFSM. The legality and result of a travel action depend on more than the player's current location. They also depend on where the car is, whether fuel is available, and whether the player is sober enough to drive.

## Memory

The player does not know the correct church at the start. The correct church can be remembered only when the character is healthy enough mentally.

Current rule:

```text
CorrectChurchKnown becomes true when:
Hangover <= MemoryThreshold
&& Drunkenness < SobrietyRequirement
```

## Victory and Defeat

The player wins if:

- `CurrentLocation == CorrectChurch`
- `HasRings == true`
- `RemainingTime > 0`
- `Health > 0`
- `Hangover < 100`

The player loses if:

- `RemainingTime <= 0`
- `Health <= 0`
- `Hangover >= 100`

## Transition Examples

### Example 1: Driving from StripClub to GasStation

```text
delta(StripClub, DriveTo(GasStation), GameState)
[
    CarLocation == StripClub
    && Fuel >= RequiredFuel
    && Drunkenness < DrivingLimit
]
/
    Fuel -= RequiredFuel
    RemainingTime -= TravelTime
    CarLocation = GasStation
    ApplyPassiveTurnEffects()
    RollRandomEvent()
-> GasStation, UpdatedGameState
```

### Example 2: Buying electrolytes at GasStation

```text
delta(GasStation, BuyElectrolytes, GameState)
[
    Money >= ElectrolyteCost
]
/
    Money -= ElectrolyteCost
    Health += HealthGain
    Hangover -= HangoverReduction
    Drunkenness -= DrunkennessReduction
    RemainingTime -= PurchaseTime
    ApplyPassiveTurnEffects()
-> GasStation, UpdatedGameState
```

### Example 3: Buying alcohol at Bar

```text
delta(Bar, BuyAlcohol, GameState)
[
    Money >= AlcoholCost
    && Health > AlcoholHealthCost
    && RemainingTime < MaxRemainingTime
]
/
    Money -= AlcoholCost
    Health -= AlcoholHealthCost
    Hangover -= HangoverReduction
    Drunkenness += DrunkennessIncrease
    RemainingTime += TimeGain
    ApplyPassiveTurnEffects()
-> Bar, UpdatedGameState
```

### Example 4: Recovering at the Strip Club

```text
delta(StripClub, RestAtStripClub, GameState)
[
    Money >= StripClubServiceCost
    && Health < MaxHealth
    && RemainingTime > StripClubServiceTimeCost
]
/
    Money -= StripClubServiceCost
    Health += RandomHealthGain
    RemainingTime -= StripClubServiceTimeCost
    ApplyPassiveTurnEffects()
-> StripClub, UpdatedGameState
```

### Example 5: Entering the correct church with rings

```text
delta(CorrectChurch, EnterChurch, GameState)
[
    HasRings == true
    && RemainingTime > 0
    && Health > 0
    && Hangover < 100
]
/
    GameResult = Victory
-> Victory, UpdatedGameState
```

## Diagrams

- [EFSM state flow](diagrams/efsm-state-flow.mmd)
- [Turn flow](diagrams/turn-flow.mmd)
- [Game map](diagrams/game-map.mmd)
- [Resource pressure](diagrams/resource-pressure.mmd)
