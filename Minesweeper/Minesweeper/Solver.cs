using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minesweeper
{
    public static class Solver
    {
        static bool br = false;

        public static bool Solv()
        {
            //dict.Clear();
            br = false;
            Basic();
            return br;
        }
        //check if situation is obvious around one rectangle
        public static void Basic()
        {
            for (int i = 0; i < Form1.X; i++)
            {
                for (int j = 0; j < Form1.Y; j++)
                {
                    if (!br)
                    {
                        if ((Map.RevealedMap[i, j] - '0' > 0) && (Map.RevealedMap[i, j] - '0' <= 9))
                        {
                            int flags = 0;
                            int spots = 0;
                            for (int k = -1; k < 2; k++)
                            {
                                for (int l = -1; l < 2; l++)
                                {
                                    if ((i + k >= 0) && (i + k < Form1.X) && (j + l >= 0) && (j + l < Form1.Y))
                                    {
                                        if (Map.RevealedMap[i + k, j + l] == 'f')
                                        {
                                            flags += 1;
                                        }
                                        if (Map.RevealedMap[i + k, j + l] == 'n')
                                        {
                                            spots += 1;
                                        }
                                    }
                                }
                            }
                            int rest = Map.RevealedMap[i, j] - '0' - flags;
                            if ((rest == 0) && (spots != 0))
                            {
                                MarkSafe(i, j);
                                br = true;
                            }
                            else if ((rest == spots) && (spots != 0))
                            {
                                MarkBombs(i, j);
                                br = true;
                            }
                            //else if (spots != 0) { AddLine(i, j,rest); }
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
                    if ((i + xx >= 0) && (i + xx < Form1.X) && (j + yy >= 0) && (j + yy < Form1.Y))
                    {
                        if (Map.RevealedMap[i + xx, j + yy] == 'n')
                        {
                            Map.RevealedMap[i + xx, j + yy] = 's';
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
                    if ((i + xx >= 0) && (i + xx < Form1.X) && (j + yy >= 0) && (j + yy < Form1.Y))
                    {
                        if (Map.RevealedMap[i + xx, j + yy] == 'n')
                        {
                            Map.RevealedMap[i + xx, j + yy] = 'm';
                        }
                    }
                }
            }
        }
        //Code bellow is under construction and it's not called by the rest of the code
        //
        //
        //
        /*
        public static void Matrix()
        {
            SolveMatrix();
        }

        public static void SolveMatrix()
        {

        }

        public static void AddLine(int Ax,int Ay,int result)
        {
            for (int i = -1; i < 2; i++)
            {
                for (int j = -1; j < 2; j++)
                {
                    if ((i + Ax >= 0) && (i + Ax < Form1.x) && (j + Ay >= 0) && (j + Ay < Form1.y))
                    {
                        if (Map.RevealedMap[i + Ax, j + Ay] == 'n')
                        {
                            string s = string.Format("{0},{1}", i + Ax, j + Ay);
                            if (dict.ContainsKey(s))
                            {
                                matrixA[dict[s], lines] = 1;
                            } else
                            {
                                dict.Add(s, variables);
                                variables++;
                            } 
                        }
                    }
                }
            }
            matrixA[0, lines] = result;
            lines += 1;
        }*/
    }
    }
