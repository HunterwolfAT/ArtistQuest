﻿using System;
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
        String MSGBoxText;
        String ShownMSGBoxText;
        String MSGBoxTextName;
        Vector2 MSGposition;
        public Boolean ShowMSG;
        Boolean MSGisdone;
        Color MSGColor;

        int scrollcounter;

        // Inventory variables
        Sprite InventoryBackground;
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

        //Fade-Control variables
        Sprite ScreenFadeTex;
        public bool FadedOut = false;
        public bool Fading = false;
        int FadeOpacity;
        int FadeStep = 5;

        // Key sprites
        Sprite EnterKeySprite;
        Sprite SpaceKeySprite;
        Sprite ShiftKeySprite;

        // Font font
        SpriteFont Font;

        public GUI()
        {
            MSGBoxText = "";
            ShownMSGBoxText = "";
            ShowMSG = false;
            MSGisdone = false;
            MSGposition = new Vector2(10f, 10f);
            MSGColor = Color.White;
            MsgBox = new Sprite();

            InventoryBackground = new Sprite();
            InventoryHighlight = new Sprite();
            InvPositionY = 543;
            InventoryBackground.Position = new Vector2(0f, (float)InvPositionY);
            ShowInventory = false;
            InvList = new List<Item>();
            InventorySelected = 0;

            ItemPic = new Sprite();

            ScreenFadeTex = new Sprite();
            FadeOpacity = 0;

            EnterKeySprite = new Sprite();
            SpaceKeySprite = new Sprite();
            ShiftKeySprite = new Sprite();

            scrollcounter = 0;
        }
        
        public GUI(SpriteFont font, List<Item> inventory)
        {
            MSGBoxText = "";
            ShownMSGBoxText = "";
            ShowMSG = false;
            MSGisdone = false;
            Font = font;
            MSGposition = new Vector2(10f, 10f);
            MSGColor = Color.White;
            MsgBox = new Sprite();

            InventoryBackground = new Sprite();
            InventoryHighlight = new Sprite();
            InvPositionY = 543;
            InventoryBackground.Position = new Vector2(0f, (float)InvPositionY);
            ShowInventory = false;
            InvList = inventory;
            InventorySelected = 0;

            ItemPic = new Sprite();

            ScreenFadeTex = new Sprite();

            EnterKeySprite = new Sprite();
            SpaceKeySprite = new Sprite();
            ShiftKeySprite = new Sprite();

            scrollcounter = 0;
        }

        public void LoadContent(ContentManager myContentManager)
        {
            Font = myContentManager.Load<SpriteFont>("Defaultfont");
            MsgBox.LoadContent(myContentManager, "textbox");
            InventoryBackground.LoadContent(myContentManager, "inventoryback");
            InventoryHighlight.LoadContent(myContentManager, "inventoryselected");

            EnterKeySprite.LoadContent(myContentManager, "keys/enterkey");
            SpaceKeySprite.LoadContent(myContentManager, "keys/spacekey");
            ShiftKeySprite.LoadContent(myContentManager, "keys/rshiftkey");

            ScreenFadeTex.LoadContent(myContentManager, "fadepic");
            ScreenFadeTex.Color = new Color(255, 255, 255, FadeOpacity);
        }

        public void DisplayMSG(String MSG, Boolean scrolling, String name = "")
        {
            MSGBoxText = parseText(MSG);
            ShowMSG = true;
            ShownMSGBoxText = "";
            MSGBoxTextName = name;
            if (!scrolling)
                ShownMSGBoxText = MSGBoxText;

            MSGposition = new Vector2(200, 300);
            MsgBox.Position = new Vector2(MSGposition.X - 10, MSGposition.Y - 15);
            MSGisdone = false;
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
            
            InventoryBackground.Draw(mySpriteBatch);
            
            //Inventory needs to be shown UNDER the messagebox
            if (ShowInventory)
            {
                invtext = "Close Inventory";
                
                InventoryHighlight.Position = new Vector2(70 + (110 * InventorySelected), InvPositionY + 10);
                InventoryHighlight.Draw(mySpriteBatch);

                if (InventoryActive && InvList.Count > 0 && InventorySelected < InvList.Count())
                {
                    mySpriteBatch.DrawString(Font, InvList[InventorySelected].Name, new Vector2(20 + (110 * InventorySelected), InvPositionY - 60), Color.AntiqueWhite);
                    ShiftKeySprite.Position = new Vector2(230, InvPositionY - 95);
                    ShiftKeySprite.Draw(mySpriteBatch);
                    mySpriteBatch.DrawString(Font, "Look at", new Vector2(250, InvPositionY - 110), Color.Wheat);

                    if (interactwithobject != null)
                    {
                        EnterKeySprite.Position = new Vector2(340, InvPositionY - 95);
                        EnterKeySprite.Draw(mySpriteBatch);
                        mySpriteBatch.DrawString(Font, "Use with " + interactwithobject.name, new Vector2(360, InvPositionY - 110), Color.Wheat);
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
                mySpriteBatch.DrawString(Font, invtext, new Vector2(70, InvPositionY - 110), Color.Wheat);
            }

            if (interactwithobject != null && verbmenuopen == false && ShowMSG == false && InventoryActive == false && ShowItemBox == false)
            {
                EnterKeySprite.Position = new Vector2(710, 445);
                EnterKeySprite.Draw(mySpriteBatch);
                mySpriteBatch.DrawString(Font, "Interact", new Vector2(725, 435), Color.Wheat);
                //Draw the name of the object in front of the key symbol - but 
                //do it so that the name can be as long as it wants and the text will 
                //be displayed at the same position
                mySpriteBatch.DrawString(Font, interactwithobject.name, new Vector2(695 - Font.MeasureString(interactwithobject.name).Length(), 435), Color.Wheat);
            }

            // The Fade-Sprite over everything
            if (FadeOpacity != 0)
            {
                ScreenFadeTex.Color = new Color(255, 255, 255, FadeOpacity);
                ScreenFadeTex.Draw(mySpriteBatch);
            }

            //Messagebox needs to be shown OVER the Inventory
            if (ShowMSG)
            {
                MsgBox.Position = new Vector2(MSGposition.X+190, MSGposition.Y+42); 
                MsgBox.Draw(mySpriteBatch);
                mySpriteBatch.DrawString(Font, ShownMSGBoxText, MSGposition, MSGColor);

                //Draw Key-Prompt
                if (MSGBoxText == ShownMSGBoxText)
                {
                    EnterKeySprite.Position = new Vector2(MSGposition.X + 380, MSGposition.Y + 95);
                    EnterKeySprite.Draw(mySpriteBatch);
                    mySpriteBatch.DrawString(Font, "Continue", new Vector2(MSGposition.X + 395, MSGposition.Y + 90), Color.Wheat);
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

                mySpriteBatch.DrawString(Font, ItemName, new Vector2(MSGposition.X + 140, 180), Color.Wheat);
                mySpriteBatch.DrawString(Font, notification, new Vector2(MSGposition.X + 140, 200), Color.Wheat);
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

        public void SetOpacity(int value)
        {
            FadeOpacity = value;
            if (value == 0)
                FadedOut = false;
            else
                FadedOut = true;
        }

        public void Update()
        {
            if (MSGBoxText.Length > ShownMSGBoxText.Length)
            {
                ShownMSGBoxText += MSGBoxText[ShownMSGBoxText.Length];
                scrollcounter = -1;
            }
            scrollcounter++;


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
                    if (FadeOpacity < 0)
                    {
                        FadeOpacity = 0;
                        FadedOut = false;
                        Fading = false;
                    }
                    else
                        FadeOpacity -= FadeStep;
                }
                else
                {
                    if (FadeOpacity > 255)
                    {
                        FadeOpacity = 255;
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
