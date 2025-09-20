using Snake.Core;

namespace Snake.Maps
{
    public static class MapLoader
    {
        private static readonly char[] PlayerChars = ['A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J'];

        public static Map Load(string mapName)
        {
            string mapPath = Path.Combine(AppContext.BaseDirectory, "Map_files", $"{mapName}.txt");
            var lines = File.ReadAllLines(mapPath);

            int height = lines.Length;
            int width = height == 0 ? 0 : lines.Max(l => l.Length);

            var playerCharsSet = new HashSet<char>(PlayerChars);
            var game = new char[height, width];
            var bodyPositions = new Dictionary<char, List<Position>>();
            var headPositions = new Dictionary<char, Position>();

            for (int y = 0; y < height; y++)
            {
                var line = lines[y];
                for (int x = 0; x < width; x++)
                {
                    char c = x < line.Length ? line[x] : ' ';
                    bool isPlayerBody = playerCharsSet.Contains(c);

                    game[y, x] = (char.IsDigit(c) || isPlayerBody) ? ' ' : c;

                    if (isPlayerBody)
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

                if (!headPositions.TryGetValue(expectedHeadChar, out var headPos))
                {
                    throw new InvalidDataException($"Snake '{bodyChar}' is missing its head '{expectedHeadChar}' in map '{mapName}'.");
                }

                var startBody = bodies.OrderBy(p => Math.Abs(p.Left - headPos.Left) + Math.Abs(p.Top - headPos.Top)).First();
                
                int dx = headPos.Left - startBody.Left;
                int dy = headPos.Top - startBody.Top;
                Directions dir;

                if (dx == 1 && dy == 0) dir = Directions.Right;
                else if (dx == -1 && dy == 0) dir = Directions.Left;
                else if (dx == 0 && dy == 1) dir = Directions.Down;
                else if (dx == 0 && dy == -1) dir = Directions.Up;
                else
                {
                    throw new InvalidDataException($"Snake '{bodyChar}' head is not adjacent to its body in map '{mapName}'.");
                }
                
                snakeStarts.Add((headPos, dir));
            }

            return new Map(game, width, height, snakeStarts);
        }
    }
}