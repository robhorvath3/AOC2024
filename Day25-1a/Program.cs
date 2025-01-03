using System.IO;
using System.Text;
using System.Runtime.Intrinsics;
using System.Diagnostics;

namespace Day25_1a
{
    internal struct Key
    {
        public int[] Height;

        public Key(int[] heights)
        {
            if (heights.Length > 8)
                throw new NotImplementedException();

            Height = heights;
        }
    }
    
    internal struct Lock
    {
        public int[] Height;
        
        public Lock(int[] heights)
        {
            if (heights.Length > 8)
                throw new NotImplementedException();

            Height = heights;
        }

        public bool DoesKeyFit(Key key)
        {
            if (Height.Length != key.Height.Length)
                throw new NotImplementedException();

            for (int i = 0; i < Height.Length; i++)
            {
                if (Height[i] < key.Height[i])
                    return false;
            }

            return true;
        }
    }

    internal class Program
    {        
        static void Main(string[] args)
        {
            List<Lock> locks = new();
            List<Key> keys = new();
            int max_col = 4;
            int max_row = 6;

            using (StreamReader sr = new StreamReader("Input.txt"))
            {
                try
                {
                    string[] lines = sr.ReadToEnd().Split("\r\n");
                    List<char[]> input_object = new();
                    
                    for (int l = 0; l < lines.Length; l++)
                    {
                        // once we hit a zero length line or the end, we have
                        // hit the end of an input object (a lock or a key)
                        if (lines[l].Length == 0 || l == lines.Length - 1)
                        {
                            // need to add our last line & process if this is the 
                            // last row
                            if (l == lines.Length - 1)
                                input_object.Add(lines[l].ToCharArray());

                            // char @ (0, max_row) is '.' for locks, '#' for keys;
                            // we count teeth for keys, but empty spaces for locks
                            // (i.e. the # of spaces where there are no pins)
                            char char2count = input_object[max_row][0];
                            int[] heights = new int[max_col + 1];

                            // count the height of the teeth on keys
                            // or the free space on locks
                            for (int col = 0; col <= max_col; col++)
                            {
                                int height = 0;

                                for (int row = max_row - 1; row > 0; row--)
                                {
                                    if (input_object[row][col] == char2count)
                                        height++;
                                    else
                                        break;
                                }

                                heights[col] = height;
                            }

                            // add our objects to their collections
                            if (char2count == '.') // a lock
                                locks.Add(new Lock(heights));
                            else
                                keys.Add(new Key(heights));

                            // clear our input object
                            input_object.Clear();
                        }
                        else
                        {
                            // add the new row to our in-progress input row collection
                            input_object.Add(lines[l].ToCharArray());
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Exception: {ex.Message}");

                    // Get stack trace for the exception with source file information
                    var st = new StackTrace(ex, true);
                    // Get the top stack frame
                    var frame = st.GetFrame(0);
                    // Get the line number from the stack frame
                    var line_no = frame.GetFileLineNumber();

                    Console.WriteLine($"Trace: {st}\nFrame: {frame}\nLine: {line_no}");
                }
                finally
                {
                    sr.Close();
                }

                Console.WriteLine($"Found {locks.Count} locks and {keys.Count} keys. Testing...");

                long unique_lock_key_pairs = 0;
                var exec_timer = System.Diagnostics.Stopwatch.StartNew();

                // try every unique combination
                foreach(Lock l in locks)
                {
                    foreach (Key k in keys)
                    {
                        if (l.DoesKeyFit(k))
                            unique_lock_key_pairs++;
                    }
                }
                exec_timer.Stop();

                long nanos_per_tick = (1000L * 1000L * 1000L) / Stopwatch.Frequency;

                Console.WriteLine($"We found {unique_lock_key_pairs} unique lock/key pairs in {exec_timer.ElapsedTicks} ticks (approx {exec_timer.ElapsedTicks * nanos_per_tick} ns)");
            }
        }
    }
}
