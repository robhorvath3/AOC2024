using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace Day13_2
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
        public long X;
        public long Y;

        public Prize(long x, long y)
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

        public ClawMachine(int button_a_x, int button_a_y, int button_b_x, int button_b_y, long prize_x, long prize_y)
        {
            A = new Button(button_a_x, button_a_y);
            B = new Button(button_b_x, button_b_y);
            P = new Prize(prize_x, prize_y);
        }

        // this just uses a system of equations with m (# A button presses) and n (# of B button presses)
        // equating to the supplied prize coordinates; a final check is made to validate the solution
        //
        // example (A: X+94 Y+34, B: X+22 Y+67, Prize: X=8400 Y=5400):
        // 
        // 94M  + 22N = 8400
        // 34M  + 67N = 5400
        // ==================
        // 128M + 89N = 13800
        //
        // M = 13800 - 89N
        //     -----------
        //        128
        //
        // Then rewriting the first equation only in terms of n:
        //
        // 94M + 22N = 8400 =>
        //
        // 94((13800 - 89N)/128) + 22N = 8400
        //
        // solving for n:
        //
        //     ((8400 * 128) - (13800 * 94))
        // n = -----------------------------
        //       ((128 * 22) - (94 * 89))
        //
        // using this information, generalize back to the equation in Solve():
        //
        // i.e. 8400 is P.X, 128 is (A.X + A.Y), 13800 is (P.X + P.Y),
        // 94 is (A.X), 22 is (B.X), and 89 is (B.X + B.Y)
        //
        // when taken together, you get the equation in Solve()
        //
        // once n is calculated, plug it back into the equation above (using the
        // generalized variables) to solve for m, and then validate it
        public (long, long) Solve()
        {
            long numerator = ((A.X + A.Y) * P.X) - ((P.X + P.Y) * A.X);
            long denominator = ((A.X + A.Y) * B.X) - ((B.X + B.Y) * A.X);

            if (numerator % denominator != 0)
                return (-1, -1);

            long n = numerator / denominator;
            long m = ((P.X + P.Y) - ((B.X + B.Y) * n)) / (A.X + A.Y);

            if ((m * A.X) + (n * B.X) != P.X)
            {
                //Console.WriteLine($"Bad result at (m, n) == ({m}, {n}) for (A.X, B.X) == ({A.X}, {B.X}) != P.X ({P.X})");
                return (-1, -1);
            }

            if ((m * A.Y) + (n * B.Y) != P.Y)
            {
                //Console.WriteLine($"Bad result at (m, n) == ({m}, {n}) for (A.Y, B.Y) == ({A.Y}, {B.Y}) != P.Y ({P.Y})");
                return (-1, -1);
            }

            return (m, n);
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

                        ClawMachine c = new ClawMachine(a_x, a_y, b_x, b_y, p_x + 10000000000000, p_y + 10000000000000);
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
                long m, n;
                (m, n) = Machines[i].Solve();

                if (m != -1 && n != -1)
                {
                    long cost = (3 * m) + n;
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
