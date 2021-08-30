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
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PlainTextFileSearcher.Winforms
{
    public partial class Form1 : Form
    {
        private const string SEARCH = "Search";
        private const string CANCEL = "Cancel";

        public Form1()
        {
            InitializeComponent();
            btnSearch.Enabled = true;
        }

        private async void btnSearch_Click(object sender, EventArgs e)
        {
            if (btnSearch.Text == SEARCH)
            {
                var stopwatch = new Stopwatch();
                var stopwatchOneMethod = new Stopwatch();

                btnSearch.Text = CANCEL;
                var filePath = @"C:\Users\CDHel\Desktop\articles";

                stopwatch.Start();
                var SearchService = new SearchService(filePath, tbxSearch.Text);
                var results = await SearchService.FindInAllFiles();
                stopwatch.Stop();

                stopwatchOneMethod.Start();
                var SearchServiceOneMethod = new SearchServiceOneMethod(filePath, tbxSearch.Text);
                var resultsOneMethod = await SearchServiceOneMethod.FindInAllFiles();
                stopwatchOneMethod.Stop();

                tbxSearchResults.Text += Environment.NewLine + $"Loop Method MilliSecondsTime: {stopwatch.ElapsedMilliseconds}";
                tbxSearchResults.Text += Environment.NewLine + $"OneMethod MilliSecondsTime: {stopwatchOneMethod.ElapsedMilliseconds}";
                tbxSearchResults.Text += Environment.NewLine + $"Milliseconds Saved: {stopwatch.ElapsedMilliseconds - stopwatchOneMethod.ElapsedMilliseconds}";
                tbxSearchResults.Text += Environment.NewLine;
                tbxSearchResults.Text += Environment.NewLine + "~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~";
                tbxSearchResults.Text += Environment.NewLine;
                tbxSearchResults.Text += string.Join(Environment.NewLine, results);
            }
            else
            {
                btnSearch.Text = SEARCH;

                //TODO cancelation
            }
        }

        private void btnOpenFolder_Click(object sender, EventArgs e)
        {
            folderBrowserDialog.ShowDialog();
            lblOpenedFolder.Text = folderBrowserDialog.SelectedPath;
            btnSearch.Enabled = true;
        }
    }
}
