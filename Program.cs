using Snake.Core;
using Snake.Maps;
using Snake.Snakes;

namespace Snake;

static class Program
{

    static async Task Main(string[] args)
    {
        var map = MapLoader.Load("accuracy_map");
        
        var controls = new SnakeControls(ConsoleKey.UpArrow, ConsoleKey.DownArrow,
            ConsoleKey.LeftArrow, ConsoleKey.RightArrow);
        var player1 = new SnakePlayer(ConsoleColor.Cyan, "Joe", controls);
        
        
        var game = new GameManager([player1], 3, map);
        await game.StartAsync();
        Console.ForegroundColor = ConsoleColor.Green;
    }
}
