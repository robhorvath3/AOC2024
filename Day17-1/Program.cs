using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace Day17_1
{
    internal class Program
    {
        internal static class Computer
        {
            public static int IP = 0;
            public static ulong RegA = 0;
            public static ulong RegB = 0;
            public static ulong RegC = 0;
            public static int OutCount = 0;
            public static int[] Program;

            public static void Run()
            {
                if (Program == null)
                    throw new InvalidOperationException();

                do
                {
                    ExecOpcode(Program[IP], Program[IP + 1]);

                } while (IP < Program.Length - 1);
            }

            internal static ulong ComboOperand(int operand)
            {
                switch (operand)
                {
                    case 0:
                    case 1:
                    case 2:
                    case 3:
                        return (ulong)operand;
                    case 4:
                        return RegA;
                    case 5:
                        return RegB;
                    case 6:
                        return RegC;
                    case 7:
                    default:
                        throw new InvalidOperationException();
                }
            }

            internal static void ExecOpcode(int opcode, int operand)
            {
                switch (opcode)
                {
                    case 0:
                        Adv(operand);
                        IP += 2;
                        break;
                    case 1:
                        Bxl(operand);
                        IP += 2;
                        break;
                    case 2:
                        Bst(operand);
                        IP += 2;
                        break;
                    case 3:
                        bool jumped = Jnz(operand);
                        if (!jumped)
                            IP += 2;
                        break;
                    case 4:
                        Bxc(operand);
                        IP += 2;
                        break;
                    case 5:
                        Out(operand);
                        IP += 2;
                        break;
                    case 6:
                        Bdv(operand);
                        IP += 2;
                        break;
                    case 7:
                        Cdv(operand);
                        IP += 2;
                        break;
                }
            }

            public static void Adv(int operand)
            {
                RegA = (ulong)(RegA / Math.Pow(2, ComboOperand(operand)));
            }

            public static void Bxl(int operand)
            {
                RegB = RegB ^ (ulong)operand;
            }

            public static void Bst(int operand)
            {
                RegB = ComboOperand(operand) % 8;
            }

            public static bool Jnz(int operand)
            {
                if (RegA == 0) return false;

                IP = operand;
                return true;
            }

            public static void Bxc(int operand)
            {
                RegB = RegB ^ RegC;
            }

            public static void Out(int operand)
            {
                OutCount++;

                if (OutCount > 1)
                    Console.Write(",");

                Console.Write(ComboOperand(operand) % 8);
            }

            public static void Bdv(int operand)
            {
                RegB = (ulong)(RegA / Math.Pow(2, ComboOperand(operand)));
            }

            public static void Cdv(int operand)
            {
                RegC = (ulong)(RegA / Math.Pow(2, ComboOperand(operand)));
            }
        }
        
        static void Main(string[] args)
        {
            using (StreamReader sr = new StreamReader("Input.txt"))
            {
                try
                {
                    string[] input = sr.ReadToEnd().Split("\r\n");

                    foreach (string line in input)
                    {
                        if (line.Trim().Length == 0) continue;

                        string[] parts = line.Split(':');

                        switch (parts[0])
                        {
                            case "Register A":
                                Computer.RegA = ulong.Parse(parts[1]);
                                break;
                            case "Register B":
                                Computer.RegB = ulong.Parse(parts[1]);
                                break;
                            case "Register C":
                                Computer.RegC = ulong.Parse(parts[1]);
                                break;
                            case "Program":
                                Computer.Program = parts[1].Split(',').Select(x => int.Parse(x)).ToArray();
                                break;
                            default:
                                throw new InvalidOperationException();
                        }
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

                Computer.Run();
            }
        }
    }
}
