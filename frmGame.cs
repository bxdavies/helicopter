using helicopter.Properties; // Imports Images
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace helicopter
{
    public partial class frmGame : Form
    {
        public frmGame()
        {
            InitializeComponent();
            this.SetStyle(ControlStyles.DoubleBuffer | ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint, true);
            this.UpdateStyles();

        }

        // Declare Public Varibales
        public List<Control> lisTopObstacles = new List<Control>();
        public List<Control> lisBottomObstacles = new List<Control>();
        public Random rndRandomNumber = new Random();
        public int intScore = 0;
        public bool blnGameStart = false;
        public bool blnGameActive = false;

        public Thread t;
        private void frmGame_Load(object sender, EventArgs e)
        {
            for (int i = 0; i < 22; i++)
            {
                PictureBox picbObstacles = new PictureBox
                {
                    Name = "",
                    Location = new Point(i * 30, 0),
                    Size = new Size(30, 62),
                    Margin = new Padding(0, 0, 0, 0),
                    Height = rndRandomNumber.Next(1, 200),
                    Image = Resources.obstacle,
                };
                lisTopObstacles.Add(picbObstacles);
                Controls.Add(picbObstacles);
            }

            int intBottomObstacleHeightY = 0;
            for (int i = 0; i < 22; i++)
            {
                intBottomObstacleHeightY = rndRandomNumber.Next(1, 200);
                PictureBox picbObstacles = new PictureBox
                {
                    Name = "",
                    Location = new Point(i * 30, ClientSize.Height - intBottomObstacleHeightY),
                    Size = new Size(30, 62),
                    Margin = new Padding(0, 0, 0, 0),
                    Height = intBottomObstacleHeightY,
                    Image = Resources.obstacle,
                };
                lisBottomObstacles.Add(picbObstacles);
                Controls.Add(picbObstacles);
            }

            // Start 
            t = new Thread(thHelicopterMovement);

            
        }


        private void timCreateObstacles_Tick(object sender, EventArgs e)
        {
            // Update Score
            intScore = intScore + 1;
            lblScore.Text = intScore.ToString();

            // Change speed based on Score
            if (intScore > 200)
            {
                timCreateObstacles.Interval = 50;
            }
            else if (intScore > 400)
            {
                timCreateObstacles.Interval = 25;
            }
            else if (intScore > 600)
            {
                timCreateObstacles.Interval = 15;
            }
            else if (intScore > 1000)
            {
                timCreateObstacles.Interval = 2;
            }

            // Remove obstacle
            Controls.Remove(lisTopObstacles.First());
            lisTopObstacles.First().Dispose();
            lisTopObstacles.RemoveAt(0);

            Controls.Remove(lisBottomObstacles.First());
            lisBottomObstacles.First().Dispose();
            lisBottomObstacles.RemoveAt(0);
            foreach (var conObstacle in lisTopObstacles)
            {
                conObstacle.Location = new Point(conObstacle.Location.X - 30, conObstacle.Location.Y);
            }

            foreach (var conObstacle in lisBottomObstacles)
            {
                conObstacle.Location = new Point(conObstacle.Location.X - 30, conObstacle.Location.Y);
            }

            // Create new obstalkes
            PictureBox picbObstacles = new PictureBox
            {
                Name = "",
                Location = new Point(660 - 30, 0),
                Size = new Size(30, 62),
                Margin = new Padding(0, 0, 0, 0),
                Height = rndRandomNumber.Next(1, 200),
                Image = Resources.obstacle,
            };
            lisTopObstacles.Add(picbObstacles);
            Controls.Add(picbObstacles);

            int intBottomObstacleHeightY = 0;
            intBottomObstacleHeightY = rndRandomNumber.Next(1, 200);
            PictureBox picbBObstacles = new PictureBox
            {
                Name = "",
                Location = new Point(660 - 30, ClientSize.Height - intBottomObstacleHeightY),
                Size = new Size(30, 62),
                Margin = new Padding(0, 0, 0, 0),
                Height = intBottomObstacleHeightY,
                Image = Resources.obstacle,
            };
            lisBottomObstacles.Add(picbBObstacles);
            Controls.Add(picbBObstacles);

        }

        private void thHelicopterMovement()
        {
            while (true)
            {
                if (InvokeRequired)
                {
                    picbHelicopter.Invoke(new MethodInvoker(funcMoveHelicopter));
                }
                else
                {
                    funcMoveHelicopter();
                }
                Thread.Sleep(10);
            }
        }

        private void funcMoveHelicopter()
        {
            System.Windows.Forms.MouseButtons pressedButtons = System.Windows.Forms.Control.MouseButtons;

            if (pressedButtons == MouseButtons.Left)
            {
                picbHelicopter.Location = new Point(picbHelicopter.Location.X, picbHelicopter.Location.Y - 3);
            }
            else
            {
                picbHelicopter.Location = new Point(picbHelicopter.Location.X, picbHelicopter.Location.Y + 3);
            }
        }



        private void funcHelicopterHitCheck()
        {
            if (picbHelicopter.Location.Y < lisTopObstacles.ElementAt(3).Height || picbHelicopter.Location.Y < lisTopObstacles.ElementAt(4).Height || picbHelicopter.Location.Y < lisTopObstacles.ElementAt(5).Height)
            {
                funcGameOver();
            }
            if (picbHelicopter.Location.Y > lisBottomObstacles.ElementAt(3).Location.Y || picbHelicopter.Location.Y > lisBottomObstacles.ElementAt(4).Location.Y || picbHelicopter.Location.Y > lisBottomObstacles.ElementAt(5).Location.Y)
            {
                funcGameOver();
            }
        }

        private void funcGameOver()
        {
            timCreateObstacles.Stop();
            t.Suspend();

            DialogResult dialogResult = MessageBox.Show("Game Over! \n \n Play again ?", "Game Over!", MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes)
            {
                t.Resume();
                timCreateObstacles.Start();
            }
            else if (dialogResult == DialogResult.No)
            {
                Close();
            }
        }

        private void frmGame_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                t.Abort();
            }
            catch (ThreadStateException)
            {
            }
        }

        private void frmGame_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Space)
            {
                if (blnGameStart == false)
                {
                    blnGameStart = true;
                    blnGameActive = true;
                    timCreateObstacles.Start();
                    t.Start();
                }

                else if (blnGameActive == true)
                {
                    blnGameActive = false;
                    timCreateObstacles.Stop();
                    t.Suspend();
                }
                else if (blnGameActive == false)
                {
                    blnGameActive = true;
                    timCreateObstacles.Start();
                    t.Resume();
                }
            }
        }

        private void picbHelicopter_LocationChanged(object sender, EventArgs e)
        {
            funcHelicopterHitCheck();
        }
    }

}
