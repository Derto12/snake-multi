using Snake.Core;

namespace Snake.Maps;

public class Map(char[,] layout, int width, int height, List<(Position, Directions)> snakeStarts)
{
    public int Width { get; private set; } = width;
    public int Height { get; private set; } = height;

    public int MaxPlayers => SnakeStarts.Count;

    public List<(Position, Directions)> SnakeStarts { get; private set; } = snakeStarts;

    public bool IsPositionFree(Position pos) =>
        layout[pos.Top, pos.Left] == ' ';
    
    public bool IsPositionFree(int top, int left) =>
        layout[top, left] == ' ';

    public char GetValueAt(Position pos) =>
        layout[pos.Top, pos.Left];

    public void SetPosition(Position pos, char value) =>
        layout[pos.Top, pos.Left] = value;

    public void FreePositions(IEnumerable<Position> positions)
    {
        foreach (var pos in positions)
        {
            layout[pos.Top, pos.Left] = ' ';
        }
    }
    
    public void Render()
    {
        Console.Clear();
        Console.CursorVisible = false;

        for (int y = 0; y < Height; y++)
        {
            for (int x = 0; x < Width; x++)
            {
                Console.Write(layout[y, x]);
            }
            Console.WriteLine();
        }
    }
}

