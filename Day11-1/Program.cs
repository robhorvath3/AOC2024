using System.IO;
using System.Text;
using System.Collections.Generic;

namespace Day11_1
{
    internal class Program
    {
        internal class Stone
        {
            public Int64 Engraving;

            public Stone(Int64 engraving)
            {
                this.Engraving = engraving;
            }

            public Stone? Blink()
            {
                if (this.Engraving == 0)
                {
                    this.Engraving = 1;
                    return null;
                }
                else if (this.Engraving.ToString().Trim().Length % 2 != 0)
                {
                    this.Engraving *= 2024;
                    return null;
                }
                else
                {
                    StringBuilder sb_left = new StringBuilder();
                    StringBuilder sb_right = new StringBuilder();
                    string eng = this.Engraving.ToString().Trim();
                    int eng_len = eng.Length;

                    for (int i = 0; i < eng_len; i++)
                    {
                        if (i < eng_len / 2)
                        {
                            sb_left.Append(eng[i]);
                        }
                        else
                        {
                            sb_right.Append(eng[i]);
                        }
                    }

                    this.Engraving = Int64.Parse(sb_left.ToString());
                    return new Stone(Int64.Parse(sb_right.ToString()));
                }
            }
        }
        
        static void Main(string[] args)
        {
            string input;
            LinkedList<Stone> stone_list = new LinkedList<Stone>();

            using (StreamReader sr = new StreamReader("Input.txt"))
            {
                try
                {
                    input = sr.ReadToEnd();

                    Int64[] stone_numbers = input.Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(x => Int64.Parse(x)).ToArray();

                    for (int i = 0; i < stone_numbers.Length; i++)
                    {
                        stone_list.AddLast(new Stone(stone_numbers[i]));
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

            for (int blink_count = 0; blink_count < 25; blink_count++)
            {
                Dictionary<Stone, Stone> new_stones = new Dictionary<Stone, Stone>();

                foreach (Stone stone in stone_list)
                {
                    Stone? new_stone = stone.Blink();

                    if (new_stone != null)
                    {
                        new_stones.Add(stone, new_stone);
                    }
                }

                foreach (Stone source_stone in new_stones.Keys)
                {
                    LinkedListNode<Stone> new_stone_node = new LinkedListNode<Stone>(new_stones[source_stone]);
                    stone_list.AddAfter(stone_list.Find(source_stone), new_stone_node);
                }
            }

            Console.WriteLine($"After blinking 25 times, there are now {stone_list.Count} stones");
        }
    }
}
