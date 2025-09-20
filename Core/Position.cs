namespace Snake.Core;

public readonly record struct Position(int Top, int Left)
{
    public Position MoveToDirection(Directions direction) =>
        direction switch
        {
            Directions.Up => this with { Top = Top - 1 },
            Directions.Down => this with { Top = Top + 1 },
            Directions.Left => this with { Left = Left - 1 },
            Directions.Right => this with { Left = Left + 1 },
            _ => this
        };
}