using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Minesweeper
{

    public partial class Form1 : Form
    {
        bool flagMode; // testing rectangles or placing flags
        bool running;
        int sec = 0; // measuring time
        int min = 0;
        bool first = true; // first move
        int perSqrX = 1; //how many pixels are in one rectangle horizontally
        int perSqrY = 1;//how many pixels are in one rectangle vertically 
        int bombCurrent = BombTotal; //BombTotal - flags = bombCurrent

        public static int X = 16; //number of rectangles in a row
        public static int Y = 16; //number of rectangles in a collumn
        public static int BombTotal = 40; //number of bombs at the begining

        public Form1()
        {
            InitializeComponent();
        }

        //inicialize, hide buttons, show others, draw cage
        private void button1_Click(object sender, EventArgs e)
        {
            flagMode = false;

            label2.Text = string.Format("Mines left: {0}", BombTotal);
            timer1.Start();
            button1.Visible = false;
            button2.Visible = false;
            running = true;
            button3.Visible = false;
            button4.Visible = true;
            button5.Visible = true;
            button6.Visible = true;
            label1.Visible = true;
            label2.Visible = true;
            textBox4.Visible = true;
            perSqrX = (this.Width - 60) / X;
            perSqrY = (this.Height - 80) / Y;
            DrawCage();
            button5.Text = "I want to place flags";
        }
        //measuring time and displaying it
        private void timer1_Tick(object sender, EventArgs e)
        {
            sec += 1;
            if (sec == 60)
            {
                sec = 0;
                min += 1;
            }
            label1.Text = string.Format("Time {0}:{1}", min, sec);


        }

        // returning everything into start position
        public void GameOver()
        {
            running = false;
            first = true;
            System.Drawing.Graphics formGraphics = CreateGraphics();
            System.Drawing.SolidBrush drawBrush = new System.Drawing.SolidBrush(System.Drawing.Color.White);
            formGraphics.FillRectangle(drawBrush, 10, 30, this.Width, this.Height);

            textBox4.Visible = false;
            button1.Visible = true;
            button2.Visible = true;
            button3.Visible = true;
            button4.Visible = false;
            button5.Visible = false;
            button6.Visible = false;
            timer1.Stop();
            label1.Visible = false;
            label2.Visible = false;
            sec = 0; min = 0;
            bombCurrent = BombTotal;

        }

        //menu button
        private void button4_Click(object sender, EventArgs e)
        {
            GameOver(); ;
        }
        // procedure for drawing cage called by pressing button1 (start game)
        public void DrawCage()
        {
            System.Drawing.Graphics graphicsObj;
            graphicsObj = this.CreateGraphics();
            Pen myPen = new Pen(System.Drawing.Color.Black, 1);

            int cageWidth = X * perSqrX;
            int cageHeight = Y * perSqrY;
            graphicsObj.DrawLine(myPen, 10, 30, 10 + cageWidth, 30);
            graphicsObj.DrawLine(myPen, 10, 30, 10, 30 + cageHeight);
            graphicsObj.DrawLine(myPen, 10 + cageWidth, 30, 10 + cageWidth, 30 + cageHeight);
            graphicsObj.DrawLine(myPen, 10, 30 + cageHeight, 10 + cageWidth, 30 + cageHeight);

            for (int i = 1; i < X; i++)
            {
                graphicsObj.DrawLine(myPen, 10 + i * perSqrX, 30, 10 + i * perSqrX, 30 + cageHeight);
            }

            for (int i = 1; i < Y; i++)
            {
                graphicsObj.DrawLine(myPen, 10, 30 + perSqrY * i, 10 + cageWidth, 30 + perSqrY * i);
            }
        }
        //exit button
        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        //While game is "running" decide which rectangle is touched
        //and call Show which will check game logics
        private void Form1_MouseClick(object sender, MouseEventArgs e)
        {
            if (running)
            {
                int squareX = (e.X - 10) / (perSqrX);
                int squareY = (e.Y - 30) / (perSqrY);
                if ((squareX >= 0) && (squareX < X) && (squareY >= 0) && (squareY < Y))
                {
                    if (flagMode) { FillFlag(squareX, squareY); }
                    else
                    {
                        label1.Text = string.Format("{0},{1}", squareX, squareY);
                        if (first) { first = false; Map.GenerateMins(squareX, squareY); }
                        if (Map.RevealedMap[squareX, squareY] == 'n')
                        {
                            Show(squareX, squareY);
                        }
                    }
                }
            }
        }
        //called by mouse click. Draws red rectangle (flag)
        // or removes flag (depending on current status). Counts number of potential mines.
        //checks if user has won
        public void FillFlag(int Fx, int Fy)
        {
            int xx = 10 + (Fx) * perSqrX;
            int yy = 30 + (Fy) * perSqrY;
            System.Drawing.Graphics formGraphics = CreateGraphics();
            System.Drawing.SolidBrush drawBrush = new System.Drawing.SolidBrush(System.Drawing.Color.Red);
            if (Map.RevealedMap[Fx, Fy] == 'f')
            {
                drawBrush.Color = System.Drawing.Color.White;
                Map.RevealedMap[Fx, Fy] = 'n';
                bombCurrent += 1;
                label2.Text = string.Format("Bombs: {0}", bombCurrent);
                formGraphics.FillRectangle(drawBrush, new Rectangle(xx + 1, yy + 1, perSqrX - 1, perSqrY - 1));
            }
            else if ((Map.RevealedMap[Fx, Fy] == 'n') && (bombCurrent > 0))
            {
                bombCurrent -= 1;
                label2.Text = string.Format("Bombs: {0}", bombCurrent);
                Map.RevealedMap[Fx, Fy] = 'f';
                formGraphics.FillRectangle(drawBrush, new Rectangle(xx + 1, yy + 1, perSqrX - 1, perSqrY - 1));
                if (bombCurrent == 0)
                {
                    if (Map.CheckWin())
                    {
                        timer1.Stop();
                        MessageBox.Show("You have won!");
                    }
                }
            }
        }

        //make gray rectangle
        public void FillZero(int Fx, int Fy)
        {
            System.Drawing.Graphics formGraphics = CreateGraphics();
            System.Drawing.SolidBrush drawBrush = new System.Drawing.SolidBrush(System.Drawing.Color.DarkGray);
            formGraphics.FillRectangle(drawBrush, new Rectangle(Fx, Fy, perSqrX - 1, perSqrY - 1));
        }
        //Show result of player´s move and mark it into revealed map
        public void Show(int Sx, int Sy)
        {
            int xx = 10 + (Sx) * perSqrX;
            int yy = 30 + (Sy) * perSqrY;
            int Number = Map.Reveal(Sx, Sy);
            if (Number == -1) { MessageBox.Show("You have lost!"); GameOver(); }
            else
            {
                string drawString = string.Concat(Number);

                System.Drawing.Graphics formGraphics = CreateGraphics();
                System.Drawing.Font drawFont = new System.Drawing.Font("Arial", 3 * perSqrY / 4);
                System.Drawing.SolidBrush drawBrush = new System.Drawing.SolidBrush(System.Drawing.Color.Black);
                if (Number >= 9)
                {
                    Map.RevealedMap[Sx, Sy] = 'b';
                }
                else if (Number == 0)
                {
                    Map.RevealedMap[Sx, Sy] = 'v';
                }
                else
                {
                    string n = string.Concat(Number);
                    Map.RevealedMap[Sx, Sy] = n[0];
                }
                if (Number == 0)
                {

                    FillZero(xx + 1, yy + 1);
                    if (Sx < X - 1)
                    {
                        if (Map.RevealedMap[Sx + 1, Sy] == 'n') { Show(Sx + 1, Sy); }
                        if (Sy < Y - 1 && Map.RevealedMap[Sx + 1, Sy + 1] == 'n') { Show(Sx + 1, Sy + 1); }
                        if (Sy > 0 && Map.RevealedMap[Sx + 1, Sy - 1] == 'n') { Show(Sx + 1, Sy - 1); }
                    }
                    if (Sx > 0)
                    {
                        if (Map.RevealedMap[Sx - 1, Sy] == 'n') { Show(Sx - 1, Sy); }
                        if (Sy < Y - 1 && Map.RevealedMap[Sx - 1, Sy + 1] == 'n') { Show(Sx - 1, Sy + 1); }
                        if (Sy > 0 && Map.RevealedMap[Sx - 1, Sy - 1] == 'n') { Show(Sx - 1, Sy - 1); }
                    }
                    if (Sy < Y - 1 && Map.RevealedMap[Sx, Sy + 1] == 'n') { Show(Sx, Sy + 1); }
                    if (Sy > 0 && Map.RevealedMap[Sx, Sy - 1] == 'n') { Show(Sx, Sy - 1); }
                }
                else
                if (Number >= 9)
                {
                    drawBrush.Color = System.Drawing.Color.Black;
                    formGraphics.FillRectangle(drawBrush, new Rectangle(xx, yy, perSqrX, perSqrY));
                }
                else
                {

                    switch (Number)
                    {
                        case 1:
                            drawBrush.Color = System.Drawing.Color.Blue;
                            break;
                        case 2:
                            drawBrush.Color = System.Drawing.Color.Green;
                            break;
                        case 3:
                            drawBrush.Color = System.Drawing.Color.Red;
                            break;
                        case 4:
                            drawBrush.Color = System.Drawing.Color.DarkBlue;
                            break;
                        case 5:
                            drawBrush.Color = System.Drawing.Color.DarkRed;
                            break;
                        case 6:
                            drawBrush.Color = System.Drawing.Color.Cyan;
                            break;
                        case 7:
                            drawBrush.Color = System.Drawing.Color.Black;
                            break;
                        case 8:
                            drawBrush.Color = System.Drawing.Color.Gray;
                            break;

                        default:
                            drawBrush.Color = System.Drawing.Color.Yellow;
                            break;
                    }
                    System.Drawing.StringFormat drawFormat = new System.Drawing.StringFormat();
                    formGraphics.DrawString(drawString, drawFont, drawBrush, xx + perSqrX / 3, yy, drawFormat);
                }
            }
        }

        //change of placing flags/trying rectangles
        private void button5_Click(object sender, EventArgs e)
        {
            if (flagMode)
            {
                flagMode = false;
                button5.Text = "I want to place flags";
            }
            else
            {
                flagMode = true;
                button5.Text = "I want to fire";
            }
        }

        //settings button, saving changes by user
        private void button3_Click(object sender, EventArgs e)
        {
            if (button3.Text == "Settings")
            {
                button1.Visible = false;
                button2.Visible = false;
                button3.Text = "ok";
                label3.Visible = true;
                label4.Visible = true;
                label5.Visible = true;
                textBox1.Visible = true;
                textBox1.Text = string.Concat(X);
                textBox2.Visible = true;
                textBox2.Text = string.Concat(Y);
                textBox3.Visible = true;
                textBox3.Text = string.Concat(BombTotal);
                radioButton1.Visible = true;
                radioButton2.Visible = true;
                radioButton3.Visible = true;
                radioButton4.Visible = true;
                radioButton1.Checked = true;

            }
            else if (button3.Text == "ok")
            {
                    if (textBox1.Text != "") X = int.Parse(textBox1.Text);
                    if (textBox2.Text != "") Y = int.Parse(textBox2.Text);
                    if (textBox3.Text != "") BombTotal = int.Parse(textBox3.Text);
                    if (BombTotal + 9 >= X * Y) BombTotal = X * Y - 10;
                
                bombCurrent = BombTotal;
                button1.Visible = true;
                button2.Visible = true;
                button3.Text = "Settings";
                label3.Visible = false;
                label4.Visible = false;
                label5.Visible = false;
                textBox1.Visible = false;
                textBox2.Visible = false;
                textBox3.Visible = false;
                radioButton1.Visible = false;
                radioButton2.Visible = false;
                radioButton3.Visible = false;
                radioButton4.Visible = false;
            }

        }

        private void button6_Click(object sender, EventArgs e)
        {
            for (int k  = 0; k < int.Parse(textBox4.Text); k++)
            {
                bool possible;
                possible =Solver.Solv();
                if (!possible) {
                    if (bombCurrent != 0)
                    {
                        MessageBox.Show("You can do it on your own!");
                        break;
                     }
                }
                for (int i = 0; i < X; i++)
                {
                    for (int j = 0; j < Y; j++)
                    {
                        if (Map.RevealedMap[i, j] == 's')
                        {
                            Map.RevealedMap[i, j] = 'n';
                            Show(i, j);
                        }
                        else if (Map.RevealedMap[i, j] == 'm')
                        {
                            Map.RevealedMap[i, j] = 'n';
                            FillFlag(i, j);
                        }
                    }
                }
            }
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            textBox1.Text = "8";
            textBox2.Text = "8";
            textBox3.Text = "10";
        }

        private void radioButton3_CheckedChanged(object sender, EventArgs e)
        {
            textBox1.Text = "16";
            textBox2.Text = "16";
            textBox3.Text = "40";
        }

        private void radioButton4_CheckedChanged(object sender, EventArgs e)
        {
            textBox1.Text = "24";
            textBox2.Text = "24";
            textBox3.Text = "99";
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            button1.Width = this.Width / 2;
            button1.Left = this.Width / 4;
            button1.Height = this.Height / 6;
            button1.Top = this.Height / 6;

            button2.Width = this.Width / 2;
            button2.Left = this.Width / 4;
            button2.Height = this.Height / 6;
            button2.Top = this.Height / 2;

            button3.Width = this.Width / 2;
            button3.Left = this.Width / 4;
            button3.Height = this.Height / 6;
            button3.Top = this.Height / 3;
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            radioButton1.Checked = true;
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            radioButton1.Checked = true;
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            radioButton1.Checked = true;
        }
    }
}

