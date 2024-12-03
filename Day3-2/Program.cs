using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace Day3_2
{
    enum InstType
    {
        None,
        Mul,
        Do,
        Dont,
    };
    
    internal class Instruction
    {
        internal int Index = 0;
        internal InstType InstType = InstType.None;
        internal Int64 Operand1 = 0;
        internal Int64 Operand2 = 0;        
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            string input;
            Int64 result = 0;
            List<Instruction> instructions = new List<Instruction>();

            Regex mul_regex = new Regex(@"mul\((\d{1,3}),(\d{1,3})\)");
            Regex do_regex = new Regex(@"(do\(\))");
            Regex dont_regex = new Regex(@"(don't\(\))");

            using (StreamReader sr = new StreamReader("Input.txt"))
            {
                try
                {
                    input = sr.ReadToEnd();

                    {
                        MatchCollection mul_matches = mul_regex.Matches(input);

                        for (int j = 0; j < mul_matches.Count; j++)
                        {
                            Instruction i = new Instruction();
                            i.Index = mul_matches[j].Index;
                            i.InstType = InstType.Mul;
                            i.Operand1 = Int64.Parse(mul_matches[j].Groups[1].Value);
                            i.Operand2 = Int64.Parse(mul_matches[j].Groups[2].Value);

                            instructions.Add(i);
                        }
                    }

                    {
                        MatchCollection do_matches = do_regex.Matches(input);

                        for (int j = 0; j < do_matches.Count; j++)
                        {
                            Instruction i = new Instruction();
                            i.Index = do_matches[j].Index;
                            i.InstType = InstType.Do;

                            instructions.Add(i);
                        }
                    }

                    {
                        MatchCollection dont_matches = dont_regex.Matches(input);

                        for (int j = 0; j < dont_matches.Count; j++)
                        {
                            Instruction i = new Instruction();
                            i.Index = dont_matches[j].Index;
                            i.InstType = InstType.Dont;

                            instructions.Add(i);
                        }
                    }

                    instructions.Sort((x, y) => x.Index.CompareTo(y.Index));

                    bool MultAndSum = true;

                    for (int j = 0; j < instructions.Count; j++)
                    {
                        switch (instructions[j].InstType)
                        {
                            case InstType.Do:
                                MultAndSum = true;
                                continue;
                            case InstType.Dont:
                                MultAndSum = false;
                                continue;
                            case InstType.Mul:
                                if (MultAndSum)
                                    result += instructions[j].Operand1 * instructions[j].Operand2;
                                break;
                            case InstType.None:
                                throw new Exception("Instruction type NONE encountered");
                        }
                    }

                    Console.WriteLine($"Summing the active mul() instructions yields {result}");
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
