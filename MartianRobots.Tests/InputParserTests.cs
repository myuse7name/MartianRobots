using MartianRobots.Domain;
using MartianRobots.Parsing;

namespace MartianRobots.Tests;

public class InputParserTests
{
    [Fact]
    public void Parse_ShouldAcceptWhitespaceSeparatedInput()
    {
        var input = "5\t3\r\n1  1 E\r\nRFRFRFRF\r\n";

        var data = new InputParser().Parse(input);

        Assert.Equal(new World(5, 3).MaxX, data.World.MaxX);
        Assert.Equal(new World(5, 3).MaxY, data.World.MaxY);
        Assert.Single(data.Robots);
        Assert.Equal(new Position(1, 1), data.Robots[0].Position);
        Assert.Equal(Orientation.East, data.Robots[0].Orientation);
        Assert.Equal("RFRFRFRF", data.Robots[0].Instructions);
    }
}