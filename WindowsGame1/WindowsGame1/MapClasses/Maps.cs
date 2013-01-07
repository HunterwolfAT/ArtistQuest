using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace WindowsGame1
{
    public class Maps : storagemanager
    {
        public String name;

        public Sprite image;
        public String backgroundimage;
        public String backgroundmusic;
        public List<Rectangle> Walkrects = new List<Rectangle>();
        public List<Object> Objects = new List<Object>();
        public Boolean introplayed = false;

        //Editor Variables
        public Boolean ShowWalkmap = false;
        public Boolean ShowObjectRects = false;
        public Boolean ShowInteractionHandles = false;
        public int HighlightedWalkmap = 0;

        public Maps()
        {
            image = new Sprite();

            backgroundimage = "roomfarbefragezeichen";
            backgroundmusic = null;

            name = "NewRoom";
        }

        public void LoadContent(ContentManager myContentManager)
        {
            image.LoadContent(myContentManager, backgroundimage);
        }

        public List<Rectangle> getWalkrects() { return Walkrects; }
        public List<Object> getObjects() { return Objects; }

        public void AddWalkRect(Rectangle rect)
        {
            Walkrects.Add(rect);
        }

        public void AddObject(Object obj)
        {
            Objects.Add(obj);
        }

        public Object FindObject(String name)
        {
            foreach (Object obj in Objects)
            {
                if (obj.name == name)
                {
                    return obj;
                }
            }

            return null;
        }

        public void Draw(SpriteBatch mySpriteBatch, GraphicsDeviceManager graphics, Vector2 playerpos, Boolean frontofplayer = false, Boolean isascriptrunning = false)
        {

            if (frontofplayer == false)
            {
                image.Draw(mySpriteBatch);

                foreach (Object obj in Objects)
                {
                    if (playerpos.Y > (obj.getActualRect().Y + obj.getActualRect().Height))
                        obj.Draw(mySpriteBatch, isascriptrunning);
                }

                // Draw the WalkMap rectangles, if option in editor was checked:
                if (ShowWalkmap)
                {
                    if (Walkrects.Count > 0)
                    {
                        foreach (Rectangle rect in Walkrects)
                        {
                            Texture2D tex = new Texture2D(graphics.GraphicsDevice, graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight);

                            Color[] data = new Color[graphics.PreferredBackBufferWidth * graphics.PreferredBackBufferHeight];

                            for (int i = 0; i < data.Length; ++i) data[i] = Color.Chocolate;

                            if (HighlightedWalkmap < Walkrects.Count)
                            {
                                if (Walkrects[HighlightedWalkmap] == rect)
                                    //Highlighted Rectangle
                                    for (int i = 0; i < data.Length; ++i) data[i] = Color.Fuchsia;
                            }

                            tex.SetData(data);


                            mySpriteBatch.Draw(tex, rect, Color.White);
                        }
                    }
                }

                if (ShowObjectRects)
                {
                    if (Objects.Count > 0)
                    {
                        foreach (Object obj in Objects)
                        {
                            Texture2D tex = new Texture2D(graphics.GraphicsDevice, graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight);

                            Color[] data = new Color[graphics.PreferredBackBufferWidth * graphics.PreferredBackBufferHeight];

                            for (int i = 0; i < data.Length; ++i) data[i] = Color.Aqua;

                            if (HighlightedWalkmap < Walkrects.Count)
                            {
                                //if (Walkrects[HighlightedWalkmap] == obj)
                                //    //Highlighted Rectangle
                                //    for (int i = 0; i < data.Length; ++i) data[i] = Color.Fuchsia;
                            }

                            tex.SetData(data);


                            mySpriteBatch.Draw(tex, new Rectangle(obj.rect.X + (int)obj.rectOffset.X, obj.rect.Y + (int)obj.rectOffset.Y, obj.rect.Width, obj.rect.Height), Color.White);
                        }
                    }
                }
            }
            else if (frontofplayer == true)
            {
                foreach (Object obj in Objects)
                {
                    if (playerpos.Y <= (obj.getActualRect().Y + obj.getActualRect().Height))
                        obj.Draw(mySpriteBatch, isascriptrunning);
                }
            }

            if (ShowInteractionHandles)
            {
                foreach (Object obj in Objects)
                {
                    Texture2D tex = new Texture2D(graphics.GraphicsDevice, 10, 10);

                    Color[] data = new Color[10 * 10];

                    for (int i = 0; i < data.Length; ++i) data[i] = Color.Red;

                    tex.SetData(data);

                    mySpriteBatch.Draw(tex, new Rectangle(((obj.getActualRect().X + obj.getActualRect().Width/2) + (int)obj.interactOffset.X) - 2, ((obj.getActualRect().Y + obj.getActualRect().Height/2) + (int)obj.interactOffset.Y) - 2, 4, 4), Color.White);
                }
            }

        }

        public Script PlayIntro()
        {
            foreach (Object obj in Objects)
            {
                if (obj.name == "System")
                {
                    foreach (Script script in obj.scripts)
                    {
                        if (script.Name == "Intro")
                        {
                            if (!introplayed)
                            {
                                introplayed = true;
                                return script;
                            }
                        }
                    }
                }
            }

            Console.WriteLine("Map doesnt have intro or the intro has already been played and therefore disabled.");
            return null;
        }

    }
}
