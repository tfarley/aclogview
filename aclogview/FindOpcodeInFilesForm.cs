using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

using aclogview.Properties;

namespace aclogview
{
    public partial class FindOpcodeInFilesForm : Form
    {
        public FindOpcodeInFilesForm()
        {
            InitializeComponent();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            txtSearchPathRoot.Text = Settings.Default.FindOpcodeInFilesRoot;
            txtOpcode.Text = Settings.Default.FindOpcodeInFilesOpcode.ToString("X4");

            typeof(DataGridView).InvokeMember("DoubleBuffered", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.SetProperty, null, dataGridView1, new object[] { true });
            dataGridView1.RowHeadersVisible = false;
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView1.Columns[0].ValueType = typeof(int);
            dataGridView1.Columns[1].ValueType = typeof(int);
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            searchAborted = true;

            Settings.Default.FindOpcodeInFilesRoot = txtSearchPathRoot.Text;
            Settings.Default.FindOpcodeInFilesOpcode = OpCode;

            base.OnClosing(e);
        }

        int OpCode
        {
            get
            {
                int value;

                int.TryParse(txtOpcode.Text, NumberStyles.HexNumber, null, out value);

                return value;
            }
        }

        private void btnChangeSearchPathRoot_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog openFolder = new FolderBrowserDialog())
            {
                if (openFolder.ShowDialog() == DialogResult.OK)
                    txtSearchPathRoot.Text = openFolder.SelectedPath;
            }
        }


        private readonly List<string> filesToProcess = new List<string>();
        private int opCodeToSearchFor;
        private int filesProcessed;
        private bool searchAborted;

        private class ProcessFileResut
        {
            public int Hits;
            public string FileName;
        }

        private readonly ConcurrentBag<ProcessFileResut> processFileResuts = new ConcurrentBag<ProcessFileResut>();

        private void btnStartSearch_Click(object sender, EventArgs e)
        {
            dataGridView1.RowCount = 0;

            try
            {
                btnStartSearch.Enabled = false;

                filesToProcess.Clear();
                opCodeToSearchFor = OpCode;
                filesProcessed = 0;
                searchAborted = false;

                ProcessFileResut result;
                while (!processFileResuts.IsEmpty)
                    processFileResuts.TryTake(out result);

                filesToProcess.AddRange(Directory.GetFiles(txtSearchPathRoot.Text, "*.pcap", SearchOption.AllDirectories));
                filesToProcess.AddRange(Directory.GetFiles(txtSearchPathRoot.Text, "*.pcapng", SearchOption.AllDirectories));

                toolStripStatusLabel1.Text = "Files Processed: 0 of " + filesToProcess.Count;

                txtSearchPathRoot.Enabled = false;
                txtOpcode.Enabled = false;
                btnChangeSearchPathRoot.Enabled = false;
                btnStopSearch.Enabled = true;

                timer1.Start();

                new Thread(() =>
                {
                    // Do the actual search here
                    DoSearch();

                    if (!Disposing && !IsDisposed)
                        btnStopSearch.BeginInvoke((Action)(() => btnStopSearch_Click(null, null)));
                }).Start();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());

                btnStopSearch_Click(null, null);
            }
        }

        private void btnStopSearch_Click(object sender, EventArgs e)
        {
            searchAborted = true;

            timer1.Stop();

            timer1_Tick(null, null);

            txtSearchPathRoot.Enabled = true;
            txtOpcode.Enabled = true;
            btnChangeSearchPathRoot.Enabled = true;
            btnStartSearch.Enabled = true;
            btnStopSearch.Enabled = false;
        }


        private void DoSearch()
        {
            Parallel.ForEach(filesToProcess, (currentFile) =>
            {
                if (searchAborted || Disposing || IsDisposed)
                    return;

                try
                {
                    ProcessFile(currentFile);
                }
                catch { }
            });
        }

        private void ProcessFile(string fileName)
        {
            int hits = 0;

            var records = PCapReader.LoadPcap(fileName, ref searchAborted);

            if (searchAborted || Disposing || IsDisposed)
                return;

            foreach (var record in records)
            {
                if (record.opcodes.Contains((PacketOpcode)opCodeToSearchFor))
                    hits++;

                /*foreach (BlobFrag frag in record.netPacket.fragList_)
                {
                    if (frag.dat_.Length > 20)
                    {
                        BinaryReader fragDataReader = new BinaryReader(new MemoryStream(frag.dat_));

                        // Custom search can go here
                    }
                }*/
            }

            Interlocked.Increment(ref filesProcessed);

            processFileResuts.Add(new ProcessFileResut() { Hits = hits, FileName = fileName });
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            ProcessFileResut result;
            while (!processFileResuts.IsEmpty)
            {
                if (processFileResuts.TryTake(out result))
                {
                    var length = new FileInfo(result.FileName).Length;

                    if (result.Hits > 0)
                        dataGridView1.Rows.Add(result.Hits, length, result.FileName);
                }
            }

            toolStripStatusLabel1.Text = "Files Processed: " + filesProcessed + " of " + filesToProcess.Count;
        }


        private void dataGridView1_CellContentDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            var fileName = (string)dataGridView1.Rows[e.RowIndex].Cells[2].Value;

            System.Diagnostics.Process.Start(Application.ExecutablePath, '"' + fileName + '"');
        }
    }
}
