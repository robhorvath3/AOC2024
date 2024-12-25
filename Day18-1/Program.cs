using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Collections.Generic;

namespace Day18_1
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

                if (new_coord.X < 0 || new_coord.X > MaxCoord.X || new_coord.Y < 0 || new_coord.Y > MaxCoord.Y)
                    new_coord.Set(-1, -1);

                return new_coord;
            }
        }
        
        static bool IsMapSquareOpen(Coord square)
        {
            if (square.X < 0 || square.X > MaxCoord.X || square.Y < 0 || square.Y > MaxCoord.Y)
                return false;

            if (Map[square.Y][square.X])
                return true;

            return false;
        }
                        
        internal class Graph
        {
            private int _Vertices;
            private int _Edges;
            private Dictionary<Coord, List<Coord>> _Adjacency = new Dictionary<Coord, List<Coord>>();

            public int V()
            {
                return _Vertices;
            }

            public int E()
            {
                return _Edges;
            }

            public void AddVertex(Coord pos)
            {
                if (!DoesVertexExist(pos))
                {
                    _Adjacency.Add(pos, new List<Coord>());
                    _Vertices++;
                }                
            }

            public void AddEdge(Coord v1, Coord v2)
            {
                if (!DoesVertexExist(v1))
                    AddVertex(v1);

                if (!DoesVertexExist(v2))
                    AddVertex(v2);

                if (!_Adjacency[v1].Contains(v2) && !_Adjacency[v2].Contains(v1))
                {
                    _Adjacency[v1].Add(v2);
                    _Adjacency[v2].Add(v1);

                    _Edges++;
                }
            }

            public bool DoesVertexExist(Coord pos)
            {
                return _Adjacency.ContainsKey(pos);
            }

            public bool DoesEdgeExist(Coord v1, Coord v2)
            {
                return (_Adjacency[v1].Contains(v2) && _Adjacency[v2].Contains(v1));
            }

            public List<Coord> Adj(Coord v)
            {
                return _Adjacency[v];
            }
        }

        internal class BFSShortestPath
        {
            private Dictionary<Coord, bool> _Marked = new Dictionary<Coord, bool>();
            private Dictionary<Coord, Coord> _EdgeTo = new Dictionary<Coord, Coord>();
            private Coord _Source = new Coord();

            public BFSShortestPath(Graph graph, Coord source)
            {
                _Source = source;
                BFS(graph, source);
            }

            private void BFS(Graph graph, Coord source)
            {
                Queue<Coord> queue = new Queue<Coord>();

                if (!_Marked.ContainsKey(source))
                    _Marked.Add(source, true);
                else
                    _Marked[source] = true;
                
                queue.Enqueue(source);

                while (queue.Count > 0)
                {
                    Coord v = queue.Dequeue();

                    foreach (Coord w in graph.Adj(v))
                    {
                        if (!_Marked.ContainsKey(w) || !_Marked[w])
                        {
                            _EdgeTo.Add(w, v);

                            if (!_Marked.ContainsKey(w))
                                _Marked.Add(w, true);
                            else
                                _Marked[w] = true;

                            queue.Enqueue(w);
                        }
                    }
                }
            }

            public bool HasPathTo(Coord dest)
            {
                if (!_Marked.ContainsKey(dest))
                    return false;
                else
                    return _Marked[dest];
            }

            public Stack<Coord> PathTo(Coord dest)
            {
                if (!HasPathTo(dest)) return null;

                Stack<Coord> path = new Stack<Coord>();

                for (Coord c = dest; !c.IsEqual(_Source); c = _EdgeTo[c])
                    path.Push(c);
                path.Push(_Source);
                return path;
            }
        }

        static List<bool[]> Map = new List<bool[]>();
        static List<Coord> CorruptMemory = new List<Coord>();
        static List<Coord> VertexProcessed = new List<Coord>();

        static Coord MaxCoord = new Coord(70, 70);
        static Coord StartPos = new Coord(0, 0);
        static Coord EndPos = new Coord(70, 70);

        static void Main(string[] args)
        {
            using (StreamReader sr = new StreamReader("Input.txt"))
            {
                string? line = "";

                try
                {
                    while (true)
                    {
                        line = sr.ReadLine()?.Trim();
                        if (line == null || line.Length == 0) break;

                        string[] coords = line.Split(',');
                        CorruptMemory.Add(new Coord(int.Parse(coords[0]), int.Parse(coords[1])));
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

            // create the map
            for (int y = 0; y <= MaxCoord.Y; y++)
            {
                bool[] row = new bool[MaxCoord.X + 1];

                for (int x = 0; x <= MaxCoord.X; x++)
                {
                    row[x] = true;
                }

                Map.Add(row);
            }

            // add first 1024 corrupt blocks to the map
            for (int i = 0; i < 1024; i++)
            {
                Map[CorruptMemory[i].Y][CorruptMemory[i].X] = false;
            }

            // create our graph
            Graph g = new Graph();

            // the graph class checks to see if vertices and edges exist already,
            // so we don't need to check here
            for (int y = 0; y <= MaxCoord.Y; y++)
            {
                for (int x = 0; x <= MaxCoord.X; x++)
                {
                    if (Map[y][x])
                    {
                        Coord square_pos = new Coord(x, y);
                        g.AddVertex(square_pos);

                        foreach (var dir in Enum.GetValues<Dir>())
                        {
                            Coord square_in_dir = square_pos.GetCoordInDirection(dir);

                            if (square_in_dir.X != -1 && square_in_dir.Y != -1 && Map[square_in_dir.Y][square_in_dir.X])
                                g.AddEdge(square_pos, square_in_dir);
                        }
                    }
                }
            }

            BFSShortestPath search_paths = new BFSShortestPath(g, StartPos);

            Stack<Coord> path_to_end = search_paths.PathTo(EndPos);

            Console.SetWindowSize(MaxCoord.X + 30, MaxCoord.Y + 10);
            Console.Clear();

            for (int y = 0; y <= MaxCoord.Y; y++)
            {
                for (int x = 0; x <= MaxCoord.X; x++)
                {
                    if (Map[y][x])
                        Console.Write(".");
                    else
                        Console.Write("#");
                }
                Console.Write('\n');
            }

            (int console_x, int console_y) = Console.GetCursorPosition();
            ConsoleColor bg = Console.BackgroundColor;
            ConsoleColor fg = Console.ForegroundColor;

            Console.BackgroundColor = ConsoleColor.Red;
            Console.ForegroundColor = ConsoleColor.White;

            foreach (Coord c in path_to_end)
            {
                Console.SetCursorPosition(c.X, c.Y);
                Console.Write('O');
            }

            Console.BackgroundColor = bg;
            Console.ForegroundColor = fg;

            Console.SetCursorPosition(console_x, console_y);

            Console.WriteLine($"\nThe shortest path from ({StartPos.X}, {StartPos.Y}) to ({EndPos.X}, {EndPos.Y}) has {path_to_end.Count - 1} moves.");
        }
    }
}