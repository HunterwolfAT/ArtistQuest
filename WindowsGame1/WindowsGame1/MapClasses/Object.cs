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
        public float opacity;

        public List<Script> scripts;
        public List<Sprite> images;
        public List<String> imagenames;
        public List<AnimatedSprite> aniimages;

        /// <summary>
        /// Represents everything in a room/map that isn't the player. Null-Constructor for the storagemanager!
        /// </summary>
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
            opacity = 1f;

            scripts = new List<Script>();
        }
        
        /// <summary>
        /// Returns the rect including every offset
        /// </summary>
        public Rectangle getActualRect()
        {
            return new Rectangle(rect.X + (int)rectOffset.X, rect.Y + (int)rectOffset.Y, rect.Width, rect.Height);
        }

        /// <summary>
        /// Represents everything in a room/map that isn't the player.
        /// </summary>
        /// <param name="newname">Name of the Object</param>
        /// <param name="newimage">Name of an imagefile</param>
        /// <param name="position">Position of the new Object</param>
        /// <param name="newwalkable">Defines if the player can walk through/over the object</param>
        /// <param name="newvisible">Defines wether the Object will be drawn</param>
        /// <param name="newradius">The radius in which the played needs to be to interact with the Object (Objects need Verbs so something happens)</param>
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

        /// <summary>
        /// Initializes the rect of the Object
        /// </summary>
        public void Init()
        {
            rect.Width = images[0].Texture.Width;
            rect.Height = images[0].Texture.Height;
        }

        /// <summary>
        /// Add an image to the imagelist of the object.
        /// </summary>
        /// <param name="myContentManager">XNA Contentmanager</param>
        /// <param name="spritename">Name of the image file in the XNA Contentpipeline</param>
        public void AddSprite(ContentManager myContentManager, String spritename)
        {
            Sprite NewSprite = new Sprite();
            NewSprite.LoadContent(myContentManager, spritename);

            images.Add(NewSprite);
            imagenames.Add(spritename);
        }

        /// <summary>
        /// Add an animation
        /// </summary>
        /// <param name="myContentManager">XNA Contentmanager</param>
        /// <param name="imagename">Name of the image file in the XNA Contentpipelin</param>
        /// <param name="colum">Number of frames in one column of the image</param>
        /// <param name="row">Number of frames in one row of the image</param>
        /// <param name="name">Name of the new animation</param>
        /// <param name="speed">How fast the animation is going to be played</param>
        public void AddAniSprite(ContentManager myContentManager, String imagename, int colum, int row, string name, int speed)
        {
            Texture2D texture = myContentManager.Load<Texture2D>(imagename);

            AnimatedSprite NewAniSprite = new AnimatedSprite(name, imagename, texture, row, colum, speed);
            aniimages.Add(NewAniSprite);
        }

        /// <summary>
        /// Render the object at it's position on the screen, with it's current frame (either animation or not)
        /// </summary>
        /// <param name="mySpriteBatch">XNA SpriteBatch</param>
        public void Draw(SpriteBatch mySpriteBatch, Boolean isascriptrunning)
        {
            if (aniimagenum == -1)
            {
                if (isascriptrunning)
                    images[imagenum].Color = Color.White;
                
                //images[imagenum].Color.A = (byte)opacity;     //This is XNA3.5 Code!
                if (visible)
                    images[imagenum].Draw(mySpriteBatch, opacity);
            }
            else
            {
                //if (isascriptrunning)
                //    aniimages[aniimagenum].Color = Color.White;
                
                aniimages[aniimagenum].Draw(mySpriteBatch, new Vector2(rect.X, rect.Y), opacity);
            }
        }

        /// <summary>
        /// Process various calculations regarding the Object.
        /// </summary>
        /// <param name="playerrect">The rect of the player character. Needed for collisiondetection and interaction.</param>
        /// <returns></returns>
        public Boolean Update(Rectangle playerrect, int playerdirection)
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

            if (CheckPlayerDistance(playerrect, playerdirection) && activeverbcount != 0)
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

            // Fade the object out or in
            if (fading)
            {
                Console.WriteLine(opacity);
                if (fadedout)
                {
                    opacity += (float)movingspeed / 100;
                    if (opacity >= 1)
                    {
                        opacity = 1;
                        fadedout = false;
                        fading = false;
                    }
                }
                else
                {
                    opacity -= (float)movingspeed / 100;
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

        /// <summary>
        /// Initialize to move the object over the specified path. Values are relative, not absolute.
        /// </summary>
        /// <param name="x">How far the object is supposed to move in X</param>
        /// <param name="y">How far the object is supposed to move in Y</param>
        /// <param name="speed">How many pixels the object is supposed to move in one update</param>
        public void move(int x, int y, int speed)
        {
            OriginalPostition = new Vector2(rect.X, rect.Y);
            deltaX = x;
            deltaY = y;
            movingspeed = speed;
            moving = true;
        }

        /// <summary>
        /// Initializes a fade in (appear)
        /// </summary>
        /// <param name="speed">Will be added to Opacity variable every Update</param>
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

        /// <summary>
        /// Initializes a fade out (disappear)
        /// </summary>
        /// <param name="speed">Will be substracted from Opacity variable every Update</param>
        public void fadeout(int speed)
        {
            movingspeed = speed;
            fading = true;
            fadedout = false;
        }

        /// <summary>
        /// Determines if the player is closer to the object than it's range.
        /// </summary>
        /// <param name="playerrect">The rect of the player character</param>
        /// <param name="playerdirection">The way the player is facing. 0 = Up, 1 = Right, 2 = Down, 3 = Left</param>
        /// <returns>Return true, if the player is facing the object and is closer to the object than its range value.</returns>
        public Boolean CheckPlayerDistance(Rectangle playerrect, int playerdirection)
        {
            Vector2 obrect = new Vector2(rect.X + (rect.Width / 2) + rectOffset.X + interactOffset.X, rect.Y + (rect.Height / 2) + rectOffset.Y + interactOffset.Y);
            Vector2 playerpos = new Vector2(playerrect.X + (playerrect.Width / 2), playerrect.Y + (playerrect.Height / 2));

            if (playerdirection == 0 && obrect.Y < playerpos.Y || playerdirection == 1 && obrect.X > playerpos.X ||
                playerdirection == 2 && obrect.Y > playerpos.Y || playerdirection == 3 && obrect.X < playerpos.X)
            {
                if (Vector2.DistanceSquared(playerpos, obrect) < radius)
                    return true;      
            }

            return false;
        }

        /// <summary>
        /// Find a certain script in this object's list of scripts by name.
        /// </summary>
        /// <param name="name">The name of the script you're looking for</param>
        public Script FindScript(String name)
        {
            foreach (Script script in scripts)
                if (script.Name == name)
                    return script;

            return null;
        }

        /// <summary>
        /// Creates an empty script and adds it to it's list.
        /// </summary>
        /// <param name="name">The name of the new script</param>
        public void AddScript(String name)
        {
            Script newscript = new Script(name);
            scripts.Add(newscript);
        }
    }
}
