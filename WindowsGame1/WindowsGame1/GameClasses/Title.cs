using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.IO;
using Microsoft.Xna.Framework.Media;

namespace WindowsGame1
{
    class Title
    {
        //Pictures!
        Sprite TitlePic;
        Sprite Background;
        Sprite NewGame;
        Sprite LoadGame;
        Sprite SaveGame;
        Sprite Resume;
        Sprite Exit;
        Sprite FileSlot;
        Sprite ArrowDown;
        Sprite ArrowUp;

        //Menu-Variables
        public int SelectedIndex;
        public int ConfirmedSelectedIndex;
        public int ScrollIndex;
        SpriteFont myfont;
        Boolean DontSave;
        public Song titlesong;
        public Song lastsong;   // The song that was played during the game before the titlescreen was called up
        public String newsavename;
        public Boolean Startup;
        public String Mode;
        public List<String> Savefilenames;

        //Stuff for the snow-particle thingies!
        Sprite Flake;
        List<Snowflake> Flakes;
        int GenerateMax = 10;
        int maxTDL = 8000;
        Random Die;
        int MakeSnowCounter = 0;

        public Title()
        {
            TitlePic = new Sprite();
            Background = new Sprite();
            NewGame = new Sprite();
            LoadGame = new Sprite();
            SaveGame = new Sprite();
            Exit = new Sprite();
            Resume = new Sprite();
            FileSlot = new Sprite();
            ArrowDown = new Sprite();
            ArrowUp = new Sprite();

            Flake = new Sprite();

            SelectedIndex = 0;
            ConfirmedSelectedIndex = -1;
            ScrollIndex = 0;
            Startup = true;
            Mode = "Menu";
            newsavename = "";

            Savefilenames = new List<String>();

            //lastsong = new Song();

            Flakes = new List<Snowflake>();
            Die = new Random();
        }

        public void LoadContent(ContentManager myContentManager, String titlesongname)
        {
            TitlePic.LoadContent(myContentManager, "title/artistquest");
            Background.LoadContent(myContentManager, "title/titlebackground");
            NewGame.LoadContent(myContentManager, "title/titlenewgame");
            LoadGame.LoadContent(myContentManager, "title/titleloadgame");
            SaveGame.LoadContent(myContentManager, "title/titlesavegame");
            Exit.LoadContent(myContentManager, "title/titleexit");
            Resume.LoadContent(myContentManager, "title/titleresume");
            FileSlot.LoadContent(myContentManager, "title/titlefileblank");
            ArrowDown.LoadContent(myContentManager, "title/arroww");
            ArrowUp.LoadContent(myContentManager, "title/arrowwup");

            Flake.LoadContent(myContentManager, "title/snowflake");

            TitlePic.Position   =   new Vector2(396, 70);
            NewGame.Position    =   new Vector2(400, 160);
            SaveGame.Position   =   new Vector2(400, 220);
            LoadGame.Position   =   new Vector2(400, 280);
            Exit.Position       =   new Vector2(400, 340);
            Resume.Position     =   new Vector2(400, 160);
            FileSlot.Position   =   new Vector2(320, 160);
            ArrowUp.Position    =   new Vector2(638, 140);
            ArrowDown.Position  =   new Vector2(638, 360);

            myfont = myContentManager.Load<SpriteFont>("Titlefont");

            titlesong = myContentManager.Load<Song>("music\\" + titlesongname);
        }

        public int Update(GameTime gametime)
        {
            MakeSnowCounter++;
            
            //Create a random number of snowflakes!
            if (MakeSnowCounter >= 10)
            {
                for (int x = 0; x <= Die.Next(GenerateMax); x++)
                {
                    //Randomly create the x-position of the new flake!
                    int XPos = Die.Next(790);
                    int TTL = Die.Next(3000, maxTDL);
                    int YVel = Die.Next(4);
                    int XVel = Die.Next(2);
                    int XPositive = Die.Next(5);
                    if (XPositive > 3)
                        XVel *= -1;
                    Snowflake NewFlake = new Snowflake(new Vector2(XPos, -15), new Vector2(XVel, YVel), TTL);
                    Flakes.Add(NewFlake);
                }

                MakeSnowCounter = 0;
            }

            List<Snowflake> SnowsToRemove = new List<Snowflake>();
            //Cicle through all the flakes and Update them and draw em!
            foreach(Snowflake flake in Flakes)
            {
                if (flake.Update(gametime) == true)
                {
                    SnowsToRemove.Add(flake);
                }
            }

            foreach (Snowflake flake in SnowsToRemove)
            {
                Flakes.Remove(flake);
            }

            return ConfirmedSelectedIndex;
        }

        public void SelectUp()
        {
            if (Mode == "Menu" && Startup && SelectedIndex == 2 || Mode == "Menu" && DontSave && SelectedIndex == 2)
                SelectedIndex -= 2;
            if (Mode == "Load" && SelectedIndex > -1 && ScrollIndex > 0 || Mode == "Save" && SelectedIndex > -1 && ScrollIndex > 0)
                SelectedIndex--;
            else if (SelectedIndex > 0)
                SelectedIndex--;

            if (Mode == "Save" && SelectedIndex == -1 && ScrollIndex > 0 || Mode == "Load" && SelectedIndex == -1 && ScrollIndex > 0)
            {
                ScrollIndex--;
                SelectedIndex++;
            }
        }
        
        public void SelectDown()
        {
            if (Mode == "Menu" && Startup && SelectedIndex == 0 || Mode == "Menu" && DontSave && SelectedIndex == 0)
                SelectedIndex += 2;
            else if (Mode == "Menu" && SelectedIndex < 3)
                SelectedIndex++;
            else if (Mode == "Save" && SelectedIndex < 5 && (SelectedIndex + ScrollIndex + 1) <= Savefilenames.Count - 1 || Mode == "Load" && SelectedIndex < 5 && (SelectedIndex + ScrollIndex + 1) < Savefilenames.Count)
                SelectedIndex++;

            if (Mode == "Save" && SelectedIndex == 5 && (SelectedIndex + ScrollIndex) <= Savefilenames.Count - 1|| Mode == "Load" && SelectedIndex == 5 && (SelectedIndex + ScrollIndex + 1) <= Savefilenames.Count)
            {
                ScrollIndex++;
                SelectedIndex--;
            }
            Console.WriteLine("Selected Index: " + SelectedIndex.ToString());
            Console.WriteLine("ScrollIndex: " + ScrollIndex.ToString());
            Console.WriteLine("Savefilenames.Count: " + Savefilenames.Count.ToString());
        }

        public void Confirm()
        {
            ConfirmedSelectedIndex = SelectedIndex;
        }

        public void Decline()
        {
            if (Mode == "Save" || Mode == "Load")
            {
                Mode = "Menu";
                SelectedIndex = 0;
            }
            if (Mode == "WriteSave")
            {
                Mode = "Save";
            }
        }

        public void Show(bool dontsave, Song songbefore)
        {
            ConfirmedSelectedIndex = -1;
            DontSave = dontsave;
            lastsong = songbefore;
        }

        public void Draw(SpriteBatch mySpriteBatch, Boolean GameStartup, SpriteFont Font, String Path)
        {
            Background.Draw(mySpriteBatch);

            //Draw the Snowflakes
            foreach (Snowflake flake in Flakes)
            {
                Flake.Position = flake.Position;
                Flake.Color.A = 255;
                Flake.Draw(mySpriteBatch);
            }

            TitlePic.Draw(mySpriteBatch);
            
            if (GameStartup)
            {
                mySpriteBatch.DrawString(myfont, "Loading...", new Vector2(330, 200), Color.LightPink);
            }
            else
            {
                if (Mode == "Menu")
                {
                    if (Startup)
                        NewGame.Draw(mySpriteBatch);   
                    else
                        Resume.Draw(mySpriteBatch);

                    if (Startup || DontSave)
                    {
                        SaveGame.Color = Color.Gray;
                    }

                    SaveGame.Draw(mySpriteBatch);
                    LoadGame.Draw(mySpriteBatch);

                    Exit.Draw(mySpriteBatch);

                    NewGame.Color = Color.White;
                    SaveGame.Color = Color.White;
                    LoadGame.Color = Color.White;
                    Exit.Color = Color.White;
                    Resume.Color = Color.White;

                    switch (SelectedIndex)
                    {
                        case 0:
                            if (Startup)
                            {
                                NewGame.Color = Color.Magenta;
                                NewGame.Draw(mySpriteBatch);
                            }
                            else
                            {
                                Resume.Color = Color.Magenta;
                                Resume.Draw(mySpriteBatch);
                            }
                            break;
                        case 1:
                            if(!Startup)
                                SaveGame.Color = Color.Magenta;
                            SaveGame.Draw(mySpriteBatch);
                            break;
                        case 2:
                            LoadGame.Color = Color.Magenta;
                            LoadGame.Draw(mySpriteBatch);
                            break;
                        case 3:
                            Exit.Color = Color.Magenta;
                            Exit.Draw(mySpriteBatch);
                            break;
                    }
                }
                if (Mode == "Save" || Mode == "Load" || Mode == "WriteSave")
                {
                    FileSlot.Color = Color.White;

                    //Draw the bars behind the filenames
                    for (int i = 1; i <= 5; i++)
                    {
                        FileSlot.Position = new Vector2(410, 100+(i*50));
                        FileSlot.Draw(mySpriteBatch);
                    }

                    FileSlot.Color = Color.Magenta;
                    FileSlot.Position = new Vector2(410, 100 + ((SelectedIndex+1) * 50));
                    FileSlot.Draw(mySpriteBatch);

                    //Get the names of the available save files!
                    Savefilenames.Clear();
                    foreach(string f in Directory.GetFiles(Path))
                    {
                        string filename=f.Substring(f.LastIndexOf(@"\")+1);
                        //string fn_withoutextn=filename.Substring(0,filename.IndexOf(@"."));
                        if (Mode == "Save" && filename != "Autosave" || Mode == "Load" || Mode == "WriteSave")
                            Savefilenames.Add(filename.ToString());
                    }

                    if (ScrollIndex + 5 < Savefilenames.Count())
                        ArrowDown.Draw(mySpriteBatch);

                    if (ScrollIndex > 0)
                        ArrowUp.Draw(mySpriteBatch);

                  
                    for (int x = 0; x < 5; x++)
                    {
                        if (Savefilenames.Count > x + ScrollIndex)
                        {
                            if (x == SelectedIndex && Mode != "WriteSave")
                                mySpriteBatch.DrawString(Font, Savefilenames[x + ScrollIndex], new Vector2(220, 137 + (x * 50)), Color.YellowGreen);
                            else
                            {
                                mySpriteBatch.DrawString(Font, Savefilenames[x + ScrollIndex], new Vector2(220, 137 + (x * 50)), Color.Red);
                            }
                        }
                    }


                    //int counter = 0;
                    //foreach (string file in Savefilenames)
                    //{
                    //    if (counter == SelectedIndex && Mode != "WriteSave")
                    //        mySpriteBatch.DrawString(Font, file, new Vector2(220, 137 + (counter * 50)), Color.YellowGreen);
                    //    else
                    //        mySpriteBatch.DrawString(Font, file, new Vector2(220, 137 + (counter * 50)), Color.Red);

                    //    counter++;
                    //}

                    if (Mode == "WriteSave")
                    {
                        mySpriteBatch.DrawString(Font, ">:" + newsavename, new Vector2(220, 137 + (SelectedIndex * 50)), Color.YellowGreen);
                    }

                }
                
            }
        }

    }
}
