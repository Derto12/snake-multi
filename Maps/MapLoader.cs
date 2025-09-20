using Snake.Core;

namespace Snake.Maps
{
    public static class MapLoader
    {
        private static readonly char[] PlayerChars = { 'A','B','C','D','E','F','G','H','I','J' };

        public static Map Load(string mapName)
        {
            string mapPath = Path.Combine(AppContext.BaseDirectory, "Map_files", $"{mapName}.txt");
            var lines = File.ReadAllLines(mapPath ?? throw new FileNotFoundException(mapPath));

            int height = lines.Length;
            int width = height == 0 ? 0 : lines.Max(l => l.Length);
            
            var raw = new char[height, width];
            var game = new char[height, width];
            
            var bodyPositions = new Dictionary<char, List<Position>>();
            var headPositions = new Dictionary<char, Position>();

            for (int y = 0; y < height; y++)
            {
                var line = lines[y] ?? string.Empty;
                for (int x = 0; x < width; x++)
                {
                    char c = x < line.Length ? line[x] : ' ';
                    raw[y, x] = c;
                    game[y, x] = (char.IsDigit(c) || Array.IndexOf(PlayerChars, c) >= 0) ? ' ' : c;
                    
                    if (Array.IndexOf(PlayerChars, c) >= 0)
                    {
                        if (!bodyPositions.TryGetValue(c, out var list))
                        {
                            list = new List<Position>();
                            bodyPositions[c] = list;
                        }
                        list.Add(new Position(y, x));
                    }
                    else if (char.IsDigit(c))
                    {
                        headPositions[c] = new Position(y, x);
                    }
                }
            }
            
            var snakeStarts = new List<(Position, Directions)>();
            foreach (var kv in bodyPositions.OrderBy(k => k.Key))
            {
                char bodyChar = kv.Key;
                var bodies = kv.Value;
                int playerIndex = bodyChar - 'A';
                char expectedHeadChar = (char)('0' + playerIndex);
                
                Position startBody = bodies[0];
                Directions dir;

                if (headPositions.TryGetValue(expectedHeadChar, out var headPos))
                {
                    startBody = bodies.OrderBy(p => Math.Abs(p.Left - headPos.Left) + Math.Abs(p.Top - headPos.Top)).First();
                    
                    if (headPos.Left > 0 && raw[headPos.Top, headPos.Left - 1] == bodyChar) dir = Directions.Right;
                    else if (headPos.Left < width - 1 && raw[headPos.Top, headPos.Left + 1] == bodyChar) dir = Directions.Left;
                    else if (headPos.Top > 0 && raw[headPos.Top - 1, headPos.Left] == bodyChar) dir = Directions.Down;
                    else if (headPos.Top < height - 1 && raw[headPos.Top + 1, headPos.Left] == bodyChar) dir = Directions.Up;
                    else
                    {
                        int dx = headPos.Left - startBody.Left;
                        int dy = headPos.Top - startBody.Top;
                        if (Math.Abs(dx) >= Math.Abs(dy)) dir = dx > 0 ? Directions.Right : Directions.Left;
                        else dir = dy > 0 ? Directions.Down : Directions.Up;
                    }
                }
                else
                {
                    startBody = bodies[0];
                    dir = Directions.Right;
                }

                snakeStarts.Add((startBody, dir));
            }
            
            return new Map(game, width, height, snakeStarts);
        }
    }
}
