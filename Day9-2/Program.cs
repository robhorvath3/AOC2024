using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace Day9_2
{
    internal class Program
    {
        internal const int FREE_BLOCK = -1;
        internal const int EOB = -1;

        internal class DiskBlock
        {
            public int FileId;

            public DiskBlock(int file_id)
            {
                FileId = file_id;
            }
        }

        internal class Disk
        {
            public List<DiskBlock> Blocks = new List<DiskBlock>();

            public int GetFirstFreeBlockLargeEnough(int size, int orig_pos)
            {
                int consecutive_free_blocks = 0;
                int first_free_block = -1;

                for (int i = 0; i < orig_pos; i++)
                {
                    if (Blocks[i].FileId == FREE_BLOCK)
                    {
                        if (first_free_block == -1)
                            first_free_block = i;
                        
                        consecutive_free_blocks++;

                        if (consecutive_free_blocks >= size)
                            return first_free_block;
                    }
                    else
                    {
                        consecutive_free_blocks = 0;
                        first_free_block = -1;
                    }
                }

                return EOB;
            }

            public bool RelocateFile(int file_block, int size)
            {
                int new_location = GetFirstFreeBlockLargeEnough(size, file_block);
                int file_id = Blocks[file_block].FileId;

                if (new_location == EOB)
                {
                    return false;
                }

                for (int i = 0; i < size; i++)
                {
                    Blocks[new_location + i].FileId = file_id;
                    Blocks[file_block + i].FileId = FREE_BLOCK;
                }

                return true;
            }

            public (int block, int size) GetPreviousFile(int block)
            {
                int file_id = -1;
                int current_block = block;
                int file_size = 0;

                while (Blocks[current_block].FileId == FREE_BLOCK && current_block > 0)
                {
                    current_block--;
                }

                if (current_block == 0)
                {
                    return (file_id, file_size);
                }

                file_id = Blocks[current_block].FileId;

                do
                {
                    file_size++;
                    current_block--;
                } while (current_block >= 0 && Blocks[current_block].FileId == file_id);

                return (current_block + 1, file_size);
            }

            public Int64 Checksum()
            {
                Int64 checksum = 0;

                for (int i = 0; i < Blocks.Count; i++)
                {
                    if (Blocks[i].FileId == FREE_BLOCK)
                        continue;

                    checksum += Blocks[i].FileId * i;
                }

                return checksum;
            }
        }

        static void Main(string[] args)
        {
            string input = "";

            using (StreamReader sr = new StreamReader("Input.txt"))
            {
                try
                {
                    input = sr.ReadToEnd();                    
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

            char[] disk_map = input.ToCharArray();

            Disk disk = new Disk();

            for (int i = 0; i < disk_map.Length; i++)
            {
                int size_in_blocks = (int)char.GetNumericValue(disk_map[i]);
                int file_id = (i % 2 == 0) ? (i / 2) : FREE_BLOCK;

                for (int j = 0; j < size_in_blocks; j++)
                {
                    DiskBlock new_disk_block = new DiskBlock(file_id);
                    disk.Blocks.Add(new_disk_block);
                }
            }

            int current_block = disk.Blocks.Count - 1;

            do
            {
                int file_start_block = -1;
                int file_size = 0;

                (file_start_block, file_size) = disk.GetPreviousFile(current_block);
                disk.RelocateFile(file_start_block, file_size);

                current_block = file_start_block - 1;
            } while (current_block >= 0);

            Console.WriteLine($"Finished compacting disk (whole file method), checksum == {disk.Checksum()}");
        }
    }
}
