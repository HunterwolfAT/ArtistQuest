using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace WindowsGame1
{
    public class Savegame : storagemanager
    {
        public String Name;
        public Vector2 Playerpos;
        public List<String> GVName;   // GameVariables Name
        public List<String> GVValue;  // GameVariables Value
        public List<String> Inventory;
        public String CurrentRoom;
        public List<MapSave> MapSaves;

        public Savegame()
        {
            Name = "";
            GVName = new List<String>();
            GVValue = new List<String>();
            Inventory = new List<String>();
            Playerpos = new Vector2(0,0);
            MapSaves = new List<MapSave>();
            CurrentRoom = "";
        }

        public Savegame(List<String[]> gamevariables, List<Item> inventory, Maps map)
        {
            Name = "";
            Playerpos = new Vector2(0, 0);
            CurrentRoom = map.name;

            Inventory = new List<String>();
            foreach (Item item in inventory)
            {
                Inventory.Add(item.Name);
            }

            MapSaves = new List<MapSave>();
            MapSaves.Add(new MapSave(map.name, map.introplayed, map.getObjects()));

            GVName = new List<String>();
            GVValue = new List<String>();
            
            foreach (String[] str in gamevariables)
            {
                GVName.Add(str[0]);
                GVValue.Add(str[1]);
            }
        }

        public void PrepareSave(String name, List<String[]> gamevariables, List<Item> inventory, Vector2 playerpos, Maps map)
        {
            Name = name;
            Playerpos = playerpos;
            CurrentRoom = map.name;

            Inventory.Clear();
            foreach (Item item in inventory)
            {
                Inventory.Add(item.Name);
            }

            bool alreadyexists = false;
            MapSave oldSave = null;

            // Check if the object is has already been saved
            foreach (MapSave save in MapSaves)
            {
                if (save.name == map.name)
                {
                    alreadyexists = true;
                    oldSave = save;
                }
            }

            // if it has, remove the old one:
            if (alreadyexists)
            {
                MapSaves.Remove(oldSave);
            }

            // Put current map in the list
            MapSaves.Add(new MapSave(map.name, map.introplayed, map.getObjects()));

            foreach (String[] str in gamevariables)
            {
                GVName.Add(str[0]);
                GVValue.Add(str[1]);
            }
        }
    }
}
