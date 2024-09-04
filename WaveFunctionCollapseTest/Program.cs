using SFML.Graphics;
using SFML.System;
using SFML.Window;
using WaveFunctionCollapseTest;

Vector2u atlasSize = new(12, 21);
Vector2u gridSize = new(70, 70);
Texture tex = new("Resources/tilemap.png");

RenderWindow window = new(new VideoMode(1600, 1280), "Wave function collapse");
View view = new(new Vector2f(), new Vector2f(1600, 1280));
window.SetView(view);

Tilemap tilemap = new(tex, atlasSize, gridSize);

window.SetKeyRepeatEnabled(true);
window.Resized += (s, e) => window.SetView(new View(new Vector2f(), new Vector2f(e.Width, e.Height)));
window.Closed += (s, e) => window.Close();
window.KeyPressed += (s, e) =>
{

    if (e.Code == Keyboard.Key.Space)
    {
        if (e.Control)
        {
            for (int i = 0; i < 10; i++)
                tilemap.Tick();
        }
        else
        {
            tilemap.Tick();
        }
    }

    if (e.Code == Keyboard.Key.R)
        tilemap = new(tex, atlasSize, gridSize);
};

while (window.IsOpen)
{
    window.Clear(new Color(135, 206, 235));
    window.DispatchEvents();

    window.Draw(tilemap);

    window.Display();
}
