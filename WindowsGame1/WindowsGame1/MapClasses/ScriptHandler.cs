using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace WindowsGame1
{
    class ScriptHandler
    {
        public Script activescript;
        Boolean ScriptRunning = false;

        public int Commandcounter = -1;
        public int oldCommandcounter = -1;

        public double WaitCounter = 0;
        public double LastTimeCheck = 0;

        List<bool> IFstack;

        Maps map;
        Player player;
        Itemhandler items;
        Game1 game;

        public ScriptHandler(Maps newmap, Player newplayer, Itemhandler newitems, Game1 newgame = null)
        {
            activescript = new Script("null");
            map = newmap;
            player = newplayer;
            items = newitems;
            game = newgame;

            IFstack = new List<bool>();
        }

        public Boolean IsScriptRunning()
        {
            return ScriptRunning;
        }

        public void RunScript(Script newscript)
        {
            if (!ScriptRunning)
            {
                Console.WriteLine("RUNNING SCRIPT NOW");
                ScriptRunning = true;
                activescript = newscript;
                IFstack.Clear();
            }
        }

        public void Update(GUI gui, List<String[]> gamevariables, GameTime gametime)
        {
            if (ScriptRunning)
            {
                Boolean RunNextCommand = true;

                //Console.WriteLine(Commandcounter.ToString() + " Count: " + activescript.Commands.Count.ToString());

                if ((Commandcounter + 1) < activescript.Commands.Count)
                {
                    Commandcounter++;
                    //Console.WriteLine(Commandcounter.ToString() + " Count: " + activescript.Commands.Count.ToString());

                    if (Commandcounter != 0 && activescript.Commands[Commandcounter - 1].Type == "Message")
                    {
                        if (gui.IsDone())
                        {
                            //Dont do anything
                        }
                        else
                        {
                            RunNextCommand = false;
                            Commandcounter--;
                        }
                    }
                    else if (Commandcounter != 0 && activescript.Commands[Commandcounter - 1].Type == "Add Item" || Commandcounter != 0 && activescript.Commands[Commandcounter - 1].Type == "Remove Item")
                    {
                        if (gui.ShowItemBox)
                        {
                            RunNextCommand = false;
                            Commandcounter--;
                        }
                    }
                    else if (Commandcounter != 0 && activescript.Commands[Commandcounter - 1].Type == "Screenfade")
                    {
                        if (gui.Fading)
                        {
                            RunNextCommand = false;
                            Commandcounter--;
                        }
                        
                    }
                    else if (Commandcounter != 0 && activescript.Commands[Commandcounter - 1].Type == "Wait")
                    {
                        WaitCounter = WaitCounter + gametime.ElapsedGameTime.TotalMilliseconds;
                        double WaitCounterSeconds = WaitCounter / 1000;

                        //Console.WriteLine(WaitCounterSeconds.ToString());

                        if (activescript.Commands[Commandcounter - 1].IArgs[0] > WaitCounterSeconds)
                        {
                            RunNextCommand = false;
                            Commandcounter--;
                        }
                    }
                    else if (Commandcounter != 0 && activescript.Commands[Commandcounter - 1].Type == "Move Object" && activescript.Commands[Commandcounter - 1].SArgs[1] == "wait")
                    {
                        if (activescript.Commands[Commandcounter - 1].SArgs[0] == "*PLAYER")
                        {
                            if (game.player.moving)
                            {
                                RunNextCommand = false;
                                Commandcounter--;
                            }
                        }
                        else
                        {
                            if (game.map.FindObject(activescript.Commands[Commandcounter - 1].SArgs[0]).moving)
                            {
                                RunNextCommand = false;
                                Commandcounter--;
                            }
                        }
                    }
                    else if (Commandcounter != 0 && activescript.Commands[Commandcounter - 1].Type == "Fade Object" && activescript.Commands[Commandcounter - 1].SArgs[2] == "wait")
                    {
                        if (game.map.FindObject(activescript.Commands[Commandcounter - 1].SArgs[0]).fading)
                        {
                            RunNextCommand = false;
                            Commandcounter--;
                        }
                    }
                    else if (Commandcounter != 0 && activescript.Commands[Commandcounter - 1].Type == "Wait for Enter")
                    {
                        if (game.KoldState.IsKeyUp(Keys.Enter))
                        {
                            RunNextCommand = false;
                            Commandcounter--;
                        }
                    }

                }
                else    // were on the last command in the script now
                {
                    Boolean endscript = true;

                    //if this last command is a message, dont end it until the message has been clicked away!
                    if (activescript.Commands.Count != 0)
                    {
                        if (activescript.Commands[Commandcounter].Type == "Message")
                        {
                            if (gui.IsDone())
                            {
                                endscript = true;
                            }
                            else
                            {
                                endscript = false;
                            }
                        }
                    }

                    //then, end it
                    if (endscript)
                    {
                        Console.WriteLine("SCRIPT IS DONE!");
                        ScriptRunning = false;
                        Commandcounter = -1;
                        oldCommandcounter = -1;

                        IFstack.Clear();
                    }
                }

                if (oldCommandcounter != Commandcounter && RunNextCommand)
                {
                    //activescript.Commands[Commandcounter].Run();
                    Console.WriteLine("EXECUTING COMMAND NR. " + Commandcounter.ToString() + ": " + activescript.Commands[Commandcounter].Type);
                    
                    # region Message
                    if (activescript.Commands[Commandcounter].Type == "Message")
                    {
                        if (activescript.Commands[Commandcounter].SArgs.Count != 0)
                        {
                            Boolean firstMSG = false;

                            if (Commandcounter > 0)
                            {
                                if (activescript.Commands[Commandcounter - 1].Type != "Message")
                                    firstMSG = true;
                            }
                            else
                                firstMSG = true;

                            Boolean downbox = true;
                            if (game.player.position.Y > 290)
                                downbox = false;

                            gui.DisplayMSG(activescript.Commands[Commandcounter].SArgs[0], true, "LOL", downbox, firstMSG);
                        }
                        else
                            Console.WriteLine("No Message was defined!");
                    }
                    #endregion
                    #region ChangeSprite
                    else if (activescript.Commands[Commandcounter].Type == "ChangeSprite")
                    {
                        Object ActiveObject = map.FindObject(activescript.Commands[Commandcounter].SArgs[0]);
                        ActiveObject.imagenum = activescript.Commands[Commandcounter].IArgs[0];
                    }
                    #endregion
                    #region Change GlobalVariable
                    else if (activescript.Commands[Commandcounter].Type == "Change GlobalVariable")
                    {
                        Boolean VariableExists = false;

                        foreach (String[] variable in gamevariables)
                        {
                            if (variable[0] == activescript.Commands[Commandcounter].SArgs[0])
                            {
                                variable[1] = activescript.Commands[Commandcounter].SArgs[1];
                                VariableExists = true;
                            }
                        }

                        if (!VariableExists)
                        {
                            String[] newvariable = new String[2] { activescript.Commands[Commandcounter].SArgs[0], activescript.Commands[Commandcounter].SArgs[1] };
                            gamevariables.Add(newvariable);
                        }
                    }
                    #endregion
                    #region Change Visible
                    else if (activescript.Commands[Commandcounter].Type == "Change Visible")
                    {
                        //fetch the object this command wants to manipulate
                        Object ActiveObject = map.FindObject(activescript.Commands[Commandcounter].SArgs[0]);

                        if (ActiveObject != null)
                        {
                            if (activescript.Commands[Commandcounter].SArgs[1] == "ON")
                                ActiveObject.visible = true;
                            else if (activescript.Commands[Commandcounter].SArgs[1] == "OFF")
                                ActiveObject.visible = false;
                        }
                        else
                            Console.WriteLine("SCRIPT ERROR: Could not find Object");
                    }
                    else if (activescript.Commands[Commandcounter].Type == "Change Walkable")
                    {
                        //fetch the object this command wants to manipulate
                        Object ActiveObject = map.FindObject(activescript.Commands[Commandcounter].SArgs[0]);

                        if (ActiveObject != null)
                        {
                            if (activescript.Commands[Commandcounter].SArgs[1] == "ON")
                                ActiveObject.walkable = true;
                            else if (activescript.Commands[Commandcounter].SArgs[1] == "OFF")
                                ActiveObject.walkable = false;
                        }
                        else
                            Console.WriteLine("SCRIPT ERROR: Could not find Object");
                    }
                    #endregion
                    #region IF-Statement
                    else if (activescript.Commands[Commandcounter].Type == "IF")
                    {
                        Boolean StatementIsTrue = false;

                        Console.WriteLine(activescript.Commands[Commandcounter].SArgs[0]);

                        //If you compare two variables...
                        if (activescript.Commands[Commandcounter].SArgs[0] == "Variablecheck")
                        {
                            // First and second variable that will be compared
                            String Value1 = "";
                            String Value2 = "";

                            // EQUALS | NEQUALS | BIGGER | SMALLER
                            String Operator = activescript.Commands[Commandcounter].SArgs[2];

                            foreach (String[] var in gamevariables)
                            {
                                if (var[0] == activescript.Commands[Commandcounter].SArgs[1])
                                    Value1 = var[1];
                                if (var[0] == activescript.Commands[Commandcounter].SArgs[3])
                                    Value2 = var[1];
                            }

                            //Check if the first value isn't a global variable, but a property
                            if (activescript.Commands[Commandcounter].SArgs[1] == "PlayerXPos")
                                Value1 = game.player.position.X.ToString();
                            if (activescript.Commands[Commandcounter].SArgs[1] == "PlayerYPos")
                                Value1 = game.player.position.Y.ToString();

                            if (activescript.Commands[Commandcounter].SArgs[3][0] == '\\')
                                Value2 = activescript.Commands[Commandcounter].SArgs[3].Remove(0, 1);    //Removes the first '\' char

                            //If one of the values can not be found in the GlobalVariables list, the statement will be FALSE
                            if (Value1 != "" || Value2 != "")
                            {
                                if (Operator == "EQUAL")
                                {
                                    if (Value1 == Value2)
                                        StatementIsTrue = true;
                                    else
                                        StatementIsTrue = false;
                                }
                                else if (Operator == "NEQUAL")
                                {
                                    if (Value1 != Value2)
                                        StatementIsTrue = true;
                                    else
                                        StatementIsTrue = false;
                                }
                                else if (Operator == "BIGGER")
                                {
                                    if (int.Parse(Value1) > int.Parse(Value2))
                                        StatementIsTrue = true;
                                    else
                                        StatementIsTrue = false;
                                }
                                else if (Operator == "SMALLER")
                                {
                                    if (int.Parse(Value1) < int.Parse(Value2))
                                        StatementIsTrue = true;
                                    else
                                        StatementIsTrue = false;
                                }
                            }
                        }
                        // ...or if you need to check items
                        else if (activescript.Commands[Commandcounter].SArgs[0] == "Itemcheck")
                        {
                            foreach (Item item in player.InvList)
                            {
                                if (item.Name == activescript.Commands[Commandcounter].SArgs[1])
                                    StatementIsTrue = true;
                            }

                            if (StatementIsTrue == false && activescript.Commands[Commandcounter].SArgs[2] == "is not in inventory")
                                StatementIsTrue = true;

                            else if (StatementIsTrue == true && activescript.Commands[Commandcounter].SArgs[2] == "is not in inventory")
                                StatementIsTrue = false;
                        }
                        else
                        {
                            Console.WriteLine("IF ERROR: I dunno what Im supposed to compare!");
                        }

                        // Doing what has to be done
                        if (StatementIsTrue)
                        {
                            //If the IF Statement is true, dont do anything and just move on!
                        }
                        else
                        {
                            //if its NOT true, then move to the END IF line

                            int ifcounter = 0;
                            for (int a = Commandcounter + 1; a < activescript.Commands.Count(); a++)
                            {
                                Console.WriteLine(a + " " + activescript.Commands[a].Type);
                                if (activescript.Commands[a].Type == "IF")
                                    ifcounter++;

                                if (activescript.Commands[a].Type == "END IF" || activescript.Commands[a].Type == "ELSE")
                                {
                                    if (ifcounter > 0 && activescript.Commands[a].Type == "END IF")
                                        ifcounter--;

                                    if (ifcounter == 0)
                                    {
                                        Console.WriteLine("SETTING SCRIPT TO LINE: " + a);
                                        //Set the Commandcounter to the new line
                                        Commandcounter = a - 1;
                                        if (activescript.Commands[a].Type == "ELSE")
                                            Commandcounter = a;
                                        break;
                                    }
                                }
                            }
                        }

                        //Adding the result to the if-stack
                        IFstack.Add(StatementIsTrue);
                        Console.WriteLine("IF - IFstack at the end: " + IFstack[IFstack.Count - 1].ToString());

                    }
                    #endregion
                    #region ELSE-Statement
                    else if (activescript.Commands[Commandcounter].Type == "ELSE")
                    {
                        //First, go back to find the corrosponding IF condition, and see if it was true or not

                        //Find the IF-result from the top of the IF-Stack:
                        Boolean StatementIsTrue = false;
                        
                        if (IFstack.Count > 0)
                            StatementIsTrue = IFstack[IFstack.Count - 1];

                        Console.WriteLine("ELSE - IFstack: " + IFstack[IFstack.Count - 1].ToString() + " is " + StatementIsTrue.ToString());


                        if (StatementIsTrue == false)
                        {
                            //Dont do anything.
                        }
                        if (StatementIsTrue == true)
                        {
                            //Jump to END IF
                            int ifcounter = 0;
                            for (int a = Commandcounter; a < activescript.Commands.Count(); a++)
                            {
                                Console.WriteLine(a + " " + activescript.Commands[a].Type);
                                if (activescript.Commands[a].Type == "IF")
                                    ifcounter++;

                                if (activescript.Commands[a].Type == "END IF")
                                {
                                    if (ifcounter > 0)
                                        ifcounter--;

                                    if (ifcounter == 0)
                                    {
                                        Console.WriteLine("SETTING SCRIPT TO LINE: " + (a - 1));
                                        //Set the Commandcounter to the new line
                                        Commandcounter = a - 1;
                                        break;
                                    }
                                }
                            }
                        }


                    }
                    #endregion
                    #region END IF-Statement
                    else if (activescript.Commands[Commandcounter].Type == "END IF")
                    {
                        //Dont. Do. Anything.
                        //Except poping of the latest result from the if-stack
                        if (IFstack.Count > 0)
                            IFstack.RemoveAt(IFstack.Count - 1);
                        else
                            Console.WriteLine("END IF ERROR: The IF-Stack was already empty! Was there even a IF before it?");
                    }
                    #endregion
                    #region Add Item
                    else if (activescript.Commands[Commandcounter].Type == "Add Item")
                    {
                        Item ActiveItem = items.FindItem(activescript.Commands[Commandcounter].SArgs[0]);
                        if (ActiveItem != null)
                        {
                            gui.DisplayNewItem(ActiveItem, true);
                            player.InvList.Add(ActiveItem);
                        }
                        else
                            Console.WriteLine("SCRIPT ERROR: Can't find Item!");
                    }
                    #endregion
                    #region Remove Item
                    else if (activescript.Commands[Commandcounter].Type == "Remove Item")
                    {
                        Item ActiveItem = items.FindItem(activescript.Commands[Commandcounter].SArgs[0]);
                        if (ActiveItem != null)
                        {
                            gui.DisplayNewItem(ActiveItem, false);
                            player.InvList.Remove(ActiveItem);
                        }
                        else
                            Console.WriteLine("SCRIPT ERROR: Can't find Item!");
                    }
                    #endregion
                    #region Change Verb
                    else if (activescript.Commands[Commandcounter].Type == "Change verb")
                    {
                        Object targetobject = map.FindObject(activescript.Commands[Commandcounter].SArgs[0]);

                        if (targetobject != null)
                        {

                            Script targetscript = targetobject.FindScript(activescript.Commands[Commandcounter].SArgs[1]);

                            if (targetobject != null)
                            {
                                if (activescript.Commands[Commandcounter].SArgs[2] == "ON")
                                    targetscript.Active = true;
                                else if (activescript.Commands[Commandcounter].SArgs[2] == "OFF")
                                    targetscript.Active = false;
                                else
                                    Console.WriteLine("Change Verb ERROR: Something weird is going on with the ON/OFF parameter!");
                            }
                            else
                            {
                                Console.WriteLine("Change Verb ERROR: Couldn't find the verb!");
                            }
                        }
                        else
                            Console.WriteLine("Change Verb ERROR: Couldn't find the object!");
                    }
                    #endregion
                    #region END SCRIPT
                    else if (activescript.Commands[Commandcounter].Type == "END SCRIPT")
                    {
                        Commandcounter = activescript.Commands.Count();
                    }
                    #endregion
                    #region Screenfade
                    else if (activescript.Commands[Commandcounter].Type == "Screenfade")
                    {
                        if (activescript.Commands[Commandcounter].SArgs[0] == "OUT")
                            gui.FadeOut();
                        if (activescript.Commands[Commandcounter].SArgs[0] == "IN")
                            gui.FadeIn();
                    }
                    #endregion
                    #region Teleport
                    else if (activescript.Commands[Commandcounter].Type == "Teleport")
                    {
                        //Find the path to all the maps
                        string path = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase);
                        path = path.Substring(6, path.Length - 6);      // Getting rid of the file:\ it puts in front of the path with the above line
                        string relPath = System.IO.Path.Combine(path + "\\saves\\", activescript.Commands[Commandcounter].SArgs[0]);

                        game.SaveGame("Autosave");
                        game.LoadMap(relPath);
                        game.LoadObjStat();

                        game.PlayIntro();
                       

                        player.position.X = activescript.Commands[Commandcounter].IArgs[0];
                        player.position.Y = activescript.Commands[Commandcounter].IArgs[1];
                    }
                    #endregion
                    #region Play Animation
                    else if (activescript.Commands[Commandcounter].Type == "Play Animation")
                    {
                        Object SelectedObject = null;

                        //Find the right object
                        foreach (Object obj in game.map.getObjects())
                        {
                            if (obj.name == activescript.Commands[Commandcounter].SArgs[0])
                            {
                                SelectedObject = obj;
                            }
                        }

                        if (SelectedObject != null)
                        {
                            AnimatedSprite SelectedSprite = null;
                            int SpriteCounter = 0;
                            int SpriteNumber = 0;

                            //Find the right animation
                            foreach (AnimatedSprite sprite in SelectedObject.aniimages)
                            {
                                if (sprite.name == activescript.Commands[Commandcounter].SArgs[1])
                                {
                                    SelectedSprite = sprite;
                                    SpriteNumber = SpriteCounter;
                                }
                                SpriteCounter++;
                            }

                            if (SelectedSprite != null)
                            {

                                //Play that shit! Or stop it! Depends.
                                if (activescript.Commands[Commandcounter].SArgs[2] == "once")
                                {
                                    SelectedObject.aniimagenum = SpriteNumber;
                                    SelectedObject.keepanimating = false;
                                }
                                if (activescript.Commands[Commandcounter].SArgs[2] == "start")
                                {
                                    SelectedObject.aniimagenum = SpriteNumber;
                                    SelectedObject.keepanimating = true;
                                }
                                if (activescript.Commands[Commandcounter].SArgs[2] == "stop")
                                {
                                    SelectedObject.aniimagenum = -1;
                                    SelectedObject.keepanimating = false;
                                }
                            }
                            else
                                Console.WriteLine("Animation Command ERROR: Couldn't find the animation in the object!");
                        }
                        else
                            Console.WriteLine("Animation Command ERROR: Couldn't find the object!");
                    }
                    #endregion
                    #region Wait
                    else if (activescript.Commands[Commandcounter].Type == "Wait")
                    {
                        WaitCounter = 0;
                        Console.WriteLine("OJHÖLDKJFHLKSDJFHLSKDJHFSLDKJ");
                    }
                    #endregion
                    #region Turn Player Invisible
                    else if (activescript.Commands[Commandcounter].Type == "Turn Player Invisible")
                    {
                        game.player.visible = false;
                    }
                    #endregion
                    #region Turn Player Visible
                    else if (activescript.Commands[Commandcounter].Type == "Turn Player Visible")
                    {
                        game.player.visible = true;
                    }
                    #endregion
                    #region Move Object
                    else if (activescript.Commands[Commandcounter].Type == "Move Object")
                    {
                        int x = activescript.Commands[Commandcounter].IArgs[0];
                        int y = activescript.Commands[Commandcounter].IArgs[1];
                        int speed = activescript.Commands[Commandcounter].IArgs[2];

                        if (activescript.Commands[Commandcounter].SArgs[0] == "*PLAYER")
                        {
                            bool animate;
                            if (activescript.Commands[Commandcounter].SArgs[2] == "with animation")
                                animate = true;
                            else
                                animate = false;

                            player.move(x, y, speed, animate);
                        }
                        else
                        {
                            game.map.FindObject(activescript.Commands[Commandcounter].SArgs[0]).move(x, y, speed);
                        }
                    }
                    #endregion
                    #region Fade Object
                    else if (activescript.Commands[Commandcounter].Type == "Fade Object")
                    {
                        int speed = activescript.Commands[Commandcounter].IArgs[0];

                        if (activescript.Commands[Commandcounter].SArgs[1] == "in")
                            game.map.FindObject(activescript.Commands[Commandcounter].SArgs[0]).fadein(speed);
                        if (activescript.Commands[Commandcounter].SArgs[1] == "out")
                            game.map.FindObject(activescript.Commands[Commandcounter].SArgs[0]).fadeout(speed);
                    }
                    #endregion
                    else
                        Console.WriteLine("Couldn't get that command right!");
                }

                oldCommandcounter = Commandcounter;
            }
        }

    }
}
