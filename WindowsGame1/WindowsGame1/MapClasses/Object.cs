using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace WindowsGame1
{
    public class Object
    {
        public String name;
        public String imagename;
        public int imagenum = -1;
        public int aniimagenum = -1;
        public Boolean keepanimating = false;
        public Color color;
        public Rectangle rect;
        public Vector2 rectOffset;
        public Vector2 interactOffset;
        public Boolean walkable { get; set; }
        public Boolean visible { get; set; }
        public float radius;
        public Boolean moving = false;
        public int deltaX;
        public int deltaY;
        public Vector2 OriginalPostition;
        public int movingspeed;
        public Boolean fading = false;
        public Boolean fadedout = false;
        public int opacity;

        public List<Script> scripts;
        public List<Sprite> images;
        public List<String> imagenames;
        public List<AnimatedSprite> aniimages;

        public Object()
        {
            name = "";
            color = Color.White;
            rect = new Rectangle();
            walkable = true;
            visible = true;
            radius = 6000f;

            rectOffset = new Vector2(0f, 0f);

            images = new List<Sprite>();
            imagenames = new List<String>();
            aniimages = new List<AnimatedSprite>();
            keepanimating = false;

            imagenum = 0;

            movingspeed = 1;

            moving = false;

            fading = false;
            fadedout = false;
            opacity = 255;

            scripts = new List<Script>();
        }
        
        public Rectangle getActualRect()
        {
            return new Rectangle(rect.X + (int)rectOffset.X, rect.Y + (int)rectOffset.Y, rect.Width, rect.Height);
        }
        
        public Object(String newname, String newimage, Vector2 position, Boolean newwalkable = true, Boolean newvisible = true, float newradius = 6000f)
        {
            name = newname;
            rect = new Rectangle((int)position.X, (int)position.Y, 0, 0);
            rectOffset = new Vector2(0f, 0f);
            walkable = newwalkable;
            visible = newvisible;
            radius = newradius;

            color = Color.White;

            imagenames = new List<String>();
            imagenames.Add(newimage);

            images = new List<Sprite>();
            aniimages = new List<AnimatedSprite>();
            keepanimating = false;
            
            Sprite image = new Sprite();
            images.Add(image);
            imagenum = 0;

            scripts = new List<Script>();
        }

        public void LoadContent(ContentManager myContentManager)
        {

            if (images.Count > 0)
            {
                images[0].LoadContent(myContentManager, imagenames[0]);

                images[0].Position.X = rect.X + rect.Width / 2;
                images[0].Position.Y = rect.Y + rect.Height / 2;
            }

            if (images.Count > 1)
            {
                for (int a = 1; a < images.Count(); a++)
                {
                    images[a].LoadContent(myContentManager, imagenames[a]);
                }
            }

            //Console.WriteLine(this.name + "LOLOL " + aniimages.Count.ToString());

            if (aniimages.Count() > 0)
            {
                for (int k = 0; k < aniimages.Count(); k++)
                {
                    //Console.WriteLine("Loading " + k.ToString() + ": " + aniimages[k].name);
                    aniimages[k].LoadContent(myContentManager);
                }
            }

        }

        public void Init()
        {
            rect.Width = images[0].Texture.Width;
            rect.Height = images[0].Texture.Height;
        }

        public void AddSprite(ContentManager myContentManager, String spritename)
        {
            Sprite NewSprite = new Sprite();
            NewSprite.LoadContent(myContentManager, spritename);

            images.Add(NewSprite);
            imagenames.Add(spritename);
        }

        public void AddAniSprite(ContentManager myContentManager, String imagename, int colum, int row, string name, int speed)
        {
            Texture2D texture = myContentManager.Load<Texture2D>(imagename);

            AnimatedSprite NewAniSprite = new AnimatedSprite(name, imagename, texture, row, colum, speed);
            aniimages.Add(NewAniSprite);
        }

        public void Draw(SpriteBatch mySpriteBatch)
        {
            if (aniimagenum == -1)
            {
                images[imagenum].Color.A = (byte)opacity;
                if (visible)
                    images[imagenum].Draw(mySpriteBatch);
            }
            else
            {
                aniimages[aniimagenum].Draw(mySpriteBatch, new Vector2(rect.X, rect.Y));
            }
        }

        public Boolean Update(Rectangle playerrect)
        {
            bool playernear = false;

            images[imagenum].Color = Color.White;

            //Count the actual active verbs in the object
            int activeverbcount = 0;
            foreach (Script verb in scripts)
            {
                if (verb.Active)
                    activeverbcount++;
            }

            if (CheckPlayerDistance(playerrect) && activeverbcount != 0)
            {
                images[imagenum].Color = Color.YellowGreen;
                playernear = true;
            }

            /*foreach (Script script in scripts)
                script.Update();*/

            foreach (Sprite image in images)
            {
                image.Position.X = rect.X + rect.Width / 2;
                image.Position.Y = rect.Y + rect.Height / 2;
            }

            //If the object is supposed to be moving, move it!
            if (moving)
            {
                if (deltaX > 0)
                    rect.X += movingspeed;
                else
                    rect.X -= movingspeed;

                if (deltaY > 0)
                    rect.Y += movingspeed;
                else
                    rect.Y -= movingspeed;

                bool XDestinationReached = false;
                bool YDestinationReached = false;

                Console.WriteLine(deltaX);
                Console.WriteLine(deltaY);

                if (deltaX > 0 && OriginalPostition.X + deltaX <= rect.X)
                {
                    rect.X = (int)OriginalPostition.X + deltaX;
                    XDestinationReached = true;
                }
                if (deltaX < 0 && OriginalPostition.X + deltaX >= rect.X)
                {
                    rect.X = (int)OriginalPostition.X + deltaX;
                    XDestinationReached = true;
                }

                if (deltaY > 0 && OriginalPostition.Y + deltaY <= rect.Y)
                {
                    rect.Y = (int)OriginalPostition.Y + deltaY;
                    YDestinationReached = true;
                }
                if (deltaY < 0 && OriginalPostition.Y + deltaY >= rect.Y)
                {
                    rect.Y = (int)OriginalPostition.Y + deltaY;
                    YDestinationReached = true;
                }

                if (deltaX == 0)
                {
                    XDestinationReached = true;
                    rect.X = (int)OriginalPostition.X;
                }
                if (deltaY == 0)
                {
                    YDestinationReached = true;
                    rect.Y = (int)OriginalPostition.Y;
                }

                if (XDestinationReached && YDestinationReached)
                {
                    moving = false;
                }
            }

            if (fading)
            {
                Console.WriteLine(opacity);
                if (fadedout)
                {
                    opacity += movingspeed;
                    if (opacity >= 255)
                    {
                        opacity = 255;
                        fadedout = false;
                        fading = false;
                    }
                }
                else
                {
                    opacity -= movingspeed;
                    if (opacity <= 0)
                    {
                        opacity = 0;
                        fadedout = true;
                        fading = false;
                        visible = false;
                    }
                }


            }

            //if there is an animation, play it!
            if (aniimagenum != -1)
            {
                if (aniimages[aniimagenum].Update() && !keepanimating)
                    aniimagenum = -1;
            }

            return playernear;
        }

        public void move(int x, int y, int speed)
        {
            OriginalPostition = new Vector2(rect.X, rect.Y);
            deltaX = x;
            deltaY = y;
            movingspeed = speed;
            moving = true;
        }

        public void fadein(int speed)
        {
            movingspeed = speed;
            fading = true;
            fadedout = true;

            if (!visible)
            {
                visible = true;
                opacity = 0;
            }
        }

        public void fadeout(int speed)
        {
            movingspeed = speed;
            fading = true;
            fadedout = false;
        }

        public Boolean CheckPlayerDistance(Rectangle playerrect)
        {
            Vector2 obrect = new Vector2(rect.X + (rect.Width / 2) + rectOffset.X + interactOffset.X, rect.Y + (rect.Height / 2) + rectOffset.Y + interactOffset.Y);
            Vector2 playerpos = new Vector2(playerrect.X + (playerrect.Width / 2), playerrect.Y + (playerrect.Height / 2));

            //Console.WriteLine(Vector2.DistanceSquared(playerpos, obrect));

            if (Vector2.DistanceSquared(playerpos, obrect) < radius)
            {
                //Console.WriteLine(Math.Sqrt((obrect.X - playerpos.X) * (obrect.X - playerpos.X) + (playerpos.Y - obrect.Y) * (playerpos.Y - obrect.Y)));
                return true;
            }
            else
                return false;
        }

        public Script FindScript(String name)
        {
            foreach (Script script in scripts)
                if (script.Name == name)
                    return script;

            return null;
        }

        public void AddScript(String name)
        {
            Script newscript = new Script(name);
            scripts.Add(newscript);
        }
    }
}
