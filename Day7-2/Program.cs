using System.Data;
using System.Drawing;
using System.IO;
using System.Text;

namespace Day7_2
{
    internal class Program
    {
        internal class Equation
        {
            public Int64 Total;
            public Int64[] Terms;

            Int64 _Concat(Int64 x, Int64 y)
            {
                string s = x.ToString() + y.ToString();
                return Int64.Parse(s);
            }

            bool Add(Int64 expected_total, Int64 running_total, int depth)
            {
                if (depth == Terms.Length - 1)
                {
                    if ((running_total + Terms[depth]) == expected_total)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }

                if (Add(expected_total, running_total + Terms[depth], depth + 1))
                {
                    return true;
                }

                if (Mult(expected_total, running_total + Terms[depth], depth + 1))
                {
                    return true;
                }

                if (Concat(expected_total, running_total + Terms[depth], depth + 1))
                {
                    return true;
                }

                return false;
            }

            bool Mult(Int64 expected_total, Int64 running_total, int depth)
            {
                if (depth == Terms.Length - 1)
                {
                    if ((running_total * Terms[depth]) == expected_total)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }

                if (Add(expected_total, running_total * Terms[depth], depth + 1))
                {
                    return true;
                }

                if (Mult(expected_total, running_total * Terms[depth], depth + 1))
                {
                    return true;
                }

                if (Concat(expected_total, running_total * Terms[depth], depth + 1))
                {
                    return true;
                }

                return false;
            }

            bool Concat(Int64 expected_total, Int64 running_total, int depth)
            {
                if (depth == Terms.Length - 1)
                {
                    if ((_Concat(running_total, Terms[depth])) == expected_total)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }

                if (Add(expected_total, _Concat(running_total, Terms[depth]), depth + 1))
                {
                    return true;
                }

                if (Mult(expected_total, _Concat(running_total, Terms[depth]), depth + 1))
                {
                    return true;
                }

                if (Concat(expected_total, _Concat(running_total, Terms[depth]), depth + 1))
                {
                    return true;
                }

                return false;
            }

            public Equation(Int64 total, Int64[] terms)
            {
                this.Total = total;
                this.Terms = terms;
            }

            public bool HasValidSolution()
            {
                if (Add(Total, Terms[0], 1))
                {
                    return true;
                }
                else if (Mult(Total, Terms[0], 1))
                {
                    return true;
                }
                else if (Concat(Total, Terms[0], 1))
                {
                    return true;
                }

                return false;
            }
        }

        static void Main(string[] args)
        {
            List<Equation> equations = new List<Equation>();

            using (StreamReader sr = new StreamReader("Input.txt"))
            {
                string? line = "";

                try
                {
                    while (true)
                    {
                        line = sr.ReadLine()?.Trim();
                        if (line == null || line.Length == 0) break;

                        string[] raw_numbers = line.Split(':');
                        Int64 total = Int64.Parse(raw_numbers[0]);

                        Int64[] terms = raw_numbers[1].Trim().Split(' ').Select(Int64.Parse).ToArray<Int64>();

                        equations.Add(new Equation(total, terms));
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

            int valid_solutions = 0;
            Int64 valid_sum = 0;

            foreach (Equation e in equations)
            {
                if (e.HasValidSolution())
                {
                    valid_solutions++;
                    valid_sum += e.Total;
                }
            }

            Console.WriteLine($"There were {valid_solutions} valid equations, totaling {valid_sum}");
        }
    }
}