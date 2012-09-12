using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace WindowsGame1
{
    public class Command
    {
        public String Type;
        public List<String> SArgs;
        public List<int> IArgs;

        public GUI gui;

        public Command()
        {
            Type = "";
            SArgs = new List<String>();
            IArgs = new List<int>();

            //gui = null;
        }
        
        public Command(String type, List<String> stringargs=null, List<int> intargs=null, GUI newgui = null)
        {
            Type = type;
            if (stringargs == null)
                SArgs = new List<String>();
            else
                SArgs = stringargs;

            if (intargs == null)
                IArgs = new List<int>();
            else
                IArgs = intargs;

            gui = newgui;
        }

        public List<String> GetSArgs()
        {
            return SArgs;
        }

        public void Run()
        {
            //Console.WriteLine("WERE DOING THIS");
            if (Type == "Message")
            {
                if (SArgs.Count != 0)
                    gui.DisplayMSG(SArgs[0],true);
                else
                    Console.WriteLine("No Message was defined!");
            }
            else
                Console.WriteLine("Couldn't get that command right!");

        }
    }
}
