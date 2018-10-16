using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Stopwatch
{
    public partial class Form1 : Form
    {
        Stopwatch stopwatch = new Stopwatch(); //Initialize stopwatch object

        public Form1()
        {
            InitializeComponent();
        }

        private void startButton_Click(object sender, EventArgs e) //Starts stopwatch - calls Start() method
        {
            stopwatch.Start();
        }

        private void stopButton_Click(object sender, EventArgs e) //Stops stopwatch - calls Stop() method, checks if stopwatch is started to turn it off, shows time on list
        {
            if (stopwatch.IsStarted)
            {
                stopwatch.Stop();
                timesList.Items.Add(stopwatch.TimeToShow);
            }
        }

        private void resetButton_Click(object sender, EventArgs e) //Resets stopwatch - calls Reset() method, clears times list
        {
            stopwatch.Reset();
            timesList.Items.Clear();
        }
    }
}
