﻿using System.Diagnostics.CodeAnalysis;
using SFML.System;

namespace WaveFunctionCollapseTest;

public class Tile
{
    [MemberNotNullWhen(true, nameof(CollapsedState))]
    public bool IsCollapsed => CollapsedState != null;
    public TileState? CollapsedState { get; private set; }
    public float Entropy { get; private set; }

    public float TotalWeight => PossibleStates.Sum(ps => ps.Weight);

    public List<(float Weight, TileState State)> PossibleStates => _possibleStates;
    private List<(float Weight, TileState State)> _possibleStates = [];

    public Vector2u Position { get; }

    public Tile(Vector2u position)
    {
        Position = position;
    }

    public void Update(Neighbors neighbors)
    {
        List<(float Weight, TileState State)> result = [];

        float roadWeight = 10.0f / ((float)Math.Pow(61.5f, neighbors.Count(n => n?.IsCollapsed == true && n.CollapsedState.HasFlag(TileFlags.IsRoad)) * 5 + 1));

        result.Add((10, TileState.Grass));
        result.AddRange(TileState.AllStates
            .Where(s => s.HasFlag(TileFlags.IsRoad))
            .Select(s => (
            Weight: s.HasFlag(TileFlags.IsRoad | TileFlags.ConnectsOnTop | TileFlags.ConnectsOnBottom) || 
                    s.HasFlag(TileFlags.IsRoad | TileFlags.ConnectsOnRight | TileFlags.ConnectsOnLeft) 
                    ? roadWeight * 10 : roadWeight,
            State: s))
        );

        if (neighbors.Right is { IsCollapsed: true })
        {
            if (neighbors.Right.CollapsedState.HasFlag(TileFlags.IsRoad | TileFlags.ConnectsOnLeft))
                result.RemoveAll(r => !r.State.HasFlag(TileFlags.ConnectsOnRight));
            else
                result.RemoveAll(r => r.State.HasFlag(TileFlags.ConnectsOnRight));
        }

        if (neighbors.Up is { IsCollapsed: true })
        {
            if (neighbors.Up.CollapsedState.HasFlag(TileFlags.IsRoad | TileFlags.ConnectsOnBottom))
                result.RemoveAll(r => !r.State.HasFlag(TileFlags.ConnectsOnTop));
            else
                result.RemoveAll(r => r.State.HasFlag(TileFlags.ConnectsOnTop));
        }

        if (neighbors.Bottom is { IsCollapsed: true })
        {
            if (neighbors.Bottom.CollapsedState.HasFlag(TileFlags.IsRoad | TileFlags.ConnectsOnTop))
                result.RemoveAll(r => !r.State.HasFlag(TileFlags.ConnectsOnBottom));
            else
                result.RemoveAll(r => r.State.HasFlag(TileFlags.ConnectsOnBottom));
        }

        if (neighbors.Left is { IsCollapsed: true })
        {
            if (neighbors.Left.CollapsedState.HasFlag(TileFlags.IsRoad | TileFlags.ConnectsOnRight))
                result.RemoveAll(r => !r.State.HasFlag(TileFlags.ConnectsOnLeft));
            else
                result.RemoveAll(r => r.State.HasFlag(TileFlags.ConnectsOnLeft));

        }

        _possibleStates = result;
        UpdateEntropy();
    }

    public bool Collapse(Neighbors neighbors)
    {
        float summedUpWeight = 0;
        float totalWeight = (float)Random.Shared.NextDouble() * TotalWeight;
        foreach (var (weight, state) in PossibleStates)
        {
            summedUpWeight += weight;

            if (summedUpWeight > totalWeight)
            {
                CollapsedState = state;
                return true;
            }
        }

        return false;
    }

    private void UpdateEntropy()
    {
        float totalWeight = PossibleStates.Sum(ps => ps.Weight);

        Entropy = PossibleStates.Sum(ps =>
        {
            float probability = ps.Weight / totalWeight;
            return -probability * (float)Math.Log2(probability);
        });
    }
}

public static class TileExtensions
{
    public static bool CouldHave(this Tile? tile, TileFlags flags)
    {
        return tile is null or { IsCollapsed: false } || tile.CollapsedState.HasFlag(flags);
    }
}