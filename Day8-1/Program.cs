using System.Data;
using System.Drawing;
using System.IO;
using System.Text;

namespace Day8_1
{
    internal class Program
    {
        internal class Coord
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

            public void Set(int x, int y)
            {
                X = x; Y = y;
            }

            public void Set(Coord new_coord)
            {
                X = new_coord.X;
                Y = new_coord.Y;
            }

            public Coord Dist(Coord new_coord)
            {
                return new Coord(new_coord.X - X, new_coord.Y - Y);
            }
        }

        static void Main(string[] args)
        {
            Dictionary<char, List<Coord>> antennas = new Dictionary<char, List<Coord>>();
            int antenna_count = 0;
            Coord MaxCoord = new Coord();
            List<Coord> antinodes = new List<Coord>();

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

                        for (int x = 0; x < line.Length; x++)
                        {
                            if (line[x] != '.')
                            {
                                if (!antennas.ContainsKey(line[x]))
                                {
                                    antennas.Add(line[x], new List<Coord>());
                                }

                                antennas[line[x]].Add(new Coord(x, y));
                                antenna_count++;
                            }
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

            // et tu, brute force?
            for (int y = 0; y <= MaxCoord.Y; y++)
            {
                for (int x = 0; x <= MaxCoord.X; x++)
                {
                    Coord pos = new Coord(x, y);

                    foreach (char antenna in antennas.Keys)
                    {
                        List<Coord> distances = new List<Coord>();
                        List<Coord> comparisons = new List<Coord>();

                        foreach(Coord loc in antennas[antenna])
                        {
                            distances.Add(pos.Dist(loc));
                        }

                        for (int i = 0; i < distances.Count; i++)
                        {
                            for (int j = 0; j < distances.Count; j++)
                            {
                                if (i == j)
                                    continue;

                                if (comparisons.Any(item => item.X == j && item.Y == i))
                                    continue;

                                if ((distances[i].X == distances[j].X * 2 && distances[i].Y == distances[j].Y * 2) ||
                                    (distances[j].X == distances[i].X * 2 && distances[j].Y == distances[i].Y * 2))
                                {
                                    if (!antinodes.Any(item => item.X == x && item.Y == y))
                                    {
                                        antinodes.Add(pos);
                                        Console.WriteLine($"Antinode for {antenna} found @ ({x}, {y}) - distance to antenna 1({antenna}) == ({distances[i].X}, {distances[i].Y}) distance to antenna 2({antenna}) == ({distances[j].X}, {distances[j].Y})");
                                    }                                    
                                }

                                comparisons.Add(new Coord(i, j));
                            }
                        }
                    }
                }
            }
            
            Console.WriteLine($"There are {antenna_count} antennas with {antinodes.Count} unique antinodes");
        }
    }
}