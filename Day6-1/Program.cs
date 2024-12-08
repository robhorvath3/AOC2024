using System.ComponentModel;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace Day6_1
{
    internal class Program
    {
        internal enum Dir
        {
            Up,
            Down,
            Left,
            Right,
        }

        internal class Guard
        {
            public Coord Pos;
            public Dir Direction;
            public Coord ArenaMax;
            int _VisitedSquares = 1;
            bool[,]? _Visited = null;
            public List<string> Map = new List<string>();

            public Guard()
            {
                Pos = new Coord(0, 0);
                Direction = Dir.Up;
                ArenaMax = new Coord(0, 0);
            }

            public class Coord
            {
                public int X = 0;
                public int Y = 0;

                public Coord(int x, int y)
                {
                    X = x; Y = y;
                }
            }

            public void Move()
            {
                switch (Direction)
                {
                    case Dir.Up:
                        Pos.Y--;
                        break;
                    case Dir.Down:
                        Pos.Y++;
                        break;
                    case Dir.Left:
                        Pos.X--;
                        break;
                    case Dir.Right:
                        Pos.X++;
                        break;
                }
            }

            public void Turn()
            {
                switch (Direction)
                {
                    case Dir.Up:
                        Direction = Dir.Right;
                        break;
                    case Dir.Down:
                        Direction = Dir.Left;
                        break;
                    case Dir.Left:
                        Direction = Dir.Up;
                        break;
                    case Dir.Right:
                        Direction = Dir.Down;
                        break;
                }
            }

            public bool IsBlocked()
            {
                switch (Direction)
                {
                    case Dir.Up:
                        if (Pos.Y - 1 < 0)
                            return false;
                        if (Map[Pos.Y - 1][Pos.X] == '#')
                            return true;
                        else
                            return false;
                    case Dir.Down:
                        if (Pos.Y + 1 > ArenaMax.Y)
                            return false;
                        if (Map[Pos.Y + 1][Pos.X] == '#')
                            return true;
                        else
                            return false;
                    case Dir.Left:
                        if (Pos.X - 1 < 0)
                            return false;
                        if (Map[Pos.Y][Pos.X - 1] == '#')
                            return true;
                        else
                            return false;
                    case Dir.Right:
                        if (Pos.X + 1 > ArenaMax.X)
                            return false;
                        if (Map[Pos.Y][Pos.X + 1] == '#')
                            return true;
                        else
                            return false;
                    default:
                        throw new Exception("WTF Exception - should not occur");
                }
            }

            public bool IsOutOfBounds()
            {
                if (Pos.X > ArenaMax.X || Pos.X < 0 || Pos.Y > ArenaMax.Y || Pos.Y < 0)
                {
                    return true;
                }

                return false;
            }

            public void AddMapRow(string row)
            {
                Map.Add(row);
                ArenaMax.X = row.Length - 1;
                ArenaMax.Y = Map.Count - 1;
                
                if (row.Contains('^'))
                {
                    Pos.X = row.IndexOf('^');
                    Pos.Y = Map.Count - 1;
                }
            }

            public void NotifyMapComplete()
            {
                _Visited = new bool[Map.Count, Map[0].Length];
            }

            public void IncVisitedSquares()
            {
                if (_Visited != null && _Visited[Pos.Y, Pos.X] == false)
                {
                    _VisitedSquares++;
                    _Visited[Pos.Y, Pos.X] = true;
                }
            }

            public int GetVisitedSquares()
            {
                return _VisitedSquares;
            }
        }

        static void Main(string[] args)
        {
            Guard g = new Guard();

            using (StreamReader sr = new StreamReader("Input.txt"))
            {
                try
                {
                    string? line = "";

                    // read input map
                    while (true)
                    {
                        line = sr.ReadLine()?.Trim();
                        if (line == null || line.Length == 0) break;

                        g.AddMapRow(line);
                    }
                    g.NotifyMapComplete();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Exception: {ex.Message}");
                }
                finally
                {
                    sr.Close();
                }
            }

            while (true)
            {
                while (g.IsBlocked())
                    g.Turn();

                g.Move();

                if (g.IsOutOfBounds())
                    break;
                else
                    g.IncVisitedSquares();
            }

            Console.WriteLine($"The guard visited {g.GetVisitedSquares()} squares");
        }
    }
}
