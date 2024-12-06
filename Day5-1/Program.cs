using System.Data;
using System.Drawing;
using System.IO;
using System.Text;

namespace Day5_1
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Dictionary<int, List<int>> proper_order = new Dictionary<int, List<int>>();
            List<int[]> print_order = new List<int[]>();

            int middle_sum = 0;

            using (StreamReader sr = new StreamReader("Input.txt"))
            {
                string? line = "";

                try
                {
                    // read proper ordering rules
                    while (true)
                    {
                        line = sr.ReadLine()?.Trim();
                        if (line == null || line.Length == 0) break;

                        int[] rule = line.Split('|').Select(int.Parse).ToArray<int>();
                        
                        if (proper_order.ContainsKey(rule[0]))
                        {
                            proper_order[rule[0]].Add(rule[1]);
                        }
                        else
                        {
                            List<int> new_rule_list = new List<int>();
                            new_rule_list.Add(rule[1]);
                            proper_order.Add(rule[0], new_rule_list);
                        }
                    }

                    // read the existing print order
                    while (true)
                    {
                        line = sr.ReadLine()?.Trim();
                        if (line == null || line.Length == 0) break;

                        int[] doc_order = line.Split(',').Select(int.Parse).ToArray<int>();
                        print_order.Add(doc_order);
                    }

                    // Side note - please don't ever do the following --
                    // accidentally exponential on unconstrained input is not fun
                    
                    // validate print order against the rules
                    for (int i = 0; i < print_order.Count; i++)
                    {
                        bool doc_follows_rule = true;

                        // no need to check the first element because by definition
                        // nothing comes before it
                        for (int j = 1; j < print_order[i].Length; j++)
                        {
                            // find the rules that pertain to this number (if any)
                            if (proper_order.ContainsKey(print_order[i][j]))
                            {
                                // make sure none of the pages that must come
                                // later are printed before
                                for (int y = 0; y < j; y++)
                                {
                                    for (int z = 0; z < proper_order[print_order[i][j]].Count; z++)
                                    {
                                        if (print_order[i][y] == proper_order[print_order[i][j]][z])
                                        {
                                            doc_follows_rule = false;
                                            goto JMPLBL;
                                        }
                                    }
                                }
                            }
                        }

                        JMPLBL:

                        if (doc_follows_rule)
                        {
                            middle_sum += print_order[i][print_order[i].Length / 2];
                        }
                    }

                    Console.WriteLine($"The sum of the middle page numbers of correctly ordered documents is {middle_sum}");
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
        }
    }
}