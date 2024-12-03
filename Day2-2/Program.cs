namespace Day2_2
{
    internal class Program
    {
        static void Main(string[] args)
        {
            int safe_reports = 0;
            List<string> reports = new List<string>();

            using (StreamReader sr = new StreamReader("Input.txt"))
            {
                try
                {
                    do
                    {
                        reports.Add(sr.ReadLine().Trim());

                    } while (!sr.EndOfStream);

                    for (int i = 0; i < reports.Count; i++)
                    {
                        List<int> levels = reports[i].Split(' ')?.Select(int.Parse)?.ToList<int>();

                        if (levels == null)
                            continue;

                        if (!is_safe(levels))
                        {
                            for (int j = 0; j < levels.Count; j++)
                            {
                                List<int> new_levels = new List<int>();

                                for (int z = 0; z < levels.Count; z++)
                                {
                                    if (z != j)
                                        new_levels.Add(levels[z]);
                                }

                                if (is_safe(new_levels))
                                {
                                    safe_reports++;
                                    break;
                                }
                            }
                        }
                        else
                        {
                            safe_reports++;
                        }
                    }

                    Console.WriteLine($"\nSafe Reports: {safe_reports}");
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

        static bool is_safe(List<int> levels)
        {
            if (levels[0] == levels[1])
                return false;

            bool increasing = (levels[0] < levels[1] ? true : false);
            bool failure = false;

            for (int j = 0; j < (levels.Count - 1); j++)
            {
                if (levels[j] > levels[j + 1] && increasing)
                {
                    failure = true;
                    break;
                }

                if (levels[j] < levels[j + 1] && !increasing)
                {
                    failure = true;
                    break;
                }

                var diff = Math.Abs(levels[j + 1] - levels[j]);

                if (diff == 0 || diff > 3)
                {
                    failure = true;
                    break;
                }
            }

            if (!failure)
            {
                Console.Write($"\nsafe levels == ");

                for (int z = 0; z < levels.Count; z++)
                {
                    Console.Write(levels[z] + " ");
                }
                return true;
            }

            return false;
        }
    }
}
