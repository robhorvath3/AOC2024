using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace Day3_1
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string input;
            Int64 result = 0;

            Regex regex = new Regex(@"mul\((\d{1,3}),(\d{1,3})\)");

            using (StreamReader sr = new StreamReader("Input.txt"))
            {
                try
                {
                    input = sr.ReadToEnd();

                    MatchCollection matches = regex.Matches(input);
                    
                    for (int i = 0; i < matches.Count; i++)
                    {
                        result += (Int64.Parse(matches[i].Groups[1].Value) * Int64.Parse(matches[i].Groups[2].Value));
                    }

                    Console.WriteLine($"Summing the {matches.Count} mul() instructions yields {result}");
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
