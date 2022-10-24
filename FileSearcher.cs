/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 */

using System.Text.RegularExpressions;

namespace HungarianNotationHunter
{
    internal static class FileSearcher
    {
        const bool STRICT_MODE = true; // Set to true to enable a more specific regex to try to target variable declarations.

        public class HungarianCounter
        {
            public List<(string, int, string)> LineOccurrences;
            public int Count { get { return LineOccurrences.Count; } }

            public HungarianCounter()
            {
                LineOccurrences = new List<(string, int, string)>();
            }

            public void AddOccurrence(string file, int lineNumber, string rawLine)
            {
                LineOccurrences.Add((Path.GetFileName(file),lineNumber,rawLine));
            }
        }



        public static IEnumerable<string> Search(string rootDir, List<string> fileExts)
        {
            var dirQueue = new Queue<string>();
            dirQueue.Enqueue(rootDir);

            while (dirQueue.Count > 0)
            {
                string dir = dirQueue.Dequeue();
                foreach(string file in Directory.GetFiles(dir).Where(x => fileExts.Any(y => x.EndsWith(y))))
                {
                    yield return file;
                }
                
                foreach(var subdir in Directory.GetDirectories(dir))
                {
                    dirQueue.Enqueue(subdir);
                }
            }
        }



        public static Dictionary<string, HungarianCounter> GenericIdentifier(IEnumerable<string> filepaths)
        {
            Regex rgx;
            if (STRICT_MODE)
            {
                rgx = new Regex(@"[a-zA-Z][a-zA-Z0-9]+\s+(?<id>[a-z]+)[A-Z]");
            }
            else
            {
                rgx = new Regex(@"\s(?<id>[a-z]+)[A-Z]");
            }

            var matches = new Dictionary<string, HungarianCounter>();

            foreach(string filepath in filepaths)
            {
                int lineNumber = 0;
                foreach(string line in File.ReadLines(filepath))
                {
                    lineNumber++;

                    var m = rgx.Match(line);
                    if(m.Success)
                    {
                        string key = m.Groups["id"].Value;

                        if (!matches.ContainsKey(key))
                        {
                            matches.Add(key, new HungarianCounter());
                        }
                        matches[key].AddOccurrence(filepath, lineNumber, line.Trim());
                    }
                }
            }

            return matches;
        }
    }
}
