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

        public Thread thrHelicopterMovement;


        // Form Load Event
        private void frmGame_Load(object sender, EventArgs e)
        {

            // Create top Obstacles
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

            // Create Bottom Obstacles
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

            // Start Helicopter Movement Thread
            thrHelicopterMovement = new Thread(funcHelicopterMovement);
        }


        // Create Obstacle Timmer Tick
        private void timCreateObstacles_Tick(object sender, EventArgs e)
        {
            // Update Score
            intScore = intScore + 1;
            lblScore.Text = intScore.ToString();

            // Change speed based on Score
            if (intScore > 200 && intScore < 400)
            {
                timCreateObstacles.Interval = 50;
            }
            else if (intScore > 400 && intScore < 600)
            {
                timCreateObstacles.Interval = 25;
            }
            else if (intScore > 600 && intScore < 1000)
            {
                timCreateObstacles.Interval = 15;
            }
            else if (intScore > 1000)
            {
                timCreateObstacles.Interval = 2;
            }

            // Remove first obstacle from the top 
            Controls.Remove(lisTopObstacles.First());
            lisTopObstacles.First().Dispose();
            lisTopObstacles.RemoveAt(0);

            // Remove first obstacle from the bottom
            Controls.Remove(lisBottomObstacles.First());
            lisBottomObstacles.First().Dispose();
            lisBottomObstacles.RemoveAt(0);

            // Move each obstacle right 30px
            foreach (var conObstacle in lisTopObstacles)
            {
                conObstacle.Location = new Point(conObstacle.Location.X - 30, conObstacle.Location.Y);
            }

            foreach (var conObstacle in lisBottomObstacles)
            {
                conObstacle.Location = new Point(conObstacle.Location.X - 30, conObstacle.Location.Y);
            }

            // Create a new top obstacle
            PictureBox picbObstacles = new PictureBox
            {
                Name = "",
                Location = new Point(Size.Width - 30, 0),
                Size = new Size(30, 62),
                Margin = new Padding(0, 0, 0, 0),
                Height = rndRandomNumber.Next(1, 200),
                Image = Resources.obstacle,
            };
            lisTopObstacles.Add(picbObstacles);
            Controls.Add(picbObstacles);

            // Create a new bottom obstacle
            int intBottomObstacleHeightY = 0;
            intBottomObstacleHeightY = rndRandomNumber.Next(1, 200);
            PictureBox picbBObstacles = new PictureBox
            {
                Name = "",
                Location = new Point(Size.Width - 30, ClientSize.Height - intBottomObstacleHeightY),
                Size = new Size(30, 62),
                Margin = new Padding(0, 0, 0, 0),
                Height = intBottomObstacleHeightY,
                Image = Resources.obstacle,
            };
            lisBottomObstacles.Add(picbBObstacles);
            Controls.Add(picbBObstacles);
        }


        // Helicopter Movement
        private void funcHelicopterMovement()
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

        // Move the Helicopter Up if Left Mouse Button Pressed else move the Helicopter Down
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

        // Check if the Helicopter has hit an obstacle
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

        // Run when Game Over
        private void funcGameOver()
        {
            timCreateObstacles.Stop();
            thrHelicopterMovement.Suspend();

            // Ask user if they want to play again
            DialogResult dialogResult = MessageBox.Show("Game Over! \n \n Play again ?", "Game Over!", MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes)
            {
                thrHelicopterMovement.Resume();
                timCreateObstacles.Start();
            }
            else if (dialogResult == DialogResult.No)
            {
                Debug.WriteLine("exiting");
                Close();
            }
        }

        // Form Close Event
        private void frmGame_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                thrHelicopterMovement.Abort();
            }
            catch (ThreadStateException)
            {

            }
        }

        // Key Down Event
        private void frmGame_KeyDown(object sender, KeyEventArgs e)
        {

            // Space to start / pause game
            if (e.KeyCode == Keys.Space)
            {
                if (blnGameStart == false)
                {
                    blnGameStart = true;
                    blnGameActive = true;
                    timCreateObstacles.Start();
                    thrHelicopterMovement.Start();
                }

                else if (blnGameActive == true)
                {
                    blnGameActive = false;
                    timCreateObstacles.Stop();
                    thrHelicopterMovement.Suspend();
                }
                else if (blnGameActive == false)
                {
                    blnGameActive = true;
                    timCreateObstacles.Start();
                    thrHelicopterMovement.Resume();
                }
            }

            // Cheats
            if (e.KeyCode == Keys.E)
            {
                thrHelicopterMovement.Suspend();
            }
        }

        // Helicopter Location Changed Event
        private void picbHelicopter_LocationChanged(object sender, EventArgs e)
        {
            funcHelicopterHitCheck();
        }
    }

}
