using Snake.Core;
using Snake.Maps;

namespace Snake.Snakes;

public class SnakePlayer: SnakeBase
{
    public SnakeControls Controls { get; set; }

    public SnakePlayer(ConsoleColor color, string name, SnakeControls controls, Position position, Directions direction)
        : base(name, direction, position, color)
    {
        Controls = controls;
    }

    public SnakePlayer(ConsoleColor color, string name, SnakeControls controls) 
        : base(name, Directions.Right, new Position(0, 0), color)
    {
        Controls = controls;
    }

    public void ChangeDirection(ConsoleKey key)
    {
        var dir = Controls.GetDirection(key);
        if (dir.HasValue) Direction = dir.Value;
    }
    
    public override Task CalcHeadPosition(Map map, List<Position> apples)
    {
        HeadPosition = HeadPosition.MoveToDirection(Direction);
        return Task.CompletedTask;
    }
}
