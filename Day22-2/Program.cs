using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Runtime.Intrinsics;

namespace Day22_2
{
    internal class Program
    {
        const int InputCount = 2020;
        const long ModConstant = 16777216;
        static Vector256<long>[] SecretNumbers = new Vector256<long>[InputCount / 4];

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

        static Vector256<long>[] GetPrices()
        {
            Vector256<long>[] result = new Vector256<long>[InputCount / 4];

            for (int i = 0; i < InputCount / 4; i++)
            {
                string sdecon0 = SecretNumbers[i].GetElement(0).ToString();
                string sdecon1 = SecretNumbers[i].GetElement(1).ToString();
                string sdecon2 = SecretNumbers[i].GetElement(2).ToString();
                string sdecon3 = SecretNumbers[i].GetElement(3).ToString();

                long decon0 = long.Parse(sdecon0.Substring(sdecon0.Length - 1, 1));
                long decon1 = long.Parse(sdecon1.Substring(sdecon1.Length - 1, 1));
                long decon2 = long.Parse(sdecon2.Substring(sdecon2.Length - 1, 1));
                long decon3 = long.Parse(sdecon3.Substring(sdecon3.Length - 1, 1));

                result[i] = Vector256.Create(decon0, decon1, decon2, decon3);
            }

            return result;
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

            Dictionary<(int, int, int, int, int), long> Tracker = new();
            Vector256<long>[,] RollingPrices = new Vector256<long>[InputCount / 4, 5];

            // Get our initial prices
            var new_prices = GetPrices();
            for (int j = 0; j < InputCount / 4; j++)
            {
                RollingPrices[j, 0] = new_prices[j];                
            }

            for (int i = 0; i < 2000; i++)
            {
                NextSecretNumber();

                new_prices = GetPrices();

                if (i < 3)
                {
                    for (int j = 0; j < InputCount / 4; j++)
                        RollingPrices[j, i + 1] = new_prices[j];
                }
                else
                {
                    for (int j = 0; j < InputCount / 4; j++)
                    {
                        if (i > 3)
                        {
                            RollingPrices[j, 0] = RollingPrices[j, 1];
                            RollingPrices[j, 1] = RollingPrices[j, 2];
                            RollingPrices[j, 2] = RollingPrices[j, 3];
                            RollingPrices[j, 3] = RollingPrices[j, 4];
                        }

                        RollingPrices[j, 4] = new_prices[j];

                        int pc1_1 = (int)RollingPrices[j, 1].GetElement(0) - (int)RollingPrices[j, 0].GetElement(0);
                        int pc2_1 = (int)RollingPrices[j, 2].GetElement(0) - (int)RollingPrices[j, 1].GetElement(0);
                        int pc3_1 = (int)RollingPrices[j, 3].GetElement(0) - (int)RollingPrices[j, 2].GetElement(0);
                        int pc4_1 = (int)RollingPrices[j, 4].GetElement(0) - (int)RollingPrices[j, 3].GetElement(0);
                        int pc1_2 = (int)RollingPrices[j, 1].GetElement(1) - (int)RollingPrices[j, 0].GetElement(1);
                        int pc2_2 = (int)RollingPrices[j, 2].GetElement(1) - (int)RollingPrices[j, 1].GetElement(1);
                        int pc3_2 = (int)RollingPrices[j, 3].GetElement(1) - (int)RollingPrices[j, 2].GetElement(1);
                        int pc4_2 = (int)RollingPrices[j, 4].GetElement(1) - (int)RollingPrices[j, 3].GetElement(1);
                        int pc1_3 = (int)RollingPrices[j, 1].GetElement(2) - (int)RollingPrices[j, 0].GetElement(2);
                        int pc2_3 = (int)RollingPrices[j, 2].GetElement(2) - (int)RollingPrices[j, 1].GetElement(2);
                        int pc3_3 = (int)RollingPrices[j, 3].GetElement(2) - (int)RollingPrices[j, 2].GetElement(2);
                        int pc4_3 = (int)RollingPrices[j, 4].GetElement(2) - (int)RollingPrices[j, 3].GetElement(2);
                        int pc1_4 = (int)RollingPrices[j, 1].GetElement(3) - (int)RollingPrices[j, 0].GetElement(3);
                        int pc2_4 = (int)RollingPrices[j, 2].GetElement(3) - (int)RollingPrices[j, 1].GetElement(3);
                        int pc3_4 = (int)RollingPrices[j, 3].GetElement(3) - (int)RollingPrices[j, 2].GetElement(3);
                        int pc4_4 = (int)RollingPrices[j, 4].GetElement(3) - (int)RollingPrices[j, 3].GetElement(3);

                        if (!Tracker.ContainsKey((pc1_1, pc2_1, pc3_1, pc4_1, (j * 4))))
                            Tracker.Add((pc1_1, pc2_1, pc3_1, pc4_1, (j * 4)), RollingPrices[j, 4].GetElement(0));
                        
                        if (!Tracker.ContainsKey((pc1_2, pc2_2, pc3_2, pc4_2, (j * 4) + 1)))
                            Tracker.Add((pc1_2, pc2_2, pc3_2, pc4_2, (j * 4) + 1), RollingPrices[j, 4].GetElement(1));
                        
                        if (!Tracker.ContainsKey((pc1_3, pc2_3, pc3_3, pc4_3, (j * 4) + 2)))
                            Tracker.Add((pc1_3, pc2_3, pc3_3, pc4_3, (j * 4) + 2), RollingPrices[j, 4].GetElement(2));
                        
                        if (!Tracker.ContainsKey((pc1_4, pc2_4, pc3_4, pc4_4, (j * 4) + 3)))
                            Tracker.Add((pc1_4, pc2_4, pc3_4, pc4_4, (j * 4) + 3), RollingPrices[j, 4].GetElement(3));
                    }
                }
            }

            long most_bananas = 0;
            Dictionary<(int, int, int, int), long> Comparer = new();

            foreach ((int pc1, int pc2, int pc3, int pc4, int monkey) in Tracker.Keys)
            {
                if (!Comparer.ContainsKey((pc1, pc2, pc3, pc4)))
                    Comparer.Add((pc1, pc2, pc3, pc4), Tracker[(pc1, pc2, pc3, pc4, monkey)]);
                else
                    Comparer[(pc1, pc2, pc3, pc4)] += Tracker[(pc1, pc2, pc3, pc4, monkey)];
            }

            foreach ((int pc1, int pc2, int pc3, int pc4) in Comparer.Keys)
            {
                if (Comparer[(pc1, pc2, pc3, pc4)] > most_bananas)
                    most_bananas = Comparer[(pc1, pc2, pc3, pc4)];
            }

            Console.WriteLine($"The most bananas you can get is {most_bananas}");
        }
    }
}
