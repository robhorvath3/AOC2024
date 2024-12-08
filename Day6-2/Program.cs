using System.ComponentModel;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace Day6_2
{
    // brute-force? yup.

    internal class Program
    {
        internal enum Dir
        {
            Up = 0,
            Down = 1,
            Left = 2,
            Right = 3,
            ENUM_COUNT,
        }
        
        internal enum VisitStatus
        {
            None = 0,
            Perm = 1,
            Temp = 2,
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

            public Coord PeekNextSquare(Dir direction)
            {
                switch (direction)
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

        internal class Guard
        {
            public Coord Pos = new Coord();
            Dir Direction = Dir.Up;
            VisitStatus VisitStatus = VisitStatus.Perm;
            Coord SavedPos = new Coord();
            Dir SavedDirection = Dir.Up;

            public Guard(Coord pos)
            {
                Pos.X = pos.X; Pos.Y = pos.Y;
            }

            public int Begin()
            {
                int loops = 0;

                MarkVisit();

                while (true)
                {
                    while (IsBlocked())
                        Turn();

                    Coord next_pos = PeekNextPos();

                    if (!Map.IsOutOfBounds(next_pos) && !Map.IsObstacle(next_pos) && !Map.HasVisits(next_pos))
                    {
                        // Save our position
                        SavePos();

                        // Change to temp mode
                        VisitStatus = VisitStatus.Temp;

                        // Place a temp obstacle at the next position
                        Map.PlaceTempObstacle(next_pos);

                        while (true)
                        {
                            while (IsBlocked())
                                Turn();

                            Move();
                            //Console.WriteLine($"Executed {VisitStatus} move to ({Pos.X}, {Pos.Y})");

                            if (IsOutOfBounds())
                            {
                                RestorePos();
                                Map.ClearTempVisits();
                                Map.ClearTempObstacle();
                                VisitStatus = VisitStatus.Perm;
                                break;
                            }
                            else if (Map.HasVisitsInDirection(Pos, Direction))
                            {
                                loops++;

                                RestorePos();
                                Map.ClearTempVisits();
                                Map.ClearTempObstacle();
                                VisitStatus = VisitStatus.Perm;
                                break;
                            }
                            else
                            {
                                MarkVisit();
                            }
                        }
                    }

                    Move();
                    //Console.WriteLine($"Executed {VisitStatus} move to ({Pos.X}, {Pos.Y})");

                    if (IsOutOfBounds())
                    {
                        break;
                    }
                    else
                    {
                        MarkVisit();
                    }
                }

                return loops;
            }
            
            public Coord PeekNextPos()
            {
                return Pos.PeekNextSquare(Direction);
            }

            public bool IsBlocked()
            {
                Coord next_pos = PeekNextPos();

                if (Map.IsOutOfBounds(next_pos))
                {
                    return false;
                }

                if (Map.Squares != null && Map.Squares[next_pos.Y, next_pos.X].IsObstacle)
                {
                    return true;
                }

                return false;
            }

            public bool IsOutOfBounds()
            {
                return Map.IsOutOfBounds(Pos);               
            }

            public void Turn()
            {
                switch (Direction)
                {
                    case Dir.Up:
                        Direction = Dir.Right;
                        break;
                    case Dir.Down:
                        Direction = Dir.Left;
                        break;
                    case Dir.Left:
                        Direction = Dir.Up;
                        break;
                    case Dir.Right:
                        Direction = Dir.Down;
                        break;
                    default:
                        throw new NotImplementedException();
                }
            }

            public void Move()
            {
                switch (Direction)
                {
                    case Dir.Up:
                        Pos.Y--;
                        break;
                    case Dir.Down:
                        Pos.Y++;
                        break;
                    case Dir.Left:
                        Pos.X--;
                        break;
                    case Dir.Right:
                        Pos.X++;
                        break;
                    default:
                        throw new NotImplementedException();
                }                
            }

            public void MarkVisit()
            {
                if (Map.Squares != null && Pos.X >= 0 && Pos.X <= Map.MapMax.X && Pos.Y >= 0 && Pos.Y <= Map.MapMax.Y)
                {
                    Map.Squares[Pos.Y, Pos.X].Visits[(int)Direction] = VisitStatus;
                }
            }

            public void SavePos()
            {
                SavedPos.Set(Pos);
                SavedDirection = Direction;
            }

            public void RestorePos()
            {
                Pos.Set(SavedPos);
                Direction = SavedDirection;
            }
        }

        internal class MapSquare
        {
            public bool IsObstacle = false;

            public VisitStatus[] Visits = new VisitStatus[(int)Dir.ENUM_COUNT];

            public MapSquare()
            {
                for (int d = (int)Dir.Up; d <= (int)Dir.Right; d++)
                {
                    Visits[d] = VisitStatus.None;
                }
            }

            public void ClearTemporaryVisits()
            {
                for (int d = (int)Dir.Up; d <= (int)Dir.Right; d++)
                {
                    if (Visits[d] == VisitStatus.Temp)
                    {
                        Visits[d] = VisitStatus.None;
                    }
                }
            }
        }
        
        internal static class Map
        {
            public static MapSquare[,]? Squares = null;
            public static Coord TempObstaclePos = new Coord();
            public static Coord MapMax = new Coord();

            public static Coord InjestMap(List<string> raw_map)
            {
                Coord guard_pos = new Coord();
                Squares = new MapSquare[raw_map.Count, raw_map[0].Length];
                MapMax.Set(raw_map[0].Length - 1, raw_map.Count - 1);

                for (int y = 0; y < raw_map.Count; y++)
                {
                    for (int x = 0; x < raw_map[y].Length; x++)
                    {
                        Squares[y, x] = new MapSquare();

                        if (raw_map[y][x] == '^')
                        {
                            guard_pos.Set(x, y);
                        }
                        else if (raw_map[y][x] == '#')
                        {
                            Squares[y, x].IsObstacle = true;
                        }
                    }
                }

                return guard_pos;
            }

            public static void PlaceTempObstacle(Coord temp_obstacle_coord)
            {
                if (Squares != null)
                {
                    Squares[temp_obstacle_coord.Y, temp_obstacle_coord.X].IsObstacle = true;
                    TempObstaclePos = temp_obstacle_coord;
                }
            }

            public static void ClearTempObstacle()
            {
                if (Squares != null)
                {
                    Squares[TempObstaclePos.Y, TempObstaclePos.X].IsObstacle = false;
                }
            }

            public static void ClearTempVisits()
            {
                if (Squares == null) { return; }

                for (int y = 0; y <= MapMax.Y; y++)
                {
                    for (int x = 0; x <= MapMax.X; x++)
                    {
                        Squares[y, x].ClearTemporaryVisits();
                    }
                }
            }

            public static int CountVisitedSquares()
            {
                int visited_squares = 0;

                for (int y = 0; y <= MapMax.Y; y++)
                {
                    for (int x = 0; x <= MapMax.X; x++)
                    {
                        if (HasVisits(new Coord(x, y)))
                            visited_squares++;
                    }
                }

                return visited_squares;
            }
            
            public static bool HasVisits(Coord pos)
            {
                if (Squares == null) { return false; }

                for (int d = (int)Dir.Up; d <= (int)Dir.Right; d++)
                {
                    if (Squares[pos.Y, pos.X].Visits[d] != VisitStatus.None)
                    {
                        return true;
                    }
                }

                return false;
            }

            public static bool HasVisitsInDirection(Coord pos, Dir direction)
            {
                if (Squares == null) { return false; }
                                
                if (Squares[pos.Y, pos.X].Visits[(int)direction] != VisitStatus.None)
                {
                    return true;
                }
                
                return false;
            }

            public static bool IsObstacle(Coord pos)
            {
                if (Squares == null) { return false; }

                return Squares[pos.Y, pos.X].IsObstacle;
            }

            public static bool IsOutOfBounds(Coord pos)
            {

                if (pos.X < 0 || pos.X > MapMax.X || pos.Y < 0 || pos.Y > MapMax.Y)
                {
                    return true;
                }

                return false;
            }
        }

        static void Main(string[] args)
        {
            List<string> MapInput = new List<string>();
            Guard? g = null;

            using (StreamReader sr = new StreamReader("Input.txt"))
            {
                try
                {
                    string? line = "";

                    // read input map
                    while (true)
                    {
                        line = sr.ReadLine()?.Trim();
                        if (line == null || line.Length == 0) break;

                        MapInput.Add(line);
                    }

                    // PROCESS MAP INPUT
                    Coord guard_pos = Map.InjestMap(MapInput);
                    g = new Guard(guard_pos);
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

            if (g != null)
            {
                int loops = g.Begin();

                Console.WriteLine($"You can place obstacles in {loops} squares to create a loop (guard visited {Map.CountVisitedSquares()} squares)");
            }
        }
    }
}
