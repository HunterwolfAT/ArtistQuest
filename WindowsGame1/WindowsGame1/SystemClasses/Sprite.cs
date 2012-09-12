using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace WindowsGame1
{
    public class Sprite
    {
        public Texture2D Texture;
        public Vector2 Position = Vector2.Zero;
        public Rectangle? SourceRect = null;
        public Color Color = Color.White;
        public float Rotation = 0f;
        public Vector2 Origin;
        public float Scale = 1f;
        public SpriteEffects Effects = SpriteEffects.None;
        public float LayerDepth = 0;

        public void LoadContent(ContentManager myContentMangager, String filename)
        {
             Texture = myContentMangager.Load<Texture2D>(filename);
            Origin = new Vector2(Texture.Width / 2, Texture.Height / 2);
            Position = Origin;
        }
        
        public void Draw(SpriteBatch spriteBatch)
        {
            if (Texture == null)
                return;

            spriteBatch.Draw(
                Texture,
                Position,
                SourceRect,
                Color,
                Rotation,
                Origin,
                Scale,
                Effects,
                LayerDepth);
        }
    }
}
