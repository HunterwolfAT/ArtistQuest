using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WindowsGame1
{
    public class Script
    {
        public String Name;
        public List<Command> Commands;

        public Boolean ScriptRunning;
        public int Commandcounter;
        public int oldCommandcounter;

        //Only relevant for verbs of objects
        public Boolean Active;

        public Script()
        {
            Name = "Look at";
            Commands = new List<Command>();
            ScriptRunning = false;
            Commandcounter = -1;
            oldCommandcounter = -1;

            Active = true;
        }
        
        public Script(String name)
        {
            Name = name;
            Commands = new List<Command>();
            ScriptRunning = false;
            Commandcounter = -1;
            oldCommandcounter = -1;

            Active = true;
        }

        public void RunScript()
        {
            //foreach (Command com in Commands)
            //    com.Run();
            if (!ScriptRunning)
                ScriptRunning = true;
        }

        public void AddCommand(String type, List<String> sargs, List<int> iargs, GUI gui = null)
        {
            Command newcom = new Command(type, sargs, iargs, gui);
            Commands.Add(newcom);
        }

        public void InsertCommand(int index, String type, List<String> sargs, List<int> iargs, GUI gui = null)
        {
            Command newcom = new Command(type, sargs, iargs, gui);
            Commands.Insert(index, newcom);
        }

        public List<Command> GetCommands()
        {
            return Commands;
        }

        public void Update()
        {
            if (ScriptRunning)
            {
                if ((Commandcounter + 1) < Commands.Count)
                {
                    if (Commands[Commandcounter + 1].Type == "Message")
                    {
                        if (Commands[Commandcounter + 1].gui.ShowMSG == false)
                            Commandcounter++;
                    }
                }
                else
                {
                    ScriptRunning = false;
                    Commandcounter = -1;
                    oldCommandcounter = -1;
                }

                if (oldCommandcounter != Commandcounter)
                    Commands[Commandcounter].Run();

                oldCommandcounter = Commandcounter;
            }
        }
    }
}
