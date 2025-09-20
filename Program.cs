using Snake.Core;
using Snake.Maps;

namespace Snake;

static class Program
{

    static async Task Main(string[] args)
    {
        var map = MapLoader.Load("accuracy_map");
        var game = new GameManager([], 4, map);
        await game.StartAsync();
    }
}
