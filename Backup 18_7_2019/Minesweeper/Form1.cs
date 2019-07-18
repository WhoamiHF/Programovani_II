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
        public static bool gameMode;
        public static bool running;
        public static int x = 10;
        public static int y = 10;
        public static int perSqrX = 1;
        public static int persqrY = 1;
        public static int sec = 0;
        public static int min = 0;
        public static int bombTotal = 1;
        public static int bombCurrent = bombTotal;
        public static char[,] revealedMap = new char[x + 1, y + 1];
        public static int[,] publicMap = new int[x + 1, y + 1];
        public static bool first = true;
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            timer1.Start();
            button1.Visible = false;
            button2.Visible = false;
            running = true;
            button3.Visible = false;
            button4.Visible = true;
            button5.Visible = true;
            label1.Visible = true;
            label2.Visible = true;
            perSqrX = (this.Width - 60) / x;
            persqrY = (this.Height - 80) / y;
            DrawCage();
        }
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

        public void gameOver()
        {
            running = false;
            first = true;
            System.Drawing.Graphics formGraphics = CreateGraphics();
            System.Drawing.SolidBrush drawBrush = new System.Drawing.SolidBrush(System.Drawing.Color.White);
            formGraphics.FillRectangle(drawBrush, 10, 30, this.Width, this.Height);

            button1.Visible = true;
            button2.Visible = true;
            button3.Visible = true;
            button4.Visible = false;
            button5.Visible = false;
            timer1.Stop();
            label1.Visible = false;
            label2.Visible = false;
            sec = 0; min = 0;
            bombCurrent = bombTotal;

        }

        private void button4_Click(object sender, EventArgs e)
        {
            gameOver(); ;
        }
        public void DrawCage()
        {
            System.Drawing.Graphics graphicsObj;
            graphicsObj = this.CreateGraphics();
            Pen myPen = new Pen(System.Drawing.Color.Black, 1);

            int cageWidth = x * perSqrX;
            int cageHeight = y * persqrY;
            graphicsObj.DrawLine(myPen, 10, 30, 10 + cageWidth, 30);
            graphicsObj.DrawLine(myPen, 10, 30, 10, 30 + cageHeight);
            graphicsObj.DrawLine(myPen, 10 + cageWidth, 30, 10 + cageWidth, 30 + cageHeight);
            graphicsObj.DrawLine(myPen, 10, 30 + cageHeight, 10 + cageWidth, 30 + cageHeight);

            for (int i = 1; i < x; i++)
            {
                graphicsObj.DrawLine(myPen, 10 + i * perSqrX, 30, 10 + i * perSqrX, 30 + cageHeight);
            }

            for (int i = 1; i < y; i++)
            {
                graphicsObj.DrawLine(myPen, 10, 30 + persqrY * i, 10 + cageWidth, 30 + persqrY * i);
            }
        }
        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void Form1_MouseClick(object sender, MouseEventArgs e)
        {
            if (running)
            {
                int squareX = (e.X - 10) / (perSqrX);
                int squareY = (e.Y - 30) / (persqrY);
                if (gameMode) { FillFlag(squareX, squareY); }
                else
                {
                    label1.Text = string.Format("{0},{1}", squareX, squareY);
                    if (first) { first = false; Map.GenerateMins(squareX, squareY); }
                    if (revealedMap[squareX, squareY] == 'n')
                    {
                        Show(squareX, squareY);
                    }
                }
            }
        }
        public void FillFlag(int Fx, int Fy)
        {
            int xx = 10 + (Fx) * perSqrX;
            int yy = 30 + (Fy) * persqrY;
            System.Drawing.Graphics formGraphics = CreateGraphics();
            System.Drawing.SolidBrush drawBrush = new System.Drawing.SolidBrush(System.Drawing.Color.Red);
            if (revealedMap[Fx, Fy] == 'f')
            {
                drawBrush.Color = System.Drawing.Color.LightGray;
                revealedMap[Fx, Fy] = 'n';
                bombCurrent += 1;
                formGraphics.FillRectangle(drawBrush, new Rectangle(xx + 1, yy + 1, perSqrX - 1, persqrY - 1));
            }
            else if (revealedMap[Fx, Fy] == 'n')
            {
                bombCurrent -= 1;
                revealedMap[Fx, Fy] = 'f';
                formGraphics.FillRectangle(drawBrush, new Rectangle(xx + 1, yy + 1, perSqrX - 1, persqrY - 1));
                if (bombCurrent == 0)
                {
                    if (Map.CheckWin())
                    {
                        timer1.Stop();
                        MessageBox.Show("You have won!");
                    }
                }
            }
            label2.Text = string.Format("Bombs: {0}", bombCurrent);


        }

        public void FillZero(int Fx, int Fy)
        {
            System.Drawing.Graphics formGraphics = CreateGraphics();
            System.Drawing.SolidBrush drawBrush = new System.Drawing.SolidBrush(System.Drawing.Color.DarkGray);
            formGraphics.FillRectangle(drawBrush, new Rectangle(Fx, Fy, perSqrX - 1, persqrY - 1));
        }
        public void Show(int Sx, int Sy)
        {
            int xx = 10 + (Sx) * perSqrX;
            int yy = 30 + (Sy) * persqrY;
            int Number = Map.Reveal(Sx, Sy);
            if (Number == -1) { MessageBox.Show("You have lost!"); gameOver(); }
            else
            {
                string drawString = string.Concat(Number);

                System.Drawing.Graphics formGraphics = CreateGraphics();
                System.Drawing.Font drawFont = new System.Drawing.Font("Arial", 3 * persqrY / 4);
                System.Drawing.SolidBrush drawBrush = new System.Drawing.SolidBrush(System.Drawing.Color.Black);
                if (Number >= 9)
                {
                    revealedMap[Sx, Sy] = 'b';
                }
                else if (Number == 0)
                {
                    revealedMap[Sx, Sy] = 'v';
                }
                else
                {
                    string n = string.Concat(Number);
                    revealedMap[Sx, Sy] = n[0];
                }
                if (Number == 0)
                {

                    FillZero(xx + 1, yy + 1);
                    if (Sx < x - 1)
                    {
                        if (revealedMap[Sx + 1, Sy] == 'n') { Show(Sx + 1, Sy); }
                        if (Sy < y - 1 && revealedMap[Sx + 1, Sy + 1] == 'n') { Show(Sx + 1, Sy + 1); }
                        if (Sy > 0 && revealedMap[Sx + 1, Sy - 1] == 'n') { Show(Sx + 1, Sy - 1); }
                    }
                    if (Sx > 0)
                    {
                        if (revealedMap[Sx - 1, Sy] == 'n') { Show(Sx - 1, Sy); }
                        if (Sy < y - 1 && revealedMap[Sx - 1, Sy + 1] == 'n') { Show(Sx - 1, Sy + 1); }
                        if (Sy > 0 && revealedMap[Sx - 1, Sy - 1] == 'n') { Show(Sx - 1, Sy - 1); }
                    }
                    if (Sy < y - 1 && revealedMap[Sx, Sy + 1] == 'n') { Show(Sx, Sy + 1); }
                    if (Sy > 0 && revealedMap[Sx, Sy - 1] == 'n') { Show(Sx, Sy - 1); }
                }
                else
                if (Number >= 9)
                {
                    drawBrush.Color = System.Drawing.Color.Black;
                    formGraphics.FillRectangle(drawBrush, new Rectangle(xx, yy, perSqrX, persqrY));
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

        private void button5_Click(object sender, EventArgs e)
        {
            if (gameMode)
            {
                gameMode = false;
                button5.Text = "Fire";
            }
            else
            {
                gameMode = true;
                button5.Text = "Flags";
            }
        }
    }
    public static class Map
    {
        private static int[,] map = new int[Form1.x + 1, Form1.y + 1];
        public static void GenerateMins(int sqX, int sqY)
        {
            for (int i = 0; i < Form1.y; i++)
            {
                for (int j = 0; j < Form1.x; j++)
                {
                    map[j, i] = 0;
                }
            }

            for (int j = -1; j < 2; j++)
            {
                for (int k = -1; k < 2; k++)
                {
                    map[sqX + j, sqY + k] = 100;
                }//OPRAVIT HRANICE
            }

            Random Rndm = new Random();
            for (int i = 0; i < Form1.bombTotal; i++)
            {
                bool Ok = false;
                while (Ok == false)
                {
                    int bombPlacementX = Rndm.Next(Form1.x);
                    int bombPlacementY = Rndm.Next(Form1.y);
                    if (map[bombPlacementX, bombPlacementY] < 9)
                    {
                        Ok = true;
                        map[bombPlacementX, bombPlacementY] = 9;
                        for (int j = -1; j < 2; j++)
                        {
                            for (int k = -1; k < 2; k++)
                            {
                                if (bombPlacementX + j >= 0 && bombPlacementY + k >= 0 && bombPlacementX + j < Form1.x && bombPlacementY + k < Form1.y)
                                {
                                    map[bombPlacementX + j, bombPlacementY + k] += 1;
                                }
                            }
                        }
                    }
                }
            }
            for (int j = -1; j < 2; j++)
            {
                for (int k = -1; k < 2; k++)
                {
                    map[sqX + j, sqY + k] -= 100;
                }
            }
            InicializeRevealedMap();
        }

        public static void InicializeRevealedMap()
        {
            for (int i = 0; i < Form1.x; i++)
            {
                for (int j = 0; j < Form1.y; j++)
                {
                    Form1.revealedMap[i, j] = 'n';
                }
            }
        }
        public static bool CheckWin()
        {
            bool ok = true;
            for (int i = 0; i < Form1.x; i++)
            {
                for (int j = 0; j < Form1.y; j++)
                {
                    if (map[i, j] >= 9)
                    {
                        if (Form1.revealedMap[i, j] != 'f') { ok = false; }
                    }
                }
            }
            return ok;
        }
        public static int Reveal(int Rx, int Ry)
        {
            if (map[Rx, Ry] > 50) { return 0; }
            else
            {
                if (map[Rx, Ry] >= 9)
                {
                    GenerateMins(1, 1);
                    return -1;
                }
                else
                    return map[Rx, Ry];
            }
        }

    }
}
