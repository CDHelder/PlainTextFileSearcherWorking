using PlainTextFileSearcher.Business;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PlainTextFileSearcher.Winforms
{
    public partial class Form1 : Form
    {
        private const string SEARCH = "Search";
        private const string CANCEL = "Cancel";

        public Stopwatch stopwatch1 = new();
        public Stopwatch stopwatchOneMethod = new Stopwatch();
        public string filePath = @"C:\Users\CDHel\Desktop\articles";

        public CancellationTokenSource cTS;
        public Task searchServiceTask;
        public SearchService searchService;
        public Task searchServiceOneMethodTask;
        public SearchServiceOneMethod searchServiceOneMethod;
        public List<Tuple<int, int, string>> results;

        public Form1()
        {
            InitializeComponent();
            btnSearch.Enabled = true;
        }

        public void SearchInAllFiles()
        {
            results = searchService.FindInAllFiles();
        }

        private async void btnSearch_Click(object sender, EventArgs e)
        {
            if (btnSearch.Text == SEARCH)
            {
                btnSearch.Text = CANCEL;

                cTS = new CancellationTokenSource();
                var token = cTS.Token;

                stopwatch1.Start();
                await LoopMethodService(token);
                stopwatch1.Stop();

                //await OneMethodService(token);

                tbxSearchResults.Text = string.Join(Environment.NewLine, results);
            }
            else
            {
                btnSearch.Text = SEARCH;
                tbxSearchResults.Text = "";

                cTS.Cancel();

                var sdfg = cTS.IsCancellationRequested;
                //tbxSearchResults.Text = string.Join(Environment.NewLine, results.Result);

            }
        }

        private async Task LoopMethodService(CancellationToken token)
        {
            searchService = new SearchService(filePath, tbxSearch.Text, token);
            searchServiceTask = Task.Run(() =>
            {
                SearchInAllFiles();
            }, token);
            await searchServiceTask;
        }

        private async Task OneMethodService(CancellationToken token)
        {
            stopwatchOneMethod.Start();
            searchServiceOneMethod = new SearchServiceOneMethod(filePath, tbxSearch.Text, token);
            searchServiceOneMethodTask = Task.Run(() =>
            {
                searchServiceOneMethod.FindInAllFiles();
            }, token);
            await searchServiceOneMethodTask;
            stopwatchOneMethod.Stop();

            tbxSearchResults.Text += Environment.NewLine + $"Loop Methods MilliSecondsTime: {stopwatch1.ElapsedMilliseconds}";
            tbxSearchResults.Text += Environment.NewLine + $"All In One Method MilliSecondsTime: {stopwatchOneMethod.ElapsedMilliseconds}";
            tbxSearchResults.Text += Environment.NewLine + $"Milliseconds Saved: {stopwatch1.ElapsedMilliseconds - stopwatchOneMethod.ElapsedMilliseconds}";
            tbxSearchResults.Text += Environment.NewLine;
            tbxSearchResults.Text += Environment.NewLine + "~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~";
            tbxSearchResults.Text += Environment.NewLine;
        }

        private void btnOpenFolder_Click(object sender, EventArgs e)
        {
            folderBrowserDialog.ShowDialog();
            lblOpenedFolder.Text = folderBrowserDialog.SelectedPath;
            btnSearch.Enabled = true;
        }
    }
}
