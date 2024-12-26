using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Day20_2
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

        internal struct Coord : IEquatable<Coord>
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

            public void Set(Coord coord)
            {
                X = coord.X;
                Y = coord.Y;
            }
            public int Manhattan(Coord other_coord)
            {
                return Math.Abs(X - other_coord.X) + Math.Abs(Y - other_coord.Y);
            }

            public override bool Equals(object? obj) => obj is Coord other && this.Equals(other);

            public bool Equals(Coord other_coord)
            {
                if (other_coord.X == X && other_coord.Y == Y)
                    return true;

                return false;
            }

            public override int GetHashCode() => (X, Y).GetHashCode();

            public static bool operator ==(Coord a, Coord b)
            {
                if (a.X == b.X && a.Y == b.Y)
                    return true;

                return false;
            }

            public static bool operator !=(Coord a, Coord b) => !(a == b);

            public bool IsDirBackward(Dir original_dir, Dir new_dir)
            {
                switch (original_dir)
                {
                    case Dir.Up:
                        return new_dir == Dir.Down;
                    case Dir.Down:
                        return new_dir == Dir.Up;
                    case Dir.Left:
                        return new_dir == Dir.Right;
                    case Dir.Right:
                        return new_dir == Dir.Left;
                    default:
                        throw new NotImplementedException();
                }
            }
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

            public void RemoveVertex(Coord pos)
            {
                if (DoesVertexExist(pos))
                {
                    if (_Adjacency.ContainsKey(pos))
                    {
                        Dictionary<Coord, List<Coord>> vertices_to_remove = new Dictionary<Coord, List<Coord>>();

                        foreach (Coord adj in _Adjacency[pos])
                        {
                            if (!vertices_to_remove.ContainsKey(pos))
                                vertices_to_remove.Add(pos, new List<Coord>());

                            vertices_to_remove[pos].Add(adj);
                            
                            if (_Adjacency.ContainsKey(adj) && _Adjacency[adj].Contains(pos))
                            {
                                if (!vertices_to_remove.ContainsKey(adj))
                                    vertices_to_remove.Add(adj, new List<Coord>());

                                vertices_to_remove[adj].Add(pos);
                            }
                        }

                        foreach ((Coord v, List<Coord> neighbors) in vertices_to_remove)
                        {
                            foreach (Coord neighbor in neighbors)
                            {
                                _Adjacency[v].Remove(neighbor);
                                _Edges--;
                            }

                            if (_Adjacency[v].Count == 0)
                                _Adjacency.Remove(v);                            
                        }
                    }

                    _Vertices--;
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

                for (Coord c = dest; !(c == _Source); c = _EdgeTo[c])
                    path.Push(c);
                path.Push(_Source);
                return path;
            }
        }

        // cleaned up the Map class too
        internal class Map
        {
            private char _start_char, _end_char;
            private char _empty_char, _wall_char;
            private List<bool[]> _Squares = new List<bool[]>();
            private List<Coord> _Walls = new List<Coord>();
            private Coord _MaxCoord = new Coord();
            public Coord StartPos = new Coord();
            public Coord EndPos = new Coord();

            public Map(char start_char, char end_char, char empty_char, char wall_char)
            {
                _start_char = start_char;
                _end_char = end_char;
                _empty_char = empty_char;
                _wall_char = wall_char;
            }

            public bool IsPosValid(Coord pos)
            {
                if (pos.X < 0 || pos.X > _MaxCoord.X || pos.Y < 0 || pos.Y > _MaxCoord.Y) 
                    return false;

                return true;
            }

            public void AddWall(Coord pos)
            {
                if (!IsPosValid(pos)) return;
                
                _Squares[pos.Y][pos.X] = false;
                
                if (!_Walls.Contains(pos))
                    _Walls.Add(pos);
            }

            public void RemoveWall(Coord pos)
            {
                if (!IsPosValid(pos)) return;

                _Squares[pos.Y][pos.X] = true;

                if (_Walls.Contains(pos))
                    _Walls.Remove(pos);
            }

            public bool IsWall(Coord pos)
            {
                if (!IsPosValid(pos)) return true;

                return !_Squares[pos.Y][pos.X];
            }

            public List<Coord> GetCopyOfWallCoords()
            {
                return new List<Coord>(_Walls);
            }

            public int MaxX()
            {
                return _MaxCoord.X;
            }

            public int MaxY()
            {
                return _MaxCoord.Y;
            }

            public Coord MaxCoord()
            {
                return _MaxCoord;
            }

            public Coord GetCoordInDirection(Coord src, Dir direction)
            {
                Coord new_coord = new Coord();
                new_coord.Set(src);

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

                if (new_coord.X < 0 || new_coord.X > _MaxCoord.X || new_coord.Y < 0 || new_coord.Y > _MaxCoord.Y)
                    new_coord.Set(-1, -1);

                return new_coord;
            }

            public (bool, Coord) IsWallAndShouldRemove(Coord current_pos, Dir direction)
            {
                if (!IsPosValid(current_pos)) return (false, new Coord());

                Coord wall_coord = GetCoordInDirection(current_pos, direction);
                Coord wall_coord_2deep = GetCoordInDirection(wall_coord, direction);

                if (!IsWall(wall_coord))
                    return (false, new Coord());

                // wall going 2 deep is a no-go
                if (IsWall(wall_coord_2deep))
                    return (false, new Coord());

                return (true, wall_coord);
            }

            public void Add(string new_map_row)
            {
                int new_row_index = _Squares.Count;
                bool[] bool_row = new bool[new_map_row.Length];

                if (new_map_row.IndexOf(_start_char) != -1)
                    StartPos.Set(new_map_row.IndexOf(_start_char), new_row_index);

                if (new_map_row.IndexOf(_end_char) != -1)
                    EndPos.Set(new_map_row.IndexOf(_end_char), new_row_index);

                for (int x = 0; x < new_map_row.Length; x++)
                {
                    if (new_map_row[x] == _empty_char || new_map_row[x] == _start_char || new_map_row[x] == _end_char)
                        bool_row[x] = true;
                    else if (new_map_row[x] == _wall_char)
                    {
                        bool_row[x] = false;
                        _Walls.Add(new Coord(x, new_row_index));
                    }
                }

                _Squares.Add(bool_row);
                if (_MaxCoord.X < 0) _MaxCoord.X = new_map_row.Length - 1;
                _MaxCoord.Y = _Squares.Count - 1;
            }

            public bool this[Coord c]
            {
                get => _Squares[c.Y][c.X];
                set => _Squares[c.Y][c.X] = value;
            }
        }
        
        static void Main(string[] args)
        {
            // construct the map object
            Map map = new Map('S', 'E', '.', '#');

            // read input
            using (StreamReader sr = new StreamReader("Input.txt"))
            {
                try
                {
                    string[] lines = sr.ReadToEnd().Split("\r\n");

                    foreach (string line in lines)
                        map.Add(line);
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

            // create our graph
            Graph g = new Graph();

            // the graph class checks to see if vertices and edges exist already,
            // so we don't need to check here
            for (int y = 0; y <= map.MaxY(); y++)
            {
                for (int x = 0; x <= map.MaxX(); x++)
                {
                    Coord square_pos = new Coord(x, y);

                    if (map[square_pos])
                    {                        
                        g.AddVertex(square_pos);

                        foreach (var dir in Enum.GetValues<Dir>())
                        {
                            Coord square_in_dir = map.GetCoordInDirection(square_pos, dir);

                            if (square_in_dir.X != -1 && square_in_dir.Y != -1 && map[square_in_dir])
                                g.AddEdge(square_pos, square_in_dir);
                        }
                    }
                }
            }
                
            // get our baseline stats

            // Recaculate the shortest paths
            BFSShortestPath baseline_paths = new BFSShortestPath(g, map.StartPos);

            // See if we have a path to the end
            Stack<Coord> path_to_end = baseline_paths.PathTo(map.EndPos);
            
            if (path_to_end == null)
            {
                Console.WriteLine($"There was no path from the start square to the end square");
                return;
            }

            // stats & counters
            Coord[] path = path_to_end.ToArray();
            int picos_to_save = 100;
            long paths_that_save_enough_picos = 0;

            // find the paths from each node that will save
            // us a min of picos_to_save + 1 (101) picoseconds;
            for (int i = 0; i < path.Length - (picos_to_save + 1); i++)
            {
                // find the manhattan distances to all points on the path
                // that could possibly save at least picos_to_save + 1 (101)
                // picoseconds (each cheat will take a min of 1 picosecond)
                for (int j = i + (picos_to_save + 1); j < path.Length; j++)
                {
                    if (i == j) continue; // don't compare against ourselves

                    // picosecond savings by jumping to a square is (j - i)
                    // so if the savings less the cost of the cheat (manhattan)
                    // is >= picos_to_save (100), we have a winner
                    int m_d = path[i].Manhattan(path[j]);

                    if (m_d > 20) continue; // Manhattan must be a max of 20 picos

                    if ((j - i) - m_d >= picos_to_save)
                        paths_that_save_enough_picos++;
                }
            }

            Console.WriteLine($"After analyzing {path.Length} squares on the path, we have determined that there are {paths_that_save_enough_picos} shortcuts that saved at least {picos_to_save} picoseconds from the race");
        }
    }
}