using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Microsoft.Xna.Framework;
using System.Drawing;

namespace WindowsGame1
{
    public partial class Form1 : Form
    {
        public Game1 game;

        public String ToolState = "None";

        public int selectedWalkRect = 0;
        public int selectedObject = 0;

        public bool ShowPlayerPos = false;

        private Graphics graphics;
        private Pen greenpen;
        private String AnimationImageFilename;

        public Form1(Game1 mygame)
        {
            InitializeComponent();
            game = mygame;
            graphics = this.CreateGraphics();
            greenpen = new Pen(System.Drawing.Color.Green, 1);
        }

        public void UpdateEditor()
        {
            ToolStatusLabel.Text = "Current Tool: " + ToolState;

            roomnametextbox.Text = game.map.name;

            if (game.map.ShowInteractionHandles)
                ShowObjHandles.Checked = true;
            else
                ShowObjHandles.Checked = false;
            
            listBox1.Items.Clear();
            foreach (Microsoft.Xna.Framework.Rectangle rect in game.map.getWalkrects())
            {
                listBox1.Items.Add(rect);
            }

            GlobVarListBox.Items.Clear();
            foreach (String[] var in game.GameVariables)
            {
                GlobVarListBox.Items.Add(var[0] + ": " + var[1]);
            }

            listBox2.Items.Clear();
            objectlistbox.Items.Clear();
            foreach (Object obj in game.map.getObjects())
            {
                //if (!listBox1.Items.Contains(rect))
                listBox2.Items.Add(obj.name);
                objectlistbox.Items.Add(obj.name);
            }

            verblist.Items.Clear();
            if (listBox2.SelectedIndex != -1)
            {
                //objx.Value = game.map.getObjects()[listBox2.SelectedIndex].rect.X;
                //objy.Value = game.map.getObjects()[listBox2.SelectedIndex].rect.Y;
                objwalkable.Checked = game.map.getObjects()[listBox2.SelectedIndex].walkable;
                objvisible.Checked = game.map.getObjects()[listBox2.SelectedIndex].visible;
                objwalkable.Enabled = true;
                objvisible.Enabled = true;
                rectheight.Value = game.map.getObjects()[listBox2.SelectedIndex].rect.Height;
                rectwidth.Value = game.map.getObjects()[listBox2.SelectedIndex].rect.Width;
                rectoffsetx.Value = (int)game.map.getObjects()[listBox2.SelectedIndex].rectOffset.X;
                rectoffsety.Value = (int)game.map.getObjects()[listBox2.SelectedIndex].rectOffset.Y;

                foreach (Script script in game.map.getObjects()[listBox2.SelectedIndex].scripts)
                {
                    verblist.Items.Add(script.Name);
                }
            }
            else
            {
                groupBox2.Text = "Object";
                //objx.Value = 0;
                //objy.Value = 0;
                objwalkable.Enabled = false;
                objvisible.Enabled = false;
            }

            itemlistbox.Items.Clear();
            foreach (Item item in game.items.ReturnItemList())
            {
                itemlistbox.Items.Add(item.Name);
            }

            comitemlist.Items.Clear();
            foreach (Item item in game.items.ReturnItemList())
            {
                comitemlist.Items.Add(item.Name);
            }

            aniobjectlistbox.Items.Clear();
            foreach (Object obj in game.map.getObjects())
                aniobjectlistbox.Items.Add(obj.name);     

            verblistscript.Items.Clear();
            scriptitemscriptlistbox.Items.Clear();
            commandlistbox.Items.Clear();

            UpdateCommandlist();
            
        }

        private void AddCommand(String type, List<int> iargs, List<String> sargs, int insertindex)
        {
            if (insertindex != -1)
            {
                if (tabControl3.TabPages[tabControl3.SelectedIndex].Name == "verbstab")
                    game.map.getObjects()[objectlistbox.SelectedIndex].scripts[verblistscript.SelectedIndex].InsertCommand(commandlistbox.SelectedIndex, type, sargs, iargs, game.gui);
                if (tabControl3.TabPages[tabControl3.SelectedIndex].Name == "itemstab")
                    game.items.ReturnItem(scriptitemlistbox.SelectedIndex).scripts[scriptitemscriptlistbox.SelectedIndex].InsertCommand(commandlistbox.SelectedIndex, type, sargs, iargs, game.gui);
            }
            else
            {
                if(tabControl3.TabPages[tabControl3.SelectedIndex].Name == "verbstab")
                    game.map.getObjects()[objectlistbox.SelectedIndex].scripts[verblistscript.SelectedIndex].AddCommand(type, sargs, iargs, game.gui);
                if (tabControl3.TabPages[tabControl3.SelectedIndex].Name == "itemstab")
                    game.items.ReturnItem(scriptitemlistbox.SelectedIndex).scripts[scriptitemscriptlistbox.SelectedIndex].AddCommand(type, sargs, iargs, game.gui);
                
            }

            UpdateCommandlist();
        }

        private void UpdateCommandlist()
        {
            int ifcount = 0;
            if (tabControl3.TabPages[tabControl3.SelectedIndex].Name == "verbstab")
            {
                if (objectlistbox.SelectedIndex != -1 && verblistscript.SelectedIndex != -1)
                {
                    commandlistbox.Items.Clear();
                    foreach (Command com in game.map.getObjects()[objectlistbox.SelectedIndex].scripts[verblistscript.SelectedIndex].GetCommands())
                    {
                        if (com.Type == "END IF")
                            ifcount--;

                        String commandtext = "";

                        if (com.Type == "ELSE")
                            ifcount--;
                        
                        for (int o = 0; o < ifcount; o++)
                        {
                            commandtext += "    ";
                        }

                        if (com.Type == "ELSE")
                            ifcount++;

                         commandtext += com.Type + ": ";

                        foreach (String str in com.SArgs)
                        {
                            commandtext += str + " ";
                        }

                        foreach (int inta in com.IArgs)
                        {
                            commandtext += inta + " ";
                        }

                        commandlistbox.Items.Add(commandtext);

                        if (com.Type == "IF")
                            ifcount++;

                    }
                }
            }
            if (tabControl3.TabPages[tabControl3.SelectedIndex].Name == "itemstab")
            {
                if (scriptitemlistbox.SelectedIndex != -1 && scriptitemscriptlistbox.SelectedIndex != -1)
                {
                    commandlistbox.Items.Clear();
                    foreach (Command com in game.items.ReturnItem(scriptitemlistbox.SelectedIndex).scripts[scriptitemscriptlistbox.SelectedIndex].GetCommands())
                    {
                        if (com.Type == "END IF")
                            ifcount--;

                        String commandtext = "";

                        if (com.Type == "ELSE")
                            ifcount--;

                        for (int o = 0; o < ifcount; o++)
                        {
                            commandtext += "    ";
                        }

                        if (com.Type == "ELSE")
                            ifcount++;

                        commandtext += com.Type + ": ";

                        foreach (String str in com.SArgs)
                        {
                            commandtext += str + " ";
                        }

                        foreach (int inta in com.IArgs)
                        {
                            commandtext += inta + " ";
                        }

                        commandlistbox.Items.Add(commandtext);

                        if (com.Type == "IF")
                            ifcount++;

                    }
                }
            }
        }

        private void UpdateGrid()
        {
            if (AnimationImageFilename != "" && AnimationImageFilename != null && aniframeXTB.Text != "" && aniframeYTB.Text != "")
            {
                AnimationPB.Load(AnimationImageFilename);

                graphics = Graphics.FromImage(AnimationPB.Image);

                int width = AnimationPB.Image.Width / int.Parse(aniframeXTB.Text);
                int height = AnimationPB.Image.Height / int.Parse(aniframeYTB.Text);
                
                for (int x = 1; x <= int.Parse(aniframeXTB.Text); x++)
                    graphics.DrawLine(greenpen, width * x, 0, width * x, AnimationPB.Image.Height);

                for (int y = 1; y <= int.Parse(aniframeYTB.Text); y++)
                    graphics.DrawLine(greenpen, 0, height * y, AnimationPB.Image.Width, height * y);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //listBox1.Dispose();
            listBox1.Items.Clear();
            foreach (Microsoft.Xna.Framework.Rectangle rect in game.map.getWalkrects())
            {
                //if (!listBox1.Items.Contains(rect))
                    listBox1.Items.Add(rect);
            }
        }

        private void listBox1_DoubleClick(object sender, EventArgs e)
        {
            if (game.map.HighlightedWalkmap > game.map.getWalkrects().Count)
                game.map.HighlightedWalkmap = game.map.getWalkrects().Count - 2;
            game.map.getWalkrects().RemoveAt(listBox1.SelectedIndex);
            selectedWalkRect = listBox1.SelectedIndex - 1;
            UpdateEditor();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
                game.map.ShowWalkmap = true;
            else
                game.map.ShowWalkmap = false;
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox2.Checked)
                game.player.showRect = true;
            else
                game.player.showRect = false;
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            game.map.HighlightedWalkmap = listBox1.SelectedIndex;
            selectedWalkRect = listBox1.SelectedIndex;
            ToolState = "MoveWalkmap";
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            game.map.backgroundimage = textBox1.Text;
            game.map.LoadContent(game.Content);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (objectpic.Text == "")
                objectpic.Text = "mandl";
            Object newObject = new Object(objname.Text, objectpic.Text, new Vector2(100f,100f));
            newObject.LoadContent(game.Content);
            newObject.Init();                       // Sets up the Rectangle of the Object right
            game.map.AddObject(newObject);
            UpdateEditor();
        }

        private void listBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            groupBox2.Text = game.map.getObjects()[listBox2.SelectedIndex].name;
            //objx.Value = game.map.getObjects()[listBox2.SelectedIndex].rect.X;
            //objy.Value = game.map.getObjects()[listBox2.SelectedIndex].rect.Y;
            objrange.Value = (decimal)game.map.getObjects()[listBox2.SelectedIndex].radius;
            objwalkable.Checked = game.map.getObjects()[listBox2.SelectedIndex].walkable;
            objvisible.Checked = game.map.getObjects()[listBox2.SelectedIndex].visible;
            objwalkable.Enabled = true;
            objvisible.Enabled = true;
            rectheight.Value = game.map.getObjects()[listBox2.SelectedIndex].rect.Height;
            rectwidth.Value = game.map.getObjects()[listBox2.SelectedIndex].rect.Width;
            rectoffsetx.Value = (int)game.map.getObjects()[listBox2.SelectedIndex].rectOffset.X;
            rectoffsety.Value = (int)game.map.getObjects()[listBox2.SelectedIndex].rectOffset.Y;

            selectedObject = listBox2.SelectedIndex;
            ToolState = "MoveObject";

            verblist.Items.Clear();
            foreach (Script script in game.map.getObjects()[listBox2.SelectedIndex].scripts)
            {
                verblist.Items.Add(script.Name);
            }

            objimagelistbox.Items.Clear();
            foreach (Sprite sprite in game.map.getObjects()[listBox2.SelectedIndex].images)
            {
                objimagelistbox.Items.Add(sprite);
            }

            objanimimagelistbox.Items.Clear();
            foreach (AnimatedSprite sprite in game.map.getObjects()[listBox2.SelectedIndex].aniimages)
            {
                objanimimagelistbox.Items.Add(sprite);
            }

            //UpdateEditor();
        }

        //private void objx_ValueChanged(object sender, EventArgs e)
        //{
        //    if (listBox2.SelectedIndex != -1)
        //        game.map.getObjects()[listBox2.SelectedIndex].rect.X = (int)objx.Value;
        //}

        //private void objy_ValueChanged(object sender, EventArgs e)
        //{
        //    if (listBox2.SelectedIndex != -1)
        //        game.map.getObjects()[listBox2.SelectedIndex].rect.Y = (int)objy.Value;
        //}

        private void objdelete_Click(object sender, EventArgs e)
        {
            if (listBox2.SelectedIndex != -1)
                game.map.getObjects().RemoveAt(listBox2.SelectedIndex);
            UpdateEditor();
        }

        private void objwalkable_CheckedChanged(object sender, EventArgs e)
        {
            game.map.getObjects()[listBox2.SelectedIndex].walkable = objwalkable.Checked;
        }

        private void objvisible_CheckedChanged(object sender, EventArgs e)
        {
            game.map.getObjects()[listBox2.SelectedIndex].visible = objvisible.Checked;
        }

        private void objrange_ValueChanged(object sender, EventArgs e)
        {
            if (listBox2.SelectedIndex != -1)
                game.map.getObjects()[listBox2.SelectedIndex].radius = (float)objrange.Value;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            ToolState = "CreateWalkMap";
            UpdateEditor();
        }

        private void addverbbutton_Click(object sender, EventArgs e)
        {
            if (verbtextbox.Text != "" && listBox2.SelectedIndex != -1)
                game.map.getObjects()[listBox2.SelectedIndex].AddScript(verbtextbox.Text);

            verblist.Items.Clear();
            foreach (Script verb in game.map.getObjects()[listBox2.SelectedIndex].scripts)
            {
                verblist.Items.Add(verb.Name);
            }
            //UpdateEditor();
        }

        private void closeEditorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Hide();
        }

        private void exitGameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateEditor();
        }

        private void objectlistbox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (objectlistbox.SelectedIndex != -1)
            {
                verblistscript.Items.Clear();
                foreach (Script script in game.map.getObjects()[objectlistbox.SelectedIndex].scripts)
                {
                    verblistscript.Items.Add(script.Name);
                }
            }
        }

        private void MSG_com_Click(object sender, EventArgs e)
        {
            if (Com_SArg.Text != "")
            {
                List<String> sargs = new List<String>();
                List<int> iargs = new List<int>();
                sargs.Add(Com_SArg.Text);


                if (tabControl3.TabPages[tabControl3.SelectedIndex].Name == "verbstab" && objectlistbox.SelectedIndex != -1 && verblistscript.SelectedIndex != -1
                    || tabControl3.TabPages[tabControl3.SelectedIndex].Name == "itemstab" && scriptitemlistbox.SelectedIndex != -1 && scriptitemscriptlistbox.SelectedIndex != -1)
                    AddCommand("Message", iargs, sargs, commandlistbox.SelectedIndex);

                Com_SArg.Text = "";
                Com_SArg.Focus();
                
            }
        }

        private void saveRoomToolStripMenuItem_Click(object sender, EventArgs e)
        {
            saveFileDialog1.FileName = game.map.name;
            if(saveFileDialog1.ShowDialog() == DialogResult.OK)
                game.SaveMap(saveFileDialog1.FileName);
        }

        private void loadRoomToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if(openFileDialog1.ShowDialog() == DialogResult.OK)
                game.LoadMap(openFileDialog1.FileName);
        }

        private void roomnametextbox_TextChanged(object sender, EventArgs e)
        {
            game.map.name = roomnametextbox.Text;
        }

        private void listBox2_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (MessageBox.Show("Do you really want to delete this object?", "Delete Object", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                if (listBox2.SelectedIndex != -1)
                {
                    game.map.getObjects().RemoveAt(listBox2.SelectedIndex);
                    selectedObject--;
                }
                UpdateEditor();
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (listBox2.SelectedIndex != -1 && objimagename.Text != "")
            {
                game.map.getObjects()[listBox2.SelectedIndex].AddSprite(game.Content, objimagename.Text);

                objimagelistbox.Items.Clear();
                foreach (Sprite sprite in game.map.getObjects()[listBox2.SelectedIndex].images)
                {
                    objimagelistbox.Items.Add(sprite);
                }
            }
        }


        private void objimagelistbox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBox2.SelectedIndex != -1)
            {
                if (game.map.getObjects()[listBox2.SelectedIndex].images.Count > objimagelistbox.SelectedIndex)
                    game.map.getObjects()[listBox2.SelectedIndex].imagenum = objimagelistbox.SelectedIndex;
            }
        }

        private void tabControl2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (objectlistbox.SelectedIndex != -1)
            {
                comimglistbox.Items.Clear();
                foreach (Sprite sprite in game.map.getObjects()[objectlistbox.SelectedIndex].images)
                {
                    comimglistbox.Items.Add(sprite);
                }
                
                verbcombobox.Items.Clear();
                foreach (Script verb in game.map.getObjects()[objectlistbox.SelectedIndex].scripts)
                {
                    verbcombobox.Items.Add(verb.Name);
                }
            }

            ifglobvarlistbox.Items.Clear();
            ifitemlistbox.Items.Clear();
            asglobvarlistbox.Items.Clear();
            comglobvarlistbox.Items.Clear();
            foreach (String[] var in game.GameVariables)
            {
                ifglobvarlistbox.Items.Add(var[0] + ": " + var[1]);
                asglobvarlistbox.Items.Add(var[0] + ": " + var[1]);
                comglobvarlistbox.Items.Add(var[0] + ": " + var[1]);
            }

            //Add Player Position to the choices at Ifgobalvarlistbox
            ifglobvarlistbox.Items.Add("Player X-Position");
            ifglobvarlistbox.Items.Add("Player Y-Position");

            foreach (Item item in game.items.ReturnItemList())
            {
                ifitemlistbox.Items.Add(item.Name);
            }

            walkvisCombobox.Items.Clear();
            foreach (Object obj in game.map.getObjects())
                walkvisCombobox.Items.Add(obj.name);

            comitemlist.Items.Clear();
            foreach (Item item in game.items.ReturnItemList())
            {
                comitemlist.Items.Add(item.Name);
            }

            
        }

        private void button6_Click(object sender, EventArgs e)
        {
            if (comimglistbox.SelectedIndex != -1)
            {
                List<String> sargs = new List<String>();
                List<int> iargs = new List<int>();

                //sargs.Add("");
                sargs.Add(objectlistbox.Items[objectlistbox.SelectedIndex].ToString());
                iargs.Add(comimglistbox.SelectedIndex);

                if (tabControl3.TabPages[tabControl3.SelectedIndex].Name == "verbstab" && objectlistbox.SelectedIndex != -1 && verblistscript.SelectedIndex != -1
                    || tabControl3.TabPages[tabControl3.SelectedIndex].Name == "itemstab" && scriptitemlistbox.SelectedIndex != -1 && scriptitemscriptlistbox.SelectedIndex != -1)
                    AddCommand("ChangeSprite", iargs, sargs, commandlistbox.SelectedIndex);
            }
        }

        private void commandlistbox_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (MessageBox.Show("Do you really want to delete this command?", "Delete Command", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                if (tabControl3.TabPages[tabControl3.SelectedIndex].Name == "verbstab" && commandlistbox.SelectedIndex != -1 && objectlistbox.SelectedIndex != -1 && verblistscript.SelectedIndex != -1)
                {
                    game.map.getObjects()[objectlistbox.SelectedIndex].scripts[verblistscript.SelectedIndex].Commands.RemoveAt(commandlistbox.SelectedIndex);
                    selectedObject--;
                }
                else if (tabControl3.TabPages[tabControl3.SelectedIndex].Name == "itemstab" &&commandlistbox.SelectedIndex != -1 && scriptitemscriptlistbox.SelectedIndex != -1 && scriptitemlistbox.SelectedIndex != -1)
                {
                    game.items.ReturnItem(scriptitemlistbox.SelectedIndex).scripts[scriptitemscriptlistbox.SelectedIndex].Commands.RemoveAt(commandlistbox.SelectedIndex);
                }
                UpdateCommandlist();
            }
        }

        private void verblistscript_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (objectlistbox.SelectedIndex != -1 && verblistscript.SelectedIndex != -1)
            {
                UpdateCommandlist();
            }
        }

        private void showObjRects_CheckedChanged(object sender, EventArgs e)
        {
            game.map.ShowObjectRects = !game.map.ShowObjectRects;
        }

        private void rectwidth_ValueChanged(object sender, EventArgs e)
        {
            if (listBox2.SelectedIndex != -1)
            {
                game.map.getObjects()[listBox2.SelectedIndex].rect.Width = (int)rectwidth.Value;
            }
        }

        private void rectheight_ValueChanged(object sender, EventArgs e)
        {
            if (listBox2.SelectedIndex != -1)
            {
                game.map.getObjects()[listBox2.SelectedIndex].rect.Height = (int)rectheight.Value;
            }
        }

        private void rectoffsetx_ValueChanged(object sender, EventArgs e)
        {
            if (listBox2.SelectedIndex != -1)
            {
                game.map.getObjects()[listBox2.SelectedIndex].rectOffset.X = (float)rectoffsetx.Value;
            }
        }

        private void rectoffsety_ValueChanged(object sender, EventArgs e)
        {
            if (listBox2.SelectedIndex != -1)
            {
                game.map.getObjects()[listBox2.SelectedIndex].rectOffset.Y = (float)rectoffsety.Value;
            }
        }

        private void CreateNewGlobalVar_Click(object sender, EventArgs e)
        {
            bool makenewvariable = true;
            foreach (String[] var in game.GameVariables)
            {
                if (var[0] == globalvarname.Text)
                {
                    var[1] = globalvarvalue.Text;
                    makenewvariable = false;
                }
            }
            
            if (makenewvariable)
                game.AddGlobalVariable(globalvarname.Text, globalvarvalue.Text);

            UpdateEditor();
        }

        private void globalvarlistbox_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            game.GameVariables.RemoveAt(GlobVarListBox.SelectedIndex);
            UpdateEditor();
        }

        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void button7_Click(object sender, EventArgs e)
        {
           
            List<String> sargs = new List<String>();
            List<int> iargs = new List<int>();

            if (ifglobalvarRB.Checked && ifglobvarlistbox.SelectedIndex != -1 && islistbox.SelectedIndex != -1)
            {
                sargs.Add("Variablecheck");

                //If one of the non-global varibles have been chosen...
                if (ifglobvarlistbox.SelectedIndex >= (ifglobvarlistbox.Items.Count - 2))
                {
                    if (ifglobvarlistbox.SelectedIndex == ifglobvarlistbox.Items.Count - 1) // Y-POS of Player
                        sargs.Add("PlayerYPos");
                    if (ifglobvarlistbox.SelectedIndex == ifglobvarlistbox.Items.Count - 2) // X-POS of Player
                        sargs.Add("PlayerXPos");
                }
                else //and if a global variable has been chosen...
                {
                    sargs.Add(game.GameVariables[ifglobvarlistbox.SelectedIndex][0]);
                }

                sargs.Add(islistbox.Items[islistbox.SelectedIndex].ToString());

                if (asglobvarRB.Checked && asglobvarlistbox.SelectedIndex != -1)
                    sargs.Add(game.GameVariables[asglobvarlistbox.SelectedIndex][0]);
                else if (asuservarRB.Checked)
                    sargs.Add("\\" + asuservartextbox.Text);

                if (tabControl3.TabPages[tabControl3.SelectedIndex].Name == "verbstab" && objectlistbox.SelectedIndex != -1 && verblistscript.SelectedIndex != -1
                    || tabControl3.TabPages[tabControl3.SelectedIndex].Name == "itemstab" && scriptitemlistbox.SelectedIndex != -1 && scriptitemscriptlistbox.SelectedIndex != -1)
                    AddCommand("IF", iargs, sargs, commandlistbox.SelectedIndex);
            }

            if (ifitemRB.Checked && ifitemlistbox.SelectedIndex != -1)
            {
                sargs.Add("Itemcheck");

                sargs.Add(game.items.ReturnItem(ifitemlistbox.SelectedIndex).Name);

                if (isiteminventoryRB.Checked)
                    sargs.Add("is in inventory");

                if (isnotiteminventoryRB.Checked)
                    sargs.Add("is not in inventory");

                if (tabControl3.TabPages[tabControl3.SelectedIndex].Name == "verbstab" && objectlistbox.SelectedIndex != -1 && verblistscript.SelectedIndex != -1
                    || tabControl3.TabPages[tabControl3.SelectedIndex].Name == "itemstab" && scriptitemlistbox.SelectedIndex != -1 && scriptitemscriptlistbox.SelectedIndex != -1)
                    AddCommand("IF", iargs, sargs, commandlistbox.SelectedIndex);
            }
        }

        private void button8_Click(object sender, EventArgs e)
        {
                List<String> sargs = new List<String>();
                List<int> iargs = new List<int>();

                if (tabControl3.TabPages[tabControl3.SelectedIndex].Name == "verbstab" && objectlistbox.SelectedIndex != -1 && verblistscript.SelectedIndex != -1
                    || tabControl3.TabPages[tabControl3.SelectedIndex].Name == "itemstab" && scriptitemlistbox.SelectedIndex != -1 && scriptitemscriptlistbox.SelectedIndex != -1)
                    AddCommand("END IF", iargs, sargs, commandlistbox.SelectedIndex);
        }

        private void objimagelistbox_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (objimagelistbox.SelectedIndex != -1 && listBox2.SelectedIndex != -1)
            {
                game.map.getObjects()[listBox2.SelectedIndex].images.RemoveAt(objimagelistbox.SelectedIndex);

                if (game.map.getObjects()[listBox2.SelectedIndex].imagenum >= game.map.getObjects()[listBox2.SelectedIndex].images.Count)
                    game.map.getObjects()[listBox2.SelectedIndex].imagenum = game.map.getObjects()[listBox2.SelectedIndex].images.Count - 1;

                objimagelistbox.Items.Clear();
                foreach (Sprite sprite in game.map.getObjects()[listBox2.SelectedIndex].images)
                {
                    objimagelistbox.Items.Add(sprite);
                }
            }
        }

        private void comglobvarlistbox_SelectedIndexChanged(object sender, EventArgs e)
        {
            comglobvarname.Text = game.GameVariables[comglobvarlistbox.SelectedIndex][0];
            comglobvarvalue.Text = game.GameVariables[comglobvarlistbox.SelectedIndex][1];
        }

        private void button9_Click(object sender, EventArgs e)
        {
            List<String> sargs = new List<String>();
            List<int> iargs = new List<int>();

            sargs.Add(comglobvarname.Text);
            sargs.Add(comglobvarvalue.Text);

            if (tabControl3.TabPages[tabControl3.SelectedIndex].Name == "verbstab" && objectlistbox.SelectedIndex != -1 && verblistscript.SelectedIndex != -1
                    || tabControl3.TabPages[tabControl3.SelectedIndex].Name == "itemstab" && scriptitemlistbox.SelectedIndex != -1 && scriptitemscriptlistbox.SelectedIndex != -1)
                AddCommand("Change GlobalVariable", iargs, sargs, commandlistbox.SelectedIndex);
        }

        private void globalvarlistbox_SelectedIndexChanged(object sender, EventArgs e)
        {
            globalvarname.Text = game.GameVariables[GlobVarListBox.SelectedIndex][0];
            globalvarvalue.Text = game.GameVariables[GlobVarListBox.SelectedIndex][1];
        }

        private void button10_Click(object sender, EventArgs e)
        {
            if (walkvisCombobox.SelectedIndex != -1)
            {
                List<String> sargs = new List<String>();
                List<int> iargs = new List<int>();

                sargs.Add(walkvisCombobox.Items[walkvisCombobox.SelectedIndex].ToString());

                if (walkonRB.Checked)
                    sargs.Add("ON");
                else
                    sargs.Add("OFF");

                if (tabControl3.TabPages[tabControl3.SelectedIndex].Name == "verbstab" && objectlistbox.SelectedIndex != -1 && verblistscript.SelectedIndex != -1
                    || tabControl3.TabPages[tabControl3.SelectedIndex].Name == "itemstab" && scriptitemlistbox.SelectedIndex != -1 && scriptitemscriptlistbox.SelectedIndex != -1)
                    AddCommand("Change Walkable", iargs, sargs, commandlistbox.SelectedIndex);
            }
        }

        private void setvisibility_Click(object sender, EventArgs e)
        {
            if (walkvisCombobox.SelectedIndex != -1)
            {
                List<String> sargs = new List<String>();
                List<int> iargs = new List<int>();

                sargs.Add(walkvisCombobox.Items[walkvisCombobox.SelectedIndex].ToString());
                
                if (visonRB.Checked)
                    sargs.Add("ON");
                else
                    sargs.Add("OFF");

                if (tabControl3.TabPages[tabControl3.SelectedIndex].Name == "verbstab" && objectlistbox.SelectedIndex != -1 && verblistscript.SelectedIndex != -1
                    || tabControl3.TabPages[tabControl3.SelectedIndex].Name == "itemstab" && scriptitemlistbox.SelectedIndex != -1 && scriptitemscriptlistbox.SelectedIndex != -1)
                    AddCommand("Change Visible", iargs, sargs, commandlistbox.SelectedIndex);
            }
        }

        private void tabControl3_SelectedIndexChanged(object sender, EventArgs e)
        {
            scriptitemlistbox.Items.Clear();
            foreach (Item item in game.items.ReturnItemList())
            {
                scriptitemlistbox.Items.Add(item.Name);
            }
        }

        private void makeitembutton_Click(object sender, EventArgs e)
        {
            if (itemnametextbox.Text != "" && itempicturetextbox.Text != "")
            {
                game.items.AddItem(itemnametextbox.Text, itempicturetextbox.Text);
                UpdateEditor();
            }
        }

        private void scriptitemlistbox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (scriptitemlistbox.Items.Count > 0)
            {
                scriptitemscriptlistbox.Items.Clear();
                foreach (Script script in game.items.ReturnItem(scriptitemlistbox.SelectedIndex).scripts)
                {
                    scriptitemscriptlistbox.Items.Add(script.Name);
                }
            }
        }

        private void button10_Click_1(object sender, EventArgs e)
        {
            if (objectlistbox.SelectedIndex != -1 && scriptitemlistbox.SelectedIndex != -1)
            {
                game.items.ReturnItem(scriptitemlistbox.SelectedIndex).scripts.Add(new Script(game.map.getObjects()[objectlistbox.SelectedIndex].name));

                scriptitemscriptlistbox.Items.Clear();
                foreach (Script script in game.items.ReturnItem(scriptitemlistbox.SelectedIndex).scripts)
                {
                    scriptitemscriptlistbox.Items.Add(script.Name);
                }
            }
        }

        private void scriptitemscriptlistbox_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateCommandlist();
        }

        private void itemaddinventorybutton_Click(object sender, EventArgs e)
        {
            if (itemlistbox.SelectedIndex != -1)
            {
                //game.player.InvList.Add(game.items.ReturnItem(itemlistbox.SelectedIndex));
                game.player.AddItem(game.items.ReturnItem(itemlistbox.SelectedIndex), game.Content);
            }
        }

        private void itemremoveinventorybutton_Click(object sender, EventArgs e)
        {
            if (itemlistbox.SelectedIndex != -1)
            {
                game.player.InvList.Remove(game.items.ReturnItem(itemlistbox.SelectedIndex));
            }
        }

        private void itemlistbox_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (itemlistbox.SelectedIndex != -1)
            {
                game.items.items.RemoveAt(itemlistbox.SelectedIndex);
                UpdateEditor();
            }
        }

        private void comaddinventorybutton_Click(object sender, EventArgs e)
        {
            if (comitemlist.SelectedIndex != -1)
            {
                List<String> sargs = new List<String>();
                List<int> iargs = new List<int>();

                sargs.Add(comitemlist.Items[comitemlist.SelectedIndex].ToString());

                if (tabControl3.TabPages[tabControl3.SelectedIndex].Name == "verbstab" && objectlistbox.SelectedIndex != -1 && verblistscript.SelectedIndex != -1
                    || tabControl3.TabPages[tabControl3.SelectedIndex].Name == "itemstab" && scriptitemlistbox.SelectedIndex != -1 && scriptitemscriptlistbox.SelectedIndex != -1)
                    AddCommand("Add Item", iargs, sargs, commandlistbox.SelectedIndex);
            }
        }

        private void comremoveintentorybutton_Click(object sender, EventArgs e)
        {
            if (comitemlist.SelectedIndex != -1)
            {
                List<String> sargs = new List<String>();
                List<int> iargs = new List<int>();

                sargs.Add(comitemlist.Items[comitemlist.SelectedIndex].ToString());

                if (tabControl3.TabPages[tabControl3.SelectedIndex].Name == "verbstab" && objectlistbox.SelectedIndex != -1 && verblistscript.SelectedIndex != -1
                    || tabControl3.TabPages[tabControl3.SelectedIndex].Name == "itemstab" && scriptitemlistbox.SelectedIndex != -1 && scriptitemscriptlistbox.SelectedIndex != -1)
                    AddCommand("Remove Item", iargs, sargs, commandlistbox.SelectedIndex);
            }
        }

        private void scriptitemscriptlistbox_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (scriptitemscriptlistbox.SelectedIndex != -1 && scriptitemlistbox.SelectedIndex != -1)
            {
                game.items.ReturnItem(scriptitemlistbox.SelectedIndex).scripts.RemoveAt(scriptitemscriptlistbox.SelectedIndex);
                scriptitemscriptlistbox.Items.Clear();
                foreach (Item item in game.items.ReturnItemList())
                {
                    scriptitemscriptlistbox.Items.Add(item.Name);
                }
            }

        }

        private void button12_Click(object sender, EventArgs e)
        {
            List<String> sargs = new List<String>();
            List<int> iargs = new List<int>();

            if (tabControl3.TabPages[tabControl3.SelectedIndex].Name == "verbstab" && objectlistbox.SelectedIndex != -1 && verblistscript.SelectedIndex != -1
                || tabControl3.TabPages[tabControl3.SelectedIndex].Name == "itemstab" && scriptitemlistbox.SelectedIndex != -1 && scriptitemscriptlistbox.SelectedIndex != -1)
                AddCommand("END SCRIPT", iargs, sargs, commandlistbox.SelectedIndex);
        }

        private void button11_Click(object sender, EventArgs e)
        {
            List<String> sargs = new List<String>();
            List<int> iargs = new List<int>();


            if (tabControl3.TabPages[tabControl3.SelectedIndex].Name == "verbstab" && objectlistbox.SelectedIndex != -1 && verblistscript.SelectedIndex != -1
                || tabControl3.TabPages[tabControl3.SelectedIndex].Name == "itemstab" && scriptitemlistbox.SelectedIndex != -1 && scriptitemscriptlistbox.SelectedIndex != -1)
                AddCommand("ELSE", iargs, sargs, commandlistbox.SelectedIndex);
        }

        private void comremoveselectionbutton_Click(object sender, EventArgs e)
        {
            commandlistbox.SelectedIndex = -1;
        }

        private void comupbutton_Click(object sender, EventArgs e)
        {
            Command combuffer = new Command();

            if (commandlistbox.SelectedIndex != 0)
            {
                if (tabControl3.TabPages[tabControl3.SelectedIndex].Name == "verbstab" && objectlistbox.SelectedIndex != -1 && verblistscript.SelectedIndex != -1 && commandlistbox.SelectedIndex != -1)
                {
                    combuffer = game.map.getObjects()[objectlistbox.SelectedIndex].scripts[verblistscript.SelectedIndex].Commands[commandlistbox.SelectedIndex];
                    game.map.getObjects()[objectlistbox.SelectedIndex].scripts[verblistscript.SelectedIndex].Commands.RemoveAt(commandlistbox.SelectedIndex);
                    game.map.getObjects()[objectlistbox.SelectedIndex].scripts[verblistscript.SelectedIndex].Commands.Insert(commandlistbox.SelectedIndex - 1, combuffer);
                }
                if (tabControl3.TabPages[tabControl3.SelectedIndex].Name == "itemstab" && scriptitemlistbox.SelectedIndex != -1 && scriptitemscriptlistbox.SelectedIndex != -1 && commandlistbox.SelectedIndex != -1)
                {
                    combuffer = game.items.ReturnItemList()[scriptitemlistbox.SelectedIndex].scripts[scriptitemscriptlistbox.SelectedIndex].Commands[commandlistbox.SelectedIndex];
                    game.items.ReturnItemList()[scriptitemlistbox.SelectedIndex].scripts[scriptitemscriptlistbox.SelectedIndex].Commands.RemoveAt(commandlistbox.SelectedIndex);
                    game.items.ReturnItemList()[scriptitemlistbox.SelectedIndex].scripts[scriptitemscriptlistbox.SelectedIndex].Commands.Insert(commandlistbox.SelectedIndex - 1, combuffer);
                }
                UpdateCommandlist();
            }
        }

        private void comdownbutton_Click(object sender, EventArgs e)
        {
            Command combuffer = new Command();

            if (commandlistbox.SelectedIndex != commandlistbox.Items.Count - 1)
            {
                if (tabControl3.TabPages[tabControl3.SelectedIndex].Name == "verbstab" && objectlistbox.SelectedIndex != -1 && verblistscript.SelectedIndex != -1 && commandlistbox.SelectedIndex != -1)
                {
                    combuffer = game.map.getObjects()[objectlistbox.SelectedIndex].scripts[verblistscript.SelectedIndex].Commands[commandlistbox.SelectedIndex];
                    game.map.getObjects()[objectlistbox.SelectedIndex].scripts[verblistscript.SelectedIndex].Commands.RemoveAt(commandlistbox.SelectedIndex);
                    game.map.getObjects()[objectlistbox.SelectedIndex].scripts[verblistscript.SelectedIndex].Commands.Insert(commandlistbox.SelectedIndex + 1, combuffer);
                }
                if (tabControl3.TabPages[tabControl3.SelectedIndex].Name == "itemstab" && scriptitemlistbox.SelectedIndex != -1 && scriptitemscriptlistbox.SelectedIndex != -1 && commandlistbox.SelectedIndex != -1)
                {
                    combuffer = game.items.ReturnItemList()[scriptitemlistbox.SelectedIndex].scripts[scriptitemscriptlistbox.SelectedIndex].Commands[commandlistbox.SelectedIndex];
                    game.items.ReturnItemList()[scriptitemlistbox.SelectedIndex].scripts[scriptitemscriptlistbox.SelectedIndex].Commands.RemoveAt(commandlistbox.SelectedIndex);
                    game.items.ReturnItemList()[scriptitemlistbox.SelectedIndex].scripts[scriptitemscriptlistbox.SelectedIndex].Commands.Insert(commandlistbox.SelectedIndex + 1, combuffer);
                }
                UpdateCommandlist();
            }
        }

        private void saveRoomToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            //String Path = 
            //game.SaveMap();
        }

        private void verblist_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (verblist.SelectedIndex != -1 && listBox2.SelectedIndex != -1)
            {
                if (game.map.getObjects()[listBox2.SelectedIndex].scripts[verblist.SelectedIndex].Active)
                    VerbisactiveCheckbox.Checked = true;
                else
                    VerbisactiveCheckbox.Checked = false;
            }
        }

        private void VerbisactiveCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            if (verblist.SelectedIndex != -1 && listBox2.SelectedIndex != -1)
            {
                if (VerbisactiveCheckbox.Checked == true)
                    game.map.getObjects()[listBox2.SelectedIndex].scripts[verblist.SelectedIndex].Active = true;
                else
                    game.map.getObjects()[listBox2.SelectedIndex].scripts[verblist.SelectedIndex].Active = false;
            }
        }

        private void setverbactivebutton_Click(object sender, EventArgs e)
        {
            if (verbcombobox.SelectedIndex != -1 && walkvisCombobox.SelectedIndex != -1)
            {
                List<String> sargs = new List<String>();
                List<int> iargs = new List<int>();

                sargs.Add(game.map.getObjects()[walkvisCombobox.SelectedIndex].name);
                
                sargs.Add(game.map.getObjects()[walkvisCombobox.SelectedIndex].scripts[verbcombobox.SelectedIndex].Name);

                if (verbactiveonRB.Checked == true)
                    sargs.Add("ON");
                if (verbactiveoffRB.Checked == true)
                    sargs.Add("OFF");

                if (tabControl3.TabPages[tabControl3.SelectedIndex].Name == "verbstab" && objectlistbox.SelectedIndex != -1 && verblistscript.SelectedIndex != -1
                    || tabControl3.TabPages[tabControl3.SelectedIndex].Name == "itemstab" && scriptitemlistbox.SelectedIndex != -1 && scriptitemscriptlistbox.SelectedIndex != -1)
                    AddCommand("Change verb", iargs, sargs, commandlistbox.SelectedIndex);
            }
        }

        private void walkvisCombobox_SelectedIndexChanged(object sender, EventArgs e)
        {
                verbcombobox.Items.Clear();
                verbcombobox.Text = "";
                verbcombobox.SelectedIndex = -1;
                foreach (Script verb in game.map.getObjects()[walkvisCombobox.SelectedIndex].scripts)
                {
                    verbcombobox.Items.Add(verb.Name);
                }

                comanilistbox.Items.Clear();
                comanilistbox.Text = "";
                comanilistbox.SelectedIndex = -1;
                if (game.map.getObjects()[walkvisCombobox.SelectedIndex].aniimages.Count > 0)
                {
                    foreach (AnimatedSprite sprite in game.map.getObjects()[walkvisCombobox.SelectedIndex].aniimages)
                    {
                        comanilistbox.Items.Add(sprite.name);
                    }
                }
        }

        private void button14_Click(object sender, EventArgs e)
        {
            List<String> sargs = new List<String>();
            List<int> iargs = new List<int>();

            sargs.Add("OUT");

            if (tabControl3.TabPages[tabControl3.SelectedIndex].Name == "verbstab" && objectlistbox.SelectedIndex != -1 && verblistscript.SelectedIndex != -1
                || tabControl3.TabPages[tabControl3.SelectedIndex].Name == "itemstab" && scriptitemlistbox.SelectedIndex != -1 && scriptitemscriptlistbox.SelectedIndex != -1)
                AddCommand("Screenfade", iargs, sargs, commandlistbox.SelectedIndex);
        }

        private void button15_Click(object sender, EventArgs e)
        {
            List<String> sargs = new List<String>();
            List<int> iargs = new List<int>();

            sargs.Add("IN");

            if (tabControl3.TabPages[tabControl3.SelectedIndex].Name == "verbstab" && objectlistbox.SelectedIndex != -1 && verblistscript.SelectedIndex != -1
                || tabControl3.TabPages[tabControl3.SelectedIndex].Name == "itemstab" && scriptitemlistbox.SelectedIndex != -1 && scriptitemscriptlistbox.SelectedIndex != -1)
                AddCommand("Screenfade", iargs, sargs, commandlistbox.SelectedIndex);
        }

        private void numericUpDown2_ValueChanged(object sender, EventArgs e)
        {
            if (listBox2.SelectedIndex != -1)
            {
                game.map.getObjects()[listBox2.SelectedIndex].interactOffset.X = (float)interactoffsetX.Value;
            }
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            if (listBox2.SelectedIndex != -1)
            {
                game.map.getObjects()[listBox2.SelectedIndex].interactOffset.Y = (float)interactoffsetY.Value;
            }
        }

        private void ShowObjHandles_CheckedChanged(object sender, EventArgs e)
        {
            if (ShowObjHandles.Checked)
                game.map.ShowInteractionHandles = true;
            else
                game.map.ShowInteractionHandles = false;
        }

        private void button16_Click(object sender, EventArgs e)
        {
            if (telname.Text != "" && telposx.Text != "" && telposy.Text != "")
            {
                List<String> sargs = new List<String>();
                List<int> iargs = new List<int>();

                sargs.Add(telname.Text);
                iargs.Add(int.Parse(telposx.Text));
                iargs.Add(int.Parse(telposy.Text));

                if (tabControl3.TabPages[tabControl3.SelectedIndex].Name == "verbstab" && objectlistbox.SelectedIndex != -1 && verblistscript.SelectedIndex != -1
                    || tabControl3.TabPages[tabControl3.SelectedIndex].Name == "itemstab" && scriptitemlistbox.SelectedIndex != -1 && scriptitemscriptlistbox.SelectedIndex != -1)
                    AddCommand("Teleport", iargs, sargs, commandlistbox.SelectedIndex);
            }
        }

        private void newRoomToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult result1 = MessageBox.Show("Do you really want to delete all this beautiful work?", "Make a new room.", MessageBoxButtons.YesNo);

            if (result1 == DialogResult.Yes)
            {
                game.map.Objects.Clear();
                game.map.Walkrects.Clear();
                game.map.name = "New Map";
            }
        }

        private void LoadPictureButton_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                AnimationImageFilename = openFileDialog1.FileName;

                AnimationPB.Load(AnimationImageFilename);
                UpdateGrid();
            }
        }

        private void button17_Click(object sender, EventArgs e)
        {
            UpdateGrid();
        }

        private void aniobjectlistbox_SelectedIndexChanged(object sender, EventArgs e)
        {
            animationlistbox.Items.Clear();
            if (game.map.getObjects()[aniobjectlistbox.SelectedIndex].aniimages.Count > 0)
            {
                foreach (AnimatedSprite sprite in game.map.getObjects()[aniobjectlistbox.SelectedIndex].aniimages)
                    animationlistbox.Items.Add(sprite.name);
            }
        }

        private void addanimationButton_Click(object sender, EventArgs e)
        {
            if (aniobjectlistbox.SelectedIndex != -1 && AniPictureNameTB.Text != "" && aniframeXTB.Text != "" && aniframeYTB.Text != "" && aninameTB.Text != "")
            {
                game.map.getObjects()[aniobjectlistbox.SelectedIndex].AddAniSprite(game.Content, AniPictureNameTB.Text, int.Parse(aniframeXTB.Text), int.Parse(aniframeYTB.Text), aninameTB.Text, (int)anispeedBox.Value);
            }
            UpdateEditor();
        }

        private void animationlistbox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (aniobjectlistbox.SelectedIndex != -1 && animationlistbox.SelectedIndex != -1)
            {
                aninameTB.Text = game.map.getObjects()[aniobjectlistbox.SelectedIndex].aniimages[animationlistbox.SelectedIndex].name;
                aniframeXTB.Text = game.map.getObjects()[aniobjectlistbox.SelectedIndex].aniimages[animationlistbox.SelectedIndex].Columns.ToString();
                aniframeYTB.Text = game.map.getObjects()[aniobjectlistbox.SelectedIndex].aniimages[animationlistbox.SelectedIndex].Rows.ToString();
                anispeedBox.Value = game.map.getObjects()[aniobjectlistbox.SelectedIndex].aniimages[animationlistbox.SelectedIndex].GetAnimationSpeed();
            }
        }

        private void animationlistbox_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (aniobjectlistbox.SelectedIndex != -1 && animationlistbox.SelectedIndex != -1)
            {
                if (game.map.getObjects()[aniobjectlistbox.SelectedIndex].aniimagenum == animationlistbox.SelectedIndex)
                {
                    MessageBox.Show("Are you out of your mind?! You can't delete an animation that is currently running!", "ILLEGAL OPERATION, MAN!");
                }
                else
                {
                    DialogResult result1 = MessageBox.Show("Do you really want to delete this animation?", "Delete Animation", MessageBoxButtons.YesNo);

                    if (result1 == DialogResult.Yes)
                    {
                        game.map.getObjects()[aniobjectlistbox.SelectedIndex].aniimages.RemoveAt(animationlistbox.SelectedIndex);

                        animationlistbox.Items.Clear();
                        if (game.map.getObjects()[aniobjectlistbox.SelectedIndex].aniimages.Count > 0)
                        {
                            foreach (AnimatedSprite sprite in game.map.getObjects()[aniobjectlistbox.SelectedIndex].aniimages)
                                animationlistbox.Items.Add(sprite.name);
                        }
                    }
                }
            }
        }

        private void anipreviewButton_Click(object sender, EventArgs e)
        {
            if (aniobjectlistbox.SelectedIndex != -1 && animationlistbox.SelectedIndex != -1)
            {
                game.map.getObjects()[aniobjectlistbox.SelectedIndex].aniimagenum = animationlistbox.SelectedIndex;
            }
        }

        private void button18_Click(object sender, EventArgs e)
        {
            if (aniobjectlistbox.SelectedIndex != -1 && animationlistbox.SelectedIndex != -1)
            {
                game.map.getObjects()[aniobjectlistbox.SelectedIndex].aniimagenum = -1;
            }
        }

        private void complayaniButton_Click(object sender, EventArgs e)
        {
            if (walkvisCombobox.SelectedIndex != -1 && comanilistbox.SelectedIndex != -1)
            {
                List<String> sargs = new List<String>();
                List<int> iargs = new List<int>();

                sargs.Add(walkvisCombobox.Text);
                sargs.Add(comanilistbox.Text);
                sargs.Add("once");

                if (tabControl3.TabPages[tabControl3.SelectedIndex].Name == "verbstab" && objectlistbox.SelectedIndex != -1 && verblistscript.SelectedIndex != -1
                    || tabControl3.TabPages[tabControl3.SelectedIndex].Name == "itemstab" && scriptitemlistbox.SelectedIndex != -1 && scriptitemscriptlistbox.SelectedIndex != -1)
                    AddCommand("Play Animation", iargs, sargs, commandlistbox.SelectedIndex);
            }
        }

        private void comstartaniButton_Click(object sender, EventArgs e)
        {
            if (walkvisCombobox.SelectedIndex != -1 && comanilistbox.SelectedIndex != -1)
            {
                List<String> sargs = new List<String>();
                List<int> iargs = new List<int>();

                sargs.Add(walkvisCombobox.Text);
                sargs.Add(comanilistbox.Text);
                sargs.Add("start");

                if (tabControl3.TabPages[tabControl3.SelectedIndex].Name == "verbstab" && objectlistbox.SelectedIndex != -1 && verblistscript.SelectedIndex != -1
                    || tabControl3.TabPages[tabControl3.SelectedIndex].Name == "itemstab" && scriptitemlistbox.SelectedIndex != -1 && scriptitemscriptlistbox.SelectedIndex != -1)
                    AddCommand("Play Animation", iargs, sargs, commandlistbox.SelectedIndex);
            }
        }

        private void comstopaniButton_Click(object sender, EventArgs e)
        {
            if (walkvisCombobox.SelectedIndex != -1 && comanilistbox.SelectedIndex != -1)
            {
                List<String> sargs = new List<String>();
                List<int> iargs = new List<int>();

                sargs.Add(walkvisCombobox.Text);
                sargs.Add(comanilistbox.Text);
                sargs.Add("stop");

                if (tabControl3.TabPages[tabControl3.SelectedIndex].Name == "verbstab" && objectlistbox.SelectedIndex != -1 && verblistscript.SelectedIndex != -1
                    || tabControl3.TabPages[tabControl3.SelectedIndex].Name == "itemstab" && scriptitemlistbox.SelectedIndex != -1 && scriptitemscriptlistbox.SelectedIndex != -1)
                    AddCommand("Play Animation", iargs, sargs, commandlistbox.SelectedIndex);
            }
        }

        private void comwaitButton_Click(object sender, EventArgs e)
        {
            List<String> sargs = new List<String>();
            List<int> iargs = new List<int>();

            iargs.Add((int)comwaitsecondsBox.Value);

            if (tabControl3.TabPages[tabControl3.SelectedIndex].Name == "verbstab" && objectlistbox.SelectedIndex != -1 && verblistscript.SelectedIndex != -1
                || tabControl3.TabPages[tabControl3.SelectedIndex].Name == "itemstab" && scriptitemlistbox.SelectedIndex != -1 && scriptitemscriptlistbox.SelectedIndex != -1)
                AddCommand("Wait", iargs, sargs, commandlistbox.SelectedIndex);
        }

        private void button21_Click(object sender, EventArgs e)
        {
            List<String> sargs = new List<String>();
            List<int> iargs = new List<int>();

            if (tabControl3.TabPages[tabControl3.SelectedIndex].Name == "verbstab" && objectlistbox.SelectedIndex != -1 && verblistscript.SelectedIndex != -1
                || tabControl3.TabPages[tabControl3.SelectedIndex].Name == "itemstab" && scriptitemlistbox.SelectedIndex != -1 && scriptitemscriptlistbox.SelectedIndex != -1)
                AddCommand("Turn Player Invisible", iargs, sargs, commandlistbox.SelectedIndex);
        }

        private void button22_Click(object sender, EventArgs e)
        {
            List<String> sargs = new List<String>();
            List<int> iargs = new List<int>();

            if (tabControl3.TabPages[tabControl3.SelectedIndex].Name == "verbstab" && objectlistbox.SelectedIndex != -1 && verblistscript.SelectedIndex != -1
                || tabControl3.TabPages[tabControl3.SelectedIndex].Name == "itemstab" && scriptitemlistbox.SelectedIndex != -1 && scriptitemscriptlistbox.SelectedIndex != -1)
                AddCommand("Turn Player Visible", iargs, sargs, commandlistbox.SelectedIndex);
        }

        private void comobjmoveButton_Click(object sender, EventArgs e)
        {
            if (walkvisCombobox.SelectedIndex != -1)
            {
                List<String> sargs = new List<String>();
                List<int> iargs = new List<int>();

                sargs.Add(walkvisCombobox.Text);
                if (comobjmovewaitCB.Checked)
                    sargs.Add("wait");
                else
                    sargs.Add("dont wait");

                iargs.Add((int)comobjmoveXBox.Value);
                iargs.Add((int)comobjmoveYBox.Value);
                iargs.Add((int)comobjmoveSpeedBox.Value);

                if (tabControl3.TabPages[tabControl3.SelectedIndex].Name == "verbstab" && objectlistbox.SelectedIndex != -1 && verblistscript.SelectedIndex != -1
                    || tabControl3.TabPages[tabControl3.SelectedIndex].Name == "itemstab" && scriptitemlistbox.SelectedIndex != -1 && scriptitemscriptlistbox.SelectedIndex != -1)
                    AddCommand("Move Object", iargs, sargs, commandlistbox.SelectedIndex);
            }
        }

        private void button19_Click(object sender, EventArgs e)
        {
            if (walkvisCombobox.SelectedIndex != -1)
            {
                List<String> sargs = new List<String>();
                List<int> iargs = new List<int>();

                sargs.Add(walkvisCombobox.Text);
                sargs.Add("in");
                if (comobjmovewaitCB.Checked)
                    sargs.Add("wait");
                else
                    sargs.Add("dont wait");

                iargs.Add((int)comobjmoveSpeedBox.Value);

                if (tabControl3.TabPages[tabControl3.SelectedIndex].Name == "verbstab" && objectlistbox.SelectedIndex != -1 && verblistscript.SelectedIndex != -1
                    || tabControl3.TabPages[tabControl3.SelectedIndex].Name == "itemstab" && scriptitemlistbox.SelectedIndex != -1 && scriptitemscriptlistbox.SelectedIndex != -1)
                    AddCommand("Fade Object", iargs, sargs, commandlistbox.SelectedIndex);
            }
        }

        private void button20_Click(object sender, EventArgs e)
        {
            if (walkvisCombobox.SelectedIndex != -1)
            {
                List<String> sargs = new List<String>();
                List<int> iargs = new List<int>();

                sargs.Add(walkvisCombobox.Text);
                sargs.Add("out");
                if (comobjmovewaitCB.Checked)
                    sargs.Add("wait");
                else
                    sargs.Add("dont wait");

                iargs.Add((int)comobjmoveSpeedBox.Value);

                if (tabControl3.TabPages[tabControl3.SelectedIndex].Name == "verbstab" && objectlistbox.SelectedIndex != -1 && verblistscript.SelectedIndex != -1
                    || tabControl3.TabPages[tabControl3.SelectedIndex].Name == "itemstab" && scriptitemlistbox.SelectedIndex != -1 && scriptitemscriptlistbox.SelectedIndex != -1)
                    AddCommand("Fade Object", iargs, sargs, commandlistbox.SelectedIndex);
            }
        }

        private void ShowPlPosCB_CheckedChanged(object sender, EventArgs e)
        {
            ShowPlayerPos = ShowPlPosCB.Checked;
        }

    }
}
