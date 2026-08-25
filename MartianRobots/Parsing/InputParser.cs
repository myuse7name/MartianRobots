using MartianRobots.Domain;
using MartianRobots.Models;

namespace MartianRobots.Parsing;

/// <summary>
/// Parses input text into structured data containing world dimensions and robot instructions.
/// Validates and converts raw input strings into Position, Orientation, and instruction sequences.
/// </summary>
public sealed class InputParser
{
    public InputData Parse(string input)
    {
        var tokens = input.Split(
            (char[]?)null,
            StringSplitOptions.RemoveEmptyEntries);

        if (tokens.Length < 2)
        {
            throw new ArgumentException(
                "Invalid input. The first line must contain the world size in the format 'x y'.");
        }

        if (!int.TryParse(tokens[0], out var maxX) ||
            !int.TryParse(tokens[1], out var maxY))
        {
            throw new ArgumentException(
                "Invalid world size. Example: 5 3");
        }

        var world = new World(maxX, maxY);

        var robots = new List<RobotInstruction>();

        for (var i = 2; i < tokens.Length; i += 4)
        {
            if (i + 3 >= tokens.Length)
            {
                throw new ArgumentException(
                    "Invalid robot input. Each robot must include a start position and instruction sequence.");
            }

            if (!int.TryParse(tokens[i], out var x) ||
                !int.TryParse(tokens[i + 1], out var y))
            {
                throw new ArgumentException(
                    $"Invalid robot coordinates near '{tokens[i]} {tokens[i + 1]}'.");
            }

            var orientation = tokens[i + 2] switch
            {
                "N" => Orientation.North,
                "E" => Orientation.East,
                "S" => Orientation.South,
                "W" => Orientation.West,
                _ => throw new ArgumentException(
                    $"Invalid orientation '{tokens[i + 2]}'. Valid values are N, E, S and W.")
            };

            var instructions = tokens[i + 3];

            if (instructions.Any(x => x is not ('L' or 'R' or 'F')))
            {
                throw new ArgumentException(
                    $"Invalid instruction sequence '{instructions}'. Only L, R and F are supported.");
            }

            robots.Add(
                new RobotInstruction(
                    new Position(x, y),
                    orientation,
                    instructions));
        }

        return new InputData(world, robots);
    }
}