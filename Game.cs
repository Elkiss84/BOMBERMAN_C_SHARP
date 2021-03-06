﻿using BOMBERMAN.GameObj;
using BOMBERMAN.GameWorlds;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BOMBERMAN
{
    class Game
    {
        private World map;
        private Rectangle gameArea;
        internal World Map { get => map; set => map = value; }
        internal List<Bombe> BombInGame { get => bombInGame; set => bombInGame = value; }
        private List<Bombe> bombInGame;
        public bool PauseGame { get; set; }

        public Game(Rectangle area, Image[] p1Sprites, Image[] p2Sprites, MainWindow.Gameparam gameparam)
        {
            map = new World(p1Sprites, p2Sprites, gameparam);
            map.CreateTiles(Properties.Resources.brickDest, Properties.Resources.briqueSolid, 60);
            gameArea = area;
            bombInGame = new List<Bombe>();
            PauseGame = false;

        }

        public Game(Rectangle area, Image[] p1Sprites, Image[] p2Sprites, GameState access)
        {
            gameArea = area;   
            
            bombInGame = access.BombeIngame;

            if (bombInGame == null)
                bombInGame = new List<Bombe>();

            map = new World(p1Sprites, p2Sprites, access);
            map.LoadTiles();

            if(bombInGame != null)
                foreach (Bombe item in bombInGame)
                {
                    item.LoadSprites(Properties.Resources.Bombe);
                }
        
            PauseGame = false;
        }

        public void KeyDownActionPlayer1(Keys touch)
        {

            switch (touch)
            {
                case Keys.Z:
                    map.Player1.mvnttDirection = Player.Direction.UP;
                    map.Player1.Sprite = map.Player1.PlayerSprites[Player.Direction.UP];
                    break;
                case Keys.W:
                    map.Player1.mvnttDirection = Player.Direction.DOWN;
                    map.Player1.Sprite = map.Player1.PlayerSprites[Player.Direction.DOWN];
                    break;
                case Keys.S:
                    map.Player1.mvnttDirection = Player.Direction.RIGHT;
                    map.Player1.Sprite = map.Player1.PlayerSprites[Player.Direction.RIGHT];
                    break;
                case Keys.Q:
                    map.Player1.mvnttDirection = Player.Direction.LEFT;
                    map.Player1.Sprite = map.Player1.PlayerSprites[Player.Direction.LEFT];
                    break;
                case Keys.Space:
                    map.Player1.DropBomb(bombInGame, map.MapMatrice);
                    break;
                case Keys.Escape:
                    PauseGame = true;
                    break;
                default:
                    break;
            }
        }

        public void KeyDownActionPlayer2(Keys touch)
        {

            switch (touch)
            {
                case Keys.Up:
                    map.Player2.mvnttDirection = Player.Direction.UP;
                    map.Player2.Sprite = map.Player2.PlayerSprites[Player.Direction.UP];
                    break;
                case Keys.Down:
                    map.Player2.mvnttDirection = Player.Direction.DOWN;
                    map.Player2.Sprite = map.Player2.PlayerSprites[Player.Direction.DOWN];
                    break;
                case Keys.Right:
                    map.Player2.mvnttDirection = Player.Direction.RIGHT;
                    map.Player2.Sprite = map.Player2.PlayerSprites[Player.Direction.RIGHT];
                    break;
                case Keys.Left:
                    map.Player2.mvnttDirection = Player.Direction.LEFT;
                    map.Player2.Sprite = map.Player2.PlayerSprites[Player.Direction.LEFT];
                    break;
                case Keys.ControlKey:
                    map.Player2.DropBomb(bombInGame, map.MapMatrice);
                    break;
                default:
                    break;
            }
        }

        public void KeyUpActionPlayer1(Keys touch)
        {
            switch (touch)
            {
                case Keys.S:
                    map.Player1.IndexFrame = 0;
                    map.Player1.mvnttDirection = Player.Direction.NONE;
                    break;
                case Keys.Q:
                    map.Player1.IndexFrame = 0;
                    map.Player1.mvnttDirection = Player.Direction.NONE;
                    break;
                case Keys.Z:
                    map.Player1.IndexFrame = 0;
                    map.Player1.mvnttDirection = Player.Direction.NONE;
                    break;
                case Keys.W:
                    map.Player1.IndexFrame = 0;
                    map.Player1.mvnttDirection = Player.Direction.NONE;
                    break;
                default:
                    break;
            }

        }

        public void KeyUpActionPlayer2(Keys touch)
        {
            switch (touch)
            {
                case Keys.Right:
                    map.Player2.IndexFrame = 0;
                    map.Player2.mvnttDirection = Player.Direction.NONE;
                    break;
                case Keys.Left:
                    map.Player2.IndexFrame = 0;
                    map.Player2.mvnttDirection = Player.Direction.NONE;
                    break;
                case Keys.Up:
                    map.Player2.IndexFrame = 0;
                    map.Player2.mvnttDirection = Player.Direction.NONE;
                    break;
                case Keys.Down:
                    map.Player2.IndexFrame = 0;
                    map.Player2.mvnttDirection = Player.Direction.NONE;
                    break;
                default:
                    break;
            }

        }


        //gestion des déplacement des joueurs et de leurs collision avec les box
        public bool checkCollisionPlayer(Player player1, Player player2, Tile[,] tileMap, Player.Direction playerDirection)
        {
            int line = player1.CasePosition[0];
            int col = player1.CasePosition[1];

            switch (playerDirection)
            {
                case Player.Direction.RIGHT:

                    Rectangle rect = new Rectangle(
                        player1.Source.X,
                        player1.Source.Y,
                        player1.Source.Width,
                        player1.Source.Height - 5);


                    if ((player1.Source.X + player1.Source.Width) >= gameArea.Width - 25)
                        return true;

                    if (col >= 8)
                        return false;
                    else
                    {
                        if (line < 8)
                            if (CollisionBetweenRectagle(rect, tileMap[col + 1, line + 1].Source) && CollisionBetweenRectagle(rect, tileMap[col + 1, line].Source))
                                if (!tileMap[col + 1, line + 1].IsFree || !tileMap[col + 1, line].IsFree)
                                    return true;

                        if (line > 0)
                            if (CollisionBetweenRectagle(rect, tileMap[col + 1, line - 1].Source) && CollisionBetweenRectagle(rect, tileMap[col + 1, line].Source))
                                if (!tileMap[col + 1, line - 1].IsFree || !tileMap[col + 1, line].IsFree)
                                    return true;

                        if (!tileMap[col + 1, line].IsFree && CollisionBetweenRectagle(rect, tileMap[col + 1, line].Source))
                            return true;
                    }




                    return false;

                case Player.Direction.LEFT:

                    rect = new Rectangle(
                        player1.Source.X,
                        player1.Source.Y,
                        player1.Source.Width,
                        player1.Source.Height - 5);

                    if (player1.Source.X <= 25)
                        return true;

                    if (col <= 0)
                        return false;
                    else
                    {
                        if (line > 0)
                            if (CollisionBetweenRectagle(rect, tileMap[col - 1, line - 1].Source) && CollisionBetweenRectagle(rect, tileMap[col - 1, line].Source))
                                if (!tileMap[col - 1, line - 1].IsFree || !tileMap[col - 1, line].IsFree)
                                    return true;

                        if (line < 8)
                            if (CollisionBetweenRectagle(rect, tileMap[col - 1, line + 1].Source) && CollisionBetweenRectagle(rect, tileMap[col - 1, line].Source))
                                if (!tileMap[col - 1, line + 1].IsFree || !tileMap[col - 1, line].IsFree)
                                    return true;

                        if (CollisionBetweenRectagle(rect, tileMap[col-1, line].Source) && !tileMap[col-1, line].IsFree)
                            return true;
                    }


                    return false;

                case Player.Direction.DOWN:

                    rect = new Rectangle(
                       player1.Source.X,
                       player1.Source.Y + 2,
                       player1.Source.Width,
                       player1.Source.Height);

                    if (player1.Source.Y + player1.Source.Height >= gameArea.Height - 25)
                        return true;


                    if (col > 0 && line < 8)
                        if (CollisionBetweenRectagle(rect, tileMap[col - 1, line + 1].Source) && CollisionBetweenRectagle(rect, tileMap[col, line + 1].Source))
                            if (!tileMap[col - 1, line + 1].IsFree || !tileMap[col, line + 1].IsFree)
                                return true;

                    if (col < 8 && line < 8)
                        if (CollisionBetweenRectagle(rect, tileMap[col + 1, line + 1].Source) && CollisionBetweenRectagle(rect, tileMap[col, line + 1].Source))
                            if (!tileMap[col + 1, line + 1].IsFree || !tileMap[col, line + 1].IsFree)
                                return true;

                    if (line < 8)
                        if (!tileMap[col, line + 1].IsFree && CollisionBetweenRectagle(rect, tileMap[col, line + 1].Source))
                            return true;


                    return false;

                case Player.Direction.UP:

                    rect = new Rectangle(
                       player1.Source.X,
                       player1.Source.Y - 2,
                       player1.Source.Width,
                       player1.Source.Height);

                    if (player1.Source.Y <= 25)
                        return true;



                    if (col > 0 && line > 0)
                        if (CollisionBetweenRectagle(rect, tileMap[col - 1, line - 1].Source) && CollisionBetweenRectagle(rect, tileMap[col, line - 1].Source))
                            if (!tileMap[col - 1, line - 1].IsFree || !tileMap[col, line - 1].IsFree)
                                return true;

                    if (col < 8 && line > 0)
                        if (CollisionBetweenRectagle(rect, tileMap[col + 1, line - 1].Source) && CollisionBetweenRectagle(rect, tileMap[col, line - 1].Source))
                            if (!tileMap[col + 1, line - 1].IsFree || !tileMap[col, line - 1].IsFree)
                                return true;


                    if (line > 0)
                        if (!tileMap[col, line - 1].IsFree)
                            if (CollisionBetweenRectagle(rect, tileMap[col, line - 1].Source))
                                return true;

                    return false;

                case Player.Direction.NONE:
                    return false;
                default:
                    return false;

            }
        }

        public bool CollisionBetweenRectagle(Rectangle rect1, Rectangle rect2)
        {
            if (rect1.X + rect1.Width < rect2.X || rect2.X + rect2.Width < rect1.X
                 || rect1.Y + rect1.Height < rect2.Y || rect2.Y + rect2.Height < rect1.Y)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public void PlayersLogic()
        {
            map.Player1.CheckLocation(60);

            if (map.Player1.mvnttDirection != Player.Direction.NONE)
            {
                if (!checkCollisionPlayer(map.Player1, null, map.MapMatrice, map.Player1.mvnttDirection))
                {
                    map.Player1.MoveToDirection();
                }
                map.Player1.UpdateFrame(20);

                //map.Player2.IndexFrame = 1;
            }

            if (map.Player2 != null)
            {
                map.Player2.CheckLocation(60);

                if (map.Player2.mvnttDirection != Player.Direction.NONE)
                {
                    if (!checkCollisionPlayer(map.Player2, null, map.MapMatrice, map.Player2.mvnttDirection))
                    {
                        map.Player2.MoveToDirection();
                    }
                    map.Player2.UpdateFrame(20);

                    //map.Player2.IndexFrame = 1;
                }
            }
        }

        public void BombeLogic()
        {
            List<Bombe> toRemove = new List<Bombe>();
            foreach (Bombe bombe in bombInGame)
            {
                bombe.UpdateFrame(20);
                bombe.ExplosionTiming(20);

                if (bombe.IsExplosed)
                {
                    bombe.Propagation(map.Player1, map.Player2, map.MapMatrice);
                    toRemove.Add(bombe);

                }

            }

            for (int i = 0; i < toRemove.Count; i++)
            {
                bombInGame.Remove(toRemove[i]);
            }

        }

        public void Interraction()
        {
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    if (map.MapMatrice[i, j].Fire)
                    {
                        if (CollisionBetweenRectagle(map.Player1.Source, map.MapMatrice[i, j].Source))
                        {

                            if (map.Player1.Life <= 0)
                            {
                                map.Player1.IsAlive = false;
                                map.MapMatrice[i, j].FireTime = 0;
                                map.Player1.LoadSprites(Properties.Resources.WB_DEAD);
                                map.Player1.IndexFrame = 2;
                            }
                            else
                            {
                              
                                map.MapMatrice[i, j].FireTime = 0;
                                map.MapMatrice[i, j].IndexFrame = 2;
                                map.MapMatrice[i, j].UnloadSprite();
                                map.Player1.Life--;
                            }
                        }


                        if (map.Player2 != null)
                        {
                            if (CollisionBetweenRectagle(map.Player2.Source, map.MapMatrice[i, j].Source))
                            {
                                if (map.Player2.Life <= 0)
                                {
                                    map.Player2.IsAlive = false;
                                    map.Player2.LoadSprites(Properties.Resources.WB_DEAD);
                                    map.MapMatrice[i, j].FireTime = 0;
                                    map.Player2.FrameSpeed = 10;
                                    map.Player1.IndexFrame = 2;

                                }
                                else
                                {
                                    map.Player2.Life--;
                                    map.MapMatrice[i, j].FireTime = 0;
                                    map.MapMatrice[i, j].IndexFrame = 2;
                                    map.MapMatrice[i, j].UnloadSprite();
                                }
                            }
                        }

                        if (map.MapMatrice[i, j].FireTime <= 0)
                        {
                            map.MapMatrice[i, j].FireTime = 500;// à revoir avec temps timer
                            map.MapMatrice[i, j].Fire = false;
                            map.MapMatrice[i, j].IndexFrame = 0;
                        }
                        else
                        {
                            map.MapMatrice[i, j].FireTime -= 100;
                        }
                        map.MapMatrice[i, j].UpdateFrame(20);
                    }
                }
            }
        }

        public void BonusLogic()
        {
            map.Player1.CheckLocation(60);
            int colp1 = map.Player1.CasePosition[1], linep1 = map.Player1.CasePosition[0];


            if (map.MapMatrice[colp1, linep1].bonus != null)
            {
                switch (map.MapMatrice[colp1, linep1].bonus.BonusTtype)
                {
                    case Bonus.Bonustype.BOMBE:
                        if (map.Player1.NbBombe < 10)
                        {
                            map.Player1.NbBombe++;
                            map.MapMatrice[colp1, linep1].bonus = null;
                        }
                        break;
                    case Bonus.Bonustype.SPEED:
                        if (map.Player1.Vitesse < 10)
                        {
                            map.Player1.Vitesse++;
                            map.MapMatrice[colp1, linep1].bonus = null;
                        }
                        break;
                    case Bonus.Bonustype.D_SPEED:
                        if (map.Player1.Vitesse > 1)
                        {
                            map.Player1.Vitesse--;
                            map.MapMatrice[colp1, linep1].bonus = null;
                        }
                        break;
                    case Bonus.Bonustype.DISAMORCE:
                        if (map.Player1.Bonusplayer != Bonus.Bonustype.NONE)
                        {
                            map.Player1.Bonusplayer = Bonus.Bonustype.DISAMORCE;
                            map.MapMatrice[colp1, linep1].bonus = null;
                        }
                        break;
                    case Bonus.Bonustype.LIFE:
                        if (map.Player1.Life < 5)
                        {
                            map.Player1.Life++;
                            map.MapMatrice[colp1, linep1].bonus = null;
                        }
                        break;
                    case Bonus.Bonustype.EFFECT:
                        if (map.Player1.BombeEffect < 8)
                        {
                            map.Player1.BombeEffect++;
                            map.MapMatrice[colp1, linep1].bonus = null;
                        }
                        break;
                    case Bonus.Bonustype.D_EFFET:
                        if (map.Player1.BombeEffect >= 2)
                        {
                            map.Player1.BombeEffect--;
                            map.MapMatrice[colp1, linep1].bonus = null;
                        }
                        break;
                    case Bonus.Bonustype.KICK:
                        if (map.Player1.Bonusplayer != Bonus.Bonustype.NONE)
                        {
                            map.Player1.Bonusplayer = Bonus.Bonustype.KICK;
                            map.MapMatrice[colp1, linep1].bonus = null;
                        }
                        break;
                    case Bonus.Bonustype.LAUNCH:
                        if (map.Player1.Bonusplayer != Bonus.Bonustype.NONE)
                        {
                            map.Player1.Bonusplayer = Bonus.Bonustype.LAUNCH;
                            map.MapMatrice[colp1, linep1].bonus = null;
                        }
                        break;
                    default:
                        break;
                }

            }


            if (map.Player2 != null)
            {
                map.Player2.CheckLocation(60);

                int colp2 = map.Player2.CasePosition[1], linep2 = map.Player2.CasePosition[0];

                if (map.MapMatrice[colp2, linep2].bonus != null)
                {
                    switch (map.MapMatrice[colp2, linep2].bonus.BonusTtype)
                    {
                        case Bonus.Bonustype.BOMBE:
                            if (map.Player2.NbBombe < 10)
                            {
                                map.Player2.NbBombe++;
                                map.MapMatrice[colp1, linep1].bonus = null;
                            }
                            break;
                        case Bonus.Bonustype.SPEED:
                            if (map.Player2.Vitesse < 30)
                            {
                                map.Player2.Vitesse++;
                                map.MapMatrice[colp2, linep2].bonus = null;
                            }
                            break;
                        case Bonus.Bonustype.D_SPEED:
                            if (map.Player2.Vitesse > 1)
                            {
                                map.Player2.Vitesse--;
                                map.MapMatrice[colp2, linep2].bonus = null;
                            }
                            break;
                        case Bonus.Bonustype.DISAMORCE:
                            if (map.Player2.Bonusplayer != Bonus.Bonustype.NONE)
                            {
                                map.Player2.Bonusplayer = Bonus.Bonustype.DISAMORCE;
                                map.MapMatrice[colp2, linep2].bonus = null;
                            }
                            break;
                        case Bonus.Bonustype.LIFE:
                            if (map.Player2.Life < 5)
                            {
                                map.Player2.Life++;
                                map.MapMatrice[colp2, linep2].bonus = null;
                            }

                            break;
                        case Bonus.Bonustype.EFFECT:
                            if (map.Player2.BombeEffect < 9)
                            {
                                map.Player2.BombeEffect++;
                                map.MapMatrice[colp2, linep2].bonus = null;
                            }
                            break;
                        case Bonus.Bonustype.D_EFFET:
                            if (map.Player2.BombeEffect >= 2)
                            {
                                map.Player2.BombeEffect--;
                                map.MapMatrice[colp2, linep2].bonus = null;
                            }
                            break;
                        case Bonus.Bonustype.KICK:
                            if (map.Player2.Bonusplayer != Bonus.Bonustype.NONE)
                            {
                                map.Player2.Bonusplayer = Bonus.Bonustype.KICK;
                                map.MapMatrice[colp2, linep2].bonus = null;
                            }
                            break;
                        case Bonus.Bonustype.LAUNCH:
                            if (map.Player2.Bonusplayer != Bonus.Bonustype.NONE)
                            {
                                map.Player2.Bonusplayer = Bonus.Bonustype.LAUNCH;
                                map.MapMatrice[colp2, linep2].bonus = null;
                            }
                            break;
                        default:
                            break;
                    }
                }
            }


        }

        public bool GameOver()
        {
            if (map.Player1.Life <= 0 || map.Player2.Life <= 0)
                return true;
            else
                return false;
        }

        public void LoadGame(Graphics gr)
        {
            map.DrawWorldsTiles(gr);
            map.LoadPlayerOnMap(gr);
            map.RefreshMap(gr);
        }

        public void GameLogic(Graphics gr)
        {
            if (!PauseGame)
            {
                Interraction();
                BombeLogic();
                BonusLogic();
                PlayersLogic();

                if (BombInGame.Count >= 0)
                {
                    foreach (Bombe item in BombInGame)
                    {
                        item.DrawObject(gr);
                    }
                }
            }

            map.RefreshMap(gr);
        }

        public void SaveGame()
        {
            string path;

            using (FolderBrowserDialog folder = new FolderBrowserDialog())
            {
                if (folder.ShowDialog() == DialogResult.OK)
                {
                    path = folder.SelectedPath + "\\map.dat";

                    System.Runtime.Serialization.IFormatter formatter = new BinaryFormatter();
                    System.IO.FileStream filestream = new System.IO.FileStream(path, System.IO.FileMode.Create);
                    try
                    {
                        formatter.Serialize(filestream, new GameState(map.Player1,map.Player2, map.MapMatrice));
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("An error has occured : " + ex.Message);
                        return;
                    }
                    MessageBox.Show("File " + path + " saved successfuly !");
                    filestream.Close();
                }


            }

        }

        public void ContinueGame()
        {

            GameState sauvegarde;
            OpenFileDialog file = new OpenFileDialog();

            if (file.ShowDialog() == DialogResult.OK)
            {
                string path = file.FileName;
             

                System.Runtime.Serialization.IFormatter formatter = new BinaryFormatter();
                System.IO.FileStream filestream = new System.IO.FileStream(path, System.IO.FileMode.Open);
                try
                {
                    sauvegarde = (GameState)formatter.Deserialize(filestream);

                }
                catch (Exception ex)
                {
                    MessageBox.Show("An error has occured : " + ex.Message);
                    return;
                }

                MessageBox.Show("Partie saucegarder");
            }

        }

   
    }
}
