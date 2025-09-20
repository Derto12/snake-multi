using Snake.Core;
using Snake.Maps;

namespace Snake.Snakes;
public interface ISnake
{
    Queue<Position> Positions { get; }
    bool Alive { get; set; }
    int Length { get; }
    
    Task CalcHeadPosition(Map map, List<Position> apples);
    void MoveBody();
    Position GetHeadPosition();
    Position GetTailPrevPosition();
    void Grow(int amount);
    void ClearFromScreen();
    void Print();
    void Reset();
}
