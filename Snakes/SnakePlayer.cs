using Snake.Core;
using Snake.Maps;

namespace Snake.Snakes;

public class SnakePlayer(ConsoleColor color, string name, SnakeControls controls, Position position, Directions direction)
    : SnakeBase(name, direction, position, color)
{
    public SnakeControls Controls { get; set; } = controls;

    public void ChangeDirection(ConsoleKey key)
    {
        Direction = Controls.GetDirection(key);
    }
    
    public override Task CalcHeadPosition(Map map, List<Position> apples)
    {
        HeadPosition = HeadPosition.MoveToDirection(Direction);
        return Task.CompletedTask;
    }
}
