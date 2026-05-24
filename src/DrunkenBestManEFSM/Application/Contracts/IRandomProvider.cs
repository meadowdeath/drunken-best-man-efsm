namespace DrunkenBestManEFSM.Application.Contracts;

/// <summary>
/// Provides random values for application-level orchestration.
/// </summary>
public interface IRandomProvider
{
    int Next(int minValue, int maxValue);

    T PickOne<T>(IReadOnlyList<T> values);
}
