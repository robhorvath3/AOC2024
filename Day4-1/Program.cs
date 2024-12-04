using System.Data;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace Day4_1
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string[] crossword = null;
            Int64 xmas_count = 0;

            Regex xmas_regex = new Regex(@"XMAS");
            Regex samx_regex = new Regex(@"SAMX");

            using (StreamReader sr = new StreamReader("Input.txt"))
            {
                try
                {
                    crossword = sr.ReadToEnd().Split("\r\n");
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

            // Horizontal Matches
            for (int row = 0; row < crossword.Length; row++)
            {
                MatchCollection xmas_matches = xmas_regex.Matches(crossword[row]);
                MatchCollection samx_matches = samx_regex.Matches(crossword[row]);

                xmas_count += xmas_matches.Count + samx_matches.Count;
            }

            // Flip the puzzle to vertical
            string[] crossword_vert = new string[crossword[0].Length];

            StringBuilder sb_col2row = new StringBuilder();

            for (int col = 0; col < crossword[0].Length; col++)
            {
                sb_col2row.Clear();

                for (int row = 0; row < crossword.Length; row++)
                {
                    sb_col2row.Append(crossword[row][col]);
                }

                crossword_vert[col] = sb_col2row.ToString();
            }

            // Vertical Matches
            for (int row = 0; row < crossword_vert.Length; row++)
            {
                MatchCollection xmas_matches = xmas_regex.Matches(crossword_vert[row]);
                MatchCollection samx_matches = samx_regex.Matches(crossword_vert[row]);

                xmas_count += xmas_matches.Count + samx_matches.Count;
            }

            // Flip the puzzle to diagonal right
            // Just create a list of diagonals long enough to hold the 4 chars
            List<string> crossword_diag_right = new List<string>();

            StringBuilder sb_2diag = new StringBuilder();

            // Bottom half of right diag
            for (int row = crossword.Length - 4; row >= 0; row--)
            {
                sb_2diag.Clear();
                int local_col = 0;

                for (int local_row = row; local_row < crossword.Length; local_row++)
                {
                    sb_2diag.Append(crossword[local_row][local_col]);
                    local_col++;
                }

                crossword_diag_right.Add(sb_2diag.ToString());
            }

            // Top half of right diag
            for (int col = crossword[0].Length - 4; col >= 1; col--)
            {
                sb_2diag.Clear();
                int local_row = 0;

                for (int local_col = col; local_col < crossword[0].Length; local_col++)
                {
                    sb_2diag.Append(crossword[local_row][local_col]);
                    local_row++;
                }

                crossword_diag_right.Add(sb_2diag.ToString());
            }

            // Diagonal Right Matches
            for (int diag = 0; diag < crossword_diag_right.Count; diag++)
            {
                MatchCollection xmas_matches = xmas_regex.Matches(crossword_diag_right[diag]);
                MatchCollection samx_matches = samx_regex.Matches(crossword_diag_right[diag]);

                xmas_count += xmas_matches.Count + samx_matches.Count;
            }

            // Flip the puzzle to diagonal left
            // Just create a list of diagonals long enough to hold the 4 chars
            List<string> crossword_diag_left = new List<string>();

            // Bottom half of left diag
            for (int row = crossword.Length - 4; row >= 0; row--)
            {
                sb_2diag.Clear();
                int local_col = crossword[0].Length - 1;

                for (int local_row = row; local_row < crossword.Length; local_row++)
                {
                    sb_2diag.Append(crossword[local_row][local_col]);
                    local_col--;
                }

                crossword_diag_left.Add(sb_2diag.ToString());
            }

            // Top half of left diag
            for (int col = crossword[0].Length - 2; col >= 3; col--)
            {
                sb_2diag.Clear();
                int local_row = 0;

                for (int local_col = col; local_col >= 0; local_col--)
                {
                    sb_2diag.Append(crossword[local_row][local_col]);
                    local_row++;
                }

                crossword_diag_left.Add(sb_2diag.ToString());
            }

            // Diagonal Left Matches
            for (int diag = 0; diag < crossword_diag_left.Count; diag++)
            {
                MatchCollection xmas_matches = xmas_regex.Matches(crossword_diag_left[diag]);
                MatchCollection samx_matches = samx_regex.Matches(crossword_diag_left[diag]);

                xmas_count += xmas_matches.Count + samx_matches.Count;
            }

            Console.WriteLine($"XMAS appears in the crossword puzzle {xmas_count} times");
        }
    }
}