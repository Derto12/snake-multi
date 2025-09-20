using System.Collections.Concurrent;
using System.Diagnostics;
using Snake.Maps;
using Snake.Snakes;

namespace Snake.Core;

public class GameManager
{
    private readonly Map _map;
    private List<Position> _apples = new();
    private static readonly Random Random = new();
    private readonly ConcurrentQueue<ConsoleKey> _inputQueue = new();

    private List<ISnake> _snakes = new();
    private int GameSpeed { get; set; } = 100;
    private bool GameEnded { get; set; }

    public GameManager(List<SnakePlayer> players, int botCount, Map map)
    {
        _map = map;
        AddSnakes(players, botCount);
    }

    private async Task CaptureInputAsync()
    {
        while (!GameEnded)
        {
            var key = Console.ReadKey(true).Key;
            _inputQueue.Enqueue(key);
            await Task.Delay(5);
        }
    }

    private void ProcessInputQueue()
    {
        while (_inputQueue.TryDequeue(out var key))
        {
            foreach (var s in _snakes.OfType<SnakePlayer>())
            {
                s.ChangeDirection(key);
            }

            if (key == ConsoleKey.Escape)
                GameEnded = true;
        }
    }


    private void AddSnakes(List<SnakePlayer> players, int botCount)
    {
        int count = 0;
        for (; count < _map.MaxPlayers && count < players.Count; count++)
        {
            var (pos, dir) = _map.SnakeStarts[count];
            var player = players[count];
            _snakes.Add(new SnakePlayer(player.Color, player.Name, player.Controls, pos, dir));
        }
        for (; count < _map.MaxPlayers && count < botCount; count++)
        {
            var (pos, dir) = _map.SnakeStarts[count];
            _snakes.Add(new SnakeBot(pos, dir));
        }
    }

    private void GenerateApples(int count)
    {
        for (int i = 0; i < count; i++)
        {
            int left = -1, top = -1;
            bool success = false;
            while (!success)
            {
                top =  Random.Next(1, _map.Height);
                left = Random.Next(1, _map.Width);

                success = _map.IsPositionFree(top, left);
            }
            _apples.Add(new Position(top, left));
        }
    }

    private void WriteApples()
    {
        foreach (var apple in _apples)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.SetCursorPosition(apple.Left, apple.Top);
            Console.Write("O");
        }
    }

    private async Task GameLoopAsync()
    {
        _map.Render();
        GenerateApples(3);
        WriteApples();

        while (!GameEnded)
        {
            var watch = Stopwatch.StartNew();

            ProcessInputQueue();

            // move snakes in parallel
            await Task.WhenAll(_snakes.Select(s => s.CalcHeadPosition(_map, _apples)));

            // check deaths in parallel
            await Task.WhenAll(_snakes.Select(CheckIfKilled));
            
            Console.SetCursorPosition(0, _map.Height + 1);

            CleanUpSnakes(_snakes.Where(s => !s.Alive));
            _snakes = _snakes.Where(s => s.Alive).ToList();

            if (_snakes.Count == 0) GameEnded = true;

            foreach (var snake in _snakes)
            {
                snake.MoveBody();
                _map.SetPosition(snake.GetHeadPosition(), 'X');
                _map.SetPosition(snake.GetTailPrevPosition(), ' ');
                snake.Print();
            }
            
            CheckForApples();
            GenerateApples(3 - _apples.Count);
            WriteApples();

            watch.Stop();
            var delay = GameSpeed - (int)watch.ElapsedMilliseconds;
            if (delay > 0) await Task.Delay(delay);
        }
    }



    private bool IsKilled(ISnake snake)
    {
        var headPosition = snake.GetHeadPosition();
        if (!_map.IsPositionFree(headPosition)) return true;
        
        var otherSnakeHeads = _snakes.Where(s => s != snake)
            .Select(s => s.GetHeadPosition());
        
        return otherSnakeHeads.Contains(headPosition);
    }

    private Task CheckIfKilled(ISnake snake)
    {
        if (IsKilled(snake))
        {
            snake.Alive = false;
        }
        return Task.CompletedTask;
    }

    private void CleanUpSnakes(IEnumerable<ISnake> snakes)
    {
        foreach (var snake in snakes)
        {
            _map.FreePositions(snake.Positions);
            snake.ClearFromScreen();
        }
    }

    private void CheckForApples()
    {
        var remainderApples = new List<Position>();
        foreach (var apple in _apples)
        {
            var snake = _snakes.Find(s => s.GetHeadPosition() == apple);
            if (snake == null) remainderApples.Add(apple);
            else snake.Grow(1);
        }
        
        _apples = remainderApples;
    }
    
    public async Task StartAsync()
    {
        var moveSnakesAsync = GameLoopAsync();
        var captureInputAsync =  CaptureInputAsync();
        
        await Task.WhenAll(moveSnakesAsync, captureInputAsync);
    }

    public void Reset()
    {
        GameEnded = false;

        foreach (var snake in _snakes)
        {
            snake.Reset();
        }
        _apples.Clear();
    }
}