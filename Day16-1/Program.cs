using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Collections.Generic;

namespace Day16_1
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
                Coord new_coord = new Coord();
                new_coord.Set(this);

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

            public int Manhattan()
            {
                return Math.Abs(X - EndPos.X) + Math.Abs(Y - EndPos.Y);
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
        
        static bool IsSquareOpen(Coord square)
        {
            if (Map[square.Y][square.X] == '.' || Map[square.Y][square.X] == 'E')
                return true;

            return false;
        }

        static List<char[]> Map = new List<char[]>();
        
        internal class NodeInfo : IComparer<NodeInfo>, IComparable<NodeInfo>
        {
            public int F = 0;
            public int G = 0;
            public int H = 0;
            public Dir Direction;
            public Coord Parent = new Coord();

            public NodeInfo(int f, int g, int h, Dir direction)
            {
                F = f;
                G = g;
                H = h;
                Direction = direction;
            }

            public int Compare(NodeInfo? a, NodeInfo? b)
            {
                if (a == null && b == null)
                    return 0;
                else if (a == null)
                    return -1;
                else if (b == null)
                    return 1;

                return a.F.CompareTo(b.F);
            }

            public int CompareTo(NodeInfo? other)
            {
                if (other == null) return 1;

                return this.F.CompareTo(other.F);
            }
        }

        // for A*
        static PriorityQueue<Coord, NodeInfo> OpenQueue = new PriorityQueue<Coord, NodeInfo>();
        static PriorityQueue<Coord, NodeInfo> ClosedQueue = new PriorityQueue<Coord, NodeInfo>();

        static Coord MaxCoord = new Coord();
        static Coord StartPos = new Coord();
        static Coord EndPos = new Coord();

        static void Main(string[] args)
        {
            using (StreamReader sr = new StreamReader("Input.txt"))
            {
                string? line = "";
                int y = -1;

                try
                {
                    while (true)
                    {
                        line = sr.ReadLine()?.Trim();
                        if (line == null || line.Length == 0) break;

                        y++;

                        if (MaxCoord.X < 0)
                        {
                            MaxCoord.X = line.Length - 1;
                        }

                        char[] squares = line.ToCharArray();
                        Map.Add(squares);

                        if (line.IndexOf('S') != -1)
                        {
                            StartPos.Set(line.IndexOf('S'), y);
                        }

                        if (line.IndexOf('E') != -1)
                        {
                            EndPos.Set(line.IndexOf('E'), y);
                        }
                    }
                    MaxCoord.Y = y;
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

            OpenQueue.Enqueue(StartPos, new NodeInfo(0, 0, 0, Dir.Right));

            while (OpenQueue.Count > 0)
            {
                NodeInfo? node;
                Coord node_coord;
                Coord peek_coord = OpenQueue.Peek();
                OpenQueue.Remove(peek_coord, out node_coord, out node);

                Dictionary<Coord, Dir> successor_list = new Dictionary<Coord, Dir>();

                // forward
                Coord forward = node_coord.GetCoordInDirection(node.Direction);
                if (IsSquareOpen(forward))
                {
                    successor_list.Add(forward, node.Direction);
                }

                // clockwise
                Dir clockwise_dir = ClockwiseDir(node.Direction);
                Coord clockwise = node_coord.GetCoordInDirection(clockwise_dir);
                if (IsSquareOpen(clockwise))
                {
                    successor_list.Add(clockwise, clockwise_dir);
                }

                // counterclockwise
                Dir counterclockwise_dir = CounterClockwiseDir(node.Direction);
                Coord counterclockwise = node_coord.GetCoordInDirection(counterclockwise_dir);
                if (IsSquareOpen(counterclockwise))
                {
                    successor_list.Add(counterclockwise, counterclockwise_dir);
                }

                foreach (Coord successor in successor_list.Keys)
                {
                    bool found = false;

                    if (successor.IsEqual(EndPos))
                        found = true;

                    NodeInfo new_node = new NodeInfo(0, 0, 0, successor_list[successor]);

                    if (successor_list[successor] == node.Direction)
                    {
                        new_node.G = node.G + 1;                        
                    }
                    else
                    {
                        new_node.G = node.G + 1001;
                    }

                    new_node.H = successor.Manhattan();
                    new_node.F = new_node.G + new_node.H;
                    new_node.Parent.Set(node_coord);

                    if (found)
                    {
                        ClosedQueue.Enqueue(node_coord, node);
                        ClosedQueue.Enqueue(successor, new_node);
                        goto Finished;
                    }

                    NodeInfo? successor_open_peek_node;
                    Coord successor_open_peek_coord;

                    if (OpenQueue.Remove(successor, out successor_open_peek_coord, out successor_open_peek_node))
                    {
                        if (successor_open_peek_node.F < new_node.F)
                        {
                            OpenQueue.Enqueue(successor_open_peek_coord, successor_open_peek_node);
                            continue;
                        }
                        else
                        {
                            OpenQueue.Enqueue(successor_open_peek_coord, successor_open_peek_node);
                        }
                    }

                    NodeInfo? successor_closed_peek_node;
                    Coord successor_closed_peek_coord;

                    if (ClosedQueue.Remove(successor, out successor_closed_peek_coord, out successor_closed_peek_node))
                    {
                        if (successor_closed_peek_node.F < new_node.F)
                        {
                            ClosedQueue.Enqueue(successor_closed_peek_coord, successor_closed_peek_node);
                            continue;
                        }
                        else
                        {
                            ClosedQueue.Enqueue(successor_closed_peek_coord, successor_closed_peek_node);
                            OpenQueue.Enqueue(successor, new_node);
                        }
                    }
                    else
                    {
                        OpenQueue.Enqueue(successor, new_node);
                    }
                }

                ClosedQueue.Enqueue(node_coord, node);
            }

            Finished:

            NodeInfo? end_node;
            Coord end_coord;

            if (!ClosedQueue.Remove(EndPos, out end_coord, out end_node))
            {
                Console.WriteLine("Something bad happened: we got to the end, but can't find the ending tile!");
            }
            else
            {
                Console.WriteLine($"Using A* to find the shortest path through the maze, the total score is {end_node.G}.");
            }
        }       
    }
}