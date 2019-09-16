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
            
            
                //Directory.GetFiles(txtBxDirName.Text, txtBxFileName.Text, SearchOption.AllDirectories);
            
            
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
            var FilesInDirectory = txtBxDirName.Text;
            var Pattern = txtBxFileName.Text;
            List<string> paths = new List<string>();
            var count = 0;
            char PathSeparator = '\\';
            DirectoryInfo workDirectory = new DirectoryInfo(FilesInDirectory);

            IEnumerable<string> SearchedFiles = workDirectory.GetFiles(Pattern, SearchOption.AllDirectories).Select(f => f.FullName.Substring(f.FullName.LastIndexOf(FilesInDirectory))).ToList();
            progressBar1.Invoke(new Action(() => progressBar1.Maximum = SearchedFiles.Count()));
            SearchedFiles.Where(s => File.ReadAllText(s).Contains(txtBxSearch.Text)).ToList();
            //try
            //{
            //    foreach (var f in SearchedFiles)
            //    {
            //        var root = new TreeNode();
            //        _stopper.WaitOne();
            //        lblFilename.Invoke(new Action(() => lblFilename.Text = f));

            //        SearchedFiles.Where(s => File.ReadAllText(s).Contains(txtBxSearch.Text)).ToList();
                   
            //        count++;
            //        progressBar1.Invoke(new Action(() => progressBar1.Value = count));
            //        lblFileCount.Invoke(new Action(() => lblFileCount.Text = $"Файлов обработано {count}"));
            //        treeView1.Invoke(new Action(() => treeView1.Nodes.Add(root)));
            //    }

            //    _isWork = false;
            //}
            //catch (System.Exception excp)
            //{ }

            OutputTreeView(treeView1, SearchedFiles, PathSeparator);

          // if (count == FilesInDirectory.Length) _stopper.Close();
            
        }


        private static void OutputTreeView(TreeView TreeviewNode, IEnumerable<string> UniqueFilesPath, char PathSeparator)
        {
            TreeNode LastNode = null;

            foreach (string PathToFile in UniqueFilesPath)
            {
                string SubPathAgg = string.Empty;

                foreach (string SubPath in PathToFile.Split(PathSeparator))
                {
                    SubPathAgg += SubPath + PathSeparator;

                    TreeNode[] Nodes = TreeviewNode.Nodes.Find(SubPathAgg, true);

                    if (Nodes.Length == 0)
                    {
                        if (LastNode == null)
                        {
                            LastNode = TreeviewNode.Nodes.Add(SubPathAgg, SubPath);
                        }
                        else
                        {
                            LastNode = LastNode.Nodes.Add(SubPathAgg, SubPath);
                        }
                    }
                    else
                    {
                        LastNode = Nodes[0];
                    }
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
