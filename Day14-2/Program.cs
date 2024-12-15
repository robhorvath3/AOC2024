using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using static Day14_2.Program;

namespace Day14_2
{
    internal class Program
    {
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
        }

        internal class Robot
        {
            public Coord StartPos;
            public Coord Velocity;
            public Coord FinalPos;
            public Coord MaxCoord;

            public Robot(int pos_x, int pos_y, int vel_x, int vel_y)
            {
                StartPos = new Coord(pos_x, pos_y);
                Velocity = new Coord(vel_x, vel_y);
                FinalPos = new Coord(pos_x, pos_y);
            }
        }

        static List<Robot> robots = new List<Robot>();
        static Coord ArenaSize = new Coord(101, 103);

        static void SetRobotPositions(int Seconds)
        {
            foreach (Robot r in robots)
            {
                r.FinalPos.X = r.StartPos.X + (r.Velocity.X * Seconds) % ArenaSize.X;
                if (r.FinalPos.X < 0)
                {
                    r.FinalPos.X = ArenaSize.X + r.FinalPos.X;
                }
                else if (r.FinalPos.X > ArenaSize.X - 1)
                {
                    r.FinalPos.X -= ArenaSize.X;
                }

                r.FinalPos.Y = r.StartPos.Y + (r.Velocity.Y * Seconds) % ArenaSize.Y;
                if (r.FinalPos.Y < 0)
                {
                    r.FinalPos.Y = ArenaSize.Y + r.FinalPos.Y;
                }
                else if (r.FinalPos.Y > ArenaSize.Y - 1)
                {
                    r.FinalPos.Y -= ArenaSize.Y;
                }                
            }
        }

        static void Main(string[] args)
        {
            using (StreamReader sr = new StreamReader("Input.txt"))
            {
                string? line = "";

                try
                {
                    while (true)
                    {
                        line = sr.ReadLine()?.Trim();
                        if (line == null || line.Length == 0) break;

                        string[] robot_stats = line.Split(' ');
                        string[] pos = robot_stats[0].Substring(robot_stats[0].IndexOf('=') + 1).Split(',');
                        string[] vel = robot_stats[1].Substring(robot_stats[1].IndexOf('=') + 1).Split(',');

                        robots.Add(new Robot(int.Parse(pos[0]), int.Parse(pos[1]), int.Parse(vel[0]), int.Parse(vel[1])));
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

                int elapsed_seconds = 0;

                Console.SetWindowSize(ArenaSize.X + 10, ArenaSize.Y + 10);
                
                List<string> blank_screen_rows = new List<string>();

                for (int y = 0; y < ArenaSize.Y; y++)
                {
                    StringBuilder row = new StringBuilder();

                    for (int x = 0; x < ArenaSize.X; x++)
                    {
                        row.Append('.');
                    }

                    blank_screen_rows.Add(row.ToString());
                }

                while (true)
                {
                    elapsed_seconds++;

                    SetRobotPositions(elapsed_seconds);

                    Dictionary<Coord, int> robot_positions = new Dictionary<Coord, int>();

                    foreach (Robot r in robots)
                    {
                        try
                        {
                            robot_positions.Add(r.FinalPos, 1);
                        }
                        catch
                        {

                        }
                    }

                    if (robot_positions.Count == robots.Count)
                        break;
                }

                Console.Clear();
                foreach (string row in blank_screen_rows)
                {
                    Console.WriteLine(row);
                }

                SetRobotPositions(elapsed_seconds);

                foreach (Robot r in robots)
                {
                    Console.SetCursorPosition(r.FinalPos.X, r.FinalPos.Y);
                    Console.Write('#');
                }

                Console.SetCursorPosition(0, ArenaSize.Y + 2);
                Console.WriteLine($"Seconds elapsed: {elapsed_seconds}");
            }
        }
    }
}
