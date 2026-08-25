using MartianRobots.Domain;

namespace MartianRobots.Models;

/// <summary>
/// Contains the parsed input data: the world configuration and a collection of robot instructions to execute.
/// </summary>
public sealed record InputData(
    World World,
    IReadOnlyList<RobotInstruction> Robots);