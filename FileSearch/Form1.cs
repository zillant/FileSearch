using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Threading;
using System.Diagnostics;

namespace FileSearch
{
    public partial class Form1 : Form
    {
        public IEnumerable<FileSystemEntry> FileSystemEntries { get; }
        private static ManualResetEvent _stopper = new ManualResetEvent(false);
        private delegate void EventHandle();
        bool _paused;
        Thread _SearchThrd;
        string[] _FilesInDirectory;
        bool _isWork;
        DateTime startTime;
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
                txtBxDirName.Text = folderBrowserDialog1.SelectedPath;

        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            startTime = DateTime.Now;
            treeView1.PathSeparator = @"\";
            _FilesInDirectory = Directory.GetFiles(txtBxDirName.Text, txtBxFileName.Text, SearchOption.AllDirectories);
            progressBar1.Maximum = _FilesInDirectory.Length;
            
            _SearchThrd = new Thread(new ThreadStart(SearchFile));
            timer1.Start();

            if (txtBxSearch.Text == String.Empty) MessageBox.Show("Заполните поле Что ищем?");
            if (txtBxFileName.Text == String.Empty) MessageBox.Show("Заполните поле Шаблон файла");

            _paused = false;
            button2.Text = "Остановить";

            if ((txtBxSearch.Text != String.Empty) && (txtBxFileName.Text != String.Empty))
                _SearchThrd.Start();

            
            _stopper.Set();

        }

        private void SearchFile()
        {
            _isWork = true;
            var FilesInDirectory = _FilesInDirectory;
            List<string> paths = new List<string>();
            var count = 0;
            var root = new TreeNode();
            treeView1.Invoke(new Action(() => treeView1.Nodes.Add(root)));
            
            try
            {
                foreach (var f in FilesInDirectory)
                {
                    _stopper.WaitOne();
                    lblFilename.Invoke(new Action(() => lblFilename.Text = f));
                    if (File.ReadAllText(f).Contains(txtBxSearch.Text))
                    {
                        //listBox1.Invoke(new Action(() => listBox1.Items.Add(f)));
                        paths.Add(f);
                        root.
                    }
                    count++;
                    progressBar1.Invoke(new Action(() => progressBar1.Value = count));
                    lblFileCount.Invoke(new Action(() => lblFileCount.Text = $"Файлов обработано {count}"));
                }

                _isWork = false;
            }
            catch (System.Exception excp)
            { }

          // if (count == FilesInDirectory.Length) _stopper.Close();
            
        }

        private static void PopulateTreeView(TreeView treeView, IEnumerable<string> paths, char pathSeparator)
        {
            TreeNode lastNode = null;
            string subPathAgg;
            foreach (string path in paths)
            {
                subPathAgg = string.Empty;
                foreach (string subPath in path.Split(pathSeparator))
                {
                    subPathAgg += subPath + pathSeparator;
                    TreeNode[] nodes = treeView.Nodes.Find(subPathAgg, true);
                    if (nodes.Length == 0)
                        if (lastNode == null)
                            lastNode = treeView.Nodes.Add(subPathAgg, subPath);
                        else
                            lastNode = lastNode.Nodes.Add(subPathAgg, subPath);
                    else
                        lastNode = nodes[0];
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (!_paused)
            {
                StopSearchThread();
                button2.Text = "Возобновить";
                _isWork = false;
            }
            else
            {
                ContinueSearchThread();
                button2.Text = "Остановить";
                _isWork = true;
            }
        }

        private void ContinueSearchThread()
        {
            _stopper.Set();
            _paused = false;
        }

        private void StopSearchThread()
        {
            _paused = true;
            _stopper.Reset();
           
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            var ts = DateTime.Now - startTime;
            string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}", ts.Hours, ts.Minutes, ts.Seconds, ts.Milliseconds / 10);
            if (_isWork)
            {
                lblElapsedTime.Invoke(new Action(() => lblElapsedTime.Text = elapsedTime));
            }
        }
    }


}
