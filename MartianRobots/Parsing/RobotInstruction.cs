using MartianRobots.Domain;

namespace MartianRobots.Models;

/// <summary>
/// Represents the initial state and command instructions for a robot on Mars.
/// Contains the robot's starting position, orientation, and the sequence of movement/rotation commands to execute.
/// </summary>
public sealed record RobotInstruction(
    Position Position,
    Orientation Orientation,
    string Instructions);