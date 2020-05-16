﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using BOMBERMAN.GameObj;

namespace BOMBERMAN.GameWorlds
{
    class World
    {
        private Tile[,] mapMatrice;
        private Player player1,player2;

        internal Tile[,] MapMatrice { get => mapMatrice; set => mapMatrice = value; }
        internal Player Player1 { get => player1; set => player1 = value; }
        internal Player Player2 { get => player2; set => player2 = value; }

        private enum Difficulty
        {
            eazy = 1,
            medium = 2,
            hard = 3,
            veryHard = 4,
        }

        public World()
        {
            mapMatrice = new Tile[9, 9];
            player1 = new Player("teste", new int[] { 0, 0 }, 30,40,3,0);
            player1.ChPlayer = Player.Character.BLACK;
            player1.LoadSprites(Properties.Resources.WB_DOWN);
            player2 = new Player("teste", new int[]{ 8, 8 }, 30,40,3,0);
            player2.ChPlayer = Player.Character.PINK;
            player2.LoadSprites(Properties.Resources.WB_UP);
        }


        public void CreateTiles(Image tileDest, Image tileNotDest,int tilesSize)
        {
            Random nbalea = new Random();
            int t = 10;
            int dTile = 4;
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    if (j % 2 != 0 && i % 2 != 0)
                    {
                        mapMatrice[i, j] = new Tile(new Point(i * tilesSize+25, j * tilesSize+25), tilesSize);
                        mapMatrice[i, j].CasePosition =new int[]{i,j};
                        mapMatrice[i, j].LoadSprites(tileNotDest);
                        mapMatrice[i, j].IsDestoyable = false;
                        mapMatrice[i, j].IsSolid = true;
                        mapMatrice[i, j].Occupied = true;

                    }
                    else
                    {
                        if (dTile > 0)
                        {
                            t = nbalea.Next(1, 5);
                            if (t % 2 == 0)
                            {
                                mapMatrice[i, j] = new Tile(new Point(i * tilesSize+25, j * tilesSize+25), tilesSize);
                                mapMatrice[i, j].UnloadSprite();
                                mapMatrice[i, j].IsDestoyable = false;
                                mapMatrice[i, j].IsSolid = false;
                                mapMatrice[i, j].Occupied = false;
                                mapMatrice[i, j].CasePosition = new int[] { i, j };
                            }
                            else
                            {
                                mapMatrice[i, j] = new Tile(new Point(i * tilesSize+25, j * tilesSize+25), tilesSize);
                                mapMatrice[i, j].LoadSprites(tileDest);
                                mapMatrice[i, j].IsDestoyable = true;
                                mapMatrice[i, j].IsSolid = true;
                                mapMatrice[i, j].Occupied = true;
                                mapMatrice[i, j].CasePosition = new int[] { i, j };
                                dTile--;
                            }

                        }
                        else
                        {
                            mapMatrice[i, j] = new Tile(new Point(i * tilesSize+25, j * tilesSize+25), tilesSize);
                            mapMatrice[i, j].UnloadSprite();
                            mapMatrice[i, j].IsDestoyable = false;
                            mapMatrice[i, j].IsSolid = false;
                            mapMatrice[i, j].Occupied = false;
                            mapMatrice[i, j].CasePosition = new int[] { i, j };
                        }
                    }
                  

                }
            }

        }

        public void DrawWorldsTiles(Graphics gr)
        {
            for (int i = 0; i < 9; i++)
                for (int j = 0; j < 9; j++)
                {
                    mapMatrice[i, j].DrawObject(gr);

                }
        }

        public void RefreshMap(Graphics gr)
        {
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    if(mapMatrice[i,j].Fire)
                    {
                        mapMatrice[i, j].LoadSprites(Properties.Resources.FIRE);
                    }
                    else if(!mapMatrice[i,j].IsSolid)
                    {
                        mapMatrice[i, j].UnloadSprite();
                    }

                }
            }
            DrawWorldsTiles(gr);
            player1.DrawObject(gr);
            player2.DrawObject(gr);
        }

        public void LoadPlayerOnMap(Graphics gr)
        {
            int line = player1.CasePosition[0];
            int col = player1.CasePosition[1];

            for (int i = 0; i < 4; i++)
            {
                mapMatrice[line, i].UnloadSprite();
                mapMatrice[line, i].IsSolid = false;
                mapMatrice[line, i].Occupied = false;
                mapMatrice[line, i].IsDestoyable = false;
                mapMatrice[i, col].UnloadSprite();
                mapMatrice[i, col].IsSolid = false;
                mapMatrice[i, col].Occupied = false;
                mapMatrice[i, col].IsDestoyable = false;
            }

            for (int i = 4; i > 0 ; i--)
            {
                mapMatrice[8, i].UnloadSprite();
                mapMatrice[8, i].IsSolid = false;
                mapMatrice[8, i].Occupied = false;
                mapMatrice[8, i].IsDestoyable = false;
                mapMatrice[i,8].UnloadSprite();
                mapMatrice[i, 8].IsSolid = false;
                mapMatrice[i, 8].Occupied = false;
                mapMatrice[i, 8].IsDestoyable = false;

            }
            player1.DrawObject(gr);
            player2.DrawObject(gr);
        }

    
    }
}