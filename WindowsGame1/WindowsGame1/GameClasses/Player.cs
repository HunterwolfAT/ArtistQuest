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
        public AnimatedSprite animimage;
        public Vector2 acc, speed, position;
        public Rectangle playerRect;
        public int direction;
        public bool iswalking = false;
        public int animationcounter = 0;
        public Boolean visible = true;
        //public Rectangle Rect;

        public bool showRect = false;

        public verbmenu verbmenu;
        public Object selectedObject;

        public List<Item> InvList;

        public bool clipping = true;

        public Player()
        {
            playerRect = new Rectangle(200, 200, 1, 1);
            position = new Vector2(200f, 150f);

            verbmenu = new verbmenu();

            InvList = new List<Item>();
        }

        public void LoadContent(ContentManager myContentManager, SpriteFont font)
        {
            image = myContentManager.Load<Texture2D>("figur");
            animimage = new AnimatedSprite("hero", "figur", image, 4, 9);

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
                animimage.Draw(mySpriteBatch, new Vector2(playerRect.X - ((image.Width / animimage.Columns) / 5), playerRect.Y - (((image.Height / animimage.Rows) / 5) * 4)));
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

            iswalking = true;
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

        public void Update()
        {
            //speed += acc;
            //image.Position += speed;
            playerRect.X = (int)position.X;
            playerRect.Y = (int)position.Y;

            animate();
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
                else
                    animationcounter = 0;

                animimage.SetFrame(9 + (int)(animationcounter / 4));
            }
            if (direction == (int)directions.down)
            {
                if (iswalking)
                {
                    animationcounter++;
                    if (animationcounter > 35)
                        animationcounter = 0;
                }
                else
                    animationcounter = 0;

                animimage.SetFrame((int)(animationcounter / 4));
            }
            if (direction == (int)directions.left)
            {
                if (iswalking)
                {
                    animationcounter++;
                    if (animationcounter > 31)
                        animationcounter = 0;
                }
                else
                    animationcounter = 0;

                animimage.SetFrame(27 + (int)(animationcounter / 4));
            }
            if (direction == (int)directions.right)
            {
                if (iswalking)
                {
                    animationcounter++;
                    if (animationcounter > 31)
                        animationcounter = 0;
                }
                else
                    animationcounter = 0;

                animimage.SetFrame(18 + (int)(animationcounter / 4));
            }
        }
    }
}
