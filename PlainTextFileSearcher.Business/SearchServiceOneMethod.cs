using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PlainTextFileSearcher.Business
{
    public class SearchServiceOneMethod
    {
        private List<string> filePaths = new List<string>();
        private List<Tuple<int, int, string>> allResults = new List<Tuple<int, int, string>>();
        private string searchWord;
        private CancellationToken cancellationToken;

        public SearchServiceOneMethod(string startPath, string searchWord, CancellationToken token)
        {
            filePaths.AddRange(Directory.GetFiles(startPath, "*", SearchOption.AllDirectories));
            this.searchWord = searchWord;
            cancellationToken = token;
        }

        public Task<List<Tuple<int, int, string>>> FindInAllFiles()
        {
            for (int i = 0; i < filePaths.Count; i++)
            {
                var lines = File.ReadAllLines(filePaths[i]);
                var fileResults = new List<Tuple<int, int>>();

                if (cancellationToken.IsCancellationRequested)
                {
                    return Task.FromResult(allResults);
                }

                for (int j = 0; j < lines.Length; j++)
                {
                    List<int> lineResults = new List<int>();
                    int nextIndex = 0;

                    while (nextIndex >= 0 && nextIndex + searchWord.Length <= lines.Length)
                    {
                        if (nextIndex != -1)
                            nextIndex = lines[j].IndexOf(searchWord, nextIndex, StringComparison.OrdinalIgnoreCase); 

                        if (nextIndex >= 0)
                        {
                            lineResults.Add(nextIndex);
                            nextIndex += searchWord.Length;
                        }
                    }

                    for (int r = 0; r < lineResults.Count; r++)
                    {
                        fileResults.Add(new Tuple<int, int>(j, lineResults[r]));
                    }
                }

                var fileResultsCount = fileResults.Count;

                for (int h = 0; h < fileResultsCount; h++)
                {
                    allResults.Add(new Tuple<int, int, string>(fileResults[h].Item1, fileResults[h].Item2, Path.GetFileName(filePaths[i])));
                }
            }

            return Task.FromResult(allResults);
        }
    }
}
