using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace WindowsGame1
{
    public class Player
    {
        enum directions { up, right, down, left };

        public Texture2D image;
        public Texture2D talkingimage;
        public Sprite imageascii;
        public AnimatedSprite animimage;
        public AnimatedSprite talkimage;

        public Vector2 acc, speed, position;
        public Rectangle playerRect;
        public int direction;
        public bool iswalking = false;
        public bool istalking = false;
        public int animationcounter = 0;
        public Boolean visible = true;

        //public Rectangle Rect;
        Vector2 OriginalPostition;

        int deltaX = 0;
        int deltaY = 0;
        int xspeed = 0;
        int yspeed = 0;

        bool movewithanim = true;
        public bool moving = false;
        public bool movelock = false;

        public bool showRect = false;

        public verbmenu verbmenu;
        public Object selectedObject;

        public List<Item> InvList;

        public bool asciimode = false;

        public bool clipping = true;


        public Player()
        {
            playerRect = new Rectangle(200, 200, 1, 1);
            position = new Vector2(352f, 212f);

            verbmenu = new verbmenu();

            InvList = new List<Item>();

            direction = 2;
        }

        public void LoadContent(ContentManager myContentManager, SpriteFont font)
        {
            image = myContentManager.Load<Texture2D>("figur");
            talkingimage = myContentManager.Load<Texture2D>("figurtalking");
            imageascii = new Sprite();
            imageascii.LoadContent(myContentManager, "at");
            animimage = new AnimatedSprite("hero", "figur", image, 4, 9);
            talkimage = new AnimatedSprite("herotalking", "figurtalking", talkingimage, 4, 5);

            playerRect.Width = (animimage.Texture.Width / animimage.Columns) / 2;
            playerRect.Height = (animimage.Texture.Height / animimage.Rows) / 5;

            verbmenu.LoadContent(myContentManager, font);
        }

        public void AddItem(Item item, ContentManager myContentManager)
        {
            item.LoadContent(myContentManager);
            InvList.Add(item);
        }

        public void Draw(SpriteBatch mySpriteBatch, GraphicsDeviceManager graphics)
        {
            if (showRect)
            {
                Texture2D tex = new Texture2D(graphics.GraphicsDevice, graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight);

                Color[] data = new Color[graphics.PreferredBackBufferWidth * graphics.PreferredBackBufferHeight];
                for (int i = 0; i < data.Length; ++i) data[i] = Color.DarkViolet;
                tex.SetData(data);

                mySpriteBatch.Draw(tex, playerRect, Color.White);
            }

            if (visible)
            {
                if (!asciimode)
                {
                    if (!istalking)
                        animimage.Draw(mySpriteBatch, new Vector2(playerRect.X - ((image.Width / animimage.Columns) / 5), playerRect.Y - (((image.Height / animimage.Rows) / 5) * 4)));
                    else
                        talkimage.Draw(mySpriteBatch, new Vector2(playerRect.X - ((image.Width / animimage.Columns) / 5), playerRect.Y - (((image.Height / animimage.Rows) / 5) * 4)));
                }
                else
                {
                    imageascii.Position = new Vector2(position.X, position.Y);
                    imageascii.Draw(mySpriteBatch);
                }
            }
                //animimage.Draw(mySpriteBatch, new Vector2(playerRect.X, playerRect.Y));
            
        }

        public bool CheckCollision(Vector2 delta, List<Rectangle> rects, List<Object> objects)
        {
            bool allisok = false;

            //Rounding up the delta so the player wont get stuck in walls
            int intx = (int)Math.Ceiling(delta.X) - 1;
            int inty = (int)Math.Ceiling(delta.Y);
            
            foreach (Rectangle walkmap in rects)
            {
                Rectangle deltaRect = new Rectangle(playerRect.X + intx, playerRect.Y + inty, playerRect.Width, playerRect.Height);

                if (walkmap.Contains(deltaRect))
                {
                    allisok = true;
                }
            }
            
            foreach (Object obj in objects)
            {
                if (obj.getActualRect().Intersects(new Rectangle(playerRect.X + intx, playerRect.Y + inty, playerRect.Width, playerRect.Height)) && obj.walkable == false)
                    allisok = false;
            }

            if (!clipping)
                allisok = true;

            return allisok;
        }

        public void move(float deltax, float deltay)
        {
            position.X += deltax;
            position.Y += deltay;

            if (deltay > 0)
                direction = (int)directions.down;
            if (deltay < 0)
                direction = (int)directions.up;
            if (deltax > 0)
                direction = (int)directions.right;
            if (deltax < 0)
                direction = (int)directions.left;

            iswalking = true;
        }

        public void move(int deltax, int deltay)
        {
            position.X += deltax;
            position.Y += deltay;

            if (deltay > 0)
                direction = (int)directions.down;
            if (deltay < 0)
                direction = (int)directions.up;
            if (deltax > 0)
                direction = (int)directions.right;
            if (deltax < 0)
                direction = (int)directions.left;

            if (movewithanim)
                iswalking = true;
        }

        public void move(int x, int y, int speed, bool animate)
        {
            OriginalPostition = new Vector2(position.X, position.Y);
            deltaX = x;
            deltaY = y;
            xspeed = yspeed = speed;
            moving = true;
            movewithanim = animate;

            if (deltaX == 0)
                xspeed = 0;
            else if (deltaX < 0)
                xspeed *= -1;

            if (deltaY == 0)
                yspeed = 0;
            else if (deltaY < 0)
                yspeed *= -1;
        }

        public void CheckObject(int screenwidth)
        {
            if (selectedObject != null)
            {
                if (!verbmenu.Shown)
                    verbmenu.Show(selectedObject, screenwidth);
                else
                    verbmenu.Hide();
            }
        }

        public void Update(bool guimsgscrollingdone)
        {
            // Automatic Movement over a specified distance
            if (moving)
            {
                move(xspeed, yspeed);

                bool XDestinationReached = false;
                bool YDestinationReached = false;

                if (deltaX > 0 && OriginalPostition.X + deltaX <= position.X)
                {
                    position.X = (int)OriginalPostition.X + deltaX;
                    XDestinationReached = true;
                }
                if (deltaX < 0 && OriginalPostition.X + deltaX >= position.X)
                {
                    position.X = (int)OriginalPostition.X + deltaX;
                    XDestinationReached = true;
                }

                if (deltaY > 0 && OriginalPostition.Y + deltaY <= position.Y)
                {
                    position.Y = (int)OriginalPostition.Y + deltaY;
                    YDestinationReached = true;
                }
                if (deltaY < 0 && OriginalPostition.Y + deltaY >= position.Y)
                {
                    position.Y = (int)OriginalPostition.Y + deltaY;
                    YDestinationReached = true;
                }

                if (deltaX == 0)
                {
                    XDestinationReached = true;
                    position.X = (int)OriginalPostition.X;
                }
                if (deltaY == 0)
                {
                    YDestinationReached = true;
                    position.Y = (int)OriginalPostition.Y;
                }

                if (XDestinationReached && YDestinationReached)
                {
                    moving = false;
                    movewithanim = true;
                }
            }

            if (!asciimode)
            {
                playerRect.X = (int)position.X;
                playerRect.Y = (int)position.Y;
            }
            else
            {
                playerRect.X = (int)position.X - playerRect.Width / 2;
                playerRect.Y = (int)position.Y - playerRect.Height / 2;
            }

            if (guimsgscrollingdone)
                istalking = false;
            else
                istalking = true;

            animate();

            if (!moving)
                iswalking = false;
            //animimage.Update();
        }

        public void animate()
        {
            if (direction == (int)directions.up)
            {
                if (iswalking)
                {
                    animationcounter++;
                    if (animationcounter > 35)
                        animationcounter = 0;
                }
                else if (istalking)
                {
                    animationcounter++;
                    if (animationcounter > 14)
                        animationcounter = 0;
                }
                else
                    animationcounter = 0;

                animimage.SetFrame(9 + (int)(animationcounter / 4));
                talkimage.SetFrame(5 + (int)(animationcounter / 3));
            }
            if (direction == (int)directions.down)
            {
                if (iswalking)
                {
                    animationcounter++;
                    if (animationcounter > 35)
                        animationcounter = 0;
                }
                else if (istalking)
                {
                    animationcounter++;
                    if (animationcounter > 14)
                        animationcounter = 0;
                }
                else
                    animationcounter = 0;

                animimage.SetFrame((int)(animationcounter / 4));
                talkimage.SetFrame((int)(animationcounter / 3));
            }
            if (direction == (int)directions.left)
            {
                if (iswalking)
                {
                    animationcounter++;
                    if (animationcounter > 31)
                        animationcounter = 0;
                }
                else if (istalking)
                {
                    animationcounter++;
                    if (animationcounter > 14)
                        animationcounter = 0;
                }
                else
                    animationcounter = 0;

                animimage.SetFrame(27 + (int)(animationcounter / 4));
                talkimage.SetFrame(15 + (int)(animationcounter / 3));
            }
            if (direction == (int)directions.right)
            {
                if (iswalking)
                {
                    animationcounter++;
                    if (animationcounter > 31)
                        animationcounter = 0;
                }
                else if (istalking)
                {
                    animationcounter++;
                    if (animationcounter > 14)
                        animationcounter = 0;
                }
                else
                    animationcounter = 0;

                animimage.SetFrame(18 + (int)(animationcounter / 4));
                talkimage.SetFrame(10 + (int)(animationcounter / 3));
            }
        }

        public void toggleAscii(bool on = true)
        {
            asciimode = on;
            if (asciimode)
            {
                //playerRect = new Rectangle((int)position.X, (int)position.Y, 24, 26);
                playerRect.Width = 24;
                playerRect.Height = 26;
            }
            else
            {
                playerRect.Width = (animimage.Texture.Width / animimage.Columns) / 2;
                playerRect.Height = (animimage.Texture.Height / animimage.Rows) / 5;
            }
        }
    }
}
