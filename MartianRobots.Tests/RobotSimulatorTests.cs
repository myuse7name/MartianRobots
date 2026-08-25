using MartianRobots.Domain;
using MartianRobots.Services;

namespace MartianRobots.Tests;

/// <summary>
/// Test suite for the RobotSimulator class, verifying robot movement, rotation, and instruction execution.
/// Tests cover turn operations, forward movement, grid boundary constraints, and scent detection.
/// </summary>
public class RobotSimulatorTests
{
    /// <summary>
    /// Tests that a robot facing North correctly turns to face West when TurnLeft is called.
    /// </summary>
    [Fact]
    public void TurnLeft_FromNorth_ShouldFaceWest()
    {
        // Arrange: Create a robot at position (1,1) facing North and a simulator instance
        var robot = new Robot(
            new Position(1, 1),
            Orientation.North);

        var simulator = new RobotSimulator();

        // Act: Execute a left turn command
        simulator.TurnLeft(robot);

        // Assert: Verify the robot's orientation changed to West
        Assert.Equal(
            Orientation.West,
            robot.Orientation);
    }

    /// <summary>
    /// Tests that a robot attempting to move forward off the grid is correctly marked as lost.
    /// Robot starts at the top-right corner (5,3) and tries to move North (off the grid).
    /// </summary>
    [Fact]
    public void MoveForward_OffGrid_ShouldMarkRobotAsLost()
    {
        // Arrange: Create a world with bounds 5x3 and a robot at the edge position (5,3) facing North
        var world = new World(5, 3);

        var robot = new Robot(
            new Position(5, 3),
            Orientation.North);

        var simulator = new RobotSimulator();

        // Act: Attempt to move the robot forward (which would take it off the grid)
        simulator.MoveForward(robot, world);

        // Assert: Verify the robot's IsLost flag is set to true
        Assert.True(robot.IsLost);
    }

    /// <summary>
    /// Theory test that verifies ExecuteInstructions correctly processes multiple instruction sequences.
    /// Tests various command combinations (R=turn right, L=turn left, F=move forward) and validates final position and orientation.
    /// </summary>
    [Theory]
    [InlineData("RFRFRFRF", 1, 1, Orientation.East)]
    [InlineData("RRRR", 1, 1, Orientation.East)]
    [InlineData("L", 1, 1, Orientation.North)]
    public void ExecuteInstructions_ShouldUpdatePositionAndOrientation
    (
        string instructions,
        int expectedX,
        int expectedY,
        Orientation expectedOrientation)
    {
        // Arrange: Create a 5x3 world and place a robot at (1,1) facing East
        var world = new World(5, 3);

        var robot = new Robot(
            new Position(1, 1),
            Orientation.East);

        var simulator = new RobotSimulator();

        // Act: Execute the instruction string on the robot
        simulator.ExecuteInstructions(
            robot,
            world,
            instructions);

        // Assert: Verify the robot's final position matches expected coordinates
        Assert.Equal(
            new Position(expectedX, expectedY),
            robot.Position);

        // Assert: Verify the robot's final orientation matches expected direction
        Assert.Equal(
            expectedOrientation,
            robot.Orientation);
    }

    /// <summary>
    /// Tests that a robot moving forward within the grid bounds updates its position correctly.
    /// Verifies the robot stays on grid and is not marked as lost.
    /// </summary>
    [Fact]
    public void MoveForward_WithinBounds_ShouldUpdatePosition()
    {
        // Arrange: Create a 5x3 world and place a robot at (1,1) facing North
        var world = new World(5, 3);

        var robot = new Robot(
            new Position(1, 1),
            Orientation.North);

        var simulator = new RobotSimulator();

        // Act: Execute a forward movement command
        simulator.MoveForward(robot, world);

        // Assert: Verify the robot's position was updated to (1,2) - one step north
        Assert.Equal(
            new Position(1, 2),
            robot.Position);

        // Assert: Verify the robot is not marked as lost
        Assert.False(robot.IsLost);
    }

    /// <summary>
    /// Tests that a robot attempting to move to a position with a scent (from a lost robot) ignores the move.
    /// The robot stays at its current position and is not marked as lost.
    /// </summary>
    [Fact]
    public void MoveForward_WhenScentExists_ShouldIgnoreMove()
    {
        // Arrange: Create a 5x3 world and add a scent at position (5,3)
        var world = new World(5, 3);

        world.Scents.Add(
            new Scent(new Position(5, 3)));

        // Create a robot at (5,3) facing East
        var robot = new Robot(
            new Position(5, 3),
            Orientation.East);

        var simulator = new RobotSimulator();

        // Act: Attempt to move the robot forward (would go to 6,3 but scent blocks it)
        simulator.MoveForward(robot, world);

        // Assert: Verify the robot stayed at its original position (5,3)
        Assert.Equal(
            new Position(5, 3),
            robot.Position);

        // Assert: Verify the robot is not marked as lost (scent warning prevented the move)
        Assert.False(robot.IsLost);
    }

    /// <summary>
    /// Tests that a robot at a position with a scent ignores forward movement regardless of its orientation.
    /// Verifies the scent detection logic works independently of robot direction.
    /// </summary>
    [Fact]
    public void MoveForward_WhenScentExistsAtPosition_IgnoresMoveRegardlessOfOrientation()
    {
        // Arrange: Create a 5x3 world and add a scent at position (5,3)
        var world = new World(5, 3);
        world.Scents.Add(new Scent(new Position(5, 3)));

        // Create a robot at (5,3) facing North
        var robot = new Robot(
            new Position(5, 3),
            Orientation.North);

        var simulator = new RobotSimulator();

        // Act: Attempt to move the robot forward
        simulator.MoveForward(robot, world);

        // Assert: Verify the robot stayed at its original position
        Assert.Equal(new Position(5, 3), robot.Position);

        // Assert: Verify the robot's orientation was not changed
        Assert.Equal(Orientation.North, robot.Orientation);

        // Assert: Verify the robot is not marked as lost (scent prevented the move)
        Assert.False(robot.IsLost);
    }

    /// <summary>
    /// Theory test that validates robot execution against expected challenge results.
    /// Tests two scenarios:
    /// 1. A robot that completes instructions and returns to start position
    /// 2. A robot that falls off the grid and becomes lost while executing instructions
    /// </summary>
    [Theory]
    [InlineData(1, 1, Orientation.East, "RFRFRFRF", 1, 1,Orientation.East,false)]
    [InlineData(3, 2, Orientation.North, "FRRFLLFFRRFLL", 3, 3,Orientation.North,true)]
    public void ExecuteInstructions_ShouldProduceExpectedChallengeResults(
        int startX,
        int startY,
        Orientation startOrientation,
        string instructions,
        int expectedX,
        int expectedY,
        Orientation expectedOrientation,
        bool expectedLost)
    {
        // Arrange: Create a 5x3 world and place a robot at the starting position and orientation
        var world = new World(5, 3);

        var robot = new Robot(
            new Position(startX, startY),
            startOrientation);

        var simulator = new RobotSimulator();

        // Act: Execute the instruction sequence on the robot
        simulator.ExecuteInstructions(
            robot,
            world,
            instructions);

        // Assert: Verify the robot's final X and Y coordinates match expected values
        Assert.Equal(
            new Position(expectedX, expectedY),
            robot.Position);

        // Assert: Verify the robot's final orientation matches expected direction
        Assert.Equal(
            expectedOrientation,
            robot.Orientation);

        // Assert: Verify the robot's lost state matches expected value (true if lost, false if still on grid)
        Assert.Equal(
            expectedLost,
            robot.IsLost);
    }

    /// <summary>
    /// Integration test that verifies multiple robots interact correctly through scent markers.
    /// First robot executes instructions and may fall off the grid leaving a scent,
    /// then a second robot navigates with awareness of the scent from the first robot's path.
    /// </summary>
    [Fact]
    public void ExecuteInstructions_ShouldRespectScentFromPreviousRobot()
    {
        // Arrange: Create a 5x3 world and simulator
        var world = new World(5, 3);
        var simulator = new RobotSimulator();

        // First robot setup: Position at (3,2) facing North
        var firstRobot = new Robot(
            new Position(3, 2),
            Orientation.North);

        // Act: Execute instructions for the first robot (which will fall off the grid and leave a scent)
        simulator.ExecuteInstructions(firstRobot, world, "FRRFLLFFRRFLL");

        // Arrange: Create a second robot at starting position (0,3) facing West
        var secondRobot = new Robot(new Position(0, 3), Orientation.West);

        // Act: Execute instructions for the second robot (which should avoid the scent left by the first robot)
        simulator.ExecuteInstructions(secondRobot, world, "LLFFFLFLFL");

        // Assert: Verify the second robot's final position (should have navigated around the scent)
        Assert.Equal(
            new Position(2, 3),
            secondRobot.Position);

        // Assert: Verify the second robot's final orientation
        Assert.Equal(
            Orientation.South,
            secondRobot.Orientation);

        // Assert: Verify the second robot successfully avoided falling off the grid
        Assert.False(secondRobot.IsLost);
    }
}
