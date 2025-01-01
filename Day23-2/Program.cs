using System.Drawing;
using System.IO;
using System.Text;

namespace Day23_2
{
    internal class Program
    {
        internal class Graph
        {
            List<string> _Vertices = new();
            List<string> _CMax = new();
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

            public void Sort()
            {
                _Vertices.Sort();

                foreach (string v in _Vertices)
                {
                    _Edges[v].Sort();
                }
            }
            
            public List<string> FindMaximumClique()
            {
                Sort();
                Clique(new List<string>(), _Vertices);
                _CMax.Sort();
                return _CMax;
            }

            // HAL Open Science "A Review on Algorithms for Maximum Clique Problem"
            // Qinghua Wu, Jin-Kao Hao
            // https://univ-angers.hal.science/hal-02709508/document
            private void Clique(List<string> C, List<string> P)
            {
                if (C.Count > _CMax.Count)
                    _CMax = C;

                if (C.Count + P.Count > _CMax.Count)
                {
                    List<string> p_i = new List<string>(P);
                    foreach (string p in P)
                    {
                        p_i.Remove(p);
                        List<string> c_i = new List<string>(C);
                        c_i.Add(p);
                        List<string> p_ii = p_i.Intersect(_Edges[p]).ToList();
                        Clique(c_i, p_ii);
                    }
                }
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

            List<string> max_clique = g.FindMaximumClique();
            string password = string.Join(',', max_clique);

            Console.WriteLine($"The maximum clique contains {max_clique.Count} computers, and they create the following password: {password}");
        }
    }
}
