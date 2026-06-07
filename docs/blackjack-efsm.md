# Nested Blackjack EFSM

## Purpose

Blackjack is an optional Casino feature for DrunkenBestManEFSM. It gives the player a high-risk way to obtain money during the main game.

The Casino is not mandatory and is not a final boss. The player must be able to complete the main game without visiting it. Its purpose is to create a strategic tradeoff:

- Winning Blackjack increases `Money`.
- Losing Blackjack decreases `Money`.
- Every completed round consumes `RemainingTime`.
- Time passing still worsens the main-game condition through passive effects.
- Spending too much time or money in the Casino can make the wedding objective impossible.

Money gained from Blackjack can be used in the main game to buy fuel, electrolytes, or alcohol.

## Why Blackjack Is a Nested State Machine

Blackjack has its own states, actions, rules, context, and terminal results. It is not just a single main-game action.

The main EFSM controls locations, health, hangover, time, fuel, car position, rings, and victory or defeat. Blackjack controls cards, hands, betting, dealer behavior, and round outcome.

This makes Blackjack a nested EFSM:

```text
Main EFSM at Casino
-> PlayBlackjack
-> Nested Blackjack EFSM
-> BlackjackRoundResult
-> Main EFSM resumes at Casino
```

The nested machine should not directly own all main-game resources. It should produce a result that the main EFSM consumes.

## Relationship with the Main EFSM

When the player is at `Casino`, the main game may expose `PlayBlackjack`.

Starting Blackjack activates a `BlackjackGameState`. While Blackjack is active, normal main-game actions are temporarily suspended. When the round finishes, control returns to the main game at `Casino`.

Conceptual flow:

1. Main EFSM has `CurrentLocation = Casino`.
2. Player chooses `PlayBlackjack`.
3. Application creates an active `BlackjackGameState`.
4. The nested Blackjack EFSM runs until `Finished`.
5. The nested machine returns `BlackjackRoundResult`.
6. The main EFSM applies money and time changes.
7. The main game applies passive turn effects.
8. The main game checks defeat conditions.
9. Control resumes at `Casino`.

Blackjack decides:

- Cards dealt.
- Player hand value.
- Dealer behavior.
- Round winner.
- Blackjack result.

The main game decides:

- How money changes affect the player.
- How much remaining time is consumed.
- Passive hangover and drunkenness effects.
- Whether the overall game is lost.

## Blackjack States

`WaitingForBet`

The round exists, but no valid bet has been accepted. The player may place a valid bet or leave.

`InitialDeal`

Two cards are dealt to the player and two cards to the dealer. Natural Blackjack outcomes may be detected here.

`PlayerTurn`

The player may choose `Hit` or `Stand`.

`DealerTurn`

The dealer follows deterministic rules. No player input is required.

`Resolving`

Player and dealer hands are compared.

`Finished`

The nested machine has reached a terminal state and returns a result to the main EFSM.

## Blackjack Actions

`PlaceBet`

Accepted only while `WaitingForBet` and only when the bet is valid.

`Hit`

Adds one card to the player's hand. If the hand exceeds `21`, the round finishes with `DealerWin`. If the hand reaches exactly `21`, the nested EFSM transitions to `DealerTurn`.

`Stand`

Ends the player's turn and activates `DealerTurn`.

`Leave`

Allows the player to cancel before a round begins. The initial version does not allow abandoning a round after cards are dealt.

## Blackjack Extended State

Implemented `BlackjackGameState`:

- `BlackjackState State`
- `BlackjackHand PlayerHand`
- `BlackjackHand DealerHand`
- `BlackjackDeck Deck`
- `int BetAmount`
- `BlackjackResult Result`

`BlackjackGameState` belongs only to the nested machine. Blackjack-specific card and deck data should not be placed directly inside the main `GameState`.

The main `GameState` does not store player cards, dealer cards, or deck data. Application manages the active nested session and passes a completed `BlackjackRoundResult` back to the main EFSM.

## Card and Hand Rules

Conceptual card model:

`Card`

- `CardSuit Suit`
- `CardRank Rank`

Suits:

- `Hearts`
- `Diamonds`
- `Clubs`
- `Spades`

Ranks:

- `Ace`
- `Two` through `Ten`
- `Jack`
- `Queen`
- `King`

Card values:

- Numbered cards use their numeric value.
- `Jack`, `Queen`, and `King` are worth `10`.
- `Ace` is worth `11` unless that would make the hand exceed `21`; then it is worth `1`.

Hand rules:

- Calculate total value.
- Detect bust.
- Detect natural Blackjack.
- Correctly handle one or multiple Aces.

Natural Blackjack is represented explicitly through `PlayerBlackjack` and `DealerBlackjack`.

## Dealer Behavior

The initial dealer rule is deterministic:

- Dealer must `Hit` while hand value is below `17`.
- Dealer must `Stand` at `17` or above.
- Dealer stands on every `17`, including soft `17`, for the initial version.
- Dealer behavior does not require player input.
- Dealer behavior should still be represented through explicit state transitions.

Conceptual transitions:

```text
DealerTurn
[DealerValue < 17]
/ DrawCard()
-> DealerTurn

DealerTurn
[DealerValue >= 17]
-> Resolving
```

Implementation should avoid hiding the nested-machine behavior inside Presentation. Application may automatically advance dealer turns, but the rules and state changes belong to Domain.

## Betting and Economy

Implemented values:

- `MinimumBet = 10`
- `MaximumBet = 30`
- `BlackjackRoundTimeCost = 4 minutes`
- `BlackjackExitTimeCost = 0 minutes` when leaving before betting

Net result model:

| Result | MoneyChange |
| --- | --- |
| `PlayerWin` | `+BetAmount` |
| `PlayerBlackjack` | `+BetAmount` |
| `DealerWin` | `-BetAmount` |
| `DealerBlackjack` | `-BetAmount` |
| `Draw` | `0` |
| `Exited` | `0` |

This represents net profit or loss.

Example with initial `Money = 50` and `Bet = 10`:

- `PlayerWin`: final money is `60`.
- `DealerWin`: final money is `40`.
- `Draw`: final money is `50`.

Each completed round consumes `BlackjackRoundTimeCost` regardless of result. After a completed round, the main EFSM applies:

- Remaining time reduction.
- Passive hangover increase.
- Passive drunkenness reduction.
- Main-game defeat checks.

Blackjack must not generate free money without time or risk.

No fixed round limit is needed initially. The player may continue playing while:

- Money is sufficient for a valid bet.
- Remaining time is sufficient.
- The main game is still in progress.

## Round Results

Implemented `BlackjackResult` values:

- `None`
- `PlayerWin`
- `DealerWin`
- `Draw`
- `PlayerBlackjack`
- `DealerBlackjack`
- `Exited`

Result passed from the nested machine to the main machine:

`BlackjackRoundResult`

- `BlackjackResult Result`
- `int BetAmount`
- `int MoneyChange`
- `int TimeCost`

The nested Blackjack EFSM produces `BlackjackRoundResult`. The main EFSM consumes it and updates its own `GameState`.

This prevents the nested machine from directly controlling all main-game resources.

## Layer Responsibilities

Domain/Blackjack:

- Cards.
- Hands.
- Deck state.
- Blackjack rules.
- Dealer rules.
- Outcome rules.
- Nested state transitions.
- Blackjack round result.

Application/Blackjack:

- Create and manage active Blackjack sessions.
- Create a shuffled deck using `IRandomProvider`.
- Coordinate player actions.
- Automatically advance dealer turns.
- Convert internal state into DTOs.
- Return `UseCaseResult<T>`.

Presentation/Blackjack:

- Ask for bets.
- Allow cancellation before a round begins.
- Display player cards.
- Hide one dealer card during `PlayerTurn`.
- Reveal dealer hand after the round.
- Ask the player to `Hit` or `Stand`.
- Display the result.

Infrastructure:

- Continue providing randomness through the existing `IRandomProvider` implementation.
- Do not contain Blackjack rules.

Main Domain:

- Contains the `Casino` location and `PlayBlackjack` action.
- Applies `BlackjackRoundResult` to the main `GameState`.
- Applies normal passive effects and outcome checks.

## Folder Organization

Blackjack uses subfolders inside the existing layers where needed.

```text
Domain/
|-- Enums/
|   `-- Blackjack/
|-- Models/
|   `-- Blackjack/
|-- Rules/
|   `-- Blackjack/
|-- Results/
|   `-- Blackjack/
`-- Transitions/
    `-- Blackjack/

Application/
|-- DTOs/
|   `-- Blackjack/
`-- Services/
    `-- Blackjack/

Presentation/
|-- Menus/
|   `-- Blackjack/
`-- Renderers/
    `-- Blackjack/
```

Root files in each layer continue representing the main game. Blackjack subfolders represent the nested machine. Shared contracts such as `IRandomProvider` remain in their existing root folders. Shared generic results such as `UseCaseResult<T>` remain outside Blackjack-specific folders.

Do not create `MainGame` subfolders at this stage.

## Simplified Initial Scope

The implemented initial version includes:

- Single player.
- One dealer.
- One active hand for the player.
- One standard deck.
- `PlaceBet`, `Hit`, `Stand`, and `Leave`.
- Deterministic dealer behavior.
- Net money result.
- Round time cost.

The initial version does not implement:

- Split.
- Double down.
- Insurance.
- Surrender.
- Multiple players.
- Multiple simultaneous hands.
- Multiple decks.

## Risks and Balance Considerations

The Casino should feel tempting but dangerous.

Blackjack can solve a money shortage, but it can also waste the player's time or leave the player unable to buy fuel, electrolytes, or alcohol. Because each completed round consumes time and causes the main game to apply passive effects afterward, repeated Casino play can make the wedding objective unwinnable.

The player must still be able to win the main game without visiting the Casino.

## Diagrams

- [Blackjack state flow](diagrams/blackjack-state-flow.mmd)
- [Nested EFSM flow](diagrams/nested-efsm-flow.mmd)

## Implementation Boundaries

- Domain owns card values, hand evaluation, dealer rules, outcome rules, and nested state transitions.
- Application creates and shuffles decks through `IRandomProvider`, stores the active session, coordinates player actions, and advances dealer turns by calling Domain transitions.
- Presentation displays the Blackjack menu, cards, visible dealer information, and action results.
- The main EFSM consumes only `BlackjackRoundResult`; it does not draw cards, calculate hand values, or decide Blackjack winners.
