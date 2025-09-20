using System.Text;
using Snake.Core;
using Snake.Maps;

namespace Snake.Snakes;

class SnakeBot(Position position, Directions direction)
    : SnakeBase(GenerateName(), direction, position, GetRandomColor())
{
    private Queue<Position> _currentPath = new();

    public override Task CalcHeadPosition(Map map, List<Position> apples)
    {
        bool needsNewPath = _currentPath.Count == 0 
                            || !IsTargetValid(_currentPath.Last(), apples) 
                            || !IsPathClear(_currentPath, map);

        if (needsNewPath)
        {
            return Task.Run(() =>
            {
                var newPath = new List<Position>();

                if (apples.Any())
                {
                    var target = ChooseTarget(apples, map);
                    newPath = FindPathPositions(target, map);
                }

                if (newPath.Count == 0)
                {
                    var safeMove = FindSafeMove(map);
                    newPath.Add(safeMove);
                }

                _currentPath = new Queue<Position>(newPath);
                HeadPosition = _currentPath.Dequeue();
            });
        }

        if (_currentPath.Count > 0)
        {
            HeadPosition = _currentPath.Dequeue();
        }

        return Task.CompletedTask;
    }

    private Position FindSafeMove(Map map)
    {
        var head = HeadPosition;
        var candidates = new List<Position>
        {
            new Position(head.Top - 1, head.Left), // Up
            new Position(head.Top + 1, head.Left), // Down
            new Position(head.Top, head.Left - 1), // Left
            new Position(head.Top, head.Left + 1)  // Right
        };

        var safeMoves = candidates.Where(map.IsPositionFree).ToList();

        if (safeMoves.Any())
        {
            return safeMoves[Random.Shared.Next(safeMoves.Count)];
        }

        // PANIC: No safe moves are available. Move forward into doom.
        return Direction switch
        {
            Directions.Up => candidates[0],
            Directions.Down => candidates[1],
            Directions.Left => candidates[2],
            Directions.Right => candidates[3],
            _ => head // Default case, should not be reached
        };
    }
    private bool IsTargetValid(Position target, List<Position> apples) =>
        apples.Any(a => a.Left == target.Left && a.Top == target.Top);

    private bool IsPathClear(Queue<Position> path, Map map)
    {
        foreach (var pos in path)
        {
            if (!map.IsPositionFree(pos))
                return false;
        }
        return true;
    }

    private Position ChooseTarget(List<Position> apples, Map map)
    {
        var evaluations = new List<(Position Target, List<Position> Path)>();
        foreach (var apple in apples)
        {
            var path = FindPathPositions(apple, map);
            if (path.Count > 0)
            {
                evaluations.Add((apple, path));
            }
        }
        
        if (!evaluations.Any())
        {
            return apples.First();
        }

        var bestTarget = evaluations
            .OrderBy(e => e.Path.Count)
            .ThenByDescending(e => CountFreeSpaceAround(e.Target, map))
            .First()
            .Target;

        return bestTarget;
    }

    private int CountFreeSpaceAround(Position pos, Map map)
    {
        int freeSpace = 0;
        if (map.IsPositionFree(new Position(pos.Top - 1, pos.Left))) freeSpace++;
        if (map.IsPositionFree(new Position(pos.Top + 1, pos.Left))) freeSpace++;
        if (map.IsPositionFree(new Position(pos.Top, pos.Left - 1))) freeSpace++;
        if (map.IsPositionFree(new Position(pos.Top, pos.Left + 1))) freeSpace++;
        return freeSpace;
    }


    private List<Position> FindPathPositions(Position target, Map map, double weight = 1.5)
    {
        var path = new List<Position>();
        var startPos = HeadPosition;
        
        var allNodes = new Dictionary<(int, int), PathLocation>();
        
        var openList = new PriorityQueue<PathLocation, int>();
        var closedSet = new HashSet<(int, int)>();
        
        var startNode = new PathLocation 
        { 
            X = startPos.Left, 
            Y = startPos.Top,
            G = 0
        };
        startNode.H = ComputeHScore(startNode.X, startNode.Y, target.Left, target.Top, weight);
        startNode.F = startNode.G + startNode.H;

        allNodes[(startNode.X, startNode.Y)] = startNode;
        openList.Enqueue(startNode, startNode.F);
        
        PathLocation? current = null;


        while (openList.Count > 0)
        {
            current = openList.Dequeue();
            
            if (current.X == target.Left && current.Y == target.Top)
            {
                break; 
            }

            closedSet.Add((current.X, current.Y));

            foreach (var neighborCoord in GetWalkableAdjacentCoordinates(current.X, current.Y, map))
            {
                if (closedSet.Contains(neighborCoord))
                {
                    continue;
                }
                
                if (!allNodes.TryGetValue(neighborCoord, out var neighbor))
                {
                    neighbor = new PathLocation { X = neighborCoord.X, Y = neighborCoord.Y };
                    allNodes[neighborCoord] = neighbor;
                }
                
                int tentativeG = current.G + 1;
                
                if (tentativeG < neighbor.G)
                {
                    neighbor.Parent = current;
                    neighbor.G = tentativeG;
                    neighbor.H = ComputeHScore(neighbor.X, neighbor.Y, target.Left, target.Top, weight);
                    neighbor.F = neighbor.G + neighbor.H;
                    
                    openList.Enqueue(neighbor, neighbor.F);
                }
            }
        }
        
        if (current != null && current.X == target.Left && current.Y == target.Top)
        {
            while (current != null && current.Parent != null)
            {
                path.Add(new Position(current.Y, current.X));
                current = current.Parent;
            }
            path.Reverse();
        }

        return path;
    }

    private IEnumerable<(int X, int Y)> GetWalkableAdjacentCoordinates(int x, int y, Map map)
    {
        // Check Up
        if (map.IsPositionFree(new Position(y - 1, x)))
            yield return (x, y - 1);
        // Check Down
        if (map.IsPositionFree(new Position(y + 1, x)))
            yield return (x, y + 1);
        // Check Left
        if (map.IsPositionFree(new Position(y, x - 1)))
            yield return (x - 1, y);
        // Check Right
        if (map.IsPositionFree(new Position(y, x + 1)))
            yield return (x + 1, y);
    }

    private int ComputeHScore(int x, int y, int targetX, int targetY, double weight = 1.5)
    {
        // Weighted Manhattan distance
        return (int)(weight * (Math.Abs(targetX - x) + Math.Abs(targetY - y)));
    }
    

    private static string GenerateName()
    {
        char[] vowel = ['A', 'E', 'I', 'O', 'U'];
        char[] cnst =
        [
            'B', 'C', 'D', 'F', 'G', 'H', 'J', 'K', 'L', 'M', 'N',
            'P', 'Q', 'R', 'S', 'T', 'V', 'W', 'X', 'Y', 'Z'
        ];

        int nameLength = Random.Next(3, 7);
        int starterLetter = Random.Next(0, 2);
        bool aIsSet = false;

        StringBuilder sr = new StringBuilder();

        if (starterLetter == 0)
        {
            sr.Append(vowel[Random.Next(0, vowel.Length)]);
            aIsSet = true;
        }
        else sr.Append(cnst[Random.Next(0, cnst.Length)]);

        for (int i = 0; i < nameLength; i++)
        {
            if (!aIsSet)
            {
                sr.Append(vowel[Random.Next(0, vowel.Length)]);
                aIsSet = true;
            }
            else
            {
                sr.Append(cnst[Random.Next(0, cnst.Length)]);
                aIsSet = false;
            }
        }

        return sr.ToString();
    }

    private static ConsoleColor GetRandomColor() =>
        (ConsoleColor)Random.Next(1, 16);
}
