using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Storage;
using System.IO;

namespace WindowsGame1
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        //************INSTANCE ALL VARIABLES**************
        //SUPER SECRET ONLY ONCE IN A LIFETIME ADJUSTING VARIABLES
        private String Projectname = "ArtistQuestProt";
        private String FirstRoom = "ProtRoom";
        private String MenuSong = "ColdFunk";
        private Boolean Debug = true;
        private Boolean ShowTitle = true;
        
        //System Variables
        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;
        private Boolean GameStartup = true;
        private TextInput textinput;
        private Vector2 baseScreenSize = new Vector2(800, 480);
        private Matrix SpriteScale;
        

        //Game critical Objects
        public Project proj;
        public Sound sound;
        public Maps map;
        public Player player;
        public Itemhandler items;
        private Form1 Editor;           // The Form for the Editor

        //Game Variables
        private Title title;
        public List<String[]> GameVariables;
        public Savegame save;

        //Variables for Controls
        float thumbx, thumby;
        bool EditorToggleHold = false;
        private MouseState oldState;
        public KeyboardState KoldState;

        //Variables for GUI
        public GUI gui;
        private SpriteFont font;

        //ScriptHandler
        private ScriptHandler scripthandler;

        //Variables for the Editor
        private Boolean EditorON = false;
        private Rectangle NewWalkMap;
        private Boolean SelectWalkMap = false;
        private Point E_ClickedOn = new Point(-1,-1);
        public List<Snapshot> Snapshots;

        //Constructor
        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            // Define resolution of the game
            graphics.PreferredBackBufferWidth = 800;
            graphics.PreferredBackBufferHeight = 480;
            //graphics.ToggleFullScreen();

            //this.Window.AllowUserResizing = true;
            this.Window.Title = "Artist Quest Alpha v0.8";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here    
            title = new Title();

            textinput = new TextInput();

            if (!ShowTitle)
            {
                InitializeContent();
                title.Startup = false;
                GameStartup = false;
            }

            base.Initialize();
        }

        private void InitializeContent()
        {
            map = new Maps();

            if (FirstRoom != "")
            {
                string path = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase);
                path = path.Substring(6, path.Length - 6);      // Getting rid of the file:\ it puts in front of the path with the above line
                string relPath = System.IO.Path.Combine(path + "\\saves\\", FirstRoom);
                LoadMap(relPath, true);
            }
            else
            {
                //Test-Walkmaps
                map.AddWalkRect(new Rectangle(174, 101, 271, 183));
                map.AddWalkRect(new Rectangle(174, 258, 71, 149));
            }

            player = new Player();
            items = new Itemhandler();

            GameVariables = new List<String[]>();

            proj = new Project(Projectname, GameVariables, items.ReturnItemList());

            save = new Savegame(proj.GetGlobalVariables(), player.InvList, map);

            LoadProjectfile();

            Editor = new Form1(this);
            NewWalkMap = new Rectangle(0, 0, 0, 0);

            Snapshots = new List<Snapshot>();
        }

    

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            
            font = Content.Load<SpriteFont>("Defaultfont");

            sound = new Sound(GetGamePath(), this.Content);

            //Loads the title (and loading-) screen
            title.LoadContent(this.Content, MenuSong);

            sound.PlayMusic(title.titlesong);

            if (!ShowTitle)
                LoadContentContent();

            float Screenscalex = graphics.GraphicsDevice.Viewport.Width / baseScreenSize.X;
            float Screenscaley = graphics.GraphicsDevice.Viewport.Height / baseScreenSize.Y;

            SpriteScale = Matrix.CreateScale(Screenscalex, Screenscaley, 1);
        }

        /// <summary>
        /// LoadContentContent loads everything else that isnt the title screen
        /// </summary>
        private void LoadContentContent()
        {

            // TODO: use this.Content to load your game content here
            map.LoadContent(this.Content);
            player.LoadContent(this.Content, font);
            gui = new GUI(font, player.InvList, proj);
            gui.LoadContent(this.Content);

            scripthandler = new ScriptHandler(map, player, items, this);
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            //if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            //    this.Exit();

            if (ShowTitle)
            {
                if (title.Update(gameTime) == -1)
                    CheckControls();
                else
                {
                    if (title.Mode == "Menu")
                    {
                        switch (title.Update(gameTime))
                        {

                            case 0:
                                ShowTitle = false;
                                if (title.Startup)      // Starting the game from the beginning
                                {
                                    
                                    Script introscript = map.PlayIntro();
                                    if (introscript != null && !Debug)
                                    {
                                        gui.SetOpacity(1f);
                                        scripthandler.RunScript(introscript);
                                    }

                                    // If the map has a background song, play it now!
                                    if (map.backgroundmusic != null)
                                    {
                                        //sound.LoadMusic(map.backgroundmusic, Content);    //It loads all the music at the beginning of the game anyway!
                                        sound.PlayMusic(map.backgroundmusic);
                                    }
                                    else
                                        sound.StopMusic();

                                    title.Startup = false;
                                }

                                if (title.lastsong != null)
                                    sound.PlayMusic(title.lastsong.Name);
                               
                                break;
                            case 1:
                                title.Mode = "Save";
                                title.ConfirmedSelectedIndex = -1;
                                title.SelectedIndex = 0;
                                break;
                            case 2:
                                title.Mode = "Load";
                                title.ConfirmedSelectedIndex = -1;
                                title.SelectedIndex = 0;
                                break;
                            case 3:
                                this.Exit();
                                break;
                        }
                    }
                    else if (title.Mode == "WriteSave")
                    {
                        SaveGame(title.newsavename);
                        title.Mode = "Menu";
                        title.ConfirmedSelectedIndex = -1;
                    }
                    else if (title.Mode == "Save")
                    {
                        title.newsavename = "";
                        title.Mode = "WriteSave";
                        title.ConfirmedSelectedIndex = -1;
                    }
                    else if (title.Mode == "Load")
                    {
                        LoadSave(title.Savefilenames[title.SelectedIndex + title.ScrollIndex]);
                        title.Mode = "Menu";
                        ShowTitle = false;
                        title.ConfirmedSelectedIndex = -1;
                        title.Startup = false;
                    }
                    else
                        title.ConfirmedSelectedIndex = -1;
                }
            }
            else // Regular Game Logic
            {
                player.Update(gui.MSGTextDisplayed());

                if (Editor.ShowPlayerPos)
                    Console.WriteLine("Player X: " + player.position.X.ToString() + " Y: " + player.position.Y.ToString());

                bool objectfound = false;
                foreach (Object obj in map.getObjects())
                {
                    if (obj.Update(player.playerRect, player.direction))
                    {
                        player.selectedObject = obj;
                        objectfound = true;
                    }
                }

                if (objectfound == false)
                    player.selectedObject = null;

                gui.Update();

                CheckControls();

                scripthandler.Update(gui, GameVariables, gameTime);
            }


            bool guiisdone = true;
            if (gui != null)
                guiisdone = gui.MSGTextDisplayed();
            sound.Update(guiisdone);

            base.Update(gameTime);

        }

        /// <summary>
        ///This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            
            GraphicsDevice.Clear(Color.CornflowerBlue);
            
            // TODO: Add your drawing code here
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone, null, SpriteScale);

            if (ShowTitle)
            {
                title.Draw(spriteBatch, GameStartup, font, GetGamePath("savegames\\"));
                if (GameStartup)
                {
                    InitializeContent();
                    LoadContentContent();
                    GameStartup = false;
                }
            }
            else
            {
                // Draw everything behind the player
                map.Draw(spriteBatch, graphics, new Vector2(player.position.X, player.position.Y + player.playerRect.Height), false, scripthandler.IsScriptRunning());
                player.Draw(spriteBatch, graphics);
                // And then everything in front of him
                map.Draw(spriteBatch, graphics, new Vector2(player.position.X, player.position.Y + player.playerRect.Height), true, scripthandler.IsScriptRunning());

                // Drawing UserInterface last
                player.verbmenu.Draw(spriteBatch);
                gui.Draw(spriteBatch, player.verbmenu.Shown, player.selectedObject, scripthandler.IsScriptRunning());
            }

            spriteBatch.End();

            base.Draw(gameTime);
        }

        private void CheckControls()
        {
            #region Keyboard Input
            KeyboardState KnewState = Keyboard.GetState();

            //KEYBOARD SECTION
            if (Keyboard.GetState().IsKeyUp(Keys.F3))
                EditorToggleHold = false;

            if (KnewState.IsKeyDown(Keys.F4) && KoldState.IsKeyUp(Keys.F4))
                player.clipping = !player.clipping;

            if (KnewState.IsKeyDown(Keys.F2) && KoldState.IsKeyUp(Keys.F2) && Debug)
                player.movelock = !player.movelock;

            // CHANGE THE FRIGGIN REOLUTION AT WILL MOFO
            if (KnewState.IsKeyDown(Keys.F5) && KoldState.IsKeyUp(Keys.F5))
            {
                if (graphics.PreferredBackBufferHeight == 480)
                {
                    graphics.PreferredBackBufferWidth = 1280;
                    graphics.PreferredBackBufferHeight = 720;
                }
                else if (graphics.PreferredBackBufferHeight == 720)
                {
                    graphics.PreferredBackBufferWidth = 1920;
                    graphics.PreferredBackBufferHeight = 1080;
                }
                else if (graphics.PreferredBackBufferHeight == 1080)
                {
                    graphics.PreferredBackBufferWidth = 800;
                    graphics.PreferredBackBufferHeight = 480;
                }

                graphics.ApplyChanges();

                float Screenscalex = graphics.GraphicsDevice.Viewport.Width / baseScreenSize.X;
                float Screenscaley = graphics.GraphicsDevice.Viewport.Height / baseScreenSize.Y;

                SpriteScale = Matrix.CreateScale(Screenscalex, Screenscaley, 1);
            }

            if (KnewState.IsKeyDown(Keys.M) && KoldState.IsKeyUp(Keys.M))
            {
                sound.mute = !sound.mute;
            }

            if (KnewState.IsKeyDown(Keys.F6) && KoldState.IsKeyUp(Keys.F6))
            {
                graphics.IsFullScreen = !graphics.IsFullScreen;
                graphics.ApplyChanges();
            }

            // Title-Screen keyboard checks
            if (ShowTitle)
            {
                if (KnewState.IsKeyDown(Keys.Up) && KoldState.IsKeyUp(Keys.Up))
                {
                    title.SelectUp();
                }
                if (KnewState.IsKeyDown(Keys.Down) && KoldState.IsKeyUp(Keys.Down))
                {
                    title.SelectDown();
                }
                if (KnewState.IsKeyDown(Keys.Enter) && KoldState.IsKeyUp(Keys.Enter))
                {
                    title.Confirm();
                }
                if (KnewState.IsKeyDown(Keys.Escape) && KoldState.IsKeyUp(Keys.Escape))
                {
                    title.Decline();
                }
                if (KnewState.IsKeyDown(Keys.F10) && KoldState.IsKeyUp(Keys.F10))
                {
                    graphics.ToggleFullScreen();
                    graphics.ApplyChanges();
                }
                if (title.Mode == "WriteSave")
                {
                    String input = textinput.getInput(KnewState, title.newsavename);
                    if (input != null)
                        title.newsavename = input;
                }
            }
            else if (!player.movelock)
            {
                // REGULAR GAME KEYBOARD CHECKS

                // When ESC - Show Title Screen
                if (KnewState.IsKeyDown(Keys.Escape) && KoldState.IsKeyUp(Keys.Escape))
                {
                    ShowTitle = true;
                    title.SelectedIndex = 0;

                    title.Show(scripthandler.IsScriptRunning(), sound.currentsong);
                    sound.PlayMusic(title.titlesong);
                }

                // Show Player position in the room for reasons
                if (KnewState.IsKeyDown(Keys.F2) && KoldState.IsKeyUp(Keys.F2) && Debug)
                {
                    Console.WriteLine("Player Postion: X: " + player.position.X + " Y: " + player.position.Y);
                }
                
                // Start up the Editor-Mode
                if (Keyboard.GetState().IsKeyDown(Keys.F3) && EditorToggleHold == false && Debug)
                {
                    if (EditorON == false)
                    {
                        Editor.Show();
                    }
                    else
                        Editor.Hide();

                    //toggle debug variables
                    EditorON = !EditorON;
                    this.IsMouseVisible = !this.IsMouseVisible;

                    Editor.UpdateEditor();
                    EditorToggleHold = true;
                }

                if (KnewState.IsKeyDown(Keys.Space) && KoldState.IsKeyUp(Keys.Space))
                {
                    if (!scripthandler.IsScriptRunning())
                        gui.ShowInventory = !gui.ShowInventory;
                    if (gui.InventorySelected >= gui.InvList.Count && gui.InvList.Count > 0)
                        gui.InventorySelected = gui.InvList.Count - 1;
                }

                if (gui.ShowInventory == false)
                {

                    if (player.verbmenu.Shown == false)
                    {
                        // Movement of the Player
                        if (scripthandler.IsScriptRunning() == false)
                        {
                            if (Keyboard.GetState().IsKeyDown(Keys.Left))
                            {
                                player.direction = 3;
                                if (player.CheckCollision(new Vector2(-2, 0), map.getWalkrects(), map.Objects))
                                    player.move(-2, 0);
                            }
                            if (Keyboard.GetState().IsKeyDown(Keys.Right))
                            {
                                player.direction = 1;
                                if (player.CheckCollision(new Vector2(2, 0), map.getWalkrects(), map.Objects))
                                    player.move(2, 0);
                            }
                            if (Keyboard.GetState().IsKeyDown(Keys.Up))
                            {
                                player.direction = 0;
                                if (player.CheckCollision(new Vector2(0, -2), map.getWalkrects(), map.Objects))
                                    player.move(0, -2);
                            }
                            if (Keyboard.GetState().IsKeyDown(Keys.Down))
                            {
                                player.direction = 2;
                                if (player.CheckCollision(new Vector2(0, 2), map.getWalkrects(), map.Objects))
                                    player.move(0, 2);
                            }
                        }
                        // Check for Objects to interact with and interact with them
                        if (KnewState.IsKeyDown(Keys.Enter) && KoldState.IsKeyUp(Keys.Enter))
                        {
                            if (!gui.CheckControls() && gui.ShowMSG == false && !scripthandler.IsScriptRunning())
                                player.CheckObject(graphics.PreferredBackBufferWidth);
                        }
                    }
                    else // Verbmenu is shown
                    {
                        // Close menu without doing anything
                        if (KnewState.IsKeyDown(Keys.RightShift) && KoldState.IsKeyUp(Keys.RightShift))
                        {
                            player.verbmenu.Hide();
                        }
                        // Navigate menu
                        if (KnewState.IsKeyDown(Keys.Up) && KoldState.IsKeyUp(Keys.Up))
                        {
                            player.verbmenu.goUp();
                        }
                        if (KnewState.IsKeyDown(Keys.Down) && KoldState.IsKeyUp(Keys.Down))
                        {
                            player.verbmenu.goDown(player.selectedObject.scripts);
                        }
                        if (KnewState.IsKeyDown(Keys.Enter) && KoldState.IsKeyUp(Keys.Enter))
                        {
                            player.verbmenu.Hide();
                            if (player.selectedObject.scripts.Count != 0)
                                scripthandler.RunScript(player.selectedObject.scripts[player.verbmenu.getSelectedItem()]);
                        }
                    }
                }
                else // Inventory is shown
                {
                    if (gui.InventoryActive)
                    {
                        if (KnewState.IsKeyDown(Keys.Right) && KoldState.IsKeyUp(Keys.Right))
                        {
                            if (player.InvList.Count - 1 > gui.InventorySelected)
                            {
                                gui.InventorySelected++;
                            }
                        }

                        if (KnewState.IsKeyDown(Keys.Left) && KoldState.IsKeyUp(Keys.Left))
                        {
                            if (gui.InventorySelected > 0)
                            {
                                gui.InventorySelected--;
                            }
                        }

                        if (KnewState.IsKeyDown(Keys.RightShift) && KoldState.IsKeyUp(Keys.RightShift))
                        {
                            if (player.InvList.Count > 0)
                                //Run the Look-at script
                                scripthandler.RunScript(player.InvList[gui.InventorySelected].scripts[0]);
                        }

                        if (KnewState.IsKeyDown(Keys.Enter) && KoldState.IsKeyUp(Keys.Enter))
                        {
                            if (gui.ShowMSG)
                            {
                                gui.CheckControls();
                            }
                            else if (player.verbmenu.Shown || player.selectedObject != null)
                            {
                                if (player.InvList.Count > 0)
                                {
                                    bool foundscript = false;

                                    //Check if the item has a script for this object. Otherwise run default script
                                    foreach (Script script in player.InvList[gui.InventorySelected].scripts)
                                    {
                                        Console.WriteLine(script.Name + "  " + player.selectedObject.name);
                                        if (script.Name == player.selectedObject.name)
                                        {
                                            player.verbmenu.Hide();
                                            gui.ShowInventory = false;
                                            scripthandler.RunScript(script);
                                            foundscript = true;
                                        }
                                    }

                                    if (!foundscript)
                                    {
                                        player.verbmenu.Hide();
                                        gui.ShowInventory = false;
                                        scripthandler.RunScript(player.InvList[gui.InventorySelected].scripts[1]);
                                    }
                                }
                            }
                        }
                    }
                }
            }

            KoldState = KnewState;
            #endregion

            #region GamePad Input
            // GAMEPAD INPUT
            thumbx = GamePad.GetState(PlayerIndex.One).ThumbSticks.Left.X;
            thumby = GamePad.GetState(PlayerIndex.One).ThumbSticks.Left.Y;
            if (thumbx >= 0.1 || thumbx <= -0.1)
            {
                //player.image.Position += new Vector2(thumbx, -thumby);
                Vector2 deltapos = new Vector2(thumbx * 2, 0);
                if (player.CheckCollision(deltapos, map.getWalkrects(), map.Objects))
                    player.move(deltapos.X, 0);
            }
            if (thumby <= -0.1 || thumby >= 0.1)
            {
                Vector2 deltapos = new Vector2(0, -thumby * 2);
                if (player.CheckCollision(deltapos, map.getWalkrects(), map.Objects))
                    player.move(0, deltapos.Y);
            }

            #endregion

            #region Mouse Input
            MouseState newState = Mouse.GetState();

            if (EditorON)
            {
                //ON LEFT BUTTON HOLD
                if (newState.LeftButton == ButtonState.Pressed && newState.X >= 0 && newState.Y >= 0 && newState.X <= graphics.PreferredBackBufferWidth && newState.Y <= graphics.PreferredBackBufferHeight)
                {
                    // Move Walkmap
                    if (Editor.ToolState == "MoveWalkmap" || Editor.ToolState == "MovingWalkmap")
                    {
                        if (map.getWalkrects().Count >= Editor.selectedWalkRect && Editor.selectedObject != -1)
                        {
                            if (Editor.ToolState == "MoveWalkmap")
                            {
                                E_ClickedOn = new Point(newState.X, newState.Y);
                                if (map.getWalkrects()[Editor.selectedWalkRect].Contains(E_ClickedOn))
                                    Editor.ToolState = "MovingWalkmap";
                            }
                            else if (Editor.ToolState == "MovingWalkmap")
                            {
                                // Move the Rect by the Delta
                                int deltaX = newState.X - E_ClickedOn.X;
                                int deltaY = newState.Y - E_ClickedOn.Y;
                                map.getWalkrects()[Editor.selectedWalkRect] = new Rectangle(map.getWalkrects()[Editor.selectedWalkRect].X + deltaX, map.getWalkrects()[Editor.selectedWalkRect].Y + deltaY, map.getWalkrects()[Editor.selectedWalkRect].Width, map.getWalkrects()[Editor.selectedWalkRect].Height);
                                E_ClickedOn = new Point(newState.X, newState.Y);
                            }
                        }
                    }

                    // Move Object
                    if (Editor.ToolState == "MoveObject" || Editor.ToolState == "MovingObject")
                    {
                        if (map.getObjects().Count >= Editor.selectedObject && Editor.selectedObject > -1)
                        {
                            if (Editor.ToolState == "MoveObject")
                            {
                                E_ClickedOn = new Point(newState.X, newState.Y);
                                if (map.getObjects()[Editor.selectedObject].getActualRect().Contains(E_ClickedOn))
                                    Editor.ToolState = "MovingObject";
                            }
                            else if (Editor.ToolState == "MovingObject")
                            {
                                // Move the Rect by the Delta
                                int deltaX = newState.X - E_ClickedOn.X;
                                int deltaY = newState.Y - E_ClickedOn.Y;
                                map.getObjects()[Editor.selectedObject].rect = new Rectangle(map.getObjects()[Editor.selectedObject].rect.X + deltaX, map.getObjects()[Editor.selectedObject].rect.Y + deltaY, map.getObjects()[Editor.selectedObject].rect.Width, map.getObjects()[Editor.selectedObject].rect.Height);
                                E_ClickedOn = new Point(newState.X, newState.Y);
                            }
                        }
                    }
                }

                //ON LEFT BUTTON RELEASE
                if (newState.LeftButton == ButtonState.Released && oldState.LeftButton == ButtonState.Pressed && newState.X >= 0 && newState.Y >= 0 && newState.X <= graphics.PreferredBackBufferWidth && newState.Y <= graphics.PreferredBackBufferHeight)
                {
                    if (Editor.ToolState == "MovingWalkmap")
                    {
                        Editor.ToolState = "MoveWalkmap";
                    }
                    if (Editor.ToolState == "MovingObject")
                    {
                        Editor.ToolState = "MoveObject";
                        Editor.UpdateEditor();
                    }
                }

                //SINGLE LEFT CLICK
                if (newState.LeftButton == ButtonState.Pressed && oldState.LeftButton == ButtonState.Released && newState.X >= 0 && newState.Y >= 0 && newState.X <= graphics.PreferredBackBufferWidth && newState.Y <= graphics.PreferredBackBufferHeight)
                {
                    // The "Create a Walkmap"-Tool
                    if (Editor.ToolState == "CreateWalkMap")
                    {
                        if (SelectWalkMap == false)
                        {
                            NewWalkMap = new Rectangle(newState.X, newState.Y, 0, 0);
                            SelectWalkMap = true;
                        }
                        else
                        {
                            NewWalkMap = new Rectangle(NewWalkMap.X, NewWalkMap.Y, newState.X - NewWalkMap.X, newState.Y - NewWalkMap.Y);
                            map.AddWalkRect(NewWalkMap);
                            Editor.UpdateEditor();
                            SelectWalkMap = false;
                        }
                    }
                }

                //SINGLE RIGHT CLICK
                if (newState.RightButton == ButtonState.Pressed && oldState.RightButton == ButtonState.Released && newState.X >= 0 && newState.Y >= 0 && newState.X <= graphics.PreferredBackBufferWidth && newState.Y <= graphics.PreferredBackBufferHeight)
                {
                    // Reset your Editortool
                    Editor.ToolState = "None";
                    SelectWalkMap = false;
                    Editor.UpdateEditor();
                }
            }

            oldState = newState;
            #endregion
        }

        public void AddGlobalVariable(String name, String value)
        {
            String[] newvariable = new String[2] {name, value};
            GameVariables.Add(newvariable);
        }

        public void PlayIntro()
        {
            Script introscript = map.PlayIntro();
            if (introscript != null)
            {
                Console.WriteLine("Running intro.");
                scripthandler.RunScript(introscript);
            }
        }

        public String GetGamePath(String Filename = null)
        {
            //Find the path where the game itself sits
            string path = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase);
            path = path.Substring(6, path.Length - 6);      // Getting rid of the file:\ it puts in front of the path with the above line

            string relPath = "";

            if (Filename != null)
            {
                relPath = System.IO.Path.Combine(path, Filename);
            }
            else
            {
                relPath = path;
            }

            return relPath;
        }

        public void SaveMap(String Filename)
        {
            //Get the Path to the directory from which the game is run
            String path = Filename.Substring(0, Filename.LastIndexOf('\\'));
            path = path.Substring(0, path.LastIndexOf("\\"));

            //Always reset this variable
            map.introplayed = false;

            Console.WriteLine("Saving.....");

            if (map.Save(path + "\\" + "dummyfile"))
            {
                map.Save(Filename);
                Console.WriteLine("Saved successfully to " + Filename);
            }
            else
                Console.WriteLine("The Map could not be saved!");
            
            //Save the Projectfile too

            //Empty the player inventory, otherwise the serialization wont work right because
            //there are two lists with items and that gets it confused or something
            List<Item> tempitemlist = new List<Item>();
            foreach (Item item in player.InvList)
            {
                tempitemlist.Add(item);
            }
            player.InvList.Clear();

            //Save the actual file
            proj.SetGlobalVariables(GameVariables);
            proj.SetItems(items.ReturnItemList());
            proj.Save(path + "\\" + proj.Name + ".prj");

            //then, restory the player inventory
            foreach (Item item in tempitemlist)
            {
                player.InvList.Add(item);
                player.InvList[player.InvList.Count-1].LoadContent(Content);
            }
        }

        public void LoadMap(String Filename, bool Startup = false)
        {
            map = (Maps)Maps.Load(Filename, typeof(Maps));

            if (map == null)
            {
                Console.WriteLine("Oh no, something went wrong with the map-file!");
                map = new Maps();
            }
            
            if (!Startup)
                map.LoadContent(this.Content);

            if (map.Objects.Count > 0)
            {
                foreach (Object obj in map.Objects)
                {
                    obj.LoadContent(this.Content);
                    
                    if (obj.opacity > 1f)
                        obj.opacity = 1f;

                    if (obj.scripts.Count > 0)
                    {
                        foreach (Script script in obj.scripts)
                        {
                            if (script.Commands.Count > 0)
                            {
                                foreach (Command com in script.Commands)
                                {
                                    com.gui = gui;
                                }
                            }
                        }
                    }
                }

                if (!Startup)
                {
                    // If the map has a background song, play it now!
                    if (map.backgroundmusic != null)
                    {
                        //sound.LoadMusic(map.backgroundmusic, Content);    //It loads all the music at the beginning of the game anyway!
                        sound.PlayMusic(map.backgroundmusic);
                    }
                    else
                        sound.StopMusic();
                }

            }

            //Make a new instance of the scripthandler with the freshly loaded map
            scripthandler = new ScriptHandler(map, player, items, this);

            if (!Startup)
            {
                //Also make a snapshot right awayy
                Snapshot newsnapshot = new Snapshot(GameVariables, map, map.Objects);

                Snapshots.Add(newsnapshot);
            }

            Console.WriteLine("MAP LOCKED AND LOADED, SIR!");
        }

        public void LoadProjectfile()
        {
            string path = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase);
            path = path.Substring(6, path.Length - 6);      // Getting rid of the file:\ it puts in front of the path with the above line
            string relPath = System.IO.Path.Combine(path,proj.Name + ".prj");
            proj = (Project)Project.Load(relPath, typeof(Project));

            if (proj != null)
            {
                GameVariables = proj.GetGlobalVariables();
                items.items = proj.GetItems();
            }
            else
            {
                proj = new Project(Projectname, GameVariables, items.ReturnItemList());

                Console.WriteLine("Could not load Projectfile!");
            }

            items.LoadContent(Content);
        }

        public void LoadSnapshot(Snapshot snapshot)
        {
            // Load Global Variables
            foreach (String[] var in GameVariables)
            {
                int i = 0;
                foreach (String name in snapshot.GVName)
                {
                    if (name == var[0])
                        var[1] = snapshot.GVValue[i];

                    i++;
                }
            }
            // Load Object data
            foreach (SnapObject snapj in snapshot.snapjects)
            {
                Object obj = map.FindObject(snapj.name);
                if (obj != null)
                {
                    obj.color = snapj.color;
                    obj.imagenum = snapj.imagenum;
                    obj.opacity = snapj.opacity;
                    obj.rect = snapj.rect;
                    obj.visible = snapj.visible;
                    obj.walkable = snapj.walkable;

                    for (int i = 0; i < snapj.VBName.Count; i++)
                    {
                        foreach (Script scr in obj.scripts)
                        {
                            if (scr.Name == snapj.VBName[i])
                            {
                                scr.Active = snapj.VBactive[i];
                            }
                        }
                    }
                }
                else
                    Console.WriteLine("SNAPSHOT LOAD ERROR: Object wasn't found!");
            }

            // Load the player inventory
            player.InvList.Clear();
            foreach (Item item in snapshot.snapventory)
                player.InvList.Add(item);
        }

        public void SaveGame(String Filename)
        {
            //Find the path to all the saves
            string path = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase);
            path = path.Substring(6, path.Length - 6);      // Getting rid of the file:\ it puts in front of the path with the above line
            string relPath = System.IO.Path.Combine(path + "\\savegames\\", Filename);

            save.PrepareSave(Filename, proj.GetGlobalVariables(), player.InvList, player.position, map, player.asciimode);

            if (save.Save(path + "\\" + "dummyfile"))
            {
                save.Save(relPath);
                Console.WriteLine("Saved the game under: " + Filename);
            }
            else
                Console.WriteLine("The Game could not be saved!");
        }

        // Loads the image number for all the objects in the new room from the previously saved (in the scripthandler in the TELEPORT command) Autosave file
        // Used when entering a previously entered room
        public void LoadObjStat()
        {
            //Find the path to all the saves
            string path = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase);
            path = path.Substring(6, path.Length - 6);      // Getting rid of the file:\ it puts in front of the path with the above line
            string relPath = System.IO.Path.Combine(path + "\\savegames\\", "Autosave");

            save = (Savegame)Savegame.Load(relPath, typeof(Savegame));

            if (save == null)
            {
                save = new Savegame(proj.GetGlobalVariables(), player.InvList, map);
                Console.WriteLine("OH OH! Autosave could not be loaded!");
            }

            //Load everything into all of the variables!
            foreach (MapSave mapy in save.MapSaves)
            {
                //THE MAP LAYER
                if (mapy.name == map.name)
                {
                    map.introplayed = mapy.introplayed;
                    //THE OBJECT LAYER
                    foreach (Object obj in map.getObjects())
                    {
                        foreach (ObjectSave objsv in mapy.objectsaves)
                        {
                            if (obj.name == objsv.name)
                            {
                                obj.imagenum = objsv.imagenum;
                                obj.visible = objsv.visible;
                                obj.walkable = objsv.walkable;

                                //THE SCRIPT LAYER
                                foreach (ScriptSave scrsv in objsv.scriptsaves)
                                {
                                    foreach (Script scr in obj.scripts)
                                    {
                                        if (scrsv.name == scr.Name)
                                        {
                                            scr.Active = scrsv.active;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        public void LoadSave(String Filename, bool Startup = false)
        {
            //Find the path to all the saves
            string path = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase);
            path = path.Substring(6, path.Length - 6);      // Getting rid of the file:\ it puts in front of the path with the above line
            string relPath = System.IO.Path.Combine(path + "\\savegames\\", Filename);

            save = (Savegame)Savegame.Load(relPath, typeof(Savegame));

            if (save == null)
            {
                save = new Savegame(proj.GetGlobalVariables(), player.InvList, map);
                Console.WriteLine("OH OH! Save could not be loaded!");
            }

            //Load all the stuff
            string relPath2 = System.IO.Path.Combine(path + "\\saves\\", save.CurrentRoom);
            LoadMap(relPath2, Startup);

            player.InvList.Clear();
            foreach (String itemname in save.Inventory)
            {
                Item coolitem = items.FindItem(itemname);
                if (coolitem != null)
                    player.InvList.Add(coolitem);
            }

            int counter = 0;
            foreach (String[] GV in GameVariables)
            {
                foreach (String VarName in save.GVName)
                {
                    if (VarName == GV[0])
                        GV[1] = save.GVValue[counter];
                }
                counter++;
            }

            // Load Player Position
            player.position = save.Playerpos;

            //Load everything into all of the variables!
            foreach (MapSave mapy in save.MapSaves)
            {
                //THE MAP LAYER
                if (mapy.name == map.name)
                {
                    map.introplayed = mapy.introplayed;
                    //THE OBJECT LAYER
                    foreach (Object obj in map.getObjects())
                    {
                        foreach (ObjectSave objsv in mapy.objectsaves)
                        {
                            if (obj.name == objsv.name)
                            {
                                obj.imagenum = objsv.imagenum;
                                obj.visible = objsv.visible;
                                obj.walkable = objsv.walkable;

                                //THE SCRIPT LAYER
                                foreach (ScriptSave scrsv in objsv.scriptsaves)
                                {
                                    foreach (Script scr in obj.scripts)
                                    {
                                        if (scrsv.name == scr.Name)
                                        {
                                            scr.Active = scrsv.active;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            player.toggleAscii(save.asciimode);
            gui.toggleAscii(save.asciimode);
            player.verbmenu.toggleAscii(save.asciimode);

            // If the map has a background song, play it now!
            if (map.backgroundmusic != null)
            {
                //sound.LoadMusic(map.backgroundmusic, Content);    //It loads all the music at the beginning of the game anyway!
                sound.PlayMusic(map.backgroundmusic);
            }
            else
                sound.StopMusic();

        }
    }
}
