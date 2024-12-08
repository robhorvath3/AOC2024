using System.Data;
using System.Drawing;
using System.IO;
using System.Text;

namespace Day8_2
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

            public Coord(Coord new_coord)
            {
                X = new_coord.X;
                Y = new_coord.Y;
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

            bool[,] antinodes = new bool[MaxCoord.X + 1, MaxCoord.Y + 1];
            
            // et tu, brute force?
            foreach (char antenna in antennas.Keys)
            {
                List<Coord> comparisons = new List<Coord>();

                for (int i = 0; i < antennas[antenna].Count; i++)
                {
                    for (int j = 1; j < antennas[antenna].Count; j++)
                    {
                        if (i == j)
                            continue;

                        if (comparisons.Any(item => item.X == j && item.Y == i))
                            continue;

                        comparisons.Add(new Coord(i, j));

                        Coord dist = antennas[antenna][i].Dist(antennas[antenna][j]);
                        Coord current = new Coord(antennas[antenna][i]);

                        do
                        {
                            antinodes[current.X, current.Y] = true;
                            current.X -= dist.X;
                            current.Y -= dist.Y;
                        } while (current.X >= 0 && current.X <= MaxCoord.X && current.Y >= 0 && current.Y <= MaxCoord.Y);

                        current = new Coord(antennas[antenna][i]);

                        do
                        {
                            antinodes[current.X, current.Y] = true;
                            current.X += dist.X;
                            current.Y += dist.Y;
                        } while (current.X >= 0 && current.X <= MaxCoord.X && current.Y >= 0 && current.Y <= MaxCoord.Y);
                    }
                }
            }

            int antinode_count = 0;
            for (int x = 0; x <= MaxCoord.X; x++)
            {
                for (int y = 0; y <= MaxCoord.Y; y++)
                {
                    if (antinodes[x, y])
                        antinode_count++;
                }
            }

            Console.WriteLine($"There are {antenna_count} antennas with {antinode_count} unique antinodes");
        }
    }
}