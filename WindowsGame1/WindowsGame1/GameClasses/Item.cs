using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace WindowsGame1
{
    public class Item
    {
        public String Name;
        public String Picturename;
        Sprite Picture;

        public List<Script> scripts;

        public Item()
        {
            Name = "";
            Picture = new Sprite();

            scripts = new List<Script>();
        }

        public Item(String name, String picname)
        {
            Name = name;
            Picturename = picname;

            Picture = new Sprite();

            scripts = new List<Script>();
        }

        public void LoadContent(ContentManager myContentManager)
        {
            Picture.LoadContent(myContentManager, Picturename);
        }

        public void Draw(Vector2 position, SpriteBatch mySpriteBatch)
        {
            Picture.Position = position;
            Picture.Draw(mySpriteBatch);
        }

        public Sprite getPicture()
        {
            return Picture;
        }
    }
}
