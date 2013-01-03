using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace WindowsGame1
{
    public class verbmenu
    {
        Sprite background;
        Sprite backgroundascii;
        SpriteFont font;

        Vector2 position;
        public Boolean Shown;
        public Boolean asciiMode;
        int selected;

        Sprite EnterKeySprite;
        Sprite ShiftKeySprite;
        Sprite EnterKeySpriteascii;
        Sprite ShiftKeySpriteascii;

        Color TextColor;
        Color ActivateCol;
        Color DeactivateCol;
        
        Object CurrentObject;

        public verbmenu()
        {
            Shown = false;
            background = new Sprite();
            backgroundascii = new Sprite();

            EnterKeySprite = new Sprite();
            ShiftKeySprite = new Sprite();
            EnterKeySpriteascii = new Sprite();
            ShiftKeySpriteascii = new Sprite();

            TextColor = Color.White;
            ActivateCol = Color.SeaShell;
            DeactivateCol = Color.Black;
        }

        public void LoadContent(ContentManager myContentManager, SpriteFont myfont)
        {
            background.LoadContent(myContentManager, "menu");
            backgroundascii.LoadContent(myContentManager, "menuascii");
            font = myfont;

            EnterKeySprite.LoadContent(myContentManager, "keys/enterkey");
            ShiftKeySprite.LoadContent(myContentManager, "keys/rshiftkey");
            EnterKeySpriteascii.LoadContent(myContentManager, "keys/enterkeyascii");
            ShiftKeySpriteascii.LoadContent(myContentManager, "keys/rshiftkeyascii");
        }

        public void Show(Object obj, int screenwidth)
        {
            Shown = true;
            CurrentObject = obj;
            position = new Vector2(obj.rect.X + obj.rect.Width, obj.rect.Y - obj.rect.Height);
            if (position.Y < 80)
            {
                position.Y = 80;
                position.X += 50;
            }
            if (position.X > screenwidth - 60)
                position.X = screenwidth - 120;
            background.Position = position;
            backgroundascii.Position = position;
            selected = 0;
        }

        public void Hide()
        {
            Shown = false;
        }

        public void goUp()
        {
            if (selected - 1 >= 0)
                selected--;
        }

        public void goDown(List<Script> verblist)
        {
            //Count the currently active verbs
            int verbcount = 0;
            
            foreach (Script verb in verblist)
            {
                if (verb.Active)
                    verbcount++;
            }
            
            if (selected + 1 < verbcount)
                selected++;
        }

        public int getSelectedItem()
        {
            while (CurrentObject.scripts[selected].Active == false)
                selected++;
            
            return selected;
        }

        public void toggleAscii(bool on = true)
        {
            asciiMode = on;
            if (asciiMode)
            {
                TextColor = Color.Green;
                ActivateCol = Color.LightGreen;
                DeactivateCol = Color.DarkGreen;
            }
            else
            {
                TextColor = Color.White;
                ActivateCol = Color.SeaShell;
                DeactivateCol = Color.Black;
            }
        }

        public void Draw(SpriteBatch mySpriteBatch)
        {
            if (Shown)
            {
                if (!asciiMode)
                    background.Draw(mySpriteBatch);
                else
                    backgroundascii.Draw(mySpriteBatch);
                
                if (CurrentObject.scripts.Count != 0)
                {
                    mySpriteBatch.DrawString(font, CurrentObject.name, new Vector2(position.X - 37, position.Y - 70), TextColor);
                    
                    int a = 0;
                    foreach (Script script in CurrentObject.scripts)
                    {
                        if(script.Active)
                        {
                            Vector2 textpos = new Vector2(position.X - 33, (position.Y + (a * 28)) - 40);
                            if (a == selected)
                                mySpriteBatch.DrawString(font, script.Name, textpos, ActivateCol);
                            else
                                mySpriteBatch.DrawString(font, script.Name, textpos, DeactivateCol);
                            a++;
                            // Don't draw more than three verbs
                            if (a == 3)
                                break;
                        }
                    }

                    //Draw the enter-key
                    if (!asciiMode)
                    {                  
                        EnterKeySprite.Position = new Vector2(position.X - 70, position.Y + 68);
                        EnterKeySprite.Draw(mySpriteBatch);
                    }
                    else
                    {
                        EnterKeySpriteascii.Position = new Vector2(position.X - 70, position.Y + 68);
                        EnterKeySpriteascii.Draw(mySpriteBatch);
                    }

                    mySpriteBatch.DrawString(font, "Enter", new Vector2(position.X - 56, position.Y + 68), TextColor);

                    //Draw the SHift-Key
                    if (!asciiMode)
                    {
                        ShiftKeySprite.Position = new Vector2(position.X + 10, position.Y + 64);
                        ShiftKeySprite.Draw(mySpriteBatch);
                    }
                    else
                    {
                        ShiftKeySpriteascii.Position = new Vector2(position.X + 10, position.Y + 64);
                        ShiftKeySpriteascii.Draw(mySpriteBatch);
                    }

                    mySpriteBatch.DrawString(font, "Cancel", new Vector2(position.X + 10, position.Y + 68), TextColor);
                }
            }
        }

    }
}
