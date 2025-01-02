using System.IO;
using System.Text;
using System.Runtime.Intrinsics;
using System.Diagnostics;

namespace Day25_1
{
    internal struct Key
    {
        public Vector256<int> Height;

        public Key(int columns, int[] heights)
        {
            if (columns > 8)
                throw new NotImplementedException();

            Height = Vector256.Create((columns >= 1) ? heights[0] : 0,
                                      (columns >= 2) ? heights[1] : 0,
                                      (columns >= 3) ? heights[2] : 0,
                                      (columns >= 4) ? heights[3] : 0,
                                      (columns >= 5) ? heights[4] : 0,
                                      (columns >= 6) ? heights[5] : 0,
                                      (columns >= 7) ? heights[6] : 0,
                                      (columns >= 8) ? heights[7] : 0);
        }
    }
    
    internal struct Lock
    {
        public Vector256<int> Height;
        
        public Lock(int columns, int[] heights)
        {
            if (columns > 8)
                throw new NotImplementedException();

            Height = Vector256.Create((columns >= 1) ? heights[0] : int.MaxValue,
                                      (columns >= 2) ? heights[1] : int.MaxValue,
                                      (columns >= 3) ? heights[2] : int.MaxValue,
                                      (columns >= 4) ? heights[3] : int.MaxValue,
                                      (columns >= 5) ? heights[4] : int.MaxValue,
                                      (columns >= 6) ? heights[5] : int.MaxValue,
                                      (columns >= 7) ? heights[6] : int.MaxValue,
                                      (columns >= 8) ? heights[7] : int.MaxValue);            
        }

        public bool DoesKeyFit(Key key)
        {
            // the height of the lock's free space must be >= the height of the key
            Vector256<int> comparison_result = Vector256.GreaterThanOrEqual(Height, key.Height);

            // the comparison should yield all zeros in a valid key, signifying that
            // the height of the lock was greater than or equal to the key for all columns
            // Vector256 >= will return a -1 for each vector component that meets the 
            // criteria, so we need to sum to -8 (8 slots for 32-bit int in 256-bit vector)
            if (Vector256.Sum(comparison_result) == -8)
                return true;
            else
                return false;
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
                                locks.Add(new Lock(max_col + 1, heights));
                            else
                                keys.Add(new Key(max_col + 1, heights));

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

                // try every unique combination
                foreach(Lock l in locks)
                {
                    foreach (Key k in keys)
                    {
                        if (l.DoesKeyFit(k))
                            unique_lock_key_pairs++;
                    }
                }

                Console.WriteLine($"We found {unique_lock_key_pairs} unique lock/key pairs");
            }
        }
    }
}
