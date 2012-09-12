using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WindowsGame1
{
    public class ObjectSave
    {
        public String name;
        public int imagenum;
        public Boolean visible;
        public Boolean walkable;
        public List<ScriptSave> scriptsaves;

        public ObjectSave()
        {
            name = "";
            imagenum = 0;
            visible = false;
            walkable = false;
            scriptsaves = new List<ScriptSave>();
        }

        public ObjectSave(String Name, int Imagenum, Boolean Visible, Boolean Walkable, List<Script> scripts)
        {
            name = Name;
            imagenum = Imagenum;
            visible = Visible;
            walkable = Walkable;

            scriptsaves = new List<ScriptSave>();

            foreach(Script scri in scripts)
            {
               scriptsaves.Add(new ScriptSave(scri.Name, scri.Active));
            }

        }
    }
}
