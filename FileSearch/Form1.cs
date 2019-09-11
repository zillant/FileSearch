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

namespace FileSearch
{
    public partial class Form1 : Form
    {

        private static ManualResetEvent _stopper = new ManualResetEvent(false);
        private delegate void EventHandle();
        bool _paused;
        Thread _SearchThrd;
        string[] _FilesInDirectory;
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
            _FilesInDirectory = Directory.GetFiles(txtBxDirName.Text, txtBxFileName.Text, SearchOption.AllDirectories);
            progressBar1.Maximum = _FilesInDirectory.Length;
            
            _SearchThrd = new Thread(new ThreadStart(SearchFile));
           
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

            var FilesInDirectory = _FilesInDirectory;

            var count = 0;
                  
            
            foreach (var f in FilesInDirectory)
            {
                _stopper.WaitOne();
                if (File.ReadAllText(f).Contains(txtBxSearch.Text))
                {
                    listBox1.Invoke(new Action(() => listBox1.Items.Add(f)));
                }
                count++;
                progressBar1.Invoke(new Action(() => progressBar1.Value = count));
            }

          // if (count == FilesInDirectory.Length) _stopper.Close();
            
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (!_paused)
            {
                StopSearchThread();
                button2.Text = "Возобновить";
            }
            else
            {
                ContinueSearchThread();
                button2.Text = "Остановить";
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
    }
}
