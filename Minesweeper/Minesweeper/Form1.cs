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
        public static bool flagMode; // testing rectangles or placing flags
        public static bool running;
        public static int x = 30; //number of rectangles in a row
        public static int y = 16; //number of rectangles in a collumn
        public static int perSqrX = 1; //how many pixels are in one rectangle horizontally
        public static int persqrY = 1;//how many pixels are in one rectangle vertically 
        public static int sec = 0; // measuring time
        public static int min = 0;
        public static int bombTotal = 99; //number of bombs at the begining
        public static int bombCurrent = bombTotal; //bombTotal - flags = bombCurrent
        public static bool first = true; // first move

        public Form1()
        {
            InitializeComponent();
        }

        //inicialize, hide buttons, show others, draw cage
        private void button1_Click(object sender, EventArgs e)
        {
            flagMode = false;
            label2.Text = string.Format("Mines left: {0}", bombCurrent);
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
            perSqrX = (this.Width - 60) / x;
            persqrY = (this.Height - 80) / y;
            DrawCage();
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
        public void gameOver()
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
            bombCurrent = bombTotal;

        }

        //menu button
        private void button4_Click(object sender, EventArgs e)
        {
            gameOver(); ;
        }
        // procedure for drawing cage called by pressing button1 (start game)
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
                int squareY = (e.Y - 30) / (persqrY);
                if ((squareX >= 0) && (squareX < x) && (squareY >= 0) && (squareY < y))
                {
                    if (flagMode) { FillFlag(squareX, squareY); }
                    else
                    {
                        label1.Text = string.Format("{0},{1}", squareX, squareY);
                        if (first) { first = false; Map.GenerateMins(squareX, squareY); }
                        if (Map.revealedMap[squareX, squareY] == 'n')
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
            int yy = 30 + (Fy) * persqrY;
            System.Drawing.Graphics formGraphics = CreateGraphics();
            System.Drawing.SolidBrush drawBrush = new System.Drawing.SolidBrush(System.Drawing.Color.Red);
            if (Map.revealedMap[Fx, Fy] == 'f')
            {
                drawBrush.Color = System.Drawing.Color.White;
                Map.revealedMap[Fx, Fy] = 'n';
                bombCurrent += 1;
                label2.Text = string.Format("Bombs: {0}", bombCurrent);
                formGraphics.FillRectangle(drawBrush, new Rectangle(xx + 1, yy + 1, perSqrX - 1, persqrY - 1));
            }
            else if ((Map.revealedMap[Fx, Fy] == 'n') && (bombCurrent > 0))
            {
                bombCurrent -= 1;
                label2.Text = string.Format("Bombs: {0}", bombCurrent);
                Map.revealedMap[Fx, Fy] = 'f';
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
        }

        //make gray rectangle
        public void FillZero(int Fx, int Fy)
        {
            System.Drawing.Graphics formGraphics = CreateGraphics();
            System.Drawing.SolidBrush drawBrush = new System.Drawing.SolidBrush(System.Drawing.Color.DarkGray);
            formGraphics.FillRectangle(drawBrush, new Rectangle(Fx, Fy, perSqrX - 1, persqrY - 1));
        }
        //Show result of player´s move and mark it into revealed map
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
                    Map.revealedMap[Sx, Sy] = 'b';
                }
                else if (Number == 0)
                {
                    Map.revealedMap[Sx, Sy] = 'v';
                }
                else
                {
                    string n = string.Concat(Number);
                    Map.revealedMap[Sx, Sy] = n[0];
                }
                if (Number == 0)
                {

                    FillZero(xx + 1, yy + 1);
                    if (Sx < x - 1)
                    {
                        if (Map.revealedMap[Sx + 1, Sy] == 'n') { Show(Sx + 1, Sy); }
                        if (Sy < y - 1 && Map.revealedMap[Sx + 1, Sy + 1] == 'n') { Show(Sx + 1, Sy + 1); }
                        if (Sy > 0 && Map.revealedMap[Sx + 1, Sy - 1] == 'n') { Show(Sx + 1, Sy - 1); }
                    }
                    if (Sx > 0)
                    {
                        if (Map.revealedMap[Sx - 1, Sy] == 'n') { Show(Sx - 1, Sy); }
                        if (Sy < y - 1 && Map.revealedMap[Sx - 1, Sy + 1] == 'n') { Show(Sx - 1, Sy + 1); }
                        if (Sy > 0 && Map.revealedMap[Sx - 1, Sy - 1] == 'n') { Show(Sx - 1, Sy - 1); }
                    }
                    if (Sy < y - 1 && Map.revealedMap[Sx, Sy + 1] == 'n') { Show(Sx, Sy + 1); }
                    if (Sy > 0 && Map.revealedMap[Sx, Sy - 1] == 'n') { Show(Sx, Sy - 1); }
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

        //change of placing flags/trying rectangles
        private void button5_Click(object sender, EventArgs e)
        {
            if (flagMode)
            {
                flagMode = false;
                button5.Text = "Fire";
            }
            else
            {
                flagMode = true;
                button5.Text = "Flags";
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
                textBox1.Text = string.Concat(x);
                textBox2.Visible = true;
                textBox2.Text = string.Concat(y);
                textBox3.Visible = true;
                textBox3.Text = string.Concat(bombTotal);
            }
            else if (button3.Text == "ok")
            {
                if (textBox1.Text != "") x = int.Parse(textBox1.Text);
                if (textBox2.Text != "") y = int.Parse(textBox2.Text);
                if (textBox3.Text != "") bombTotal = int.Parse(textBox3.Text);
                if (bombTotal + 9 >= x * y) bombTotal = x * y - 10;
                bombCurrent = bombTotal;
                button1.Visible = true;
                button2.Visible = true;
                button3.Text = "Settings";
                label3.Visible = false;
                label4.Visible = false;
                label5.Visible = false;
                textBox1.Visible = false;
                textBox2.Visible = false;
                textBox3.Visible = false;
            }

        }

        private void button6_Click(object sender, EventArgs e)
        {
            for (int k  = 0; k < int.Parse(textBox4.Text); k++)
            {


                Solver.Solv();
                for (int i = 0; i < x; i++)
                {
                    for (int j = 0; j < y; j++)
                    {
                        if (Map.revealedMap[i, j] == 's')
                        {
                            Map.revealedMap[i, j] = 'n';
                            Show(i, j);
                        }
                        else if (Map.revealedMap[i, j] == 'm')
                        {
                            Map.revealedMap[i, j] = 'n';
                            FillFlag(i, j);
                        }
                    }
                }
            }
        }
    }
    //class with map - map is hiden from form1 class
    public static class Map
    {
        private static int[,] map = new int[Form1.x + 1, Form1.y + 1];
        public static char[,] revealedMap = new char[Form1.x + 1, Form1.y + 1];
        public static int[,] publicMap = new int[Form1.x + 1, Form1.y + 1];

        //place correct amount of mins into sheet
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
                    if ((sqX + j >= 0) && (sqX + j < Form1.x) && (sqY + k >= 0) && (sqY + k < Form1.y))
                    {
                        map[sqX + j, sqY + k] = 100;
                    }
                }
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
                    revealedMap[i, j] = 'n';
                }
            }
        }

        //check if on every bomb is flag (it's not possible to place more flags
        //than amount of bombs so it's correct deciding

        public static bool CheckWin()
        {
            bool ok = true;
            for (int i = 0; i < Form1.x; i++)
            {
                for (int j = 0; j < Form1.y; j++)
                {
                    if (map[i, j] >= 9)
                    {
                        if (revealedMap[i, j] != 'f') { ok = false; }
                    }
                }
            }
            return ok;
        }
        //Answer about player's move. If he blow the mine, make new sheet so it's impossible
        // to continue (player will be returned into menu)
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

    public static class Solver
    {
        public static bool br = false;
        public static void Solv()
        {
            br = false;
            Basic();
        }
        //check if situation is obvious around one rectangle
        public static void Basic()
        {
            for (int i = 0; i < Form1.x; i++)
            {
                for (int j = 0; j < Form1.y; j++)
                {
                    if (!br) 
                    {
                        if ((Map.revealedMap[i, j] - '0' > 0) && (Map.revealedMap[i, j] - '0' <= 9))
                        {
                            int flags = 0;
                            int spots = 0;
                            for (int k = -1; k < 2; k++)
                            {
                                for (int l = -1; l < 2; l++)
                                {
                                    if ((i + k >= 0) && (i + k < Form1.x) && (j + l >= 0) && (j + l < Form1.y))
                                    {
                                        if (Map.revealedMap[i + k, j + l] == 'f')
                                        {
                                            flags += 1;
                                        }
                                        if (Map.revealedMap[i + k, j + l] == 'n')
                                        {
                                            spots += 1;
                                        }
                                    }
                                }
                            }
                            if ((Map.revealedMap[i, j] - '0' - flags == 0) && (spots != 0))
                            {
                                Map.revealedMap[i, j] = '.';
                                MarkSafe(i, j);
                                br = true;
                            }
                            else if ((Map.revealedMap[i, j] - '0' - flags == spots)&& (spots != 0))
                            {
                                Map.revealedMap[i, j] = '.';
                                MarkBombs(i, j);
                                br = true;
                            }
                        }
                    }
                }
            }
        }

        public static void MarkSafe(int xx, int yy)
        {
            for (int i = -1; i < 2; i++)
            {
                for (int j = -1; j < 2; j++)
                {
                    if ((i + xx >= 0) && (i + xx < Form1.x) && (j + yy >= 0) && (j + yy < Form1.y))
                    {
                        if (Map.revealedMap[i + xx, j + yy] == 'n')
                        {
                            Map.revealedMap[i + xx, j + yy] = 's';
                        }
                    }
                }
            }
        }

        public static void MarkBombs(int xx, int yy)
        {
            for (int i = -1; i < 2; i++)
            {
                for (int j = -1; j < 2; j++)
                {
                    if ((i + xx >= 0) && (i + xx < Form1.x) && (j + yy >= 0) && (j + yy < Form1.y))
                    {
                        if (Map.revealedMap[i + xx, j + yy] == 'n')
                        {
                            Map.revealedMap[i + xx, j + yy] = 'm';
                        }
                    }
                }
            }
        }
    }
}

