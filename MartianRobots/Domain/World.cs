namespace MartianRobots.Domain;

public sealed class World
{
    public int MaxX { get; }

    public int MaxY { get; }

    public HashSet<Scent> Scents { get; } = new();

    public World(int maxX, int maxY)
    {
        MaxX = maxX;
        MaxY = maxY;
    }
}
