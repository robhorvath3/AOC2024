using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Runtime.Intrinsics;

namespace Day22_1
{
    internal class Program
    {
        const int InputCount = 2020;
        const long ModConstant = 16777216;
        static Vector256<long>[] SecretNumbers = new Vector256<long>[InputCount / 4];
        static Vector256<long>[] AndF = new Vector256<long>[InputCount / 4];

        static Vector256<long>[] GetSecretNumbersCopy()
        {
            var result = new Vector256<long>[InputCount / 4];
            SecretNumbers.CopyTo(result, 0);
            return result;
        }

        static Vector256<long>[] MultBy(long mult)
        {
            Vector256<long>[] result = new Vector256<long>[InputCount / 4];

            for (int i = 0; i < InputCount / 4; i++)
                result[i] = Vector256.Multiply<long>(mult, SecretNumbers[i]);

            return result;
        }

        static void Mix(Vector256<long>[] input)
        {
            for (int i = 0; i < InputCount / 4; i++)
                SecretNumbers[i] = Vector256.Xor<long>(input[i], SecretNumbers[i]);
        }

        static void Mod()
        {
            for (int i = 0; i < InputCount / 4; i++)
            {
                long decon0 = SecretNumbers[i].GetElement(0) % ModConstant;
                long decon1 = SecretNumbers[i].GetElement(1) % ModConstant;
                long decon2 = SecretNumbers[i].GetElement(2) % ModConstant;
                long decon3 = SecretNumbers[i].GetElement(3) % ModConstant;
                SecretNumbers[i] = Vector256.Create(decon0, decon1, decon2, decon3);
            }
        }

        static Vector256<long>[] DivBy32()
        {
            Vector256<long>[] result = new Vector256<long>[InputCount / 4];

            for (int i = 0; i < InputCount / 4; i++)
                result[i] = Vector256.ShiftRightLogical(SecretNumbers[i], 5);

            return result;
        }

        static long Sum()
        {
            long sum = 0;

            for (int i = 0; i < InputCount / 4; i++)
                sum += Vector256.Sum(SecretNumbers[i]);

            return sum;
        }

        static void Prune()
        {
            Mod();
        }

        static void NextSecretNumber()
        {
            Vector256<long>[] mult64_result = MultBy(64);
            Mix(mult64_result);
            Prune();
            Vector256<long>[] div32_result = DivBy32();
            Mix(div32_result);
            Prune();
            Vector256<long>[] mult2048_result = MultBy(2048);
            Mix(mult2048_result);
            Prune();
        }

        static void Main(string[] args)
        {
            using (StreamReader sr = new StreamReader("Input.txt"))
            {
                try
                {
                    long[] input = sr.ReadToEnd().Split("\r\n").Select(n => long.Parse(n)).ToArray();

                    for (int i = 0; i < InputCount / 4; i++)
                    {
                        SecretNumbers[i] = Vector256.Create<long>(input, i * 4);
                        AndF[i] = Vector256.Create((long)0xF, (long)0xF, (long)0xF, (long)0xF);
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

            Console.WriteLine($"Initial {SecretNumbers[0].GetElement(0)}");

            for (int i = 0; i < 2000; i++)
            {
                NextSecretNumber();
                Console.WriteLine($"{SecretNumbers[0].GetElement(0)}");
            }

            Console.WriteLine($"The sum of everyone's 2000th new secret number is {Sum()}");
        }
    }
}
