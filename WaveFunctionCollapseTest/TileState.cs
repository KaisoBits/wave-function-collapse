using SFML.System;

namespace WaveFunctionCollapseTest;

public record class TileState(Vector2u TextureCoord, TileFlags Flags)
{
    public static IReadOnlyList<TileState> AllStates => _allStates;
    private static readonly List<TileState> _allStates = [];

    public static TileState Grass { get; } = Create(new Vector2u(0, 0));
    public static TileState Flowers { get; } = Create(new Vector2u(2, 2));

    public static TileState RoadHorLeft { get; } = Create(new Vector2u(5, 0), TileFlags.IsRoad | TileFlags.ConnectsOnRight);
    public static TileState RoadHorMid { get; } = Create(new Vector2u(6, 0), TileFlags.IsRoad | TileFlags.ConnectsOnLeft | TileFlags.ConnectsOnRight);
    public static TileState RoadHorRight { get; } = Create(new Vector2u(7, 0), TileFlags.IsRoad | TileFlags.ConnectsOnLeft);

    public static TileState RoadVertTop { get; } = Create(new Vector2u(4, 1), TileFlags.IsRoad | TileFlags.ConnectsOnBottom);
    public static TileState RoadVertMid { get; } = Create(new Vector2u(4, 2), TileFlags.IsRoad | TileFlags.ConnectsOnTop | TileFlags.ConnectsOnBottom);
    public static TileState RoadVertBottom { get; } = Create(new Vector2u(4, 3), TileFlags.IsRoad | TileFlags.ConnectsOnTop);

    public static TileState RoadTopLeft { get; } = Create(new Vector2u(5, 1), TileFlags.IsRoad | TileFlags.ConnectsOnBottom | TileFlags.ConnectsOnRight);
    public static TileState RoadTopMid { get; } = Create(new Vector2u(6, 1), TileFlags.IsRoad | TileFlags.ConnectsOnLeft | TileFlags.ConnectsOnBottom | TileFlags.ConnectsOnRight);
    public static TileState RoadTopRight { get; } = Create(new Vector2u(7, 1), TileFlags.IsRoad | TileFlags.ConnectsOnLeft | TileFlags.ConnectsOnBottom);

    public static TileState RoadMidLeft { get; } = Create(new Vector2u(5, 2), TileFlags.IsRoad | TileFlags.ConnectsOnTop | TileFlags.ConnectsOnBottom | TileFlags.ConnectsOnRight);
    public static TileState RoadMidMid { get; } = Create(new Vector2u(6, 2), TileFlags.IsRoad | TileFlags.ConnectsOnLeft | TileFlags.ConnectsOnBottom | TileFlags.ConnectsOnRight | TileFlags.ConnectsOnTop);
    public static TileState RoadMidRight { get; } = Create(new Vector2u(7, 2), TileFlags.IsRoad | TileFlags.ConnectsOnBottom | TileFlags.ConnectsOnLeft | TileFlags.ConnectsOnTop);

    public static TileState RoadBottomLeft { get; } = Create(new Vector2u(5, 3), TileFlags.IsRoad | TileFlags.ConnectsOnTop | TileFlags.ConnectsOnRight);
    public static TileState RoadBottomMid { get; } = Create(new Vector2u(6, 3), TileFlags.IsRoad | TileFlags.ConnectsOnLeft | TileFlags.ConnectsOnTop | TileFlags.ConnectsOnRight);
    public static TileState RoadBottomRight { get; } = Create(new Vector2u(7, 3), TileFlags.IsRoad | TileFlags.ConnectsOnLeft | TileFlags.ConnectsOnTop);

    public bool HasFlag(TileFlags flag) => (Flags & flag) == flag;

    private static TileState Create(Vector2u textureCoord, TileFlags flags = TileFlags.None)
    {
        TileState result = new(textureCoord, flags);
        _allStates.Add(result);
        return result;
    }
};

[Flags]
public enum TileFlags
{
    None = 0,
    IsRoad = 1 << 0,
    ConnectsOnLeft = 1 << 1,
    ConnectsOnTop = 1 << 2,
    ConnectsOnRight = 1 << 3,
    ConnectsOnBottom = 1 << 4,
}