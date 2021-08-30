using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace PlainTextFileSearcher.Business
{
    public class SearchService
    {
        private List<string> filePaths = new List<string>();
        private List<Tuple<int, int, string>> allResults = new List<Tuple<int, int, string>>();
        private string searchWord;

        public SearchService(string startPath, string searchWord)
        {
            filePaths.AddRange(Directory.GetFiles(startPath, "*", SearchOption.AllDirectories));
            this.searchWord = searchWord;
        }

        public async Task<List<Tuple<int, int, string>>> FindInAllFiles()
        {
            for (int i = 0; i < filePaths.Count; i++)
            {
                var fileResults = await FindInFile(filePaths[i]);

                var fileResultsCount = fileResults.Count;

                for (int h = 0; h < fileResultsCount; h++)
                {
                    allResults.Add(new Tuple<int, int, string> (fileResults[h].Item1, fileResults[h].Item2, Path.GetFileName(filePaths[i])));
                }
            }

            return allResults;
        }

        private async Task<List<Tuple<int, int>>> FindInFile(string filePath)
        {
            var lines = File.ReadAllLines(filePath);
            var fileResults = new List<Tuple<int, int>>();

            for (int i = 0; i < lines.Length; i++)
            {
                List<int> lineResults = await FindAllInString(lines[i]);

                for (int r = 0; r < lineResults.Count; r++)
                {
                    fileResults.Add(new Tuple<int, int>(i, lineResults[r]));
                }
            }

            return fileResults;
        }

        private Task<List<int>> FindAllInString(string line)
        {
            List<int> results = new List<int>();
            int nextIndex = 0;

            while (nextIndex >= 0 && nextIndex + searchWord.Length <= line.Length)
            {
                nextIndex = NextIndexOf(line, nextIndex);
                if (nextIndex >= 0)
                {
                    results.Add(nextIndex);
                    nextIndex += searchWord.Length;
                }
            }

            return Task.FromResult(results);
        }

        private int NextIndexOf(string input, int startIndex)
        {
            if (startIndex == -1)
                return -1;
            var result = input.IndexOf(searchWord, startIndex, StringComparison.OrdinalIgnoreCase);
            return result;
        }
    }
}
