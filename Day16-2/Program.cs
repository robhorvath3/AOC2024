using System.Data;
using System.IO;
using System.Text;
using System.Collections.Generic;

// this one was hard; I stubbornly stuck with a two priority queue based Dijkstra, and did not track directions
// once moving to a single queue and tracking position and direction, the Dijkstra works as expected
namespace Day16_2
{
    internal class Program
    {
        internal enum Dir
        {
            Up = 0,
            Down,
            Left,
            Right,
        }

        internal struct Coord
        {
            public int X;
            public int Y;

            public Coord()
            {
                X = -1; Y = -1;
            }

            public Coord(int x, int y)
            {
                X = x; Y = y;
            }

            public Coord(Coord pos)
            {
                X = pos.X;
                Y = pos.Y;
            }

            public void Set(int x, int y)
            {
                X = x; Y = y;
            }

            public void Set(Coord new_coord)
            {
                X = new_coord.X;
                Y = new_coord.Y;
            }

            public bool IsEqual(Coord other_coord)
            {
                if (other_coord.X == X && other_coord.Y == Y)
                    return true;

                return false;
            }

            public Coord GetCoordInDirection(Dir direction)
            {
                Coord new_coord = new Coord(this);

                switch (direction)
                {
                    case Dir.Up:
                        new_coord.Y--;
                        break;
                    case Dir.Down:
                        new_coord.Y++;
                        break;
                    case Dir.Left:
                        new_coord.X--;
                        break;
                    case Dir.Right:
                        new_coord.X++;
                        break;
                    default:
                        throw new NotImplementedException();
                }

                return new_coord;
            }
        }

        internal static Dir ClockwiseDir(Dir dir)
        {
            switch (dir)
            {
                case Dir.Up:
                    return Dir.Right;
                case Dir.Down:
                    return Dir.Left;
                case Dir.Left:
                    return Dir.Up;
                case Dir.Right:
                    return Dir.Down;
                default:
                    throw new NotImplementedException();
            }
        }

        internal static bool IsClockwiseDir(Dir entering_dir, Dir exiting_dir)
        {
            if (entering_dir == exiting_dir) return false;

            Dir clockwise = ClockwiseDir(entering_dir);

            if (exiting_dir == clockwise) return true;

            return false;
        }

        internal static Dir CounterClockwiseDir(Dir dir)
        {
            switch (dir)
            {
                case Dir.Up:
                    return Dir.Left;
                case Dir.Down:
                    return Dir.Right;
                case Dir.Left:
                    return Dir.Down;
                case Dir.Right:
                    return Dir.Up;
                default:
                    throw new NotImplementedException();
            }
        }

        internal static bool IsCounterClockwiseDir(Dir entering_dir, Dir exiting_dir)
        {
            if (entering_dir == exiting_dir) return false;

            Dir ccw = CounterClockwiseDir(entering_dir);

            if (exiting_dir == ccw) return true;

            return false;
        }

        internal static int GetCost(Dir entering_dir, Dir exiting_dir)
        {
            if (entering_dir == exiting_dir)
                return 1;

            if (IsClockwiseDir(entering_dir, exiting_dir) || IsCounterClockwiseDir(entering_dir, exiting_dir))
                return 1001;

            return 2001;
        }

        static bool IsSquareOpen(Coord square)
        {
            if (Map[square.Y][square.X] == '.' || Map[square.Y][square.X] == 'E' || Map[square.Y][square.X] == 'S')
                return true;

            return false;
        }

        internal static Dictionary<(Coord, Dir), int> Visited = new Dictionary<(Coord, Dir), int>();
        internal static PriorityQueue<NodeInfo, int> Queue = new PriorityQueue<NodeInfo, int>();
        internal static HashSet<Coord> ShortestPaths = new HashSet<Coord>();
        internal static int ShortestPathLength = 0;

        internal class NodeInfo
        {
            public Coord Pos;
            public Dir Direction;
            public HashSet<Coord> Moves = new HashSet<Coord>();

            public NodeInfo(Coord pos, Dir direction, HashSet<Coord> moves)
            {
                this.Pos = pos;
                this.Direction = direction;
                this.Moves = moves;
            }
        }

        internal static List<char[]> Map = new List<char[]>();
        internal static Coord MaxCoord = new Coord();
        internal static Coord StartPos = new Coord();
        internal static Coord EndPos = new Coord();

        static void Main(string[] args)
        {
            using (StreamReader sr = new StreamReader("Input.txt"))
            {
                try
                {
                    string[] lines = sr.ReadToEnd().Split("\r\n");
                    MaxCoord.Set(lines[0].Length - 1, lines.Length - 1);

                    for (int y = 0; y < lines.Length; y++)
                    {
                        if (lines[y].IndexOf('S') != -1)
                            StartPos.Set(lines[y].IndexOf('S'), y);

                        if (lines[y].IndexOf('E') != -1)
                            EndPos.Set(lines[y].IndexOf('E'), y);

                        Map.Add(lines[y].ToCharArray());
                    }
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

            ShortestPaths.Add(StartPos);
            Queue.Enqueue(new NodeInfo(StartPos, Dir.Right, []), 0);

            while (Queue.TryDequeue(out NodeInfo current_node, out int current_cost))
            {
                if (current_node.Pos.IsEqual(EndPos))
                {
                    if (ShortestPathLength == 0)
                        ShortestPathLength = current_cost;

                    if (current_cost > ShortestPathLength)
                        break;

                    ShortestPaths.UnionWith(current_node.Moves);
                }

                if (Visited.GetValueOrDefault((current_node.Pos, current_node.Direction), int.MaxValue) < current_cost)
                    continue;

                Visited[(current_node.Pos, current_node.Direction)] = current_cost;

                Coord forward = current_node.Pos.GetCoordInDirection(current_node.Direction);
                if (IsSquareOpen(forward))
                {
                    HashSet<Coord> new_moves = new HashSet<Coord>(current_node.Moves) { forward };
                    Queue.Enqueue(new NodeInfo(forward, current_node.Direction, new_moves), current_cost + 1);
                }

                Dir cw_dir = ClockwiseDir(current_node.Direction);
                Coord cw = current_node.Pos.GetCoordInDirection(cw_dir);
                if (IsSquareOpen(cw))
                {
                    HashSet<Coord> new_moves = new HashSet<Coord>(current_node.Moves) { cw };
                    Queue.Enqueue(new NodeInfo(cw, cw_dir, new_moves), current_cost + 1001);
                }

                Dir ccw_dir = CounterClockwiseDir(current_node.Direction);
                Coord ccw = current_node.Pos.GetCoordInDirection(ccw_dir);
                if (IsSquareOpen(ccw))
                {
                    HashSet<Coord> new_moves = new HashSet<Coord>(current_node.Moves) { ccw };
                    Queue.Enqueue(new NodeInfo(ccw, ccw_dir, new_moves), current_cost + 1001);
                }
            }

            Console.WriteLine($"The number of squares on all shortest paths is {ShortestPaths.Count}");
        }
    }
}