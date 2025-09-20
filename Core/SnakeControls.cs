namespace Snake.Core;

public struct SnakeControls(ConsoleKey up, ConsoleKey down, ConsoleKey left, ConsoleKey right)
{
    public Directions? GetDirection(ConsoleKey key)
    {
        return key switch
        {
            _ when key == up    => Directions.Up,
            _ when key == down  => Directions.Down,
            _ when key == left  => Directions.Left,
            _ when key == right => Directions.Right,
            _ => null
        };
    }
}