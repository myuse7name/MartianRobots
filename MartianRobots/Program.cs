using MartianRobots.Domain;
using MartianRobots.Parsing;
using MartianRobots.Services;
using System.Text;

var input = Console.IsInputRedirected
    ? Console.In.ReadToEnd()
    : ReadInteractiveInput();

var parser = new InputParser();
var simulator = new RobotSimulator();

var data = parser.Parse(input);

foreach (var robotInstruction in data.Robots)
{
    var robot = new Robot(
        robotInstruction.Position,
        robotInstruction.Orientation);

    simulator.ExecuteInstructions(
        robot,
        data.World,
        robotInstruction.Instructions);

    Console.WriteLine(
        $"{robot.Position.X} {robot.Position.Y} {ToOutput(robot.Orientation)}" +
        (robot.IsLost ? " LOST" : string.Empty));
}

static string ToOutput(Orientation orientation)
{
    return orientation switch
    {
        Orientation.North => "N",
        Orientation.East => "E",
        Orientation.South => "S",
        Orientation.West => "W",
        _ => throw new InvalidOperationException()
    };
}

static string ReadInteractiveInput()
{
    Console.WriteLine("Martian Robots Instructions");
    Console.WriteLine("---------------------------");    
    Console.WriteLine("1. Enter the world size using X and Y coordinates (Example: 5 3)");        
    Console.WriteLine("2. Enter how many robots you want to simulate.");    
    Console.WriteLine("3. For each robot, enter its starting position. Format: x y direction. (Example: 1 1 E");
    Console.WriteLine("4. Enter the movement instructions. L=Left, R=Right, F=Forward. (Example: RFRFRFRF)");    
    Console.WriteLine();

    var builder = new StringBuilder();

    builder.AppendLine(ReadRequiredLine("World size: "));

    var robotCount = int.Parse(
        ReadRequiredLine("Number of robots: "));

    for (var robotNumber = 1; robotNumber <= robotCount; robotNumber++)
    {
        Console.WriteLine();
        Console.WriteLine($"Robot {robotNumber} of {robotCount}");

        builder.AppendLine(
            ReadRequiredLine("Starting position: "));

        builder.AppendLine(
            ReadRequiredLine("Instructions: "));
    }

    return builder.ToString();
}

static string ReadRequiredLine(string prompt)
{
    Console.Write(prompt);
    return Console.ReadLine()
        ?? throw new InvalidOperationException("Input ended before the required value was entered.");
}
