using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WindowsGame1
{
    public class Project : storagemanager
    {
        public String Name;
        public List<String[]> GameVariables;
        public List<Item> Items;

        public Project()
        {
            Name = "";
            GameVariables = new List<String[]>();
            Items = new List<Item>();
        }
        
        public Project(String name, List<String[]> globalvariables, List<Item> items)
        {
            Name = name;
            GameVariables = globalvariables;
            Items = items;
        }

        public void SetGlobalVariables(List<String[]> globalvariables)
        {
            this.GameVariables = globalvariables;
        }

        public List<String[]> GetGlobalVariables()
        {
            return GameVariables;
        }

        public void SetItems(List<Item> items)
        {
            Items = items;
        }

        public List<Item> GetItems()
        {
            return Items;
        }


    }
}
