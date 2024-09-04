using System.Collections;

namespace WaveFunctionCollapseTest;

public readonly record struct Neighbors(Tile? Up, Tile? Left, Tile? Bottom, Tile? Right) : IEnumerable<Tile?>
{
    public IEnumerator<Tile?> GetEnumerator()
    {
        yield return Up;
        yield return Left;
        yield return Bottom;
        yield return Right;
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
