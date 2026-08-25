namespace MartianRobots.Domain;

public sealed class Robot
{
    public Position Position { get; set; }

    public Orientation Orientation { get; set; }

    public bool IsLost { get; set; }

    public Robot(Position position, Orientation orientation)
    {
        Position = position;
        Orientation = orientation;
    }
}