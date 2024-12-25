using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace Day19_1
{
    internal class Program
    {
        internal class RKPatternMatch
        {
            private long _Hash;
            private int _SrcStartIdx;
            private int _Length;

            public RKPatternMatch(long hash, int start_idx, int length)
            {
                _Hash = hash;
                _SrcStartIdx = start_idx;
                _Length = length;
            }

            public int StartIdx()
            {
                return _SrcStartIdx;
            }

            public int Length()
            {
                return _Length;
            }

            public long GetMatchingHash()
            {
                return _Hash;
            }
        }
        
        internal class RKPattern
        {
            private string _Pattern;
            private long _Hash = 0;
            private int _Length = 0;
            private static long _Q = 347_811_194_367_163;
            private static int _R = 256;
            private long _RM = 0;
            
            public RKPattern(string pattern)
            {
                _Pattern = pattern;
                _Length = _Pattern.Length;
                _RM = 1;

                for (int i = 1; i <= _Length - 1; i++)
                    _RM = (_R * _RM) % _Q;
                _Hash = ComputeHash(pattern, _Length);
            }

            public static long ComputeHash(string key, int m)
            {
                long h = 0;
                for (int j = 0; j < m; j++)
                    h = (_R * h + (long)((byte)key[j])) % _Q;
                return h;
            }

            public long Hash()
            {
                return _Hash;
            }

            public int Length()
            {
                return _Length;
            }

            public int M()
            {
                return _Length;
            }

            public string Pattern()
            {
                return _Pattern;
            }

            public static long Q()
            {
                return _Q;
            }

            public long RM()
            {
                return _RM;
            }

            public static int R()
            {
                return _R;
            }
        }

        internal class RKMultiMatch
        {
            private Dictionary<long, RKPattern> _PatternHashes = new();
            private SortedList<int, List<long>> _PatternHashesByLength = new(
                Comparer<int>.Create((x, y) => y.CompareTo(x)));
            private string _Text;

            public RKMultiMatch()
            {
                _Text = string.Empty;
            }

            public RKMultiMatch(string text)
            {
                _Text = text;
            }

            public void AddPattern(string pattern)
            {
                RKPattern new_pattern = new(pattern);
                _PatternHashes.Add(new_pattern.Hash(), new_pattern);

                if (!_PatternHashesByLength.ContainsKey(new_pattern.Length()))
                    _PatternHashesByLength.Add(new_pattern.Length(), new List<long>());

                _PatternHashesByLength[new_pattern.Length()].Add(new_pattern.Hash());
            }

            public int PatternCount()
            {
                return _PatternHashes.Count;
            }

            public string GetPattern(long hash)
            {
                if (!_PatternHashes.ContainsKey(hash))
                    return string.Empty;

                return _PatternHashes[hash].Pattern();
            }

            public void SetText(string text)
            {
                _Text = text;
            }

            public (bool, Dictionary<int, List<RKPatternMatch>> Matches) FindAllPatterns()
            {
                Dictionary<int, List<RKPatternMatch>> pattern_matches = new();

                for (int i = 0; i < _PatternHashesByLength.Count; i++)
                {
                    int current_m = _PatternHashesByLength.Keys[i];
                    int n = _Text.Length;

                    long text_hash = RKPattern.ComputeHash(_Text, current_m);

                    if (_PatternHashesByLength.Values[i].Contains(text_hash))
                    {
                        RKPatternMatch new_match = new(text_hash, 0, current_m);
                        
                        if (pattern_matches.ContainsKey(0))
                            pattern_matches[0].Add(new_match);
                        else
                        {
                            pattern_matches.Add(0, new List<RKPatternMatch>());
                            pattern_matches[0].Add(new_match);
                        }
                    }

                    // RM is the same for any given pattern length m
                    long rm_at_length_m = _PatternHashes[_PatternHashesByLength.Values[i][0]].RM();

                    for (int j = current_m; j < n; j++)
                    {
                        text_hash = (text_hash + RKPattern.Q() - (rm_at_length_m * (long)((byte)_Text[j - current_m])) % RKPattern.Q());
                        text_hash = (text_hash * RKPattern.R() + (long)((byte)_Text[j])) % RKPattern.Q();

                        if (_PatternHashesByLength.Values[i].Contains(text_hash))
                        {
                            int new_start_idx = j - current_m + 1;
                            RKPatternMatch new_match = new(text_hash, new_start_idx, current_m);

                            if (pattern_matches.ContainsKey(new_start_idx))
                                pattern_matches[new_start_idx].Add(new_match);
                            else
                            {
                                pattern_matches.Add(new_start_idx, new List<RKPatternMatch>());
                                pattern_matches[new_start_idx].Add(new_match);
                            }
                        }
                    }
                }

                // for memoizing our failed paths (if a given length
                // has not worked starting at a given position in the
                // string, no other string of that given length will
                // work either!)
                Dictionary<int, List<int>> FitFailures = new();

                // try to fit our matches for 100% string coverage
                bool Fit(int start_idx, int depth)
                {
                    if (pattern_matches.ContainsKey(start_idx))
                    {
                        foreach (var match in pattern_matches[start_idx])
                        {
                            int match_len = match.Length();

                            if (FitFailures.ContainsKey(start_idx) && FitFailures[start_idx].Contains(match_len))
                                return false;

                            if (start_idx + match_len < _Text.Length)
                            {
                                bool fit = Fit(start_idx + match_len, depth + 1);

                                if (!fit)
                                {
                                    if (!FitFailures.ContainsKey(start_idx))
                                        FitFailures.Add(start_idx, new List<int>());

                                    FitFailures[start_idx].Add(match_len);

                                    continue;
                                }

                                return fit;
                            }
                            else
                                return true;
                        }
                    }
                    
                    return false;
                }

                return (Fit(0, 0), pattern_matches);
            }
        }
        
        static void Main(string[] args)
        {
            RKMultiMatch MultiMatch = new();
            List<string> designs = new();

            using (StreamReader sr = new("Input.txt"))
            {
                try
                {
                    string[] lines = sr.ReadToEnd().Split("\r\n");
                    string[] patterns = lines[0].Split(", ");
                    foreach (string p in patterns)
                    {
                        MultiMatch.AddPattern(p);
                    }

                    for (int i = 2; i < lines.Length; i++)
                    {
                        designs.Add(lines[i].Trim());
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

            int possible_patterns = 0;

            foreach (string design in designs)
            {
                MultiMatch.SetText(design);

                (bool patterns_fit_design, Dictionary<int, List<RKPatternMatch>> matches) = MultiMatch.FindAllPatterns();

                if (patterns_fit_design)
                    possible_patterns++;
            }

            Console.WriteLine($"After reviewing {designs.Count} designs and taking into the account the {MultiMatch.PatternCount()} available patterns, {possible_patterns} of those designs are possible.");
        }
    }
}
