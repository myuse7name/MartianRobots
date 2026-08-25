# Martian Robots

A C#/.NET solution for the Martian Robots programming challenge.

## Requirements

- .NET 10 SDK

## Running the Application

The application reads the challenge input from standard input and writes one result per robot.

When running from the Visual Studio debugger, the program displays prompts and examples. Enter the world size, the number of robots, and each robot's starting position and instructions. The program then prints the robot results.


## Approach

The solution is split into separate responsibilities:

- Domain models represent the core business concepts.
- RobotSimulator contains the movement and navigation logic.
- InputParser is responsible for converting the input format into domain objects.
- Unit tests verify robot movement, loss detection and scent behaviour.

## Architecture

- Domain contains the core business models.
- Services contains robot movement logic.
- Parsing handles transformation of input text into domain objects.
- Tests verify all core business rules.


### Separation

Movement logic is isolated within 'RobotSimulator' to keep business rules separate from application input/output concerns.

### Extensibility

The challenge notes that additional command types may be added in the future.

The simulator keeps command dispatch in one place. Adding a command means adding a case there, with unsupported commands rejected explicitly rather than silently ignored.

### Scent Handling

Scents are stored in the 'World' object as a collection of positions:

This ensures dangerous moves from a previously lost position can be identified and ignored by future robots, regardless of their orientation.

## Assumptions

- Input is valid according to the challenge specification.
- Coordinates are within the limits defined by the challenge.
- Instruction sets only contain supported commands ('L', 'R', 'F').

## Testing

The test suite verifies:

- Turning left
- Moving forward
- Moving out of bounds
- Robot loss detection
- Scent generation
- Scent reuse by subsequent robots
- Expected results from the challenge examples

## Future Improvements

If this were developed further I would:

- Add validation and error handling
- Add structured logging
- Add integration tests

For a production-facing version, I would keep this simulation as a library behind a small HTTP API or a batch job interface. 
Requests and results could be stored in a relational database if an audit history were needed; the current challenge does not require persistence because each run owns its world state. 

