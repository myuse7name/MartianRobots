using MartianRobots.Domain;

namespace MartianRobots.Services;

public sealed class RobotSimulator
{
    public void ExecuteInstructions(Robot robot, World world, string instructions)
    {
        foreach (var instruction in instructions)
        {
            if (robot.IsLost)
            {
                return;
            }

            switch (instruction)
            {
                case 'L':
                    TurnLeft(robot);
                    break;

                case 'R':
                    TurnRight(robot);
                    break;

                case 'F':
                    MoveForward(robot, world);
                    break;

                default:
                    throw new InvalidOperationException(
                        $"Unsupported instruction: {instruction}");
            }
        }
    }

    public void TurnLeft(Robot robot)
    {
        robot.Orientation = robot.Orientation switch
        {
            Orientation.North => Orientation.West,
            Orientation.West => Orientation.South,
            Orientation.South => Orientation.East,
            Orientation.East => Orientation.North,
            _ => throw new InvalidOperationException()
        };
    }

    public void TurnRight(Robot robot)
    {
        robot.Orientation = robot.Orientation switch
        {
            Orientation.North => Orientation.East,
            Orientation.East => Orientation.South,
            Orientation.South => Orientation.West,
            Orientation.West => Orientation.North,
            _ => throw new InvalidOperationException()
        };
    }

    public Position GetForwardPosition(Robot robot)
    {
        return robot.Orientation switch
        {
            Orientation.North => new Position(
                robot.Position.X,
                robot.Position.Y + 1),

            Orientation.East => new Position(
                robot.Position.X + 1,
                robot.Position.Y),

            Orientation.South => new Position(
                robot.Position.X,
                robot.Position.Y - 1),

            Orientation.West => new Position(
                robot.Position.X - 1,
                robot.Position.Y),

            _ => throw new InvalidOperationException()
        };
    }

    public void MoveForward(Robot robot, World world)
    {
        var nextPosition = GetForwardPosition(robot);

        var isOutsideWorld =
            nextPosition.X < 0 ||
            nextPosition.X > world.MaxX ||
            nextPosition.Y < 0 ||
            nextPosition.Y > world.MaxY;

        if (!isOutsideWorld)
        {
            robot.Position = nextPosition;
            return;
        }

        var scent = new Scent(robot.Position);

        if (world.Scents.Contains(scent))
        {
            return;
        }

        world.Scents.Add(scent);
        robot.IsLost = true;
    }
}