using System.Data;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace Day15_1
{
    internal class Program
    {
        internal enum Spot
        {
            Empty,
            Wall,
            Box,
        }

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

            public Coord GetCoordInDir(Dir dir)
            {
                switch (dir)
                {
                    case Dir.Up:
                        return new Coord(X, Y - 1);
                    case Dir.Down:
                        return new Coord(X, Y + 1);
                    case Dir.Left:
                        return new Coord(X - 1, Y);
                    case Dir.Right:
                        return new Coord(X + 1, Y);
                    default:
                        throw new NotImplementedException();
                }
            }
        }

        internal static List<Spot[]> Map = new List<Spot[]>();
        internal static List<Dir> Moves = new List<Dir>();
        internal static Coord RobotPos = new Coord();

        static void Main(string[] args)
        {
            Coord WarehouseSize = new Coord();

            using (StreamReader sr = new StreamReader("Input.txt"))
            {
                string? line = "";
                int y = 0;

                try
                {
                    while (true)
                    {
                        line = sr.ReadLine()?.Trim();
                        if (line == null || line.Length == 0) break;

                        y++;

                        Spot[] spots = new Spot[line.Length];

                        for (int i = 0; i < line.Length; i++)
                        {
                            if (line[i] == '#')
                            {
                                spots[i] = Spot.Wall;
                            }
                            else if (line[i] == 'O')
                            {
                                spots[i] = Spot.Box;
                            }
                            else if (line[i] == '.')
                            {
                                spots[i] = Spot.Empty;
                            }
                            else if (line[i] == '@')
                            {
                                spots[i] = Spot.Empty;
                                RobotPos.Set(i, y - 1);
                            }
                        }

                        WarehouseSize.Set(line.Length, y);
                        Map.Add(spots);
                    }

                    while (true)
                    {
                        line = sr.ReadLine()?.Trim();
                        if (line == null || line.Length == 0) break;

                        foreach (char c in line)
                        {
                            switch (c)
                            {
                                case '^':
                                    Moves.Add(Dir.Up);
                                    break;
                                case 'v':
                                    Moves.Add(Dir.Down);
                                    break;
                                case '<':
                                    Moves.Add(Dir.Left);
                                    break;
                                case '>':
                                    Moves.Add(Dir.Right);
                                    break;
                                default:
                                    throw new NotImplementedException();
                            }
                        }
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

            foreach (Dir move in Moves)
            {
                List<Coord> boxes_to_move = new List<Coord>();

                Coord next_coord = RobotPos.GetCoordInDir(move);

                if (Map[next_coord.Y][next_coord.X] == Spot.Wall)
                {
                    continue;
                }
                else if (Map[next_coord.Y][next_coord.X] == Spot.Empty)
                {
                    RobotPos.Set(next_coord);
                    continue;
                }
                else if (Map[next_coord.Y][next_coord.X] == Spot.Box)
                {
                    boxes_to_move.Add(next_coord);
                    Coord next_next_coord = next_coord.GetCoordInDir(move);

                    while (Map[next_next_coord.Y][next_next_coord.X] == Spot.Box)
                    {
                        boxes_to_move.Add(next_next_coord);
                        next_next_coord.Set(next_next_coord.GetCoordInDir(move));
                    }

                    if (Map[next_next_coord.Y][next_next_coord.X] != Spot.Empty)
                    {
                        boxes_to_move.Clear();
                        continue;
                    }

                    for (int i = boxes_to_move.Count - 1; i >= 0; i--)
                    {
                        Coord current_box = boxes_to_move[i];
                        Coord new_box_square = current_box.GetCoordInDir(move);

                        Map[current_box.Y][current_box.X] = Spot.Empty;
                        Map[new_box_square.Y][new_box_square.X] = Spot.Box;
                    }

                    RobotPos.Set(next_coord);
                }
            }

            Int64 gps = 0;

            for (int y = 1; y < WarehouseSize.Y - 1; y++)
            {
                for (int x = 1; x < WarehouseSize.X - 1; x++)
                {
                    if (Map[y][x] == Spot.Box)
                    {
                        gps += (100 * y) + x;
                    }
                }
            }
                                
            Console.WriteLine($"The gps value of the boxes is {gps}");
        }
    }
}
