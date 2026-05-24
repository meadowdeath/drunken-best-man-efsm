using DrunkenBestManEFSM.Application.Contracts;

namespace DrunkenBestManEFSM.Infrastructure.Random;

/// <summary>
/// Provides random values using System.Random.
/// </summary>
public sealed class SystemRandomProvider : IRandomProvider
{
    private readonly System.Random random;

    public SystemRandomProvider()
        : this(new System.Random())
    {
    }

    public SystemRandomProvider(System.Random random)
    {
        this.random = random;
    }

    public int Next(int minValue, int maxValue) =>
        random.Next(minValue, maxValue);

    public T PickOne<T>(IReadOnlyList<T> values)
    {
        if (values.Count == 0)
        {
            throw new ArgumentException("Cannot pick a value from an empty collection.", nameof(values));
        }

        return values[random.Next(0, values.Count)];
    }
}
