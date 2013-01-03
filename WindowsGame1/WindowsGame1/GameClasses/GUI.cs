using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace WindowsGame1
{
    public class GUI
    {
        //Message (Text-) box variables
        Sprite MsgBox;
        Sprite MsgBoxAscii;
        String MSGBoxText;
        String ShownMSGBoxText;
        String MSGBoxTextName;
        Vector2 MSGposition;
        public Boolean ShowMSG;
        Boolean MSGisdone;
        Color MSGColor;
        float MSGOpacity = 0f;
        Boolean MSGMoving;
        int MSGYOffset;

        int scrollcounter;

        // Inventory variables
        Sprite InventoryBackground;
        Sprite InventoryBackgroundAscii;
        Sprite InventoryHighlight;
        int InvPositionY;
        public bool ShowInventory;
        public List<Item> InvList;
        public bool InventoryActive;

        public int InventorySelected;

        // NewItemBox variables
        public bool ShowItemBox;
        public bool AddedItem;      //If true, Item has been added, if false, Item has been removed from player inventory
        Sprite ItemPic;
        String ItemName;

        // ASCII-Mode
        public bool asciiMode;

        //Fade-Control variables
        Sprite ScreenFadeTex;
        public bool FadedOut = false;
        public bool Fading = false;
        float FadeOpacity;
        float FadeStep = 0.01f;

        // Key sprites
        Sprite EnterKeySprite;
        Sprite SpaceKeySprite;
        Sprite ShiftKeySprite;

        // Font font
        SpriteFont Font;
        Color FontColor;

        public GUI()
        {
            MSGBoxText = "";
            ShownMSGBoxText = "";
            ShowMSG = false;
            MSGisdone = true;
            MSGposition = new Vector2(10f, 10f);
            MSGColor = Color.White;
            MsgBox = new Sprite();
            MsgBoxAscii = new Sprite();
            asciiMode = false;

            InventoryBackground = new Sprite();
            InventoryBackgroundAscii = new Sprite();
            InventoryHighlight = new Sprite();
            InvPositionY = 543;
            InventoryBackground.Position = new Vector2(0f, (float)InvPositionY);
            ShowInventory = false;
            InvList = new List<Item>();
            InventorySelected = 0;

            ItemPic = new Sprite();

            ScreenFadeTex = new Sprite();
            FadeOpacity = 0f;

            EnterKeySprite = new Sprite();
            SpaceKeySprite = new Sprite();
            ShiftKeySprite = new Sprite();

            FontColor = Color.Wheat;

            scrollcounter = 0;
        }
        
        public GUI(SpriteFont font, List<Item> inventory)
        {
            MSGBoxText = "";
            ShownMSGBoxText = "";
            ShowMSG = false;
            MSGisdone = true;
            Font = font;
            MSGposition = new Vector2(10f, 10f);
            MSGColor = Color.White;
            MsgBox = new Sprite();
            MsgBoxAscii = new Sprite();
            asciiMode = false;

            InventoryBackground = new Sprite();
            InventoryBackgroundAscii = new Sprite();
            InventoryHighlight = new Sprite();
            InvPositionY = 543;
            InventoryBackground.Position = new Vector2(0f, (float)InvPositionY);
            ShowInventory = false;
            InvList = inventory;
            InventorySelected = 0;

            ItemPic = new Sprite();

            ScreenFadeTex = new Sprite();
            FadeOpacity = 0f;

            EnterKeySprite = new Sprite();
            SpaceKeySprite = new Sprite();
            ShiftKeySprite = new Sprite();

            FontColor = Color.Wheat;

            scrollcounter = 0;
        }

        public void LoadContent(ContentManager myContentManager)
        {
            Font = myContentManager.Load<SpriteFont>("Defaultfont");
            MsgBox.LoadContent(myContentManager, "textbox");
            MsgBoxAscii.LoadContent(myContentManager, "textboxascii");
            InventoryBackground.LoadContent(myContentManager, "inventoryback");
            InventoryBackgroundAscii.LoadContent(myContentManager, "inventorybackascii");
            InventoryHighlight.LoadContent(myContentManager, "inventoryselected");

            EnterKeySprite.LoadContent(myContentManager, "keys/enterkey");
            SpaceKeySprite.LoadContent(myContentManager, "keys/spacekey");
            ShiftKeySprite.LoadContent(myContentManager, "keys/rshiftkey");

            ScreenFadeTex.LoadContent(myContentManager, "fadepic");
            ScreenFadeTex.Color = new Color(255, 255, 255, FadeOpacity);
        }

        public void DisplayMSG(String MSG, Boolean scrolling, String name = "", Boolean down = true, Boolean first = false)
        {
            MSGBoxText = parseText(MSG);
            ShowMSG = true;
            ShownMSGBoxText = "";
            MSGBoxTextName = name;
            if (!scrolling)
                ShownMSGBoxText = MSGBoxText;
            
            //Would the box overlay the player?
            if (down)
                MSGposition = new Vector2(200, 300);
            else
                MSGposition = new Vector2(200, 100);

            MsgBox.Position = new Vector2(MSGposition.X - 10, MSGposition.Y - 15);
            MSGisdone = false;
            if (first)
            {
                MSGMoving = true;
                MSGYOffset = 15;
                MSGOpacity = 0f;
            }
        }

        public void DisplayNewItem(Item targetitem, bool Addedtoinventory)
        {
            ItemPic = targetitem.getPicture();
            ItemPic.Position = new Vector2(300, 200);
            ItemName = targetitem.Name;
            ShowItemBox = true;
            AddedItem = Addedtoinventory;
        }

        public void AddItem(String name, String picture, ContentManager myContentManager)
        {
            Item newitem = new Item(name, picture);
            newitem.LoadContent(myContentManager);

            InvList.Add(newitem);
        }

        public void FadeOut()
        {
            Console.WriteLine("Im supposed to fade out now!");
            if(!FadedOut)
            {
                Fading = true;
            }
        }

        public void FadeIn()
        {
            if (FadedOut)
                Fading = true;
        }

        public void Draw(SpriteBatch mySpriteBatch, Boolean verbmenuopen, Object interactwithobject, Boolean ascriptisrunning)
        {
            String invtext = "Open Inventory";

            if (verbmenuopen)
                invtext = "Use Item";

            if (!asciiMode)
            {
                InventoryBackground.Draw(mySpriteBatch);
            }
            else
            {
                InventoryBackgroundAscii.Position = InventoryBackground.Position;
                InventoryBackgroundAscii.Draw(mySpriteBatch);
            }

            //Inventory needs to be shown UNDER the messagebox
            if (ShowInventory)
            {
                invtext = "Close Inventory";
                
                InventoryHighlight.Position = new Vector2(70 + (110 * InventorySelected), InvPositionY + 10);
                InventoryHighlight.Draw(mySpriteBatch);

                if (InventoryActive && InvList.Count > 0 && InventorySelected < InvList.Count())
                {
                    mySpriteBatch.DrawString(Font, InvList[InventorySelected].Name, new Vector2(20 + (110 * InventorySelected), InvPositionY - 60), FontColor);
                    ShiftKeySprite.Position = new Vector2(230, InvPositionY - 95);
                    ShiftKeySprite.Draw(mySpriteBatch);
                    mySpriteBatch.DrawString(Font, "Look at", new Vector2(250, InvPositionY - 110), FontColor);

                    if (interactwithobject != null)
                    {
                        EnterKeySprite.Position = new Vector2(340, InvPositionY - 95);
                        EnterKeySprite.Draw(mySpriteBatch);
                        mySpriteBatch.DrawString(Font, "Use with " + interactwithobject.name, new Vector2(360, InvPositionY - 110), FontColor);
                    }
                }

                int a = 0;
                foreach (Item item in InvList)
                {
                    item.Draw(new Vector2(70 + (110 * a), InvPositionY + 20), mySpriteBatch);
                    a++;
                }
            }

            if (!ascriptisrunning)
            {
                SpaceKeySprite.Position = new Vector2(40, InvPositionY - 95);
                SpaceKeySprite.Draw(mySpriteBatch);
                mySpriteBatch.DrawString(Font, invtext, new Vector2(70, InvPositionY - 110), FontColor);
            }

            if (interactwithobject != null && verbmenuopen == false && ShowMSG == false && InventoryActive == false && ShowItemBox == false && ascriptisrunning == false)
            {
                EnterKeySprite.Position = new Vector2(710, 445);
                EnterKeySprite.Draw(mySpriteBatch);
                mySpriteBatch.DrawString(Font, "Interact", new Vector2(725, 435), FontColor);
                //Draw the name of the object in front of the key symbol - but 
                //do it so that the name can be as long as it wants and the text will 
                //be displayed at the same position
                mySpriteBatch.DrawString(Font, interactwithobject.name, new Vector2(695 - Font.MeasureString(interactwithobject.name).Length(), 435), FontColor);
            }

            // The Fade-Sprite over everything
            if (FadeOpacity > 0f)
            {
                ScreenFadeTex.Color = new Color(255, 255, 255);
                ScreenFadeTex.Draw(mySpriteBatch, FadeOpacity);
            }

            //Messagebox needs to be shown OVER the Inventory
            if (ShowMSG)
            {
                if (!asciiMode)
                {
                    MsgBox.Position = new Vector2(MSGposition.X + 190, MSGposition.Y + 42 + MSGYOffset);
                    MsgBox.Draw(mySpriteBatch, MSGOpacity);
                }
                else
                {
                    MsgBoxAscii.Position = new Vector2(MSGposition.X + 190, MSGposition.Y + 42);
                    MsgBoxAscii.Draw(mySpriteBatch);
                }
                
                
                if (!MSGMoving)
                    mySpriteBatch.DrawString(Font, ShownMSGBoxText, MSGposition, MSGColor);

                //Draw Key-Prompt
                if (MSGBoxText == ShownMSGBoxText)
                {
                    EnterKeySprite.Position = new Vector2(MSGposition.X + 380, MSGposition.Y + 95);
                    EnterKeySprite.Draw(mySpriteBatch);
                    mySpriteBatch.DrawString(Font, "Continue", new Vector2(MSGposition.X + 395, MSGposition.Y + 90), FontColor);
                }
            }

            // Item added/removed notification
            if (ShowItemBox)
            {
                MsgBox.Position = new Vector2(MSGposition.X + 190, 200);
                MsgBox.Draw(mySpriteBatch);
                ItemPic.Draw(mySpriteBatch);

                String notification;
                
                if(AddedItem)
                    notification = " added to the inventory!";
                else
                    notification = " removed from inventory.";

                mySpriteBatch.DrawString(Font, ItemName, new Vector2(MSGposition.X + 140, 180), FontColor);
                mySpriteBatch.DrawString(Font, notification, new Vector2(MSGposition.X + 140, 200), FontColor);
            }
        }

        private String parseText(String text)
        {
            String line = String.Empty;
            String returnString = String.Empty;
            String[] wordArray = text.Split(' ');

            foreach (String word in wordArray)
            {
                if (Font.MeasureString(line + word).Length() > MsgBox.Texture.Width - 15)
                {
                    returnString = returnString + line + '\n';
                    line = String.Empty;
                }

                line = line + word + ' ';
            }

            return returnString + line;
        }


        public bool CheckControls()
        {
            if (ShowMSG)
            {
                if (MSGBoxText.Length == ShownMSGBoxText.Length)
                {
                    ShowMSG = false;
                    MSGisdone = true;
                    return true;
                }
                else
                    ShownMSGBoxText = MSGBoxText;
            }
            if (ShowItemBox)
            {
                ShowItemBox = false;
                return true;
            }
            return false;
        }

        public Boolean IsDone()
        {
            if (!MSGisdone)
                return false;
            else
            {
                //MSGisdone = false;
                return true;
            }
        }

        public void SetOpacity(float value)
        {
            FadeOpacity = value;
            if (value < 1f)
                FadedOut = false;
            else
                FadedOut = true;
        }

        public void toggleAscii(bool on = true)
        {
            asciiMode = on;
            if (asciiMode)
            {
                FontColor = Color.Green;
                MSGColor = Color.Green;
            }
            else
            {
                FontColor = Color.Wheat;
                MSGColor = Color.White;
            }
        }

        public void Update()
        {
            if (MSGMoving)
            {
                MSGOpacity += 0.1f;
                if (MSGOpacity > 1f)
                    MSGOpacity = 1f;

                MSGYOffset -= 1;
                if (MSGYOffset == 0)
                    MSGYOffset = 0;

                if (MSGYOffset == 0 && MSGOpacity == 1f)
                    MSGMoving = false;
            }
            else
            {
                if (MSGBoxText.Length > ShownMSGBoxText.Length)
                {
                    ShownMSGBoxText += MSGBoxText[ShownMSGBoxText.Length];
                    scrollcounter = -1;
                }
                scrollcounter++;
            }


            if (ShowInventory)
            {
                if (InvPositionY > 410)
                    InvPositionY -= 5;
                if (InvPositionY <= 410)
                {
                    InvPositionY = 410;
                    InventoryActive = true;
                }
            }
            else
            {
                InventoryActive = false;
                if (InvPositionY < 543)
                    InvPositionY += 7;
                if (InvPositionY >= 543)
                    InvPositionY = 543;
            }

            if (Fading)
            {
                Console.WriteLine("HJDLKDLK");
                Console.WriteLine(FadeOpacity);
                if (FadedOut)
                {
                    if (FadeOpacity < 0f)
                    {
                        FadeOpacity = 0f;
                        FadedOut = false;
                        Fading = false;
                    }
                    else
                        FadeOpacity -= FadeStep;
                }
                else
                {
                    if (FadeOpacity > 1f)
                    {
                        FadeOpacity = 1f;
                        FadedOut = true;
                        Fading = false;
                    }
                    else
                        FadeOpacity += FadeStep;
                }
            }
            
            InventoryBackground.Position.Y = (float)InvPositionY;
        }
    }
}
