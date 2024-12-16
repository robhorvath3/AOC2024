using System.Data;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace Day15_2
{
    internal class Program
    {
        internal enum Spot
        {
            Empty,
            Wall,
            BoxLeft,
            BoxRight,
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

            public Coord(Coord other_coord)
            {
                X = other_coord.X;
                Y = other_coord.Y;
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

                        Spot[] spots = new Spot[line.Length * 2];

                        for (int i = 0; i < line.Length; i++)
                        {
                            if (line[i] == '#')
                            {
                                spots[2 * i] = Spot.Wall;
                                spots[(2 * i) + 1] = Spot.Wall;
                            }
                            else if (line[i] == 'O')
                            {
                                spots[2 * i] = Spot.BoxLeft;
                                spots[(2 * i) + 1] = Spot.BoxRight;
                            }
                            else if (line[i] == '.')
                            {
                                spots[2 * i] = Spot.Empty;
                                spots[(2 * i) + 1] = Spot.Empty;
                            }
                            else if (line[i] == '@')
                            {
                                spots[2 * i] = Spot.Empty;
                                spots[(2 * i) + 1] = Spot.Empty;
                                RobotPos.Set(2 * i, y - 1);
                            }
                        }

                        WarehouseSize.Set(line.Length * 2, y);
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
                // right & left we handle the exact same way as 15-1
                else if ((move == Dir.Left || move == Dir.Right) && (Map[next_coord.Y][next_coord.X] == Spot.BoxLeft || Map[next_coord.Y][next_coord.X] == Spot.BoxRight))
                {
                    boxes_to_move.Add(next_coord);
                    Coord next_next_coord = next_coord.GetCoordInDir(move);

                    while (Map[next_next_coord.Y][next_next_coord.X] == Spot.BoxLeft || Map[next_next_coord.Y][next_next_coord.X] == Spot.BoxRight)
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

                        Map[new_box_square.Y][new_box_square.X] = Map[current_box.Y][current_box.X];
                        Map[current_box.Y][current_box.X] = Spot.Empty;
                    }

                    RobotPos.Set(next_coord);
                }
                // for up & down, we make an ascending list of box coordinates that touch the robot & other boxes and then move them in reverse order
                // but only if all boxes that are touched can move (i.e. no wall)
                else if ((move == Dir.Up || move == Dir.Down) && (Map[next_coord.Y][next_coord.X] == Spot.BoxLeft || Map[next_coord.Y][next_coord.X] == Spot.BoxRight))
                {
                    List<Dictionary<Coord, bool>> box_rows = new List<Dictionary<Coord, bool>>();
                    box_rows.Add(new Dictionary<Coord, bool>());

                    // add the robot's position coordinate as row 0
                    int row = 0;
                    box_rows[row].Add(RobotPos, false);
                    
                    while (true)
                    {
                        row++;
                        
                        box_rows.Add(new Dictionary<Coord, bool>());
                        
                        foreach (Coord next_next_coord in box_rows[row - 1].Keys)
                        {
                            Coord next_next_coord2 = new Coord(next_next_coord.GetCoordInDir(move));
                            Coord other_box_coord = new Coord();

                            // make sure this is a box coordinate
                            if (Map[next_next_coord2.Y][next_next_coord2.X] != Spot.BoxLeft && Map[next_next_coord2.Y][next_next_coord2.X] != Spot.BoxRight)
                            {
                                continue;
                            }

                            // get & mark the associated box coordinate to form the pair
                            if (Map[next_next_coord2.Y][next_next_coord2.X] == Spot.BoxLeft)
                            {
                                other_box_coord.Set(next_next_coord2.GetCoordInDir(Dir.Right));
                            }
                            else if (Map[next_next_coord2.Y][next_next_coord2.X] == Spot.BoxRight)
                            {
                                other_box_coord.Set(next_next_coord2.GetCoordInDir(Dir.Left));
                            }
                            else
                            {
                                throw new NotImplementedException();
                            }

                            // get the next coordinates for each box coordinate in the direction of the move
                            Coord next_next_coord2_next = next_next_coord2.GetCoordInDir(move);
                            Coord other_box_coord_next = other_box_coord.GetCoordInDir(move);

                            // short-circuit if there's a wall
                            if (Map[next_next_coord2_next.Y][next_next_coord2_next.X] == Spot.Wall || Map[other_box_coord_next.Y][other_box_coord_next.X] == Spot.Wall)
                            {
                                goto Escape;
                            }
                            
                            // mark the box coordinate and whether or not the next coord (in the direction of the move)
                            // is an open spot or another box (we already made sure it wasn't a wall)
                            if (Map[next_next_coord2_next.Y][next_next_coord2_next.X] == Spot.Empty)
                            {
                                if (!box_rows[row].ContainsKey(next_next_coord2))
                                    box_rows[row].Add(next_next_coord2, true);
                            }
                            else
                            {
                                if (!box_rows[row].ContainsKey(next_next_coord2)) 
                                    box_rows[row].Add(next_next_coord2, false);
                            }

                            if (Map[other_box_coord_next.Y][other_box_coord_next.X] == Spot.Empty)
                            {
                                if (!box_rows[row].ContainsKey(other_box_coord))
                                    box_rows[row].Add(other_box_coord, true);
                            }
                            else
                            {
                                if (!box_rows[row].ContainsKey(other_box_coord))
                                    box_rows[row].Add(other_box_coord, false);
                            }
                        }

                        // we're not done until there's empty spots in front of all touched boxes in the last row
                        // if there's more touched boxes, keep going to the next row outward (in the direction of the move)
                        bool all_clear = true;

                        foreach (bool r in box_rows[row].Values)
                        {
                            if (!r)
                                all_clear = false;
                        }

                        if (all_clear)
                            break;
                    }

                    for (int i = box_rows.Count - 1; i >= 1; i--)
                    {
                        // sanity check -- there should always be an even number of box coordinates (2 per box) in each row
                        if (box_rows[i].Count % 2 != 0)
                            throw new Exception("WTF");

                        // perform the move
                        foreach (Coord coord in box_rows[i].Keys)
                        {
                            Coord dest_coord = coord.GetCoordInDir(move);
                            Map[dest_coord.Y][dest_coord.X] = Map[coord.Y][coord.X];
                            Map[coord.Y][coord.X] = Spot.Empty;
                        }
                    }

                    // move the robot
                    RobotPos.Set(next_coord);

                    // our escape hatch with a dummy variable so the label works
                    Escape:
                    var shit = false;
                }
            }

            Int64 gps = 0;

            // score the GPS
            for (int y = 1; y < WarehouseSize.Y - 1; y++)
            {
                for (int x = 2; x < WarehouseSize.X - 2; x++)
                {
                    if (Map[y][x] == Spot.BoxLeft)
                    {
                        gps += (100 * y) + x;
                    }
                }
            }
                 
            for (int y = 0; y < Map.Count; y++)
            {
                for (int x = 0; x < Map[y].Length; x++)
                {
                    if (RobotPos.X == x && RobotPos.Y == y)
                    {
                        Console.Write('@');
                        continue;
                    }

                    switch (Map[y][x])
                    {
                        case Spot.Empty:
                            Console.Write('.');
                            break;
                        case Spot.BoxLeft:
                            Console.Write('[');
                            break;
                        case Spot.BoxRight:
                            Console.Write(']');
                            break;
                        case Spot.Wall:
                            Console.Write('#');
                            break;
                    }                    
                }
                Console.Write('\n');
            }

            Console.WriteLine($"\nThe gps value of the boxes is {gps}");
        }
    }
}
