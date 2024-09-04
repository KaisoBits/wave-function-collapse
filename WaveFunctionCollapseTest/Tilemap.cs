using SFML.Graphics;
using SFML.System;

namespace WaveFunctionCollapseTest;

public class Tilemap : Transformable, Drawable
{
    private readonly Texture _texture;
    private readonly Vector2u _atlasSize;
    private readonly Vector2u _gridSize;
    private readonly Clock _clock;

    private readonly VertexArray _vertexArray;
    private readonly Vector2u _textureTileSize;

    private readonly Tile[,] _map;

    private readonly Font _font = new("Resources/Roboto-Bold.ttf");
    private readonly Text _text;

    private readonly RectangleShape _rectangleShape;

    private readonly List<Tile> _lowestEntropySortedTiles = [];

    public Tilemap(Texture texture, Vector2u atlasSize, Vector2u gridSize, Clock clock)
    {
        _texture = texture;
        _atlasSize = atlasSize;
        _gridSize = gridSize;
        _clock = clock;

        _vertexArray = new VertexArray(PrimitiveType.Quads, 4);

        _textureTileSize = new Vector2u(_texture.Size.X / atlasSize.X, _texture.Size.Y / atlasSize.Y);

        _map = new Tile[gridSize.X, gridSize.Y];
        for (uint y = 0; y < gridSize.Y; y++)
            for (uint x = 0; x < gridSize.X; x++)
            {
                Tile t = new(new Vector2u(x, y));
                _map[x, y] = t;
                _lowestEntropySortedTiles.Add(t);
            }

        foreach (Tile tile in _lowestEntropySortedTiles)
            tile.Update(GetNeighbors(tile.Position));

        _rectangleShape = new RectangleShape((Vector2f)_textureTileSize)
        {
            FillColor = new Color(200, 0, 200, 50),
            OutlineColor = Color.Red,
            OutlineThickness = -1
        };

        _text = new Text("", _font)
        {
            CharacterSize = 8,
            Origin = new Vector2f(-4, -1)
        };
    }

    public void Tick()
    {
        if (_lowestEntropySortedTiles is [])
            return;

        Tile tileToCollapse = _lowestEntropySortedTiles[0];

        Neighbors collapsedTileNeighbors = GetNeighbors(tileToCollapse.Position);
        tileToCollapse.Collapse();
        foreach (Tile? neighbor in collapsedTileNeighbors)
        {
            neighbor?.Update(GetNeighbors(neighbor.Position));
        }

        _lowestEntropySortedTiles.Remove(tileToCollapse);
        _lowestEntropySortedTiles.Sort((a, b) => a.Entropy.CompareTo(b.Entropy));
    }

    public void Draw(RenderTarget target, RenderStates states)
    {
        Vector2f fullSize = new Vector2f(_textureTileSize.X * _gridSize.X, _textureTileSize.Y * _gridSize.Y);

        for (uint x = 0; x < _gridSize.X; x++)
        {
            for (uint y = 0; y < _gridSize.Y; y++)
            {
                Tile tile = _map[x, y];

                RenderStates rs = states;
                rs.Transform.Translate(new Vector2f(x * _textureTileSize.X, y * _textureTileSize.Y) - (fullSize / 2.0f));

                if (tile.IsCollapsed)
                    DrawTile(tile, target, rs);
                else
                    DrawPossibility(tile, target, rs);
            }
        }
    }

    public void DrawTile(Tile tile, RenderTarget target, RenderStates states)
    {
        if (!tile.IsCollapsed)
            throw new Exception("Can't draw not collapsed tile");

        Vector2f textureCoord = new(tile.CollapsedState.TextureCoord.X * _textureTileSize.X, tile.CollapsedState.TextureCoord.Y * _textureTileSize.Y);

        _vertexArray[0] = new Vertex(new Vector2f(_textureTileSize.X, 0), textureCoord + new Vector2f(_textureTileSize.X, 0));
        _vertexArray[1] = new Vertex(new Vector2f(0, 0), textureCoord + new Vector2f(0, 0));
        _vertexArray[2] = new Vertex(new Vector2f(0, _textureTileSize.Y), textureCoord + new Vector2f(0, _textureTileSize.Y));
        _vertexArray[3] = new Vertex(new Vector2f(_textureTileSize.X, _textureTileSize.Y), textureCoord + new Vector2f(_textureTileSize.X, _textureTileSize.Y));
        states.Texture = _texture;

        target.Draw(_vertexArray, states);
    }

    public void DrawPossibility(Tile tile, RenderTarget target, RenderStates states)
    {
        if (tile.IsCollapsed)
            throw new Exception("Can't draw collapsed tile as a possibility");

        float ratio = (_clock.ElapsedTime.AsSeconds() * 0.5f) % 1;
        int index = (int)Math.Floor(ratio * tile.PossibleStates.Count);

        TileState currentlyShownState = tile.PossibleStates[index].State;
        Vector2f textureCoord = new(currentlyShownState.TextureCoord.X * _textureTileSize.X, currentlyShownState.TextureCoord.Y * _textureTileSize.Y);

        _vertexArray[0] = new Vertex(new Vector2f(_textureTileSize.X, 0), textureCoord + new Vector2f(_textureTileSize.X, 0));
        _vertexArray[1] = new Vertex(new Vector2f(0, 0), textureCoord + new Vector2f(0, 0));
        _vertexArray[2] = new Vertex(new Vector2f(0, _textureTileSize.Y), textureCoord + new Vector2f(0, _textureTileSize.Y));
        _vertexArray[3] = new Vertex(new Vector2f(_textureTileSize.X, _textureTileSize.Y), textureCoord + new Vector2f(_textureTileSize.X, _textureTileSize.Y));
        states.Texture = _texture;

        target.Draw(_vertexArray, states);

        if (_lowestEntropySortedTiles[0] == tile)
            target.Draw(_rectangleShape, states);

        _text.DisplayedString = tile.PossibleStates.Count.ToString();
        target.Draw(_text, states);
    }

    private Neighbors GetNeighbors(Vector2u position)
    {
        Vector2u upPos = position - new Vector2u(0, 1);
        Vector2u leftPos = position - new Vector2u(1, 0);
        Vector2u downPos = position + new Vector2u(0, 1);
        Vector2u rightPos = position + new Vector2u(1, 0);
        return new Neighbors(GetTileAt(upPos), GetTileAt(leftPos), GetTileAt(downPos), GetTileAt(rightPos));
    }

    private Tile? GetTileAt(Vector2u position)
    {
        return IsInBounds(position) ? _map[position.X, position.Y] : null;
    }

    private bool IsInBounds(Vector2u position)
    {
        return position.X >= 0 && position.X < _gridSize.X &&
            position.Y >= 0 && position.Y < _gridSize.Y;
    }

    private float Lerp(float value, float min, float max)
    {
        return Math.Clamp(min + (max - min) * value, min, max);
    }
}
