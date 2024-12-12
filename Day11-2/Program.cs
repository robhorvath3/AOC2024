using System.IO;
using System.Text;
using System.Collections.Generic;

namespace Day11_2
{
    // no brute force today
    // flip the problem
    internal class Program
    {
        internal struct Pair
        {
            public long X;
            public long Y;

            public Pair(long x, long y)
            {
                X = x;
                Y = y;
            }

            public void Set(long x, long y)
            {
                X = x;
                Y = y;
            }
        }

        internal static Dictionary<long, Pair> pair_cache = new Dictionary<long, Pair>();
        static Pair Blink(long engraving)
        {
            if (pair_cache.ContainsKey(engraving))
                return pair_cache[engraving];

            Pair pair = new Pair();

            if (engraving == 0)
            {
                pair.Set(1, -1);
            }
            else
            {
                string eng = engraving.ToString();

                if (eng.Length % 2 == 0)
                {
                    pair.Set(long.Parse(eng.Substring(0, eng.Length / 2)), long.Parse(eng.Substring(eng.Length / 2, eng.Length / 2)));
                }
                else
                {
                    pair.Set(engraving * 2024, -1);
                }                
            }

            pair_cache.Add(engraving, pair);
            return pair;
        }

        internal static Dictionary<long, long> stone_map;
        internal static Dictionary<long, long> stone_updates = new Dictionary<long, long>();

        internal static void ProcessStones(long k)
        {
            Pair pair = Blink(k);

            if (stone_updates.ContainsKey(pair.X))
            {
                stone_updates[pair.X] += stone_map[k];
            }
            else
            {
                stone_updates.Add(pair.X, stone_map[k]);
            }

            if (pair.Y != -1)
            {
                if (stone_updates.ContainsKey(pair.Y))
                {
                    stone_updates[pair.Y] += stone_map[k];
                }
                else
                {
                    stone_updates.Add(pair.Y, stone_map[k]);
                }
            }
        }

        static void Main(string[] args)
        {
            string input;

            using (StreamReader sr = new StreamReader("Input.txt"))
            {
                try
                {
                    input = sr.ReadToEnd();
                    stone_map = input.Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(x => Int64.Parse(x)).ToDictionary(s => s, s => (long)1);
                    
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
            
            int blink_count = 0;

            do
            {
                foreach (long k in stone_map.Keys)
                {
                    if (stone_map[k] > 0)
                        ProcessStones(k);
                }

                stone_map = stone_updates.ToDictionary();
                stone_updates.Clear();
                blink_count++;

            } while (blink_count < 75);

            long stone_count = 0;
            foreach (long k in stone_map.Keys)
            {
                stone_count += stone_map[k];
            }

            Console.WriteLine($"After blinking 75 times, there are now {stone_count} stones");
        }
    }
}
