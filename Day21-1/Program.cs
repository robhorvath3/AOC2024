using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using static Day21_1.Program;

namespace Day21_1
{
    internal class Program
    {
        internal struct PinCode
        {
            public string Seq;

            public PinCode(string seq)
            {
                Seq = seq;
            }

            public int GetNumeric()
            {
                return int.Parse(Seq.Substring(0, 3));
            }
        }

        static void Main(string[] args)
        {
            // our pin codes
            List<PinCode> pins = new();

            // read input
            using (StreamReader sr = new StreamReader("Input.txt"))
            {
                try
                {
                    string[] lines = sr.ReadToEnd().Split("\r\n");

                    foreach (string line in lines)
                        pins.Add(new PinCode(line));
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

            // I know this is stupid, but I have a feeling it will make
            // part 2 faster
            //
            // priorities:
            // robots will begin moving arrows from the A key
            // 1 - avoid the blank spots in the keypads
            // 2 - left before up/down
            // 3 - up/down before right
            // 4 - see #1
            Dictionary<char, Dictionary<char, string>> numeric_paths = new();
            numeric_paths.Add('7', new Dictionary<char, string>());
            numeric_paths['7'].Add('8', ">");
            numeric_paths['7'].Add('9', ">>");
            numeric_paths['7'].Add('4', "v");
            numeric_paths['7'].Add('5', "v>");
            numeric_paths['7'].Add('6', "v>>");
            numeric_paths['7'].Add('1', "vv");
            numeric_paths['7'].Add('2', "vv>");
            numeric_paths['7'].Add('3', "vv>>");
            numeric_paths['7'].Add('0', ">vvv");
            numeric_paths['7'].Add('A', ">>vvv");
            numeric_paths.Add('8', new Dictionary<char, string>());
            numeric_paths['8'].Add('7', "<");
            numeric_paths['8'].Add('9', ">");
            numeric_paths['8'].Add('4', "<v");
            numeric_paths['8'].Add('5', "v");
            numeric_paths['8'].Add('6', "v>");
            numeric_paths['8'].Add('1', "<vv");
            numeric_paths['8'].Add('2', "vv");
            numeric_paths['8'].Add('3', "vv>");
            numeric_paths['8'].Add('0', "vvv");
            numeric_paths['8'].Add('A', "vvv>");
            numeric_paths.Add('9', new Dictionary<char, string>());
            numeric_paths['9'].Add('7', "<<");
            numeric_paths['9'].Add('8', "<");
            numeric_paths['9'].Add('4', "<<v");
            numeric_paths['9'].Add('5', "<v");
            numeric_paths['9'].Add('6', "v");
            numeric_paths['9'].Add('1', "<<vv");
            numeric_paths['9'].Add('2', "<vv");
            numeric_paths['9'].Add('3', "vv");
            numeric_paths['9'].Add('0', "<vvv");
            numeric_paths['9'].Add('A', "vvv");
            numeric_paths.Add('4', new Dictionary<char, string>());
            numeric_paths['4'].Add('7', "^");
            numeric_paths['4'].Add('8', "^>");
            numeric_paths['4'].Add('9', "^>>");
            numeric_paths['4'].Add('5', ">");
            numeric_paths['4'].Add('6', ">>");
            numeric_paths['4'].Add('1', "v");
            numeric_paths['4'].Add('2', "v>");
            numeric_paths['4'].Add('3', "v>>");
            numeric_paths['4'].Add('0', ">vv");
            numeric_paths['4'].Add('A', ">>vv");
            numeric_paths.Add('5', new Dictionary<char, string>());
            numeric_paths['5'].Add('7', "<^");
            numeric_paths['5'].Add('8', "^");
            numeric_paths['5'].Add('9', "^>");
            numeric_paths['5'].Add('4', "<");
            numeric_paths['5'].Add('6', ">");
            numeric_paths['5'].Add('1', "<v");
            numeric_paths['5'].Add('2', "v");
            numeric_paths['5'].Add('3', "v>");
            numeric_paths['5'].Add('0', "vv");
            numeric_paths['5'].Add('A', "vv>");
            numeric_paths.Add('6', new Dictionary<char, string>());
            numeric_paths['6'].Add('7', "<<^");
            numeric_paths['6'].Add('8', "<^");
            numeric_paths['6'].Add('9', "^");
            numeric_paths['6'].Add('4', "<<");
            numeric_paths['6'].Add('5', "<");
            numeric_paths['6'].Add('1', "<<v");
            numeric_paths['6'].Add('2', "<v");
            numeric_paths['6'].Add('3', "v");
            numeric_paths['6'].Add('0', "<vv");
            numeric_paths['6'].Add('A', "vv");
            numeric_paths.Add('1', new Dictionary<char, string>());
            numeric_paths['1'].Add('7', "^^");
            numeric_paths['1'].Add('8', "^^>");
            numeric_paths['1'].Add('9', "^^>>");
            numeric_paths['1'].Add('4', "^");
            numeric_paths['1'].Add('5', "^>");
            numeric_paths['1'].Add('6', "^>>");
            numeric_paths['1'].Add('2', ">");
            numeric_paths['1'].Add('3', ">>");
            numeric_paths['1'].Add('0', ">v");
            numeric_paths['1'].Add('A', ">>v");
            numeric_paths.Add('2', new Dictionary<char, string>());
            numeric_paths['2'].Add('7', "<^^");
            numeric_paths['2'].Add('8', "^^");
            numeric_paths['2'].Add('9', "^^>");
            numeric_paths['2'].Add('4', "<^");
            numeric_paths['2'].Add('5', "^");
            numeric_paths['2'].Add('6', "^>");
            numeric_paths['2'].Add('1', "<");
            numeric_paths['2'].Add('3', ">");
            numeric_paths['2'].Add('0', "v");
            numeric_paths['2'].Add('A', "v>");
            numeric_paths.Add('3', new Dictionary<char, string>());
            numeric_paths['3'].Add('7', "<<^^");
            numeric_paths['3'].Add('8', "<^^");
            numeric_paths['3'].Add('9', "^^");
            numeric_paths['3'].Add('4', "<<^");
            numeric_paths['3'].Add('5', "<^");
            numeric_paths['3'].Add('6', "^");
            numeric_paths['3'].Add('1', "<<");
            numeric_paths['3'].Add('2', "<");
            numeric_paths['3'].Add('0', "<v");
            numeric_paths['3'].Add('A', "v");
            numeric_paths.Add('0', new Dictionary<char, string>());
            numeric_paths['0'].Add('7', "^^^<");
            numeric_paths['0'].Add('8', "^^^");
            numeric_paths['0'].Add('9', "^^^>");
            numeric_paths['0'].Add('4', "^^<");
            numeric_paths['0'].Add('5', "^^");
            numeric_paths['0'].Add('6', "^^>");
            numeric_paths['0'].Add('1', "^<");
            numeric_paths['0'].Add('2', "^");
            numeric_paths['0'].Add('3', "^>");
            numeric_paths['0'].Add('A', ">");
            numeric_paths.Add('A', new Dictionary<char, string>());
            numeric_paths['A'].Add('7', "^^^<<");
            numeric_paths['A'].Add('8', "<^^^");
            numeric_paths['A'].Add('9', "^^^");
            numeric_paths['A'].Add('4', "^^<<");
            numeric_paths['A'].Add('5', "<^^");
            numeric_paths['A'].Add('6', "^^");
            numeric_paths['A'].Add('1', "^<<");
            numeric_paths['A'].Add('2', "<^");
            numeric_paths['A'].Add('3', "^");
            numeric_paths['A'].Add('0', "<");

            Dictionary<char, Dictionary<char, string>> dir_paths = new();
            dir_paths.Add('^', new Dictionary<char, string>());
            dir_paths['^'].Add('A', ">");
            dir_paths['^'].Add('<', "v<");
            dir_paths['^'].Add('v', "v");
            dir_paths['^'].Add('>', "v>");
            dir_paths.Add('A', new Dictionary<char, string>());
            dir_paths['A'].Add('^', "<");
            dir_paths['A'].Add('<', "v<<");
            dir_paths['A'].Add('v', "<v");
            dir_paths['A'].Add('>', "v");
            dir_paths.Add('<', new Dictionary<char, string>());
            dir_paths['<'].Add('^', ">^");
            dir_paths['<'].Add('A', ">>^");
            dir_paths['<'].Add('v', ">");
            dir_paths['<'].Add('>', ">>");
            dir_paths.Add('v', new Dictionary<char, string>());
            dir_paths['v'].Add('^', "^");
            dir_paths['v'].Add('A', "^>");
            dir_paths['v'].Add('<', "<");
            dir_paths['v'].Add('>', ">");
            dir_paths.Add('>', new Dictionary<char, string>());
            dir_paths['>'].Add('^', "<^");
            dir_paths['>'].Add('A', "^");
            dir_paths['>'].Add('<', "<<");
            dir_paths['>'].Add('v', "<");

            Dictionary<(char, char, int), long> KeyCostCache = new();

            long CalcKeyPath(char srckey, char destkey, int depth, int maxdepth)
            {
                if (KeyCostCache.ContainsKey((srckey, destkey, depth)))
                    return KeyCostCache[(srckey, destkey, depth)];

                string path;

                if (srckey != destkey)
                    if (depth == 0)
                        path = numeric_paths[srckey][destkey] + 'A';
                    else
                        path = dir_paths[srckey][destkey] + 'A';
                else
                    path = "A";

                long cost = 0;

                if (depth == maxdepth)
                    return (long)path.Length;

                for (int i = 0; i < path.Length; i++)
                {
                    char next_srckey = (i == 0) ? 'A' : path[i - 1];
                    cost += CalcKeyPath(next_srckey, path[i], depth + 1, maxdepth);
                }

                KeyCostCache.Add((srckey, destkey, depth), cost);

                return cost;
            }

            long sequenceval = 0;

            foreach (PinCode pc in pins)
            {
                int directional_levels = 3;
                long cost = 0;

                for (int i = 0; i < pc.Seq.Length; i++)
                {
                    char srckey = (i == 0) ? 'A' : pc.Seq[i - 1];
                    cost += CalcKeyPath(srckey, pc.Seq[i], 0, directional_levels - 1);
                }

                sequenceval += cost * pc.GetNumeric();

                Console.WriteLine($"PIN ({pc.Seq}) [{cost} / {pc.GetNumeric()} / {cost * pc.GetNumeric()}]");
            }

            Console.WriteLine($"Total sum of complexities is {sequenceval}");
        }
    }
}