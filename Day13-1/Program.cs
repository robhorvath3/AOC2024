using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace Day13_1
{
    internal struct Button
    {
        public int X;
        public int Y;

        public Button(int x, int y)
        {
            X = x;
            Y = y;
        }
    }

    internal struct Prize
    {
        public int X;
        public int Y;

        public Prize(int x, int y)
        {
            X = x;
            Y = y;
        }
    }

    internal class ClawMachine
    {
        public Button A;
        public Button B;
        public Prize P;

        public ClawMachine(int button_a_x, int button_a_y, int button_b_x, int button_b_y, int prize_x, int prize_y)
        {
            A = new Button(button_a_x, button_a_y);
            B = new Button(button_b_x, button_b_y);
            P = new Prize(prize_x, prize_y);
        }

        public (int, int) Solve()
        {
            for (int n = 100; n >= 0; n--)
            {
                int numerator = (P.X + P.Y) - (n * (B.X + B.Y));
                int denominator = (A.X + A.Y);

                if (numerator % denominator == 0)
                {
                    int m = numerator / denominator;

                    if (m > 100 || m < 0)
                    {
                        continue;
                    }
                    else
                    {
                        if ((m * A.X) + (n * B.X) != P.X)
                        {
                            //Console.WriteLine($"Bad result at (m, n) == ({m}, {n}) for (A.X, B.X) == ({A.X}, {B.X}) != P.X ({P.X})");
                            continue;
                        }

                        if ((m * A.Y) + (n * B.Y) != P.Y)
                        {
                            //Console.WriteLine($"Bad result at (m, n) == ({m}, {n}) for (A.Y, B.Y) == ({A.Y}, {B.Y}) != P.Y ({P.Y})");
                            continue;
                        }

                        return (m, n);
                    }
                }                    
            }

            return (-1, -1);
        }
    }
    
    internal class Program
    {
        static List<ClawMachine> Machines = new List<ClawMachine>();

        static void Main(string[] args)
        {
            using (StreamReader sr = new StreamReader("Input.txt"))
            {
                string? line1 = "";
                string? line2 = "";
                string? line3 = "";
                string? line4 = "";

                try
                {
                    while (true)
                    {
                        line1 = sr.ReadLine()?.Trim();

                        if (line1 == null) break;

                        line2 = sr.ReadLine()?.Trim();
                        line3 = sr.ReadLine()?.Trim();
                        line4 = sr.ReadLine()?.Trim();

                        int a_x, a_y, b_x, b_y, p_x, p_y;

                        a_x = int.Parse(line1.Substring(line1.IndexOf('X') + 2, 2));
                        a_y = int.Parse(line1.Substring(line1.IndexOf('Y') + 2, 2));
                        b_x = int.Parse(line2.Substring(line2.IndexOf('X') + 2, 2));
                        b_y = int.Parse(line2.Substring(line2.IndexOf('Y') + 2, 2));

                        string[] l3 = line3.Split(',');
                        p_x = int.Parse(l3[0].Substring(l3[0].IndexOf('X') + 2));
                        p_y = int.Parse(l3[1].Substring(l3[1].IndexOf('Y') + 2));

                        ClawMachine c = new ClawMachine(a_x, a_y, b_x, b_y, p_x, p_y);
                        Machines.Add(c);

                        if (line4 == null)
                            break;
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

            long total_cost = 0;

            for (int i = 0; i < Machines.Count; i++)
            {
                int m, n;
                (m, n) = Machines[i].Solve();

                if (m != -1 && n != -1)
                {
                    int cost = (3 * m) + n;
                    Console.WriteLine($"The solution to claw machine #{(i + 1)} is {m} A button presses and {n} B button presses at a cost of {cost} coins");
                    total_cost += cost;
                }
                else
                    Console.WriteLine($"Claw machine #{(i + 1)} is not solvable");
            }

            Console.WriteLine($"Total cost to win all winnable prizes is {total_cost}");
        }
    }
}
