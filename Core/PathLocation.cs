namespace Snake.Core;

public class PathLocation
{
    public int X { get; init; }
    public int Y { get; init; }
    public int G { get; set; } = int.MaxValue;
    public int H { get; set; }
    public int F { get; set; }
    public PathLocation? Parent { get; set; }
}
