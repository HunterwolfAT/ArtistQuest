using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace WindowsGame1
{
    public class AnimatedSprite
    {
        public string name;
        public string imagefilename;
        public Texture2D Texture { get; set; }
        public int Rows { get; set; }
        public int Columns { get; set; }
        public Color color;
        private int currentFrame;
        private int totalFrames;
        public int animationspeed = 1;
        private int animationtick = 0;

        public AnimatedSprite()
        {
            //Rows = 0;
            //Columns = 0;
            currentFrame = 0;
            color = Color.White;
            totalFrames = Rows * Columns;
        }
        
        public AnimatedSprite(string newname, string filename, Texture2D texture, int rows, int columns, int speed = 1)
        {
            Texture = texture;
            Rows = rows;
            Columns = columns;
            currentFrame = 0;
            totalFrames = Rows * Columns;
            animationspeed = speed;
            name = newname;
            imagefilename = filename;
            color = Color.White;
        }

        public void LoadContent(ContentManager myContentMangager)
        {
            Texture = myContentMangager.Load<Texture2D>(imagefilename);
            totalFrames = Rows * Columns;
        }

        public Boolean Update()
        {
            animationtick++;
            if (animationtick >= animationspeed)
            {
                currentFrame++;
                if (currentFrame >= totalFrames)
                {
                    currentFrame = 0;
                    animationtick = 0;
                    return true;
                }
                animationtick = 0;
            }
            return false;
        }

        public void SetFrame(int frame)
        {
            currentFrame = frame;
        }

        public void SetAnimationSpeed(int speed)
        {
            animationspeed = speed;
        }

        public int GetAnimationSpeed()
        {
            return animationspeed;
        }

        public void Draw(SpriteBatch spriteBatch, Vector2 location)
        {
            int width = Texture.Width / Columns;
            int height = Texture.Height / Rows;
            int row = (int)((float)currentFrame / (float)Columns);
            int column = currentFrame % Columns;

            Rectangle sourceRectangle = new Rectangle(width * column, height * row, width, height);
            Rectangle destinationRectangle = new Rectangle((int)location.X, (int)location.Y, width, height);

            //Console.WriteLine(sourceRectangle);
            //Console.WriteLine(destinationRectangle);

            spriteBatch.Draw(Texture, destinationRectangle, sourceRectangle, color);
        }
    }
}