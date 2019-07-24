using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minesweeper
{
    //class with map - map is hiden from form1 class
    public static class Map
    {
        private static int[,] map = new int[Form1.Y + 1, Form1.Y + 1];
        public static char[,] RevealedMap = new char[Form1.X + 1, Form1.Y + 1];

        //place correct amount of mins into sheet
        public static void GenerateMins(int sqX, int sqY)
        {
            map = new int[Form1.X + 1, Form1.Y + 1];
            RevealedMap = new char[Form1.X + 1, Form1.Y + 1];
            for (int i = 0; i < Form1.Y; i++)
            {
                for (int j = 0; j < Form1.X; j++)
                {
                    map[j, i] = 0;
                }
            }

            for (int j = -1; j < 2; j++)
            {
                for (int k = -1; k < 2; k++)
                {
                    if ((sqX + j >= 0) && (sqX + j < Form1.X) && (sqY + k >= 0) && (sqY + k < Form1.Y))
                    {
                        map[sqX + j, sqY + k] = 100;
                    }
                }
            }

            Random Rndm = new Random();
            for (int i = 0; i < Form1.BombTotal; i++)
            {
                bool Ok = false;
                while (Ok == false)
                {
                    int bombPlacementX = Rndm.Next(Form1.X);
                    int bombPlacementY = Rndm.Next(Form1.Y);
                    if (map[bombPlacementX, bombPlacementY] < 9)
                    {
                        Ok = true;
                        map[bombPlacementX, bombPlacementY] = 9;
                        for (int j = -1; j < 2; j++)
                        {
                            for (int k = -1; k < 2; k++)
                            {
                                if (bombPlacementX + j >= 0 && bombPlacementY + k >= 0 && bombPlacementX + j < Form1.X && bombPlacementY + k < Form1.Y)
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
                    if ((sqX + j >= 0) && (sqX + j < Form1.X) && (sqY + k >= 0) && (sqY + k < Form1.Y))
                    {
                        map[sqX + j, sqY + k] -= 100;
                    }
                }
            }
            InicializeRevealedMap();
        }


        public static void InicializeRevealedMap()
        {
            for (int i = 0; i < Form1.X; i++)
            {
                for (int j = 0; j < Form1.Y; j++)
                {
                    RevealedMap[i, j] = 'n';
                }
            }
        }

        //check if on every bomb is flag (it's not possible to place more flags
        //than amount of bombs so it's correct deciding

        public static bool CheckWin()
        {
            bool ok = true;
            for (int i = 0; i < Form1.X; i++)
            {
                for (int j = 0; j < Form1.Y; j++)
                {
                    if (map[i, j] >= 9)
                    {
                        if (RevealedMap[i, j] != 'f') { ok = false; }
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
}
