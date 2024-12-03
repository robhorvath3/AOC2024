using System.IO;

namespace Day1_1
{
    internal class Program
    {
        static void Main(string[] args)
        {
            List<int> left_list = new List<int>();
            List<int> right_list = new List<int>();
            
            using (StreamReader sr = new StreamReader("Input.txt"))
            {
                try
                {
                    do
                    {
                        var locations = sr.ReadLine().Split(' ', StringSplitOptions.RemoveEmptyEntries);

                        if (locations == null)
                        {
                            break;
                        }

                        left_list.Add(int.Parse(locations[0]));
                        right_list.Add(int.Parse(locations[1]));

                    } while (!sr.EndOfStream);

                    left_list.Sort();
                    right_list.Sort();

                    int dist = 0;

                    for (int i = 0; i < left_list.Count; i++)
                    {
                        dist += Math.Abs(left_list[i] - right_list[i]);
                    }

                    Console.WriteLine($"Distance between locations: {dist}");
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
