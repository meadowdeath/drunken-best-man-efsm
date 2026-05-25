using DrunkenBestManEFSM.Application.Contracts;
using DrunkenBestManEFSM.Application.DTOs;
using DrunkenBestManEFSM.Application.Results;
using DrunkenBestManEFSM.Domain.Models;
using DrunkenBestManEFSM.Domain.Rules;

namespace DrunkenBestManEFSM.Application.Services;

/// <summary>
/// Manages the in-memory game session.
/// </summary>
public sealed class GameSessionService
{
    private readonly IRandomProvider randomProvider;
    private GameState? currentState;

    public GameSessionService(IRandomProvider randomProvider)
    {
        this.randomProvider = randomProvider;
    }

    public UseCaseResult<GameStatusDto> StartNewGame()
    {
        var correctChurch = randomProvider.PickOne(ChurchCatalog.GetChurches());
        var memoryThreshold = randomProvider.Next(GameLimits.MinMemoryThreshold, GameLimits.MaxMemoryThreshold + 1);

        currentState = GameStateFactory.CreateInitialState(correctChurch, memoryThreshold);

        return UseCaseResult<GameStatusDto>.Ok(ToStatusDto(currentState), "UseCase.Game.Started");
    }

    public GameState? GetCurrentState() =>
        currentState;

    public void SetCurrentState(GameState state) =>
        currentState = state;

    public bool HasActiveGame() =>
        currentState is not null;

    private static GameStatusDto ToStatusDto(GameState state)
    {
        var stats = state.CharacterStats;

        return new GameStatusDto
        {
            CurrentLocation = state.CurrentLocation,
            CarLocation = state.CarLocation,
            Health = stats.Health,
            Hangover = stats.Hangover,
            Drunkenness = stats.Drunkenness,
            Fuel = stats.Fuel,
            RemainingTime = stats.RemainingTime,
            Money = stats.Money,
            HasRings = state.HasRings,
            CorrectChurchKnown = state.CorrectChurchKnown,
            CorrectChurchToDisplay = state.CorrectChurchKnown ? state.CorrectChurch : null,
            Result = state.Result
        };
    }
}
