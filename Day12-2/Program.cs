using System.Data;
using System.Drawing;
using System.IO;
using System.Text;

namespace Day12_2
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

        internal struct MapSquare
        {
            public bool[] Borders;

            public MapSquare()
            {
                Borders = new bool[4];
            }
        }

        internal struct Region
        {
            public int Perimeter;
            public int Area;
            public List<Coord> Squares = new List<Coord>();

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

            if (InputMap[map_square.Y][map_square.X] != crop)
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
                r.Squares.Add(map_square);
            }

            r.Area++;

            if (up.Y < 0)
            {
                r.Perimeter++;
                Squares[map_square.X, map_square.Y].Borders[(int)Dir.Up] = true;
            }
            else
            {
                if (InputMap[up.Y][up.X] != crop)
                {
                    r.Perimeter++;
                    Squares[map_square.X, map_square.Y].Borders[(int)Dir.Up] = true;
                }
                else
                {
                    AnalyzeSquare(up, ref r, crop);
                }
            }

            if (down.Y > MaxCoord.Y)
            {
                r.Perimeter++;
                Squares[map_square.X, map_square.Y].Borders[(int)Dir.Down] = true;
            }
            else
            {
                if (InputMap[down.Y][down.X] != crop)
                {
                    r.Perimeter++;
                    Squares[map_square.X, map_square.Y].Borders[(int)Dir.Down] = true;
                }
                else
                {
                    AnalyzeSquare(down, ref r, crop);
                }
            }

            if (left.X < 0)
            {
                r.Perimeter++;
                Squares[map_square.X, map_square.Y].Borders[(int)Dir.Left] = true;
            }
            else
            {
                if (InputMap[left.Y][left.X] != crop)
                {
                    r.Perimeter++;
                    Squares[map_square.X, map_square.Y].Borders[(int)Dir.Left] = true;
                }
                else
                {
                    AnalyzeSquare(left, ref r, crop);
                }
            }

            if (right.X > MaxCoord.X)
            {
                r.Perimeter++;
                Squares[map_square.X, map_square.Y].Borders[(int)Dir.Right] = true;
            }
            else
            {
                if (InputMap[right.Y][right.X] != crop)
                {
                    r.Perimeter++;
                    Squares[map_square.X, map_square.Y].Borders[(int)Dir.Right] = true;
                }
                else
                {
                    AnalyzeSquare(right, ref r, crop);
                }
            }
        }

        internal static int CountSides(Region region)
        {
            int sides = 0;

            foreach (Coord map_coord in region.Squares)
            {
                if (Squares[map_coord.X, map_coord.Y].Borders[(int)Dir.Left] && Squares[map_coord.X, map_coord.Y].Borders[(int)Dir.Up])
                    sides++;

                if (Squares[map_coord.X, map_coord.Y].Borders[(int)Dir.Left] && Squares[map_coord.X, map_coord.Y].Borders[(int)Dir.Down])
                    sides++;

                if (Squares[map_coord.X, map_coord.Y].Borders[(int)Dir.Right] && Squares[map_coord.X, map_coord.Y].Borders[(int)Dir.Up])
                    sides++;

                if (Squares[map_coord.X, map_coord.Y].Borders[(int)Dir.Right] && Squares[map_coord.X, map_coord.Y].Borders[(int)Dir.Down])
                    sides++;

                // |_
                if (region.Squares.Contains(new Coord(map_coord.X, map_coord.Y - 1)) &&
                    region.Squares.Contains(new Coord(map_coord.X + 1, map_coord.Y)) &&
                    !region.Squares.Contains(new Coord(map_coord.X + 1, map_coord.Y - 1)))
                    sides++;

                // _|
                if (region.Squares.Contains(new Coord(map_coord.X, map_coord.Y - 1)) &&
                    region.Squares.Contains(new Coord(map_coord.X - 1, map_coord.Y)) &&
                    !region.Squares.Contains(new Coord(map_coord.X - 1, map_coord.Y - 1)))
                    sides++;

                // r
                if (region.Squares.Contains(new Coord(map_coord.X, map_coord.Y + 1)) &&
                    region.Squares.Contains(new Coord(map_coord.X + 1, map_coord.Y)) &&
                    !region.Squares.Contains(new Coord(map_coord.X + 1, map_coord.Y + 1)))
                    sides++;

                // 7
                if (region.Squares.Contains(new Coord(map_coord.X, map_coord.Y + 1)) &&
                    region.Squares.Contains(new Coord(map_coord.X - 1, map_coord.Y)) &&
                    !region.Squares.Contains(new Coord(map_coord.X - 1, map_coord.Y + 1)))
                    sides++;
            }

            return sides;
        }

        static List<char[]> InputMap = new List<char[]>();
        static Coord MaxCoord = new Coord();
        static List<Coord> CoordsToAnalyze = new List<Coord>();
        static MapSquare[,] Squares;

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
                        InputMap.Add(crops);
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

            Squares = new MapSquare[MaxCoord.X + 1, MaxCoord.Y + 1];

            // Add all of the coordinates to the list
            for (int x = 0; x <= MaxCoord.X; x++)
            {
                for (int y = 0; y <= MaxCoord.Y; y++)
                {
                    CoordsToAnalyze.Add(new Coord(x, y));
                    Squares[x, y] = new MapSquare();
                }
            }

            Int64 fence_cost = 0;

            while (CoordsToAnalyze.Count > 0)
            {
                Region r = new Region();
                Coord starting_square = new Coord(CoordsToAnalyze[0].X, CoordsToAnalyze[0].Y);
                
                AnalyzeSquare(CoordsToAnalyze[0], ref r, InputMap[starting_square.Y][starting_square.X]);
                
                int sides = CountSides(r);
                int region_fence_cost = r.Area * sides;

                Console.WriteLine($"Region starting at ({starting_square.X}, {starting_square.Y}) has {sides} sides and an area of {r.Area} for a cost of {region_fence_cost}");
                fence_cost += region_fence_cost;
            }

            Console.WriteLine($"The total cost of fencing the crop regions on the map is {fence_cost}");
        }
    }
}