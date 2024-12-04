using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace Day4_2
{
    internal class Program
    {
        internal class SMToggle
        {
            char First;

            internal SMToggle(char first)
            {
                if (first != 'S' && first != 'M')
                    throw new ArgumentException("Only S or M, please");

                First = first;
            }

            internal char Next()
            {
                switch (First)
                {
                    case 'S':
                        return 'M';
                    case 'M':
                        return 'S';
                    default:
                        throw new Exception("SMToggle - this exception should never happen");
                }
            }
        }

        static void Main(string[] args)
        {
            string[] crossword = null;
            int mas_crosses = 0;

            using (StreamReader sr = new StreamReader("Input.txt"))
            {
                try
                {
                    crossword = sr.ReadToEnd().Split("\r\n");
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

            for (int row = 1; row < crossword.Length - 1; row++)
            {
                for (int col = 1; col < crossword[0].Length - 1; col++)
                {
                    if (crossword[row][col] == 'A')
                    {
                        SMToggle left, right;

                        if (crossword[row - 1][col - 1] == 'S' || crossword[row - 1][col - 1] == 'M')
                        {
                            left = new SMToggle(crossword[row - 1][col - 1]);

                            if (crossword[row + 1][col + 1] != left.Next())
                            {
                                continue;
                            }
                        }
                        else
                        {
                            continue;
                        }

                        if (crossword[row - 1][col + 1] == 'S' || crossword[row - 1][col + 1] == 'M')
                        {
                            right = new SMToggle(crossword[row - 1][col + 1]);

                            if (crossword[row + 1][col - 1] != right.Next())
                            {
                                continue;
                            }
                        }
                        else
                        {
                            continue;
                        }

                        // holy batsh*t batman, we found one
                        mas_crosses++;
                    }
                }
            }

            Console.WriteLine($"Crossed MAS appears in the crossword puzzle {mas_crosses} times");
        }
    }
}