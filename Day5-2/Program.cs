using System.Data;
using System.Drawing;
using System.IO;
using System.Collections;

namespace Day5_2
{
    internal class Program
    {
        static Dictionary<int, List<int>> proper_order_before = new Dictionary<int, List<int>>();
        static Dictionary<int, List<int>> proper_order_after = new Dictionary<int, List<int>>();
        static List<int[]> print_order = new List<int[]>();

        // I love this
        public class PageComparer : IComparer<int>
        {
            // this works because the given input is consistent (i.e. it will not 
            // specify that x must come before y and that y must also come before x)
            public int Compare(int x, int y)
            {
                if (proper_order_before.ContainsKey(x) && proper_order_before[x].Contains(y))
                {
                    return -1;
                }
                else if (proper_order_after.ContainsKey(x) && proper_order_after[x].Contains(y))
                {
                    return 1;
                }
                else if (proper_order_before.ContainsKey(y) && proper_order_before[y].Contains(x))
                {
                    return 1;
                }
                else if (proper_order_after.ContainsKey(y) && proper_order_after[y].Contains(x))
                {
                    return -1;
                }
                else
                {
                    return 0;
                }
            }
        }
        
        // still exponential
        static bool ValidatePrintOrder(int which_print_order)
        {
            // no need to check the first element because by definition
            // nothing comes before it
            for (int j = 1; j < print_order[which_print_order].Length; j++)
            {
                // find the rules that pertain to this number (if any)
                if (proper_order_before.ContainsKey(print_order[which_print_order][j]))
                {
                    // make sure none of the pages that must come
                    // later are printed before
                    for (int y = 0; y < j; y++)
                    {
                        for (int z = 0; z < proper_order_before[print_order[which_print_order][j]].Count; z++)
                        {
                            if (print_order[which_print_order][y] == proper_order_before[print_order[which_print_order][j]][z])
                            {
                                return false;
                            }
                        }
                    }
                }
            }
            return true;
        }
        
        static void Main(string[] args)
        {
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


                        // proper order before list (x before y)
                        if (proper_order_before.ContainsKey(rule[0]))
                        {
                            proper_order_before[rule[0]].Add(rule[1]);
                        }
                        else
                        {
                            List<int> new_rule_list = new List<int>();
                            new_rule_list.Add(rule[1]);
                            proper_order_before.Add(rule[0], new_rule_list);
                        }

                        // proper order after list (x after y)
                        if (proper_order_after.ContainsKey(rule[1]))
                        {
                            proper_order_after[rule[1]].Add(rule[0]);
                        }
                        else
                        {
                            List<int> new_rule_list = new List<int>();
                            new_rule_list.Add(rule[0]);
                            proper_order_after.Add(rule[1], new_rule_list);
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
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Exception: {ex.Message}");
                }
                finally
                {
                    sr.Close();
                }


                // validate print order against the rules
                for (int i = 0; i < print_order.Count; i++)
                {
                    bool print_order_updated = false;

                    if (!ValidatePrintOrder(i))
                    {
                        Array.Sort(print_order[i], new PageComparer());
                        print_order_updated = true;
                    }                        

                    // double check
                    if (!ValidatePrintOrder(i))
                        throw new Exception("WTF - Print order still incorrect after sorting for element #{i}");

                    if (print_order_updated)
                        middle_sum += print_order[i][print_order[i].Length / 2];
                }

                Console.WriteLine($"The sum of the middle page numbers of re-ordered documents is {middle_sum}");
            }                
        }
    }
}