using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace WindowsGame1
{
    public class SnapObject
    {
        public String name;
        public int imagenum = -1;
        public Color color;
        public Rectangle rect;
        public Boolean walkable { get; set; }
        public Boolean visible { get; set; }
        public float opacity;

        //describes scripts/verbs active state
        public List<String> VBName;
        public List<Boolean> VBactive;

        public SnapObject()
        {
            VBName = new List<String>();
            VBactive = new List<Boolean>();
        }
    }
    
    public class Snapshot
    {
        public String Mapname;
        public DateTime Timestamp;

        public List<String> GVName;   // GameVariables Name
        public List<String> GVValue;  // GameVariables Value

        public List<SnapObject> snapjects;

        public List<Item> snapventory;

        public Snapshot(List<String[]> gamevariables, Maps map, List<Object> objects, List<Item> Inventory)
        {
            Mapname = map.name;

            // Global Variables
            GVName = new List<string>();
            GVValue = new List<string>();

            foreach (String[] var in gamevariables)
            {
                GVName.Add(var[0]);
                GVValue.Add(var[1]);
            }

            // Objects
            snapjects = new List<SnapObject>();

            foreach (Object obj in objects)
            {
                SnapObject snapj = new SnapObject();
                snapj.name = obj.name;
                snapj.imagenum = obj.imagenum;
                snapj.color = obj.color;
                snapj.rect = obj.rect;
                snapj.walkable = obj.walkable;
                snapj.visible = obj.visible;
                snapj.opacity = obj.opacity;

                // Verbs
                foreach (Script script in obj.scripts)
                {
                    snapj.VBName.Add(script.Name);
                    snapj.VBactive.Add(script.Active);
                }

                snapjects.Add(snapj);
            }

            // Inventory
            snapventory = new List<Item>();

            foreach (Item item in Inventory)
                snapventory.Add(item);
           
            Timestamp = DateTime.Now;   
        }

        public Snapshot(List<String[]> gamevariables, Maps map, List<Object> objects)
        {
            Mapname = map.name;

            // Global Variables
            GVName = new List<string>();
            GVValue = new List<string>();

            foreach (String[] var in gamevariables)
            {
                GVName.Add(var[0]);
                GVValue.Add(var[1]);
            }

            // Objects
            snapjects = new List<SnapObject>();

            foreach (Object obj in objects)
            {
                SnapObject snapj = new SnapObject();
                snapj.name = obj.name;
                snapj.imagenum = obj.imagenum;
                snapj.color = obj.color;
                snapj.rect = obj.rect;
                snapj.walkable = obj.walkable;
                snapj.visible = obj.visible;
                snapj.opacity = obj.opacity;

                // Verbs
                foreach (Script script in obj.scripts)
                {
                    snapj.VBName.Add(script.Name);
                    snapj.VBactive.Add(script.Active);
                }

                snapjects.Add(snapj);
            }

            // Inventory
            snapventory = new List<Item>();

            Timestamp = DateTime.Now;
        }
    }
}
