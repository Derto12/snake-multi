using Snake.Core;
using Snake.Maps;

namespace Snake.Snakes;

public abstract class SnakeBase(string name, Directions direction, Position position, ConsoleColor color)
    : ISnake
{
    public Queue<Position> Positions { get; } = new();
    protected Position HeadPosition { get; set; } = position;
    private Position TailPrevPosition { get; set; }
    protected Directions Direction { get; set; } = direction;
    public int Length { get; private set; } = 5;
    public ConsoleColor Color { get; init; } = color;

    public bool Alive { get; set; } = true;
    public string Name { get; set; } = name;

    protected static readonly Random Random = new Random();

    private void WriteHead()
    {
        Console.SetCursorPosition(HeadPosition.Left, HeadPosition.Top);
        Console.ForegroundColor = Color;
        Console.Write("■");
    }
    private void EraseLast()
    {
        if (Positions.Count > Length)
        {
            var prevTail = TailPrevPosition;
            Console.SetCursorPosition(prevTail.Left, prevTail.Top);
            Console.Write(" ");
        }
    }

    public void MoveBody()
    {
        if (Positions.Count > Length) TailPrevPosition = Positions.Dequeue();
        Positions.Enqueue(HeadPosition);
    }

    public Position GetHeadPosition()
    {
        return HeadPosition;
    }

    public Position GetTailPrevPosition()
    {
        return TailPrevPosition;
    }

    public void Print()
    {
        WriteHead();
        EraseLast();
    }

    public void ClearFromScreen()
    {
        foreach(var pos in Positions)
        {
            Console.SetCursorPosition(pos.Left, pos.Top);
            Console.Write(" ");
        }
    }
    
    public void Reset()
    {
        Alive = true;
        Length = 5;
        Positions.Clear();
    }
    
    public void Grow(int amount)
    {
        Length += amount;
    }

    public abstract Task CalcHeadPosition(Map map, List<Position> apples);
}
