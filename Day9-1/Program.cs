using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace Day9_1
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

            public int GetFirstFreeBlock()
            {
                for (int i = 0; i < Blocks.Count; i++)
                {
                    if (Blocks[i].FileId == FREE_BLOCK)
                        return i;                    
                }

                return EOB;
            }

            public bool SwapBlock(int block)
            {
                int first_free_block = GetFirstFreeBlock();

                if (first_free_block > block)
                {
                    return false;
                }

                Blocks[first_free_block].FileId = Blocks[block].FileId;
                Blocks[block].FileId = FREE_BLOCK;

                return true;
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

            for (int z = disk.Blocks.Count - 1; z >= 0; z--)
            {
                if (disk.Blocks[z].FileId == FREE_BLOCK)
                    continue;

                if (!disk.SwapBlock(z))
                    break;
            }

            Console.WriteLine($"Finished compacting disk, checksum == {disk.Checksum()}");
        }
    }
}
