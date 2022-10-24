/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 */

using Microsoft.WindowsAPICodePack.Dialogs;
using System.Text;
using System.Text.RegularExpressions;

namespace HungarianNotationHunter
{
    public partial class MainForm : Form
    {
        IEnumerable<string> ActiveFileTargets;
        Dictionary<string, FileSearcher.HungarianCounter> SymbolList;
        List<string> ValidKeys;

        public MainForm()
        {
            InitializeComponent();

            ActiveFileTargets = new List<string>();
            SymbolList = new Dictionary<string, FileSearcher.HungarianCounter>();
            ValidKeys = new List<string>();
        }




        private void button1_Click(object sender, EventArgs e)
        {
            using (CommonOpenFileDialog dlg = new CommonOpenFileDialog())
            {
                dlg.IsFolderPicker = true;

                if (dlg.ShowDialog() != CommonFileDialogResult.Ok)
                {
                    return;
                }

                string targetFolder = dlg.FileName;
                var fileExts = fileExtBox.Text.Split(",").Select(x => x.Trim()).ToList();
                ActiveFileTargets = FileSearcher.Search(targetFolder, fileExts);
                SymbolList = FileSearcher.GenericIdentifier(ActiveFileTargets);

                if (SymbolList.Any())
                {
                    StringBuilder msg = new StringBuilder();
                    foreach (var kvp in SymbolList.OrderByDescending(x => x.Value.Count))
                    {
                        msg.AppendLine($"{kvp.Key}:{kvp.Value.Count}");
                    }
                    msg.AppendLine();

                    ValidKeys = new List<string>();
                    richTextBox1.Text = msg.ToString();
                    button2.Enabled = true;
                    button3.Enabled = true;
                    button4.Enabled = true;
                }
            }
        }

        

        private void button2_Click(object sender, EventArgs e)
        {
            StringBuilder reportBuilder = new StringBuilder();

            if(ValidKeys.Count == 0)
            {
                CommitNotationList();
            }

            foreach (string key in ValidKeys)
            {
                reportBuilder.AppendLine($"---------- {key} ----------");
                foreach (var occurrence in SymbolList[key].LineOccurrences)
                {
                    reportBuilder.AppendLine($"\t{occurrence.Item1}:{occurrence.Item2}\t{occurrence.Item3}");
                }
                reportBuilder.AppendLine();
            }

            richTextBox1.Text = reportBuilder.ToString();
        }



        private void button3_Click(object sender, EventArgs e)
        {
            var occurrenceCounter = new Dictionary<string, List<(int, string)>>();

            if (ValidKeys.Count == 0)
            {
                CommitNotationList();
            }

            foreach (string key in ValidKeys)
            {
                foreach (var occurrence in SymbolList[key].LineOccurrences)
                {
                    string filename = occurrence.Item1;
                    if (!occurrenceCounter.ContainsKey(filename))
                    {
                        occurrenceCounter.Add(filename, new List<(int, string)>());
                    }
                    occurrenceCounter[filename].Add((occurrence.Item2, occurrence.Item3));
                }
            }

            var reportBuilder = new StringBuilder();
            foreach (var element in occurrenceCounter.OrderByDescending(x => x.Value.Count))
            {
                reportBuilder.AppendLine($"---------- {element.Key} ----------");
                foreach(var occurrence in element.Value)
                {
                    reportBuilder.AppendLine($"\t{occurrence.Item1}:\t{occurrence.Item2}");
                }
                reportBuilder.AppendLine();
            }

            richTextBox1.Text = reportBuilder.ToString();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            var occurrenceCounter = new Dictionary<string, int>();

            if (ValidKeys.Count == 0)
            {
                CommitNotationList();
            }

            foreach (string key in ValidKeys)
            {
                foreach (var occurrence in SymbolList[key].LineOccurrences)
                {
                    string filename = occurrence.Item1;
                    if (!occurrenceCounter.ContainsKey(filename))
                    {
                        occurrenceCounter.Add(filename, 0);
                    }
                    occurrenceCounter[filename]++;
                }
            }

            var reportBuilder = new StringBuilder();
            foreach (var element in occurrenceCounter.OrderByDescending(x => x.Value))
            {
                reportBuilder.AppendLine($"\t{element.Key}:\t{element.Value}");
            }
            reportBuilder.AppendLine();

            richTextBox1.Text = reportBuilder.ToString();
        }


        private void CommitNotationList()
        {
            foreach (string line in richTextBox1.Text.Split('\n'))
            {
                if (!line.Contains(':'))
                {
                    continue;
                }

                string key = line.Split(':')[0];
                ValidKeys.Add(key);
            }
        }
    }
}