using System.Drawing;
using System.IO;
using System.Text;

namespace Day23_1
{
    internal class Program
    {
        internal class Graph
        {
            List<string> _Vertices = new();
            Dictionary<string, List<string>> _Edges = new();
            int _EdgeCount = 0;

            public int V()
            {
                return _Vertices.Count();
            }

            public int E()
            {
                return _EdgeCount;
            }

            public List<string> GetVertices()
            {
                return _Vertices;
            }

            public List<string> GetNeighbors(string v)
            {
                return _Edges[v];
            }

            public void AddVertex(string v)
            {
                _Vertices.Add(v);
            }

            public bool DoesVertexExist(string v)
            {
                return _Vertices.Contains(v);
            }

            public void AddEdge(string v1, string v2)
            {
                if (!_Vertices.Contains(v1)) _Vertices.Add(v1);
                if (!_Vertices.Contains(v2)) _Vertices.Add(v2);

                if (!_Edges.ContainsKey(v1)) _Edges.Add(v1, new List<string>());
                if (!_Edges.ContainsKey(v2)) _Edges.Add(v2, new List<string>());

                _Edges[v1].Add(v2);
                _Edges[v2].Add(v1);

                _EdgeCount++;
            }

            public List<(string, string, string)> Find3Cliques()
            {
                List<(string, string, string)> cliques = new();

                _Vertices.Sort();

                foreach (string v in _Vertices)
                {
                    _Edges[v].Sort();
                }

                foreach (string node in _Vertices)
                {
                    foreach (string n_u in _Edges[node])
                    {
                        if (n_u.CompareTo(node) < 0)
                            continue;

                        foreach (string n_v in _Edges[n_u])
                        {
                            if (n_v.CompareTo(n_u) < 0)
                                continue;

                            if (_Edges[n_v].Contains(node))
                                cliques.Add((node, n_u, n_v));
                        }
                    }
                }
                
                return cliques;
            }

            public int Find3CliquesIncludingAComputerStartingWithT()
            {
                int count = 0;

                List<(string, string, string)> cliques = Find3Cliques();
                foreach ((string n1, string n2, string n3) in cliques)
                {
                    if (n1[0] == 't' || n2[0] == 't' || n3[0] == 't')
                        count++;
                }

                return count;
            }
        }

        static void Main(string[] args)
        {
            Graph g = new Graph();

            using (StreamReader sr = new StreamReader("Input.txt"))
            {
                try
                {
                    string[] lines = sr.ReadToEnd().Split("\r\n");
                    
                    foreach (string line in lines)
                    {
                        string[] computers = line.Split('-');
                        g.AddEdge(computers[0], computers[1]);
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

            Console.WriteLine($"There are {g.Find3CliquesIncludingAComputerStartingWithT()} networks of three computers that contain at least one computer who's name starts with a 't'.");
        }
    }
}
