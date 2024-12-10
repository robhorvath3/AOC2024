using System.Data;
using System.Drawing;
using System.IO;
using System.Text;

namespace Day10_2
{
    // removing the distinct code from part 1 (that you added to fix part 1)
    // to answer to part 2.
    // oh well.

    internal class Program
    {
        internal enum Dir
        {
            Up,
            Down,
            Left,
            Right,
        }

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

            public Coord Move(Dir direction)
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
        }

        static List<int[]> Map = new List<int[]>();
        static Coord MaxCoord = new Coord();

        static int Explore(Coord start_pos, Coord coord, int source_height)
        {
            if (coord.X < 0 || coord.X > MaxCoord.X || coord.Y < 0 || coord.Y > MaxCoord.Y)
            {
                return 0;
            }

            if (source_height != -1 && Map[coord.Y][coord.X] != source_height + 1)
            {
                return 0;
            }

            if (Map[coord.Y][coord.X] == 9)
            {
                return 1;
            }

            return Explore(start_pos, coord.Move(Dir.Up), Map[coord.Y][coord.X]) +
                Explore(start_pos, coord.Move(Dir.Down), Map[coord.Y][coord.X]) +
                Explore(start_pos, coord.Move(Dir.Left), Map[coord.Y][coord.X]) +
                Explore(start_pos,coord.Move(Dir.Right), Map[coord.Y][coord.X]);
        }

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

                        int[] elevations = line.ToCharArray().Select(c => (int)char.GetNumericValue(c)).ToArray();
                        Map.Add(elevations);
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

            Int64 trailhead_score = 0;

            for (int y = 0; y <= MaxCoord.Y; y++)
            {
                for (int x = 0; x <= MaxCoord.X; x++)
                {
                    if (Map[y][x] != 0)
                        continue;

                    Coord pos = new Coord(x, y);
                    trailhead_score += Explore(pos, pos, -1);
                }
            }

            Console.WriteLine($"The cumulative trailhead score is {trailhead_score}");
        }
    }
}