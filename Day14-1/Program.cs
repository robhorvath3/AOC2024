using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace Day14_1
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
        
        static void Main(string[] args)
        {
            List<Robot> robots = new List<Robot>();
            Coord ArenaSize = new Coord(101, 103);
            int HorizontalDiv = ArenaSize.Y / 2;
            int VerticalDiv = ArenaSize.X / 2;

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

                int iterations = 100;
                int[] quadrants = new int[4];

                foreach (Robot r in robots)
                {
                    r.FinalPos.X = r.StartPos.X + (r.Velocity.X * iterations) % ArenaSize.X;
                    if (r.FinalPos.X < 0)
                    {
                        r.FinalPos.X = ArenaSize.X + r.FinalPos.X;
                    }
                    else if (r.FinalPos.X > ArenaSize.X - 1)
                    {
                        r.FinalPos.X -= ArenaSize.X;
                    }

                    r.FinalPos.Y = r.StartPos.Y + (r.Velocity.Y * iterations) % ArenaSize.Y;
                    if (r.FinalPos.Y < 0)
                    {
                        r.FinalPos.Y = ArenaSize.Y + r.FinalPos.Y;
                    }
                    else if (r.FinalPos.Y > ArenaSize.Y - 1)
                    {
                        r.FinalPos.Y -= ArenaSize.Y;
                    }

                    if (r.FinalPos.X != VerticalDiv && r.FinalPos.Y != HorizontalDiv)
                    {
                        // quadrant #1
                        if (r.FinalPos.X < VerticalDiv && r.FinalPos.Y < HorizontalDiv)
                        {
                            quadrants[0]++;
                        }
                        // quadrant #2
                        else if (r.FinalPos.X > VerticalDiv && r.FinalPos.Y < HorizontalDiv)
                        {
                            quadrants[1]++;
                        }
                        //quadrant #3
                        else if (r.FinalPos.X < VerticalDiv && r.FinalPos.Y > HorizontalDiv)
                        {
                            quadrants[2]++;
                        }
                        // quadrant #4
                        else
                        {
                            quadrants[3]++;
                        }
                    }
                }

                long safety_factor = (quadrants[0] * quadrants[1] * quadrants[2] * quadrants[3]);
                
                for (int i = 0; i < 4; i++)
                {
                    Console.WriteLine($"Quadrant #{(i + 1)} == {quadrants[i]}");
                }

                Console.WriteLine($"Safety factor (q1 * q2 * q3 * q4) is {safety_factor}");
            }
        }
    }
}
