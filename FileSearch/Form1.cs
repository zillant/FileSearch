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

       // private static ManualResetEvent _stopper = new ManualResetEvent(false);
        bool _paused;
        Thread _SearchThrd;
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
            _SearchThrd = new Thread(new ThreadStart(SearchFile));
            
            if (txtBxSearch.Text == String.Empty) MessageBox.Show("Заполните поле Что ищем?");
            if (txtBxFileName.Text == String.Empty) MessageBox.Show("Заполните поле Шаблон файла");

            _paused = false;
            if ((txtBxSearch.Text != String.Empty) && (txtBxFileName.Text != String.Empty))
                _SearchThrd.Start();

        }

        private void SearchFile()
        {
            string[] FilesInDirectory = Directory.GetFiles(txtBxDirName.Text, txtBxFileName.Text, SearchOption.AllDirectories);

            progressBar1.Maximum = FilesInDirectory.Length;
            var count = 0;
            foreach (var f in FilesInDirectory)
            {
                if (File.ReadAllText(f).Contains(txtBxSearch.Text))
                    listBox1.Items.Add(f);
                count++;
                progressBar1.Value = count;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (!_paused)
            {
                StopSearchThread();
                button2.Text = "Возобновить";

            }
        }

        private void StopSearchThread()
        {
           _SearchThrd.Sle
        }
    }
}
