using System.Data;
using System.Drawing;
using System.IO;
using System.Text;

namespace Day12_1
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

            public void Set(int x, int y)
            {
                X = x; Y = y;
            }

            public void Set(Coord other_coord)
            {
                X = other_coord.X;
                Y = other_coord.Y;
            }

            public Coord GetCoordInDir(Dir direction)
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

        internal struct Region
        {
            public int Perimeter;
            public int Area;

            public Region()
            {
                Perimeter = 0;
                Area = 0;
            }
        }

        static void AnalyzeSquare(Coord map_square, ref Region r, char crop)
        {
            Coord up = map_square.GetCoordInDir(Dir.Up);
            Coord down = map_square.GetCoordInDir(Dir.Down);
            Coord left = map_square.GetCoordInDir(Dir.Left);
            Coord right = map_square.GetCoordInDir(Dir.Right);

            if (Map[map_square.Y][map_square.X] != crop)
            {
                return;
            }

            if (!CoordsToAnalyze.Contains(map_square))
            {
                return;
            }
            else
            {
                CoordsToAnalyze.Remove(map_square);
            }

            r.Area++;
            
            if (up.Y < 0)
            {
                r.Perimeter++;
            }
            else
            {
                if (Map[up.Y][up.X] != crop)
                {
                    r.Perimeter++;
                }
                else
                {
                    AnalyzeSquare(up, ref r, crop);
                }
            }

            if (down.Y > MaxCoord.Y)
            {
                r.Perimeter++;
            }
            else
            {
                if (Map[down.Y][down.X] != crop)
                {
                    r.Perimeter++;
                }
                else
                {
                    AnalyzeSquare(down, ref r, crop);
                }
            }

            if (left.X < 0)
            {
                r.Perimeter++;
            }
            else
            {
                if (Map[left.Y][left.X] != crop)
                {
                    r.Perimeter++;
                }
                else
                {
                    AnalyzeSquare(left, ref r, crop);
                }
            }

            if (right.X > MaxCoord.X)
            {
                r.Perimeter++;
            }
            else
            {
                if (Map[right.Y][right.X] != crop)
                {
                    r.Perimeter++;
                }
                else
                {
                    AnalyzeSquare(right, ref r, crop);
                }
            }
        }

        static List<char[]> Map = new List<char[]>();
        static Coord MaxCoord = new Coord();
        static List<Coord> CoordsToAnalyze = new List<Coord>();

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

                        char[] crops = line.ToCharArray();
                        Map.Add(crops);
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

            // Add all of the coordinates to the list
            for (int x = 0; x <= MaxCoord.X; x++)
            {
                for (int y = 0; y <= MaxCoord.Y; y++)
                {
                    CoordsToAnalyze.Add(new Coord(x, y));
                }
            }
            
            Int64 fence_cost = 0;

            while (CoordsToAnalyze.Count > 0)
            {
                Region r = new Region();
                AnalyzeSquare(CoordsToAnalyze[0], ref r, Map[CoordsToAnalyze[0].Y][CoordsToAnalyze[0].X]);
                fence_cost += r.Area * r.Perimeter;
            }

            Console.WriteLine($"The total cost of fencing the crop regions on the map is {fence_cost}");
        }
    }
}